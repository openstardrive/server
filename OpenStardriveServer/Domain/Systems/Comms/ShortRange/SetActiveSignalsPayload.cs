namespace OpenStardriveServer.Domain.Systems.Comms.ShortRange;

public record SetActiveSignalsPayload
{
    public Signal[] ActiveSignals { get; init; }
}