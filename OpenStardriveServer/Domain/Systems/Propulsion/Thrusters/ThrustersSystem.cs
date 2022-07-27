using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Thrusters;

public class ThrustersSystem : SystemBase<ThrustersState>
{
    public ThrustersSystem(IThrusterTransformations transformations, IJson json)
    {
        SystemName = "thrusters";
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["report-state"] = (c) => Update(c, TransformResult<ThrustersState>.StateChanged(state)),
            ["configure-thrusters"] = (c) => Update(c, transformations.Configure(state, json.Deserialize<ThrusterConfigurationPayload>(c.Payload))),
            ["set-thrusters-attitude"] = (c) => Update(c, transformations.SetAttitude(state, json.Deserialize<ThrusterAttitudePayload>(c.Payload))),
            ["set-thrusters-velocity"] = (c) => Update(c, transformations.SetVelocity(state, json.Deserialize<ThrusterVelocityPayload>(c.Payload))),
            ["set-thrusters-power"] = (c) => Update(c, transformations.SetCurrentPower(state, json.Deserialize<SystemPowerPayload>(c.Payload))),
            ["set-thrusters-damaged"] = (c) => Update(c, transformations.SetDamage(state, json.Deserialize<SystemDamagePayload>(c.Payload))),
            ["set-thrusters-disabled"] = (c) => Update(c, transformations.SetDisabled(state, json.Deserialize<SystemDisabledPayload>(c.Payload)))
        };
    }
}