namespace OpenStardriveServer.Domain.Systems.Defense.EnergyBeam;

public record ConfigureEnergyBeamBankPayload
{
    public string BankName { get; init; }
    public double Frequency { get; init; }
    public double ArcDegrees { get; init; }
}