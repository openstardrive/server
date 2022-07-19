using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Thrusters
{
    public class ThrustersSystem : SystemBase<ThrustersState>
    {
        private readonly StandardSystemBaseStateTransformations<ThrustersState> standardTransformations = new();
        private readonly ThrusterTransformations transformations = new();

        public ThrustersSystem()
        {
            SystemName = "thrusters";
            CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
            {
                ["set-thrusters-power"] = (c) => Update(c, standardTransformations.SetCurrentPower(state, Json.Deserialize<SystemPowerPayload>(c.Payload))),
                ["set-thrusters-damaged"] = (c) => Update(c, standardTransformations.SetDamage(state, Json.Deserialize<SystemDamagePayload>(c.Payload))),
                ["set-thrusters-disabled"] = (c) => Update(c, standardTransformations.SetDisabled(state, Json.Deserialize<SystemDisabledPayload>(c.Payload))),
                ["configure-thrusters"] = (c) => Update(c, transformations.Configure(state, Json.Deserialize<ThrusterConfigurationPayload>(c.Payload))),
                ["set-thrusters-attitude"] = (c) => Update(c, transformations.SetAttitude(state, Json.Deserialize<ThrusterAttitudePayload>(c.Payload))),
                ["set-thrusters-velocity"] = (c) => Update(c, transformations.SetVelocity(state, Json.Deserialize<ThrusterVelocityPayload>(c.Payload)))
            };
        }
    }
}