namespace HomeIMS.SharedContracts.Domain.Articles;

public interface IArticle
{
    Guid? GroupId { get; set; }

    string? Name { get; set; }
    string? Description { get; set; }
}
