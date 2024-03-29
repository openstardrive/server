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
        GetMock<IThrusterTransforms>().Setup(x => x.SetCurrentPower(Any<ThrustersState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-power", payload, expected);
    }
    
    [Test]
    public void When_setting_required_power()
    {
        var payload = new RequiredPowerPayload();
        GetMock<IThrusterTransforms>().Setup(x => x.SetRequiredPower(Any<ThrustersState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-required-power", payload, expected);
    }
    
    [Test]
    public void When_setting_damaged()
    {
        var payload = new DamagedSystemsPayload();
        GetMock<IThrusterTransforms>().Setup(x => x.SetDamaged(Any<ThrustersState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-damaged", payload, expected);
    }
    
    [Test]
    public void When_setting_disabled()
    {
        var payload = new DisabledSystemsPayload();
        GetMock<IThrusterTransforms>().Setup(x => x.SetDisabled(Any<ThrustersState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-disabled", payload, expected);
    }

    [Test]
    public void When_setting_attitude()
    {
        var payload = new ThrusterAttitudePayload { Yaw = 1, Pitch = 2, Roll = 3 };
        GetMock<IThrusterTransforms>().Setup(x => x.SetAttitude(Any<ThrustersState>(), payload)).Returns(expected);
        TestCommandWithPayload("set-thrusters-attitude", payload, expected);
    }
    
    [Test]
    public void When_setting_velocity()
    {
        var payload = new ThrusterVelocityPayload { X = 1, Y = 2, Z = 3 };
        GetMock<IThrusterTransforms>().Setup(x => x.SetVelocity(Any<ThrustersState>(), payload)).Returns(expected);
        TestCommandWithPayload("set-thrusters-velocity", payload, expected);
    }
}