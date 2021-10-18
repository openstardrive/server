using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Chronometer;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Engines
{
    public class EnginesSystem : ISystem
    {
        public string SystemName { get; }
        public Dictionary<string, Func<Command, CommandResult>> CommandProcessors => new Dictionary<string, Func<Command, CommandResult>>
        {
            ["chronometer"] = c => Update(c, transformations.UpdateHeat(state, Json.Deserialize<ChronometerPayload>(c.Payload))),
            [$"set-{SystemName}-speed"] = c => Update(c, transformations.SetSpeed(state, Json.Deserialize<SetSpeedPayload>(c.Payload)))
        };

        private EnginesState state;
        private EnginesTransformations transformations = new EnginesTransformations();

        public EnginesSystem(string name, EnginesState initialState)
        {
            SystemName = name;
            state = initialState;
        }
        
        private CommandResult Update(Command command, TransformResult<EnginesState> result)
        {
            result.NewState.IfSome(x => state = x);
            return result.ToCommandResult(command, SystemName);
        }
    }
}