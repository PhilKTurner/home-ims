using HomeIMS.SharedContracts.EventSourcing;

namespace HomeIMS.SharedContracts.Domain.ArticleGroups.Events;

public class ArticleGroupCreated : HimsEvent, IArticleGroup
{
    public ArticleGroupCreated() : base(Guid.NewGuid())
    {
    }

    public string? Name { get; set; }
    public string? Description { get; set; }
}
