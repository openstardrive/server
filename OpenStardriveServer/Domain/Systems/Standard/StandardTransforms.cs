namespace OpenStardriveServer.Domain.Systems.Standard;

public interface IStandardTransforms<T> where T : StandardSystemBaseState
{
    TransformResult<T> SetDisabled(T state, string systemName, DisabledSystemsPayload payload);
    TransformResult<T> SetDamaged(T state, string systemName, DamagedSystemsPayload payload);
    TransformResult<T> SetCurrentPower(T state, string systemName, CurrentPowerPayload payload);
    TransformResult<T> SetRequiredPower(T state, string systemName, RequiredPowerPayload payload);
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

    public TransformResult<T> SetDamaged(T state, string systemName, DamagedSystemsPayload payload)
    {
        return payload.ValueOrNone(systemName).Case(
            some: damaged => TransformResult<T>.StateChanged(state with { Damaged = damaged }),
            none: TransformResult<T>.NoChange
        );
    }

    public TransformResult<T> SetCurrentPower(T state, string systemName, CurrentPowerPayload payload)
    {
        return payload.ValueOrNone(systemName).Case(
            some: x => TransformResult<T>.StateChanged(state with { CurrentPower = x }),
            none: TransformResult<T>.NoChange);
    }
    
    public TransformResult<T> SetRequiredPower(T state, string systemName, RequiredPowerPayload payload)
    {
        return payload.ValueOrNone(systemName).Case(
            some: x => TransformResult<T>.StateChanged(state with { RequiredPower = x }),
            none: TransformResult<T>.NoChange);
    }
}