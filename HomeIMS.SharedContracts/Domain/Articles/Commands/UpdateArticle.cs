using HomeIMS.SharedContracts.Commands;

namespace HomeIMS.SharedContracts.Domain.Articles.Commands;

public class UpdateArticle : HimsCommand, IArticle
{
    public UpdateArticle()
    {
    }

    public UpdateArticle(Article articleToUpdate)
    {
        Id = articleToUpdate.Id;
        ArticleGroupId = articleToUpdate.ArticleGroupId;
        Name = articleToUpdate.Name;
        Description = articleToUpdate.Description;
    }

    public Guid Id { get; set; }

    public Guid? ArticleGroupId { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
}