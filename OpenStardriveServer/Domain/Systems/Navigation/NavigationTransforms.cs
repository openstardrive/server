using System;
using System.Linq;
using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems.Propulsion.Engines;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Navigation;

public interface INavigationTransforms : IStandardTransforms<NavigationState>
{
    TransformResult<NavigationState> RequestCourse(NavigationState state, RequestedCourseCalculationPayload payload);
    TransformResult<NavigationState> CancelRequestedCourse(NavigationState state, CancelRequestedCourseCalculationPayload payload);
    TransformResult<NavigationState> CourseCalculated(NavigationState state, CalculatedCoursePayload payload);
    TransformResult<NavigationState> SetCourse(NavigationState state, SetCoursePayload payload);
    TransformResult<NavigationState> ClearCourse(NavigationState state);
    TransformResult<NavigationState> UpdateEta(NavigationState state, SetEtaPayload payload);
    TransformResult<NavigationState> ClearEta(NavigationState state);
    TransformResult<NavigationState> Travel(NavigationState state, ChronometerPayload payload);
}

public class NavigationTransforms : INavigationTransforms
{
    private readonly IStandardTransforms<NavigationState> standardTransforms;
    private readonly ISystemsRegistry systemsRegistry;

    public NavigationTransforms(IStandardTransforms<NavigationState> standardTransforms, ISystemsRegistry systemsRegistry)
    {
        this.standardTransforms = standardTransforms;
        this.systemsRegistry = systemsRegistry;
    }
    
    public TransformResult<NavigationState> SetDisabled(NavigationState state, string systemName, DisabledSystemsPayload payload)
    {
        return standardTransforms.SetDisabled(state, systemName, payload);
    }

    public TransformResult<NavigationState> SetDamaged(NavigationState state, string systemName, DamagedSystemsPayload payload)
    {
        return standardTransforms.SetDamaged(state, systemName, payload);
    }

    public TransformResult<NavigationState> SetCurrentPower(NavigationState state, string systemName, CurrentPowerPayload payload)
    {
        return standardTransforms.SetCurrentPower(state, systemName, payload);
    }

    public TransformResult<NavigationState> SetRequiredPower(NavigationState state, string systemName, RequiredPowerPayload payload)
    {
        return standardTransforms.SetRequiredPower(state, systemName, payload);
    }

    public TransformResult<NavigationState> RequestCourse(NavigationState state, RequestedCourseCalculationPayload payload)
    {
        return state.IfFunctional(() =>
        {
            var newRequested = new RequestedCourseCalculation
            {
                CourseId = payload.CourseId,
                Destination = payload.Destination
            };
            return state.RequestedCourseCalculations.FirstOrNone(x => x.CourseId == payload.CourseId).Case(
                some: _ => TransformResult<NavigationState>.Error($"There is already a requested course calculation with courseId: {payload.CourseId}"),
                none: () => TransformResult<NavigationState>.StateChanged(state with
                {
                    RequestedCourseCalculations = state.RequestedCourseCalculations.Append(newRequested).ToArray()
                }));
        });
    }

    public TransformResult<NavigationState> CancelRequestedCourse(NavigationState state, CancelRequestedCourseCalculationPayload payload)
    {
        return TransformResult<NavigationState>.StateChanged(state with
        {
            RequestedCourseCalculations = state.RequestedCourseCalculations.Where(x => x.CourseId != payload.CourseId).ToArray()
        });
    }

    public TransformResult<NavigationState> CourseCalculated(NavigationState state, CalculatedCoursePayload payload)
    {
        var maybeEngines = Maybe<IEnginesSystem>.None;
        if (payload.Eta != null)
        {
            maybeEngines = systemsRegistry.GetSystemByNameAs<IEnginesSystem>(payload.Eta.EngineSystem);
            if (!maybeEngines.HasValue)
            {
                return TransformResult<NavigationState>.Error($"Unable to locate the engine system: {payload.Eta.EngineSystem}");
            }
        }

        return TransformResult<NavigationState>.StateChanged(state with
        {
            RequestedCourseCalculations = state.RequestedCourseCalculations.Where(x => x.CourseId != payload.CourseId).ToArray(),
            CalculatedCourses = state.CalculatedCourses
                .Where(x => x.CourseId != payload.CourseId)
                .Append(new CalculatedCourse
                {
                    CourseId = payload.CourseId,
                    Destination = payload.Destination,
                    Coordinates = payload.Coordinates,
                    CalculatedAt = DateTimeOffset.UtcNow,
                    Eta = maybeEngines.Case(engines => CalculateEta(payload.Eta, engines), () => null)
                })
                .ToArray()
        });
    }

