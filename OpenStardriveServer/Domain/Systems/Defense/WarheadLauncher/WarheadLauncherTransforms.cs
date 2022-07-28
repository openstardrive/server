using System;
using System.Linq;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Defense.WarheadLauncher;

public interface IWarheadLauncherTransforms
{
    TransformResult<WarheadLauncherState> Load(WarheadLauncherState state, LoadWarheadPayload payload);
    TransformResult<WarheadLauncherState> Fire(WarheadLauncherState state, FireWarheadPayload payload, DateTimeOffset commandTimestamp);
    TransformResult<WarheadLauncherState> SetPower(WarheadLauncherState state, string systemName, CurrentPowerPayload payload);
    TransformResult<WarheadLauncherState> SetRequiredPower(WarheadLauncherState state, string systemName, RequiredPowerPayload payload);
    TransformResult<WarheadLauncherState> SetDamaged(WarheadLauncherState state, SystemDamagePayload payload);
    TransformResult<WarheadLauncherState> SetDisabled(WarheadLauncherState state, SystemDisabledPayload payload);
    TransformResult<WarheadLauncherState> SetInventory(WarheadLauncherState state, WarheadInventoryPayload payload);
}

public class WarheadLauncherTransforms : IWarheadLauncherTransforms
{
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
                Loaded = state.Loaded.Concat(new[] { payload.Kind }).ToArray(),
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
                    var loaded = state.Loaded.Select((y, index) => index == firstIndex ? null : y).Where(y => y != null).ToArray();
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
    
    public TransformResult<WarheadLauncherState> SetPower(WarheadLauncherState state, string systemName, CurrentPowerPayload payload)
    {
        return payload.ValueOrNone(systemName).Case(
            some: newPower => TransformResult<WarheadLauncherState>.StateChanged(state with { CurrentPower = newPower }),
            none: TransformResult<WarheadLauncherState>.NoChange);
    }
    
    public TransformResult<WarheadLauncherState> SetRequiredPower(WarheadLauncherState state, string systemName, RequiredPowerPayload payload)
    {
        return payload.ValueOrNone(systemName).Case(
            some: newRequired => TransformResult<WarheadLauncherState>.StateChanged(state with { RequiredPower = newRequired }),
            none: TransformResult<WarheadLauncherState>.NoChange);
    }
    
    public TransformResult<WarheadLauncherState> SetDamaged(WarheadLauncherState state, SystemDamagePayload payload)
    {
        return TransformResult<WarheadLauncherState>.StateChanged(state with { Damaged = payload.Damaged });
    }

    public TransformResult<WarheadLauncherState> SetDisabled(WarheadLauncherState state, SystemDisabledPayload payload)
    {
        return TransformResult<WarheadLauncherState>.StateChanged(state with { Disabled = payload.Disabled });
    }

    public TransformResult<WarheadLauncherState> SetInventory(WarheadLauncherState state, WarheadInventoryPayload payload)
    {
        return TransformResult<WarheadLauncherState>.StateChanged(state with { Inventory = payload.Inventory });
    }
}