namespace OpenStardriveServer.Domain.Systems.Sensors;

public record NewScanPayload
{
    public string ScanId { get; init; }
    public string ScanFor { get; init; }
}