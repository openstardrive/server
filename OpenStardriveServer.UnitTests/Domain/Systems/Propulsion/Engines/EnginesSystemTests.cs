using OpenStardriveServer.Domain;
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
        GetMock<IEnginesTransforms>().Setup(x => x.SetCurrentPower(Any<EnginesState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-power", payload, expected);
    }
    
    [Test]
    public void When_setting_required_power()
    {
        var payload = new RequiredPowerPayload();
        GetMock<IEnginesTransforms>().Setup(x => x.SetRequiredPower(Any<EnginesState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-required-power", payload, expected);
    }
    
    [Test]
    public void When_setting_damaged()
    {
        var payload = new DamagedSystemsPayload();
        GetMock<IEnginesTransforms>().Setup(x => x.SetDamaged(Any<EnginesState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-damaged", payload, expected);
    }
    
    [Test]
    public void When_setting_disabled()
    {
        var payload = new DisabledSystemsPayload();
        GetMock<IEnginesTransforms>().Setup(x => x.SetDisabled(Any<EnginesState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-disabled", payload, expected);
    }
        
    [Test]
    public void When_setting_speed()
    {
        var payload = new SetSpeedPayload { Speed = 3 };
        GetMock<IEnginesTransforms>().Setup(x => x.SetSpeed(Any<EnginesState>(), payload)).Returns(expected);
        TestCommandWithPayload("set-testing-engines-speed", payload, expected);
    }

    [Test]
    public void When_the_chronometer()
    {
        var payload = new ChronometerPayload { ElapsedMilliseconds = 1000 };
        GetMock<IEnginesTransforms>().Setup(x => x.UpdateHeat(Any<EnginesState>(), payload)).Returns(expected);
        TestCommandWithPayload(ChronometerCommand.Type, payload, expected);
    }
        
    [Test]
    public void When_configuring()
    {
        var payload = new EnginesConfigurationPayload();
        GetMock<IEnginesTransforms>().Setup(x => x.Configure(Any<EnginesState>(), payload)).Returns(expected);
        TestCommandWithPayload("configure-testing-engines", payload, expected);
    }

    [Test]
    public void When_getting_max_speed()
    {
        Assert.That(ClassUnderTest.MaxSpeed, Is.EqualTo(EnginesStateDefaults.Testing.SpeedConfig.MaxSpeed));
        
        var returned = TransformResult<EnginesState>.StateChanged(EnginesStateDefaults.Testing with
        {
            SpeedConfig = new EngineSpeedConfig { MaxSpeed = 2 } 
        });
        GetMock<IEnginesTransforms>().Setup(x => x.Configure(Any<EnginesState>(), Any<EnginesConfigurationPayload>())).Returns(returned);
        ClassUnderTest.CommandProcessors["configure-testing-engines"](new Command { Payload = "{}"});

        Assert.That(ClassUnderTest.MaxSpeed, Is.EqualTo(2));
    }
    
    [Test]
    public void When_getting_current_speed()
    {
        Assert.That(ClassUnderTest.CurrentSpeed, Is.EqualTo(EnginesStateDefaults.Testing.CurrentSpeed));
        
        var returned = TransformResult<EnginesState>.StateChanged(EnginesStateDefaults.Testing with
        {
            CurrentSpeed = 7 
        });
        GetMock<IEnginesTransforms>().Setup(x => x.SetSpeed(Any<EnginesState>(), Any<SetSpeedPayload>())).Returns(returned);
        ClassUnderTest.CommandProcessors["set-testing-engines-speed"](new Command { Payload = "{}"});

        Assert.That(ClassUnderTest.CurrentSpeed, Is.EqualTo(7));
    }
}