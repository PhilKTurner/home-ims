namespace HomeIMS.SharedContracts.Domain;

public interface IArticle
{
    Guid Id { get; set; }
    Guid? GroupId { get; set; }

    string? Name { get; set; }
    string? Description { get; set; }
}
