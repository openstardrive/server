using System;
using OpenStardriveServer.Domain.Chronometer;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Engines
{
    public class EnginesTransformations
    {
        public TransformResult<EnginesState> SetSpeed(EnginesState state, SetSpeedPayload payload)
        {
            if (state.CurrentSpeed == payload.Speed)
            {
                return TransformResult<EnginesState>.NoChange();
            }

            return IfFunctional(state, () => CloneThen(state, x =>
            {
                x.CurrentSpeed = payload.Speed;
            }));
        }

        public TransformResult<EnginesState> UpdateHeat(EnginesState state, ChronometerPayload payload)
        {
            var newHeat = CalculateNewHeat(state, payload);
            if (newHeat != state.CurrentHeat)
            {
                return TransformResult<EnginesState>.StateChanged(CloneThen(state, x =>
                {
                    x.CurrentHeat = newHeat;
                }));
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

            var rate = targetHeat > state.CurrentHeat
                ? GetHeatingRateInMilliseconds(state)
                : GetCoolingRateInMilliseconds(state);
            
            var heatChange = (int) (rate * payload.ElapsedMilliseconds);
            return state.CurrentHeat + heatChange;
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
                return (int) (state.HeatConfig.CruisingHeat * CrusingSpeedRatio(state));
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
        
        private TransformResult<T> IfFunctional<T>(SystemBaseState state, Func<T> stateChange)
        {
            return state.IsDisabled().OrElse(state.IsDamaged).OrElse(state.HasInsufficientPower).Case(
                some: TransformResult<T>.Error,
                none: () => TransformResult<T>.StateChanged(stateChange()));
        }
        
        private EnginesState CloneThen(EnginesState original, Action<EnginesState> then)
        {
            var clone = new EnginesState
            {
                Disabled = original.Disabled,
                Damaged = original.Damaged,
                CurrentSpeed = original.CurrentSpeed,
                SpeedConfig = original.SpeedConfig,
                CurrentHeat = original.CurrentHeat,
                HeatConfig = original.HeatConfig
            };
            then(clone);
            return clone;
        } 
    }
}