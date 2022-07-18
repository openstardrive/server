using NUnit.Framework;
using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Propulsion.Engines;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Propulsion.Engines
{
    public class EnginesTransformationsTests
    {
        private readonly EnginesTransformations classUnderTest = new EnginesTransformations();
        
        [Test]
        public void When_setting_speed_successfully()
        {
            var payload = new SetSpeedPayload { Speed = 3 };
            var expected = EnginesStateDefaults.Testing with { CurrentSpeed = 3 };

            var result = classUnderTest.SetSpeed(EnginesStateDefaults.Testing, payload);
            
            Assert.That(result.ResultType, Is.EqualTo(TransformResultType.StateChanged));
            Assert.That(result.NewState.Value.CurrentSpeed, Is.EqualTo(payload.Speed));
            Assert.That(result.NewState.Value, Is.EqualTo(expected));
        }

        [Test]
        public void When_setting_speed_and_it_is_already_at_that_speed()
        {
            var state = EnginesStateDefaults.Testing with { CurrentSpeed = 2 };
            var payload = new SetSpeedPayload { Speed = 2 };
            
            var result = classUnderTest.SetSpeed(state, payload);
            
            Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));
        }

        [Test]
        public void When_setting_speed_but_insufficient_power()
        {
            var state = EnginesStateDefaults.Testing with { CurrentPower = 0, RequiredPower = 1 };

            var payload = new SetSpeedPayload { Speed = 3 };

            var result = classUnderTest.SetSpeed(state, payload);
            
            Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
            Assert.That(result.ErrorMessage, Is.EqualTo(state.HasInsufficientPower().Value));
        }
        
        [Test]
        public void When_setting_speed_but_system_is_damaged()
        {
            var state = EnginesStateDefaults.Testing with { Damaged = true };

            var payload = new SetSpeedPayload { Speed = 3 };

            var result = classUnderTest.SetSpeed(state, payload);
            
            Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
            Assert.That(result.ErrorMessage, Is.EqualTo(state.IsDamaged().Value));
        }

        [Test]
        public void When_setting_speed_but_system_is_disabled()
        {
            var state = EnginesStateDefaults.Testing with { Disabled = true };

            var payload = new SetSpeedPayload { Speed = 3 };

            var result = classUnderTest.SetSpeed(state, payload);
            
            Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
            Assert.That(result.ErrorMessage, Is.EqualTo(state.IsDisabled().Value));
        }

        [TestCase(0, 10, 300_000, 10_000)]
        [TestCase(0, 10, 300_001, 10_000)]
        [TestCase(0, 10, 150_000, 5_000)]
        [TestCase(2_000, 10, 60_000, 4_000)]
        [TestCase(0, 8, 150_000, 4_000)]
        [TestCase(0, 6, 150_000, 3_000)]
        [TestCase(0, 6, 300_000, 6_000)]
        [TestCase(0, 6, 340_000, 6_800)]
        [TestCase(0, 6, 345_000, 6_900)]
        [TestCase(0, 6, 350_000, 7_000)]
        [TestCase(0, 6, 350_001, 7_000)]
        [TestCase(0, 6, 355_000, 7_000)]
        [TestCase(0, 6, 400_000, 7_000)]
        [TestCase(0, 1, 60_000, 200)]
        [TestCase(0, 1, 300_000, 1_000)]
        [TestCase(0, 1, 360_000, 1_200)]
        [TestCase(0, 1, 600_000, 2_000)]
        [TestCase(0, 1, 660_000, 2_000)]
        [TestCase(0, 2, 60_000, 400)]
        [TestCase(0, 2, 300_000, 2_000)]
        [TestCase(0, 2, 360_000, 2_333)]
        public void When_heating_up(int currentHeat, int currentSpeed, int elapsedMilliseconds, int expectedHeat)
        {
            var state = EnginesStateDefaults.Testing with { CurrentHeat = currentHeat, CurrentSpeed = currentSpeed };
            var payload = new ChronometerPayload {ElapsedMilliseconds = elapsedMilliseconds};

            var result = classUnderTest.UpdateHeat(state, payload);
            
            Assert.That(result.NewState.Value.CurrentHeat, Is.EqualTo(expectedHeat));
        }
        
        [TestCase(10_000, 0, 720_000, 0, false)]
        [TestCase(10_000, 0, 720_000, 2_000, true)]
        [TestCase(10_000, 0, 60_000, 9_167, true)]
        [TestCase(10_000, 6, 60_000, 9_667, true)]
        public void When_cooling_down(int currentHeat, int currentSpeed, int elapsedMilliseconds, int expectedHeat, bool isPowered)
        {
            var state = EnginesStateDefaults.Testing with
            {
                CurrentHeat = currentHeat,
                CurrentSpeed = currentSpeed,
                RequiredPower = 1,
                CurrentPower = isPowered ? 1 : 0
            };
            var payload = new ChronometerPayload {ElapsedMilliseconds = elapsedMilliseconds};

            var result = classUnderTest.UpdateHeat(state, payload);
            
            Assert.That(result.NewState.Value.CurrentHeat, Is.EqualTo(expectedHeat));
        }

        [Test]
        public void When_calculating_heat_and_there_is_no_change()
        {
            var state = EnginesStateDefaults.Testing with
            {
                CurrentHeat = EnginesStateDefaults.Testing.HeatConfig.PoweredHeat
            };

            var payload = new ChronometerPayload {ElapsedMilliseconds = 1000};

            var result = classUnderTest.UpdateHeat(state, payload);
            
            Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));
        }
    }
}