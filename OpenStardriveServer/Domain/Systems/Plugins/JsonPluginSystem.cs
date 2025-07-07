using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Plugins;

public class JsonPluginSystem : SystemBase<JsonPluginState>, IPoweredSystem
{
    public int CurrentPower => throw new NotImplementedException();
    public JsonPluginSystem(IJson json, IJsonPluginTransforms transforms) : base(json)
    {
        SystemName = "json-plugin";
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["report-state"] = c => Update(c, TransformResult<JsonPluginState>.StateChanged(state)),
            ["set-disabled"] = c => Update(c, transforms.SetDisabled(state, SystemName, Payload<DisabledSystemsPayload>(c))),
            ["set-damaged"] = c => Update(c, transforms.SetDamaged(state, SystemName, Payload<DamagedSystemsPayload>(c))),
            ["set-power"] = c => Update(c, transforms.SetCurrentPower(state, SystemName, Payload<CurrentPowerPayload>(c))),
            ["set-required-power"] = c => Update(c, transforms.SetRequiredPower(state, SystemName, Payload<RequiredPowerPayload>(c)))
        };
    }
}
