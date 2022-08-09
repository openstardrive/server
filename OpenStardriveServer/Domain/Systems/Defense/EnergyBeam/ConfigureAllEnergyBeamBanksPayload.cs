namespace OpenStardriveServer.Domain.Systems.Defense.EnergyBeam;

public record ConfigureAllEnergyBeamBanksPayload
{
    public EnergyBeamBank[] Banks;
}