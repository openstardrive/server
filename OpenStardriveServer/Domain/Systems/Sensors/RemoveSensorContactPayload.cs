namespace OpenStardriveServer.Domain.Systems.Sensors;

public record RemoveSensorContactPayload
{
    public string ContactId { get; init; }
}