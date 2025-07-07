namespace OpenStardriveServer.Domain.Systems.Plugins;

public record UpdateJsonStatePayload
{
    public string Key { get; init; }
    public object Value { get; init; }
}
