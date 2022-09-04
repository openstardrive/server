namespace OpenStardriveServer.Domain.Systems.Power;

public record ConfigurePowerPayload
{
    public int TargetOutput { get; init; }
    public int ReactorDrift { get; init; }
    public int NumberOfBatteries { get; init; }
    public int MaxBatteryCharge { get; init; }
    public int NewBatteryCharge { get; init; }
    public int UpdateRateInMilliseconds { get; init; }
}