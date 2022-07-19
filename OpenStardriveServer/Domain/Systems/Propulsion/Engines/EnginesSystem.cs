using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Engines
{
    public class EnginesSystem : StandardSystemBase<EnginesState>
    {
        private readonly EnginesTransformations transformations = new();

        public EnginesSystem(string name, EnginesState initialState)
        {
            SystemName = $"{name}-engines";
            state = initialState;
            CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
            {
                [$"set-{SystemName}-speed"] = c => Update(c, transformations.SetSpeed(state, Json.Deserialize<SetSpeedPayload>(c.Payload))),
                [$"configure-{SystemName}"] = c => Update(c, transformations.Configure(state, Json.Deserialize<EnginesConfigurationPayload>(c.Payload))),
                [ChronometerCommand.Type] = c => Update(c, transformations.UpdateHeat(state, Json.Deserialize<ChronometerPayload>(c.Payload)))
            };
            AddStandardTransforms();
        }
    }
}