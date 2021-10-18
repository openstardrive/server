using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenStardriveServer.Domain;

namespace OpenStardriveServer.IntegrationTests.Domain
{
    public class CommandRepositoryTests : WithAServiceLocatedClassUnderTest<CommandRepository>
    {
        [Test]
        public async Task Commands_can_be_round_tripped()
        {
            var command = new Command
            {
                CommandId = Guid.NewGuid(),
                ClientId = Guid.NewGuid(),
                Type = "round-trip-test",
                Payload = "any payload",
                TimeStamp = DateTimeOffset.UtcNow.AddMinutes(-5)
            };
            await ClassUnderTest.Save(command);

            var results = await ClassUnderTest.LoadPage();
            
            var found = results.FirstOrDefault(x => x.CommandId == command.CommandId);
            Assert.That(found, Is.Not.Null);
            Assert.That(found.ClientId, Is.EqualTo(command.ClientId));
            Assert.That(found.Type, Is.EqualTo(command.Type));
            Assert.That(found.Payload, Is.EqualTo(command.Payload));
            Assert.That(found.TimeStamp, Is.EqualTo(command.TimeStamp));
        }

        [Test]
        public async Task When_loading_a_page_of_commands()
        {
            var commands = Enumerable.Range(0, 20).Select(x =>
            {
                var guid = Guid.NewGuid();
                return new Command {CommandId = guid, ClientId = guid, Type = guid.ToString(), Payload = x.ToString()};
            }).ToList();

            foreach (var command in commands)
            {
                await ClassUnderTest.Save(command);
            }

            var firstCursor = (await ClassUnderTest.LoadPage(0, 1000))
                .Single(x => x.CommandId == commands[0].CommandId).RowId - 1;

            var page1 = (await ClassUnderTest.LoadPage(firstCursor, 5)).ToList();
            Assert.That(page1.Count, Is.EqualTo(5));
            for (var i = 0; i < page1.Count; i++)
            {
                Assert.That(page1[i].Payload, Is.EqualTo(commands[i].Payload));
            }
            
            var page2 = (await ClassUnderTest.LoadPage(firstCursor + 4, 7)).ToList();
            Assert.That(page2.Count, Is.EqualTo(7));
            for (var i = 0; i < page2.Count; i++)
            {
                Assert.That(page2[i].Payload, Is.EqualTo(commands[i + 4].Payload));
            }
        }
    }
}