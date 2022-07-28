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
        TestCommand("report-state", expected);
    }
    
    [Test]
    public void When_loading()
    {
        var payload = new LoadWarheadPayload { Kind = "torpedo" };
        GetMock<IWarheadLauncherTransforms>().Setup(x => x.Load(Any<WarheadLauncherState>(), payload)).Returns(expected);
        TestCommandWithPayload("load-warhead", payload, expected);
    }
    
    [Test]
    public void When_firing()
    {
        var payload = new FireWarheadPayload { Kind = "torpedo" };
        TestCommandWithPayload("fire-warhead", payload, expected, command =>
        {
            GetMock<IWarheadLauncherTransforms>().Setup(x => x.Fire(Any<WarheadLauncherState>(), payload, command.TimeStamp)).Returns(expected);
        });
    }
    
    [Test]
    public void When_setting_power()
    {
        var payload = new CurrentPowerPayload();
        GetMock<IWarheadLauncherTransforms>().Setup(x => x.SetPower(Any<WarheadLauncherState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-power", payload, expected);
    }
    
    [Test]
    public void When_setting_required_power()
    {
        var payload = new RequiredPowerPayload();
        GetMock<IWarheadLauncherTransforms>().Setup(x => x.SetRequiredPower(Any<WarheadLauncherState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-required-power", payload, expected);
    }
    
    [Test]
    public void When_setting_damaged()
    {
        var payload = new DamagedSystemsPayload();
        GetMock<IWarheadLauncherTransforms>().Setup(x => x.SetDamaged(Any<WarheadLauncherState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-damaged", payload, expected);
    }
    
    [Test]
    public void When_setting_disabled()
    {
        var payload = new DisabledSystemsPayload();
        GetMock<IWarheadLauncherTransforms>().Setup(x => x.SetDisabled(Any<WarheadLauncherState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-disabled", payload, expected);
    }
    
    [Test]
    public void When_setting_inventory()
    {
        var payload = new WarheadInventoryPayload();
        GetMock<IWarheadLauncherTransforms>().Setup(x => x.SetInventory(Any<WarheadLauncherState>(), payload)).Returns(expected);
        TestCommandWithPayload("set-warhead-inventory", payload, expected);
    }
}