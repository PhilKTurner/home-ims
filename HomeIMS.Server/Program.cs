using System.Data.Common;
using System.Security.Claims;
using System.Text.Json;
using EventStore.Client;
using HomeIMS.Server.CommandHandling;
using HomeIMS.Server.Database;
using HomeIMS.Server.Identity;
using HomeIMS.SharedContracts.Commands;
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

        var connectionStringBuilder = new DbConnectionStringBuilder();
        connectionStringBuilder.ConnectionString = builder.Configuration.GetConnectionString("HimsIdentityDatabase");

        if (Directory.Exists("/run/secrets"))
        {
            builder.Configuration.AddKeyPerFile("/run/secrets");
            connectionStringBuilder.Add("Password", builder.Configuration["hims-db-userpw"] ?? string.Empty);
        }
        else if (builder.Environment.IsDevelopment())
        {
            connectionStringBuilder.Add("Password", builder.Configuration["HimsIdentityDatabase:Password"] ?? string.Empty);
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
                .UseMySql(connectionStringBuilder.ConnectionString, dbServerVersion, mySqlOptions => mySqlOptions.EnableRetryOnFailure())
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

        builder.Services.AddSingleton<CommandRouter>();
        builder.Services.AddSingleton<IncrementCounterCommandHandler>();
        builder.Services.AddSingleton<CreateHouseholdArticleCommandHandler>();

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
                await commandRouter.Handle(httpContext.RequestServices, command);
            }
            catch (System.Exception)
            {
                
                throw;
            }
            
        })
        .WithName("PostCommand")
        .WithOpenApi();

        app.MapGet("/counter", async (HttpContext httpContext) =>
        {
            const string connectionString = "esdb://hims-eventstore:2113?tls=false&tlsVerifyCert=false";
            var settings = EventStoreClientSettings.Create(connectionString);
            var client = new EventStoreClient(settings);

            var counterEvents = client.ReadStreamAsync(Direction.Backwards, "counter-stream", StreamPosition.End);

            if (await counterEvents.ReadState == ReadState.StreamNotFound)
                return Results.Ok(0);

            var counterEventCount = await counterEvents.CountAsync();

            return Results.Ok(counterEventCount);
        });

        // Set up hosting of client WebAssembly app
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();
        app.MapFallbackToFile("index.html");

        app.Run();
    }
}
