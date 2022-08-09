using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Defense.EnergyBeam;

public class EnergyBeamSystem : SystemBase<EnergyBeamState>
{
    public EnergyBeamSystem(IEnergyBeamTransforms transforms, IJson json) : base(json)
    {
        SystemName = "energy-beams";
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["report-state"] = c => Update(c, TransformResult<EnergyBeamState>.StateChanged(state)),
            ["set-disabled"] = c => Update(c, transforms.SetDisabled(state, SystemName, Payload<DisabledSystemsPayload>(c))),
            ["set-damaged"] = c => Update(c, transforms.SetDamaged(state, SystemName, Payload<DamagedSystemsPayload>(c))),
            ["set-power"] = c => Update(c, transforms.SetCurrentPower(state, SystemName, Payload<CurrentPowerPayload>(c))),
            ["set-required-power"] = c => Update(c, transforms.SetRequiredPower(state, SystemName, Payload<RequiredPowerPayload>(c))),
            ["charge-energy-beam"] = c => Update(c, transforms.SetBankCharge(state, Payload<ChargeEnergyBeamPayload>(c))),
            ["fire-energy-beam"] = c => Update(c, transforms.Fire(state, Payload<FireEnergyBeamPayload>(c))),
            ["configure-energy-beam"] = c => Update(c, transforms.ConfigureBank(state, Payload<ConfigureEnergyBeamBankPayload>(c))),
            ["configure-all-energy-beams"] = c => Update(c, transforms.ConfigureAllBanks(state, Payload<ConfigureAllEnergyBeamBanksPayload>(c)))
        };
    }
}