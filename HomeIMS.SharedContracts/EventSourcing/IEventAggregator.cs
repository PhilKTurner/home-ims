namespace HomeIMS.SharedContracts.EventSourcing;

public interface IEventAggregator<TEntity>
    where TEntity : class, new()
{
    TEntity Aggregate { get; }

    int Version { get; set; }
}
