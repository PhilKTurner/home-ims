using HomeIMS.SharedContracts.EventSourcing;

namespace HomeIMS.SharedContracts.Domain.Articles.Events;

public class ArticleCreated : HimsEvent, IArticle
{
    public ArticleCreated() : base(Guid.NewGuid())
    {
    }

    public Guid? GroupId { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
}
