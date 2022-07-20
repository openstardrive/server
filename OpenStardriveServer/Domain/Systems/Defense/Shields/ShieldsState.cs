using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Defense.Shields;

public record ShieldsState : StandardSystemBaseState
{
    public bool Raised { get; init; }
    public double ModulationFrequency { get; init; }

    public ShieldSectionStrengths SectionStrengths { get; init; } = new();
}

public record ShieldSectionStrengths
{
    public double ForwardPercent { get; init; } = 1;
    public double AftPercent { get; init; } = 1;
    public double PortPercent { get; init; } = 1;
    public double StarboardPercent { get; init; } = 1;
}