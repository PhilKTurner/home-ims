using System.Text.Json;
using System.Text.Json.Serialization;

namespace HomeIMS.SharedContracts.Commands;

// loosely based on https://stackoverflow.com/a/38680249
public class CommandEnvelope
{
    public string? CommandTypeName { get; set; }
    public string? SerializedCommand { get; set; }

    [JsonIgnore]
    public Type CommandType => (!string.IsNullOrWhiteSpace(CommandTypeName) ? Type.GetType(CommandTypeName) : null)
                                ?? throw new Exception("Command type not found");

    public static CommandEnvelope CreateEnvelope<TCommand>(TCommand command)
        where TCommand : HimsCommand
    {
        var createdEnvelope = new CommandEnvelope
        {
            CommandTypeName = command.GetType().FullName,
            SerializedCommand = JsonSerializer.Serialize(command, typeof(TCommand))
        };

        return createdEnvelope;
    }

    public HimsCommand Deserialize()
    {
        if (CommandTypeName == null)
            throw new Exception("CommandTypeName is null");

        if (SerializedCommand == null)
            throw new Exception("SerializedCommand is null");

        var commandType = Type.GetType(CommandTypeName);
        if (commandType == null)
            throw new Exception("Command type not found");

        if (!commandType.IsSubclassOf(typeof(HimsCommand)))
            throw new Exception("Command type is not a subclass of Command");

        return JsonSerializer.Deserialize(SerializedCommand!, commandType) as HimsCommand ?? throw new Exception("Deserialization failed");
    }
}