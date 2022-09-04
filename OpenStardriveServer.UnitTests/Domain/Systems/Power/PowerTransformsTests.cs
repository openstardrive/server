using System;
using System.Collections.Generic;
using System.Linq;
using OpenStardriveServer.Domain;
using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Power;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Power;

public class PowerTransformsTests : WithAnAutomocked<PowerTransforms>
{
    [Test]
    public void When_configuring_power()
    {
        var state = new PowerState();
        var payload = new ConfigurePowerPayload
        {
            TargetOutput = 75,
            ReactorDrift = 3,
            NumberOfBatteries = 3,
            MaxBatteryCharge = 100,
            NewBatteryCharge = 80,
            UpdateRateInMilliseconds = 1701
        };
        var expectedBatteries = Enumerable.Repeat(new Battery { Charge = 80, Damaged = false }, 3);

        var result = ClassUnderTest.Configure(state, payload);

        var newState = result.NewState.Value;
        Assert.That(newState.ReactorOutput, Is.EqualTo(payload.TargetOutput));
        Assert.That(newState.Batteries, Is.EqualTo(expectedBatteries));
        Assert.That(newState.Config, Is.EqualTo(new PowerConfiguration
        {
            TargetOutput = payload.TargetOutput,
            ReactorDrift = payload.ReactorDrift,
            NumberOfBatteries = payload.NumberOfBatteries,
            MaxBatteryCharge = payload.MaxBatteryCharge,
            UpdateRateInMilliseconds = payload.UpdateRateInMilliseconds
        }));
        Assert.That(newState.MillisecondsUntilNextUpdate, Is.EqualTo(payload.UpdateRateInMilliseconds));
    }
    
    [Test]
    public void When_configuring_power_and_there_were_already_some_batteries()
    {
        var state = new PowerState
        {
            ReactorOutput = 32,
            Batteries = new []
            {
                new Battery { Charge = 100, Damaged = false },
                new Battery { Charge = 52, Damaged = true }
            },
            Config = new PowerConfiguration
            {
                TargetOutput = 44,
                ReactorDrift = 10,
                NumberOfBatteries = 2,
                MaxBatteryCharge = 101,
                UpdateRateInMilliseconds = 32000
            }
        };
        var payload = new ConfigurePowerPayload
        {
            TargetOutput = 75,
            ReactorDrift = 3,
            NumberOfBatteries = 3,
            MaxBatteryCharge = 100,
            NewBatteryCharge = 80,
            UpdateRateInMilliseconds = 1701
        };
        var expectedBatteries = new []
        {
            new Battery { Charge = 100, Damaged = false },
            new Battery { Charge = 52, Damaged = true },
            new Battery { Charge = 80, Damaged = false }
        };

        var result = ClassUnderTest.Configure(state, payload);

        var newState = result.NewState.Value;
        Assert.That(newState.ReactorOutput, Is.EqualTo(payload.TargetOutput));
        Assert.That(newState.Batteries, Is.EqualTo(expectedBatteries));
        Assert.That(newState.Config, Is.EqualTo(new PowerConfiguration
        {
            TargetOutput = payload.TargetOutput,
            ReactorDrift = payload.ReactorDrift,
            NumberOfBatteries = payload.NumberOfBatteries,
            MaxBatteryCharge = payload.MaxBatteryCharge,
            UpdateRateInMilliseconds = payload.UpdateRateInMilliseconds
        }));
        Assert.That(newState.MillisecondsUntilNextUpdate, Is.EqualTo(payload.UpdateRateInMilliseconds));
    }
    
