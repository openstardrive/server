using System;
using System.Collections.Generic;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Thrusters
{
    public class ThrustersSystem : SystemBase<ThrustersState>
    {
        private readonly ThrusterTransformations transformations = new();

        public ThrustersSystem()
        {
            SystemName = "thrusters";
            CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
            {
                ["configure-thrusters"] = (c) => Update(c, transformations.Configure(state, Json.Deserialize<ThrusterConfigurationPayload>(c.Payload))),
                ["set-thruster-attitude"] = (c) => Update(c, transformations.SetAttitude(state, Json.Deserialize<ThrusterAttitudePayload>(c.Payload))),
                ["set-thruster-velocity"] = (c) => Update(c, transformations.SetVelocity(state, Json.Deserialize<ThrusterVelocityPayload>(c.Payload)))
            };
        }
    }
}