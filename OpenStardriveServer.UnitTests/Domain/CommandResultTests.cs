using System;
using OpenStardriveServer.Domain;

namespace OpenStardriveServer.UnitTests.Domain;

public class CommandResultTests
{
    private Command testCommand = new()
    {
        ClientId = Guid.NewGuid(),
        TimeStamp = DateTimeOffset.Now.AddSeconds(-5)
    };

    private string testSystem = "test-system";
        
    [Test]
    public void When_creating_an_unrecognized_command_result()
    {
        var result = CommandResult.UnrecognizedCommand(testCommand);

        Assert.That(result.CommandId, Is.EqualTo(testCommand.CommandId));
        Assert.That(result.ClientId, Is.EqualTo(testCommand.ClientId));
        Assert.That(result.Type, Is.EqualTo(CommandResult.UnrecognizedCommandType));
        Assert.That(result.System, Is.EqualTo(""));
        Assert.That(result.Payload, Is.EqualTo("null"));
        Assert.That(result.Timestamp, Is.EqualTo(testCommand.TimeStamp));
    }
        
    [Test]
    public void When_creating_a_state_changed_command_result()
    {
        var result = CommandResult.StateChanged(testCommand, testSystem, new { data = 123 });

        Assert.That(result.CommandId, Is.EqualTo(testCommand.CommandId));
        Assert.That(result.ClientId, Is.EqualTo(testCommand.ClientId));
        Assert.That(result.Type, Is.EqualTo(CommandResult.StateUpdatedType));
        Assert.That(result.System, Is.EqualTo(testSystem));
        Assert.That(result.Payload, Is.EqualTo("{\"data\":123}"));
        Assert.That(result.Timestamp, Is.EqualTo(testCommand.TimeStamp));
    }
        
    [Test]
    public void When_creating_a_no_change_command_result()
    {
        var result = CommandResult.NoChange(testCommand, testSystem);

        Assert.That(result.CommandId, Is.EqualTo(testCommand.CommandId));
        Assert.That(result.ClientId, Is.EqualTo(testCommand.ClientId));
        Assert.That(result.Type, Is.EqualTo(CommandResult.NoChangeType));
        Assert.That(result.System, Is.EqualTo(testSystem));
        Assert.That(result.Payload, Is.EqualTo("null"));
        Assert.That(result.Timestamp, Is.EqualTo(testCommand.TimeStamp));
    }
    
    [Test]
    public void When_creating_an_error_command_result()
    {
        var result = CommandResult.Error(testCommand, testSystem, "test error");

        Assert.That(result.CommandId, Is.EqualTo(testCommand.CommandId));
        Assert.That(result.ClientId, Is.EqualTo(testCommand.ClientId));
        Assert.That(result.Type, Is.EqualTo(CommandResult.ErrorType));
        Assert.That(result.System, Is.EqualTo(testSystem));
        Assert.That(result.Payload, Is.EqualTo("\"test error\""));
        Assert.That(result.Timestamp, Is.EqualTo(testCommand.TimeStamp));
    }
}