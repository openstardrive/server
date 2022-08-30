namespace OpenStardriveServer.Domain.Systems.Comms.LongRange;

public record SetCypherPayload
{
    public string CypherId { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public Substitution[] EncodeSubstitutions { get; init; }
    public Substitution[] DecodeSubstitutions { get; init; }
}