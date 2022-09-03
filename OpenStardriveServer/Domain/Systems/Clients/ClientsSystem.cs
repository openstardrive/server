using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenStardriveServer.Domain.Systems.Clients;

public class ClientsSystem : SystemBase<ClientsState>
{
    public const string Name = "clients";

    public ClientsSystem(IClientsTransforms transforms, IJson json) : base(json)
    {
        SystemName = Name;
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["report-state"] = c => Update(c, TransformResult<ClientsState>.StateChanged(state)),
            ["register-client"] = c => Update(c, transforms.RegisterClient(state, Payload<RegisterClientPayload>(c))),
            ["set-client-operator"] = c => Update(c, transforms.SetOperator(state, Payload<ClientOperatorPayload>(c))),
            ["set-client-screen"] = c => Update(c, transforms.SetCurrentScreen(state, Payload<ClientCurrentScreenPayload>(c))),
            ["set-client-disabled"] = c => Update(c, transforms.DisableClient(state, Payload<DisableClientPayload>(c)))
        };
    }
    
    public Maybe<Client> FindClientBySecret(string secret)
    {
        return state.Clients.Where(x => x.ClientSecret == secret).FirstOrNone();
    }
}