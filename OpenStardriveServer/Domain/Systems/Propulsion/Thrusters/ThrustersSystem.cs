using System;
using System.Collections.Generic;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Thrusters
{
    public class ThrustersSystem : ISystem
    {
        private ThrustersState state = new()
        {
            Attitude = new ThrustersAttitude(),
            Velocity = new ThrusterVelocity()
        };
        private readonly ThrusterTransformations transformations = new();

        public string SystemName => "thrusters";

        public Dictionary<string, Func<Command, CommandResult>> CommandProcessors => new()
        {
            ["configure-thrusters"] = (c) => Update(c, transformations.Configure(state, Json.Deserialize<ThrusterConfigurationPayload>(c.Payload))),
            ["set-thruster-attitude"] = (c) => Update(c, transformations.SetAttitude(state, Json.Deserialize<ThrusterAttitudePayload>(c.Payload))),
            ["set-thruster-velocity"] = (c) => Update(c, transformations.SetVelocity(state, Json.Deserialize<ThrusterVelocityPayload>(c.Payload)))
        };
        
        private CommandResult Update(Command command, TransformResult<ThrustersState> result)
        {
            result.NewState.IfSome(x => state = x);
            return result.ToCommandResult(command, SystemName);
        }
    }
}