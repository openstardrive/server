using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Comms.LongRange;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Comms.LongRange;

public class LongRangeSystemTests : SystemsTest<LongRangeSystem>
{
    private readonly TransformResult<LongRangeState> expected =
        TransformResult<LongRangeState>.StateChanged(new LongRangeState());

    [Test]
    public void When_constructed_the_system_name_is_set()
    {
        Assert.That(ClassUnderTest.SystemName, Is.EqualTo("long-range-comms"));
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
        GetMock<ILongRangeTransforms>().Setup(x => x.SetDisabled(Any<LongRangeState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-disabled", payload, expected);
    }
    
    [Test]
    public void When_setting_damaged()
    {
        var payload = new DamagedSystemsPayload();
        GetMock<ILongRangeTransforms>().Setup(x => x.SetDamaged(Any<LongRangeState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-damaged", payload, expected);
    }
    
    [Test]
    public void When_setting_current_power()
    {
        var payload = new CurrentPowerPayload();
        GetMock<ILongRangeTransforms>().Setup(x => x.SetCurrentPower(Any<LongRangeState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-power", payload, expected);
    }
    
    [Test]
    public void When_setting_required_power()
    {
        var payload = new RequiredPowerPayload();
        GetMock<ILongRangeTransforms>().Setup(x => x.SetRequiredPower(Any<LongRangeState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-required-power", payload, expected);
    }
    
    [Test]
    public void When_setting_a_cypher()
    {
        var payload = new SetCypherPayload();
        GetMock<ILongRangeTransforms>().Setup(x => x.SetCypher(Any<LongRangeState>(), payload)).Returns(expected);
        TestCommandWithPayload("set-long-range-cypher", payload, expected);
    }
    
    [Test]
    public void When_updating_cypher_substitutions()
    {
        var payload = new UpdateCypherSubstitutionsPayload();
        GetMock<ILongRangeTransforms>().Setup(x => x.UpdateCypherSubstitutions(Any<LongRangeState>(), payload)).Returns(expected);
        TestCommandWithPayload("update-long-range-substitutions", payload, expected);
    }
    
    [Test]
    public void When_sending_a_long_range_message()
    {
        var payload = new LongRangeMessagePayload();
        GetMock<ILongRangeTransforms>().Setup(x => x.SendLongRangeMessage(Any<LongRangeState>(), payload)).Returns(expected);
        TestCommandWithPayload("send-long-range-message", payload, expected);
    }
}