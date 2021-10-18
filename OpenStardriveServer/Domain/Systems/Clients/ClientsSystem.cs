using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenStardriveServer.Domain.Systems.Clients
{
    public class ClientsSystem : ISystem
    {
        public static string Name = "clients";
        
        private ClientsState state = new ClientsState();
        private readonly ClientsTransformations transformations = new ClientsTransformations();

        public string SystemName => Name;

        public Dictionary<string, Func<Command, CommandResult>> CommandProcessors => new Dictionary<string, Func<Command, CommandResult>>
        {
            ["register-client"] = (c) =>
            {
                var result = transformations.RegisterClient(state, Json.Deserialize<RegisterClientPayload>(c.Payload));
                result.NewState.IfSome(x => state = x);
                return result.ToCommandResult(c, SystemName);
            }
        };

        public Maybe<Client> FindClientBySecret(string secret)
        {
            return state.Clients.Where(x => x.ClientSecret == secret).FirstOrNone();
        }
    }
}
