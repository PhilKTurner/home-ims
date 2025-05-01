using FluentResults;
using HomeIMS.SharedContracts.Commands;
using HomeIMS.SharedContracts.Domain.Inventories;
using HomeIMS.SharedContracts.Domain.Inventories.Commands;
using HomeIMS.SharedContracts.Domain.Inventories.Events;
using HomeIMS.SharedContracts.EventSourcing;

public class CreateInventoryHandler : ICommandHandler<CreateInventory, Inventory>
{
    public Result<IEnumerable<HimsEvent>> Decide(CreateInventory command, Inventory? state)
    {
        ArgumentNullException.ThrowIfNull(command);

        var creationEvent = new InventoryCreated
        {
            ArticleGroupId = command.ArticleGroupId,
            Name = command.Name,
            Description = command.Description
        };

        return Result.Ok<IEnumerable<HimsEvent>>(new List<HimsEvent> { creationEvent });
    }

    public Task<Result<Inventory?>> LoadState(CreateInventory command)
    {
        return Task.FromResult(Result.Ok(default(Inventory)));
    }
}