using EventStore.Client;
using HomeIMS.SharedContracts.Commands;
using HomeIMS.SharedContracts.Domain;
using HomeIMS.SharedContracts.Events;

public class CreateHouseholdArticleCommandHandler : ICommandHandler<CreateHouseholdArticleCommand>
{
    public async Task Handle(CreateHouseholdArticleCommand command)
    {
        const string connectionString = "esdb://hims-eventstore:2113?tls=false&tlsVerifyCert=false";
        var settings = EventStoreClientSettings.Create(connectionString);
        var client = new EventStoreClient(settings);

        Console.WriteLine($"CreateHouseholdArticleCommand {command.Id} - {command.Name} - {command.Description} - {command.EAN} - {command.TargetAmount}");

        var creationEvent = new HouseholdArticleCreatedEvent(DateTimeOffset.UtcNow); // TODO Timestamps on command creation?
        creationEvent.Id = command.Id;
        creationEvent.GroupId = command.GroupId;
        creationEvent.Name = command.Name;
        creationEvent.Description = command.Description;
        creationEvent.EAN = command.EAN;
        creationEvent.TargetAmount = command.TargetAmount;

        var eventData = new EventData(
                Uuid.NewUuid(),
                "CreationEventTest",
                creationEvent.SerializeData<IHouseholdArticle>(),
                creationEvent.SerializeMetadata()
            );

        await client.AppendToStreamAsync(
                "domain-stream",
                StreamState.Any,
                new[] { eventData }
            );
    }
}