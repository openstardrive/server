namespace OpenStardriveServer.Domain.Systems.Comms.LongRange;

public record UpdateCypherSubstitutionsPayload
{
    public string CypherId { get; init; }
    public Substitution[] DecodeSubstitutions { get; init; }
}