namespace OpenStardriveServer.Domain.Systems.Navigation;

public record SetEtaPayload
{
    public string EngineSystem { get; init; }
    public int Speed { get; init; }
    public int ArriveInMilliseconds { get; init; }
}