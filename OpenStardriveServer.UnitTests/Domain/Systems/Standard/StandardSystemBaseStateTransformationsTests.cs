using NUnit.Framework;
using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Standard;

public class StandardSystemBaseStateTransformationsTests
{
    private readonly StandardSystemBaseStateTransformations<SystemBaseState> classUnderTest = new();
    
    [TestCase(0)]
    [TestCase(2)]
    [TestCase(4)]
    public void When_setting_current_power(int currentPower)
    {
        var payload = new SystemPowerPayload { CurrentPower = currentPower };
        var expected = new SystemBaseState { CurrentPower = currentPower };

        var result = classUnderTest.SetCurrentPower(new SystemBaseState(), payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.StateChanged));
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public void When_setting_damage(bool damaged)
    {
        var payload = new SystemDamagePayload { Damaged = damaged };
        var expected = new SystemBaseState { Damaged = damaged };

        var result = classUnderTest.SetDamage(new SystemBaseState { Damaged = !damaged }, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.StateChanged));
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }
    
    
    
    [TestCase(true)]
    [TestCase(false)]
    public void When_setting_disabled(bool disabled)
    {
        var payload = new SystemDisabledPayload { Disabled = disabled };
        var expected = new SystemBaseState { Disabled = disabled };

        var result = classUnderTest.SetDisabled(new SystemBaseState { Disabled = !disabled }, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.StateChanged));
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }
}