using FluentResults;
using HomeIMS.SharedContracts.Commands;
using HomeIMS.SharedContracts.Domain.ArticleGroups;
using HomeIMS.SharedContracts.Domain.ArticleGroups.Commands;
using HomeIMS.SharedContracts.Domain.ArticleGroups.Events;
using HomeIMS.SharedContracts.EventSourcing;

namespace HomeIMS.Server.CommandHandling;

public class CreateArticleGroupHandler : ICommandHandler<CreateArticleGroup, ArticleGroup>
{
    public Result<IEnumerable<HimsEvent>> Decide(CreateArticleGroup command, ArticleGroup? state)
    {
        ArgumentNullException.ThrowIfNull(command);

        var creationEvent = new ArticleGroupCreated
        {
            Name = command.Name,
            Description = command.Description
        };

        return Result.Ok<IEnumerable<HimsEvent>>(new List<HimsEvent> { creationEvent });
    }

    public Task<Result<ArticleGroup?>> LoadState(CreateArticleGroup command)
    {
        // TODO check for duplicates maybe?

        return Task.FromResult(Result.Ok(default(ArticleGroup)));
    }
}
