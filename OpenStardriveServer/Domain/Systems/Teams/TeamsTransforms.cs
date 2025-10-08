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

        // Determine update strategy: merge vs replace
        bool shouldReplace = ShouldReplaceTeams(currentState.Teams, teams);

        if (shouldReplace)
        {
            Console.WriteLine("  Strategy: REPLACE (likely dev-client deletion)");
            var replacedState = currentState with { Teams = teams };
            Console.WriteLine($"  Final teams count: {replacedState.Teams.Length}");
            return TransformResult<TeamsState>.StateChanged(replacedState);
        }

        Console.WriteLine("  Strategy: MERGE (likely external integration)");

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

    private bool ShouldReplaceTeams(Team[] currentTeams, Team[] newTeams)
    {
        // If no current teams, it's always a replacement (initial state)
        if (currentTeams.Length == 0)
            return true;

        // If no new teams, it's a clear-all operation (replacement)
        if (newTeams.Length == 0)
            return true;

        // Heuristic: If all new teams already exist in current teams,
        // and we have fewer new teams than current teams,
        // this is likely a deletion/replacement operation (e.g., dev-client)
        if (newTeams.Length < currentTeams.Length)
        {
            var allNewTeamsExist = newTeams.All(newTeam =>
                currentTeams.Any(currentTeam => currentTeam.Id == newTeam.Id));

            if (allNewTeamsExist)
            {
                Console.WriteLine($"    Replace heuristic: All {newTeams.Length} new teams exist in current {currentTeams.Length} teams");
                return true;
            }
        }

        // If new teams contain teams not in current teams, it's likely a merge operation (e.g., Thorium)
        var hasNewTeams = newTeams.Any(newTeam =>
            !currentTeams.Any(currentTeam => currentTeam.Id == newTeam.Id));

        if (hasNewTeams)
        {
            Console.WriteLine($"    Merge heuristic: New teams contain teams not in current state");
            return false;
        }

        // Default to merge for safety
        Console.WriteLine($"    Default heuristic: Using merge strategy");
        return false;
    }
}