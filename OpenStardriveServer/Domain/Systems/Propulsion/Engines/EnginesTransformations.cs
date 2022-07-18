using System;
using System.Linq;
using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Engines
{
    public class EnginesTransformations
    {
        public TransformResult<EnginesState> SetSpeed(EnginesState state, SetSpeedPayload payload)
        {
            return state.IfFunctional(() =>
            {
                var powerRequirement = state.SpeedPowerRequirements.FirstOrDefault(x => x.Speed == payload.Speed);
                if (powerRequirement is not null && powerRequirement.PowerNeeded > state.CurrentPower)
                {
                    return TransformResult<EnginesState>.Error(SystemBaseState.InsufficientPowerError);
                }

                return TransformResult<EnginesState>.StateChanged(state with { CurrentSpeed = payload.Speed });
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
}