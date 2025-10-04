using OpenStardriveServer.Domain.Systems.Teams;
using System;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Teams;

public class TeamsSystemTests
{
    [Test]
    public void TeamsUpdate_ShouldUpdateTeamsState()
    {
        // Arrange
        var json = new OpenStardriveServer.Domain.Json();
        var transforms = new TeamsTransforms();
        var system = new TeamsSystem(json, transforms);

        var testTeams = new[]
        {
            new Team
            {
                Id = "team-uuid-1",
                Name = "Alpha Team",
                Type = "damage",
                SimulatorId = "simulator-1",
                Priority = "low",
                Location = new TeamLocation
                {
                    Id = "deck-3",
                    Name = "Engineering Bay 2",
                    Deck = new Deck
                    {
                        Id = "deck-3",
                        Number = 3,
                        Name = "Engineering Deck"
                    }
                },
                Orders = "Repair primary power conduits in Engineering Bay 2",
                Officers = new[]
                {
                    new Officer
                    {
                        Id = "crew-uuid-1",
                        Name = "Lieutenant Johnson",
                        Position = "Engineer",
                        Inventory = new[]
                        {
                            new InventoryItem { Id = "tool-1", Name = "Plasma Torch", Count = 1 },
                            new InventoryItem { Id = "tool-2", Name = "Tricorder", Count = 1 }
                        }
                    }
                }
            }
        };

        var payload = json.Serialize(testTeams);
        var command = new OpenStardriveServer.Domain.Command
        {
            Type = "teams-update",
            Payload = payload
        };

        // Act
        var result = system.CommandProcessors["teams-update"](command);

        // Assert
        Assert.That(result.System, Is.EqualTo("teams"));
        Assert.That(result.Type, Is.Not.EqualTo(OpenStardriveServer.Domain.CommandResult.NoChangeType));

        var resultState = json.Deserialize<TeamsState>(result.Payload);
        Assert.That(resultState.Teams.Length, Is.EqualTo(1));
        Assert.That(resultState.Teams[0].Id, Is.EqualTo("team-uuid-1"));
        Assert.That(resultState.Teams[0].Name, Is.EqualTo("Alpha Team"));
        Assert.That(resultState.Teams[0].Type, Is.EqualTo("damage"));
    }
}