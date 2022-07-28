using System;
using System.Linq;
using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Defense.WarheadLauncher;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Defense.WarheadLauncher;

public class WarheadLauncherTransformsTests : StandardTransformsTest<WarheadLauncherTransforms, WarheadLauncherState>
{
    private readonly WarheadLauncherState testingState = new()
    {
        Inventory = new []
        {
            new WarheadGroup { Kind = "torpedo", Number = 10 },
            new WarheadGroup { Kind = "missile", Number = 3 }
        }
    };
    
    [Test]
    public void When_loading_a_warhead()
    {
        var payload = new LoadWarheadPayload { Kind = "torpedo" };
        var expected = testingState with
        {
            Loaded = new []{ "torpedo" },
            Inventory = new[]
            {
                new WarheadGroup { Kind = "torpedo", Number = 9 },
                new WarheadGroup { Kind = "missile", Number = 3 }
            }
        };
        
        var result = ClassUnderTest.Load(testingState, payload);
        
        Assert.That(result.NewState.Value.Loaded, Is.EqualTo(expected.Loaded));
        Assert.That(result.NewState.Value.Inventory, Is.EqualTo(expected.Inventory));
    }
    
    [Test]
    public void When_loading_multiple_warheads()
    {
        var payload = new LoadWarheadPayload { Kind = "missile" };
        var state = testingState with
        {
            NumberOfLaunchers = 3,
            Loaded = new []{ "torpedo", "missile" },
            Inventory = new[]
            {
                new WarheadGroup { Kind = "torpedo", Number = 9 },
                new WarheadGroup { Kind = "missile", Number = 3 }
            }
        };
        var expected = state with
        {
            Loaded = new []{ "torpedo", "missile", "missile" },
            Inventory = new[]
            {
                new WarheadGroup { Kind = "torpedo", Number = 9 },
                new WarheadGroup { Kind = "missile", Number = 2 }
            }
        };
        
        var result = ClassUnderTest.Load(state, payload);
        
        Assert.That(result.NewState.Value.Loaded, Is.EqualTo(expected.Loaded));
        Assert.That(result.NewState.Value.Inventory, Is.EqualTo(expected.Inventory));
    }

    [Test]
    public void When_loading_a_warhead_but_there_are_none_left()
    {
        var payload = new LoadWarheadPayload { Kind = "torpedo" };
        var expected = testingState with
        {
            Inventory = new[]
            {
                new WarheadGroup { Kind = "torpedo", Number = 0 },
                new WarheadGroup { Kind = "missile", Number = 3 }
            }
        };
        
        var result = ClassUnderTest.Load(expected, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo("no torpedo available to load"));
    }
    
    [Test]
    public void When_loading_a_warhead_but_that_type_is_absent()
    {
        var payload = new LoadWarheadPayload { Kind = "mine" };

        var result = ClassUnderTest.Load(testingState, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo("no mine available to load"));
    }
    
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public void When_loading_a_warhead_but_there_is_no_free_launcher(int numberOfLaunchers)
    {
        var payload = new LoadWarheadPayload { Kind = "torpedo" };
        var state = testingState with
        {
            NumberOfLaunchers = numberOfLaunchers,
            Loaded = Enumerable.Repeat("torpedo", numberOfLaunchers).ToArray()
        };

        var result = ClassUnderTest.Load(state, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo("no launcher available to load into"));
    }
    
    [TestCase(true, false, 5, 5, StandardSystemBaseState.DamagedError)]
    [TestCase(false, true, 5, 5, StandardSystemBaseState.DisabledError)]
    [TestCase(false, false, 4, 5, StandardSystemBaseState.InsufficientPowerError)]
    public void When_loading_a_warhead_but_system_nonfunctional(bool damaged, bool disabled, int currentPower, int requiredPower, string expected)
    {
        var payload = new LoadWarheadPayload { Kind = "torpedo" };
        var state = testingState with
        {
            Damaged = damaged,
            Disabled = disabled,
            CurrentPower = currentPower,
            RequiredPower = requiredPower
        };

        var result = ClassUnderTest.Load(state, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo(expected));
    }
    
