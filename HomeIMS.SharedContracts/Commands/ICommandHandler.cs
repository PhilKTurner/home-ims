using FluentResults;
using HomeIMS.SharedContracts.EventSourcing;

namespace HomeIMS.SharedContracts.Commands;

// Decider pattern based on https://thinkbeforecoding.com/post/2021/12/17/functional-event-sourcing-decider
public interface ICommandHandler<TCommand, TState>
    where TCommand : HimsCommand
    where TState : class
{
    Task<Result<TState?>> LoadState(TCommand command);
    Result<IEnumerable<HimsEvent>> Decide(TCommand command, TState? state);
}
