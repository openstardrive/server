namespace OpenStardriveServer.Domain.Systems.Defense.Shields;

public record ShieldStrengthPayload
{
    public ShieldSectionStrengths SectionStrengths { get; init; }
}