    [Test]
    public void When_configuring_power_and_batteries_are_removed()
    {
        var state = new PowerState
        {
            ReactorOutput = 32,
            Batteries = new []
            {
                new Battery { Charge = 100, Damaged = false },
                new Battery { Charge = 52, Damaged = true }
            },
            Config = new PowerConfiguration
            {
                TargetOutput = 44,
                ReactorDrift = 10,
                NumberOfBatteries = 2,
                MaxBatteryCharge = 101,
                UpdateRateInMilliseconds = 32000
            }
        };
        var payload = new ConfigurePowerPayload
        {
            TargetOutput = 75,
            ReactorDrift = 3,
            NumberOfBatteries = 1,
            MaxBatteryCharge = 100,
            NewBatteryCharge = 80,
            UpdateRateInMilliseconds = 1701
        };
        var expectedBatteries = new []
        {
            new Battery { Charge = 100, Damaged = false }
        };

        var result = ClassUnderTest.Configure(state, payload);

        var newState = result.NewState.Value;
        Assert.That(newState.ReactorOutput, Is.EqualTo(payload.TargetOutput));
        Assert.That(newState.Batteries, Is.EqualTo(expectedBatteries));
        Assert.That(newState.Config, Is.EqualTo(new PowerConfiguration
        {
            TargetOutput = payload.TargetOutput,
            ReactorDrift = payload.ReactorDrift,
            NumberOfBatteries = payload.NumberOfBatteries,
            MaxBatteryCharge = payload.MaxBatteryCharge,
            UpdateRateInMilliseconds = payload.UpdateRateInMilliseconds
        }));
        Assert.That(newState.MillisecondsUntilNextUpdate, Is.EqualTo(payload.UpdateRateInMilliseconds));
    }

    [Test]
    public void When_the_chronometer_fires_but_it_is_not_yet_time_to_update()
    {
        var state = new PowerState
        {
            MillisecondsUntilNextUpdate = 1001
        };
        var payload = new ChronometerPayload { ElapsedMilliseconds = 1000 };

        var result = ClassUnderTest.UpdatePower(state, payload);

        Assert.That(result.NewState.Value, Is.EqualTo(state with { MillisecondsUntilNextUpdate = 1 }));
    }
    
    [Test]
    public void When_the_chronometer_fires_and_power_used_is_less_than_generated()
    {
        var state = new PowerState
        {
            ReactorOutput = 100,
            Batteries = new []
            {
                new Battery { Charge = 10, Damaged = false },
                new Battery { Charge = 0, Damaged = true },
                new Battery { Charge = 0, Damaged = false },
                new Battery { Charge = 0, Damaged = false },
            },
            MillisecondsUntilNextUpdate = 300,
            Config = new PowerConfiguration
            {
                TargetOutput = 55,
                ReactorDrift = 5,
                MaxBatteryCharge = 20,
                UpdateRateInMilliseconds = 30000
            }
        };
        var payload = new ChronometerPayload { ElapsedMilliseconds = 1000 };
        var systems = new List<IPoweredSystem>
        {
            new TestPoweredSystem { SystemName = "A", CurrentPower = 20 },
            new TestPoweredSystem { SystemName = "B", CurrentPower = 20 }
        };
        GetMock<IRandomNumberGenerator>().Setup(x => x.RandomInt(11)).Returns(4);
        GetMock<ISystemsRegistry>().Setup(x => x.GetAllPoweredSystems()).Returns(systems);

        var result = ClassUnderTest.UpdatePower(state, payload);

        var newState = result.NewState.Value;
        Assert.That(newState.MillisecondsUntilNextUpdate, Is.EqualTo(29300));
        Assert.That(newState.ReactorOutput, Is.EqualTo(54));
        Assert.That(newState.Batteries, Is.EqualTo(new []
        {
            new Battery { Charge = 20, Damaged = false },
            new Battery { Charge = 0, Damaged =  true },
            new Battery { Charge = 4, Damaged =  false },
            new Battery { Charge = 0, Damaged =  false }
        }));
    }
    
