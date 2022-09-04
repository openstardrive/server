using System;
using System.Text.Json.Serialization;

namespace OpenStardriveServer.Domain.Systems.Power;

public record PowerState
{
    public int ReactorOutput { get; init; }
    public Battery[] Batteries { get; init; } = Array.Empty<Battery>();
    public PowerConfiguration Config { get; init; } = new();
    
    [JsonIgnore]
    public long MillisecondsUntilNextUpdate { get; init; }
}

public record Battery
{
    public int Charge { get; init; }
    public bool Damaged { get; init; }
}

public record PowerConfiguration
{
    public int TargetOutput { get; init; } = 100;
    public int ReactorDrift { get; init; } = 5;
    public int NumberOfBatteries { get; init; } = 0;
    public int MaxBatteryCharge { get; init; } = 0;
    public int UpdateRateInMilliseconds { get; init; } = 30000;
}