using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Comms.ShortRange;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Comms.ShortRange;

public class ShortRangeSystemTests : SystemsTest<ShortRangeSystem>
{
    private readonly TransformResult<ShortRangeState> expected =
        TransformResult<ShortRangeState>.StateChanged(new ShortRangeState());

    [Test]
    public void When_constructed_the_system_name_is_set()
    {
        Assert.That(ClassUnderTest.SystemName, Is.EqualTo("short-range-comms"));
    }

    [Test]
    public void When_reporting_state()
    {
        TestCommand("report-state", expected);
    }

    [Test]
    public void When_setting_disabled()
    {
        var payload = new DisabledSystemsPayload();
        GetMock<IShortRangeTransforms>().Setup(x => x.SetDisabled(Any<ShortRangeState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-disabled", payload, expected);
    }
    
    [Test]
    public void When_setting_damaged()
    {
        var payload = new DamagedSystemsPayload();
        GetMock<IShortRangeTransforms>().Setup(x => x.SetDamaged(Any<ShortRangeState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-damaged", payload, expected);
    }
    
    [Test]
    public void When_setting_current_power()
    {
        var payload = new CurrentPowerPayload();
        GetMock<IShortRangeTransforms>().Setup(x => x.SetCurrentPower(Any<ShortRangeState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-power", payload, expected);
    }
    
    [Test]
    public void When_setting_required_power()
    {
        var payload = new RequiredPowerPayload();
        GetMock<IShortRangeTransforms>().Setup(x => x.SetRequiredPower(Any<ShortRangeState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-required-power", payload, expected);
    }
    
    [Test]
    public void When_configuring_frequency_ranges()
    {
        var payload = new ConfigureFrequencyRangesPayload();
        GetMock<IShortRangeTransforms>().Setup(x => x.ConfigureFrequencyRanges(Any<ShortRangeState>(), payload)).Returns(expected);
        TestCommandWithPayload("configure-short-range-frequencies", payload, expected);
    }
    
    [Test]
    public void When_setting_active_signals()
    {
        var payload = new SetActiveSignalsPayload();
        GetMock<IShortRangeTransforms>().Setup(x => x.SetActiveSignals(Any<ShortRangeState>(), payload)).Returns(expected);
        TestCommandWithPayload("set-short-range-signals", payload, expected);
    }
    
    [Test]
    public void When_setting_current_frequency()
    {
        var payload = new SetCurrentFrequencyPayload();
        GetMock<IShortRangeTransforms>().Setup(x => x.SetCurrentFrequency(Any<ShortRangeState>(), payload)).Returns(expected);
        TestCommandWithPayload("set-short-range-frequency", payload, expected);
    }
    
    [Test]
    public void When_setting_broadcasting()
    {
        var payload = new SetBroadcastingPayload();
        GetMock<IShortRangeTransforms>().Setup(x => x.SetBroadcasting(Any<ShortRangeState>(), payload)).Returns(expected);
        TestCommandWithPayload("set-short-range-broadcasting", payload, expected);
    }
}