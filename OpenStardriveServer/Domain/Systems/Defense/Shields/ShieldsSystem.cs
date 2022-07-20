using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Defense.Shields;

public class ShieldsSystem : SystemBase<ShieldsState>
{
    private readonly ShieldTransformations transformations = new();

    public ShieldsSystem()
    {
        SystemName = $"shields";
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["set-shields-power"] = (c) => Update(c, transformations.SetPower(state, Json.Deserialize<SystemPowerPayload>(c.Payload))),
            ["set-shields-damaged"] = (c) => Update(c, transformations.SetDamaged(state, Json.Deserialize<SystemDamagePayload>(c.Payload))),
            ["set-shields-disabled"] = (c) => Update(c, transformations.SetDisabled(state, Json.Deserialize<SystemDisabledPayload>(c.Payload))),
            ["raise-shields"] = (c) => Update(c, transformations.RaiseShields(state)),
            ["lower-shields"] = (c) => Update(c, transformations.LowerShields(state)),
            ["modulate-shields"] = (c) => Update(c, transformations.SetModulationFrequency(state, Json.Deserialize<ShieldModulationPayload>(c.Payload)))
        };
    }
}