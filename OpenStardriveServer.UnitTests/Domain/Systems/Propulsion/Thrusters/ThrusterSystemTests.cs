using NUnit.Framework;
using OpenStardriveServer.Domain.Systems.Propulsion.Thrusters;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Propulsion.Thrusters;

public class ThrusterSystemTests : SystemsTest<ThrustersSystem>
{
    protected override ThrustersSystem CreateClassUnderTest() => new();
    private ThrusterTransformations transformations = new();

    [Test]
    public void When_configuring_thrusters()
    {
        var payload = new ThrusterConfigurationPayload
        {
            Damaged = true,
            Disabled = true
        };
        TestCommand("configure-thrusters", payload,
            transformations.Configure(new ThrustersState(), payload));
    }

    [Test]
    public void When_setting_attitude()
    {
        var payload = new ThrusterAttitudePayload { Yaw = 1, Pitch = 2, Roll = 3 };
        TestCommand("set-thruster-attitude", payload,
            transformations.SetAttitude(new ThrustersState(), payload));
    }
    
    [Test]
    public void When_setting_velocity()
    {
        var payload = new ThrusterVelocityPayload { X = 1, Y = 2, Z = 3 };
        TestCommand("set-thruster-velocity", payload,
            transformations.SetVelocity(new ThrustersState(), payload));
    }
}