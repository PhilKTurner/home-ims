using FluentResults;
using HomeIMS.Server.EventStore.Aggregators;
using HomeIMS.SharedContracts.Commands;
using HomeIMS.SharedContracts.Domain.Inventories;
using HomeIMS.SharedContracts.Domain.Inventories.Commands;
using HomeIMS.SharedContracts.Domain.Inventories.Events;
using HomeIMS.SharedContracts.EventSourcing;

public class UpdateInventoryHandler : ICommandHandler<UpdateInventory, Inventory>
{
    private readonly IEventStore eventStore;

    public UpdateInventoryHandler(IEventStore eventStore)
    {
        this.eventStore = eventStore;
    }

    public Result<IEnumerable<HimsEvent>> Decide(UpdateInventory command, Inventory? state)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (state == null)
            return Result.Fail<IEnumerable<HimsEvent>>($"State required for handling of {nameof(UpdateInventory)} command.");

        var modificationEvent = new InventoryModified(command.Id);

        var modificationMade = false;

        if (command.ArticleGroupId.HasValue && command.ArticleGroupId.Value != state.ArticleGroupId)
        {
            modificationEvent.ArticleGroupId = command.ArticleGroupId.Value;
            modificationMade = true;
        }

        if (!string.IsNullOrWhiteSpace(command.Name) && command.Name != state.Name)
        {
            modificationEvent.Name = command.Name;
            modificationMade = true;
        }

        if (!string.IsNullOrWhiteSpace(command.Description) && command.Description != state.Description)
        {
            modificationEvent.Description = command.Description;
            modificationMade = true;
        }

        var eventList = new List<HimsEvent>();

        if (modificationMade)
        {
            eventList.Add(modificationEvent);
        }

        return Result.Ok<IEnumerable<HimsEvent>>(eventList);
    }

    public async Task<Result<Inventory?>> LoadState(UpdateInventory command)
    {
        var aggregationResult = await eventStore.AggregateStream<InventoryAggregator>(command.Id);

        return aggregationResult.IsSuccess ? Result.Ok(aggregationResult.Value?.Aggregate) : Result.Fail<Inventory?>(aggregationResult.Errors);
    }
}