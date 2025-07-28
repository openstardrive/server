using OpenStardriveServer.Domain.Systems.Propulsion.Engines;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenStardriveServer.Domain.Integrations
{
    public interface IThoriumIntegration
    {
        public void TranslateCommand(Command command);
    }
    public class ThoriumIntegration: IThoriumIntegration
    {
        private class ThoriumIds
        {
            private ThoriumIntegration parent;
            private string simulatorId;
            private string flightId;
            private string sublightId;
            private string warpId;

            public ThoriumIds(ThoriumIntegration parent)
            {
                this.parent = parent;
            }

            public async Task<string> GetSimulatorId()
            {
                if(simulatorId == null)
                {
                    await SetupSimulator();
                }

                return simulatorId;
            }

            public async Task<string> GetFlightId()
            {
                if (flightId == null)
                {
                    await SetupSimulator();
                }
                return flightId;
            }

            public async Task<string> GetSublightId()
            {
                if (sublightId == null)
                {
                    await SetupEngines();
                }
                return sublightId;
            }

            public async Task<string> GetWarpId()
            {
                if (warpId == null)
                {
                    await SetupEngines();
                }
                return warpId;
            }


            private async Task SetupSimulator()
            {
                string flightJson = await SendReq("{\"operationName\":\"Flights\",\"variables\":{},\"query\":\"query Flights {  flights {    id   simulators { id }  }}\"}");
                var flights = parent.json.Deserialize<GraphQlResult<GetFlightsResults>>(flightJson);

                if (flights.data.flights.Count > 0)
                {
                    flightId = flights.data.flights[0].id;
                    simulatorId = flights.data.flights[0].simulators[0].id;
                }
            }

            private async Task SetupEngines()
            {
                await SetupSimulator();

                string enginesJson = await SendReq($@"{{
                      ""operationName"": ""getEngines"",
                      ""variables"": {{
                        ""simulatorId"": ""{simulatorId}""
                      }},
                      ""query"": ""query getEngines($simulatorId: ID!) {{  engines(simulatorId: $simulatorId) {{    id    name    speeds {{      text      number      velocity    }}    velocity    heat    speed    on    stealthFactor  }} }}""
                    }}");

                var engines = parent.json.Deserialize<GraphQlResult<EngineResult>>(enginesJson);

                Console.WriteLine($"Engines: {engines.data.engines.Length}");

                sublightId = engines.data.engines[0].id;
                warpId = engines.data.engines[1].id;
            }

            private Task<string> SendReq(string query)
            {
                return parent.SendReq(query);
            }
        }

        private readonly IJson json;

        private ThoriumIds ids;

        private Dictionary<string, Func<Command, Task<string>>> commandHandlers;

        private static HttpClient httpClient = new()
        {
            BaseAddress = new Uri("http://localhost:3001"),
        };

        public ThoriumIntegration(IJson json)
        {
            this.json = json;
            ids= new ThoriumIds(this); 
            commandHandlers = new Dictionary<string, Func<Command, Task<string>>>
            {
                { "set-sublight-engines-speed", (Command cmd)=>{
                    var payload=json.Deserialize<SetSpeedPayload>(cmd.Payload);
                    string sublightId = ids.GetSublightId().Result;
                    string mutation = $"{{\"operationName\":\"setSpeed\",\"variables\":{{\"on\":true,\"id\":\"{sublightId}\",\"speed\":{payload.Speed}}},\"query\":\"mutation setSpeed($id: ID!, $speed: Int!, $on: Boolean) {{  setSpeed(id: $id, speed: $speed, on: $on)}}\"}}";
                    return SendReq(mutation);
                } },
                { "set-ftl-engines-speed", (Command cmd)=>{
                    var payload=json.Deserialize<SetSpeedPayload>(cmd.Payload);
                    string warpId = ids.GetWarpId().Result;
                    string mutation = $"{{\"operationName\":\"setSpeed\",\"variables\":{{\"on\":true,\"id\":\"{warpId}\",\"speed\":{payload.Speed}}},\"query\":\"mutation setSpeed($id: ID!, $speed: Int!, $on: Boolean) {{  setSpeed(id: $id, speed: $speed, on: $on)}}\"}}";
                    return SendReq(mutation);
                } }
            };
        }
        public async void TranslateCommand(Command command)
        {
            // Console.WriteLine($"Translating command: {command.Type} with payload: {command.Payload}");
            if (commandHandlers.ContainsKey(command.Type))
            {
                await commandHandlers[command.Type](command);
            }
        }

        private async Task<string> SendReq(string query)
        {
            //Console.WriteLine($"Sending request to Thorium: {query}");

            StringContent req = new StringContent(
                query,
                System.Text.Encoding.UTF8,
                "application/json"
            );

            using HttpResponseMessage response = await httpClient.PostAsync("/graphql", req);

           // Console.WriteLine($"Response: {response.StatusCode}");

            return await response.Content.ReadAsStringAsync();
        }
    }
}
