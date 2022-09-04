namespace OpenStardriveServer.Domain.Systems.Power;

public record BatteryChargePayload
{
    public int BatteryIndex { get; init; }
    public int Charge { get; init; }
}