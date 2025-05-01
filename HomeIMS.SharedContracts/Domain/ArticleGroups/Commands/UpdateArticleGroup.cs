using HomeIMS.SharedContracts.Commands;

namespace HomeIMS.SharedContracts.Domain.ArticleGroups.Commands;

public class UpdateArticleGroup : HimsCommand, IArticleGroup
{
    public UpdateArticleGroup()
    {
    }

    public UpdateArticleGroup(ArticleGroup groupToUpdate)
    {
        Id = groupToUpdate.Id;
        Name = groupToUpdate.Name;
        Description = groupToUpdate.Description;
    }

    public Guid Id { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
}
