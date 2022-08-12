namespace OpenStardriveServer.Domain.Systems.Sensors;

public record CancelScanPayload
{
    public string ScanId { get; init; }
}