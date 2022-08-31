using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Comms.ShortRange;

public interface IShortRangeTransforms : IStandardTransforms<ShortRangeState>
{
    TransformResult<ShortRangeState> ConfigureFrequencyRanges(ShortRangeState state, ConfigureFrequencyRangesPayload payload);
    TransformResult<ShortRangeState> SetActiveSignals(ShortRangeState state, SetActiveSignalsPayload payload);
    TransformResult<ShortRangeState> SetCurrentFrequency(ShortRangeState state, SetCurrentFrequencyPayload payload);
    TransformResult<ShortRangeState> SetBroadcasting(ShortRangeState state, SetBroadcastingPayload payload);
}

public class ShortRangeTransforms : IShortRangeTransforms
{
    private readonly IStandardTransforms<ShortRangeState> standardTransforms;

    public ShortRangeTransforms(IStandardTransforms<ShortRangeState> standardTransforms)
    {
        this.standardTransforms = standardTransforms;
    }
    
    public TransformResult<ShortRangeState> SetDisabled(ShortRangeState state, string systemName, DisabledSystemsPayload payload)
    {
        var result = standardTransforms.SetDisabled(state, systemName, payload);
        return result.NewState.Case(
            some: newState => newState.Disabled && newState.IsBroadcasting
                ? TransformResult<ShortRangeState>.StateChanged(newState with {IsBroadcasting = false })
                : result,
            none: () => result);
    }

    public TransformResult<ShortRangeState> SetDamaged(ShortRangeState state, string systemName, DamagedSystemsPayload payload)
    {
        var result = standardTransforms.SetDamaged(state, systemName, payload);
        return result.NewState.Case(
            some: newState => newState.Damaged && newState.IsBroadcasting
                ? TransformResult<ShortRangeState>.StateChanged(newState with {IsBroadcasting = false })
                : result,
            none: () => result);
    }

    public TransformResult<ShortRangeState> SetCurrentPower(ShortRangeState state, string systemName, CurrentPowerPayload payload)
    {
        var result = standardTransforms.SetCurrentPower(state, systemName, payload);
        return result.NewState.Case(
            some: newState => newState.CurrentPower < newState.RequiredPower && newState.IsBroadcasting
                ? TransformResult<ShortRangeState>.StateChanged(newState with {IsBroadcasting = false })
                : result,
            none: () => result);
    }

    public TransformResult<ShortRangeState> SetRequiredPower(ShortRangeState state, string systemName, RequiredPowerPayload payload)
    {
        var result = standardTransforms.SetRequiredPower(state, systemName, payload);
        return result.NewState.Case(
            some: newState => newState.CurrentPower < newState.RequiredPower && newState.IsBroadcasting
                ? TransformResult<ShortRangeState>.StateChanged(newState with {IsBroadcasting = false })
                : result,
            none: () => result);
    }

    public TransformResult<ShortRangeState> ConfigureFrequencyRanges(ShortRangeState state, ConfigureFrequencyRangesPayload payload)
    {
        return TransformResult<ShortRangeState>.StateChanged(state with
        {
            FrequencyRanges = payload.FrequencyRanges
        });
    }

    public TransformResult<ShortRangeState> SetActiveSignals(ShortRangeState state, SetActiveSignalsPayload payload)
    {
        return TransformResult<ShortRangeState>.StateChanged(state with
        {
            ActiveSignals = payload.ActiveSignals
        });
    }

    public TransformResult<ShortRangeState> SetCurrentFrequency(ShortRangeState state, SetCurrentFrequencyPayload payload)
    {
        return TransformResult<ShortRangeState>.StateChanged(state with
        {
            CurrentFrequency = payload.Frequency
        });
    }

    public TransformResult<ShortRangeState> SetBroadcasting(ShortRangeState state, SetBroadcastingPayload payload)
    {
        return state.IfFunctional(() => state with { IsBroadcasting = payload.IsBroadcasting });
    }
}