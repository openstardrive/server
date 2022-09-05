namespace OpenStardriveServer.Domain.Systems.Alert;

public record ConfigureAlertLevelsPayload
{
    public AlertLevel[] Levels { get; init; }
    public int CurrentLevel { get; init; }
}