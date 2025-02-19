namespace HomeIMS.SharedContracts.Domain;

public class Article : IArticle
{
    public Guid Id { get; set; }
    public Guid? GroupId { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
}