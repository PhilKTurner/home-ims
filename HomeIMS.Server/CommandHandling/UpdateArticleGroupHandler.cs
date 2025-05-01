using FluentResults;
using HomeIMS.Server.EventStore.Aggregators;
using HomeIMS.SharedContracts.Commands;
using HomeIMS.SharedContracts.Domain.ArticleGroups;
using HomeIMS.SharedContracts.Domain.ArticleGroups.Commands;
using HomeIMS.SharedContracts.Domain.ArticleGroups.Events;
using HomeIMS.SharedContracts.EventSourcing;

namespace HomeIMS.Server.CommandHandling;

public class UpdateArticleGroupHandler : ICommandHandler<UpdateArticleGroup, ArticleGroup>
{
    private readonly IEventStore eventStore;

    public UpdateArticleGroupHandler(IEventStore eventStore)
    {
        this.eventStore = eventStore;
    }

    public Result<IEnumerable<HimsEvent>> Decide(UpdateArticleGroup command, ArticleGroup? state)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        if (state == null)
            return Result.Fail<IEnumerable<HimsEvent>>($"State required for handling of {nameof(UpdateArticleGroup)} command.");

        var modificationEvent = new ArticleGroupModified(command.Id);

        var modificationMade = false;

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

    public async Task<Result<ArticleGroup?>> LoadState(UpdateArticleGroup command)
    {
        var aggregationResult = await eventStore.AggregateStream<ArticleGroupAggregator>(command.Id);

        return aggregationResult.IsSuccess ? Result.Ok(aggregationResult.Value?.Aggregate) : Result.Fail<ArticleGroup?>(aggregationResult.Errors);
    }
}