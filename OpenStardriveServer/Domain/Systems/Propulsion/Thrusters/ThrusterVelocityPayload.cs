namespace OpenStardriveServer.Domain.Systems.Propulsion.Thrusters
{
    public record ThrusterVelocityPayload
    {
        public int X { get; init; }
        public int Y { get; init; }
        public int Z { get; init; }
    }
}