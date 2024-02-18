using HotChocolate;
using System.Data.Common;
using HomeIMS.DataAccess;
using HomeIMS.GraphQL;
using Microsoft.EntityFrameworkCore;

// TODO Research why this approach replaced Startup in dotnet scaffolding

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRouting();

var connectionStringBuilder = new DbConnectionStringBuilder();
connectionStringBuilder.ConnectionString = builder.Configuration.GetConnectionString("HimsDatabase");

if (builder.Environment.IsDevelopment())
{
    connectionStringBuilder.Add("Password", builder.Configuration["HimsDatabase:Password"] ?? string.Empty);
} else if (Directory.Exists("/run/secrets")) {
    builder.Configuration.AddKeyPerFile("/run/secrets");
    connectionStringBuilder.Add("Password", builder.Configuration["hims-mariadb-pw"] ?? string.Empty);
} else {
    throw new Exception("Password for database access not found.");
}

builder.Services.AddDbContext<HomeImsContext>(
    options => options.UseMySql(
            connectionStringBuilder.ConnectionString,
            new MariaDbServerVersion("10.6.5"),
            options => options.EnableRetryOnFailure()
        )
);

builder.Services
       .AddGraphQLServer()
       .RegisterDbContext<HomeImsContext>()
       .AddQueryType<HomeImsQuery>()
       .AddType<ArticleType>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // CORS is only necessary in separated setup for dev environment
    app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HomeImsContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapGraphQL();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
