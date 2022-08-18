using System;
using System.Linq;
using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Navigation;
using OpenStardriveServer.Domain.Systems.Propulsion.Engines;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Navigation;

public class NavigationTransformsTests : StandardTransformsTest<NavigationTransforms, NavigationState>
{
    [Test]
    public void When_disabling()
    {
        TestStandardDisabled(new NavigationState());
    }
    
    [Test]
    public void When_damaged()
    {
        TestStandardDamaged(new NavigationState());
    }
    
    [Test]
    public void When_setting_current_power()
    {
        TestStandardCurrentPower(new NavigationState());
    }
    
    [Test]
    public void When_setting_required_power()
    {
        TestStandardRequiredPower(new NavigationState());
    }

    [Test]
    public void When_requesting_a_course_calculation()
    {
        var state = new NavigationState();
        var payload = new RequestedCourseCalculationPayload
        {
            CourseId = RandomString(),
            Destination = "Earth"
        };

        var result = ClassUnderTest.RequestCourse(state, payload);

        var requested = result.NewState.Value.RequestedCourseCalculations;
        Assert.That(requested.Length, Is.EqualTo(1));
        Assert.That(requested[0].CourseId, Is.EqualTo(payload.CourseId));
        Assert.That(requested[0].Destination, Is.EqualTo(payload.Destination));
        Assert.That(requested[0].RequestedAt, Is.EqualTo(DateTimeOffset.UtcNow).Within(TimeSpan.FromSeconds(1)));
    }
    
    [Test]
    public void When_requesting_a_course_calculation_and_there_is_another()
    {
        var state = new NavigationState
        {
            RequestedCourseCalculations = new []
            {
                new RequestedCourseCalculation { CourseId = RandomString(), Destination = "Moon", RequestedAt = DateTimeOffset.UtcNow.AddSeconds(-5) }
            }
        };
        var payload = new RequestedCourseCalculationPayload
        {
            CourseId = RandomString(),
            Destination = "Earth"
        };

        var result = ClassUnderTest.RequestCourse(state, payload);

        var requested = result.NewState.Value.RequestedCourseCalculations;
        Assert.That(requested.Length, Is.EqualTo(2));
        var expected = new[]
        {
            state.RequestedCourseCalculations[0],
            new RequestedCourseCalculation
            {
                CourseId = payload.CourseId,
                Destination = payload.Destination,
                RequestedAt = requested[1].RequestedAt
            }
        };
        Assert.That(requested, Is.EqualTo(expected));
    }
    
    [Test]
    public void When_requesting_a_course_calculation_and_there_is_another_with_the_same_course_id()
    {
        var courseId = RandomString();
        var state = new NavigationState
        {
            RequestedCourseCalculations = new []
            {
                new RequestedCourseCalculation { CourseId = courseId, Destination = "Moon", RequestedAt = DateTimeOffset.UtcNow.AddSeconds(-5) }
            }
        };
        var payload = new RequestedCourseCalculationPayload
        {
            CourseId = courseId,
            Destination = "Earth"
        };

        var result = ClassUnderTest.RequestCourse(state, payload);

        Assert.That(result.ErrorMessage, Is.EqualTo($"There is already a requested course calculation with courseId: {courseId}"));
    }
    
    [TestCase(true, false, false, StandardSystemBaseState.DisabledError)]
    [TestCase(false, true, false, StandardSystemBaseState.DamagedError)]
    [TestCase(false, false, true, StandardSystemBaseState.InsufficientPowerError)]
    public void When_requesting_a_course_calculation_but_inoperable(bool isDisabled, bool isDamaged, bool hasInsufficientPower, string expectedError)
    {
        var state = new NavigationState
        {
            Disabled = isDisabled,
            Damaged = isDamaged,
            RequiredPower = 5,
            CurrentPower = hasInsufficientPower ? 0 : 5
        };
        var payload = new RequestedCourseCalculationPayload
        {
            CourseId = RandomString(),
            Destination = "Earth"
        };

        var result = ClassUnderTest.RequestCourse(state, payload);
        
        Assert.That(result.ErrorMessage, Is.EqualTo(expectedError));
    }
    