    [Test]
    public void When_the_chronometer_fires_and_power_used_exceeds_generated()
    {
        var state = new PowerState
        {
            ReactorOutput = 100,
            Batteries = new []
            {
                new Battery { Charge = 10, Damaged = false },
                new Battery { Charge = 50, Damaged = true },
                new Battery { Charge = 50, Damaged = false },
            },
            MillisecondsUntilNextUpdate = 300,
            Config = new PowerConfiguration
            {
                TargetOutput = 55,
                ReactorDrift = 5,
                MaxBatteryCharge = 100,
                UpdateRateInMilliseconds = 30000
            }
        };
        var payload = new ChronometerPayload { ElapsedMilliseconds = 1000 };
        var systems = new List<IPoweredSystem>
        {
            new TestPoweredSystem { SystemName = "A", CurrentPower = 40 },
            new TestPoweredSystem { SystemName = "B", CurrentPower = 40 }
        };
        GetMock<IRandomNumberGenerator>().Setup(x => x.RandomInt(11)).Returns(8);
        GetMock<ISystemsRegistry>().Setup(x => x.GetAllPoweredSystems()).Returns(systems);

        var result = ClassUnderTest.UpdatePower(state, payload);

        var newState = result.NewState.Value;
        Assert.That(newState.MillisecondsUntilNextUpdate, Is.EqualTo(29300));
        Assert.That(newState.ReactorOutput, Is.EqualTo(58));
        Assert.That(newState.Batteries, Is.EqualTo(new []
        {
            new Battery { Charge = 0, Damaged = false },
            new Battery { Charge = 50, Damaged =  true },
            new Battery { Charge = 38, Damaged =  false }
        }));
    }
    
    [Test]
    public void When_the_chronometer_fires_and_power_used_exceeds_generated_and_battery_power()
    {
        var state = new PowerState
        {
            ReactorOutput = 100,
            Batteries = new []
            {
                new Battery { Charge = 2, Damaged = false },
                new Battery { Charge = 50, Damaged = true },
                new Battery { Charge = 5, Damaged = false },
            },
            MillisecondsUntilNextUpdate = 600,
            Config = new PowerConfiguration
            {
                TargetOutput = 55,
                ReactorDrift = 3,
                MaxBatteryCharge = 100,
                UpdateRateInMilliseconds = 30000
            }
        };
        var payload = new ChronometerPayload { ElapsedMilliseconds = 1000 };
        var systems = new List<IPoweredSystem>
        {
            new TestPoweredSystem { SystemName = "A", CurrentPower = 40 },
            new TestPoweredSystem { SystemName = "B", CurrentPower = 50 },
            new TestPoweredSystem { SystemName = "C", CurrentPower = 45 }
        };
        Command command = null;
        CurrentPowerPayload powerPayload = null;
        var json = "power-json";
        GetMock<IRandomNumberGenerator>().Setup(x => x.RandomInt(7)).Returns(3);
        GetMock<ISystemsRegistry>().Setup(x => x.GetAllPoweredSystems()).Returns(systems);
        GetMock<ICommandRepository>().Setup(x => x.Save(Any<Command>())).Callback<Command>(c => command = c);
        GetMock<IJson>().Setup(x => x.Serialize(Any<object>()))
            .Callback<object>(p => powerPayload = p as CurrentPowerPayload)
            .Returns(json);

        var result = ClassUnderTest.UpdatePower(state, payload);

        var newState = result.NewState.Value;
        Assert.That(newState.MillisecondsUntilNextUpdate, Is.EqualTo(29600));
        Assert.That(newState.ReactorOutput, Is.EqualTo(55));
        Assert.That(newState.Batteries, Is.EqualTo(new []
        {
            new Battery { Charge = 0, Damaged = false },
            new Battery { Charge = 50, Damaged =  true },
            new Battery { Charge = 0, Damaged =  false }
        }));
        
        Assert.That(command, Is.Not.Null);
        Assert.That(command.Type, Is.EqualTo("set-power"));
        Assert.That(command.Payload, Is.EqualTo(json));
        Assert.That(command.ClientId, Is.EqualTo(Guid.Empty));
        Assert.That(powerPayload, Is.Not.Null);
        Assert.That(powerPayload.Count, Is.EqualTo(2));
        Assert.That(powerPayload["B"], Is.EqualTo(0));
        Assert.That(powerPayload["C"], Is.EqualTo(22));
    }

