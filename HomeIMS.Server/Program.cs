
using System.Text;
using System.Text.Json;
using EventStore.Client;
using HomeIMS.Server.CommandHandling;
using HomeIMS.SharedContracts.Commands;

namespace HomeIMS.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddSingleton<CommandRouter>();
        builder.Services.AddSingleton<IncrementCounterCommandHandler>();

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

        app.UseHttpsRedirection();

        app.UseAuthorization();

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
