namespace OpenStardriveServer.Domain.Systems.Standard;

public record SystemDisabledPayload
{
    public bool Disabled { get; init; }
}