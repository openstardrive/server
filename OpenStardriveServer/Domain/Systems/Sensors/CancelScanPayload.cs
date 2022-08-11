using System;

namespace OpenStardriveServer.Domain.Systems.Sensors;

public record CancelScanPayload
{
    public Guid ScanId { get; init; }
}