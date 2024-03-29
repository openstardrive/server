using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Defense.Shields;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Defense.Shields;

public class ShieldsSystemTests : SystemsTest<ShieldsSystem>
{
    private readonly TransformResult<ShieldsState> expected =
        TransformResult<ShieldsState>.StateChanged(new ShieldsState());

    [Test]
    public void When_reporting_state()
    {
        TestCommand("report-state", TransformResult<ShieldsState>.StateChanged(new ShieldsState()));    
    }
    
    [Test]
    public void When_setting_power()
    {
        var payload = new CurrentPowerPayload();
        GetMock<IShieldTransforms>().Setup(x => x.SetCurrentPower(Any<ShieldsState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-power", payload, expected);
    }
    
    [Test]
    public void When_setting_required_power()
    {
        var payload = new RequiredPowerPayload();
        GetMock<IShieldTransforms>().Setup(x => x.SetRequiredPower(Any<ShieldsState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-required-power", payload, expected);
    }
    
    [Test]
    public void When_setting_damaged()
    {
        var payload = new DamagedSystemsPayload();
        GetMock<IShieldTransforms>().Setup(x => x.SetDamaged(Any<ShieldsState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-damaged", payload, expected);
    }
    
    [Test]
    public void When_setting_disabled()
    {
        var payload = new DisabledSystemsPayload();
        GetMock<IShieldTransforms>().Setup(x => x.SetDisabled(Any<ShieldsState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-disabled", payload, expected);
    }
    
    [Test]
    public void When_raising_shields()
    {
        GetMock<IShieldTransforms>().Setup(x => x.RaiseShields(Any<ShieldsState>())).Returns(expected);
        TestCommand("raise-shields", expected);
    }

    [Test]
    public void When_lowering_shields()
    {
        GetMock<IShieldTransforms>().Setup(x => x.LowerShields(Any<ShieldsState>())).Returns(expected);
        TestCommand("lower-shields", expected);
    }
    
    [Test]
    public void When_setting_modulation_frequency()
    {
        var payload = new ShieldModulationPayload { Frequency = 543.2 };
        GetMock<IShieldTransforms>().Setup(x => x.SetModulationFrequency(Any<ShieldsState>(), payload)).Returns(expected);
        TestCommandWithPayload("modulate-shields", payload, expected);
    }

    [Test]
    public void When_setting_section_strengths()
    {
        var payload = new ShieldStrengthPayload();
        GetMock<IShieldTransforms>().Setup(x => x.SetSectionStrengths(Any<ShieldsState>(), payload)).Returns(expected);
        TestCommandWithPayload("set-shield-strengths", payload, expected);
    }
}