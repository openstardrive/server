using OpenStardriveServer.Domain.Integrations;
using OpenStardriveServer.Domain.Systems.Teams;
using System;

namespace OpenStardriveServer.UnitTests.Domain.Integrations;

public class ThoriumIntegrationTeamsTests
{
    [Test]
    public void ThoriumTeamsIntegration_ShouldConvertEnhancedThoriumFormatToInternalFormat()
    {
        // Arrange
        var json = new OpenStardriveServer.Domain.Json();
        var thoriumIntegration = new ThoriumIntegration(json);

        // This is the enhanced payload format with wrapped teams and detailed officer information
        var thoriumPayload = """
        {
            "teams": [
                {
                    "id": "0749999b-b858-4670-8b4e-83cab95c8cd9",
                    "class": "Team",
                    "simulatorId": "52bc8751-5e5f-4de1-b49a-2da67a1642e1",
                    "type": "security",
                    "name": "Testing",
                    "location": "13e46b9a-df22-4838-a801-502432d62a1d",
                    "locationName": "Main Bridge",
                    "priority": "low",
                    "orders": "Do the thing!",
                    "officers": [
                        {
                            "id": "03bcc10d-a1af-4284-a824-837eb195a3ac",
                            "firstName": "Alan",
                            "lastName": "Fitzpatrick",
                            "fullName": "Alan Fitzpatrick",
                            "rank": "Master-at-arms",
                            "position": "Security Officer",
                            "shift": "Alpha",
                            "inventory": []
                        },
                        {
                            "id": "f20258fe-5ec2-47e0-985b-f5eda152f4ba",
                            "firstName": "Sarah",
                            "lastName": "Connor",
                            "fullName": "Sarah Connor",
                            "rank": "Lieutenant",
                            "position": "Security Chief",
                            "shift": "Alpha",
                            "inventory": []
                        }
                    ],
                    "officerCount": 2,
                    "cleared": false
                }
            ]
        }
        """;

        var command = new OpenStardriveServer.Domain.Command
        {
            Type = "teams-update",
            Payload = thoriumPayload
        };

        // Act
        thoriumIntegration.TranslateCommand(command);

        // Assert - verify the payload was converted to our internal format
        var convertedTeams = json.Deserialize<Team[]>(command.Payload);

        Assert.That(convertedTeams.Length, Is.EqualTo(1));

        var team = convertedTeams[0];
        Assert.That(team.Id, Is.EqualTo("0749999b-b858-4670-8b4e-83cab95c8cd9"));
        Assert.That(team.Name, Is.EqualTo("Testing"));
        Assert.That(team.Type, Is.EqualTo("security"));
        Assert.That(team.SimulatorId, Is.EqualTo("52bc8751-5e5f-4de1-b49a-2da67a1642e1"));
        Assert.That(team.Priority, Is.EqualTo("low"));
        Assert.That(team.Orders, Is.EqualTo("Do the thing!"));

        // Verify location conversion
        Assert.That(team.Location, Is.Not.Null);
        Assert.That(team.Location.Id, Is.EqualTo("13e46b9a-df22-4838-a801-502432d62a1d"));
        Assert.That(team.Location.Name, Is.EqualTo("Main Bridge"));

        // Verify enhanced officers conversion  
        Assert.That(team.Officers.Length, Is.EqualTo(2));
        
        Assert.That(team.Officers[0].Id, Is.EqualTo("03bcc10d-a1af-4284-a824-837eb195a3ac"));
        Assert.That(team.Officers[0].Name, Is.EqualTo("Master-at-arms Alan Fitzpatrick")); // Enhanced officer with rank
        Assert.That(team.Officers[0].Position, Is.EqualTo("Security Officer"));
        
        Assert.That(team.Officers[1].Id, Is.EqualTo("f20258fe-5ec2-47e0-985b-f5eda152f4ba"));
        Assert.That(team.Officers[1].Name, Is.EqualTo("Lieutenant Sarah Connor")); // Enhanced officer with rank
        Assert.That(team.Officers[1].Position, Is.EqualTo("Security Chief"));
    }
}