    [Test]
    public void When_cancelling_a_course_calculation()
    {
        var courseId = RandomString();
        var state = new NavigationState
        {
            RequestedCourseCalculations = new []
            {
                new RequestedCourseCalculation { CourseId = RandomString() },
                new RequestedCourseCalculation { CourseId = courseId, Destination = "Moon", RequestedAt = DateTimeOffset.UtcNow.AddSeconds(-5) },
                new RequestedCourseCalculation { CourseId = RandomString() },
            }
        };
        var payload = new CancelRequestedCourseCalculationPayload
        {
            CourseId = courseId
        };
        var expected = new[] { state.RequestedCourseCalculations[0], state.RequestedCourseCalculations[2] };

        var result = ClassUnderTest.CancelRequestedCourse(state, payload);

        Assert.That(result.NewState.Value.RequestedCourseCalculations, Is.EqualTo(expected));
    }

    [Test]
    public void When_setting_a_calculated_course_without_an_eta()
    {
        var state = new NavigationState();
        var payload = new CalculatedCoursePayload
        {
            CourseId = RandomString(),
            Destination = "Mars",
            Coordinates = new Coordinates { X = 22.0, Y = 33.3, Z = 44.1 }
        };

        var result = ClassUnderTest.CourseCalculated(state, payload);

        var calculated = result.NewState.Value.CalculatedCourses;
        Assert.That(calculated.Length, Is.EqualTo(1));
        Assert.That(calculated[0].CourseId, Is.EqualTo(payload.CourseId));
        Assert.That(calculated[0].Destination, Is.EqualTo(payload.Destination));
        Assert.That(calculated[0].Coordinates, Is.EqualTo(payload.Coordinates));
        Assert.That(calculated[0].Eta, Is.Null);
        Assert.That(calculated[0].CalculatedAt, Is.EqualTo(DateTimeOffset.UtcNow).Within(TimeSpan.FromSeconds(1)));
    }
    
    [TestCase(1, 100, new [] { 100, 50, 33, 25, 20, 16, 14, 12, 11, 10 })]
    [TestCase(6, 100, new [] { 600, 300, 200, 150, 120, 100, 85, 75, 66, 60 })]
    [TestCase(6, 10000, new [] { 60000, 30000, 20000, 15000, 12000, 10000, 8571, 7500, 6666, 6000 })]
    public void When_setting_a_calculated_course_with_an_eta(int speed, int milliseconds, int[] expectedMilliseconds)
    {
        var state = new NavigationState();
        var payload = new CalculatedCoursePayload
        {
            CourseId = RandomString(),
            Destination = "Jupiter",
            Coordinates = new Coordinates { X = 22.0, Y = 33.3, Z = 44.1 },
            Eta = new SetEtaPayload
            {
                EngineSystem = "ftl-engines",
                Speed = speed,
                ArriveInMilliseconds = milliseconds
            }
        };
        SetupMockEngines(0, 10, payload.Eta.EngineSystem);
        
        var result = ClassUnderTest.CourseCalculated(state, payload);

        var calculated = result.NewState.Value.CalculatedCourses;
        var expectedTravelTimes = expectedMilliseconds.Select((x, i) => new TravelTime { Speed = i + 1, ArriveInMilliseconds = x});
        Assert.That(calculated.Length, Is.EqualTo(1));
        Assert.That(calculated[0].CourseId, Is.EqualTo(payload.CourseId));
        Assert.That(calculated[0].Destination, Is.EqualTo(payload.Destination));
        Assert.That(calculated[0].Coordinates, Is.EqualTo(payload.Coordinates));
        Assert.That(calculated[0].Eta.EngineSystem, Is.EqualTo(payload.Eta.EngineSystem));
        Assert.That(calculated[0].Eta.TravelTimes, Is.EqualTo(expectedTravelTimes));
        Assert.That(calculated[0].CalculatedAt, Is.EqualTo(DateTimeOffset.UtcNow).Within(TimeSpan.FromSeconds(1)));
    }
    
    [Test]
    public void When_setting_a_calculated_course_with_an_eta_but_the_system_is_not_found()
    {
        var state = new NavigationState();
        var payload = new CalculatedCoursePayload
        {
            CourseId = RandomString(),
            Destination = "Jupiter",
            Coordinates = new Coordinates { X = 22.0, Y = 33.3, Z = 44.1 },
            Eta = new SetEtaPayload
            {
                EngineSystem = "ftl-engines",
                Speed = 5,
                ArriveInMilliseconds = 350000
            }
        };
        SetupNoMockEngines();

        var result = ClassUnderTest.CourseCalculated(state, payload);

        Assert.That(result.ErrorMessage, Is.EqualTo($"Unable to locate the engine system: {payload.Eta.EngineSystem}"));
    }
    
