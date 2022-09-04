using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Power;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Power;

public class PowerSystemTests : SystemsTest<PowerSystem>
{
    private readonly TransformResult<PowerState> expected =
        TransformResult<PowerState>.StateChanged(new PowerState());

    [Test]
    public void When_constructed_the_name_is_set()
    {
        Assert.That(ClassUnderTest.SystemName, Is.EqualTo("power"));
    }

    [Test]
    public void When_reporting_state()
    {
        TestCommand("report-state", expected);
    }
    
    [Test]
    public void When_configuring()
    {
        var state = new PowerState();
        var payload = new ConfigurePowerPayload();
        GetMock<IPowerTransforms>().Setup(x => x.Configure(state, payload)).Returns(expected);
        TestCommandWithPayload("configure-power", payload, expected);
    }
    
    [Test]
    public void When_setting_battery_damage()
    {
        var state = new PowerState();
        var payload = new BatteryDamagePayload();
        GetMock<IPowerTransforms>().Setup(x => x.SetBatteryDamage(state, payload)).Returns(expected);
        TestCommandWithPayload("set-battery-damage", payload, expected);
    }
    
    [Test]
    public void When_setting_battery_charge()
    {
        var state = new PowerState();
        var payload = new BatteryChargePayload();
        GetMock<IPowerTransforms>().Setup(x => x.SetBatteryCharge(state, payload)).Returns(expected);
        TestCommandWithPayload("set-battery-charge", payload, expected);
    }

    [Test]
    public void When_the_chronometer_fires_and_the_state_changed()
    {
        var state = new PowerState { ReactorOutput = 10, MillisecondsUntilNextUpdate = 1000};
        ClassUnderTest.SetStateForTesting(state);
        var payload = new ChronometerPayload();
        var returned = TransformResult<PowerState>.StateChanged(state with { ReactorOutput = 12 });
        GetMock<IPowerTransforms>().Setup(x => x.UpdatePower(state, payload)).Returns(returned);
        TestCommandWithPayload(ChronometerCommand.Type, payload, returned);
    }
    
    [Test]
    public void When_the_chronometer_fires_and_only_milliseconds_changed()
    {
        var state = new PowerState { ReactorOutput = 10, MillisecondsUntilNextUpdate = 2000};
        ClassUnderTest.SetStateForTesting(state);
        var payload = new ChronometerPayload();
        var returned = TransformResult<PowerState>.StateChanged(state with { MillisecondsUntilNextUpdate = 1000});
        GetMock<IPowerTransforms>().Setup(x => x.UpdatePower(state, payload)).Returns(returned);
        TestCommandWithPayload(ChronometerCommand.Type, payload, TransformResult<PowerState>.NoChange());
    }
}