using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Thrusters;

public class ThrustersSystem : SystemBase<ThrustersState>, IPoweredSystem
{
    public ThrustersSystem(IThrusterTransforms transforms, IJson json) : base(json)
    {
        SystemName = "thrusters";
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["report-state"] = c => Update(c, TransformResult<ThrustersState>.StateChanged(state)),
            ["set-thrusters-attitude"] = c => Update(c, transforms.SetAttitude(state, Payload<ThrusterAttitudePayload>(c))),
            ["set-thrusters-velocity"] = c => Update(c, transforms.SetVelocity(state, Payload<ThrusterVelocityPayload>(c))),
            ["set-power"] = c => Update(c, transforms.SetCurrentPower(state, SystemName, Payload<CurrentPowerPayload>(c))),
            ["set-required-power"] = c => Update(c, transforms.SetRequiredPower(state, SystemName, Payload<RequiredPowerPayload>(c))),
            ["set-damaged"] = c => Update(c, transforms.SetDamaged(state, SystemName, Payload<DamagedSystemsPayload>(c))),
            ["set-disabled"] = c => Update(c, transforms.SetDisabled(state, SystemName, Payload<DisabledSystemsPayload>(c)))
        };
    }

    public int CurrentPower => state.CurrentPower;
}