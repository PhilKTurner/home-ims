namespace HomeIMS.SharedContracts.Domain.InventoryItems;

public class InventoryItem : IInventoryItem, IIdentifiableEntity
{
    public Guid Id { get; set; }

    public Guid ArticleId { get; set; }
    public Guid InventoryId { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
}
