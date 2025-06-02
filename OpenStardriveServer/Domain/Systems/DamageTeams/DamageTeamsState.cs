using System;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.DamageTeams;

public record DamageTeamsState : StandardSystemBaseState
{
    public DamageTeam[] DamageTeams { get; init; } = Array.Empty<DamageTeam>();
}

public record DamageTeam
{
    public string TeamId { get; init; }
    public string Name { get; init; }
    public int MembersCount { get; init; }
    public System system { get; init; }
}

public enum System {
    LifeSupport,
    Weapons,
    Shields,
    Engines,
    Sensors,
    Navigation,
    Cargo,
    MedicalBay,
    Brig,
    HangarBay,
    Reactor,
    Hull
}