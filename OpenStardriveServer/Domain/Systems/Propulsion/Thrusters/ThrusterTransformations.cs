namespace OpenStardriveServer.Domain.Systems.Propulsion.Thrusters
{
    public class ThrusterTransformations
    {
        public TransformResult<ThrustersState> Configure(ThrustersState state, ThrusterConfigurationPayload payload)
        {
            return TransformResult<ThrustersState>.StateChanged(state with
            {
                RequiredPower = payload.RequiredPower
            });
        }
        
        public TransformResult<ThrustersState> SetAttitude(ThrustersState state, ThrusterAttitudePayload payload)
        {
            return state.IfFunctional(() => state with
            {
                Attitude = new ThrustersAttitude
                {
                    Pitch = LimitTo360Degrees(payload.Pitch),
                    Yaw = LimitTo360Degrees(payload.Yaw),
                    Roll = LimitTo360Degrees(payload.Roll)
                }
            });
        }

        private int LimitTo360Degrees(int input)
        {
            var normalized = input < 0 ? 360 + (input % 360) : input;
            return normalized % 360;
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
}