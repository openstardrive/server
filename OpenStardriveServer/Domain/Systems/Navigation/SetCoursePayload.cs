namespace OpenStardriveServer.Domain.Systems.Navigation;

public record SetCoursePayload
{
    public string CourseId { get; init; }
    public string Destination { get; init; }
    public Coordinates Coordinates { get; init; }
    public SetEtaPayload Eta { get; init; }
}