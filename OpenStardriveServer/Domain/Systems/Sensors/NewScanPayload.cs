namespace OpenStardriveServer.Domain.Systems.Sensors;

public record NewScanPayload
{
    public string ScanFor { get; init; }
}