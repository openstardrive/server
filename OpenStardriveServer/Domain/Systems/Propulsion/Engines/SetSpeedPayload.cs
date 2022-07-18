namespace OpenStardriveServer.Domain.Systems.Propulsion.Engines
{
    public record SetSpeedPayload
    {
        public int Speed { get; init; }
    }
}