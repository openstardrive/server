using System;

namespace OpenStardriveServer.Domain.Systems.Sensors;

public record RemoveSensorContactPayload
{
    public Guid ContactId { get; init; } = Guid.NewGuid();
}