using System;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Defense.Shields;

public interface IShieldTransforms : IStandardTransforms<ShieldsState>
{
    TransformResult<ShieldsState> RaiseShields(ShieldsState state);
    TransformResult<ShieldsState> LowerShields(ShieldsState state);
    TransformResult<ShieldsState> SetModulationFrequency(ShieldsState state, ShieldModulationPayload payload);
    TransformResult<ShieldsState> SetSectionStrengths(ShieldsState state, ShieldStrengthPayload payload);
}

public class ShieldTransforms : IShieldTransforms
{
    private readonly IStandardTransforms<ShieldsState> standardTransforms;

    public ShieldTransforms(IStandardTransforms<ShieldsState> standardTransforms)
    {
        this.standardTransforms = standardTransforms;
    }

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
    
    public TransformResult<ShieldsState> SetCurrentPower(ShieldsState state, string systemName, CurrentPowerPayload payload)
    {
        return payload.ValueOrNone(systemName).Case(
            some: newPower =>
            {
                var raised = state.Raised && newPower >= state.RequiredPower;
                return TransformResult<ShieldsState>.StateChanged(state with { CurrentPower = newPower, Raised = raised });
            },
            none: TransformResult<ShieldsState>.NoChange);
    }
    
    public TransformResult<ShieldsState> SetRequiredPower(ShieldsState state, string systemName, RequiredPowerPayload payload)
    {
        return payload.ValueOrNone(systemName).Case(
            some: newRequired =>
            {
                var raised = state.Raised && state.CurrentPower >= newRequired;
                return TransformResult<ShieldsState>.StateChanged(state with { RequiredPower = newRequired, Raised = raised });
            },
            none: TransformResult<ShieldsState>.NoChange);
    }

    public TransformResult<ShieldsState> SetDamaged(ShieldsState state, string systemName, DamagedSystemsPayload payload)
    {
        return payload.ValueOrNone(systemName).Case(
            some: damaged =>
            {
                var raised = state.Raised && !damaged;
                return TransformResult<ShieldsState>.StateChanged(state with { Damaged = damaged, Raised = raised });
            },
            none: TransformResult<ShieldsState>.NoChange
        );
    }

    public TransformResult<ShieldsState> SetDisabled(ShieldsState state, string systemName, DisabledSystemsPayload payload)
    {
        return standardTransforms.SetDisabled(state, systemName, payload);
    }

    public TransformResult<ShieldsState> SetSectionStrengths(ShieldsState state, ShieldStrengthPayload payload)
    {
        return TransformResult<ShieldsState>.StateChanged(state with { SectionStrengths = new ShieldSectionStrengths
        {
            ForwardPercent = Math.Max(0, payload.SectionStrengths.ForwardPercent),
            AftPercent = Math.Max(0, payload.SectionStrengths.AftPercent),
            PortPercent = Math.Max(0, payload.SectionStrengths.PortPercent),
            StarboardPercent = Math.Max(0, payload.SectionStrengths.StarboardPercent)
        }});
    }
}