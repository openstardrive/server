using System;

namespace OpenStardriveServer.Domain.Systems.Sensors;

public record ScanResultPayload
{
    public Guid ScanId { get; init; }
    public string Result { get; init; }
}