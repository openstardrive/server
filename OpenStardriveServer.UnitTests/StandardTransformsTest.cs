using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests;

public abstract class StandardTransformsTest<T, TState> : WithAnAutomocked<T>
    where T : IStandardTransforms<TState>
    where TState : StandardSystemBaseState
{
    public void TestStandardDisabled(TState state)
    {
        var systemName = "test-system";
        var payload = new DisabledSystemsPayload();
        var expected = TransformResult<TState>.NoChange();
        GetMock<IStandardTransforms<TState>>().Setup(x => x.SetDisabled(state, systemName, payload)).Returns(expected);

        var result = ClassUnderTest.SetDisabled(state, systemName, payload);
        
        Assert.That(result, Is.SameAs(expected));
    }
}