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
                        // Parse enhanced Thorium teams format: {"teams": [...]}
                        var wrappedPayload = json.Deserialize<ThoriumTeamsPayload>(cmd.Payload);
                        var thoriumTeams = wrappedPayload.Teams;

                        if (thoriumTeams.Length == 0)
                        {
                            return Task.FromResult("");
                        }
                        
                        // Convert enhanced Thorium teams format to our internal Teams format
                        var convertedTeams = ConvertThoriumTeamsToInternalFormat(thoriumTeams);
                        
                        // Create a new command with the converted payload for our Teams system
                        var internalTeamsPayload = json.Serialize(convertedTeams);
                        
                        // Update the original command's payload to the converted format
                        cmd.Payload = internalTeamsPayload;

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
                Location = ConvertThoriumLocation(thoriumTeam.LocationName, thoriumTeam.DeckName),
                Orders = thoriumTeam.Orders,
                Officers = ConvertThoriumOfficers(thoriumTeam.Officers)
            }).ToArray();
        }

        private Officer[] ConvertThoriumOfficers(JsonElement officersElement)
        {
            if (officersElement.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidOperationException("Expected officers to be an array of enhanced officer objects");
            }

            var officers = new List<Officer>();

            foreach (var element in officersElement.EnumerateArray())
            {
                if (element.ValueKind != JsonValueKind.Object)
                {
                    throw new InvalidOperationException("Expected each officer to be an enhanced officer object with fullName, rank, position, etc.");
                }

                // Parse enhanced officer objects (from getEnhancedTeamData)
                var officerId = element.TryGetProperty("id", out var idProp) ? idProp.GetString() : "";

                // Required enhanced fields
                var fullName = element.TryGetProperty("fullName", out var fullNameProp) ? fullNameProp.GetString() : null;
                var firstName = element.TryGetProperty("firstName", out var firstNameProp) ? firstNameProp.GetString() : null;
                var lastName = element.TryGetProperty("lastName", out var lastNameProp) ? lastNameProp.GetString() : null;
                var rank = element.TryGetProperty("rank", out var rankProp) ? rankProp.GetString() : null;
                var position = element.TryGetProperty("position", out var posProp) ? posProp.GetString() : null;
                var shift = element.TryGetProperty("shift", out var shiftProp) ? shiftProp.GetString() : null;

                // Validate required fields
                if (string.IsNullOrEmpty(officerId))
                {
                    throw new InvalidOperationException("Officer ID is required");
                }

                if (string.IsNullOrEmpty(fullName) && (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName)))
                {
                    throw new InvalidOperationException("Officer must have either fullName or firstName+lastName");
                }

                // Build officer name
                var officerName = fullName ?? $"{firstName} {lastName}";

                // Build enhanced name with rank if available
                var displayName = officerName;
                if (!string.IsNullOrEmpty(rank) && rank != "Unknown")
                {
                    displayName = $"{rank} {officerName}";
                }

                officers.Add(new Officer
                {
                    Id = officerId,
                    Name = displayName,
                    Position = position ?? "Officer",
                    Inventory = new InventoryItem[0]
                });
            }

            return officers.ToArray();
        }

        private TeamLocation ConvertThoriumLocation(string locationName = null, string deckName = null)
        {
            // If no location information provided, return null
            if (string.IsNullOrEmpty(locationName) && string.IsNullOrEmpty(deckName))
            {
                return null;
            }

            // Build the display name from locationName and deckName
            string displayName = null;
            if (!string.IsNullOrEmpty(locationName) && !string.IsNullOrEmpty(deckName))
            {
                displayName = $"{locationName}, {deckName}";
            }
            else if (!string.IsNullOrEmpty(locationName))
            {
                displayName = locationName;
            }
            else if (!string.IsNullOrEmpty(deckName))
            {
                displayName = deckName;
            }

            return new TeamLocation
            {
                Id = null, // No location ID in the new format
                Name = displayName,
                Deck = !string.IsNullOrEmpty(deckName) ? CreateDeckFromName(deckName) : null
            };
        }

        private Deck CreateDeckFromName(string deckName)
        {
            // Parse deck names like "Deck 7" or "Deck 1" to extract the number
            if (string.IsNullOrEmpty(deckName)) return null;

            // Try to extract deck number from names like "Deck 7"
            var parts = deckName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2 && parts[0].Equals("Deck", StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(parts[1], out int deckNumber))
                {
                    return new Deck
                    {
                        Id = $"deck-{deckNumber}", // Generate a reasonable ID
                        Number = deckNumber,
                        Name = deckName
                    };
                }
            }

            // Fallback: create a deck with the full name but unknown number
            return new Deck
            {
                Id = $"deck-{deckName.ToLowerInvariant().Replace(" ", "-")}",
                Number = 0, // Unknown deck number
                Name = deckName
            };
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
