namespace OpenStardriveServer.Domain.Systems.Sensors;

public record PassiveScanPayload
{
    public string ScanFor { get; init; }
    public string Result { get; init; }
}