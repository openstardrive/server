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
    public void When_setting_power()
    {
        var payload = new SystemPowerPayload { CurrentPower = 3 };
        GetMock<IEnginesTransformations>().Setup(x => x.SetCurrentPower(Any<EnginesState>(), Any<SystemPowerPayload>())).Returns(expected);
        TestCommand("set-testing-engines-power", payload, expected);
    }
    
    [Test]
    public void When_setting_damaged()
    {
        var payload = new SystemDamagePayload { Damaged = true };
        GetMock<IEnginesTransformations>().Setup(x => x.SetDamage(Any<EnginesState>(), Any<SystemDamagePayload>())).Returns(expected);
        TestCommand("set-testing-engines-damaged", payload, expected);
    }
    
    [Test]
    public void When_setting_disabled()
    {
        var payload = new SystemDisabledPayload { Disabled = true };
        GetMock<IEnginesTransformations>().Setup(x => x.SetDisabled(Any<EnginesState>(), Any<SystemDisabledPayload>())).Returns(expected);
        TestCommand("set-testing-engines-disabled", payload, expected);
    }
        
    [Test]
    public void When_setting_speed()
    {
        var payload = new SetSpeedPayload { Speed = 3 };
        GetMock<IEnginesTransformations>().Setup(x => x.SetSpeed(Any<EnginesState>(), Any<SetSpeedPayload>())).Returns(expected);
        TestCommand("set-testing-engines-speed", payload, expected);
    }

    [Test]
    public void When_the_chronometer()
    {
        var payload = new ChronometerPayload { ElapsedMilliseconds = 1000 };
        GetMock<IEnginesTransformations>().Setup(x => x.UpdateHeat(Any<EnginesState>(), Any<ChronometerPayload>())).Returns(expected);
        TestCommand(ChronometerCommand.Type, payload, expected);
    }
        
    [Test]
    public void When_configuring()
    {
        var payload = new EnginesConfigurationPayload();
        GetMock<IEnginesTransformations>().Setup(x => x.Configure(Any<EnginesState>(), Any<EnginesConfigurationPayload>())).Returns(expected);
        TestCommand("configure-testing-engines", payload, expected);
    }
}