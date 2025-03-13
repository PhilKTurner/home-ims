using FluentResults;
using HomeIMS.SharedContracts.EventSourcing;
using Marten;

namespace HomeIMS.Server.EventStore;

public class SimpleMartenReadModelAccessor<TAggregator> : IReadModelAccess<TAggregator>
    where TAggregator : class
{
    private readonly IDocumentStore documentStore;

    public SimpleMartenReadModelAccessor(IDocumentStore documentStore)
    {
        this.documentStore = documentStore;
    }

    public async Task<Result<TAggregator?>> GetById(Guid id)
    {
        try
        {
            using var session = documentStore.LightweightSession();

            var aggregator = await session.LoadAsync<TAggregator>(id);

            return Result.Ok(aggregator);
        }
        catch (Exception exception)
        {
            return Result.Fail<TAggregator?>(new Error($"Error during read model access").CausedBy(exception));
        }
    }

    public async Task<Result<IEnumerable<TAggregator>>> GetAll()
    {
        try
        {
            using var session = documentStore.LightweightSession();

            var aggregators = await session.Query<TAggregator>().ToListAsync();

            return Result.Ok(aggregators.AsEnumerable());
        }
        catch (Exception exception)
        {
            return Result.Fail<IEnumerable<TAggregator>>(new Error($"Error during read model access").CausedBy(exception));
        }
    }
}