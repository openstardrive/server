using OpenStardriveServer.Domain.Systems.Defense.Shields;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Defense.Shields;

public class ShieldTransformationsTests
{
    private readonly ShieldTransformations classUnderTest = new();

    [Test]
    public void When_raising_shields_successfully()
    {
        var state = new ShieldsState();
        var expected = state with { Raised = true };
        
        var result = classUnderTest.RaiseShields(state);
        
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }

    [Test]
    public void When_raising_shields_but_system_disabled()
    {
        var state = new ShieldsState { Disabled = true };
        
        var result = classUnderTest.RaiseShields(state);
        
        Assert.That(result.ErrorMessage, Is.EqualTo(StandardSystemBaseState.DisabledError));
    }
    
    [Test]
    public void When_raising_shields_but_system_damaged()
    {
        var state = new ShieldsState { Damaged = true };
        
        var result = classUnderTest.RaiseShields(state);
        
        Assert.That(result.ErrorMessage, Is.EqualTo(StandardSystemBaseState.DamagedError));
    }
    
    [Test]
    public void When_raising_shields_but_insufficient_power()
    {
        var state = new ShieldsState { CurrentPower = 0, RequiredPower = 10 };
        
        var result = classUnderTest.RaiseShields(state);
        
        Assert.That(result.ErrorMessage, Is.EqualTo(StandardSystemBaseState.InsufficientPowerError));
    }

    [Test]
    public void When_lowering_shields()
    {
        var state = new ShieldsState { Raised = true };
        var expected = state with { Raised = false };
        
        var result = classUnderTest.LowerShields(state);
        
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }

    [Test]
    public void When_setting_modulation_frequency()
    {
        var state = new ShieldsState();
        var payload = new ShieldModulationPayload { Frequency = 123.45 };
        var expected = state with { ModulationFrequency = payload.Frequency };

        var result = classUnderTest.SetModulationFrequency(state, payload);
        
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }

    [TestCase(0, true, false)]
    [TestCase(0, false, false)]
    [TestCase(4, true, false)]
    [TestCase(5, true, true)]
    [TestCase(5, false, false)]
    [TestCase(6, true, true)]
    public void When_setting_shield_power(int newPower, bool wereRaised, bool expectedRaised)
    {
        var state = new ShieldsState
        {
            CurrentPower = 5,
            RequiredPower = 5,
            Raised = wereRaised
        };
        var payload = new SystemPowerPayload { CurrentPower = newPower };
        var expected = state with { CurrentPower = newPower, Raised = expectedRaised };
        
        var result = classUnderTest.SetPower(state, payload);
        
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }

    [TestCase(true, false, false)]
    [TestCase(true, true, false)]
    [TestCase(false, true, true)]
    [TestCase(false, false, false)]
    public void When_setting_damaged(bool newDamaged, bool wereRaised, bool expectedRaised)
    {
        var state = new ShieldsState { Damaged = !newDamaged, Raised = wereRaised };
        var payload = new SystemDamagePayload { Damaged = newDamaged };
        var expected = state with { Damaged = newDamaged, Raised = expectedRaised };
        
        var result = classUnderTest.SetDamaged(state, payload);
        
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public void When_disabled(bool newDisabled)
    {
        var state = new ShieldsState { Disabled = !newDisabled };
        var payload = new SystemDisabledPayload { Disabled = newDisabled };
        var expected = state with { Disabled = newDisabled };
        
        var result = classUnderTest.SetDisabled(state, payload);
        
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }
}