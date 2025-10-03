using System;
using System.Collections.Generic;

namespace OpenStardriveServer.Domain.Systems.Teams;

public class TeamsSystem : SystemBase<TeamsState>
{
    public TeamsSystem(IJson json, ITeamsTransforms transforms) : base(json)
    {
        SystemName = "teams";
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["teams-update"] = (c) => Update(c, transforms.UpdateTeams(state, Payload<Team[]>(c)))
        };
    }
}