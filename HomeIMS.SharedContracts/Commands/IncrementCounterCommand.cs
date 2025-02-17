namespace HomeIMS.SharedContracts.Commands;

public class IncrementCounterCommand : Command
{
    public IncrementCounterCommand()
    {
        Type = CommandType.IncrementCounter;
    }
}