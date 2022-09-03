using System;

namespace OpenStardriveServer.Domain.Systems.Clients;

public record DisableClientPayload
{
    public Guid ClientId { get; init; }
    public bool Disabled { get; init; }
    public string DisabledMessage { get; init; }
}