using HomeIMS.SharedContracts.EventSourcing;

namespace HomeIMS.SharedContracts.Domain.ArticleGroups.Events;

public class ArticleGroupModified : HimsEvent, IArticleGroup
{
    public ArticleGroupModified(Guid streamId) : base(streamId)
    {
    }

    public string? Name { get; set; }
    public string? Description { get; set; }
}
