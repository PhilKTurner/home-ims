using System.Text.Json;

namespace HomeIMS.SharedContracts.Events;

public abstract class Event
{
    public Guid EventId { get; set; }
    public DateTimeOffset EventTimestamp { get; set; }

    protected Event(DateTimeOffset timestamp)
    {
        EventId = Guid.NewGuid();
        EventTimestamp = timestamp;
    }

    public byte[] SerializeMetadata()
    {
        return JsonSerializer.SerializeToUtf8Bytes(this, typeof(Event));
    }

    public byte[] SerializeData<TDataInterface>()
    {
        if (this is not TDataInterface data)
            throw new InvalidOperationException($"Event does not implement {typeof(TDataInterface).Name}");

        return JsonSerializer.SerializeToUtf8Bytes(data, typeof(TDataInterface));
    }
}
