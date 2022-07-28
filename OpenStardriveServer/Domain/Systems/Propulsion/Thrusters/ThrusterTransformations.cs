using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Propulsion.Thrusters;

public interface IThrusterTransformations
{
    TransformResult<ThrustersState> SetAttitude(ThrustersState state, ThrusterAttitudePayload payload);
    TransformResult<ThrustersState> SetVelocity(ThrustersState state, ThrusterVelocityPayload payload);
    TransformResult<ThrustersState> SetCurrentPower(ThrustersState state, string systemName, CurrentPowerPayload payload);
    TransformResult<ThrustersState> SetRequiredPower(ThrustersState state, string systemName, RequiredPowerPayload payload);
    TransformResult<ThrustersState> SetDamage(ThrustersState state, SystemDamagePayload payload);
    TransformResult<ThrustersState> SetDisabled(ThrustersState state, SystemDisabledPayload payload);
}

public class ThrusterTransformations : IThrusterTransformations
{
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
        return payload.ValueOrNone(systemName).Case(
            some: x => TransformResult<ThrustersState>.StateChanged(state with { CurrentPower = x }),
            none: TransformResult<ThrustersState>.NoChange);
    }
    
    public TransformResult<ThrustersState> SetRequiredPower(ThrustersState state, string systemName, RequiredPowerPayload payload)
    {
        return payload.ValueOrNone(systemName).Case(
            some: x => TransformResult<ThrustersState>.StateChanged(state with { RequiredPower = x }),
            none: TransformResult<ThrustersState>.NoChange);
    }
    
    public TransformResult<ThrustersState> SetDamage(ThrustersState state, SystemDamagePayload payload)
    {
        return TransformResult<ThrustersState>.StateChanged(state with { Damaged = payload.Damaged });
    }
    
    public TransformResult<ThrustersState> SetDisabled(ThrustersState state, SystemDisabledPayload payload)
    {
        return TransformResult<ThrustersState>.StateChanged(state with { Disabled = payload.Disabled });
    }
}