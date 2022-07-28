using System;
using System.Collections.Generic;

namespace OpenStardriveServer.Domain.Systems.Clients;

public interface IClientsTransforms
{
    TransformResult<ClientsState> RegisterClient(ClientsState state, RegisterClientPayload payload);
}

public class ClientsTransforms : IClientsTransforms
{
    public TransformResult<ClientsState> RegisterClient(ClientsState state, RegisterClientPayload payload)
    {
        return (payload.ClientId == Guid.Empty).MaybeIf("Invalid clientId")
            .OrElse(() => string.IsNullOrEmpty(payload.ClientSecret).MaybeIf("Invalid clientSecret"))
            .OrElse(() => string.IsNullOrEmpty(payload.Name).MaybeIf("Invalid name"))
            .Case(some: TransformResult<ClientsState>.Error, none: () =>
            {
                var clients = new List<Client>();
                clients.AddRange(state.Clients);
                clients.Add(new Client
                {
                    ClientId = payload.ClientId,
                    ClientSecret = payload.ClientSecret,
                    Name = payload.Name
                });
            
                return TransformResult<ClientsState>.StateChanged(new ClientsState
                {
                    Clients = clients 
                });
            });
    }
}