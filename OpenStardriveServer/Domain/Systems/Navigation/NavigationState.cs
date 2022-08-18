using System;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Navigation;

public record NavigationState : StandardSystemBaseState
{
    public RequestedCourseCalculation[] RequestedCourseCalculations { get; init; } = Array.Empty<RequestedCourseCalculation>();
    public CalculatedCourse[] CalculatedCourses { get; init; } = Array.Empty<CalculatedCourse>();
    public CurrentCourse CurrentCourse { get; init; }
}

public record RequestedCourseCalculation
{
    public string CourseId { get; init; }
    public string Destination { get; init; }
    public DateTimeOffset RequestedAt { get; init; } = DateTimeOffset.UtcNow;
}

public record CalculatedCourse
{
    public string CourseId { get; init; }
    public string Destination { get; init; }
    public Coordinates Coordinates { get; init; }
    public Eta Eta { get; init; }
    public DateTimeOffset CalculatedAt { get; init; }
}

public record CurrentCourse
{
    public string CourseId { get; init; }
    public string Destination { get; init; }
    public Coordinates Coordinates { get; init; }
    public Eta Eta { get; init; }
    public DateTimeOffset CourseSetAt { get; init; }
}

public record Coordinates
{
    public double X { get; init; }
    public double Y { get; init; }
    public double Z { get; init; }
}

public record Eta
{
    public string EngineSystem { get; init; }
    public TravelTime[] TravelTimes { get; init; }
}

public record TravelTime
{
    public int Speed { get; init; }
    public int ArriveInMilliseconds { get; init; }
}
