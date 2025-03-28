using HomeIMS.SharedContracts.Domain.Articles;
using HomeIMS.SharedContracts.Domain.Articles.Events;
using HomeIMS.SharedContracts.EventSourcing;

namespace HomeIMS.Server.EventStore.Aggregators;

public class ArticleAggregator : Article, IEventAggregator<Article>
{
    public Article Aggregate => (Article)this;

    public int Version { get; set; }

    public void Apply(ArticleCreated @event)
    {
        Id = @event.StreamId;
        ArticleGroupId = @event.ArticleGroupId;
        Name = @event.Name;
        Description = @event.Description;
    }

    public void Apply(ArticleModified @event)
    {
        ArticleGroupId = @event.ArticleGroupId ?? ArticleGroupId;
        Name = @event.Name ?? Name;
        Description = @event.Description ?? Description;
    }
}
