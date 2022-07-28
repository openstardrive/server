using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Propulsion.Engines;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Propulsion.Engines;

public class EnginesSystemTests : SystemsTest<TestingEnginesSystem>
{
    private readonly TransformResult<EnginesState> expected =
        TransformResult<EnginesState>.StateChanged(new EnginesState());

    [Test]
    public void When_reporting_state()
    {
        TestCommand("report-state", TransformResult<EnginesState>.StateChanged(EnginesStateDefaults.Testing));    
    }
    
    [Test]
    public void When_setting_power()
    {
        var payload = new CurrentPowerPayload();
        GetMock<IEnginesTransformations>().Setup(x => x.SetCurrentPower(Any<EnginesState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-power", payload, expected);
    }
    
    [Test]
    public void When_setting_required_power()
    {
        var payload = new RequiredPowerPayload();
        GetMock<IEnginesTransformations>().Setup(x => x.SetRequiredPower(Any<EnginesState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-required-power", payload, expected);
    }
    
    [Test]
    public void When_setting_damaged()
    {
        var payload = new DamagedSystemsPayload();
        GetMock<IEnginesTransformations>().Setup(x => x.SetDamaged(Any<EnginesState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-damaged", payload, expected);
    }
    
    [Test]
    public void When_setting_disabled()
    {
        var payload = new DisabledSystemsPayload();
        GetMock<IEnginesTransformations>().Setup(x => x.SetDisabled(Any<EnginesState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-disabled", payload, expected);
    }
        
    [Test]
    public void When_setting_speed()
    {
        var payload = new SetSpeedPayload { Speed = 3 };
        GetMock<IEnginesTransformations>().Setup(x => x.SetSpeed(Any<EnginesState>(), payload)).Returns(expected);
        TestCommandWithPayload("set-testing-engines-speed", payload, expected);
    }

    [Test]
    public void When_the_chronometer()
    {
        var payload = new ChronometerPayload { ElapsedMilliseconds = 1000 };
        GetMock<IEnginesTransformations>().Setup(x => x.UpdateHeat(Any<EnginesState>(), payload)).Returns(expected);
        TestCommandWithPayload(ChronometerCommand.Type, payload, expected);
    }
        
    [Test]
    public void When_configuring()
    {
        var payload = new EnginesConfigurationPayload();
        GetMock<IEnginesTransformations>().Setup(x => x.Configure(Any<EnginesState>(), payload)).Returns(expected);
        TestCommandWithPayload("configure-testing-engines", payload, expected);
    }
}