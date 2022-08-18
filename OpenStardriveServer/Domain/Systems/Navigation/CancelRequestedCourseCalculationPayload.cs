namespace OpenStardriveServer.Domain.Systems.Navigation;

public record CancelRequestedCourseCalculationPayload
{
    public string CourseId { get; init; }
}