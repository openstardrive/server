using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Defense.Shields;

public class ShieldsSystem : SystemBase<ShieldsState>
{
    public ShieldsSystem(IShieldTransforms transforms, IJson json) : base(json)
    {
        SystemName = "shields";
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["report-state"] = c => Update(c, TransformResult<ShieldsState>.StateChanged(state)),
            ["set-power"] = c => Update(c, transforms.SetCurrentPower(state, SystemName, Payload<CurrentPowerPayload>(c))),
            ["set-required-power"] = c => Update(c, transforms.SetRequiredPower(state, SystemName, Payload<RequiredPowerPayload>(c))),
            ["set-damaged"] = c => Update(c, transforms.SetDamaged(state, SystemName, Payload<DamagedSystemsPayload>(c))),
            ["set-disabled"] = c => Update(c, transforms.SetDisabled(state, SystemName, Payload<DisabledSystemsPayload>(c))),
            ["raise-shields"] = c => Update(c, transforms.RaiseShields(state)),
            ["lower-shields"] = c => Update(c, transforms.LowerShields(state)),
            ["modulate-shields"] = c => Update(c, transforms.SetModulationFrequency(state, Payload<ShieldModulationPayload>(c))),
            ["set-shield-strengths"] = c => Update(c, transforms.SetSectionStrengths(state, Payload<ShieldStrengthPayload>(c)))
        };
    }
}