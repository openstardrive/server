using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Engines;

public abstract class EnginesSystem : SystemBase<EnginesState>
{
    protected EnginesSystem(string systemName, EnginesState initialState, IEnginesTransformations transformations, IJson json)
    {
        SystemName = systemName;
        state = initialState;
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["report-state"] = (c) => Update(c, TransformResult<EnginesState>.StateChanged(state)),
            [$"set-{SystemName}-speed"] = c => Update(c, transformations.SetSpeed(state, json.Deserialize<SetSpeedPayload>(c.Payload))),
            [$"configure-{SystemName}"] = c => Update(c, transformations.Configure(state, json.Deserialize<EnginesConfigurationPayload>(c.Payload))),
            [ChronometerCommand.Type] = c => Update(c, transformations.UpdateHeat(state, json.Deserialize<ChronometerPayload>(c.Payload))),
            [$"set-{SystemName}-power"] = (c) => Update(c, transformations.SetCurrentPower(state, json.Deserialize<SystemPowerPayload>(c.Payload))),
            [$"set-{SystemName}-damaged"] = (c) => Update(c, transformations.SetDamage(state, json.Deserialize<SystemDamagePayload>(c.Payload))),
            [$"set-{SystemName}-disabled"] = (c) => Update(c, transformations.SetDisabled(state, json.Deserialize<SystemDisabledPayload>(c.Payload)))
        };
    }
}

public class FtlEnginesSystem : EnginesSystem
{
    public FtlEnginesSystem(IEnginesTransformations transformations, IJson json)
        : base("ftl-engines", EnginesStateDefaults.Ftl, transformations, json)
    { }
}

public class SublightEnginesSystem : EnginesSystem
{
    public SublightEnginesSystem(IEnginesTransformations transformations, IJson json)
        : base("sublight-engines", EnginesStateDefaults.Sublight, transformations, json)
    { }
}