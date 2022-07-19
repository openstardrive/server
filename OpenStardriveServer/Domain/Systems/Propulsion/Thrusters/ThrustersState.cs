using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Thrusters;

public record ThrustersState : StandardSystemBaseState
{
    public ThrustersAttitude Attitude { get; init; } = new();
    public ThrusterVelocity Velocity { get; init; } = new();
}

public record ThrustersAttitude
{
    public int Yaw { get; init; }
    public int Pitch { get; init; }
    public int Roll { get; init; }
}

public record ThrusterVelocity
{
    public int X { get; init; }
    public int Y { get; init; }
    public int Z { get; init; }
}