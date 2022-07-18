using NUnit.Framework;
using OpenStardriveServer.Domain;
using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems.Propulsion.Engines;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Propulsion.Engines
{
    public class EnginesSystemTests : SystemsTest<EnginesSystem>
    {
        protected override EnginesSystem CreateClassUnderTest() => new("testing", EnginesStateDefaults.Testing);
        
        [Test]
        public void When_setting_speed()
        {
            var payload = new SetSpeedPayload { Speed = 3 };
            var command = new Command
            {
                Payload = Json.Serialize(payload)
            };
            var expected = new EnginesTransformations()
                .SetSpeed(EnginesStateDefaults.Testing, payload)
                .ToCommandResult(command, ClassUnderTest.SystemName);
            
            var result = ClassUnderTest.CommandProcessors["set-testing-speed"](command);
            
            AssertCommandResult(result, expected);
        }

        [Test]
        public void When_the_chronometer()
        {
            var payload = new ChronometerPayload { ElapsedMilliseconds = 1000 };
            var command = new Command
            {
                Payload = Json.Serialize(payload)
            };
            var expected = new EnginesTransformations()
                .UpdateHeat(EnginesStateDefaults.Testing, payload)
                .ToCommandResult(command, ClassUnderTest.SystemName);

            var result = ClassUnderTest.CommandProcessors[ChronometerCommand.Type](command);
            
            AssertCommandResult(result, expected);
        }
    }
}