    [Test]
    public void When_setting_a_calculated_course_and_there_is_a_matching_request()
    {
        var courseId = RandomString();
        var state = new NavigationState
        {
            RequestedCourseCalculations = new []
            {
                new RequestedCourseCalculation { CourseId = RandomString() },
                new RequestedCourseCalculation { CourseId = courseId },
                new RequestedCourseCalculation { CourseId = RandomString() }
            }
        };
        var payload = new CalculatedCoursePayload
        {
            CourseId = courseId,
            Destination = "Saturn",
            Coordinates = new Coordinates { X = 22.0, Y = 33.3, Z = 44.1 }
        };
        var expected = new[] { state.RequestedCourseCalculations[0], state.RequestedCourseCalculations[2] };

        var result = ClassUnderTest.CourseCalculated(state, payload);

        Assert.That(result.NewState.Value.RequestedCourseCalculations, Is.EqualTo(expected));
    }
    
    [Test]
    public void When_setting_a_calculated_course_and_there_is_already_another_course_with_that_id()
    {
        var courseId = RandomString();
        var state = new NavigationState
        {
            CalculatedCourses = new []
            {
                new CalculatedCourse { CourseId = RandomString() },
                new CalculatedCourse { CourseId = courseId },
                new CalculatedCourse { CourseId = RandomString() }
            }
        };
        var payload = new CalculatedCoursePayload
        {
            CourseId = courseId,
            Destination = "Mars",
            Coordinates = new Coordinates { X = 22.0, Y = 33.3, Z = 44.1 }
        };

        var result = ClassUnderTest.CourseCalculated(state, payload);

        var calculated = result.NewState.Value.CalculatedCourses;
        Assert.That(calculated.Length, Is.EqualTo(3));
        Assert.That(calculated[0], Is.SameAs(state.CalculatedCourses[0]));
        Assert.That(calculated[1], Is.SameAs(state.CalculatedCourses[2]));
        Assert.That(calculated[2].CourseId, Is.EqualTo(payload.CourseId));
        Assert.That(calculated[2].Destination, Is.EqualTo(payload.Destination));
        Assert.That(calculated[2].Coordinates, Is.EqualTo(payload.Coordinates));
        Assert.That(calculated[2].Eta, Is.Null);
        Assert.That(calculated[2].CalculatedAt, Is.EqualTo(DateTimeOffset.UtcNow).Within(TimeSpan.FromSeconds(1)));
    }

    [Test]
    public void When_setting_current_course()
    {
        var state = new NavigationState();
        var payload = new SetCoursePayload
        {
            CourseId = RandomString(),
            Destination = "Alpha Centauri",
            Coordinates = new Coordinates { X = 123.45, Y = 234.56, Z = 345.67 },
        };

        var result = ClassUnderTest.SetCourse(state, payload);

        var currentCourse = result.NewState.Value.CurrentCourse;
        Assert.That(currentCourse.CourseId, Is.EqualTo(payload.CourseId));
        Assert.That(currentCourse.Destination, Is.EqualTo(payload.Destination));
        Assert.That(currentCourse.Coordinates, Is.EqualTo(payload.Coordinates));
        Assert.That(currentCourse.Eta, Is.Null);
        Assert.That(currentCourse.CourseSetAt, Is.EqualTo(DateTimeOffset.UtcNow).Within(TimeSpan.FromSeconds(1)));
    }
    
