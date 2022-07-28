using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Propulsion.Engines;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Propulsion.Engines;

public class EnginesTransformationsTests : WithAnAutomocked<EnginesTransformations>
{
    [Test]
    public void When_setting_speed_successfully()
    {
        var payload = new SetSpeedPayload { Speed = 3 };
        var expected = EnginesStateDefaults.Testing with { CurrentSpeed = 3 };

        var result = ClassUnderTest.SetSpeed(EnginesStateDefaults.Testing, payload);
            
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.StateChanged));
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }

    [Test]
    public void When_setting_speed_and_it_is_already_at_that_speed()
    {
        var originalState = EnginesStateDefaults.Testing with { CurrentSpeed = 2 };
        var payload = new SetSpeedPayload { Speed = 2 };

        var result = ClassUnderTest.SetSpeed(originalState, payload);
            
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.StateChanged));
        Assert.That(result.NewState.Value, Is.EqualTo(originalState));
    }
        
    [TestCase(-1)]
    [TestCase(11)]
    public void When_setting_speed_to_an_invalid_value(int invalidSpeed)
    {
        var payload = new SetSpeedPayload { Speed = invalidSpeed };

        var result = ClassUnderTest.SetSpeed(EnginesStateDefaults.Testing, payload);
            
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo("invalid speed"));
    }

    [Test]
    public void When_setting_speed_but_insufficient_power()
    {
        var state = EnginesStateDefaults.Testing with { CurrentPower = 0, RequiredPower = 1 };

        var payload = new SetSpeedPayload { Speed = 3 };

        var result = ClassUnderTest.SetSpeed(state, payload);
            
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo(StandardSystemBaseState.InsufficientPowerError));
    }
        
    [Test]
    public void When_setting_speed_but_insufficient_power_for_that_speed()
    {
        var state = EnginesStateDefaults.Testing with
        {
            CurrentPower = 1,
            RequiredPower = 1,
            SpeedPowerRequirements = new[]
            {
                new SpeedPowerRequirement { Speed = 2, PowerNeeded = 2 }
            }
        };

        var payload = new SetSpeedPayload { Speed = 2 };

        var result = ClassUnderTest.SetSpeed(state, payload);
            
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo(StandardSystemBaseState.InsufficientPowerError));
    }
        
    [Test]
    public void When_setting_speed_with_enough_power_for_that_speed()
    {
        var state = EnginesStateDefaults.Testing with
        {
            CurrentPower = 2,
            RequiredPower = 1,
            SpeedPowerRequirements = new[]
            {
                new SpeedPowerRequirement { Speed = 2, PowerNeeded = 2 }
            }
        };
        var payload = new SetSpeedPayload { Speed = 2 };
        var expected = state with { CurrentSpeed = payload.Speed};

        var result = ClassUnderTest.SetSpeed(state, payload);
            
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.StateChanged));
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }
        
    [Test]
    public void When_setting_speed_but_system_is_damaged()
    {
        var state = EnginesStateDefaults.Testing with { Damaged = true };

        var payload = new SetSpeedPayload { Speed = 3 };

        var result = ClassUnderTest.SetSpeed(state, payload);
            
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo(StandardSystemBaseState.DamagedError));
    }

    [Test]
    public void When_setting_speed_but_system_is_disabled()
    {
        var state = EnginesStateDefaults.Testing with { Disabled = true };
        var payload = new SetSpeedPayload { Speed = 3 };

        var result = ClassUnderTest.SetSpeed(state, payload);
            
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo(StandardSystemBaseState.DisabledError));
    }

    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(false, false)]
    [TestCase(true, false)]
    public void When_setting_damage(bool newDamaged, bool expectChange)
    {
        var systemName = "engines";
        var payload = new DamagedSystemsPayload { ["other"] = false };
        if (expectChange)
        {
            payload[systemName] = newDamaged;
        }

        var result = ClassUnderTest.SetDamaged(new EnginesState { Damaged = !newDamaged }, systemName, payload);

        if (expectChange)
        {
            var expected = new EnginesState { Damaged = newDamaged };
            Assert.That(result.NewState.Value, Is.EqualTo(expected));
        }
        else
        {
            Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));    
        }
    }

    [Test]
    public void When_system_is_damaged_speed_drops_to_zero()
    {
        var systemName = "engines";
        var state = EnginesStateDefaults.Testing with { CurrentSpeed = 2 };
        var expected = EnginesStateDefaults.Testing with { CurrentSpeed = 0, Damaged = true };
        var payload = new DamagedSystemsPayload { [systemName] = true };

        var result = ClassUnderTest.SetDamaged(state, systemName, payload);
            
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(4)]
    public void When_power_changes(int newPower)
    {
        var systemName = "engines";
        var state = EnginesStateDefaults.Testing with { CurrentPower = 2 };
        var expected = EnginesStateDefaults.Testing with { CurrentPower = newPower };
        var payload = new CurrentPowerPayload
        {
            ["other"] = 11,
            [systemName] = newPower
        };

        var result = ClassUnderTest.SetCurrentPower(state, systemName, payload);
            
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }
    
    [Test]
    public void When_power_changes_but_no_matching_system()
    {
        var state = EnginesStateDefaults.Testing with { CurrentPower = 2 };
        var payload = new CurrentPowerPayload { ["other"] = 11 };

        var result = ClassUnderTest.SetCurrentPower(state, "engines", payload);
            
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));
    }
        
    [TestCase(2, 0, 0)]
    [TestCase(9, 0, 0)]
    [TestCase(2, 2, 2)]
    [TestCase(7, 2, 6)]
    [TestCase(9, 2, 6)]
    [TestCase(9, 4, 6)]
    [TestCase(9, 5, 8)]
    public void When_power_drops_speed_drops_to_next_available(int priorSpeed, int newPower, int expectedSpeed)
    {
        var state = EnginesStateDefaults.Testing with {
            CurrentSpeed = priorSpeed,
            CurrentPower = 2,
            RequiredPower = 2,
            SpeedPowerRequirements = new []
            {
                new SpeedPowerRequirement { Speed = 7, PowerNeeded = 5 },
                new SpeedPowerRequirement { Speed = 9, PowerNeeded = 8 }
            }
        };
        var expected = state with { CurrentPower = newPower, CurrentSpeed = expectedSpeed};
        var payload = new CurrentPowerPayload { ["engines"] = newPower };

        var result = ClassUnderTest.SetCurrentPower(state, "engines", payload);
            
        Assert.That(result.NewState.Value.CurrentSpeed, Is.EqualTo(expectedSpeed));
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }
    
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(4)]
    public void When_required_power_changes(int newPower)
    {
        var systemName = "engines";
        var state = EnginesStateDefaults.Testing with { RequiredPower = 2 };
        var expected = EnginesStateDefaults.Testing with { RequiredPower = newPower };
        var payload = new RequiredPowerPayload
        {
            ["other"] = 11,
            [systemName] = newPower
        };

        var result = ClassUnderTest.SetRequiredPower(state, systemName, payload);
            
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }
    
    [Test]
    public void When_required_power_changes_but_no_matching_system()
    {
        var state = EnginesStateDefaults.Testing with { RequiredPower = 2 };
        var payload = new RequiredPowerPayload { ["other"] = 11 };

        var result = ClassUnderTest.SetRequiredPower(state, "engines", payload);
            
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));
    }
    
    [TestCase(2, 2, 0, 2)]
    [TestCase(2, 2, 3, 0)]
    [TestCase(9, 8, 3, 9)]
    [TestCase(7, 5, 6, 7)]
    public void When_required_power_increases_speed_drops_to_next_available(int priorSpeed, int currentPower, int newRequiredPower, int expectedSpeed)
    {
        var state = EnginesStateDefaults.Testing with {
            CurrentSpeed = priorSpeed,
            CurrentPower = currentPower,
            RequiredPower = 2,
            SpeedPowerRequirements = new []
            {
                new SpeedPowerRequirement { Speed = 7, PowerNeeded = 5 },
                new SpeedPowerRequirement { Speed = 9, PowerNeeded = 8 }
            }
        };
        var expected = state with { RequiredPower = newRequiredPower, CurrentSpeed = expectedSpeed};
        var payload = new RequiredPowerPayload { ["engines"] = newRequiredPower };

        var result = ClassUnderTest.SetRequiredPower(state, "engines", payload);
        
        Assert.That(result.NewState.Value.CurrentSpeed, Is.EqualTo(expectedSpeed));
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }
        
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void When_setting_disabled(bool newDisabled, bool expectChange)
    {
        var systemName = "test-engines";
        var state = EnginesStateDefaults.Testing with { Disabled = !newDisabled };
        var payload = new DisabledSystemsPayload();
        if (expectChange)
        {
            payload[systemName] = newDisabled;
        }
        
        var result = ClassUnderTest.SetDisabled(state, systemName, payload);

        if (expectChange)
        {
            var expected = state with { Disabled = newDisabled };
            Assert.That(result.NewState.Value, Is.EqualTo(expected));    
        }
        else
        {
            Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));
        }
    }

    [TestCase(0, 10, 300_000, 10_000)]
    [TestCase(0, 10, 300_001, 10_000)]
    [TestCase(0, 10, 150_000, 5_000)]
    [TestCase(2_000, 10, 60_000, 4_000)]
    [TestCase(0, 8, 150_000, 4_000)]
    [TestCase(0, 6, 150_000, 3_000)]
    [TestCase(0, 6, 300_000, 6_000)]
    [TestCase(0, 6, 340_000, 6_800)]
    [TestCase(0, 6, 345_000, 6_900)]
    [TestCase(0, 6, 350_000, 7_000)]
    [TestCase(0, 6, 350_001, 7_000)]
    [TestCase(0, 6, 355_000, 7_000)]
    [TestCase(0, 6, 400_000, 7_000)]
    [TestCase(0, 1, 60_000, 200)]
    [TestCase(0, 1, 300_000, 1_000)]
    [TestCase(0, 1, 360_000, 1_200)]
    [TestCase(0, 1, 600_000, 2_000)]
    [TestCase(0, 1, 660_000, 2_000)]
    [TestCase(0, 2, 60_000, 400)]
    [TestCase(0, 2, 300_000, 2_000)]
    [TestCase(0, 2, 360_000, 2_333)]
    public void When_heating_up(int currentHeat, int currentSpeed, int elapsedMilliseconds, int expectedHeat)
    {
        var state = EnginesStateDefaults.Testing with { CurrentHeat = currentHeat, CurrentSpeed = currentSpeed };
        var payload = new ChronometerPayload {ElapsedMilliseconds = elapsedMilliseconds};

        var result = ClassUnderTest.UpdateHeat(state, payload);
            
        Assert.That(result.NewState.Value.CurrentHeat, Is.EqualTo(expectedHeat));
    }
        
    [TestCase(10_000, 0, 720_000, 0, false)]
    [TestCase(10_000, 0, 720_000, 2_000, true)]
    [TestCase(10_000, 0, 60_000, 9_167, true)]
    [TestCase(10_000, 6, 60_000, 9_667, true)]
    public void When_cooling_down(int currentHeat, int currentSpeed, int elapsedMilliseconds, int expectedHeat, bool isPowered)
    {
        var state = EnginesStateDefaults.Testing with
        {
            CurrentHeat = currentHeat,
            CurrentSpeed = currentSpeed,
            RequiredPower = 1,
            CurrentPower = isPowered ? 1 : 0
        };
        var payload = new ChronometerPayload {ElapsedMilliseconds = elapsedMilliseconds};

        var result = ClassUnderTest.UpdateHeat(state, payload);
            
        Assert.That(result.NewState.Value.CurrentHeat, Is.EqualTo(expectedHeat));
    }

    [Test]
    public void When_calculating_heat_and_there_is_no_change()
    {
        var state = EnginesStateDefaults.Testing with
        {
            CurrentHeat = EnginesStateDefaults.Testing.HeatConfig.PoweredHeat
        };

        var payload = new ChronometerPayload {ElapsedMilliseconds = 1000};

        var result = ClassUnderTest.UpdateHeat(state, payload);
            
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));
    }

    [Test]
    public void When_configuring_engines()
    {
        var payload = new EnginesConfigurationPayload
        {
            HeatConfig = new EngineHeatConfig
            {
                PoweredHeat = 9,
                CruisingHeat = 99,
                MaxHeat = 999,
                MinutesAtMaxSpeed = 9,
                MinutesToCoolDown = 99
            },
            SpeedConfig = new EngineSpeedConfig
            {
                CruisingSpeed = 72,
                MaxSpeed = 103
            },
            SpeedPowerRequirements = new[]
            {
                new SpeedPowerRequirement { Speed = 23, PowerNeeded = 5 },
                new SpeedPowerRequirement { Speed = 62, PowerNeeded = 7 }
            }
        };
        var expected = EnginesStateDefaults.Testing with
        {
            HeatConfig = payload.HeatConfig,
            SpeedConfig = payload.SpeedConfig,
            SpeedPowerRequirements = payload.SpeedPowerRequirements
        };
            
        var result = ClassUnderTest.Configure(EnginesStateDefaults.Testing, payload);
            
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }
    
    [Test]
    public void When_configuring_engines_and_speed_requirement_is_higher()
    {
        var payload = new EnginesConfigurationPayload
        {
            HeatConfig = new EngineHeatConfig
            {
                PoweredHeat = 9,
                CruisingHeat = 99,
                MaxHeat = 999,
                MinutesAtMaxSpeed = 9,
                MinutesToCoolDown = 99
            },
            SpeedConfig = new EngineSpeedConfig
            {
                CruisingSpeed = 72,
                MaxSpeed = 103
            },
            SpeedPowerRequirements = new[]
            {
                new SpeedPowerRequirement { Speed = 6, PowerNeeded = 7 }
            }
        };
        var state = EnginesStateDefaults.Testing with
        {
            CurrentSpeed = 6,
            CurrentPower = 5,
            RequiredPower = 4,
            SpeedPowerRequirements = new[]
            {
                new SpeedPowerRequirement() { Speed = 6, PowerNeeded = 5 }
            }
        };
        var expected = state with
        {
            HeatConfig = payload.HeatConfig,
            SpeedConfig = payload.SpeedConfig,
            SpeedPowerRequirements = payload.SpeedPowerRequirements,
            CurrentSpeed = 5
        };
            
        var result = ClassUnderTest.Configure(state, payload);
            
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }
}