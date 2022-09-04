using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Sensors;

public class SensorsSystem : SystemBase<SensorsState>, IPoweredSystem
{
    public SensorsSystem(IJson json, ISensorsTransforms transforms) : base(json)
    {
        SystemName = "sensors";
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["report-state"] = c => Update(c, TransformResult<SensorsState>.StateChanged(state)),
            ["set-disabled"] = c => Update(c, transforms.SetDisabled(state, SystemName, Payload<DisabledSystemsPayload>(c))),
            ["set-damaged"] = c => Update(c, transforms.SetDamaged(state, SystemName, Payload<DamagedSystemsPayload>(c))),
            ["set-power"] = c => Update(c, transforms.SetCurrentPower(state, SystemName, Payload<CurrentPowerPayload>(c))),
            ["set-required-power"] = c => Update(c, transforms.SetRequiredPower(state, SystemName, Payload<RequiredPowerPayload>(c))),
            ["new-sensor-scan"] = c => Update(c, transforms.NewScan(state, Payload<NewScanPayload>(c))),
            ["set-sensor-scan-result"] = c => Update(c, transforms.SetScanResult(state, Payload<ScanResultPayload>(c))),
            ["cancel-sensor-scan"] = c => Update(c, transforms.CancelScan(state, Payload<CancelScanPayload>(c))),
            ["passive-sensor-scan"] = c => Update(c, transforms.PassiveScan(state, Payload<PassiveScanPayload>(c))),
            ["new-sensor-contact"] = c => Update(c, transforms.NewContact(state, Payload<NewSensorContactPayload>(c))),
            ["remove-sensor-contact"] = c => Update(c, transforms.RemoveContact(state, Payload<RemoveSensorContactPayload>(c))),
            ["update-sensor-contact"] = c => Update(c, transforms.UpdateContact(state, Payload<UpdateSensorContactPayload>(c))),
            [ChronometerCommand.Type] = c => Update(c, transforms.MoveContacts(state, Payload<ChronometerPayload>(c)))
        };
    }

    public int CurrentPower => state.CurrentPower;
}