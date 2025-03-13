using HomeIMS.SharedContracts.EventSourcing;

namespace HomeIMS.Server.CommandHandling;

public class CommandHandlerBase
{
    protected readonly IEventStore eventStore;

    public CommandHandlerBase(IEventStore eventStore)
    {
        this.eventStore = eventStore;
    }
}