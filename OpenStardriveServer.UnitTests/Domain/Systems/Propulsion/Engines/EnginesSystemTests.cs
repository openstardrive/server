using NUnit.Framework;
using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems.Propulsion.Engines;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Propulsion.Engines
{
    public class EnginesSystemTests : SystemsTest<EnginesSystem>
    {
        protected override EnginesSystem CreateClassUnderTest() => new("testing", EnginesStateDefaults.Testing);

        private readonly StandardSystemBaseStateTransformations<EnginesState> standardTransformations = new();
        private readonly EnginesTransformations transformations = new();
        
        [Test]
        public void When_setting_power()
        {
            var payload = new SystemPowerPayload { CurrentPower = 3 };
            TestCommand("set-testing-engines-power", payload, standardTransformations.SetCurrentPower(EnginesStateDefaults.Testing, payload));
        }
    
        [Test]
        public void When_setting_damaged()
        {
            var payload = new SystemDamagePayload { Damaged = true };
            TestCommand("set-testing-engines-damaged", payload, standardTransformations.SetDamage(EnginesStateDefaults.Testing, payload));
        }
    
        [Test]
        public void When_setting_disabled()
        {
            var payload = new SystemDisabledPayload { Disabled = true };
            TestCommand("set-testing-engines-disabled", payload, standardTransformations.SetDisabled(EnginesStateDefaults.Testing, payload));
        }
        
        [Test]
        public void When_setting_speed()
        {
            var payload = new SetSpeedPayload { Speed = 3 };
            TestCommand("set-testing-engines-speed", payload,
                transformations.SetSpeed(EnginesStateDefaults.Testing, payload));
        }

        [Test]
        public void When_the_chronometer()
        {
            var payload = new ChronometerPayload { ElapsedMilliseconds = 1000 };
            TestCommand(ChronometerCommand.Type, payload,
                transformations.UpdateHeat(EnginesStateDefaults.Testing, payload));
        }
        
        [Test]
        public void When_configuring()
        {
            var payload = new EnginesConfigurationPayload();
            TestCommand("configure-testing-engines", payload,
                transformations.Configure(EnginesStateDefaults.Testing, payload));
        }
    }
}