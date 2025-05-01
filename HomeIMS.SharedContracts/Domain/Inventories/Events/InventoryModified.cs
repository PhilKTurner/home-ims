using HomeIMS.SharedContracts.EventSourcing;

namespace HomeIMS.SharedContracts.Domain.Inventories.Events;

public class InventoryModified : HimsEvent, IInventory
{
    public InventoryModified(Guid streamId) : base(streamId)
    {
    }

    public Guid? ArticleGroupId { get; set;}

    public string? Name { get; set; }
    public string? Description { get; set; }
}
