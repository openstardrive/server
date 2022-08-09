using System;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Defense.EnergyBeam;

public record EnergyBeamState : StandardSystemBaseState
{
    public EnergyBeamBank[] Banks { get; init; } = {
        new() { Name = "Forward", PercentCharged = 0, Frequency = 218.67, ArcDegrees = 15 },
        new() { Name = "Aft", PercentCharged = 0, Frequency = 218.67, ArcDegrees = 15 }
    };
    
    public FiredEnergyBeam LastFiredEnergyBeam { get; init; }
}

public record EnergyBeamBank
{
    public string Name { get; init; }
    public double PercentCharged { get; init; }
    public double Frequency { get; init; }
    public double ArcDegrees { get; init; }
}

public record FiredEnergyBeam
{
    public string Name { get; init; }
    public double PercentDischarged { get; init; }
    public DateTimeOffset FiredAt { get; set; }
    public double Frequency { get; init; }
    public double ArcDegrees { get; init; }
    public string Target { get; set; }
}