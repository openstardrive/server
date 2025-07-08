using OpenStardriveServer.Domain.Systems.Navigation;
using OpenStardriveServer.Domain.Systems.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Plugins;

public class JsonPluginTransformsTests: StandardTransformsTest<JsonPluginTransforms, JsonPluginState>
{
    [Test]
    public void When_disabling()
    {
        TestStandardDisabled(new JsonPluginState());
    }

    [Test]
    public void When_damaged()
    {
        TestStandardDamaged(new JsonPluginState());
    }

    [Test]
    public void When_setting_current_power()
    {
        TestStandardCurrentPower(new JsonPluginState());
    }

    [Test]
    public void When_setting_required_power()
    {
        TestStandardRequiredPower(new JsonPluginState());
    }

    [Test]
    public void When_updating_json_state_with_new_key()
    {
        var state = new JsonPluginState();
        string key="test-key";
        var payload = new UpdateJsonStatePayload
        {
            Value = "test-value"
        };

        var result = ClassUnderTest.UpdateJsonState(state, key, payload);

        var jsonState = result.NewState.Value.JsonState;

        Assert.That(jsonState, Has.Count.EqualTo(1));
        Assert.That(jsonState, Contains.Key(key));
        Assert.That(jsonState[key], Is.EqualTo(payload.Value));
    }

    [Test]
    public void When_updating_json_state_with_existing_key()
    {
        var state = new JsonPluginState
        {
            JsonState = new Dictionary<string, object>
            {
                { "existing-key", "existing-value" }
            }
        };
        string key = "existing-key";
        var payload = new UpdateJsonStatePayload
        {
            Value = "new-value"
        };
        var result = ClassUnderTest.UpdateJsonState(state, key, payload);
        var jsonState = result.NewState.Value.JsonState;
        Assert.That(jsonState, Has.Count.EqualTo(1));
        Assert.That(jsonState, Contains.Key(key));
        Assert.That(jsonState[key], Is.EqualTo(payload.Value));
    }
}
