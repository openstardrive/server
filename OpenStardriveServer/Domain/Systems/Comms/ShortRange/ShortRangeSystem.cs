using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Comms.ShortRange;

public class ShortRangeSystem : SystemBase<ShortRangeState>
{
    public ShortRangeSystem(IJson json, IShortRangeTransforms transforms) : base(json)
    {
        SystemName = "short-range-comms";
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["report-state"] = c => Update(c, TransformResult<ShortRangeState>.StateChanged(state)),
            ["set-disabled"] = c => Update(c, transforms.SetDisabled(state, SystemName, Payload<DisabledSystemsPayload>(c))),
            ["set-damaged"] = c => Update(c, transforms.SetDamaged(state, SystemName, Payload<DamagedSystemsPayload>(c))),
            ["set-power"] = c => Update(c, transforms.SetCurrentPower(state, SystemName, Payload<CurrentPowerPayload>(c))),
            ["set-required-power"] = c => Update(c, transforms.SetRequiredPower(state, SystemName, Payload<RequiredPowerPayload>(c))),
            ["configure-short-range-frequencies"] = c => Update(c, transforms.ConfigureFrequencyRanges(state, Payload<ConfigureFrequencyRangesPayload>(c))),
            ["set-short-range-signals"] = c => Update(c, transforms.SetActiveSignals(state, Payload<SetActiveSignalsPayload>(c))),
            ["set-short-range-frequency"] = c => Update(c, transforms.SetCurrentFrequency(state, Payload<SetCurrentFrequencyPayload>(c))),
            ["set-short-range-broadcasting"] = c => Update(c, transforms.SetBroadcasting(state, Payload<SetBroadcastingPayload>(c)))
        };
    }
}