using System;
using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Defense.EnergyBeam;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Defense.EnergyBeam;

public class EnergyBeamTransformsTests : StandardTransformsTest<EnergyBeamTransforms, EnergyBeamState>
{
    [Test]
    public void When_disabling()
    {
        TestStandardDisabled(new EnergyBeamState());
    }

    [TestCase("energy-beams", true, 0)]
    [TestCase("energy-beams", false, 1)]
    [TestCase("other", true, 1)]
    public void When_damaged(string poweredSystem, bool isNowDamaged, double expectedCharge)
    {
        var systemName = "energy-beams";
        var state = new EnergyBeamState
        {
            Damaged = !isNowDamaged,
            Banks = new []
            {
                new EnergyBeamBank { Name = "Bank A", PercentCharged = 1 },
                new EnergyBeamBank { Name = "Bank B", PercentCharged = 1 }
            }
        };
        var payload = new DamagedSystemsPayload
        {
            [poweredSystem] = isNowDamaged
        };

        var result = ClassUnderTest.SetDamaged(state, systemName, payload);

        if (poweredSystem == systemName)
        {
            var newState = result.NewState.Value;
            Assert.That(newState.Damaged, Is.EqualTo(isNowDamaged));
            foreach (var bank in newState.Banks)
            {
                Assert.That(bank.PercentCharged, Is.EqualTo(expectedCharge).Within(.001), $"Incorrect charge on {bank.Name}");
            }
        }
        else
        {
            Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));
        }
    }
    
    [TestCase("energy-beams", 5, 4, 0)]
    [TestCase("energy-beams", 5, 6, 1)]
    [TestCase("other", 5, 4, 1)]
    public void When_setting_current_power(string poweredSystem, int previousPower, int newPower, double expectedCharge)
    {
        var systemName = "energy-beams";
        var state = new EnergyBeamState
        {
            CurrentPower = previousPower,
            RequiredPower = 5,
            Banks = new []
            {
                new EnergyBeamBank { Name = "Bank A", PercentCharged = 1 },
                new EnergyBeamBank { Name = "Bank B", PercentCharged = 1 }
            }
        };
        var payload = new CurrentPowerPayload
        {
            [poweredSystem] = newPower
        };

        var result = ClassUnderTest.SetCurrentPower(state, systemName, payload);

        if (poweredSystem == systemName)
        {
            var newState = result.NewState.Value;
            Assert.That(newState.CurrentPower, Is.EqualTo(newPower));
            foreach (var bank in newState.Banks)
            {
                Assert.That(bank.PercentCharged, Is.EqualTo(expectedCharge).Within(.001), $"Incorrect charge on {bank.Name}");
            }
        }
        else
        {
            Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));
        }
    }
    
    [TestCase("energy-beams", 4, 1)]
    [TestCase("energy-beams", 6, 0)]
    [TestCase("other", 6, 1)]
    public void When_setting_required_power(string poweredSystem, int newPower, double expectedCharge)
    {
        var systemName = "energy-beams";
        var state = new EnergyBeamState
        {
            CurrentPower = 5,
            RequiredPower = 5,
            Banks = new []
            {
                new EnergyBeamBank { Name = "Bank A", PercentCharged = 1 },
                new EnergyBeamBank { Name = "Bank B", PercentCharged = 1 }
            }
        };
        var payload = new RequiredPowerPayload
        {
            [poweredSystem] = newPower
        };

        var result = ClassUnderTest.SetRequiredPower(state, systemName, payload);

        if (poweredSystem == systemName)
        {
            var newState = result.NewState.Value;
            Assert.That(newState.RequiredPower, Is.EqualTo(newPower));
            foreach (var bank in newState.Banks)
            {
                Assert.That(bank.PercentCharged, Is.EqualTo(expectedCharge).Within(.001), $"Incorrect charge on {bank.Name}");
            }
        }
        else
        {
            Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));
        }
    }

    [TestCase(true, false, false, StandardSystemBaseState.DisabledError)]
    [TestCase(false, true, false, StandardSystemBaseState.DamagedError)]
    [TestCase(false, false, true, StandardSystemBaseState.InsufficientPowerError)]
    public void When_setting_charge_but_non_functional(bool isDisabled, bool isDamaged, bool notEnoughPower, string expectedError)
    {
        var state = new EnergyBeamState
        {
            Disabled = isDisabled,
            Damaged = isDamaged,
            CurrentPower = notEnoughPower ? 0 : 1,
            RequiredPower = 1
        };
        var payload = new ChargeEnergyBeamPayload
        {
            BankName = "Aft",
            NewCharge = .9
        };

        var result = ClassUnderTest.SetBankCharge(state, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo(expectedError));
    }

    [TestCase(.1, .1)]
    [TestCase(.9, .9)]
    [TestCase(1, 1)]
    [TestCase(1.2, 1.2)]
    [TestCase(0, 0)]
    [TestCase(-1, 0)]
    [TestCase(-.01, 0)]
    public void When_setting_charge(double newCharge, double expectedCharge)
    {
        var state = new EnergyBeamState();
        var payload = new ChargeEnergyBeamPayload
        {
            BankName = "Aft",
            NewCharge = newCharge
        };

        var result = ClassUnderTest.SetBankCharge(state, payload);

        var newState = result.NewState.Value;
        Assert.That(newState.Banks[0].PercentCharged, Is.EqualTo(0));
        Assert.That(newState.Banks[1].PercentCharged, Is.EqualTo(expectedCharge));
    }

    [Test]
    public void When_setting_charge_on_an_unknown_bank()
    {
        var state = new EnergyBeamState();
        var payload = new ChargeEnergyBeamPayload
        {
            BankName = "Fake One",
            NewCharge = .75
        };
        
        var result = ClassUnderTest.SetBankCharge(state, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo($"Unknown bank: {payload.BankName}"));
    }
    
    [TestCase(true, false, false, StandardSystemBaseState.DisabledError)]
    [TestCase(false, true, false, StandardSystemBaseState.DamagedError)]
    [TestCase(false, false, true, StandardSystemBaseState.InsufficientPowerError)]
    public void When_firing_but_non_functional(bool isDisabled, bool isDamaged, bool notEnoughPower, string expectedError)
    {
        var state = new EnergyBeamState
        {
            Disabled = isDisabled,
            Damaged = isDamaged,
            CurrentPower = notEnoughPower ? 0 : 1,
            RequiredPower = 1
        };
        var payload = new FireEnergyBeamPayload
        {
            BankName = "Aft",
            DischargePercent = 1
        };

        var result = ClassUnderTest.Fire(state, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo(expectedError));
    }
    
    [Test]
    public void When_firing_an_unknown_bank()
    {
        var state = new EnergyBeamState();
        var payload = new FireEnergyBeamPayload
        {
            BankName = "Fake One",
            DischargePercent = .75
        };
        
        var result = ClassUnderTest.Fire(state, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo($"Unknown bank: {payload.BankName}"));
    }

    [TestCase(1, 1, 0, 1)]
    [TestCase(1, 1.2, 0, 1)]
    [TestCase(1.3, 1.3, 0, 1.3)]
    [TestCase(1, .6, .4, .6)]
    [TestCase(.4, .6, 0, .4)]
    [TestCase(.4, .15, .25, .15)]
    public void When_firing(double priorCharge, double dischargePercent, double expectedCharge, double expectedDischarge)
    {
        var state = new EnergyBeamState
        {
            Banks = new []
            {
                new EnergyBeamBank
                {
                    Name = "Primary",
                    PercentCharged = priorCharge,
                    ArcDegrees = 123.456,
                    Frequency = 987.65
                },
                new EnergyBeamBank
                {
                    Name = "Secondary",
                    PercentCharged = priorCharge,
                    ArcDegrees = 123.456,
                    Frequency = 987.65
                }
            }
        };
        var payload = new FireEnergyBeamPayload
        {
            BankName = "Primary",
            DischargePercent = dischargePercent,
            Target = "asteroid"
        };
        var expected = state.Banks[0] with { PercentCharged = expectedCharge };

        var result = ClassUnderTest.Fire(state, payload);

        var newState = result.NewState.Value;
        Assert.That(newState.Banks[0], Is.EqualTo(expected));
        Assert.That(newState.Banks[1], Is.EqualTo(state.Banks[1]));

        var lastFired = newState.LastFiredEnergyBeam;
        Assert.That(lastFired.Name, Is.EqualTo(payload.BankName));
        Assert.That(lastFired.PercentDischarged, Is.EqualTo(expectedDischarge));
        Assert.That(lastFired.FiredAt, Is.EqualTo(DateTimeOffset.UtcNow).Within(TimeSpan.FromSeconds(1)));
        Assert.That(lastFired.Frequency, Is.EqualTo(state.Banks[0].Frequency));
        Assert.That(lastFired.ArcDegrees, Is.EqualTo(state.Banks[0].ArcDegrees));
        Assert.That(lastFired.Target, Is.EqualTo(payload.Target));
    }
    
    [Test]
    public void When_firing_an_uncharged_bank()
    {
        var state = new EnergyBeamState();
        var payload = new FireEnergyBeamPayload
        {
            BankName = "Forward",
            DischargePercent = .75
        };
        
        var result = ClassUnderTest.Fire(state, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo($"bank {payload.BankName} has no charge to fire"));
    }

    [Test]
    public void When_configuring_a_bank()
    {
        var state = new EnergyBeamState();
        var payload = new ConfigureEnergyBeamBankPayload
        {
            BankName = "Forward",
            ArcDegrees = 55.66,
            Frequency = 77.88
        };
        var expected = state.Banks[0] with
        {
            ArcDegrees = payload.ArcDegrees,
            Frequency = payload.Frequency
        };

        var result = ClassUnderTest.ConfigureBank(state, payload);

        var newState = result.NewState.Value;
        Assert.That(newState.Banks[0], Is.EqualTo(expected));
        Assert.That(newState.Banks[1], Is.EqualTo(state.Banks[1]));
    }
    
    [Test]
    public void When_configuring_a_bank_but_there_is_no_match()
    {
        var state = new EnergyBeamState();
        var payload = new ConfigureEnergyBeamBankPayload
        {
            BankName = "Wat",
            ArcDegrees = 55.66,
            Frequency = 77.88
        };

        var result = ClassUnderTest.ConfigureBank(state, payload);

        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo($"Unknown bank: {payload.BankName}"));
    }

    [Test]
    public void When_configuring_banks()
    {
        var state = new EnergyBeamState();
        var payload = new ConfigureAllEnergyBeamBanksPayload
        {
            Banks = new []
            {
                new EnergyBeamBank { Name = "Disruptors", PercentCharged = 1, Frequency = 99.99, ArcDegrees = 5 }
            }
        };
        var expected = state with { Banks = payload.Banks };

        var result = ClassUnderTest.ConfigureAllBanks(state, payload);
        
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }
}