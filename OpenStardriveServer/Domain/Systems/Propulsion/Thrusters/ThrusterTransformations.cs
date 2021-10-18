using System;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Thrusters
{
    public class ThrusterTransformations
    {
        public TransformResult<ThrustersState> Configure(ThrustersState state, ThrusterConfigurationPayload payload)
        {
            return TransformResult<ThrustersState>.StateChanged(CloneThen(state, x =>
            {
                x.Disabled = payload.Disabled;
                x.Damaged = payload.Damaged;
            }));
        }
        
        public TransformResult<ThrustersState> SetAttitude(ThrustersState state, ThrusterAttitudePayload payload)
        {
            return IfFunctional(state, () => CloneThen(state, x =>
            {
                x.Attitude = new ThrustersAttitude
                {
                    Pitch = payload.Pitch,
                    Yaw = payload.Yaw,
                    Roll = payload.Roll
                };
            }));
        }

        private TransformResult<T> IfFunctional<T>(SystemBaseState state, Func<T> stateChange)
        {
            return state.IsDisabled().OrElse(state.IsDamaged).OrElse(state.HasInsufficientPower).Case(
                some: TransformResult<T>.Error,
                none: () => TransformResult<T>.StateChanged(stateChange()));
        }

        public TransformResult<ThrustersState> SetVelocity(ThrustersState state, ThrusterVelocityPayload payload)
        {
            return IfFunctional(state, () => CloneThen(state, x =>
            {
                x.Velocity = new ThrusterVelocity
                {
                    X = payload.X,
                    Y = payload.Y,
                    Z = payload.Z,
                };
            }));
        }

        private ThrustersState CloneThen(ThrustersState original, Action<ThrustersState> then)
        {
            var clone = new ThrustersState
            {
                Disabled = original.Disabled,
                Damaged = original.Damaged,
                Attitude = original.Attitude,
                Velocity = original.Velocity
            };
            then(clone);
            return clone;
        }
    }

    public class ThrusterConfigurationPayload
    {
        public bool Disabled { get; set; }
        public bool Damaged { get; set; }
    }
}