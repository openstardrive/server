using System;

namespace OpenStardriveServer.Domain.Systems.Clients
{
    public record RegisterClientPayload
    {
        public Guid ClientId { get; init; }
        
        public string ClientSecret { get; init; }
        
        public string Name { get; init; }
    }
}