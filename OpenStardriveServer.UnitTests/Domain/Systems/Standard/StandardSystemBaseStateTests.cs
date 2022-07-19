using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Standard;

public class StandardSystemBaseStateTests
{
    [TestCase(true)]
    [TestCase(false)]
    public void When_checking_has_sufficient_power(bool hasSufficientPower)
    {
        var classUnderTest = new StandardSystemBaseState { RequiredPower = 5, CurrentPower = hasSufficientPower ? 5 : 0 };

        var result = classUnderTest.HasInsufficientPower();
        
        Assert.That(result.HasValue, Is.EqualTo(!hasSufficientPower));
        result.IfSome(message => Assert.That(message, Is.EqualTo(StandardSystemBaseState.InsufficientPowerError)));
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public void When_checking_is_disabled(bool isDisabled)
    {
        var classUnderTest = new StandardSystemBaseState { Disabled = isDisabled };

        var result = classUnderTest.IsDisabled();
        
        Assert.That(result.HasValue, Is.EqualTo(isDisabled));
        result.IfSome(message => Assert.That(message, Is.EqualTo(StandardSystemBaseState.DisabledError)));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void When_checking_is_damaged(bool isDamaged)
    {
        var classUnderTest = new StandardSystemBaseState { Damaged = isDamaged };

        var result = classUnderTest.IsDamaged();
        
        Assert.That(result.HasValue, Is.EqualTo(isDamaged));
        result.IfSome(message => Assert.That(message, Is.EqualTo(StandardSystemBaseState.DamagedError)));
    }

    [TestCase(true, true, true, StandardSystemBaseState.DisabledError)]
    [TestCase(true, true, false, StandardSystemBaseState.DisabledError)]
    [TestCase(true, false, false, StandardSystemBaseState.DisabledError)]
    [TestCase(true, false, true, StandardSystemBaseState.DisabledError)]
    [TestCase(false, true, true, StandardSystemBaseState.DamagedError)]
    [TestCase(false, true, false, StandardSystemBaseState.DamagedError)]
    [TestCase(false, false, true, StandardSystemBaseState.InsufficientPowerError)]
    [TestCase(false, false, false, "")]
    public void When_checking_if_functional_and_a_state_is_provided(bool isDisabled, bool isDamaged, bool hasInsufficientPower, string expectedError)
    {
        var classUnderTest = new StandardSystemBaseState
        {
            RequiredPower = 5,
            CurrentPower = hasInsufficientPower ? 0 : 5,
            Damaged = isDamaged,
            Disabled = isDisabled
        };
        var resultIfFunctional = classUnderTest with { RequiredPower = 10 };

        var result = classUnderTest.IfFunctional(() => resultIfFunctional);
        
        Assert.That(result.ResultType, Is.EqualTo(expectedError == "" ? TransformResultType.StateChanged : TransformResultType.Error));
        Assert.That(result.NewState.HasValue, Is.EqualTo(expectedError == ""));
        Assert.That(result.ErrorMessage, Is.EqualTo(expectedError));
        result.NewState.IfSome(state => Assert.That(state, Is.EqualTo(resultIfFunctional)));
    }

    [TestCase(true, true, true, StandardSystemBaseState.DisabledError)]
    [TestCase(true, true, false, StandardSystemBaseState.DisabledError)]
    [TestCase(true, false, false, StandardSystemBaseState.DisabledError)]
    [TestCase(true, false, true, StandardSystemBaseState.DisabledError)]
    [TestCase(false, true, true, StandardSystemBaseState.DamagedError)]
    [TestCase(false, true, false, StandardSystemBaseState.DamagedError)]
    [TestCase(false, false, true, StandardSystemBaseState.InsufficientPowerError)]
    [TestCase(false, false, false, "")]
    public void When_checking_if_functional_and_a_transform_is_provided(bool isDisabled, bool isDamaged, bool hasInsufficientPower, string expectedError)
    {
        var classUnderTest = new StandardSystemBaseState
        {
            RequiredPower = 5,
            CurrentPower = hasInsufficientPower ? 0 : 5,
            Damaged = isDamaged,
            Disabled = isDisabled
        };

        var result = classUnderTest.IfFunctional(() => TransformResult<StandardSystemBaseState>.NoChange());
        
        Assert.That(result.ResultType, Is.EqualTo(expectedError == "" ? TransformResultType.NoChange : TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo(expectedError));
    }
}