using System;

namespace OpenStardriveServer.Domain.Systems.Sensors;

public record NewSensorContactPayload
{
    public string ContactId { get; init; }
    public string Name { get; init; } = "";
    public string Icon { get; init; } = "";
    public Point Position { get; init; }
    public Destination[] Destinations { get; init; } = Array.Empty<Destination>();
}