namespace OpenStardriveServer.Domain.Systems.Standard;

public class StandardSystemBaseStateTransformations<T> where T : StandardSystemBaseState
{
    public TransformResult<T> SetCurrentPower(T state, SystemPowerPayload payload)
    {
        return TransformResult<T>.StateChanged(state with { CurrentPower = payload.CurrentPower });
    }
    
    public TransformResult<T> SetDamage(T state, SystemDamagePayload payload)
    {
        return TransformResult<T>.StateChanged(state with { Damaged = payload.Damaged });
    }
    
    public TransformResult<T> SetDisabled(T state, SystemDisabledPayload payload)
    {
        return TransformResult<T>.StateChanged(state with { Disabled = payload.Disabled });
    }
}