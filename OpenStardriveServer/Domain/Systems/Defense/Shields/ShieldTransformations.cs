using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Defense.Shields;

public class ShieldTransformations
{
    public TransformResult<ShieldsState> RaiseShields(ShieldsState state)
    {
        return state.IfFunctional(() => state with { Raised = true });
    }
    
    public TransformResult<ShieldsState> LowerShields(ShieldsState state)
    {
        return TransformResult<ShieldsState>.StateChanged(state with { Raised = false });
    }
    
    public TransformResult<ShieldsState> SetModulationFrequency(ShieldsState state, ShieldModulationPayload payload)
    {
        return TransformResult<ShieldsState>.StateChanged(state with { ModulationFrequency = payload.Frequency });
    }
    
    public TransformResult<ShieldsState> SetPower(ShieldsState state, SystemPowerPayload payload)
    {
        var raised = state.Raised && payload.CurrentPower >= state.RequiredPower;
        return TransformResult<ShieldsState>.StateChanged(state with { CurrentPower = payload.CurrentPower, Raised = raised });
    }

    public TransformResult<ShieldsState> SetDamaged(ShieldsState state, SystemDamagePayload payload)
    {
        var raised = state.Raised && !payload.Damaged;
        return TransformResult<ShieldsState>.StateChanged(state with { Damaged = payload.Damaged, Raised = raised });
    }

    public TransformResult<ShieldsState> SetDisabled(ShieldsState state, SystemDisabledPayload payload)
    {
        return TransformResult<ShieldsState>.StateChanged(state with { Disabled = payload.Disabled });
    }
}