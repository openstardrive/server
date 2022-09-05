using System.Linq;
using OpenStardriveServer.Domain.Systems.Alert;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Alert;

public class AlertTransformsTests : WithAnAutomocked<AlertTransforms>
{
    [Test]
    public void When_configuring_alert_levels()
    {
        var payload = new ConfigureAlertLevelsPayload
        {
            Levels = new []
            {
                new AlertLevel { Level = 4, Name = "Blue", Color = "#0000ff" },
                new AlertLevel { Level = 5, Name = "Black", Color = "#000000" },
            },
            CurrentLevel = 5
        };

        var result = ClassUnderTest.Configure(payload);
        
        Assert.That(result.NewState.Value, Is.EqualTo(new AlertState
        {
            AllLevels = payload.Levels,
            Current = payload.Levels[1]
        }));
    }
    
    [Test]
    public void When_configuring_alert_levels_and_current_level_is_not_found()
    {
        var payload = new ConfigureAlertLevelsPayload
        {
            Levels = new []
            {
                new AlertLevel { Level = 4, Name = "Blue", Color = "#0000ff" },
                new AlertLevel { Level = 5, Name = "Black", Color = "#000000" },
            },
            CurrentLevel = 2
        };

        var result = ClassUnderTest.Configure(payload);
        
        Assert.That(result.ErrorMessage, Is.EqualTo("No alert level was provided for currentLevel: 2"));
    }

    [Test]
    public void When_setting_alert_level()
    {
        var state = new AlertState();
        var payload = new SetAlertLevelPayload { Level = 2 };

        var result = ClassUnderTest.SetLevel(state, payload);
        
        Assert.That(result.NewState.Value, Is.EqualTo(state with { Current = state.AllLevels.Single(x => x.Level == payload.Level)}));
    }
    
    [Test]
    public void When_setting_alert_level_to_a_non_matching_value()
    {
        var state = new AlertState();
        var payload = new SetAlertLevelPayload { Level = -1 };

        var result = ClassUnderTest.SetLevel(state, payload);
        
        Assert.That(result.ErrorMessage, Is.EqualTo("There is no defined alert level -1"));
    }
}