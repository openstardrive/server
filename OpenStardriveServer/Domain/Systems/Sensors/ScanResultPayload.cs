namespace OpenStardriveServer.Domain.Systems.Sensors;

public record ScanResultPayload
{
    public string ScanId { get; init; }
    public string Result { get; init; }
}