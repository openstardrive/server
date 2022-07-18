using System;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Engines;

public record EnginesConfigurationPayload
{
    public EngineSpeedConfig SpeedConfig { get; init; }
    public EngineHeatConfig HeatConfig { get; init; }
    public int RequiredPower { get; init; }
    public SpeedPowerRequirement[] SpeedPowerRequirements = Array.Empty<SpeedPowerRequirement>();
}