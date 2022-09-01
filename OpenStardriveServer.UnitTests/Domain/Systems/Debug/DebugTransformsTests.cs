using OpenStardriveServer.Domain.Systems.Debug;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Debug;

public class DebugTransformsTests : WithAnAutomocked<DebugTransforms>
{
    [Test]
    public void When_adding_an_entry()
    {
        var state = new DebugState();
        var payload = new DebugPayload { DebugId = RandomString(), Description = "Test debug description." };

        var result = ClassUnderTest.AddEntry(state, payload);
        
        Assert.That(result.NewState.Value, Is.EqualTo(new DebugState { LastEntry = payload }));
    }
}