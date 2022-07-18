using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Chronometer;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Engines
{
    public class EnginesSystem : SystemBase<EnginesState>
    {
        private EnginesTransformations transformations = new();

        public EnginesSystem(string name, EnginesState initialState)
        {
            SystemName = name;
            state = initialState;
            CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
            {
                [ChronometerCommand.Type] = c => Update(c, transformations.UpdateHeat(state, Json.Deserialize<ChronometerPayload>(c.Payload))),
                [$"set-{SystemName}-speed"] = c => Update(c, transformations.SetSpeed(state, Json.Deserialize<SetSpeedPayload>(c.Payload)))
            };
        }
    }
}