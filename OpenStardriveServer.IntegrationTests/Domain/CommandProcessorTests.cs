using System;
using System.Linq;
using OpenStardriveServer.Domain;
using OpenStardriveServer.Domain.Systems.Propulsion.Thrusters;

namespace OpenStardriveServer.IntegrationTests.Domain
{
    public class CommandProcessorTests : WithAServiceLocatedClassUnderTest<CommandProcessor>
    {
        [Test]
        public void When_processing_a_command_end_to_end()
        {
            var command = new Command
            {
                CommandId = Guid.NewGuid(),
                Type = "set-thrusters-attitude",
                Payload = @"{""yaw"": 1, ""pitch"": 20, ""roll"": 300}",
            };
            var result = ClassUnderTest.Process(command).ToList();
            
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Type, Is.EqualTo(CommandResult.StateUpdatedType));
            Assert.That(result[0].CommandId, Is.EqualTo(command.CommandId));
            Assert.That(result[0].System, Is.EqualTo("thrusters"));

            var state = Json.Deserialize<ThrustersState>(result[0].Payload);
            Assert.That(state.Attitude.Yaw, Is.EqualTo(1));
            Assert.That(state.Attitude.Pitch, Is.EqualTo(20));
            Assert.That(state.Attitude.Roll, Is.EqualTo(300));
        }
    }
}