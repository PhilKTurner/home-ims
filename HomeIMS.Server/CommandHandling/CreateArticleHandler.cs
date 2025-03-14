using FluentResults;
using HomeIMS.SharedContracts.Commands;
using HomeIMS.SharedContracts.Domain.Articles;
using HomeIMS.SharedContracts.Domain.Articles.Commands;
using HomeIMS.SharedContracts.Domain.Articles.Events;
using HomeIMS.SharedContracts.EventSourcing;

namespace HomeIMS.Server.CommandHandling;

public class CreateArticleHandler : ICommandHandler<CreateArticle, Article>
{
    public Result<IEnumerable<HimsEvent>> Decide(CreateArticle command, Article? state)
    {
        var creationEvent = new ArticleCreated
        {
            GroupId = command.GroupId,
            Name = command.Name,
            Description = command.Description
        };

        return Result.Ok<IEnumerable<HimsEvent>>(new List<HimsEvent> { creationEvent });
    }

    public Task<Result<Article?>> LoadState(CreateArticle command)
    {
        // TODO check for duplicates maybe?

        return Task.FromResult(Result.Ok(default(Article)));
    }
}