using System;
using System.Collections.Generic;
using System.Linq;
using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Engines;

public interface IEnginesTransformations
{
    TransformResult<EnginesState> SetSpeed(EnginesState state, SetSpeedPayload payload);
    TransformResult<EnginesState> SetDamage(EnginesState state, SystemDamagePayload payload);
    TransformResult<EnginesState> SetDisabled(EnginesState state, SystemDisabledPayload payload);
    TransformResult<EnginesState> SetCurrentPower(EnginesState state, SystemPowerPayload payload);
    TransformResult<EnginesState> UpdateHeat(EnginesState state, ChronometerPayload payload);
    TransformResult<EnginesState> Configure(EnginesState state, EnginesConfigurationPayload payload);
}

public class EnginesTransformations : IEnginesTransformations
{
    public TransformResult<EnginesState> SetSpeed(EnginesState state, SetSpeedPayload payload)
    {
        if (payload.Speed < 0 || payload.Speed > state.SpeedConfig.MaxSpeed)
        {
            return TransformResult<EnginesState>.Error("invalid speed");
        }
        return state.IfFunctional(() =>
        {
            var powerRequired = CalculatePowerRequirements(state).Find(x => x.speed == payload.Speed).powerRequired;
            if (powerRequired > state.CurrentPower)
            {
                return TransformResult<EnginesState>.Error(StandardSystemBaseState.InsufficientPowerError);
            }

            return TransformResult<EnginesState>.StateChanged(state with { CurrentSpeed = payload.Speed });
        });
    }

    private List<(int speed, int powerRequired)> CalculatePowerRequirements(EnginesState state)
    {
        var result = new List<(int speed, int powerRequired)> { (0, 0) };
        var lastRequirement = state.RequiredPower;
        for (var speed = 1; speed <= state.SpeedConfig.MaxSpeed; speed++)
        {
            var defaultRequirement = new SpeedPowerRequirement { PowerNeeded = lastRequirement };
            var required = state.SpeedPowerRequirements.FirstOrDefault(x => x.Speed == speed, defaultRequirement).PowerNeeded;
            lastRequirement = required;
            result.Add((speed, lastRequirement));
        }
        return result;
    }

    public TransformResult<EnginesState> SetDamage(EnginesState state, SystemDamagePayload payload)
    {
        var newSpeed = payload.Damaged ? 0 : state.CurrentSpeed;
        return TransformResult<EnginesState>.StateChanged(state with { Damaged = payload.Damaged, CurrentSpeed = newSpeed });
    }
        
    public TransformResult<EnginesState> SetDisabled(EnginesState state, SystemDisabledPayload payload)
    {
        var newSpeed = payload.Disabled ? 0 : state.CurrentSpeed;
        return TransformResult<EnginesState>.StateChanged(state with { Disabled = payload.Disabled, CurrentSpeed = newSpeed });
    }
        
    public TransformResult<EnginesState> SetCurrentPower(EnginesState state, SystemPowerPayload payload)
    {
        var newSpeed = state.CurrentSpeed;
        if (state.CurrentSpeed > 0)
        {
            newSpeed = CalculatePowerRequirements(state)
                .Where(x => x.speed <= state.CurrentSpeed && x.powerRequired <= payload.CurrentPower)
                .LastOrDefault((speed: 0, powerRequired: 0)).speed;
        }
        return TransformResult<EnginesState>.StateChanged(state with
        {
            CurrentPower = payload.CurrentPower,
            CurrentSpeed = newSpeed
        });
    }

    public TransformResult<EnginesState> UpdateHeat(EnginesState state, ChronometerPayload payload)
    {
        var newHeat = CalculateNewHeat(state, payload);
        if (newHeat != state.CurrentHeat)
        {
            return TransformResult<EnginesState>.StateChanged(state with { CurrentHeat = newHeat });
        }
            
        return TransformResult<EnginesState>.NoChange();
    }

    private int CalculateNewHeat(EnginesState state, ChronometerPayload payload)
    {
        var targetHeat = GetTargetHeat(state);
        if (state.CurrentHeat == targetHeat)
        {
            return state.CurrentHeat;
        }

        var isHeating = targetHeat > state.CurrentHeat;
        var rate = isHeating
            ? GetHeatingRateInMilliseconds(state)
            : GetCoolingRateInMilliseconds(state);
            
        var heatChange = (int) (rate * payload.ElapsedMilliseconds);
        return isHeating
            ? Math.Min(targetHeat, state.CurrentHeat + heatChange)
            : Math.Max(targetHeat, state.CurrentHeat + heatChange);
    }

    private double MaxSpeedRatio(EnginesState state)
    {
        return state.CurrentSpeed / (double) state.SpeedConfig.MaxSpeed;
    }

    private double CrusingSpeedRatio(EnginesState state)
    {
        return state.CurrentSpeed / (double) state.SpeedConfig.CruisingSpeed;
    }

    private int GetTargetHeat(EnginesState state)
    {
        if (state.CurrentSpeed > state.SpeedConfig.CruisingSpeed)
        {
            return (int) (state.HeatConfig.MaxHeat * MaxSpeedRatio(state));
        }

        if (state.CurrentSpeed > 0)
        {
            return Math.Max(
                (int) (state.HeatConfig.CruisingHeat * CrusingSpeedRatio(state)),
                state.HeatConfig.PoweredHeat);
        }

        if (state.CurrentPower >= state.RequiredPower)
        {
            return state.HeatConfig.PoweredHeat;
        }

        return 0;
    }

    private double GetHeatingRateInMilliseconds(EnginesState state)
    {
        var ratio = Math.Max(1, state.CurrentSpeed) / (double) state.SpeedConfig.MaxSpeed;
        var totalMilliseconds = TimeSpan.FromMinutes(state.HeatConfig.MinutesAtMaxSpeed).TotalMilliseconds;
        return (state.HeatConfig.MaxHeat / totalMilliseconds) * ratio;
    }
        
    private double GetCoolingRateInMilliseconds(EnginesState state)
    {
        var ratio = 1 - MaxSpeedRatio(state);
        var totalMilliseconds = TimeSpan.FromMinutes(state.HeatConfig.MinutesToCoolDown).TotalMilliseconds;
        return -1 * (state.HeatConfig.MaxHeat / totalMilliseconds) * ratio;
    }

    public TransformResult<EnginesState> Configure(EnginesState state, EnginesConfigurationPayload payload)
    {
        return TransformResult<EnginesState>.StateChanged(state with
        {
            HeatConfig = payload.HeatConfig,
            SpeedConfig = payload.SpeedConfig,
            RequiredPower = payload.RequiredPower,
            SpeedPowerRequirements = payload.SpeedPowerRequirements
        });
    }
}