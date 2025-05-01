using HomeIMS.SharedContracts.Domain.ArticleGroups;
using HomeIMS.SharedContracts.Domain.ArticleGroups.Events;
using HomeIMS.SharedContracts.EventSourcing;

namespace HomeIMS.Server.EventStore.Aggregators;

public class ArticleGroupAggregator : ArticleGroup, IEventAggregator<ArticleGroup>
{
    public ArticleGroup Aggregate => (ArticleGroup)this;

    public int Version { get; set; }

    public void Apply(ArticleGroupCreated @event)
    {
        Id = @event.StreamId;
        Name = @event.Name;
        Description = @event.Description;
    }

    public void Apply(ArticleGroupModified @event)
    {
        Name = @event.Name ?? Name;
        Description = @event.Description ?? Description;
    }
}