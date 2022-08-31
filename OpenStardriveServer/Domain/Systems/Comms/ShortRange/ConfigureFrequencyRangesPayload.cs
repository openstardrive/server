namespace OpenStardriveServer.Domain.Systems.Comms.ShortRange;

public record ConfigureFrequencyRangesPayload
{
    public FrequencyRange[] FrequencyRanges { get; init; }
}