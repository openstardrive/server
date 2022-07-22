namespace OpenStardriveServer.Domain.Systems.Propulsion.Engines;

public static class EnginesStateDefaults
{
    public static EnginesState Ftl => new()
    {
        CurrentHeat = 1_500,
        SpeedConfig = new EngineSpeedConfig
        {
            CruisingSpeed = 6,
            MaxSpeed = 10
        },
        HeatConfig = new EngineHeatConfig
        {
            PoweredHeat = 2_000,
            CruisingHeat = 7_000,
            MaxHeat = 10_000,
            MinutesAtMaxSpeed = 5,
            MinutesToCoolDown = 12
        },
        CurrentPower = 10,
        RequiredPower = 10,
        SpeedPowerRequirements = new []
        {
            new SpeedPowerRequirement { Speed = 7, PowerNeeded = 12 },
            new SpeedPowerRequirement { Speed = 8, PowerNeeded = 15 },
            new SpeedPowerRequirement { Speed = 9, PowerNeeded = 18 },
            new SpeedPowerRequirement { Speed = 10, PowerNeeded = 20 }
        }
    };
        
    public static EnginesState Sublight => new()
    {
        CurrentHeat = 700,
        SpeedConfig = new EngineSpeedConfig
        {
            CruisingSpeed = 3,
            MaxSpeed = 5
        },
        HeatConfig = new EngineHeatConfig
        {
            PoweredHeat = 1_000,
            CruisingHeat = 3_000,
            MaxHeat = 5_000,
            MinutesAtMaxSpeed = 10,
            MinutesToCoolDown = 20
        },
        CurrentPower = 5,
        RequiredPower = 5,
        SpeedPowerRequirements = new []
        {
            new SpeedPowerRequirement { Speed = 5, PowerNeeded = 8 }
        }
    };
        
    public static EnginesState Testing => new()
    {
        CurrentHeat = 0,
        SpeedConfig = new EngineSpeedConfig
        {
            CruisingSpeed = 6,
            MaxSpeed = 10
        },
        HeatConfig = new EngineHeatConfig
        {
            PoweredHeat = 2_000,
            CruisingHeat = 7_000,
            MaxHeat = 10_000,
            MinutesAtMaxSpeed = 5,
            MinutesToCoolDown = 12
        }
    };
}