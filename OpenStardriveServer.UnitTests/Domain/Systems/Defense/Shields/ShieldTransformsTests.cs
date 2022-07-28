using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Defense.Shields;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Defense.Shields;

public class ShieldTransformsTests : StandardTransformsTest<ShieldTransforms, ShieldsState>
{
    [Test]
    public void When_raising_shields_successfully()
    {
        var state = new ShieldsState();
        var expected = state with { Raised = true };
        
        var result = ClassUnderTest.RaiseShields(state);
        
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }

    [Test]
    public void When_raising_shields_but_system_disabled()
    {
        var state = new ShieldsState { Disabled = true };
        
        var result = ClassUnderTest.RaiseShields(state);
        
        Assert.That(result.ErrorMessage, Is.EqualTo(StandardSystemBaseState.DisabledError));
    }
    
    [Test]
    public void When_raising_shields_but_system_damaged()
    {
        var state = new ShieldsState { Damaged = true };
        
        var result = ClassUnderTest.RaiseShields(state);
        
        Assert.That(result.ErrorMessage, Is.EqualTo(StandardSystemBaseState.DamagedError));
    }
    
    [Test]
    public void When_raising_shields_but_insufficient_power()
    {
        var state = new ShieldsState { CurrentPower = 0, RequiredPower = 10 };
        
        var result = ClassUnderTest.RaiseShields(state);
        
        Assert.That(result.ErrorMessage, Is.EqualTo(StandardSystemBaseState.InsufficientPowerError));
    }

    [Test]
    public void When_lowering_shields()
    {
        var state = new ShieldsState { Raised = true };
        var expected = state with { Raised = false };
        
        var result = ClassUnderTest.LowerShields(state);
        
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }

    [Test]
    public void When_setting_modulation_frequency()
    {
        var state = new ShieldsState();
        var payload = new ShieldModulationPayload { Frequency = 123.45 };
        var expected = state with { ModulationFrequency = payload.Frequency };

        var result = ClassUnderTest.SetModulationFrequency(state, payload);
        
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
        var systemName = "shields";
        var state = new ShieldsState
        {
            CurrentPower = 5,
            RequiredPower = 5,
            Raised = wereRaised
        };
        var payload = new CurrentPowerPayload
        {
            ["other"] = 11,
            [systemName] = newPower
        };
        var expected = state with { CurrentPower = newPower, Raised = expectedRaised };
        
        var result = ClassUnderTest.SetCurrentPower(state, systemName, payload);
        
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }
    
    [Test]
    public void When_setting_shield_power_but_there_is_no_match()
    {
        var systemName = "shields";
        var state = new ShieldsState();
        var payload = new CurrentPowerPayload { ["other"] = 22 };
        
        var result = ClassUnderTest.SetCurrentPower(state, systemName, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));
    }
    
    [TestCase(0, true, true)]
    [TestCase(0, false, false)]
    [TestCase(4, true, true)]
    [TestCase(5, true, true)]
    [TestCase(6, false, false)]
    [TestCase(6, true, false)]
    public void When_setting_required_power(int newPower, bool wereRaised, bool expectedRaised)
    {
        var systemName = "shields";
        var state = new ShieldsState
        {
            CurrentPower = 5,
            RequiredPower = 5,
            Raised = wereRaised
        };
        var payload = new RequiredPowerPayload
        {
            ["other"] = 11,
            [systemName] = newPower
        };
        var expected = state with { RequiredPower = newPower, Raised = expectedRaised };
        
        var result = ClassUnderTest.SetRequiredPower(state, systemName, payload);
        
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }
    
    [Test]
    public void When_setting_required_power_but_there_is_no_match()
    {
        var systemName = "shields";
        var state = new ShieldsState();
        var payload = new RequiredPowerPayload { ["other"] = 22 };
        
        var result = ClassUnderTest.SetRequiredPower(state, systemName, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));
    }

    [TestCase(true, true, true, false)]
    [TestCase(true, true, false, false)]
    [TestCase(false, true, true, true)]
    [TestCase(false, true, false, false)]
    [TestCase(false, false)]
    [TestCase(true, false)]
    public void When_setting_damaged(bool newDamaged, bool expectChange, bool wereRaised = false, bool expectedRaised = false)
    {
        var systemName = "shields";
        var payload = new DamagedSystemsPayload { ["other"] = false };
        if (expectChange)
        {
            payload[systemName] = newDamaged;
        }

        var state = new ShieldsState { Damaged = !newDamaged, Raised = wereRaised };
        var result = ClassUnderTest.SetDamaged(state, systemName, payload);

        if (expectChange)
        {
            var expected = state with { Damaged = newDamaged, Raised = expectedRaised};
            Assert.That(result.NewState.Value, Is.EqualTo(expected));
        }
        else
        {
            Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));    
        }
    }
    
    [Test]
    public void When_setting_disabled()
    {
        TestStandardDisabled(new ShieldsState());
    }

    [TestCase(.3, .4, .5, .6, .3, .4, .5, .6)]
    [TestCase(-.3, -.4, -.5, -.6, 0, 0, 0, 0)]
    [TestCase(1.3, 1.4, 1.5, 1.6, 1, 1, 1, 1)]
    [TestCase(0, 0, 0, 0, 0, 0, 0, 0)]
    [TestCase(1, 1, 1, 1, 1, 1, 1, 1)]
    public void When_setting_shield_section_strengths(double forward, double aft, double port, double starboard,
        double expectedForward, double expectedAft, double expectedPort, double expectedStarboard)
    {
        var state = new ShieldsState();
        var payload = new ShieldStrengthPayload { SectionStrengths = new ShieldSectionStrengths
        {
            ForwardPercent = forward,
            AftPercent = aft,
            PortPercent = port,
            StarboardPercent = starboard
        }};
        var expected = state with { SectionStrengths = new ShieldSectionStrengths
        {
            ForwardPercent = expectedForward,
            AftPercent = expectedAft,
            PortPercent = expectedPort,
            StarboardPercent = expectedStarboard
        } };

        var result = ClassUnderTest.SetSectionStrengths(state, payload);
        
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }
}