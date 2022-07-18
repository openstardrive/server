using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenStardriveServer.Domain.Systems.Clients
{
    public class ClientsSystem : SystemBase<ClientsState>
    {
        public static string Name = "clients";
        
        private readonly ClientsTransformations transformations = new();

        public ClientsSystem()
        {
            SystemName = Name;
            CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
            {
                ["register-client"] = (c) => Update(c, transformations.RegisterClient(state, Json.Deserialize<RegisterClientPayload>(c.Payload)))
            };
        }
        
        public Maybe<Client> FindClientBySecret(string secret)
        {
            return state.Clients.Where(x => x.ClientSecret == secret).FirstOrNone();
        }
    }
}