    [TestCase(1, 100, new [] { 100, 50, 33, 25, 20, 16, 14, 12, 11, 10 })]
    [TestCase(6, 100, new [] { 600, 300, 200, 150, 120, 100, 85, 75, 66, 60 })]
    [TestCase(6, 10000, new [] { 60000, 30000, 20000, 15000, 12000, 10000, 8571, 7500, 6666, 6000 })]
    public void When_setting_current_course_with_an_eta(int speed, int milliseconds, int[] expectedMilliseconds)
    {
        var state = new NavigationState();
        var payload = new SetCoursePayload
        {
            CourseId = RandomString(),
            Destination = "Alpha Centauri",
            Coordinates = new Coordinates { X = 123.45, Y = 234.56, Z = 345.67 },
            Eta = new SetEtaPayload
            {
                EngineSystem = "sublight-engines",
                Speed = speed,
                ArriveInMilliseconds = milliseconds
            }
        };
        var expectedTravelTimes = expectedMilliseconds.Select((x, i) => new TravelTime
        {
            Speed = i + 1,
            ArriveInMilliseconds = x
        });
        SetupMockEngines(0, 10, payload.Eta.EngineSystem);

        var result = ClassUnderTest.SetCourse(state, payload);

        var currentCourse = result.NewState.Value.CurrentCourse;
        Assert.That(currentCourse.CourseId, Is.EqualTo(payload.CourseId));
        Assert.That(currentCourse.Destination, Is.EqualTo(payload.Destination));
        Assert.That(currentCourse.Coordinates, Is.EqualTo(payload.Coordinates));
        Assert.That(currentCourse.Eta.EngineSystem, Is.EqualTo(payload.Eta.EngineSystem));
        Assert.That(currentCourse.Eta.TravelTimes, Is.EqualTo(expectedTravelTimes));
        Assert.That(currentCourse.CourseSetAt, Is.EqualTo(DateTimeOffset.UtcNow).Within(TimeSpan.FromSeconds(1)));
    }
    
    [Test]
    public void When_setting_current_course_with_an_eta_but_the_engine_system_is_not_found()
    {
        var state = new NavigationState();
        var payload = new SetCoursePayload
        {
            CourseId = RandomString(),
            Destination = "Alpha Centauri",
            Coordinates = new Coordinates { X = 123.45, Y = 234.56, Z = 345.67 },
            Eta = new SetEtaPayload
            {
                EngineSystem = "sublight-engines",
                Speed = 2,
                ArriveInMilliseconds = 123456
            }
        };
        SetupNoMockEngines();

        var result = ClassUnderTest.SetCourse(state, payload);

        Assert.That(result.ErrorMessage, Is.EqualTo($"Unable to locate the engine system: {payload.Eta.EngineSystem}"));
    }
    
    [TestCase(true, false, false, StandardSystemBaseState.DisabledError)]
    [TestCase(false, true, false, StandardSystemBaseState.DamagedError)]
    [TestCase(false, false, true, StandardSystemBaseState.InsufficientPowerError)]
    public void When_setting_course_but_inoperable(bool isDisabled, bool isDamaged, bool hasInsufficientPower, string expectedError)
    {
        var state = new NavigationState
        {
            Disabled = isDisabled,
            Damaged = isDamaged,
            RequiredPower = 5,
            CurrentPower = hasInsufficientPower ? 0 : 5
        };
        var payload = new SetCoursePayload
        {
            CourseId = RandomString(),
            Destination = "Alpha Centauri",
            Coordinates = new Coordinates { X = 123.45, Y = 234.56, Z = 345.67 }
        };

        var result = ClassUnderTest.SetCourse(state, payload);
        
        Assert.That(result.ErrorMessage, Is.EqualTo(expectedError));
    }

    [Test]
    public void When_clearing_course()
    {
        var state = new NavigationState
        {
            CurrentCourse = new CurrentCourse()
        };

        var result = ClassUnderTest.ClearCourse(state);
        
        Assert.That(result.NewState.Value, Is.EqualTo(state with { CurrentCourse = null }));
    }

    [TestCase(1, 100, new [] { 100, 50, 33, 25, 20, 16 })]
    [TestCase(2, 30000, new [] { 60000, 30000, 20000, 15000, 12000, 10000, 8571 })]
    public void When_updating_eta(int speed, int milliseconds, int[] expectedMilliseconds)
    {
        var state = new NavigationState
        {
            CurrentCourse = new CurrentCourse()
        };
        var payload = new SetEtaPayload
        {
            EngineSystem = "test-engines",
            Speed = speed,
            ArriveInMilliseconds = milliseconds 
        };
        var expectedTravelTimes = expectedMilliseconds.Select((x, i) => new TravelTime
        {
            Speed = i + 1,
            ArriveInMilliseconds = x
        });
        SetupMockEngines(0, expectedMilliseconds.Length, payload.EngineSystem);
        
        var result = ClassUnderTest.UpdateEta(state, payload);

        var newEta = result.NewState.Value.CurrentCourse.Eta;
        Assert.That(newEta.EngineSystem, Is.EqualTo(payload.EngineSystem));
        Assert.That(newEta.TravelTimes, Is.EqualTo(expectedTravelTimes));
    }

