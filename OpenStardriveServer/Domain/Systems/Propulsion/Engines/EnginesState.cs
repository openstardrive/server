namespace OpenStardriveServer.Domain.Systems.Propulsion.Engines
{
    public class EnginesState : SystemBaseState
    {
        public int CurrentSpeed { get; set; }
        public EngineSpeedConfig SpeedConfig { get; set; }
        public int CurrentHeat { get; set; }
        public EngineHeatConfig HeatConfig { get; set; }
    }

    public class EngineSpeedConfig
    {
        public int MaxSpeed { get; set; }
        public int CruisingSpeed { get; set; }
    }

    public class EngineHeatConfig
    {
        public int PoweredHeat { get; set; }
        public int CruisingHeat { get; set; }
        public int MaxHeat { get; set; }
        public int MinutesAtMaxSpeed { get; set; }
        public int MinutesToCoolDown { get; set; }
    }
}