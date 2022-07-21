using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Defense.Shields;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Defense.Shields;

public class ShieldsSystemTests : SystemsTest<ShieldsSystem>
{
    private readonly TransformResult<ShieldsState> expected =
        TransformResult<ShieldsState>.StateChanged(new ShieldsState());
    
    [Test]
    public void When_setting_power()
    {
        var payload = new SystemPowerPayload { CurrentPower = 3 };
        GetMock<IShieldTransformations>().Setup(x => x.SetPower(Any<ShieldsState>(), payload)).Returns(expected);
        TestCommand("set-shields-power", payload, expected);
    }
    
    [Test]
    public void When_setting_damaged()
    {
        var payload = new SystemDamagePayload { Damaged = true };
        GetMock<IShieldTransformations>().Setup(x => x.SetDamaged(Any<ShieldsState>(), payload)).Returns(expected);
        TestCommand("set-shields-damaged", payload, expected);
    }
    
    [Test]
    public void When_setting_disabled()
    {
        var payload = new SystemDisabledPayload { Disabled = true };
        GetMock<IShieldTransformations>().Setup(x => x.SetDisabled(Any<ShieldsState>(), payload)).Returns(expected);
        TestCommand("set-shields-disabled", payload, expected);
    }
    
    [Test]
    public void When_raising_shields()
    {
        GetMock<IShieldTransformations>().Setup(x => x.RaiseShields(Any<ShieldsState>())).Returns(expected);
        TestCommand("raise-shields", null, expected);
    }

    [Test]
    public void When_lowering_shields()
    {
        GetMock<IShieldTransformations>().Setup(x => x.LowerShields(Any<ShieldsState>())).Returns(expected);
        TestCommand("lower-shields", null, expected);
    }
    
    [Test]
    public void When_setting_modulation_frequency()
    {
        var payload = new ShieldModulationPayload { Frequency = 543.2 };
        GetMock<IShieldTransformations>().Setup(x => x.SetModulationFrequency(Any<ShieldsState>(), payload)).Returns(expected);
        TestCommand("modulate-shields", payload, expected);
    }

    [Test]
    public void When_setting_section_strengths()
    {
        var payload = new ShieldStrengthPayload();
        GetMock<IShieldTransformations>().Setup(x => x.SetSectionStrengths(Any<ShieldsState>(), payload)).Returns(expected);
        TestCommand("set-shield-strengths", payload, expected);
    }
}