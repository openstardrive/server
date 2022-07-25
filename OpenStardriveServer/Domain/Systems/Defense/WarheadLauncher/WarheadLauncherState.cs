using System;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Defense.WarheadLauncher;

public record WarheadLauncherState : StandardSystemBaseState
{
    public int NumberOfLaunchers { get; init; } = 1;
    public string[] Loaded { get; init; } = Array.Empty<string>();
    public WarheadGroup[] Inventory { get; init; } = {
        new() { Kind = "torpedo", Number = 10 }
    };
    public FiredWarhead LastFiredWarhead { get; init; }
}

public record WarheadGroup
{
    public string Kind { get; init; }
    public int Number { get; init; }
}

public record FiredWarhead
{
    public string Kind { get; init; }
    public DateTimeOffset FiredAt { get; set; }
    public string Target { get; set; }
}