namespace OpenStardriveServer.Domain.Systems.Defense.WarheadLauncher;

public record LoadWarheadPayload
{
    public string Kind { get; init; }
}