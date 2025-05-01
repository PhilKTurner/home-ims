using HomeIMS.SharedContracts.Commands;

namespace HomeIMS.SharedContracts.Domain.ArticleGroups.Commands;

public class CreateArticleGroup : HimsCommand, IArticleGroup
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}
