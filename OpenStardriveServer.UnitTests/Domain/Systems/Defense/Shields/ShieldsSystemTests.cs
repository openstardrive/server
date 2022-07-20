using OpenStardriveServer.Domain.Systems.Defense.Shields;
using OpenStardriveServer.Domain.Systems.Propulsion.Thrusters;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Defense.Shields;

public class ShieldsSystemTests : SystemsTest<ShieldsSystem>
{
    protected override ShieldsSystem CreateClassUnderTest() => new();
    private ShieldTransformations transformations = new();
    
    [Test]
    public void When_setting_power()
    {
        var payload = new SystemPowerPayload { CurrentPower = 3 };
        TestCommand("set-shields-power", payload, transformations.SetPower(new ShieldsState(), payload));
    }
    
    [Test]
    public void When_setting_damaged()
    {
        var payload = new SystemDamagePayload { Damaged = true };
        TestCommand("set-shields-damaged", payload, transformations.SetDamaged(new ShieldsState(), payload));
    }
    
    [Test]
    public void When_setting_disabled()
    {
        var payload = new SystemDisabledPayload { Disabled = true };
        TestCommand("set-shields-disabled", payload, transformations.SetDisabled(new ShieldsState(), payload));
    }
    
    [Test]
    public void When_raising_shields()
    {
        TestCommand("raise-shields", null, transformations.RaiseShields(new ShieldsState()));
    }

    [Test]
    public void When_lowering_shields()
    {
        TestCommand("lower-shields", null, transformations.LowerShields(new ShieldsState()));
    }
    
    [Test]
    public void When_setting_modulation_frequency()
    {
        var payload = new ShieldModulationPayload { Frequency = 543.2 };
        TestCommand("modulate-shields", payload, transformations.SetModulationFrequency(new ShieldsState(), payload));
    }
}