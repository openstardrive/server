using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Defense.WarheadLauncher;

public class WarheadLauncherSystem : SystemBase<WarheadLauncherState>
{
    public WarheadLauncherSystem(IWarheadLauncherTransforms transforms, IJson json) : base(json)
    {
        SystemName = "warhead-launcher";
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["report-state"] = c => Update(c, TransformResult<WarheadLauncherState>.StateChanged(state)),
            ["load-warhead"] = c => Update(c, transforms.Load(state, Payload<LoadWarheadPayload>(c))),
            ["fire-warhead"] = c => Update(c, transforms.Fire(state, Payload<FireWarheadPayload>(c), c.TimeStamp)),
            ["set-power"] = c => Update(c, transforms.SetPower(state, SystemName, Payload<CurrentPowerPayload>(c))),
            ["set-warhead-launcher-damaged"] = c => Update(c, transforms.SetDamaged(state, Payload<SystemDamagePayload>(c))),
            ["set-warhead-launcher-disabled"] = c => Update(c, transforms.SetDisabled(state, Payload<SystemDisabledPayload>(c))),
            ["set-warhead-inventory"] = c => Update(c, transforms.SetInventory(state, Payload<WarheadInventoryPayload>(c)))
        };
    }
}