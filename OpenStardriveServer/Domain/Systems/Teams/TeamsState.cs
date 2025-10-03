using System;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Teams;

public record TeamsState : StandardSystemBaseState
{
    public Team[] Teams { get; init; } = Array.Empty<Team>();
}