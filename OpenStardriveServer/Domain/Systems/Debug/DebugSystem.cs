using System;
using System.Collections.Generic;

namespace OpenStardriveServer.Domain.Systems.Debug;

public class DebugSystem : SystemBase<DebugState>
{
    public DebugSystem(IJson json, IDebugTransforms transforms) : base(json)
    {
        SystemName = "debug";
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["debug"] = (c) => Update(c, transforms.AddEntry(state, Payload<DebugPayload>(c)))
        };
    }
}