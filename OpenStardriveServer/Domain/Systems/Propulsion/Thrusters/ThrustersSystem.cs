using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Thrusters
{
    public class ThrustersSystem : StandardSystemBase<ThrustersState>
    {
        private readonly ThrusterTransformations transformations = new();

        public ThrustersSystem()
        {
            SystemName = "thrusters";
            CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
            {
                ["configure-thrusters"] = (c) => Update(c, transformations.Configure(state, Json.Deserialize<ThrusterConfigurationPayload>(c.Payload))),
                ["set-thrusters-attitude"] = (c) => Update(c, transformations.SetAttitude(state, Json.Deserialize<ThrusterAttitudePayload>(c.Payload))),
                ["set-thrusters-velocity"] = (c) => Update(c, transformations.SetVelocity(state, Json.Deserialize<ThrusterVelocityPayload>(c.Payload)))
            };
            AddStandardTransforms();
        }
    }
}