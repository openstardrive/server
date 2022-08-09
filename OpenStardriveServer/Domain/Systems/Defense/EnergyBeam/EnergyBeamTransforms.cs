using System;
using System.Linq;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Defense.EnergyBeam;

public interface IEnergyBeamTransforms : IStandardTransforms<EnergyBeamState>
{
    TransformResult<EnergyBeamState> SetBankCharge(EnergyBeamState state, ChargeEnergyBeamPayload payload);
    TransformResult<EnergyBeamState> Fire(EnergyBeamState state, FireEnergyBeamPayload payload);
    TransformResult<EnergyBeamState> ConfigureBank(EnergyBeamState state, ConfigureEnergyBeamBankPayload payload);
    TransformResult<EnergyBeamState> ConfigureAllBanks(EnergyBeamState state, ConfigureAllEnergyBeamBanksPayload payload);
}

public class EnergyBeamTransforms : IEnergyBeamTransforms
{
    private readonly IStandardTransforms<EnergyBeamState> standardTransforms;

    public EnergyBeamTransforms(IStandardTransforms<EnergyBeamState> standardTransforms)
    {
        this.standardTransforms = standardTransforms;
    }
    
    public TransformResult<EnergyBeamState> SetDisabled(EnergyBeamState state, string systemName, DisabledSystemsPayload payload)
    {
        return standardTransforms.SetDisabled(state, systemName, payload);
    }

    public TransformResult<EnergyBeamState> SetDamaged(EnergyBeamState state, string systemName, DamagedSystemsPayload payload)
    {
        return payload.ValueOrNone(systemName).Case(
            some: damaged => TransformResult<EnergyBeamState>.StateChanged(state with
            {
                Damaged = damaged,
                Banks = damaged ? Uncharged(state.Banks) : state.Banks
            }), 
            none: TransformResult<EnergyBeamState>.NoChange
        );
    }

    private EnergyBeamBank[] Uncharged(EnergyBeamBank[] banks)
    {
        return banks.Select(x => x with { PercentCharged = 0 }).ToArray();
    }

    public TransformResult<EnergyBeamState> SetCurrentPower(EnergyBeamState state, string systemName, CurrentPowerPayload payload)
    {
        return payload.ValueOrNone(systemName).Case(
            some: newPower => TransformResult<EnergyBeamState>.StateChanged(state with
            {
                CurrentPower = newPower,
                Banks = newPower < state.RequiredPower ? Uncharged(state.Banks) : state.Banks
            }),
            none: TransformResult<EnergyBeamState>.NoChange);
    }

    public TransformResult<EnergyBeamState> SetRequiredPower(EnergyBeamState state, string systemName, RequiredPowerPayload payload)
    {
        return payload.ValueOrNone(systemName).Case(
            some: newRequiredPower => TransformResult<EnergyBeamState>.StateChanged(state with
            {
                RequiredPower = newRequiredPower,
                Banks = state.CurrentPower < newRequiredPower ? Uncharged(state.Banks) : state.Banks
            }),
            none: TransformResult<EnergyBeamState>.NoChange);
    }

    public TransformResult<EnergyBeamState> SetBankCharge(EnergyBeamState state, ChargeEnergyBeamPayload payload)
    {
        return state.IfFunctional(() =>
        {
            if (state.Banks.None(x => x.Name == payload.BankName))
            {
                return TransformResult<EnergyBeamState>.Error($"Unknown bank: {payload.BankName}");
            }
            
            return TransformResult<EnergyBeamState>.StateChanged(state with
            {
                Banks = state.Banks
                    .Select(x => UpdateChargeOnMatch(x, payload.BankName, payload.NewCharge))
                    .ToArray()
            });
        });
    }

    private EnergyBeamBank UpdateChargeOnMatch(EnergyBeamBank bank, string name, double charge)
    {
        if (bank.Name == name)
        {
            return bank with
            {
                PercentCharged = Math.Max(0, charge)
            };
        }

        return bank;
    }

    public TransformResult<EnergyBeamState> Fire(EnergyBeamState state, FireEnergyBeamPayload payload)
    {
        return state.IfFunctional(() =>
        {
            return state.Banks.FirstOrNone(x => x.Name == payload.BankName).Case(
                some: bank =>
                {
                    if (bank.PercentCharged <= 0)
                    {
                        return TransformResult<EnergyBeamState>.Error($"bank {payload.BankName} has no charge to fire");
                    }
                    
                    var dischargePercent = Math.Min(payload.DischargePercent, bank.PercentCharged);
                    return TransformResult<EnergyBeamState>.StateChanged(state with
                    {
                        Banks = state.Banks
                            .Select(x => UpdateChargeOnMatch(x, payload.BankName, x.PercentCharged - dischargePercent))
                            .ToArray(),
                        LastFiredEnergyBeam = new FiredEnergyBeam
                        {
                            Name = payload.BankName,
                            PercentDischarged = dischargePercent,
                            FiredAt = DateTimeOffset.UtcNow,
                            ArcDegrees = bank.ArcDegrees,
                            Frequency = bank.Frequency,
                            Target = payload.Target
                        }
                    });
                },
                none: () => TransformResult<EnergyBeamState>.Error($"Unknown bank: {payload.BankName}"));
        });
    }

    public TransformResult<EnergyBeamState> ConfigureBank(EnergyBeamState state, ConfigureEnergyBeamBankPayload payload)
    {
        if (state.Banks.None(x => x.Name == payload.BankName))
        {
            return TransformResult<EnergyBeamState>.Error($"Unknown bank: {payload.BankName}");
        }
        
        return TransformResult<EnergyBeamState>.StateChanged(state with
        {
            Banks = state.Banks
                .Select(x => x.Name == payload.BankName
                    ? x with { Frequency = payload.Frequency, ArcDegrees = payload.ArcDegrees }
                    : x)
                .ToArray()
        });
    }

    public TransformResult<EnergyBeamState> ConfigureAllBanks(EnergyBeamState state, ConfigureAllEnergyBeamBanksPayload payload)
    {
        return TransformResult<EnergyBeamState>.StateChanged(state with { Banks = payload.Banks });
    }
}