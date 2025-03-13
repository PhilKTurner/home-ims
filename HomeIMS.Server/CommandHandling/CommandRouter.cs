using FluentResults;
using HomeIMS.SharedContracts.Commands;
using HomeIMS.SharedContracts.Domain.Articles;
using HomeIMS.SharedContracts.Domain.Articles.Commands;
using HomeIMS.SharedContracts.EventSourcing;

namespace HomeIMS.Server.CommandHandling;

public class CommandRouter
{
    private readonly IServiceProvider serviceProvider;
    private readonly IEventStore eventStore;

    public CommandRouter(IServiceProvider serviceProvider, IEventStore eventStore)
    {
        this.serviceProvider = serviceProvider;
        this.eventStore = eventStore;
    }

    public async Task<Result> Handle(HimsCommand command)
    {
        var deciderResult = default(Result<IEnumerable<HimsEvent>>);

        try
        {
            switch (command)
            {
                case CreateArticle createArticleCommand:
                    deciderResult = await DecideOnEventsForCommand<CreateArticle, Article>(createArticleCommand);
                    break;
                case UpdateArticle updateArticleCommand:
                    deciderResult = await DecideOnEventsForCommand<UpdateArticle, Article>(updateArticleCommand);
                    break;
                default:
                    return Result.Fail($"There is no handler routing available for command type '{command.GetType()}'.");
            }
        }
        catch (Exception)
        {
            return Result.Fail($"An error occurred while deciding on the command '{command.GetType()}'.");
        }

        if (deciderResult.IsFailed)
            return deciderResult.ToResult(); // TODO ???

        foreach (var decidedEvent in deciderResult.Value)
        {
            await eventStore.AppendEventToStream(decidedEvent.StreamId, decidedEvent);
        }

        return Result.Ok();
    }

    private async Task<Result<IEnumerable<HimsEvent>>> DecideOnEventsForCommand<TCommand, TState>(TCommand command)
        where TCommand: HimsCommand
        where TState: class
    {
        var createArticleHandler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TState>>();

        var loadStateResult = await createArticleHandler.LoadState((TCommand)command);
        if (loadStateResult.IsFailed)
            return loadStateResult.ToResult(); // TODO ???

        return createArticleHandler.Decide((TCommand)command, loadStateResult?.Value);
    }
}