    private Eta CalculateEta(SetEtaPayload payload, IEnginesSystem engines)
    {
        if (payload is null)
        {
            return null;
        }
        
        var millisecondsAtSpeed1 = payload.ArriveInMilliseconds * payload.Speed;
        
        return new Eta
        {
            EngineSystem = payload.EngineSystem,
            TravelTimes = Enumerable.Range(1, engines.MaxSpeed).Select(x => new TravelTime
            {
                Speed = x,
                ArriveInMilliseconds = Math.Max(0, millisecondsAtSpeed1 / x)
            }).ToArray()
        };
    }

    public TransformResult<NavigationState> SetCourse(NavigationState state, SetCoursePayload payload)
    {
        return state.IfFunctional(() =>
        {
            var maybeEngines = Maybe<IEnginesSystem>.None;
            if (payload.Eta != null)
            {
                maybeEngines = systemsRegistry.GetSystemByNameAs<IEnginesSystem>(payload.Eta.EngineSystem);
                if (!maybeEngines.HasValue)
                {
                    return TransformResult<NavigationState>.Error($"Unable to locate the engine system: {payload.Eta.EngineSystem}");
                }
            }
        
            return TransformResult<NavigationState>.StateChanged(state with
            {
                CurrentCourse = new CurrentCourse
                {
                    CourseId = payload.CourseId,
                    Destination = payload.Destination,
                    Coordinates = payload.Coordinates,
                    CourseSetAt = DateTimeOffset.UtcNow,
                    Eta = maybeEngines.Case(engines => CalculateEta(payload.Eta, engines), () => null)
                }
            });
        });
    }

    public TransformResult<NavigationState> ClearCourse(NavigationState state)
    {
        return TransformResult<NavigationState>.StateChanged(state with { CurrentCourse = null });
    }

    public TransformResult<NavigationState> UpdateEta(NavigationState state, SetEtaPayload payload)
    {
        if (state.CurrentCourse == null)
        {
            return TransformResult<NavigationState>.Error("No current course for setting the ETA");
        }
        
        return systemsRegistry.GetSystemByNameAs<IEnginesSystem>(payload.EngineSystem).Case(
            some: engines => TransformResult<NavigationState>.StateChanged(state with
            {
                CurrentCourse = state.CurrentCourse with
                {
                    Eta = CalculateEta(payload, engines)
                }
            }),
            none: () => TransformResult<NavigationState>.Error($"Unable to locate the engine system: {payload.EngineSystem}"));
    }

    public TransformResult<NavigationState> ClearEta(NavigationState state)
    {
        if (state.CurrentCourse == null)
        {
            return TransformResult<NavigationState>.Error("No current course for clearing the ETA");
        }
        
        return TransformResult<NavigationState>.StateChanged(state with
        {
            CurrentCourse = state.CurrentCourse with
            {
                Eta = null
            }
        });
    }

    public TransformResult<NavigationState> Travel(NavigationState state, ChronometerPayload payload)
    {
        if (state.CurrentCourse?.Eta is null)
        {
            return TransformResult<NavigationState>.NoChange();
        }
        
        var travelTime = state.CurrentCourse.Eta.TravelTimes[0];
        if (travelTime.ArriveInMilliseconds <= 0)
        {
            return TransformResult<NavigationState>.NoChange();
        }

        return systemsRegistry.GetSystemByNameAs<IEnginesSystem>(state.CurrentCourse.Eta.EngineSystem).Case(
            some: engines =>
            {
                if (engines.CurrentSpeed <= 0)
                {
                    return TransformResult<NavigationState>.NoChange();                   
                }
                
                var remaining = travelTime.ArriveInMilliseconds - (payload.ElapsedMilliseconds * engines.CurrentSpeed);
                var newEta = new SetEtaPayload
                {
                    EngineSystem = state.CurrentCourse.Eta.EngineSystem,
                    Speed = 1,
                    ArriveInMilliseconds = (int)remaining
                };

                return TransformResult<NavigationState>.StateChanged(state with
                {
                    CurrentCourse = state.CurrentCourse with
                    {
                        Eta = CalculateEta(newEta, engines)
                    }
                });
            },
            none: TransformResult<NavigationState>.NoChange);
    }
}