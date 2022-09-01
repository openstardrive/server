using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Debug;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Debug;

public class DebugSystemTests : SystemsTest<DebugSystem>
{
    [Test]
    public void When_debugging()
    {
        var state = new DebugState();
        var payload = new DebugPayload();
        var expected = new DebugState();
        GetMock<IDebugTransforms>().Setup(x => x.AddEntry(state, payload)).Returns(TransformResult<DebugState>.StateChanged(expected));
        
        TestCommandWithPayload("debug", payload, TransformResult<DebugState>.StateChanged(new DebugState()));
    }
}