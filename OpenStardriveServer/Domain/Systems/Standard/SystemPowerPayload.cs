namespace OpenStardriveServer.Domain.Systems.Standard;

public record SystemPowerPayload
{
    public int CurrentPower { get; init; }
}