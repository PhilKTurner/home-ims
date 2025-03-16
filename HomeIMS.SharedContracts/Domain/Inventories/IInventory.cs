namespace HomeIMS.SharedContracts.Domain.Inventories;

public interface IInventory
{
    Guid? ArticleGroupId { get; set; }

    string? Name { get; set; }
    string? Description { get; set; }
}