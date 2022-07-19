using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Engines
{
    public class EnginesSystem : SystemBase<EnginesState>
    {
        private StandardSystemBaseStateTransformations<EnginesState> standardTransformations = new();
        private EnginesTransformations transformations = new();

        public EnginesSystem(string name, EnginesState initialState)
        {
            SystemName = name;
            state = initialState;
            CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
            {
                [$"set-{SystemName}-engines-power"] = (c) => Update(c, standardTransformations.SetCurrentPower(state, Json.Deserialize<SystemPowerPayload>(c.Payload))),
                [$"set-{SystemName}-engines-damaged"] = (c) => Update(c, standardTransformations.SetDamage(state, Json.Deserialize<SystemDamagePayload>(c.Payload))),
                [$"set-{SystemName}-engines-disabled"] = (c) => Update(c, standardTransformations.SetDisabled(state, Json.Deserialize<SystemDisabledPayload>(c.Payload))),
                [$"set-{SystemName}-engines-speed"] = c => Update(c, transformations.SetSpeed(state, Json.Deserialize<SetSpeedPayload>(c.Payload))),
                [$"configure-{SystemName}-engines"] = c => Update(c, transformations.Configure(state, Json.Deserialize<EnginesConfigurationPayload>(c.Payload))),
                [ChronometerCommand.Type] = c => Update(c, transformations.UpdateHeat(state, Json.Deserialize<ChronometerPayload>(c.Payload)))
            };
        }
    }
}