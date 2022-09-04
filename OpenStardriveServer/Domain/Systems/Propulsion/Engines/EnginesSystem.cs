using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Engines;

public interface IEnginesSystem : ISystem
{
    int MaxSpeed { get; }
    int CurrentSpeed { get; }
}

public abstract class EnginesSystem : SystemBase<EnginesState>, IEnginesSystem, IPoweredSystem
{
    protected EnginesSystem(string systemName, EnginesState initialState, IEnginesTransforms transforms, IJson json) : base (json)
    {
        SystemName = systemName;
        state = initialState;
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["report-state"] = c => Update(c, TransformResult<EnginesState>.StateChanged(state)),
            [$"set-{SystemName}-speed"] = c => Update(c, transforms.SetSpeed(state, Payload<SetSpeedPayload>(c))),
            [$"configure-{SystemName}"] = c => Update(c, transforms.Configure(state, Payload<EnginesConfigurationPayload>(c))),
            [ChronometerCommand.Type] = c => Update(c, transforms.UpdateHeat(state, Payload<ChronometerPayload>(c))),
            ["set-power"] = c => Update(c, transforms.SetCurrentPower(state, systemName, Payload<CurrentPowerPayload>(c))),
            ["set-required-power"] = c => Update(c, transforms.SetRequiredPower(state, systemName, Payload<RequiredPowerPayload>(c))),
            ["set-damaged"] = c => Update(c, transforms.SetDamaged(state, systemName, Payload<DamagedSystemsPayload>(c))),
            ["set-disabled"] = c => Update(c, transforms.SetDisabled(state, SystemName, Payload<DisabledSystemsPayload>(c)))
        };
    }

    public int MaxSpeed => state.SpeedConfig.MaxSpeed;
    public int CurrentSpeed => state.CurrentSpeed;
    public int CurrentPower => state.CurrentPower;
}

public class FtlEnginesSystem : EnginesSystem
{
    public FtlEnginesSystem(IEnginesTransforms transforms, IJson json)
        : base("ftl-engines", EnginesStateDefaults.Ftl, transforms, json)
    { }
}

public class SublightEnginesSystem : EnginesSystem
{
    public SublightEnginesSystem(IEnginesTransforms transforms, IJson json)
        : base("sublight-engines", EnginesStateDefaults.Sublight, transforms, json)
    { }
}