namespace HomeIMS.SharedContracts.Domain.Articles;

public class Article : IEntityWithId, IArticle
{
    public Guid Id { get; set; }

    public Guid? GroupId { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
}
