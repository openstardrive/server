namespace OpenStardriveServer.Domain.Systems.Comms.ShortRange;

public record SetCurrentFrequencyPayload
{
    public double Frequency { get; set; }
}