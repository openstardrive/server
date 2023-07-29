using System;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Engines;

public record EnginesConfigurationPayload
{
    public EngineSpeedConfig SpeedConfig { get; init; }
    public EngineHeatConfig HeatConfig { get; init; }
    
    public SpeedPowerRequirement[] SpeedPowerRequirements { get; init; } = Array.Empty<SpeedPowerRequirement>();
}