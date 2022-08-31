namespace OpenStardriveServer.Domain.Systems.Comms.ShortRange;

public record SetBroadcastingPayload
{
    public bool IsBroadcasting { get; init; }
}