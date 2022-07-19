using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Defense.Shields;

public record ShieldsState : StandardSystemBaseState
{
    public bool Raised { get; init; }
    public double ModulationFrequency { get; init; }

    public ShieldSectionStrength[] SectionStrengths { get; init; } = {
        new() { Section = "Forward", PercentStrength = 1 },
        new() { Section = "Aft", PercentStrength = 1 },
        new() { Section = "Port", PercentStrength = 1 },
        new() { Section = "Starboard", PercentStrength = 1 }
    };
}

public record ShieldSectionStrength
{
    public string Section { get; init; }
    public decimal PercentStrength { get; init; }
}