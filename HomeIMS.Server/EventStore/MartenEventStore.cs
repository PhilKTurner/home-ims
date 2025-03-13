using FluentResults;
using HomeIMS.SharedContracts.EventSourcing;
using Marten;

namespace HomeIMS.Server.EventStore;

public class MartenEventStore : IEventStore
{
    private readonly IDocumentStore documentStore;

    public MartenEventStore(IDocumentStore documentStore)
    {
        this.documentStore = documentStore;
    }

    public async Task<Result<TAggregator>> AggregateStream<TAggregator>(Guid streamId)
        where TAggregator : class, new()
    {
        try
        {
            using var session = documentStore.LightweightSession();

            var aggregator = await session.Events.AggregateStreamAsync<TAggregator>(streamId);
            if (aggregator is null)
                return Result.Fail<TAggregator>(new Error($"Stream '{streamId}' not found."));

            return Result.Ok(aggregator);
        }
        catch (Exception exception)
        {
            return Result.Fail<TAggregator>(new Error($"An error occurred while aggregating stream '{streamId}'.").CausedBy(exception));
        }
    }

    public async Task<Result> AppendEventToStream<TEvent>(Guid streamId, TEvent eventToAppend) where TEvent : HimsEvent
    {
        try
        {
            using var session = documentStore.LightweightSession();

            session.Events.Append(streamId, eventToAppend);
            await session.SaveChangesAsync();

            return Result.Ok();
        }
        catch (Exception exception)
        {
            return Result.Fail(new Error($"An error occurred while appending event to stream '{streamId}'.").CausedBy(exception));
        }
    }
}