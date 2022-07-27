using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Propulsion.Thrusters;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Propulsion.Thrusters;

public class ThrusterSystemTests : SystemsTest<ThrustersSystem>
{
    private readonly TransformResult<ThrustersState> expected =
        TransformResult<ThrustersState>.StateChanged(new ThrustersState());

    [Test]
    public void When_reporting_state()
    {
        TestCommand("report-state", TransformResult<ThrustersState>.StateChanged(new ThrustersState()));    
    }
    
    [Test]
    public void When_setting_power()
    {
        var payload = new SystemPowerPayload { CurrentPower = 3 };
        GetMock<IThrusterTransformations>().Setup(x => x.SetCurrentPower(Any<ThrustersState>(), Any<SystemPowerPayload>())).Returns(expected);
        TestCommandWithPayload("set-thrusters-power", payload, expected);
    }
    
    [Test]
    public void When_setting_damaged()
    {
        var payload = new SystemDamagePayload { Damaged = true };
        GetMock<IThrusterTransformations>().Setup(x => x.SetDamage(Any<ThrustersState>(), Any<SystemDamagePayload>())).Returns(expected);
        TestCommandWithPayload("set-thrusters-damaged", payload, expected);
    }
    
    [Test]
    public void When_setting_disabled()
    {
        var payload = new SystemDisabledPayload { Disabled = true };
        GetMock<IThrusterTransformations>().Setup(x => x.SetDisabled(Any<ThrustersState>(), Any<SystemDisabledPayload>())).Returns(expected);
        TestCommandWithPayload("set-thrusters-disabled", payload, expected);
    }

    [Test]
    public void When_configuring_thrusters()
    {
        var payload = new ThrusterConfigurationPayload { RequiredPower = 7 };
        GetMock<IThrusterTransformations>().Setup(x => x.Configure(Any<ThrustersState>(), Any<ThrusterConfigurationPayload>())).Returns(expected);
        TestCommandWithPayload("configure-thrusters", payload, expected);
    }

    [Test]
    public void When_setting_attitude()
    {
        var payload = new ThrusterAttitudePayload { Yaw = 1, Pitch = 2, Roll = 3 };
        GetMock<IThrusterTransformations>().Setup(x => x.SetAttitude(Any<ThrustersState>(), Any<ThrusterAttitudePayload>())).Returns(expected);
        TestCommandWithPayload("set-thrusters-attitude", payload, expected);
    }
    
    [Test]
    public void When_setting_velocity()
    {
        var payload = new ThrusterVelocityPayload { X = 1, Y = 2, Z = 3 };
        GetMock<IThrusterTransformations>().Setup(x => x.SetVelocity(Any<ThrustersState>(), Any<ThrusterVelocityPayload>())).Returns(expected);
        TestCommandWithPayload("set-thrusters-velocity", payload, expected);
    }
}