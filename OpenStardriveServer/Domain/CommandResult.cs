using System;
using System.Text.Json.Serialization;

namespace OpenStardriveServer.Domain;

public class CommandResult
{
    public static string UnrecognizedCommandType = "unrecognized-command";
    public static string StateUpdatedType = "state-updated";
    public static string NoChangeType = "no-change";
    public static string ErrorType = "error";

    public static CommandResult UnrecognizedCommand(Command command) =>
        new(command, UnrecognizedCommandType, "", null);

    public static CommandResult StateChanged(Command command, string system, object newState) =>
        new(command, StateUpdatedType, system, newState);

    public static CommandResult NoChange(Command command, string system) =>
        new(command, NoChangeType, system, null);

    public static CommandResult Error(Command command, string system, string message) =>
        new(command, ErrorType, system, message);

    public CommandResult() { }

    private CommandResult(Command command, string type, string system, object payload)
    {
        CommandId = command.CommandId;
        ClientId = command.ClientId;
        Type = type;
        System = system;
        Payload = Json.Instance.Serialize(payload);
        Timestamp = command.TimeStamp;
    }

    public long RowId { get; set; }
    public Guid CommandResultId { get; set; } = Guid.NewGuid();
    public string Type { get; set; }
    public Guid CommandId { get; set; }
    public Guid ClientId { get; set; }
    public string System { get; set; } = "";
        
    [JsonConverter(typeof(RawJsonWriter))]
    public string Payload { get; set; } = "null";
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
}