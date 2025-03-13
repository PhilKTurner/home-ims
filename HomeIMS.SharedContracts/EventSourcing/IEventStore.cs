using FluentResults;

namespace HomeIMS.SharedContracts.EventSourcing;

public interface IEventStore
{
    Task<Result<TAggregator>> AggregateStream<TAggregator>(Guid streamId) where TAggregator : class, new();
    Task<Result> AppendEventToStream<TEvent>(Guid streamId, TEvent eventToAppend) where TEvent : HimsEvent;
}
