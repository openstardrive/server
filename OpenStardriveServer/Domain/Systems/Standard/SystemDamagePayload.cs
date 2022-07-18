namespace OpenStardriveServer.Domain.Systems.Standard;

public record SystemDamagePayload
{
    public bool Damaged { get; init; }
}