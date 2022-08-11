using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Sensors;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Sensors;

public class SensorsSystemTests : SystemsTest<SensorsSystem>
{
    private readonly TransformResult<SensorsState> expected =
        TransformResult<SensorsState>.StateChanged(new SensorsState());
    
    [Test]
    public void When_constructed_the_name_is_set()
    {
        Assert.That(ClassUnderTest.SystemName, Is.EqualTo("sensors"));
    }
    
    [Test]
    public void When_reporting_state()
    {
        TestCommand("report-state", TransformResult<SensorsState>.StateChanged(new SensorsState()));    
    }
    
    [Test]
    public void When_setting_disabled()
    {
        var payload = new DisabledSystemsPayload();
        GetMock<ISensorsTransforms>().Setup(x => x.SetDisabled(Any<SensorsState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-disabled", payload, expected);
    }
    
    [Test]
    public void When_setting_damaged()
    {
        var payload = new DamagedSystemsPayload();
        GetMock<ISensorsTransforms>().Setup(x => x.SetDamaged(Any<SensorsState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-damaged", payload, expected);
    }
    
    [Test]
    public void When_setting_current_power()
    {
        var payload = new CurrentPowerPayload();
        GetMock<ISensorsTransforms>().Setup(x => x.SetCurrentPower(Any<SensorsState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-power", payload, expected);
    }
    
    [Test]
    public void When_setting_required_power()
    {
        var payload = new RequiredPowerPayload();
        GetMock<ISensorsTransforms>().Setup(x => x.SetRequiredPower(Any<SensorsState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-required-power", payload, expected);
    }
    
    [Test]
    public void When_creating_a_new_scan()
    {
        var payload = new NewScanPayload();
        GetMock<ISensorsTransforms>().Setup(x => x.NewScan(Any<SensorsState>(), payload)).Returns(expected);
        TestCommandWithPayload("new-sensor-scan", payload, expected);
    }
    
    [Test]
    public void When_setting_scan_result()
    {
        var payload = new ScanResultPayload();
        GetMock<ISensorsTransforms>().Setup(x => x.SetScanResult(Any<SensorsState>(), payload)).Returns(expected);
        TestCommandWithPayload("set-sensor-scan-result", payload, expected);
    }
    
    [Test]
    public void When_canceling_a_scan()
    {
        var payload = new CancelScanPayload();
        GetMock<ISensorsTransforms>().Setup(x => x.CancelScan(Any<SensorsState>(), payload)).Returns(expected);
        TestCommandWithPayload("cancel-sensor-scan", payload, expected);
    }
    
    [Test]
    public void When_creating_a_passive_scan()
    {
        var payload = new PassiveScanPayload();
        GetMock<ISensorsTransforms>().Setup(x => x.PassiveScan(Any<SensorsState>(), payload)).Returns(expected);
        TestCommandWithPayload("passive-sensor-scan", payload, expected);
    }
    
    [Test]
    public void When_adding_a_new_contact()
    {
        var payload = new NewSensorContactPayload();
        GetMock<ISensorsTransforms>().Setup(x => x.NewContact(Any<SensorsState>(), payload)).Returns(expected);
        TestCommandWithPayload("new-sensor-contact", payload, expected);
    }
    
    [Test]
    public void When_removing_a_contact()
    {
        var payload = new RemoveSensorContactPayload();
        GetMock<ISensorsTransforms>().Setup(x => x.RemoveContact(Any<SensorsState>(), payload)).Returns(expected);
        TestCommandWithPayload("remove-sensor-contact", payload, expected);
    }
    
    [Test]
    public void When_updating_a_contact()
    {
        var payload = new UpdateSensorContactPayload();
        GetMock<ISensorsTransforms>().Setup(x => x.UpdateContact(Any<SensorsState>(), payload)).Returns(expected);
        TestCommandWithPayload("update-sensor-contact", payload, expected);
    }
    
    [Test]
    public void When_moving_contacts()
    {
        var payload = new ChronometerPayload();
        GetMock<ISensorsTransforms>().Setup(x => x.MoveContacts(Any<SensorsState>(), payload)).Returns(expected);
        TestCommandWithPayload(ChronometerCommand.Type, payload, expected);
    }
}