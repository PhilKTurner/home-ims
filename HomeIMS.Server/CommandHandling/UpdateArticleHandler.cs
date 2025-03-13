using FluentResults;
using HomeIMS.Server.EventStore.Aggregators;
using HomeIMS.SharedContracts.Commands;
using HomeIMS.SharedContracts.Domain.Articles;
using HomeIMS.SharedContracts.Domain.Articles.Commands;
using HomeIMS.SharedContracts.Domain.Articles.Events;
using HomeIMS.SharedContracts.EventSourcing;

namespace HomeIMS.Server.CommandHandling;

public class UpdateArticleHandler : ICommandHandler<UpdateArticle, Article>
{
    private readonly IEventStore eventStore;

    public UpdateArticleHandler(IEventStore eventStore)
    {
        this.eventStore = eventStore;
    }

    public Result<IEnumerable<HimsEvent>> Decide(UpdateArticle command, Article? state)
    {
        if (state == null)
            return Result.Fail<IEnumerable<HimsEvent>>($"State required for handling of {nameof(UpdateArticle)} command.");

        var modificationEvent = new ArticleModified(command.Id);

        if (command.GroupId.HasValue && command.GroupId.Value != state.GroupId)
            modificationEvent.GroupId = command.GroupId.Value;

        if (!string.IsNullOrWhiteSpace(command.Name) && command.Name != state.Name)
            modificationEvent.Name = command.Name;

        if (!string.IsNullOrWhiteSpace(command.Description) && command.Description != state.Description)
            modificationEvent.Description = command.Description;

        return Result.Ok<IEnumerable<HimsEvent>>(new List<HimsEvent> { modificationEvent });
    }

    public async Task<Result<Article?>> LoadState(UpdateArticle command)
    {
        var aggregationResult = await eventStore.AggregateStream<ArticleAggregator>(command.Id);

        return aggregationResult.IsSuccess ? Result.Ok(aggregationResult.Value?.Aggregate) : Result.Fail<Article?>(aggregationResult.Errors);
    }
}