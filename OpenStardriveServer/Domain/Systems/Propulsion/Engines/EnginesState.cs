using System;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Engines;

public record EnginesState : StandardSystemBaseState
{
    public int CurrentSpeed { get; init; }
    public EngineSpeedConfig SpeedConfig { get; init; }
    public int CurrentHeat { get; init; }
    public EngineHeatConfig HeatConfig { get; init; }
    public SpeedPowerRequirement[] SpeedPowerRequirements { get; init; } = Array.Empty<SpeedPowerRequirement>();
}

public record EngineSpeedConfig
{
    public int MaxSpeed { get; init; }
    public int CruisingSpeed { get; init; }
}

public record EngineHeatConfig
{
    public int PoweredHeat { get; init; }
    public int CruisingHeat { get; init; }
    public int MaxHeat { get; init; }
    public int MinutesAtMaxSpeed { get; init; }
    public int MinutesToCoolDown { get; init; }
}

public record SpeedPowerRequirement
{
    public int Speed { get; init; }
    public int PowerNeeded { get; init; }
}