using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Chronometer;

namespace OpenStardriveServer.Domain.Systems.Power;

public class PowerSystem : SystemBase<PowerState>
{
    public PowerSystem(IJson json, IPowerTransforms transforms) : base(json)
    {
        SystemName = "power";
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["report-state"] = c => Update(c, TransformResult<PowerState>.StateChanged(state)),
            ["configure-power"] = c => Update(c, transforms.Configure(state, Payload<ConfigurePowerPayload>(c))),
            ["set-battery-damage"] = c => Update(c, transforms.SetBatteryDamage(state, Payload<BatteryDamagePayload>(c))),
            ["set-battery-charge"] = c => Update(c, transforms.SetBatteryCharge(state,Payload<BatteryChargePayload>(c))),
            [ChronometerCommand.Type] = c =>
            {
                var oldState = state;
                var changed = Update(c, transforms.UpdatePower(state, Payload<ChronometerPayload>(c)));
                return state == oldState with { MillisecondsUntilNextUpdate = state.MillisecondsUntilNextUpdate }
                    ? CommandResult.NoChange(c, SystemName)
                    : changed;
            }
        };
    }

    public void SetStateForTesting(PowerState newState)
    {
        state = newState;
    }
}