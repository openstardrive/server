namespace OpenStardriveServer.Domain.Systems.Standard;

public class StandardSystemBase<T> : SystemBase<T> where T : StandardSystemBaseState, new()
{
    private readonly StandardSystemBaseStateTransformations<T> standardTransformations = new();
    
    protected void AddStandardTransforms()
    {
        CommandProcessors[$"set-{SystemName}-power"] = (c) => Update(c, standardTransformations.SetCurrentPower(state, Json.Deserialize<SystemPowerPayload>(c.Payload)));
        CommandProcessors[$"set-{SystemName}-damaged"] = (c) => Update(c, standardTransformations.SetDamage(state, Json.Deserialize<SystemDamagePayload>(c.Payload)));
        CommandProcessors[$"set-{SystemName}-disabled"] = (c) => Update(c, standardTransformations.SetDisabled(state, Json.Deserialize<SystemDisabledPayload>(c.Payload)));
    }
}