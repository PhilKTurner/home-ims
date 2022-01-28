using System.Data.Common;
using HomeIMS.DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddIniFile("/env-config", true);
builder.Configuration.AddKeyPerFile("/run/secrets", true);

// Add services to the container.

builder.Services.AddControllersWithViews();

var connectionStringBuilder = new DbConnectionStringBuilder();
connectionStringBuilder.ConnectionString = builder.Configuration.GetConnectionString("HimsDatabase");
connectionStringBuilder.Add("Database", builder.Configuration["DB_NAME"]);
connectionStringBuilder.Add("Password", builder.Configuration["hims-mariadb-pw"]);

builder.Services.AddDbContext<HomeImsContext>(
    options => options.UseMySql(
            connectionStringBuilder.ConnectionString,
            new MariaDbServerVersion("10.6.5"),
            options => options.EnableRetryOnFailure()
        ),
    ServiceLifetime.Transient
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");;

app.Run();
