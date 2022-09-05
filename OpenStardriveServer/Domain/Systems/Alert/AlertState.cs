namespace OpenStardriveServer.Domain.Systems.Alert;

public record AlertState
{
    public AlertLevel Current { get; init; } = AlertLevel.Green;

    public AlertLevel[] AllLevels { get; init; } = {
        AlertLevel.Red,
        AlertLevel.Yellow,
        AlertLevel.Green
    };
}

public record AlertLevel
{
    public int Level { get; init; }
    public string Name { get; init; }
    public string Color { get; init; }

    public static AlertLevel Red = new() { Level = 1, Name = "Red", Color = "#ff0000" };
    public static AlertLevel Yellow = new() { Level = 2, Name = "Yellow", Color = "#ffff00" };
    public static AlertLevel Green = new() { Level = 3, Name = "Green", Color = "#00ff00" };
}