    [TestCase(true, false, 5, 5, StandardSystemBaseState.DamagedError)]
    [TestCase(false, true, 5, 5, StandardSystemBaseState.DisabledError)]
    [TestCase(false, false, 4, 5, StandardSystemBaseState.InsufficientPowerError)]
    public void When_firing_a_warhead_but_system_nonfunctional(bool damaged, bool disabled, int currentPower, int requiredPower, string expected)
    {
        var payload = new FireWarheadPayload { Kind = "torpedo" };
        var state = testingState with
        {
            Damaged = damaged,
            Disabled = disabled,
            CurrentPower = currentPower,
            RequiredPower = requiredPower
        };

        var result = ClassUnderTest.Fire(state, payload, DateTimeOffset.UtcNow);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo(expected));
    }
    
    [Test]
    public void When_firing_a_warhead_but_nothing_loaded()
    {
        var payload = new FireWarheadPayload { Kind = "missile" };
        
        var result = ClassUnderTest.Fire(testingState, payload, DateTimeOffset.UtcNow);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo("no missile loaded to fire"));
    }
    
    [Test]
    public void When_firing_a_warhead_but_none_of_that_type_loaded()
    {
        var payload = new FireWarheadPayload { Kind = "missile" };
        var state = testingState with
        {
            Loaded = new[] { "torpedo" }
        };
        
        var result = ClassUnderTest.Fire(state, payload, DateTimeOffset.UtcNow);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo("no missile loaded to fire"));
    }
    
    [Test]
    public void When_firing_a_warhead()
    {
        var commandTimestamp = DateTimeOffset.UtcNow.AddSeconds(-2);
        var payload = new FireWarheadPayload { Kind = "torpedo", Target = "asteroid" };
        var state = testingState with
        {
            Loaded = new[] { "torpedo" }
        };
        var expected = state with
        {
            Loaded = Array.Empty<string>(),
            LastFiredWarhead = new FiredWarhead
            {
                Kind = payload.Kind,
                FiredAt = commandTimestamp,
                Target = payload.Target
            },
        };
        
        var result = ClassUnderTest.Fire(state, payload, commandTimestamp);
        
        Assert.That(result.NewState.Value.Loaded, Is.EqualTo(expected.Loaded));
        Assert.That(result.NewState.Value.LastFiredWarhead, Is.EqualTo(expected.LastFiredWarhead));
    }

    [Test]
    public void When_firing_a_warhead_and_multiple_were_loaded()
    {
        var commandTimestamp = DateTimeOffset.UtcNow.AddSeconds(-2);
        var payload = new FireWarheadPayload { Kind = "torpedo", Target = "asteroid" };
        var state = testingState with
        {
            Loaded = new[] { "torpedo", "torpedo" }
        };
        var expected = state with
        {
            Loaded = new[] { "torpedo" },
            LastFiredWarhead = new FiredWarhead
            {
                Kind = payload.Kind,
                FiredAt = commandTimestamp,
                Target = payload.Target
            },
        };
        
        var result = ClassUnderTest.Fire(state, payload, commandTimestamp);
        
        Assert.That(result.NewState.Value.Loaded, Is.EqualTo(expected.Loaded));
        Assert.That(result.NewState.Value.LastFiredWarhead, Is.EqualTo(expected.LastFiredWarhead));
    }

    [Test]
    public void When_firing_a_warhead_and_a_variety_were_loaded()
    {
        var commandTimestamp = DateTimeOffset.UtcNow.AddSeconds(-2);
        var payload = new FireWarheadPayload { Kind = "torpedo", Target = "asteroid" };
        var state = testingState with
        {
            Loaded = new[] { "missile", "torpedo", "missile", "torpedo" }
        };
        var expected = state with
        {
            Loaded = new[] { "missile", "missile", "torpedo" },
            LastFiredWarhead = new FiredWarhead
            {
                Kind = payload.Kind,
                FiredAt = commandTimestamp,
                Target = payload.Target
            }
        };
        
        var result = ClassUnderTest.Fire(state, payload, commandTimestamp);
        
        Assert.That(result.NewState.Value.Loaded, Is.EqualTo(expected.Loaded));
        Assert.That(result.NewState.Value.LastFiredWarhead, Is.EqualTo(expected.LastFiredWarhead));
    }
    
    

    [Test]
    public void When_firing_a_subsequent_warhead()
    {
        var commandTimestamp = DateTimeOffset.UtcNow.AddSeconds(-2);
        var payload = new FireWarheadPayload { Kind = "torpedo", Target = "asteroid" };
        var state = testingState with
        {
            Loaded = new[] { "torpedo" },
            LastFiredWarhead = new FiredWarhead
            {
                Kind = "missile",
                FiredAt = DateTimeOffset.UtcNow.AddMinutes(-5),
                Target = "none"
            }
        };
        var expected = state with
        {
            Loaded = Array.Empty<string>(),
            LastFiredWarhead = new FiredWarhead
            {
                Kind = payload.Kind,
                FiredAt = commandTimestamp,
                Target = payload.Target
            }
        };
        
        var result = ClassUnderTest.Fire(state, payload, commandTimestamp);
        
        Assert.That(result.NewState.Value.Loaded, Is.EqualTo(expected.Loaded));
        Assert.That(result.NewState.Value.LastFiredWarhead, Is.EqualTo(expected.LastFiredWarhead));
    }
    
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(4)]
    public void When_setting_power(int newPower)
    {
        var systemName = "Warhead Launcher";
        var state = testingState with { CurrentPower = 2 };
        var payload = new CurrentPowerPayload
        {
            ["other"] = 11,
            [systemName] = newPower
        };
        var expected = state with { CurrentPower = newPower };
        
        var result = ClassUnderTest.SetCurrentPower(state, systemName, payload);
        
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }
    
    [Test]
    public void When_setting_power_but_there_is_no_match()
    {
        var systemName = "Warhead Launcher";
        var state = testingState with { CurrentPower = 2 };
        var payload = new CurrentPowerPayload { ["other"] = 22 };
        
        var result = ClassUnderTest.SetCurrentPower(state, systemName, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));
    }
    
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(4)]
    public void When_setting_required_power(int newPower)
    {
        var systemName = "Warhead Launcher";
        var state = testingState with { CurrentPower = 2 };
        var payload = new RequiredPowerPayload
        {
            ["other"] = 11,
            [systemName] = newPower
        };
        var expected = state with { RequiredPower = newPower };
        
        var result = ClassUnderTest.SetRequiredPower(state, systemName, payload);
        
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }
    
    [Test]
    public void When_setting_required_power_but_there_is_no_match()
    {
        var systemName = "Warhead Launcher";
        var state = testingState with { CurrentPower = 2 };
        var payload = new RequiredPowerPayload { ["other"] = 22 };
        
        var result = ClassUnderTest.SetRequiredPower(state, systemName, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));
    }

    [Test]
    public void When_setting_damaged()
    {
        TestStandardDamaged(new WarheadLauncherState());
    }
    
    [Test]
    public void When_setting_disabled()
    {
        TestStandardDisabled(new WarheadLauncherState());
    }

    [Test]
    public void When_setting_inventory()
    {
        var payload = new WarheadInventoryPayload
        {
            Inventory = new[]
            {
                new WarheadGroup { Kind = "torpedo", Number = 20 },
                new WarheadGroup { Kind = "mine", Number = 9 }
            }
        };
        var expected = testingState with { Inventory = payload.Inventory };

        var result = ClassUnderTest.SetInventory(testingState, payload);
        
        Assert.That(result.NewState.Value, Is.EqualTo(expected));
    }
}