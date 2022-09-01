namespace OpenStardriveServer.Domain.Systems.Debug;

public record DebugState
{
    public DebugEntry LastEntry { get; init; }
}

public record DebugEntry
{
    public string DebugId { get; init; }
    public string Description { get; init; }
}