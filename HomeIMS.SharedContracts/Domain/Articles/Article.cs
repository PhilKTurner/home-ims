namespace HomeIMS.SharedContracts.Domain.Articles;

public class Article : IArticle, IIdentifiableEntity
{
    public Guid Id { get; set; }

    public Guid? ArticleGroupId { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
}
