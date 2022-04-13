using System.Data.Common;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Transports.AspNetCore;
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
    connectionStringBuilder.Add("Password", builder.Configuration["HimsDatabase:Password"]);
} else if (Directory.Exists("/run/secrets")) {
    builder.Configuration.AddKeyPerFile("/run/secrets");
    connectionStringBuilder.Add("Password", builder.Configuration["hims-mariadb-pw"]);
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

builder.Services.AddSingleton<HomeImsQuery>();
builder.Services.AddSingleton<HomeImsMutation>();
builder.Services.AddSingleton<HomeImsSchema>();

GraphQL.MicrosoftDI.GraphQLBuilderExtensions.AddGraphQL(builder.Services)
    .AddServer(true)
    .AddSystemTextJson()
    .AddErrorInfoProvider(options => options.ExposeExceptionStackTrace = builder.Environment.IsDevelopment())
    .AddGraphTypes(typeof(HomeImsSchema).Assembly);

var app = builder.Build();

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

app.UseEndpoints(endpoints => 
{
    endpoints.MapGraphQL<HomeImsSchema, GraphQLHttpMiddleware<HomeImsSchema>>();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
