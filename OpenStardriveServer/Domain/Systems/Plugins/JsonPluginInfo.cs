using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenStardriveServer.Domain.Systems.Plugins; 

public record JsonPluginInfo
{
    public string Name { get; init; }
    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtensionData { get; set; }
}
