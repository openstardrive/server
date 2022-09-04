namespace OpenStardriveServer.Domain.Systems.Power;

public record BatteryDamagePayload
{
    public int BatteryIndex { get; init; }
    public bool IsDamaged { get; init; }
}