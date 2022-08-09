namespace OpenStardriveServer.Domain.Systems.Defense.EnergyBeam;

public record ChargeEnergyBeamPayload
{
    public string BankName { get; init; }
    public double NewCharge { get; init; }
}