using HomeIMS.SharedContracts.Commands;

namespace HomeIMS.SharedContracts.Domain.Articles.Commands;

public class CreateArticle : HimsCommand, IArticle
{
    public Guid? ArticleGroupId { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
}
