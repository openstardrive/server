using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Plugins;

public record JsonPluginState : StandardSystemBaseState { 
    public Dictionary<string, object> JsonState { get; init; } = [];
}