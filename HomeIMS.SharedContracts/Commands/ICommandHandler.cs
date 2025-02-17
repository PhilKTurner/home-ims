namespace HomeIMS.SharedContracts.Commands;

public interface ICommandHandler<TCommand>
    where TCommand : Command
{
    Task Handle(TCommand command);
}