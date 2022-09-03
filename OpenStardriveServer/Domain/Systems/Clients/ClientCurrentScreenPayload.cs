using System;

namespace OpenStardriveServer.Domain.Systems.Clients;

public record ClientCurrentScreenPayload
{
    public Guid ClientId { get; init; }
    public string CurrentScreen { get; init; }
}