using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Navigation;
using OpenStardriveServer.Domain.Systems.Plugins;
using OpenStardriveServer.Domain.Systems.Standard;
using OpenStardriveServer.UnitTests.Domain.Systems.Propulsion.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Plugins; 

public class JsonPluginSystemTests : SystemsTest<TestingJsonPluginSystem>
{
    private readonly TransformResult<JsonPluginState> expected =
    TransformResult<JsonPluginState>.StateChanged(new JsonPluginState());

    [Test]
    public void When_constructing_the_system_name_gets_set()
    {
        Assert.That(ClassUnderTest.SystemName, Is.EqualTo("json-plugin-test"));
    }
    [Test]
    public void When_reporting_state()
    {
        TestCommand("report-state", expected);
    }
    [Test]
    public void When_setting_disabled()
    {
        var payload = new DisabledSystemsPayload();
        GetMock<IJsonPluginTransforms>().Setup(x => x.SetDisabled(Any<JsonPluginState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-disabled", payload, expected);
    }

    [Test]
    public void When_setting_damaged()
    {
        var payload = new DamagedSystemsPayload();
        GetMock<IJsonPluginTransforms>().Setup(x => x.SetDamaged(Any<JsonPluginState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-damaged", payload, expected);
    }

    [Test]
    public void When_setting_current_power()
    {
        var payload = new CurrentPowerPayload();
        GetMock<IJsonPluginTransforms>().Setup(x => x.SetCurrentPower(Any<JsonPluginState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-power", payload, expected);
    }

    [Test]
    public void When_setting_required_power()
    {
        var payload = new RequiredPowerPayload();
        GetMock<IJsonPluginTransforms>().Setup(x => x.SetRequiredPower(Any<JsonPluginState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-required-power", payload, expected);
    }

    [Test]
    public void When_updating_json_state()
    {
        var payload = new UpdateJsonStatePayload
        {
            Key = "test-key",
            Value = "test-value"
        };
        GetMock<IJsonPluginTransforms>().Setup(x => x.UpdateJsonState(Any<JsonPluginState>(), payload)).Returns(expected);
        TestCommandWithPayload("update-json-state-test", payload, expected);
    }
}