    [Test]
    public void When_updating_eta_and_the_engine_system_is_not_found()
    {
        var state = new NavigationState
        {
            CurrentCourse = new CurrentCourse()
        };
        var payload = new SetEtaPayload
        {
            EngineSystem = "sublight-engines",
            Speed = 2,
            ArriveInMilliseconds = 123456
        };
        SetupNoMockEngines();

        var result = ClassUnderTest.UpdateEta(state, payload);

        Assert.That(result.ErrorMessage, Is.EqualTo($"Unable to locate the engine system: {payload.EngineSystem}"));
    }
    
    [Test]
    public void When_updating_eta_and_there_is_no_current_course()
    {
        var state = new NavigationState();
        var payload = new SetEtaPayload
        {
            EngineSystem = "sublight-engines",
            Speed = 2,
            ArriveInMilliseconds = 123456
        };
        SetupNoMockEngines();

        var result = ClassUnderTest.UpdateEta(state, payload);

        Assert.That(result.ErrorMessage, Is.EqualTo("No current course for setting the ETA"));
    }

    [Test]
    public void When_clearing_eta()
    {
        var state = new NavigationState
        {
            CurrentCourse = new CurrentCourse { Eta = new Eta() }
        };
        
        var result = ClassUnderTest.ClearEta(state);
        
        Assert.That(result.NewState.Value, Is.EqualTo(state with { CurrentCourse = state.CurrentCourse with { Eta = null }}));
    }
    
    [Test]
    public void When_clearing_eta_but_there_is_no_course()
    {
        var state = new NavigationState
        {
            CurrentCourse = null
        };
        
        var result = ClassUnderTest.ClearEta(state);
        
        Assert.That(result.ErrorMessage, Is.EqualTo("No current course for clearing the ETA"));
    }

    [TestCase(new[] {600, 300, 200, 150}, 1, 100, new[] {500, 250, 166, 125})]
    [TestCase(new[] {600, 300, 200, 150}, 2, 100, new[] {400, 200, 133, 100})]
    [TestCase(new[] {600, 300, 200, 150}, 3, 100, new[] {300, 150, 100, 75})]
    [TestCase(new[] {600, 300, 200, 150}, 3, 150, new[] {150, 75, 50, 37})]
    [TestCase(new[] {600, 300, 200, 150}, 4, 100, new[] {200, 100, 66, 50})]
    public void When_traveling(int[] travelTimes, int speed, long elapsed, int[] expected)
    {
        var state = new NavigationState
        {
            CurrentCourse = new CurrentCourse
            {
                Eta = new Eta
                {
                    EngineSystem = "ftl-engines",
                    TravelTimes = travelTimes.Select((x, i) => new TravelTime
                    {
                        Speed = i + 1,
                        ArriveInMilliseconds = x
                    }).ToArray()
                }
            }
        };
        var expectedTravelTimes = expected.Select((x, i) => new TravelTime
        {
            Speed = i + 1,
            ArriveInMilliseconds = x
        }).ToArray();
        var payload = new ChronometerPayload { ElapsedMilliseconds = elapsed };
        SetupMockEngines(speed, travelTimes.Length, state.CurrentCourse.Eta.EngineSystem);

        var result = ClassUnderTest.Travel(state, payload);
        
        Assert.That(result.NewState.Value.CurrentCourse.Eta.EngineSystem, Is.EqualTo(state.CurrentCourse.Eta.EngineSystem));
        Assert.That(result.NewState.Value.CurrentCourse.Eta.TravelTimes, Is.EqualTo(expectedTravelTimes));
    }
    
