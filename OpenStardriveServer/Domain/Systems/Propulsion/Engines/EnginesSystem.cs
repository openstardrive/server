using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Engines;

public abstract class EnginesSystem : SystemBase<EnginesState>
{
    protected EnginesSystem(string systemName, EnginesState initialState, IEnginesTransformations transformations)
    {
        SystemName = systemName;
        state = initialState;
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            [$"set-{SystemName}-speed"] = c => Update(c, transformations.SetSpeed(state, Json.Deserialize<SetSpeedPayload>(c.Payload))),
            [$"configure-{SystemName}"] = c => Update(c, transformations.Configure(state, Json.Deserialize<EnginesConfigurationPayload>(c.Payload))),
            [ChronometerCommand.Type] = c => Update(c, transformations.UpdateHeat(state, Json.Deserialize<ChronometerPayload>(c.Payload))),
            [$"set-{SystemName}-power"] = (c) => Update(c, transformations.SetCurrentPower(state, Json.Deserialize<SystemPowerPayload>(c.Payload))),
            [$"set-{SystemName}-damaged"] = (c) => Update(c, transformations.SetDamage(state, Json.Deserialize<SystemDamagePayload>(c.Payload))),
            [$"set-{SystemName}-disabled"] = (c) => Update(c, transformations.SetDisabled(state, Json.Deserialize<SystemDisabledPayload>(c.Payload)))
        };
    }
}

public class FtlEnginesSystem : EnginesSystem
{
    public FtlEnginesSystem(IEnginesTransformations transformations)
        : base("ftl-engines", EnginesStateDefaults.Ftl, transformations)
    { }
}

public class SublightEnginesSystem : EnginesSystem
{
    public SublightEnginesSystem(IEnginesTransformations transformations)
        : base("sublight-engines", EnginesStateDefaults.Sublight, transformations)
    { }
}