using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Defense.WarheadLauncher;

public class WarheadLauncherSystem : SystemBase<WarheadLauncherState>
{
    public WarheadLauncherSystem(IWarheadLauncherTransforms transforms)
    {
        SystemName = "warhead-launcher";
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["report-state"] = (c) => Update(c, TransformResult<WarheadLauncherState>.StateChanged(state)),
            ["load-warhead"] = c => Update(c, transforms.Load(state, Json.Deserialize<LoadWarheadPayload>(c.Payload))),
            ["fire-warhead"] = c => Update(c, transforms.Fire(state, Json.Deserialize<FireWarheadPayload>(c.Payload), c.TimeStamp)),
            ["set-warhead-launcher-power"] = c => Update(c, transforms.SetPower(state, Json.Deserialize<SystemPowerPayload>(c.Payload))),
            ["set-warhead-launcher-damaged"] = c => Update(c, transforms.SetDamaged(state, Json.Deserialize<SystemDamagePayload>(c.Payload))),
            ["set-warhead-launcher-disabled"] = c => Update(c, transforms.SetDisabled(state, Json.Deserialize<SystemDisabledPayload>(c.Payload))),
            ["set-warhead-inventory"] = c => Update(c, transforms.SetInventory(state, Json.Deserialize<WarheadInventoryPayload>(c.Payload)))
        };
    }
}