    [TestCase(new[] {600, 300, 200, 150})]
    [TestCase(new[] {600, 300, 200, 150})]
    [TestCase(new[] {600, 300, 200, 150})]
    [TestCase(new[] {600, 300, 200, 150})]
    [TestCase(new[] {600, 300, 200, 150})]
    public void When_traveling_but_the_speed_is_zero(int[] travelTimes)
    {
        var state = new NavigationState
        {
            CurrentCourse = new CurrentCourse
            {
                Eta = new Eta
                {
                    EngineSystem = "ftl-engines",
                    TravelTimes = travelTimes.Select((x, i) => new TravelTime
                    {
                        Speed = i + 1,
                        ArriveInMilliseconds = x
                    }).ToArray()
                }
            }
        };
        var payload = new ChronometerPayload { ElapsedMilliseconds = 1000 };
        SetupMockEngines(0, travelTimes.Length, state.CurrentCourse.Eta.EngineSystem);

        var result = ClassUnderTest.Travel(state, payload);

        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));
    }

    private void SetupMockEngines(int currentSpeed, int maxSpeed, string systemName)
    {
        var mockEngines = GetMock<IEnginesSystem>();
        mockEngines.Setup(x => x.CurrentSpeed).Returns(currentSpeed);
        mockEngines.Setup(x => x.MaxSpeed).Returns(maxSpeed);
        GetMock<ISystemsRegistry>()
            .Setup(x => x.GetSystemByNameAs<IEnginesSystem>(systemName))
            .ReturnsSome(mockEngines.Object);
    }

    private void SetupNoMockEngines()
    {
        GetMock<ISystemsRegistry>()
            .Setup(x => x.GetSystemByNameAs<IEnginesSystem>(Any<string>()))
            .ReturnsNone();
    }

    [TestCase(new[] {600, 300, 200, 150}, 3, 200)]
    [TestCase(new[] {600, 300, 200, 150}, 3, 201)]
    [TestCase(new[] {600, 300, 200, 150}, 3, 300)]
    [TestCase(new[] {600, 300, 200, 150}, 2, 300)]
    [TestCase(new[] {600, 300, 200, 150}, 2, 340)]
    public void When_traveling_and_you_arrive_at_the_destination(int[] travelTimes, int speed, long elapsed)
    {
        var state = new NavigationState
        {
            CurrentCourse = new CurrentCourse
            {
                Eta = new Eta
                {
                    EngineSystem = "ftl-engines",
                    TravelTimes = travelTimes.Select((x, i) => new TravelTime
                    {
                        Speed = i + 1,
                        ArriveInMilliseconds = x
                    }).ToArray()
                }
            }
        };
        var expectedTravelTimes = travelTimes.Select((x, i) => new TravelTime
        {
            Speed = i + 1,
            ArriveInMilliseconds = 0
        }).ToArray();
        var payload = new ChronometerPayload { ElapsedMilliseconds = elapsed };
        SetupMockEngines(speed, travelTimes.Length, state.CurrentCourse.Eta.EngineSystem);
        
        var result = ClassUnderTest.Travel(state, payload);
        
        Assert.That(result.NewState.Value.CurrentCourse.Eta.TravelTimes, Is.EqualTo(expectedTravelTimes));
    }
    
    [Test]
    public void When_traveling_and_you_are_already_at_the_destination()
    {
        var state = new NavigationState
        {
            CurrentCourse = new CurrentCourse
            {
                Eta = new Eta
                {
                    EngineSystem = "ftl-engines",
                    TravelTimes = new [] { new TravelTime { Speed = 1, ArriveInMilliseconds = 0 }}
                }
            }
        };
        var payload = new ChronometerPayload { ElapsedMilliseconds = 1 };
        SetupMockEngines(1, 10, state.CurrentCourse.Eta.EngineSystem);
        
        var result = ClassUnderTest.Travel(state, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));
    }
    
    [Test]
    public void When_traveling_but_there_is_no_current_course()
    {
        var state = new NavigationState
        {
            CurrentCourse = null
        };
        var payload = new ChronometerPayload { ElapsedMilliseconds = 100 };
        
        var result = ClassUnderTest.Travel(state, payload);

        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));
    }
    
    [Test]
    public void When_traveling_but_there_is_no_current_course_eta()
    {
        var state = new NavigationState
        {
            CurrentCourse = new CurrentCourse
            {
                Eta = null
            }
        };
        var payload = new ChronometerPayload { ElapsedMilliseconds = 100 };
        
        var result = ClassUnderTest.Travel(state, payload);

        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));
    }
    
    [Test]
    public void When_traveling_but_the_engine_system_cannot_be_found()
    {
        var state = new NavigationState
        {
            CurrentCourse = new CurrentCourse
            {
                Eta = new Eta
                {
                    EngineSystem = "sublight-engines",
                    TravelTimes = new []{ new TravelTime { Speed = 1, ArriveInMilliseconds = 1 }} 
                }
            }
        };
        var payload = new ChronometerPayload { ElapsedMilliseconds = 100 };
        SetupNoMockEngines();
        
        var result = ClassUnderTest.Travel(state, payload);

        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));
    }
}