using System;

namespace OpenStardriveServer.Domain.Systems.Clients
{
    public class RegisterClientPayload
    {
        public Guid ClientId { get; set; }
        
        public string ClientSecret { get; set; }
        
        public string Name { get; set; }
    }
}