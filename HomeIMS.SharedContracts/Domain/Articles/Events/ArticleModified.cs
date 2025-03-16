using HomeIMS.SharedContracts.EventSourcing;

namespace HomeIMS.SharedContracts.Domain.Articles.Events;

public class ArticleModified : HimsEvent, IArticle
{
    public ArticleModified(Guid streamId) : base(streamId)
    {
    }

    public Guid? ArticleGroupId { get; set;}

    public string? Name { get; set; }
    public string? Description { get; set; }
}