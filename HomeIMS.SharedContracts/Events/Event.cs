using System.Text.Json;

namespace HomeIMS.SharedContracts.Events;

public abstract class Event
{
    public Guid Id { get; set; }
    public DateTimeOffset Timestamp { get; set; }

    protected Event(DateTimeOffset timestamp)
    {
        Id = Guid.NewGuid();
        Timestamp = timestamp;
    }

    public byte[] SerializeMetadata()
    {
        return JsonSerializer.SerializeToUtf8Bytes(this, typeof(Event));
    }
}
