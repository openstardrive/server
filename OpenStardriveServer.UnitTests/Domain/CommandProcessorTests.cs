using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using OpenStardriveServer.Domain;
using OpenStardriveServer.Domain.Systems;

namespace OpenStardriveServer.UnitTests.Domain;

public class CommandProcessorTests : WithAnAutomocked<CommandProcessor>
{
    [Test]
    public void When_processing_a_single_command_that_has_multiple_processors_and_one_throws()
    {
        var command = new Command { Type = "test-command" };
        var processors = new Dictionary<string, List<Func<Command, CommandResult>>>
        {
            ["test-command"] = new()
            {
                c => CommandResult.NoChange(c, "system-a"),
                c => throw new Exception("test exception"),
                c => CommandResult.Error(c, "system-b", "test-error")
            }
        };
        GetMock<ISystemsRegistry>().Setup(x => x.GetAllProcessors()).Returns(processors);

        var result = ClassUnderTest.Process(command).ToList();
            
        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result[0].Type, Is.EqualTo(CommandResult.NoChangeType));
        Assert.That(result[0].System, Is.EqualTo("system-a"));
        Assert.That(result[1].Type, Is.EqualTo(CommandResult.ErrorType));
        Assert.That(result[1].System, Is.EqualTo("unknown"));
        Assert.That(result[2].Type, Is.EqualTo(CommandResult.ErrorType));
        Assert.That(result[2].System, Is.EqualTo("system-b"));
    }
        
    [Test]
    public void When_processing_a_single_command_that_has_no_processors()
    {
        var command = new Command { Type = "test-command" };
        var processors = new Dictionary<string, List<Func<Command, CommandResult>>>();
        GetMock<ISystemsRegistry>().Setup(x => x.GetAllProcessors()).Returns(processors);

        var result = ClassUnderTest.Process(command).ToList();
            
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Type, Is.EqualTo(CommandResult.UnrecognizedCommandType));
    }

    [Test]
    public async Task When_processing_a_batch()
    {
        var cursor = 12;
        var commands = Enumerable.Range(1, 5).Select(x => new Command
        {
            RowId = cursor + x,
            Type = $"test-command-{x}"
        }).ToList();
        var processors = new Dictionary<string, List<Func<Command, CommandResult>>>
        {
            ["test-command-2"] = new()
            {
                c => CommandResult.NoChange(c, "system-a"),
                c => CommandResult.StateChanged(c, "system", "example-state"),
                c => CommandResult.Error(c, "system", "example-message")
            }
        };
        GetMock<ISystemsRegistry>().Setup(x => x.GetAllProcessors()).Returns(processors);
        GetMock<ICommandRepository>().Setup(x => x.LoadPage(cursor, 100)).ReturnsAsync(commands);
            
        ClassUnderTest.SetCursorForTesting(cursor);
        var cursorAfter = await ClassUnderTest.ProcessBatch();

        Assert.That(cursorAfter, Is.EqualTo(17));
        GetMock<ICommandResultRepository>().Verify(x => x.Save(Any<CommandResult>()), Times.Exactly(6));
        foreach (var command in commands)
        {
            if (command.Type == "test-command-2")
            {
                AssertCommandResultSavedWithType(command, CommandResult.StateUpdatedType);
                AssertCommandResultSavedWithType(command, CommandResult.ErrorType);
                AssertCommandResultNotSavedWithType(command, CommandResult.NoChangeType);
                AssertCommandResultNotSavedWithType(command, CommandResult.UnrecognizedCommandType);
            }
            else
            {
                AssertCommandResultSavedWithType(command, CommandResult.UnrecognizedCommandType);
            }
        }
    }

    private void AssertCommandResultSavedWithType(Command command, string type)
    {
        GetMock<ICommandResultRepository>().Verify(
            x => x.Save(It.Is<CommandResult>(
                y => y.CommandId == command.CommandId && y.Type == type)));
    }
        
    private void AssertCommandResultNotSavedWithType(Command command, string type)
    {
        GetMock<ICommandResultRepository>().VerifyNever(
            x => x.Save(It.Is<CommandResult>(
                y => y.CommandId == command.CommandId && y.Type == type)));
    }
}