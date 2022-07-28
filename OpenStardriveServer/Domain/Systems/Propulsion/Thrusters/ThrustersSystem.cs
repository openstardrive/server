using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Thrusters;

public class ThrustersSystem : SystemBase<ThrustersState>
{
    public ThrustersSystem(IThrusterTransformations transformations, IJson json) : base(json)
    {
        SystemName = "thrusters";
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["report-state"] = c => Update(c, TransformResult<ThrustersState>.StateChanged(state)),
            ["set-thrusters-attitude"] = c => Update(c, transformations.SetAttitude(state, Payload<ThrusterAttitudePayload>(c))),
            ["set-thrusters-velocity"] = c => Update(c, transformations.SetVelocity(state, Payload<ThrusterVelocityPayload>(c))),
            ["set-power"] = c => Update(c, transformations.SetCurrentPower(state, SystemName, Payload<CurrentPowerPayload>(c))),
            ["set-required-power"] = c => Update(c, transformations.SetRequiredPower(state, SystemName, Payload<RequiredPowerPayload>(c))),
            ["set-thrusters-damaged"] = c => Update(c, transformations.SetDamage(state, Payload<SystemDamagePayload>(c))),
            ["set-thrusters-disabled"] = c => Update(c, transformations.SetDisabled(state, Payload<SystemDisabledPayload>(c)))
        };
    }
}