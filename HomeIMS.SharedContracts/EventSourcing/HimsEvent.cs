using System.Text.Json.Serialization;

namespace HomeIMS.SharedContracts.EventSourcing;

public class HimsEvent
{
    public HimsEvent(Guid streamId)
    {
        StreamId = streamId;
    }

    [JsonIgnore]
    public Guid StreamId { get; }
}