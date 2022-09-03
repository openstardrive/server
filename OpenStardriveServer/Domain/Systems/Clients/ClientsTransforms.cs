using System;
using System.Linq;

namespace OpenStardriveServer.Domain.Systems.Clients;

public interface IClientsTransforms
{
    TransformResult<ClientsState> RegisterClient(ClientsState state, RegisterClientPayload payload);
    TransformResult<ClientsState> SetOperator(ClientsState state, ClientOperatorPayload payload);
    TransformResult<ClientsState> SetCurrentScreen(ClientsState state, ClientCurrentScreenPayload payload);
    TransformResult<ClientsState> DisableClient(ClientsState state, DisableClientPayload payload);
}

public class ClientsTransforms : IClientsTransforms
{
    public TransformResult<ClientsState> RegisterClient(ClientsState state, RegisterClientPayload payload)
    {
        return (payload.ClientId == Guid.Empty).MaybeIf("Invalid clientId")
            .OrElse(() => string.IsNullOrEmpty(payload.ClientSecret).MaybeIf("Invalid clientSecret"))
            .OrElse(() => string.IsNullOrEmpty(payload.Name).MaybeIf("Invalid name"))
            .Case(some: TransformResult<ClientsState>.Error, none: () => TransformResult<ClientsState>.StateChanged(state with 
            {
                Clients = state.Clients.Append(new Client
                {
                    ClientId = payload.ClientId,
                    ClientSecret = payload.ClientSecret,
                    Name = payload.Name,
                    ClientType = payload.ClientType
                }).ToList() 
            }));
    }

    public TransformResult<ClientsState> SetOperator(ClientsState state, ClientOperatorPayload payload)
    {
        return UpdateClient(state, payload.ClientId, client => client with
        {
            Operator = payload.Operator
        });
    }

    public TransformResult<ClientsState> SetCurrentScreen(ClientsState state, ClientCurrentScreenPayload payload)
    {
        return UpdateClient(state, payload.ClientId, client => client with
        {
            CurrentScreen = payload.CurrentScreen
        });
    }

    public TransformResult<ClientsState> DisableClient(ClientsState state, DisableClientPayload payload)
    {
        return UpdateClient(state, payload.ClientId, client => client with
        {
            Disabled = payload.Disabled,
            DisabledMessage = payload.DisabledMessage
        });
    }

    private TransformResult<ClientsState> UpdateClient(ClientsState state, Guid clientId, Func<Client, Client> update)
    {
        return state.Clients.FirstOrNone(x => x.ClientId == clientId).Case(
            some: client => TransformResult<ClientsState>.StateChanged(state with
            {
                Clients = state.Clients
                    .Replace(x => x.ClientId == client.ClientId, update)
                    .ToList()
            }), 
            none: () => TransformResult<ClientsState>.Error($"Unable to find client with clientId: {clientId}"));
    }
}