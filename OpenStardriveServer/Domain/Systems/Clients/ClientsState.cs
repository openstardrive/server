using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenStardriveServer.Domain.Systems.Clients;

public record ClientsState
{
    public List<Client> Clients { get; init; } = new();
}

public record Client
{
    public Guid ClientId { get; init; }
        
    [JsonIgnore]
    public string ClientSecret { get; init; }
        
    public string Name { get; init; }
    public string ClientType { get; init; }
    public string Operator { get; init; }
    public string CurrentScreen { get; init; }
    public bool Disabled { get; init; }
    public string DisabledMessage { get; init; }
}