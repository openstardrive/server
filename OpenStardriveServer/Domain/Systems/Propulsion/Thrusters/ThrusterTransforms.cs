using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Thrusters;

public interface IThrusterTransforms : IStandardTransforms<ThrustersState>
{
    TransformResult<ThrustersState> SetAttitude(ThrustersState state, ThrusterAttitudePayload payload);
    TransformResult<ThrustersState> SetVelocity(ThrustersState state, ThrusterVelocityPayload payload);
}

public class ThrusterTransforms : IThrusterTransforms
{
    private readonly IStandardTransforms<ThrustersState> standardTransforms;

    public ThrusterTransforms(IStandardTransforms<ThrustersState> standardTransforms)
    {
        this.standardTransforms = standardTransforms;
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

    public TransformResult<ThrustersState> SetCurrentPower(ThrustersState state, string systemName, CurrentPowerPayload payload)
    {
        return standardTransforms.SetCurrentPower(state, systemName, payload);
    }
    
    public TransformResult<ThrustersState> SetRequiredPower(ThrustersState state, string systemName, RequiredPowerPayload payload)
    {
        return standardTransforms.SetRequiredPower(state, systemName, payload);
    }

    public TransformResult<ThrustersState> SetDamaged(ThrustersState state, string systemName, DamagedSystemsPayload payload)
    {
        return standardTransforms.SetDamaged(state, systemName, payload);
    }

    public TransformResult<ThrustersState> SetDisabled(ThrustersState state, string systemName, DisabledSystemsPayload payload)
    {
        return standardTransforms.SetDisabled(state, systemName, payload);
    }
}