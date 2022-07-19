namespace OpenStardriveServer.Domain.Systems.Propulsion.Thrusters;

public record ThrusterAttitudePayload
{
    public int Yaw { get; init; }
    public int Pitch { get; init; }
    public int Roll { get; init; }
}