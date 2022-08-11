using System;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Sensors;

public record SensorsState : StandardSystemBaseState
{
    public SensorScan[] ActiveScans { get; init; } = Array.Empty<SensorScan>();
    public SensorScan LastUpdatedScan { get; init; }
    
    public SensorContact[] Contacts { get; init; } = Array.Empty<SensorContact>();
}

public record SensorScan
{
    public Guid ScanId { get; init; } = Guid.NewGuid();
    public string State { get; init; } = "";
    public string ScanFor { get; init; } = "";
    public string Result { get; init; } = "";
}

public static class SensorScanState
{
    public static string Active = "active";
    public static string Completed = "completed";
    public static string Canceled = "canceled";
}

public record SensorContact
{
    public Guid ContactId { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = "";
    public string Icon { get; init; } = "";
    public Point Position { get; init; }
    public Destination[] Destinations { get; init; } = Array.Empty<Destination>();
}

public record Point
{
    public double X { get; init; }
    public double Y { get; init; }
    public double Z { get; init; }
}

public record Destination
{
    public Point Position { get; init; }
    public int RemainingMilliseconds { get; init; }
}
