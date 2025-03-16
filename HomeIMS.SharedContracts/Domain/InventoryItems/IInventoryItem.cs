namespace HomeIMS.SharedContracts.Domain.InventoryItems;

public interface IInventoryItem
{
    Guid ArticleId { get; }
    Guid InventoryId { get; }

    string? Name { get; set; }
    string? Description { get; }
}
