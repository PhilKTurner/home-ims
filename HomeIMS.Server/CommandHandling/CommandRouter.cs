using HomeIMS.SharedContracts.Commands;

namespace HomeIMS.Server.CommandHandling;

public class CommandRouter
{
    public async Task Handle(IServiceProvider serviceProvider, Command command)
    {
        switch (command.Type)
        {
            case CommandType.IncrementCounter:
                var handler = serviceProvider.GetRequiredService<IncrementCounterCommandHandler>();
                await handler.Handle((IncrementCounterCommand)command);
                break;
            default:
                throw new NotImplementedException($"Command type '{command.Type}' is not implemented.");
        }
    }
}