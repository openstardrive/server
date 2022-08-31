using System;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Comms.ShortRange;

public record ShortRangeState : StandardSystemBaseState
{
    public FrequencyRange[] FrequencyRanges { get; init; } = Array.Empty<FrequencyRange>();
    public Signal[] ActiveSignals { get; init; } = Array.Empty<Signal>();
    public double CurrentFrequency { get; init; }
    public bool IsBroadcasting { get; init; }
}

public record FrequencyRange
{
    public string Name { get; init; }
    public double Min { get; init; }
    public double Max { get; init; }
}

public record Signal
{
    public double Frequency { get; init; }
    public string Name { get; init; }
}