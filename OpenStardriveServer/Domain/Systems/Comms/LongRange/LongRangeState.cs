using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Comms.LongRange;

public record LongRangeState : StandardSystemBaseState
{
    [JsonIgnore]
    public Cypher[] Cyphers { get; init; } = Array.Empty<Cypher>();
    public Cypher LastUpdatedCypher { get; init; }

    [JsonIgnore]
    public Dictionary<string, DateTimeOffset> LongRangeMessagesSentAt { get; init; } = new();
    public LongRangeMessage LastUpdatedLongRangeMessage { get; init; }
}

public record Cypher
{
    public string CypherId { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public Substitution[] EncodeSubstitutions { get; init; }
    public Substitution[] DecodeSubstitutions { get; init; }
    public double PercentDecoded { get; init; }
}

public record Substitution
{
    public string Change { get; set; }
    public string To { get; set; }
}

public record LongRangeMessage
{
    public string MessageId { get; init; }
    public string Sender { get; init; }
    public string Recipient { get; init; }
    public string Message { get; init; }
    public string CypherId { get; init; }
    public bool Outbound { get; init; }
    public DateTimeOffset SentAt { get; init; }
    public DateTimeOffset LastUpdatedAt { get; init; }
}