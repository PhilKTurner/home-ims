using HomeIMS.SharedContracts.Domain.Inventories;
using HomeIMS.SharedContracts.Domain.Inventories.Events;
using HomeIMS.SharedContracts.EventSourcing;

namespace HomeIMS.Server.EventStore.Aggregators;

public class InventoryAggregator : Inventory, IEventAggregator<Inventory>
{
    public Inventory Aggregate => (Inventory)this;

    public int Version { get; set; }

    public void Apply(InventoryCreated @event)
    {
        Id = @event.StreamId;
        ArticleGroupId = @event.ArticleGroupId;
        Name = @event.Name;
        Description = @event.Description;
    }

    public void Apply(InventoryModified @event)
    {
        ArticleGroupId = @event.ArticleGroupId ?? ArticleGroupId;
        Name = @event.Name ?? Name;
        Description = @event.Description ?? Description;
    }
}