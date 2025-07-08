namespace OpenStardriveServer.Domain.Systems.Plugins;

public record UpdateJsonStatePayload
{
    public object Value { get; init; }
}
