using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Comms.ShortRange;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Comms.ShortRange;

public class ShortRangeTransformsTests : StandardTransformsTest<ShortRangeTransforms, ShortRangeState>
{
    [Test]
    public void When_damaged()
    {
        TestStandardDamaged(new ShortRangeState());
    }

    [Test]
    public void When_damaged_broadcasting_stops()
    {
        var state = new ShortRangeState { Damaged = false, IsBroadcasting = true };
        var systemName = "comms";
        var payload = new DamagedSystemsPayload();
        var updatedState = state with { Damaged = true };
        GetMock<IStandardTransforms<ShortRangeState>>().Setup(x => x.SetDamaged(state, systemName, payload))
            .Returns(TransformResult<ShortRangeState>.StateChanged(updatedState));

        var result = ClassUnderTest.SetDamaged(state, systemName, payload);
        
        Assert.That(result.NewState.Value, Is.EqualTo(updatedState with { IsBroadcasting = false }));
    }

    [Test]
    public void When_disabled()
    {
        TestStandardDisabled(new ShortRangeState());
    }
    
    [Test]
    public void When_disabled_broadcasting_stops()
    {
        var state = new ShortRangeState { Disabled = false, IsBroadcasting = true };
        var systemName = "comms";
        var payload = new DisabledSystemsPayload();
        var updatedState = state with { Disabled = true };
        GetMock<IStandardTransforms<ShortRangeState>>().Setup(x => x.SetDisabled(state, systemName, payload))
            .Returns(TransformResult<ShortRangeState>.StateChanged(updatedState));

        var result = ClassUnderTest.SetDisabled(state, systemName, payload);
        
        Assert.That(result.NewState.Value, Is.EqualTo(updatedState with { IsBroadcasting = false }));
    }

    [Test]
    public void When_setting_current_power()
    {
        TestStandardCurrentPower(new ShortRangeState());
    }
    
    [Test]
    public void When_setting_current_power_broadcasting_stops_if_insufficient()
    {
        var state = new ShortRangeState { RequiredPower = 5, CurrentPower = 5, IsBroadcasting = true };
        var systemName = "comms";
        var payload = new CurrentPowerPayload();
        var updatedState = state with { CurrentPower = 4 };
        GetMock<IStandardTransforms<ShortRangeState>>().Setup(x => x.SetCurrentPower(state, systemName, payload))
            .Returns(TransformResult<ShortRangeState>.StateChanged(updatedState));

        var result = ClassUnderTest.SetCurrentPower(state, systemName, payload);
        
        Assert.That(result.NewState.Value, Is.EqualTo(updatedState with { IsBroadcasting = false }));
    }
    
    [Test]
    public void When_setting_required_power()
    {
        TestStandardRequiredPower(new ShortRangeState());
    }
    
    [Test]
    public void When_setting_required_power_broadcasting_stops_if_insufficient()
    {
        var state = new ShortRangeState { RequiredPower = 5, CurrentPower = 5, IsBroadcasting = true };
        var systemName = "comms";
        var payload = new RequiredPowerPayload();
        var updatedState = state with { RequiredPower = 6 };
        GetMock<IStandardTransforms<ShortRangeState>>().Setup(x => x.SetRequiredPower(state, systemName, payload))
            .Returns(TransformResult<ShortRangeState>.StateChanged(updatedState));

        var result = ClassUnderTest.SetRequiredPower(state, systemName, payload);
        
        Assert.That(result.NewState.Value, Is.EqualTo(updatedState with { IsBroadcasting = false }));
    }

    [Test]
    public void When_configuring_frequency_ranges()
    {
        var state = new ShortRangeState();
        var payload = new ConfigureFrequencyRangesPayload
        {
            FrequencyRanges = new[]
            {
                new FrequencyRange { Name = "Allies", Min = 1078.36, Max = 2345.23 },
                new FrequencyRange { Name = "Aliens", Min = 4864.81, Max = 8934.01 }
            }
        };
        
        var result = ClassUnderTest.ConfigureFrequencyRanges(state, payload);
        
        Assert.That(result.NewState.Value.FrequencyRanges, Is.SameAs(payload.FrequencyRanges));
    }

    [Test]
    public void When_setting_active_signals()
    {
        var state = new ShortRangeState();
        var payload = new SetActiveSignalsPayload
        {
            ActiveSignals = new[]
            {
                new Signal { Name = "Starship Enterprise", Frequency = 1701.4 }
            }
        };
        
        var result = ClassUnderTest.SetActiveSignals(state, payload);
        
        Assert.That(result.NewState.Value.ActiveSignals, Is.SameAs(payload.ActiveSignals));
    }

    [Test]
    public void When_setting_current_frequency()
    {
        var state = new ShortRangeState();
        var payload = new SetCurrentFrequencyPayload { Frequency = 1234.56 };

        var result = ClassUnderTest.SetCurrentFrequency(state, payload);
        
        Assert.That(result.NewState.Value.CurrentFrequency, Is.EqualTo(payload.Frequency));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void When_setting_broadcasting(bool isBroadcasting)
    {
        var state = new ShortRangeState { IsBroadcasting = !isBroadcasting };
        var payload = new SetBroadcastingPayload { IsBroadcasting = isBroadcasting };

        var result = ClassUnderTest.SetBroadcasting(state, payload);

        Assert.That(result.NewState.Value.IsBroadcasting, Is.EqualTo(isBroadcasting));
    }
    
    [TestCase(true, false, false, StandardSystemBaseState.DamagedError)]
    [TestCase(false, true, false, StandardSystemBaseState.DisabledError)]
    [TestCase(false, false, true, StandardSystemBaseState.InsufficientPowerError)]
    public void When_setting_broadcasting_but_system_is_nonfunctional(bool isDamaged, bool isDisabled, bool hasInsufficientPower, string expected)
    {
        var state = new ShortRangeState
        {
            Damaged = isDamaged,
            Disabled = isDisabled,
            RequiredPower = 5,
            CurrentPower = hasInsufficientPower ? 0 : 5 

        };
        var payload = new SetBroadcastingPayload { IsBroadcasting = true };

        var result = ClassUnderTest.SetBroadcasting(state, payload);

        Assert.That(result.ErrorMessage, Is.EqualTo(expected));
    }
}