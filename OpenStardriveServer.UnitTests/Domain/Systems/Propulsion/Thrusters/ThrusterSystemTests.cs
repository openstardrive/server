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
        var payload = new CurrentPowerPayload();
        GetMock<IThrusterTransformations>().Setup(x => x.SetCurrentPower(Any<ThrustersState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-power", payload, expected);
    }
    
    [Test]
    public void When_setting_required_power()
    {
        var payload = new RequiredPowerPayload();
        GetMock<IThrusterTransformations>().Setup(x => x.SetRequiredPower(Any<ThrustersState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-required-power", payload, expected);
    }
    
    [Test]
    public void When_setting_damaged()
    {
        var payload = new SystemDamagePayload { Damaged = true };
        GetMock<IThrusterTransformations>().Setup(x => x.SetDamage(Any<ThrustersState>(), payload)).Returns(expected);
        TestCommandWithPayload("set-thrusters-damaged", payload, expected);
    }
    
    [Test]
    public void When_setting_disabled()
    {
        var payload = new SystemDisabledPayload { Disabled = true };
        GetMock<IThrusterTransformations>().Setup(x => x.SetDisabled(Any<ThrustersState>(), payload)).Returns(expected);
        TestCommandWithPayload("set-thrusters-disabled", payload, expected);
    }

    [Test]
    public void When_setting_attitude()
    {
        var payload = new ThrusterAttitudePayload { Yaw = 1, Pitch = 2, Roll = 3 };
        GetMock<IThrusterTransformations>().Setup(x => x.SetAttitude(Any<ThrustersState>(), payload)).Returns(expected);
        TestCommandWithPayload("set-thrusters-attitude", payload, expected);
    }
    
    [Test]
    public void When_setting_velocity()
    {
        var payload = new ThrusterVelocityPayload { X = 1, Y = 2, Z = 3 };
        GetMock<IThrusterTransformations>().Setup(x => x.SetVelocity(Any<ThrustersState>(), payload)).Returns(expected);
        TestCommandWithPayload("set-thrusters-velocity", payload, expected);
    }
}