namespace OpenStardriveServer.Domain.Systems.Navigation;

public record RequestedCourseCalculationPayload
{
    public string CourseId { get; init; }
    public string Destination { get; init; }
}