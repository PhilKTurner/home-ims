namespace HomeIMS.SharedContracts.Events;

public class CounterIncrementedEvent : Event
{
    public CounterIncrementedEvent(DateTimeOffset timestamp) : base(timestamp)
    {
    }
}
