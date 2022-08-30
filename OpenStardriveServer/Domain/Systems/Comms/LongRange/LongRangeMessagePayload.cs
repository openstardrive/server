namespace OpenStardriveServer.Domain.Systems.Comms.LongRange;

public record LongRangeMessagePayload
{
    public string MessageId { get; init; }
    public string Sender { get; init; }
    public string Recipient { get; init; }
    public string Message { get; init; }
    public string CypherId { get; init; }
    public bool Outbound { get; init; }
}