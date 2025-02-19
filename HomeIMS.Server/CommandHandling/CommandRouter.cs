using HomeIMS.SharedContracts.Commands;

namespace HomeIMS.Server.CommandHandling;

public class CommandRouter
{
    public async Task Handle(IServiceProvider serviceProvider, Command command)
    {
        switch (command.Type)
        {
            case CommandType.IncrementCounter:
                await serviceProvider.GetRequiredService<IncrementCounterCommandHandler>().Handle((IncrementCounterCommand)command);
                break;
            case CommandType.CreateHouseholdArticle:
                await serviceProvider.GetRequiredService<CreateHouseholdArticleCommandHandler>().Handle((CreateHouseholdArticleCommand)command);
                break;
            default:
                throw new NotImplementedException($"There is no handler routing available for command type '{command.Type}'.");
        }
    }
}