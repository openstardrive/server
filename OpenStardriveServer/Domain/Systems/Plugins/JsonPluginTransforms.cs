using System;
using System.Linq;
using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems.Propulsion.Engines;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Plugins;

public interface IJsonPluginTransforms : IStandardTransforms<JsonPluginState>
{
    TransformResult<JsonPluginState> UpdateJsonState(JsonPluginState state, string key, UpdateJsonStatePayload payload);
}

public class JsonPluginTransforms : IJsonPluginTransforms
{
    private readonly IStandardTransforms<JsonPluginState> standardTransforms;

    public JsonPluginTransforms(IStandardTransforms<JsonPluginState> standardTransforms)
    {
        this.standardTransforms = standardTransforms;
    }
    public TransformResult<JsonPluginState> SetDisabled(JsonPluginState state, string systemName, DisabledSystemsPayload payload)
    {
        return standardTransforms.SetDisabled(state, systemName, payload);
    }
    public TransformResult<JsonPluginState> SetDamaged(JsonPluginState state, string systemName, DamagedSystemsPayload payload)
    {
        return standardTransforms.SetDamaged(state, systemName, payload);
    }
    public TransformResult<JsonPluginState> SetCurrentPower(JsonPluginState state, string systemName, CurrentPowerPayload payload)
    {
        return standardTransforms.SetCurrentPower(state, systemName, payload);
    }
    public TransformResult<JsonPluginState> SetRequiredPower(JsonPluginState state, string systemName, RequiredPowerPayload payload)
    {
        return standardTransforms.SetRequiredPower(state, systemName, payload);
    }
    public TransformResult<JsonPluginState> UpdateJsonState(JsonPluginState state, string key, UpdateJsonStatePayload payload)
    {
        return TransformResult<JsonPluginState>.StateChanged(state with
        {
            JsonState = new System.Collections.Generic.Dictionary<string, object>(state.JsonState)
            {
                [key] = payload.Value
            }
        });
    }
}