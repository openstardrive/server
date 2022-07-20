using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenStardriveServer.Domain.Systems.Clients;

public class ClientsSystem : SystemBase<ClientsState>
{
    public const string Name = "clients";

    public ClientsSystem(IClientsTransformations transformations)
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