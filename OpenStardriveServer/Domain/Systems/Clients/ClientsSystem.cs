using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenStardriveServer.Domain.Systems.Clients;

public class ClientsSystem : SystemBase<ClientsState>
{
    public const string Name = "clients";

    public ClientsSystem(IClientsTransformations transformations, IJson json) : base(json)
    {
        SystemName = Name;
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["report-state"] = (c) => Update(c, TransformResult<ClientsState>.StateChanged(state)),
            ["register-client"] = (c) => Update(c, transformations.RegisterClient(state, Payload<RegisterClientPayload>(c)))
        };
    }
    
    public Maybe<Client> FindClientBySecret(string secret)
    {
        return state.Clients.Where(x => x.ClientSecret == secret).FirstOrNone();
    }
}