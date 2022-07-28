using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Propulsion.Thrusters;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Propulsion.Thrusters;

public class ThrusterTransformsTests : StandardTransformsTest<ThrusterTransforms, ThrustersState>
{
    [TestCase(1, 2, 3, 1, 2, 3)]
    [TestCase(359, 359, 359, 359, 359, 359)]
    [TestCase(360, 361, 362, 0, 1, 2)]
    [TestCase(0, -1, -10, 0, 359, 350)]
    [TestCase(-360, -361, -370, 0, 359, 350)]
    [TestCase(-720, 720, 0, 0, 0, 0)]
    public void When_setting_attitude(int yaw, int pitch, int roll, int expectedYaw, int expectedPitch, int expectedRoll)
    {
        var payload = new ThrusterAttitudePayload { Yaw = yaw, Pitch = pitch, Roll = roll };
        var expected = new ThrustersState
        {
            Attitude = new ThrustersAttitude { Yaw = expectedYaw, Pitch = expectedPitch, Roll = expectedRoll }
        };

        var result = ClassUnderTest.SetAttitude(new ThrustersState(), payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.StateChanged));
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }

    [Test]
    public void When_setting_attitude_but_system_disabled()
    {
        var payload = new ThrusterAttitudePayload { Yaw = 1, Pitch = 2, Roll = 3 };
        var existingState = new ThrustersState { Disabled = true };
        
        var result = ClassUnderTest.SetAttitude(existingState, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo(StandardSystemBaseState.DisabledError));
    }
    
    [Test]
    public void When_setting_attitude_but_system_damaged()
    {
        var payload = new ThrusterAttitudePayload { Yaw = 1, Pitch = 2, Roll = 3 };
        var existingState = new ThrustersState { Damaged = true };
        
        var result = ClassUnderTest.SetAttitude(existingState, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo(StandardSystemBaseState.DamagedError));
    }
    
    [Test]
    public void When_setting_velocity()
    {
        var payload = new ThrusterVelocityPayload { X = 11, Y = 22, Z = 33 };
        var expected = new ThrustersState
        {
            Velocity = new ThrusterVelocity { X = 11, Y = 22, Z = 33 }
        };

        var result = ClassUnderTest.SetVelocity(new ThrustersState(), payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.StateChanged));
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }
    
    [Test]
    public void When_setting_velocity_but_system_disabled()
    {
        var payload = new ThrusterVelocityPayload { X = 11, Y = 22, Z = 33 };
        var existingState = new ThrustersState { Disabled = true };
        
        var result = ClassUnderTest.SetVelocity(existingState, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo(StandardSystemBaseState.DisabledError));
    }
    
    [Test]
    public void When_setting_velocity_but_system_damaged()
    {
        var payload = new ThrusterVelocityPayload { X = 11, Y = 22, Z = 33 };
        var existingState = new ThrustersState { Damaged = true };
        
        var result = ClassUnderTest.SetVelocity(existingState, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo(StandardSystemBaseState.DamagedError));
    }
    
    [Test]
    public void When_setting_current_power_for_the_system()
    {
        TestStandardCurrentPower(new ThrustersState());
    }

    [Test]
    public void When_setting_required_power_for_the_system()
    {
        TestStandardRequiredPower(new ThrustersState());
    }

    [Test]
    public void When_setting_damaged()
    {
        TestStandardDamaged(new ThrustersState());
    }
    
    [Test]
    public void When_setting_disabled()
    {
        TestStandardDisabled(new ThrustersState());
    }
}