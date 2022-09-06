using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.LifeSupport;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.LifeSupport;

public class LifeSupportSystemTests : SystemsTest<LifeSupportSystem>
{
    private readonly TransformResult<LifeSupportState> expected = TransformResult<LifeSupportState>.StateChanged(new LifeSupportState());

    [Test]
    public void When_constructed_the_system_name_is_set()
    {
        Assert.That(ClassUnderTest.SystemName, Is.EqualTo("life-support"));
    }

    [Test]
    public void When_reporting_state()
    {
        TestCommand("report-state", expected);
    }
    
    [Test]
    public void When_setting_damaged()
    {
        var payload = new DamagedSystemsPayload();
        GetMock<IStandardTransforms<LifeSupportState>>().Setup(x => x.SetDamaged(Any<LifeSupportState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-damaged", payload, expected);
    }
    
    [Test]
    public void When_setting_disabled()
    {
        var payload = new DisabledSystemsPayload();
        GetMock<IStandardTransforms<LifeSupportState>>().Setup(x => x.SetDisabled(Any<LifeSupportState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-disabled", payload, expected);
    }
    
    [Test]
    public void When_setting_power()
    {
        var payload = new CurrentPowerPayload();
        GetMock<IStandardTransforms<LifeSupportState>>().Setup(x => x.SetCurrentPower(Any<LifeSupportState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-power", payload, expected);
    }
    
    [Test]
    public void When_setting_required_power()
    {
        var payload = new RequiredPowerPayload();
        GetMock<IStandardTransforms<LifeSupportState>>().Setup(x => x.SetRequiredPower(Any<LifeSupportState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-required-power", payload, expected);
    }
}