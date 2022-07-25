using System;
using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Defense.WarheadLauncher;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Defense.WarheadLauncher;

public class WarheadLauncherSystemTests : SystemsTest<WarheadLauncherSystem>
{
    private readonly TransformResult<WarheadLauncherState> expected =
        TransformResult<WarheadLauncherState>.StateChanged(new WarheadLauncherState());

    [Test]
    public void When_constructed_the_system_name_is_set()
    {
        Assert.That(ClassUnderTest.SystemName, Is.EqualTo("warhead-launcher"));
    }

    [Test]
    public void When_reporting_state()
    {
        TestCommand("report-state", null, expected);
    }
    
    [Test]
    public void When_loading()
    {
        var payload = new LoadWarheadPayload { Kind = "torpedo" };
        GetMock<IWarheadLauncherTransforms>().Setup(x => x.Load(Any<WarheadLauncherState>(), payload)).Returns(expected);
        TestCommand("load-warhead", payload, expected);
    }
    
    [Test]
    public void When_firing()
    {
        var payload = new FireWarheadPayload { Kind = "torpedo" };
        GetMock<IWarheadLauncherTransforms>().Setup(x => x.Fire(Any<WarheadLauncherState>(), payload, Any<DateTimeOffset>())).Returns(expected);
        TestCommand("fire-warhead", payload, expected);
    }
    
    [Test]
    public void When_setting_power()
    {
        var payload = new SystemPowerPayload { CurrentPower = 3 };
        GetMock<IWarheadLauncherTransforms>().Setup(x => x.SetPower(Any<WarheadLauncherState>(), payload)).Returns(expected);
        TestCommand("set-warhead-launcher-power", payload, expected);
    }
    
    [Test]
    public void When_setting_damaged()
    {
        var payload = new SystemDamagePayload { Damaged = true };
        GetMock<IWarheadLauncherTransforms>().Setup(x => x.SetDamaged(Any<WarheadLauncherState>(), payload)).Returns(expected);
        TestCommand("set-warhead-launcher-damaged", payload, expected);
    }
    
    [Test]
    public void When_setting_disabled()
    {
        var payload = new SystemDisabledPayload { Disabled = true };
        GetMock<IWarheadLauncherTransforms>().Setup(x => x.SetDisabled(Any<WarheadLauncherState>(), payload)).Returns(expected);
        TestCommand("set-warhead-launcher-disabled", payload, expected);
    }
    
    [Test]
    public void When_setting_inventory()
    {
        var payload = new WarheadInventoryPayload();
        GetMock<IWarheadLauncherTransforms>().Setup(x => x.SetInventory(Any<WarheadLauncherState>(), Any<WarheadInventoryPayload>())).Returns(expected);
        TestCommand("set-warhead-inventory", payload, expected);
    }
}