using NUnit.Framework;
using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Propulsion.Thrusters;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Propulsion.Thrusters;

public class ThrusterTransformationsTests
{
    private readonly ThrusterTransformations classUnderTest = new();

    [TestCase(true, true)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(false, false)]
    public void When_configuring_thrusters(bool disabled, bool damaged)
    {
        var payload = new ThrusterConfigurationPayload
        {
            Disabled = disabled,
            Damaged = damaged
        };

        var result = classUnderTest.Configure(new ThrustersState(), payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.StateChanged));
        Assert.That(result.NewState.Value.Disabled, Is.EqualTo(disabled));
        Assert.That(result.NewState.Value.Damaged, Is.EqualTo(damaged));
    }
    
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

        var result = classUnderTest.SetAttitude(new ThrustersState(), payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.StateChanged));
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }

    [Test]
    public void When_setting_attitude_but_system_disabled()
    {
        var payload = new ThrusterAttitudePayload { Yaw = 1, Pitch = 2, Roll = 3 };
        var existingState = new ThrustersState { Disabled = true };
        
        var result = classUnderTest.SetAttitude(existingState, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo(SystemBaseState.DisabledError));
    }
    
    [Test]
    public void When_setting_attitude_but_system_damaged()
    {
        var payload = new ThrusterAttitudePayload { Yaw = 1, Pitch = 2, Roll = 3 };
        var existingState = new ThrustersState { Damaged = true };
        
        var result = classUnderTest.SetAttitude(existingState, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo(SystemBaseState.DamagedError));
    }
    
    [Test]
    public void When_setting_velocity()
    {
        var payload = new ThrusterVelocityPayload { X = 11, Y = 22, Z = 33 };
        var expected = new ThrustersState
        {
            Velocity = new ThrusterVelocity { X = 11, Y = 22, Z = 33 }
        };

        var result = classUnderTest.SetVelocity(new ThrustersState(), payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.StateChanged));
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }
    
    [Test]
    public void When_setting_velocity_but_system_disabled()
    {
        var payload = new ThrusterVelocityPayload { X = 11, Y = 22, Z = 33 };
        var existingState = new ThrustersState { Disabled = true };
        
        var result = classUnderTest.SetVelocity(existingState, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo(SystemBaseState.DisabledError));
    }
    
    [Test]
    public void When_setting_velocity_but_system_damaged()
    {
        var payload = new ThrusterVelocityPayload { X = 11, Y = 22, Z = 33 };
        var existingState = new ThrustersState { Damaged = true };
        
        var result = classUnderTest.SetVelocity(existingState, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo(SystemBaseState.DamagedError));
    }
}