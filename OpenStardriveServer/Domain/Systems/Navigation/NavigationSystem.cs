using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Navigation;

public class NavigationSystem : SystemBase<NavigationState>
{
    public NavigationSystem(IJson json, INavigationTransforms transforms) : base(json)
    {
        SystemName = "navigation";
        CommandProcessors = new Dictionary<string, Func<Command, CommandResult>>
        {
            ["report-state"] = c => Update(c, TransformResult<NavigationState>.StateChanged(state)),
            ["set-disabled"] = c => Update(c, transforms.SetDisabled(state, SystemName, Payload<DisabledSystemsPayload>(c))),
            ["set-damaged"] = c => Update(c, transforms.SetDamaged(state, SystemName, Payload<DamagedSystemsPayload>(c))),
            ["set-power"] = c => Update(c, transforms.SetCurrentPower(state, SystemName, Payload<CurrentPowerPayload>(c))),
            ["set-required-power"] = c => Update(c, transforms.SetRequiredPower(state, SystemName, Payload<RequiredPowerPayload>(c))),
            ["request-course-calculation"] = c => Update(c, transforms.RequestCourse(state, Payload<RequestedCourseCalculationPayload>(c))),
            ["cancel-course-calculation"] = c => Update(c, transforms.CancelRequestedCourse(state, Payload<CancelRequestedCourseCalculationPayload>(c))),
            ["course-calculated"] = c => Update(c, transforms.CourseCalculated(state, Payload<CalculatedCoursePayload>(c))),
            ["set-course"] = c => Update(c, transforms.SetCourse(state, Payload<SetCoursePayload>(c))),
            ["update-eta"] = c => Update(c, transforms.UpdateEta(state, Payload<SetEtaPayload>(c))),
            ["clear-eta"] = c => Update(c, transforms.ClearEta(state)),
            [ChronometerCommand.Type] = c => Update(c, transforms.Travel(state, Payload<ChronometerPayload>(c)))
        };
    }
}