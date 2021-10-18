using NUnit.Framework;
using OpenStardriveServer.Domain;
using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems.Propulsion.Engines;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Propulsion.Engines
{
    public class EnginesSystemTests : SystemsTest<EnginesSystem>
    {
        protected override EnginesSystem CreateClassUnderTest() => new EnginesSystem("ftl", EnginesStateDefaults.Ftl);
        
        [Test]
        public void When_setting_speed()
        {
            var payload = new SetSpeedPayload { Speed = 3 };
            var command = new Command
            {
                Payload = Json.Serialize(payload)
            };
            var expected = new EnginesTransformations()
                .SetSpeed(EnginesStateDefaults.Ftl, payload)
                .ToCommandResult(command, ClassUnderTest.SystemName);
            
            var result = ClassUnderTest.CommandProcessors["set-ftl-speed"](command);
            
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
                .UpdateHeat(EnginesStateDefaults.Ftl, payload)
                .ToCommandResult(command, ClassUnderTest.SystemName);

            var result = ClassUnderTest.CommandProcessors["chronometer"](command);
            
            AssertCommandResult(result, expected);
        }
    }
}