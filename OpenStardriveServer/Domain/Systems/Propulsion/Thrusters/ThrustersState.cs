namespace OpenStardriveServer.Domain.Systems.Propulsion.Thrusters
{
    public class ThrustersState : SystemBaseState
    {
        public ThrustersAttitude Attitude { get; set; }
        public ThrusterVelocity Velocity { get; set; }
    }

    public class ThrustersAttitude
    {
        public int Yaw { get; set; }
        public int Pitch { get; set; }
        public int Roll { get; set; }
    }

    public class ThrusterVelocity
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }
}