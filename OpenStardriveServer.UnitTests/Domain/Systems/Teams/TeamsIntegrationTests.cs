using OpenStardriveServer.Domain.Systems.Teams;
using System;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Teams;

public class TeamsIntegrationTests
{
    [Test]
    public void TeamsUpdate_ShouldHandleCompletePayload()
    {
        // Arrange
        var json = new OpenStardriveServer.Domain.Json();
        var transforms = new TeamsTransforms();
        var system = new TeamsSystem(json, transforms);

        // This is the exact payload structure from the user's request
        var jsonPayload = """
        [
          {
            "id": "team-uuid-1",
            "name": "Alpha Team",
            "type": "damage",
            "simulatorId": "simulator-1",
            "priority": "low",
            "location": {
              "id": "deck-3",
              "name": "Engineering Bay 2",
              "deck": {
                "id": "deck-3", 
                "number": 3,
                "name": "Engineering Deck"
              }
            },
            "orders": "Repair primary power conduits in Engineering Bay 2",
            "officers": [
              {
                "id": "crew-uuid-1",
                "name": "Lieutenant Johnson",
                "position": "Engineer",
                "inventory": [
                  {
                    "id": "tool-1",
                    "name": "Plasma Torch",
                    "count": 1
                  },
                  {
                    "id": "tool-2", 
                    "name": "Tricorder",
                    "count": 1
                  }
                ]
              },
              {
                "id": "crew-uuid-2",
                "name": "Ensign Smith", 
                "position": "Technician",
                "inventory": [
                  {
                    "id": "tool-3",
                    "name": "Repair Kit",
                    "count": 2
                  }
                ]
              }
            ]
          },
          {
            "id": "team-uuid-2",
            "name": "Bravo Team",
            "type": "security",
            "simulatorId": "simulator-1", 
            "priority": "high",
            "location": {
              "id": "deck-1",
              "name": null,
              "deck": {
                "id": "deck-1",
                "number": 1, 
                "name": "Bridge Deck"
              }
            },
            "orders": "Secure bridge area and maintain perimeter",
            "officers": [
              {
                "id": "crew-uuid-3",
                "name": "Commander Davis",
                "position": "Security Chief", 
                "inventory": [
                  {
                    "id": "weapon-1",
                    "name": "Phaser",
                    "count": 1
                  }
                ]
              }
            ]
          },
          {
            "id": "team-uuid-3",
            "name": "Medical Team One",
            "type": "medical",
            "simulatorId": "simulator-1",
            "priority": "medium", 
            "location": null,
            "orders": "Standby for casualties",
            "officers": []
          }
        ]
        """;

        var command = new OpenStardriveServer.Domain.Command
        {
            Type = "teams-update",
            Payload = jsonPayload
        };

        // Act
        var result = system.CommandProcessors["teams-update"](command);

        // Assert
        Assert.That(result.System, Is.EqualTo("teams"));
        Assert.That(result.Type, Is.Not.EqualTo(OpenStardriveServer.Domain.CommandResult.NoChangeType));

        var resultState = json.Deserialize<TeamsState>(result.Payload);
        Assert.That(resultState.Teams.Length, Is.EqualTo(3));

        // Test Alpha Team
        var alphaTeam = resultState.Teams[0];
        Assert.That(alphaTeam.Id, Is.EqualTo("team-uuid-1"));
        Assert.That(alphaTeam.Name, Is.EqualTo("Alpha Team"));
        Assert.That(alphaTeam.Type, Is.EqualTo("damage"));
        Assert.That(alphaTeam.Priority, Is.EqualTo("low"));
        Assert.That(alphaTeam.Location.Name, Is.EqualTo("Engineering Bay 2"));
        Assert.That(alphaTeam.Location.Deck.Number, Is.EqualTo(3));
        Assert.That(alphaTeam.Officers.Length, Is.EqualTo(2));
        Assert.That(alphaTeam.Officers[0].Name, Is.EqualTo("Lieutenant Johnson"));
        Assert.That(alphaTeam.Officers[0].Inventory.Length, Is.EqualTo(2));

        // Test Bravo Team
        var bravoTeam = resultState.Teams[1];
        Assert.That(bravoTeam.Id, Is.EqualTo("team-uuid-2"));
        Assert.That(bravoTeam.Type, Is.EqualTo("security"));
        Assert.That(bravoTeam.Priority, Is.EqualTo("high"));
        Assert.That(bravoTeam.Officers.Length, Is.EqualTo(1));
        Assert.That(bravoTeam.Officers[0].Name, Is.EqualTo("Commander Davis"));

        // Test Medical Team
        var medicalTeam = resultState.Teams[2];
        Assert.That(medicalTeam.Id, Is.EqualTo("team-uuid-3"));
        Assert.That(medicalTeam.Type, Is.EqualTo("medical"));
        Assert.That(medicalTeam.Priority, Is.EqualTo("medium"));
        Assert.That(medicalTeam.Officers.Length, Is.EqualTo(0));
        Assert.That(medicalTeam.Location, Is.Null);
    }
}