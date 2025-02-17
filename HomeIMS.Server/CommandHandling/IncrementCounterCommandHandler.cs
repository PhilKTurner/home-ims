using EventStore.Client;
using HomeIMS.SharedContracts.Commands;
using HomeIMS.SharedContracts.Events;

public class IncrementCounterCommandHandler : ICommandHandler<IncrementCounterCommand>
{
    public async Task Handle(IncrementCounterCommand command)
    {
            const string connectionString = "esdb://hims-eventstore:2113?tls=false&tlsVerifyCert=false";
            var settings = EventStoreClientSettings.Create(connectionString);
            var client = new EventStoreClient(settings);

        var counterEvent = new CounterIncrementedEvent(DateTimeOffset.UtcNow); // TODO Timestamps on command creation?

        var eventData = new EventData(
                Uuid.NewUuid(),
                "TestEvent",
                null,
                counterEvent.SerializeMetadata()
            );

        await client.AppendToStreamAsync(
                "counter-stream",
                StreamState.Any,
                new[] { eventData }
            );
    }
}