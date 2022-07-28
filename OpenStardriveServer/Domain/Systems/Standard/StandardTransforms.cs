namespace OpenStardriveServer.Domain.Systems.Standard;

public interface IStandardTransforms<T> where T : StandardSystemBaseState
{
    TransformResult<T> SetDisabled(T state, string systemName, DisabledSystemsPayload payload);
}

public class StandardTransforms<T> : IStandardTransforms<T> where T : StandardSystemBaseState
{
    public TransformResult<T> SetDisabled(T state, string systemName, DisabledSystemsPayload payload)
    {
        return payload.ValueOrNone(systemName).Case(
            some: disabled => TransformResult<T>.StateChanged(state with { Disabled = disabled }),
            none: TransformResult<T>.NoChange
        );
    }
}