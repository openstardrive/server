using OpenStardriveServer.Domain.Systems.Propulsion.Thrusters;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Propulsion.Thrusters;

public class ThrusterSystemTests : SystemsTest<ThrustersSystem>
{
    protected override ThrustersSystem CreateClassUnderTest() => new();
    private ThrusterTransformations transformations = new();

    [Test]
    public void When_setting_power()
    {
        var payload = new SystemPowerPayload { CurrentPower = 3 };
        TestCommand("set-thrusters-power", payload, transformations.SetCurrentPower(new ThrustersState(), payload));
    }
    
    [Test]
    public void When_setting_damaged()
    {
        var payload = new SystemDamagePayload { Damaged = true };
        TestCommand("set-thrusters-damaged", payload, transformations.SetDamage(new ThrustersState(), payload));
    }
    
    [Test]
    public void When_setting_disabled()
    {
        var payload = new SystemDisabledPayload { Disabled = true };
        TestCommand("set-thrusters-disabled", payload, transformations.SetDisabled(new ThrustersState(), payload));
    }

    [Test]
    public void When_configuring_thrusters()
    {
        var payload = new ThrusterConfigurationPayload { RequiredPower = 7 };
        TestCommand("configure-thrusters", payload, transformations.Configure(new ThrustersState(), payload));
    }

    [Test]
    public void When_setting_attitude()
    {
        var payload = new ThrusterAttitudePayload { Yaw = 1, Pitch = 2, Roll = 3 };
        TestCommand("set-thrusters-attitude", payload, transformations.SetAttitude(new ThrustersState(), payload));
    }
    
    [Test]
    public void When_setting_velocity()
    {
        var payload = new ThrusterVelocityPayload { X = 1, Y = 2, Z = 3 };
        TestCommand("set-thrusters-velocity", payload, transformations.SetVelocity(new ThrustersState(), payload));
    }
}