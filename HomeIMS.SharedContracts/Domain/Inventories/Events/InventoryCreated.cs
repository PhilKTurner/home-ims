using HomeIMS.SharedContracts.EventSourcing;

namespace HomeIMS.SharedContracts.Domain.Inventories.Events;

public class InventoryCreated : HimsEvent, IInventory
{
    public InventoryCreated() : base(Guid.NewGuid())
    {
    }

    public Guid? ArticleGroupId { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
}
