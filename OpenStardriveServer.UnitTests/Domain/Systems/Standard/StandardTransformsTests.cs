using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Standard;

public class StandardTransformsTests : WithAnAutomocked<StandardTransforms<StandardSystemBaseState>>
{
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void When_setting_disabled(bool newDisabled, bool expectChange)
    {
        var systemName = "test-system";
        var state = new StandardSystemBaseState { Disabled = !newDisabled };
        var payload = new DisabledSystemsPayload
        {
            ["other"] = true,
            ["different"] = false
        };
        if (expectChange)
        {
            payload[systemName] = newDisabled;
        }
        
        var result = ClassUnderTest.SetDisabled(state, systemName, payload);

        if (expectChange)
        {
            var expected = state with { Disabled = newDisabled };
            Assert.That(result.NewState.Value, Is.EqualTo(expected));    
        }
        else
        {
            Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));
        }
    }
}