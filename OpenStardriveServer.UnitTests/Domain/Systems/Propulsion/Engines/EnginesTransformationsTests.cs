using NUnit.Framework;
using OpenStardriveServer.Domain;
using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Propulsion.Engines;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Propulsion.Engines
{
    public class EnginesTransformationsTests
    {
        private EnginesTransformations classUnderTest = new EnginesTransformations();
        
        [Test]
        public void When_setting_speed_successfully()
        {
            var payload = new SetSpeedPayload { Speed = 3 };
            var expected = EnginesStateDefaults.Ftl;
            expected.CurrentSpeed = 3;
            
            var result = classUnderTest.SetSpeed(EnginesStateDefaults.Ftl, payload);
            
            Assert.That(result.ResultType, Is.EqualTo(TransformResultType.StateChanged));
            Assert.That(result.NewState.Value.CurrentSpeed, Is.EqualTo(payload.Speed));
            AssertJsonMatch(result.NewState.Value, expected);
        }

        [Test]
        public void When_setting_speed_but_insufficient_power()
        {
            var state = EnginesStateDefaults.Sublight;
            state.CurrentPower = 0;
            state.RequiredPower = 1;
            
            var payload = new SetSpeedPayload { Speed = 3 };

            var result = classUnderTest.SetSpeed(state, payload);
            
            Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
            Assert.That(result.ErrorMessage, Is.EqualTo(state.HasInsufficientPower().Value));
        }

        [Test]
        public void When_calculating_heat_and_there_is_no_change()
        {
            var state = EnginesStateDefaults.Sublight;
            state.CurrentHeat = state.HeatConfig.PoweredHeat;

            var payload = new ChronometerPayload {ElapsedMilliseconds = 1000};

            var result = classUnderTest.UpdateHeat(state, payload);
            
            Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));
        }

        private void AssertJsonMatch(object actual, object expected)
        {
            Assert.That(Json.Serialize(actual), Is.EqualTo(Json.Serialize(expected)));
        }
    }
}