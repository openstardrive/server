namespace OpenStardriveServer.Domain.Systems.Defense.EnergyBeam;

public record FireEnergyBeamPayload
{
    public string BankName { get; init; }
    public double DischargePercent { get; init; }
    public string Target { get; init; } = "";
}