using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Defense.WarheadLauncher;

public class WarheadLauncherSystem : SystemBase<WarheadLauncherState>, IPoweredSystem
{
    public WarheadLauncherSystem(IWarheadLauncherTransforms transforms, IJson json) : base(json)
    {
        SystemName = "warhead-launcher";
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["report-state"] = c => Update(c, TransformResult<WarheadLauncherState>.StateChanged(state)),
            ["load-warhead"] = c => Update(c, transforms.Load(state, Payload<LoadWarheadPayload>(c))),
            ["fire-warhead"] = c => Update(c, transforms.Fire(state, Payload<FireWarheadPayload>(c), c.TimeStamp)),
            ["set-power"] = c => Update(c, transforms.SetCurrentPower(state, SystemName, Payload<CurrentPowerPayload>(c))),
            ["set-required-power"] = c => Update(c, transforms.SetRequiredPower(state, SystemName, Payload<RequiredPowerPayload>(c))),
            ["set-damaged"] = c => Update(c, transforms.SetDamaged(state, SystemName, Payload<DamagedSystemsPayload>(c))),
            ["set-disabled"] = c => Update(c, transforms.SetDisabled(state, SystemName, Payload<DisabledSystemsPayload>(c))),
            ["set-warhead-inventory"] = c => Update(c, transforms.SetInventory(state, Payload<WarheadInventoryPayload>(c)))
        };
    }

    public int CurrentPower => state.CurrentPower;
}