namespace OpenStardriveServer.Domain.Chronometer;

public record ChronometerPayload
{
    public long ElapsedMilliseconds { get; init; }
}