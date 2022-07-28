using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Defense.Shields;

public class ShieldsSystem : SystemBase<ShieldsState>
{
    public ShieldsSystem(IShieldTransformations transformations, IJson json) : base(json)
    {
        SystemName = "shields";
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["report-state"] = c => Update(c, TransformResult<ShieldsState>.StateChanged(state)),
            ["set-power"] = c => Update(c, transformations.SetPower(state, SystemName, Payload<CurrentPowerPayload>(c))),
            ["set-required-power"] = c => Update(c, transformations.SetRequiredPower(state, SystemName, Payload<RequiredPowerPayload>(c))),
            ["set-damaged"] = c => Update(c, transformations.SetDamaged(state, SystemName, Payload<DamagedSystemsPayload>(c))),
            ["set-disabled"] = c => Update(c, transformations.SetDisabled(state, SystemName, Payload<DisabledSystemsPayload>(c))),
            ["raise-shields"] = c => Update(c, transformations.RaiseShields(state)),
            ["lower-shields"] = c => Update(c, transformations.LowerShields(state)),
            ["modulate-shields"] = c => Update(c, transformations.SetModulationFrequency(state, Payload<ShieldModulationPayload>(c))),
            ["set-shield-strengths"] = c => Update(c, transformations.SetSectionStrengths(state, Payload<ShieldStrengthPayload>(c)))
        };
    }
}