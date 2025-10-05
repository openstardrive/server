using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenStardriveServer.Domain.Integrations;

// Thorium-specific team models - supports multiple formats
public record ThoriumTeamsPayload
{
    [JsonPropertyName("teams")]
    public ThoriumTeam[] Teams { get; init; } = Array.Empty<ThoriumTeam>();
}

public record ThoriumTeam
{
    [JsonPropertyName("id")]
    public string Id { get; init; }
    
    [JsonPropertyName("class")]
    public string Class { get; init; } // Usually "Team" 
    
    [JsonPropertyName("simulatorId")]
    public string SimulatorId { get; init; }
    
    [JsonPropertyName("type")]
    public string Type { get; init; }
    
    [JsonPropertyName("name")]
    public string Name { get; init; }
    
    [JsonPropertyName("location")]
    public JsonElement Location { get; init; } // Can be string ID or location object
    
    [JsonPropertyName("priority")]
    public string Priority { get; init; }
    
    [JsonPropertyName("orders")]
    public string Orders { get; init; }
    
    [JsonPropertyName("officers")]
    public JsonElement Officers { get; init; } // Can be string[] or ThoriumOfficer[]
    
    [JsonPropertyName("cleared")]
    public bool Cleared { get; init; }
}

public record ThoriumOfficer
{
    [JsonPropertyName("id")]
    public string Id { get; init; }
    
    [JsonPropertyName("name")]
    public string Name { get; init; }
    
    [JsonPropertyName("position")]
    public string Position { get; init; }
    
    [JsonPropertyName("inventory")]
    public object[] Inventory { get; init; } = Array.Empty<object>(); // Can be empty or contain items
}