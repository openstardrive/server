using System;
using System.Linq;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Defense.WarheadLauncher;

public interface IWarheadLauncherTransforms : IStandardTransforms<WarheadLauncherState>
{
    TransformResult<WarheadLauncherState> Load(WarheadLauncherState state, LoadWarheadPayload payload);
    TransformResult<WarheadLauncherState> Fire(WarheadLauncherState state, FireWarheadPayload payload, DateTimeOffset commandTimestamp);
    TransformResult<WarheadLauncherState> SetInventory(WarheadLauncherState state, WarheadInventoryPayload payload);
}

public class WarheadLauncherTransforms : IWarheadLauncherTransforms
{
    private readonly IStandardTransforms<WarheadLauncherState> standardTransforms;
    
    public WarheadLauncherTransforms(IStandardTransforms<WarheadLauncherState> standardTransforms)
    {
        this.standardTransforms = standardTransforms;
    }
    
    public TransformResult<WarheadLauncherState> Load(WarheadLauncherState state, LoadWarheadPayload payload)
    {
        return state.IfFunctional(() =>
        {
            var inInventory = state.Inventory.FirstOrDefault(x => x.Kind == payload.Kind)?.Number;
            if (inInventory is null or <= 0)
            {
                return TransformResult<WarheadLauncherState>.Error($"no {payload.Kind} available to load");
            }

            if (state.Loaded.Length >= state.NumberOfLaunchers)
            {
                return TransformResult<WarheadLauncherState>.Error("no launcher available to load into");
            }

            return TransformResult<WarheadLauncherState>.StateChanged(state with
            {
                Loaded = state.Loaded.Append(payload.Kind).ToArray(),
                Inventory = state.Inventory
                    .Select(x => x with { Number = x.Kind == payload.Kind ? x.Number - 1 : x.Number })
                    .ToArray()
            });
        });
    }
    
    public TransformResult<WarheadLauncherState> Fire(WarheadLauncherState state, FireWarheadPayload payload,
        DateTimeOffset commandTimestamp)
    {
        return state.IfFunctional(() =>
        {
            return state.Loaded.FirstOrNone(x => x == payload.Kind).Case(
                some: _ =>
                {
                    var firstIndex = Array.IndexOf(state.Loaded, payload.Kind);
                    var loaded = state.Loaded.Where((_, i) => i != firstIndex).ToArray();
                    var lastFired = new FiredWarhead
                    {
                        Kind = payload.Kind,
                        Target = payload.Target,
                        FiredAt = commandTimestamp
                    };
                    return TransformResult<WarheadLauncherState>.StateChanged(state with
                    {
                        Loaded = loaded,
                        LastFiredWarhead = lastFired
                    });
                },
                none: () => TransformResult<WarheadLauncherState>.Error($"no {payload.Kind} loaded to fire"));
        });
    }
    
    public TransformResult<WarheadLauncherState> SetCurrentPower(WarheadLauncherState state, string systemName, CurrentPowerPayload payload)
    {
        return standardTransforms.SetCurrentPower(state, systemName, payload);
    }
    
    public TransformResult<WarheadLauncherState> SetRequiredPower(WarheadLauncherState state, string systemName, RequiredPowerPayload payload)
    {
        return standardTransforms.SetRequiredPower(state, systemName, payload);
    }
    
    public TransformResult<WarheadLauncherState> SetDamaged(WarheadLauncherState state, string systemName, DamagedSystemsPayload payload)
    {
        return standardTransforms.SetDamaged(state, systemName, payload);
    }

    public TransformResult<WarheadLauncherState> SetDisabled(WarheadLauncherState state, string systemName, DisabledSystemsPayload payload)
    {
        return standardTransforms.SetDisabled(state, systemName, payload);
    }

    public TransformResult<WarheadLauncherState> SetInventory(WarheadLauncherState state, WarheadInventoryPayload payload)
    {
        return TransformResult<WarheadLauncherState>.StateChanged(state with { Inventory = payload.Inventory });
    }
}