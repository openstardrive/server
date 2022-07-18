using NUnit.Framework;
using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems.Propulsion.Engines;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Propulsion.Engines
{
    public class EnginesSystemTests : SystemsTest<EnginesSystem>
    {
        protected override EnginesSystem CreateClassUnderTest() => new("testing", EnginesStateDefaults.Testing);

        private readonly EnginesTransformations transformations = new(); 
        
        [Test]
        public void When_setting_speed()
        {
            var payload = new SetSpeedPayload { Speed = 3 };
            TestCommand("set-testing-speed", payload,
                transformations.SetSpeed(EnginesStateDefaults.Testing, payload));
        }

        [Test]
        public void When_the_chronometer()
        {
            var payload = new ChronometerPayload { ElapsedMilliseconds = 1000 };
            TestCommand(ChronometerCommand.Type, payload,
                transformations.UpdateHeat(EnginesStateDefaults.Testing, payload));
        }
    }
}