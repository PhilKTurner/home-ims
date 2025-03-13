using System.Data.Common;
using System.Security.Claims;
using System.Text.Json;
using HomeIMS.Server.CommandHandling;
using HomeIMS.Server.Database;
using HomeIMS.Server.EventStore;
using HomeIMS.Server.EventStore.Aggregators;
using HomeIMS.Server.Identity;
using HomeIMS.SharedContracts.Commands;
using HomeIMS.SharedContracts.Domain.Articles;
using HomeIMS.SharedContracts.Domain.Articles.Commands;
using HomeIMS.SharedContracts.EventSourcing;
using Marten;
using Marten.Events.Projections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeIMS.Server;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var dbServerVersion = new MariaDbServerVersion(new Version(11, 7, 2));

        var eventStoreConnectionStringBuilder = new DbConnectionStringBuilder();
        eventStoreConnectionStringBuilder.ConnectionString = builder.Configuration.GetConnectionString("HimsEventStore");

        var identityConnectionStringBuilder = new DbConnectionStringBuilder();
        identityConnectionStringBuilder.ConnectionString = builder.Configuration.GetConnectionString("HimsIdentityDatabase");

        if (Directory.Exists("/run/secrets"))
        {
            builder.Configuration.AddKeyPerFile("/run/secrets");
            eventStoreConnectionStringBuilder.Add("Password", builder.Configuration["hims-eventstore-rootpw"] ?? string.Empty);
            identityConnectionStringBuilder.Add("Password", builder.Configuration["hims-db-userpw"] ?? string.Empty);
        }
        else if (builder.Environment.IsDevelopment())
        {
            identityConnectionStringBuilder.Add("Password", builder.Configuration["HimsIdentityDatabase:Password"] ?? string.Empty);
        }
        else
        {
            throw new Exception("Password for database access not found.");
        }

        // Add services to the container.
        builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
                        .AddIdentityCookies();

        // Configure app cookie
        //
        // The default values, which are appropriate for hosting the Backend and
        // BlazorWasmAuth apps on the same domain, are Lax and SameAsRequest. 
        // For more information on these settings, see:
        // https://learn.microsoft.com/aspnet/core/blazor/security/webassembly/standalone-with-identity#cross-domain-hosting-same-site-configuration
        /*
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        });
        */

        builder.Services.AddAuthorizationBuilder();

        builder.Services.AddDbContext<IdentityContext>(
            dbContextOptions => dbContextOptions
                .UseMySql(identityConnectionStringBuilder.ConnectionString, dbServerVersion, mySqlOptions => mySqlOptions.EnableRetryOnFailure())
                // The following three options help with debugging, but should
                // be changed or removed for production.
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
        );
        
        builder.Services.AddIdentityCore<HimsUser>()
                        .AddRoles<IdentityRole>()
                        .AddEntityFrameworkStores<IdentityContext>()
                        .AddApiEndpoints();

        builder.Services.AddCors(
            options => options.AddPolicy(
            "wasm",
            policy => policy.WithOrigins([builder.Configuration["BackendUrl"] ?? "https://localhost:5001", 
                builder.Configuration["FrontendUrl"] ?? "https://localhost:5002"])
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            )
        );

        builder.Services.AddMarten(options =>
        {
            options.Connection(eventStoreConnectionStringBuilder.ConnectionString);
            options.UseSystemTextJsonForSerialization();

            options.Projections.Snapshot<ArticleAggregator>(SnapshotLifecycle.Inline, projectionOptions => projectionOptions.ProjectionName = "Articles");
        });

        builder.Services.AddScoped<IEventStore, MartenEventStore>();
        builder.Services.AddScoped<IReadModelAccess<ArticleAggregator>, SimpleMartenReadModelAccessor<ArticleAggregator>>();

        builder.Services.AddScoped<CommandRouter>();
        builder.Services.AddScoped<ICommandHandler<CreateArticle, Article>, CreateArticleHandler>();
        builder.Services.AddScoped<ICommandHandler<UpdateArticle, Article>, UpdateArticleHandler>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapCustomIdentityApi<HimsUser>();

        app.UseCors("wasm");

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        
        if (builder.Environment.IsDevelopment())
        {
            await using var scope = app.Services.CreateAsyncScope();
            await IdentitySeedData.InitializeAsync(scope.ServiceProvider);
        }

        // TODO move to controller
        app.MapGet("/roles", (ClaimsPrincipal user) =>
        {
            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                var identity = (ClaimsIdentity)user.Identity;
                var roles = identity.FindAll(identity.RoleClaimType)
                    .Select(c => 
                        new
                        {
                            c.Issuer, 
                            c.OriginalIssuer, 
                            c.Type, 
                            c.Value, 
                            c.ValueType
                        });

                return TypedResults.Json(roles);
            }

            return Results.Unauthorized();
        }).RequireAuthorization();

        // TODO move to controller
        app.MapPost("/logout", async (SignInManager<HimsUser> signInManager, [FromBody] object empty) =>
        {
            if (empty is not null)
            {
                await signInManager.SignOutAsync();

                return Results.Ok();
            }

            return Results.Unauthorized();
        }).RequireAuthorization();

        app.MapPost("/command", async (HttpContext httpContext) =>
        {
            var bodyReader = new StreamReader(httpContext.Request.Body);
            var bodyContent = await bodyReader.ReadToEndAsync();

            try
            {
                var commandEnvelope = JsonSerializer.Deserialize<CommandEnvelope>(bodyContent);
                var command = commandEnvelope.Deserialize();

                var commandRouter = httpContext.RequestServices.GetRequiredService<CommandRouter>();
                await commandRouter.Handle(command);
            }
            catch (System.Exception)
            {
                
                throw;
            }
            
        })
        .WithName("PostCommand")
        .WithOpenApi();

        app.MapGet("/article/{id:guid}", async (HttpContext httpContext, Guid id) =>
        {
            var readModelAccessor = httpContext.RequestServices.GetRequiredService<IReadModelAccess<ArticleAggregator>>();

            var readResult = await readModelAccessor.GetById(id);

            if (readResult.IsSuccess)
            {
                return Results.Ok(readResult.Value?.Aggregate);
            }
            else
            {
                return Results.Problem(readResult.Errors.First().Message);
            }
        });

        // Set up hosting of client WebAssembly app
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();
        app.MapFallbackToFile("index.html");

        app.Run();
    }
}
