namespace OpenStardriveServer.Domain.Systems.Propulsion.Engines
{
    public static class EnginesStateDefaults
    {
        public static EnginesState Ftl => new EnginesState
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
            }
        };
        
        public static EnginesState Sublight => new EnginesState
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
            }
        };
        
        public static EnginesState Testing => new EnginesState
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
}