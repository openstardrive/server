using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Comms.LongRange;

public class LongRangeSystem : SystemBase<LongRangeState>, IPoweredSystem
{
    public LongRangeSystem(IJson json, ILongRangeTransforms transforms) : base(json)
    {
        SystemName = "long-range-comms";
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["report-state"] = c => Update(c, TransformResult<LongRangeState>.StateChanged(state)),
            ["set-disabled"] = c => Update(c, transforms.SetDisabled(state, SystemName, Payload<DisabledSystemsPayload>(c))),
            ["set-damaged"] = c => Update(c, transforms.SetDamaged(state, SystemName, Payload<DamagedSystemsPayload>(c))),
            ["set-power"] = c => Update(c, transforms.SetCurrentPower(state, SystemName, Payload<CurrentPowerPayload>(c))),
            ["set-required-power"] = c => Update(c, transforms.SetRequiredPower(state, SystemName, Payload<RequiredPowerPayload>(c))),
            ["set-long-range-cypher"] = c => Update(c, transforms.SetCypher(state, Payload<SetCypherPayload>(c))),
            ["update-long-range-substitutions"] = c => Update(c, transforms.UpdateCypherSubstitutions(state, Payload<UpdateCypherSubstitutionsPayload>(c))),
            ["send-long-range-message"] = c => Update(c, transforms.SendLongRangeMessage(state, Payload<LongRangeMessagePayload>(c)))
        };
    }

    public int CurrentPower => state.CurrentPower;
}