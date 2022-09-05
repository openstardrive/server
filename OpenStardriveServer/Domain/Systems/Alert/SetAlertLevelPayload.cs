namespace OpenStardriveServer.Domain.Systems.Alert;

public record SetAlertLevelPayload
{
    public int Level { get; init; }
}