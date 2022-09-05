using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Alert;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Alert;

public class AlertSystemTests : SystemsTest<AlertSystem>
{
    [Test]
    public void When_constructed_system_name_is_set()
    {
        Assert.That(ClassUnderTest.SystemName, Is.EqualTo("alert"));
    }
    
    private readonly TransformResult<AlertState> expected =
        TransformResult<AlertState>.StateChanged(new AlertState());

    [Test]
    public void When_reporting_state()
    {
        TestCommand("report-state", expected);
    }

    [Test]
    public void When_configuring()
    {
        var payload = new ConfigureAlertLevelsPayload();
        GetMock<IAlertTransforms>().Setup(x => x.Configure(payload)).Returns(expected);
        TestCommandWithPayload("configure-alert-levels", payload, expected);
    }
    
    [Test]
    public void When_setting_alert_level()
    {
        var payload = new SetAlertLevelPayload();
        GetMock<IAlertTransforms>().Setup(x => x.SetLevel(Any<AlertState>(), payload)).Returns(expected);
        TestCommandWithPayload("set-alert-level", payload, expected);
    }
}