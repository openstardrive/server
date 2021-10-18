using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenStardriveServer.Domain.Systems.Clients
{
    public class ClientsState
    {
        public List<Client> Clients { get; set; } = new List<Client>();
    }

    public class Client
    {
        public Guid ClientId { get; set; }
        
        [JsonIgnore]
        public string ClientSecret { get; set; }
        
        public string Name { get; set; }
    }
}