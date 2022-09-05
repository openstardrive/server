namespace OpenStardriveServer.Domain.Systems.Alert;

public interface IAlertTransforms
{
    TransformResult<AlertState> Configure(ConfigureAlertLevelsPayload payload);
    TransformResult<AlertState> SetLevel(AlertState state, SetAlertLevelPayload payload);
}

public class AlertTransforms : IAlertTransforms
{
    public TransformResult<AlertState> Configure(ConfigureAlertLevelsPayload payload)
    {
        return payload.Levels.FirstOrNone(x => x.Level == payload.CurrentLevel).Case(
            some: current => TransformResult<AlertState>.StateChanged(new AlertState
            {
                AllLevels = payload.Levels,
                Current = current
            }),
            none: () => TransformResult<AlertState>.Error($"No alert level was provided for currentLevel: {payload.CurrentLevel}"));
    }

    public TransformResult<AlertState> SetLevel(AlertState state, SetAlertLevelPayload payload)
    {
        return state.AllLevels.FirstOrNone(x => x.Level == payload.Level).Case(
            some: level => TransformResult<AlertState>.StateChanged(state with { Current = level }),
            none: () => TransformResult<AlertState>.Error($"There is no defined alert level {payload.Level}"));
    }
}