    [Test]
    public void When_updating_battery_damaged_status()
    {
        var state = new PowerState
        {
            Batteries = new []
            {
                new Battery { Charge = 40, Damaged = false }
            }
        };
        var payload = new BatteryDamagePayload
        {
            BatteryIndex = 0,
            IsDamaged = true
        };

        var result = ClassUnderTest.SetBatteryDamage(state, payload);
        
        Assert.That(result.NewState.Value.Batteries, Is.EqualTo(new []
        {
            new Battery { Charge = 40, Damaged = true }
        }));
    }
    
    
    [Test]
    public void When_updating_battery_damaged_status_but_the_index_is_invalid()
    {
        var state = new PowerState
        {
            Batteries = new []
            {
                new Battery { Charge = 40, Damaged = false }
            }
        };
        var payload = new BatteryDamagePayload
        {
            BatteryIndex = 1,
            IsDamaged = true
        };

        var result = ClassUnderTest.SetBatteryDamage(state, payload);
        
        Assert.That(result.ErrorMessage, Is.EqualTo($"Invalid batteryIndex: {payload.BatteryIndex}"));
    }
    
    [Test]
    public void When_updating_battery_charge()
    {
        var state = new PowerState
        {
            Batteries = new []
            {
                new Battery { Charge = 40, Damaged = false }
            },
            Config = new PowerConfiguration
            {
                MaxBatteryCharge = 50
            }
        };
        var payload = new BatteryChargePayload
        {
            BatteryIndex = 0,
            Charge = 45
        };

        var result = ClassUnderTest.SetBatteryCharge(state, payload);
        
        Assert.That(result.NewState.Value.Batteries, Is.EqualTo(new []
        {
            new Battery { Charge = 45, Damaged = false }
        }));
    }
    
    [Test]
    public void When_updating_battery_charge_to_more_than_maximum()
    {
        var state = new PowerState
        {
            Batteries = new []
            {
                new Battery { Charge = 40, Damaged = false }
            },
            Config = new PowerConfiguration
            {
                MaxBatteryCharge = 50
            }
        };
        var payload = new BatteryChargePayload
        {
            BatteryIndex = 0,
            Charge = 51
        };

        var result = ClassUnderTest.SetBatteryCharge(state, payload);
        
        Assert.That(result.ErrorMessage, Is.EqualTo("Batteries may not be charged more than the configured maximum"));
    }
    
    
    [Test]
    public void When_updating_battery_charge_to_less_than_zero()
    {
        var state = new PowerState
        {
            Batteries = new []
            {
                new Battery { Charge = 40, Damaged = false }
            },
            Config = new PowerConfiguration
            {
                MaxBatteryCharge = 50
            }
        };
        var payload = new BatteryChargePayload
        {
            BatteryIndex = 0,
            Charge = -1
        };

        var result = ClassUnderTest.SetBatteryCharge(state, payload);
        
        Assert.That(result.ErrorMessage, Is.EqualTo("Batteries may not be charged less than zero"));
    }

    [Test]
    public void When_updating_battery_charge_but_the_index_is_invalid()
    {
        var state = new PowerState
        {
            Batteries = new []
            {
                new Battery { Charge = 40, Damaged = false }
            }
        };
        var payload = new BatteryChargePayload
        {
            BatteryIndex = 1,
            Charge = 25
        };

        var result = ClassUnderTest.SetBatteryCharge(state, payload);
        
        Assert.That(result.ErrorMessage, Is.EqualTo($"Invalid batteryIndex: {payload.BatteryIndex}"));
    }
}

public class TestPoweredSystem : IPoweredSystem
{
    public string SystemName { get; init; }
    public Dictionary<string, Func<Command, CommandResult>> CommandProcessors { get; init; }

    public int CurrentPower { get; init; }
}