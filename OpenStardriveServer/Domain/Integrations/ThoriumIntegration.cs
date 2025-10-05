using OpenStardriveServer.Domain.Systems.Propulsion.Engines;
using OpenStardriveServer.Domain.Systems.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenStardriveServer.Domain.Integrations
{
    public interface IThoriumIntegration
    {
        public void TranslateCommand(Command command);
    }
    public class ThoriumIntegration : IThoriumIntegration
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
                if (simulatorId == null)
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
            ids = new ThoriumIds(this);
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
                } },
                { "teams-update", (Command cmd) => {
                    try
                    {
                        Console.WriteLine($"Teams-update payload received: {cmd.Payload}");

                        ThoriumTeam[] thoriumTeams = null;
                        
                        // Try parsing as wrapped format first: {"teams": [...]}
                        try
                        {
                            var wrappedPayload = json.Deserialize<ThoriumTeamsPayload>(cmd.Payload);
                            thoriumTeams = wrappedPayload.Teams;
                            Console.WriteLine("Successfully parsed as ThoriumTeamsPayload (wrapped format)");
                        }
                        catch (Exception ex1)
                        {
                            Console.WriteLine($"Failed to parse as wrapped format: {ex1.Message}");
                            
                            // Try parsing as direct array: [{...}, {...}]
                            try
                            {
                                thoriumTeams = json.Deserialize<ThoriumTeam[]>(cmd.Payload);
                                Console.WriteLine("Successfully parsed as ThoriumTeam array (direct format)");
                            }
                            catch (Exception ex2)
                            {
                                Console.WriteLine($"Failed to parse as direct array: {ex2.Message}");
                                throw new InvalidOperationException($"Unable to parse teams payload. Wrapped format error: {ex1.Message}; Direct array error: {ex2.Message}");
                            }
                        }

                        if (thoriumTeams == null || thoriumTeams.Length == 0)
                        {
                            Console.WriteLine("No teams found in payload");
                            return Task.FromResult("");
                        }
                        
                        // Convert Thorium teams format to our internal Teams format
                        var convertedTeams = ConvertThoriumTeamsToInternalFormat(thoriumTeams);
                        Console.WriteLine($"Converted {convertedTeams.Length} teams to internal format");
                        
                        // Create a new command with the converted payload for our Teams system
                        var internalTeamsPayload = json.Serialize(convertedTeams);
                        
                        // Update the original command's payload to the converted format
                        cmd.Payload = internalTeamsPayload;
                        Console.WriteLine($"Updated command payload: {cmd.Payload}");

                        return Task.FromResult("");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in teams-update handler: {ex}");
                        throw;
                    }
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

        private Team[] ConvertThoriumTeamsToInternalFormat(ThoriumTeam[] thoriumTeams)
        {
            return thoriumTeams.Select(thoriumTeam => new Team
            {
                Id = thoriumTeam.Id,
                Name = thoriumTeam.Name,
                Type = thoriumTeam.Type,
                SimulatorId = thoriumTeam.SimulatorId,
                Priority = thoriumTeam.Priority,
                Location = ConvertThoriumLocation(thoriumTeam.Location),
                Orders = thoriumTeam.Orders,
                Officers = ConvertThoriumOfficers(thoriumTeam.Officers)
            }).ToArray();
        }

        private Officer[] ConvertThoriumOfficers(JsonElement officersElement)
        {
            if (officersElement.ValueKind == JsonValueKind.Array)
            {
                var officers = new List<Officer>();

                foreach (var element in officersElement.EnumerateArray())
                {
                    if (element.ValueKind == JsonValueKind.String)
                    {
                        // Format 1: Array of string IDs (from real Thorium)
                        officers.Add(new Officer
                        {
                            Id = element.GetString() ?? "",
                            Name = "Unknown", // ID only, name would need lookup
                            Position = "Officer", // Default position
                            Inventory = new InventoryItem[0]
                        });
                    }
                    else if (element.ValueKind == JsonValueKind.Object)
                    {
                        // Format 2: Array of officer objects (from some Thorium variants)
                        var officerId = element.TryGetProperty("id", out var idProp) ? idProp.GetString() : "";
                        var officerName = element.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : "Unknown";
                        var officerPosition = element.TryGetProperty("position", out var posProp) ? posProp.GetString() : "Officer";

                        officers.Add(new Officer
                        {
                            Id = officerId ?? "",
                            Name = officerName ?? "Unknown",
                            Position = officerPosition ?? "Officer",
                            Inventory = new InventoryItem[0] // Handle inventory later if needed
                        });
                    }
                }

                return officers.ToArray();
            }

            return new Officer[0];
        }

        private TeamLocation ConvertThoriumLocation(JsonElement locationElement)
        {
            if (locationElement.ValueKind == JsonValueKind.String)
            {
                // Format 1: String ID (from Thorium)
                var locationId = locationElement.GetString();
                return string.IsNullOrEmpty(locationId) ? null : new TeamLocation
                {
                    Id = locationId,
                    Name = null, // ID only, name would need lookup
                    Deck = null  // Would need additional lookup
                };
            }
            else if (locationElement.ValueKind == JsonValueKind.Object)
            {
                // Format 2: Location object (from dev-client)
                var locationId = locationElement.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                var locationName = locationElement.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : null;

                Deck deck = null;
                if (locationElement.TryGetProperty("deck", out var deckProp))
                {
                    if (deckProp.ValueKind == JsonValueKind.String)
                    {
                        // Deck as string (e.g., "Deck 1")
                        var deckString = deckProp.GetString();
                        if (!string.IsNullOrEmpty(deckString))
                        {
                            deck = new Deck
                            {
                                Id = deckString.ToLowerInvariant().Replace(" ", "-"),
                                Number = ParseDeckNumber(deckString),
                                Name = deckString
                            };
                        }
                    }
                    else if (deckProp.ValueKind == JsonValueKind.Object)
                    {
                        // Deck as object (e.g., {"id": "deck-1", "name": "Deck 1", "number": 1})
                        var deckId = deckProp.TryGetProperty("id", out var deckIdProp) ? deckIdProp.GetString() : null;
                        var deckName = deckProp.TryGetProperty("name", out var deckNameProp) ? deckNameProp.GetString() : null;
                        var deckNumber = deckProp.TryGetProperty("number", out var deckNumberProp) && deckNumberProp.TryGetInt32(out var num) ? num : 1;

                        if (!string.IsNullOrEmpty(deckId))
                        {
                            deck = new Deck
                            {
                                Id = deckId,
                                Number = deckNumber,
                                Name = deckName ?? $"Deck {deckNumber}"
                            };
                        }
                    }
                }

                return string.IsNullOrEmpty(locationId) ? null : new TeamLocation
                {
                    Id = locationId,
                    Name = locationName,
                    Deck = deck
                };
            }
            else if (locationElement.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return null;
        }

        private int ParseDeckNumber(string deckName)
        {
            if (string.IsNullOrEmpty(deckName))
                return 0;

            // Try to extract number from strings like "Deck 1", "Bridge", etc.
            var words = deckName.Split(' ');
            foreach (var word in words)
            {
                if (int.TryParse(word, out var number))
                    return number;
            }

            // Default deck number if no number found
            return 1;
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
