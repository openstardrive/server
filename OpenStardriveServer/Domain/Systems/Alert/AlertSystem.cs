using System;
using System.Collections.Generic;

namespace OpenStardriveServer.Domain.Systems.Alert;

public class AlertSystem : SystemBase<AlertState>
{
    public AlertSystem(IJson json, IAlertTransforms transforms) : base(json)
    {
        SystemName = "alert";
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["report-state"] = c => Update(c, TransformResult<AlertState>.StateChanged(state)),
            ["configure-alert-levels"] = c => Update(c, transforms.Configure(Payload<ConfigureAlertLevelsPayload>(c))),
            ["set-alert-level"] = c => Update(c, transforms.SetLevel(state, Payload<SetAlertLevelPayload>(c)))
        };
    }
}