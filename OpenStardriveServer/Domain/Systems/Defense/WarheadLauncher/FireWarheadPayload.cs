namespace OpenStardriveServer.Domain.Systems.Defense.WarheadLauncher;

public record FireWarheadPayload
{
    public string Kind { get; init; } = "";
    public string Target { get; init; } = "";
}