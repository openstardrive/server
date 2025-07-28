using System.Collections.Generic;

namespace OpenStardriveServer.Domain.Integrations
{
    public record GraphQlResult<T>
    {
        public T data { get; init; }
    }
    public record GetFlightsResults
    {
        public List<Flight> flights { get; init; }
    }

    public record EngineResult
    {
        public IdResult[] engines { get; init; }
    }

    public record Flight
    {
        public string id { get; init; }
        public IdResult[] simulators { get; init; }
    }

    public record IdResult
    {
        public string id { get; init; }
    }
}
