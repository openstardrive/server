using System;

namespace OpenStardriveServer.Domain.Systems.Teams;

public interface ITeamsTransforms
{
    TransformResult<TeamsState> UpdateTeams(TeamsState currentState, Team[] teams);
}

public class TeamsTransforms : ITeamsTransforms
{
    public TransformResult<TeamsState> UpdateTeams(TeamsState currentState, Team[] teams)
    {
        var newState = currentState with 
        { 
            Teams = teams 
        };

        return TransformResult<TeamsState>.StateChanged(newState);
    }
}