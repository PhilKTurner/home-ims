namespace HomeIMS.SharedContracts.Domain.Inventories;

public class Inventory : IInventory, IIdentifiableEntity
{
    public Guid Id { get; set; }

    public Guid? ArticleGroupId { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
}