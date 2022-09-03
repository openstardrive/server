using System;

namespace OpenStardriveServer.Domain.Systems.Clients;

public record ClientOperatorPayload
{
    public Guid ClientId { get; init; }
    public string Operator { get; init; }
}