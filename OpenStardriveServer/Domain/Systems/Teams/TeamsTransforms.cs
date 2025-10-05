using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenStardriveServer.Domain.Systems.Teams;

public interface ITeamsTransforms
{
    TransformResult<TeamsState> UpdateTeams(TeamsState currentState, Team[] teams);
}

public class TeamsTransforms : ITeamsTransforms
{
    public TransformResult<TeamsState> UpdateTeams(TeamsState currentState, Team[] teams)
    {
        Console.WriteLine($"TeamsTransforms.UpdateTeams called:");
        Console.WriteLine($"  Current teams count: {currentState.Teams.Length}");
        Console.WriteLine($"  New teams count: {teams.Length}");

        if (currentState.Teams.Length > 0)
        {
            Console.WriteLine("  Existing teams:");
            foreach (var team in currentState.Teams)
            {
                Console.WriteLine($"    - {team.Id}: {team.Name}");
            }
        }

        if (teams.Length > 0)
        {
            Console.WriteLine("  New teams:");
            foreach (var team in teams)
            {
                Console.WriteLine($"    - {team.Id}: {team.Name}");
            }
        }

        // Merge teams: update existing teams by ID, add new teams
        var mergedTeams = new List<Team>(currentState.Teams);

        foreach (var newTeam in teams)
        {
            var existingIndex = mergedTeams.FindIndex(t => t.Id == newTeam.Id);
            if (existingIndex >= 0)
            {
                // Update existing team
                mergedTeams[existingIndex] = newTeam;
                Console.WriteLine($"  Updated existing team: {newTeam.Id}");
            }
            else
            {
                // Add new team
                mergedTeams.Add(newTeam);
                Console.WriteLine($"  Added new team: {newTeam.Id}");
            }
        }

        var newState = currentState with
        {
            Teams = mergedTeams.ToArray()
        };

        Console.WriteLine($"  Final teams count: {newState.Teams.Length}");
        return TransformResult<TeamsState>.StateChanged(newState);
    }
}