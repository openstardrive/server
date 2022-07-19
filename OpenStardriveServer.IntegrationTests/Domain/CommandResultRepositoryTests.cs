using System;
using System.Linq;
using System.Threading.Tasks;
using OpenStardriveServer.Domain;

namespace OpenStardriveServer.IntegrationTests.Domain;

public class CommandResultRepositoryTests : WithAServiceLocatedClassUnderTest<CommandResultRepository>
{
    [Test]
    public async Task Command_results_can_be_round_tripped()
    {
        var command = new Command
        {
            CommandId = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
        };
        var commandResult = CommandResult.StateChanged(command, "test-system", new { data = "abc123" });

        await ClassUnderTest.Save(commandResult);

        var results = await ClassUnderTest.LoadPage();

        var found = results.FirstOrDefault(x => x.CommandResultId == commandResult.CommandResultId);
        Assert.That(found, Is.Not.Null);
        Assert.That(found.Type, Is.EqualTo(commandResult.Type));
        Assert.That(found.CommandId, Is.EqualTo(commandResult.CommandId));
        Assert.That(found.ClientId, Is.EqualTo(commandResult.ClientId));
        Assert.That(found.System, Is.EqualTo(commandResult.System));
        Assert.That(found.Payload, Is.EqualTo(commandResult.Payload));
        Assert.That(found.Timestamp, Is.EqualTo(commandResult.Timestamp));
    }

    [Test]
    public async Task When_loading_a_page_of_command_results()
    {
        var commandResults = Enumerable.Range(0, 20).Select(x =>
            CommandResult.Error(new Command(), $"test-system-{x}", $"error-{x}")).ToList();
            
        foreach (var commandResult in commandResults)
        {
            await ClassUnderTest.Save(commandResult);
        }
            
        var firstCursor = (await ClassUnderTest.LoadPage(0, 1000))
            .Single(x => x.CommandResultId == commandResults[0].CommandResultId).RowId - 1;
            
        var page1 = (await ClassUnderTest.LoadPage(firstCursor, 5)).ToList();
        Assert.That(page1.Count, Is.EqualTo(5));
        for (var i = 0; i < page1.Count; i++)
        {
            Assert.That(page1[i].Payload, Is.EqualTo(commandResults[i].Payload));
        }
            
        var page2 = (await ClassUnderTest.LoadPage(firstCursor + 4, 7)).ToList();
        Assert.That(page2.Count, Is.EqualTo(7));
        for (var i = 0; i < page2.Count; i++)
        {
            Assert.That(page2[i].Payload, Is.EqualTo(commandResults[i + 4].Payload));
        }
    }
}