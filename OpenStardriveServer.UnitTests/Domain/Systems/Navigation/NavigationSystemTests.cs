using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Navigation;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Navigation;

public class NavigationSystemTests : SystemsTest<NavigationSystem>
{
    private readonly TransformResult<NavigationState> expected =
        TransformResult<NavigationState>.StateChanged(new NavigationState());
    
    [Test]
    public void When_constructing_the_system_name_gets_set()
    {
        Assert.That(ClassUnderTest.SystemName, Is.EqualTo("navigation"));
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
        GetMock<INavigationTransforms>().Setup(x => x.SetDisabled(Any<NavigationState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-disabled", payload, expected);
    }
    
    [Test]
    public void When_setting_damaged()
    {
        var payload = new DamagedSystemsPayload();
        GetMock<INavigationTransforms>().Setup(x => x.SetDamaged(Any<NavigationState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-damaged", payload, expected);
    }
    
    [Test]
    public void When_setting_current_power()
    {
        var payload = new CurrentPowerPayload();
        GetMock<INavigationTransforms>().Setup(x => x.SetCurrentPower(Any<NavigationState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-power", payload, expected);
    }
    
    [Test]
    public void When_setting_required_power()
    {
        var payload = new RequiredPowerPayload();
        GetMock<INavigationTransforms>().Setup(x => x.SetRequiredPower(Any<NavigationState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-required-power", payload, expected);
    }
    
    [Test]
    public void When_requesting_course()
    {
        var payload = new RequestedCourseCalculationPayload();
        GetMock<INavigationTransforms>().Setup(x => x.RequestCourse(Any<NavigationState>(), payload)).Returns(expected);
        TestCommandWithPayload("request-course-calculation", payload, expected);
    }
    
    [Test]
    public void When_cancelling_a_requested_course()
    {
        var payload = new CancelRequestedCourseCalculationPayload();
        GetMock<INavigationTransforms>().Setup(x => x.CancelRequestedCourse(Any<NavigationState>(), payload)).Returns(expected);
        TestCommandWithPayload("cancel-course-calculation", payload, expected);
    }
    
    [Test]
    public void When_a_course_is_calculated()
    {
        var payload = new CalculatedCoursePayload();
        GetMock<INavigationTransforms>().Setup(x => x.CourseCalculated(Any<NavigationState>(), payload)).Returns(expected);
        TestCommandWithPayload("course-calculated", payload, expected);
    }
    
    [Test]
    public void When_a_course_is_set()
    {
        var payload = new SetCoursePayload();
        GetMock<INavigationTransforms>().Setup(x => x.SetCourse(Any<NavigationState>(), payload)).Returns(expected);
        TestCommandWithPayload("set-course", payload, expected);
    }
    
    [Test]
    public void When_the_eta_is_updated()
    {
        var payload = new SetEtaPayload();
        GetMock<INavigationTransforms>().Setup(x => x.UpdateEta(Any<NavigationState>(), payload)).Returns(expected);
        TestCommandWithPayload("update-eta", payload, expected);
    }
    
    [Test]
    public void When_the_eta_is_cleared()
    {
        GetMock<INavigationTransforms>().Setup(x => x.ClearEta(Any<NavigationState>())).Returns(expected);
        TestCommand("clear-eta", expected);
    }
    
    [Test]
    public void When_the_chronometer_fires()
    {
        var payload = new ChronometerPayload();
        GetMock<INavigationTransforms>().Setup(x => x.Travel(Any<NavigationState>(), payload)).Returns(expected);
        TestCommandWithPayload(ChronometerCommand.Type, payload, expected);
    }
}