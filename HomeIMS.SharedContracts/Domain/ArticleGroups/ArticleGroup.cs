namespace HomeIMS.SharedContracts.Domain.ArticleGroups;

public class ArticleGroup : IArticleGroup, IIdentifiableEntity
{
    public Guid Id { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
}