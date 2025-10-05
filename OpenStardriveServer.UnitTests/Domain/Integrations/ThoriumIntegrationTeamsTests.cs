using OpenStardriveServer.Domain.Integrations;
using OpenStardriveServer.Domain.Systems.Teams;

namespace OpenStardriveServer.UnitTests.Domain.Integrations;

public class ThoriumIntegrationTeamsTests
{
    [Test]
    public void ThoriumTeamsIntegration_ShouldConvertThoriumFormatToInternalFormat()
    {
        // Arrange
        var json = new OpenStardriveServer.Domain.Json();
        var thoriumIntegration = new ThoriumIntegration(json);

        // This is the actual payload format from real Thorium (wrapped with string officers)
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
                    "priority": "low",
                    "orders": "Do the thing!",
                    "officers": [
                        "03bcc10d-a1af-4284-a824-837eb195a3ac",
                        "f20258fe-5ec2-47e0-985b-f5eda152f4ba"
                    ],
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

        // Verify officers conversion  
        Assert.That(team.Officers.Length, Is.EqualTo(2));
        Assert.That(team.Officers[0].Id, Is.EqualTo("03bcc10d-a1af-4284-a824-837eb195a3ac"));
        Assert.That(team.Officers[0].Name, Is.EqualTo("Unknown")); // String officers only have ID
        Assert.That(team.Officers[0].Position, Is.EqualTo("Officer")); // Default position
        Assert.That(team.Officers[1].Id, Is.EqualTo("f20258fe-5ec2-47e0-985b-f5eda152f4ba"));
        Assert.That(team.Officers[1].Name, Is.EqualTo("Unknown"));
        Assert.That(team.Officers[1].Position, Is.EqualTo("Officer"));
    }

    [Test]
    public void ThoriumTeamsIntegration_ShouldHandleNullLocation()
    {
        // Arrange
        var json = new OpenStardriveServer.Domain.Json();
        var thoriumIntegration = new ThoriumIntegration(json);

        var thoriumPayload = """
        [
            {
                "id": "test-team",
                "class": "Team",
                "simulatorId": "test-sim",
                "type": "damage",
                "name": "Test Team",
                "location": null,
                "priority": "high",
                "orders": "Test orders",
                "officers": [],
                "cleared": false
            }
        ]
        """;

        var command = new OpenStardriveServer.Domain.Command
        {
            Type = "teams-update",
            Payload = thoriumPayload
        };

        // Act
        thoriumIntegration.TranslateCommand(command);

        // Assert
        var convertedTeams = json.Deserialize<Team[]>(command.Payload);
        Assert.That(convertedTeams[0].Location, Is.Null);
    }

    [Test]
    public void ThoriumTeamsIntegration_ShouldHandleDirectArrayFormatWithObjectOfficers()
    {
        // Arrange
        var json = new OpenStardriveServer.Domain.Json();
        var thoriumIntegration = new ThoriumIntegration(json);

        // This is the direct array format with object officers (your format)
        var thoriumPayload = """
        [
            {
                "id": "test-team-direct",
                "class": "Team",
                "simulatorId": "test-sim",
                "type": "damage",
                "name": "Direct Format Team",
                "location": "bridge",
                "priority": "high",
                "orders": "Test direct format",
                "officers": [
                    {
                        "id": "officer-obj-1",
                        "name": "Officer Alpha",
                        "position": "Engineer",
                        "inventory": []
                    },
                    {
                        "id": "officer-obj-2",
                        "name": "Officer Beta",
                        "position": "Security",
                        "inventory": []
                    }
                ],
                "cleared": false
            }
        ]
        """;

        var command = new OpenStardriveServer.Domain.Command
        {
            Type = "teams-update",
            Payload = thoriumPayload
        };

        // Act
        thoriumIntegration.TranslateCommand(command);

        // Assert
        var convertedTeams = json.Deserialize<Team[]>(command.Payload);
        Assert.That(convertedTeams.Length, Is.EqualTo(1));

        var team = convertedTeams[0];
        Assert.That(team.Id, Is.EqualTo("test-team-direct"));
        Assert.That(team.Name, Is.EqualTo("Direct Format Team"));

        // Verify object officers preserve names and positions
        Assert.That(team.Officers.Length, Is.EqualTo(2));
        Assert.That(team.Officers[0].Id, Is.EqualTo("officer-obj-1"));
        Assert.That(team.Officers[0].Name, Is.EqualTo("Officer Alpha"));
        Assert.That(team.Officers[0].Position, Is.EqualTo("Engineer"));
        Assert.That(team.Officers[1].Id, Is.EqualTo("officer-obj-2"));
        Assert.That(team.Officers[1].Name, Is.EqualTo("Officer Beta"));
        Assert.That(team.Officers[1].Position, Is.EqualTo("Security"));
    }
}