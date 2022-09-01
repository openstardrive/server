namespace OpenStardriveServer.Domain.Systems.Debug;

public interface IDebugTransforms
{
    TransformResult<DebugState> AddEntry(DebugState state, DebugPayload payload);
}

public class DebugTransforms : IDebugTransforms
{
    public TransformResult<DebugState> AddEntry(DebugState state, DebugPayload payload)
    {
        return TransformResult<DebugState>.StateChanged(state with { LastEntry = payload });
    }
}