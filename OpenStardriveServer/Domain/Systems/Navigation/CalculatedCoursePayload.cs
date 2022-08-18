namespace OpenStardriveServer.Domain.Systems.Navigation;

public record CalculatedCoursePayload
{
    public string CourseId { get; init; }
    public string Destination { get; init; }
    public Coordinates Coordinates { get; init; }
    public SetEtaPayload Eta { get; init; }
}