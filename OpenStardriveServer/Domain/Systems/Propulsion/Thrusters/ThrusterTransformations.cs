namespace OpenStardriveServer.Domain.Systems.Propulsion.Thrusters
{
    public class ThrusterTransformations
    {
        public TransformResult<ThrustersState> Configure(ThrustersState state, ThrusterConfigurationPayload payload)
        {
            return TransformResult<ThrustersState>.StateChanged(state with
            {
                Disabled = payload.Disabled,
                Damaged = payload.Damaged
            });
        }
        
        public TransformResult<ThrustersState> SetAttitude(ThrustersState state, ThrusterAttitudePayload payload)
        {
            return state.IfFunctional(() => state with
            {
                Attitude = new ThrustersAttitude
                {
                    Pitch = payload.Pitch,
                    Yaw = payload.Yaw,
                    Roll = payload.Roll
                }
            });
        }

        public TransformResult<ThrustersState> SetVelocity(ThrustersState state, ThrusterVelocityPayload payload)
        {
            return state.IfFunctional(() => state with {
                Velocity = new ThrusterVelocity
                {
                    X = payload.X,
                    Y = payload.Y,
                    Z = payload.Z,
                }
            });
        }
    }

    public class ThrusterConfigurationPayload
    {
        public bool Disabled { get; set; }
        public bool Damaged { get; set; }
    }
}