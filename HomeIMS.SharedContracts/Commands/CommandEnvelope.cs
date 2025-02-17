using System.Text.Json;
using System.Text.Json.Serialization;

namespace HomeIMS.SharedContracts.Commands;

// loosely based on https://stackoverflow.com/a/38680249
public class CommandEnvelope
{
    public string? CommandTypeName { get; set; }
    public string? SerializedCommand { get; set; }

    [JsonIgnore]
    public Type CommandType => Type.GetType(CommandTypeName) ?? throw new Exception("Command type not found");

    public static CommandEnvelope CreateEnvelope(Command command)
    {
        var createdEnvelope = new CommandEnvelope
        {
            CommandTypeName = command.GetType().FullName,
            SerializedCommand = JsonSerializer.Serialize(command)
        };

        return createdEnvelope;
    }

    public Command Deserialize()
    {
        if (CommandTypeName == null)
            throw new Exception("CommandTypeName is null");

        if (SerializedCommand == null)
            throw new Exception("SerializedCommand is null");

        var commandType = Type.GetType(CommandTypeName);
        if (commandType == null)
            throw new Exception("Command type not found");

        if (!commandType.IsSubclassOf(typeof(Command)))
            throw new Exception("Command type is not a subclass of Command");

        return JsonSerializer.Deserialize(SerializedCommand!, commandType) as Command ?? throw new Exception("Deserialization failed");
    }
}