using HomeIMS.Client.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

namespace HomeIMS.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddMudServices();

        builder.Services.AddTransient<CookieHandler>(); // TODO scoped?

        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
        builder.Services.AddScoped(sp => (IAccountManagement)sp.GetRequiredService<AuthenticationStateProvider>());

        builder.Services.AddScoped(sp =>
            new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }); // TODO adjust port stuff
        builder.Services.AddHttpClient(
            "Auth",
            opt => opt.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)) // TODO adjust port stuff
            .AddHttpMessageHandler<CookieHandler>();

        await builder.Build().RunAsync();
    }
}
