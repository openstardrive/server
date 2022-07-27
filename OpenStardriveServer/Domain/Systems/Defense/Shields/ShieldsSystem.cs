using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Defense.Shields;

public class ShieldsSystem : SystemBase<ShieldsState>
{
    public ShieldsSystem(IShieldTransformations transformations, IJson json)
    {
        SystemName = "shields";
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["report-state"] = (c) => Update(c, TransformResult<ShieldsState>.StateChanged(state)),
            ["set-shields-power"] = (c) => Update(c, transformations.SetPower(state, json.Deserialize<SystemPowerPayload>(c.Payload))),
            ["set-shields-damaged"] = (c) => Update(c, transformations.SetDamaged(state, json.Deserialize<SystemDamagePayload>(c.Payload))),
            ["set-shields-disabled"] = (c) => Update(c, transformations.SetDisabled(state, json.Deserialize<SystemDisabledPayload>(c.Payload))),
            ["raise-shields"] = (c) => Update(c, transformations.RaiseShields(state)),
            ["lower-shields"] = (c) => Update(c, transformations.LowerShields(state)),
            ["modulate-shields"] = (c) => Update(c, transformations.SetModulationFrequency(state, json.Deserialize<ShieldModulationPayload>(c.Payload))),
            ["set-shield-strengths"] = (c) => Update(c, transformations.SetSectionStrengths(state, json.Deserialize<ShieldStrengthPayload>(c.Payload)))
        };
    }
}