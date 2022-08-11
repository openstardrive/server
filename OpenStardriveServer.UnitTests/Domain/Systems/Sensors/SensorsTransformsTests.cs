using System;
using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Sensors;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Sensors;

public class SensorsTransformsTests : StandardTransformsTest<SensorsTransforms, SensorsState>
{
    [Test]
    public void When_setting_disabled()
    {
        TestStandardDisabled(new SensorsState());
    }
    
    [Test]
    public void When_setting_damaged()
    {
        TestStandardDamaged(new SensorsState());
    }
    
    [Test]
    public void When_setting_current_power()
    {
        TestStandardCurrentPower(new SensorsState());
    }
    
    [Test]
    public void When_setting_required_power()
    {
        TestStandardRequiredPower(new SensorsState());
    }

    [Test]
    public void When_starting_a_scan()
    {
        var state = new SensorsState();
        var payload = new NewScanPayload { ScanFor = "life signs" };

        var result = ClassUnderTest.NewScan(state, payload);

        var newState = result.NewState.Value;
        Assert.That(newState.LastUpdatedScan.ScanFor, Is.EqualTo(payload.ScanFor));
        Assert.That(newState.LastUpdatedScan.State, Is.EqualTo(SensorScanState.Active));
        Assert.That(newState.ActiveScans.Length, Is.EqualTo(1));
        Assert.That(newState.ActiveScans, Does.Contain(newState.LastUpdatedScan));
    }
    
    [TestCase(null)]
    [TestCase("")]
    public void When_starting_a_scan_without_specifying_what_to_scan_for(string scanFor)
    {
        var state = new SensorsState();
        var payload = new NewScanPayload { ScanFor = scanFor };

        var result = ClassUnderTest.NewScan(state, payload);

        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo("you must specify what to scan for"));
    }

    [Test]
    public void When_setting_a_scan_result()
    {
        var state = new SensorsState
        {
            ActiveScans = new[]
            {
                new SensorScan { ScanFor = "life forms", State = SensorScanState.Active },
                new SensorScan { ScanFor = "black holes", State = SensorScanState.Active },
                new SensorScan { ScanFor = "debris", State = SensorScanState.Active },
            }
        };
        var payload = new ScanResultPayload
        {
            ScanId = state.ActiveScans[1].ScanId,
            Result = "None detected."
        };
        var expected = state.ActiveScans[1] with
        {
            Result = payload.Result,
            State = SensorScanState.Completed
        };
        var expectedActive = new[]
        {
            state.ActiveScans[0],
            state.ActiveScans[2]
        };

        var result = ClassUnderTest.SetScanResult(state, payload);

        var newState = result.NewState.Value;
        Assert.That(newState.LastUpdatedScan, Is.EqualTo(expected));
        Assert.That(newState.ActiveScans, Is.EqualTo(expectedActive));
    }
    
    [Test]
    public void When_setting_a_scan_result_but_there_is_no_matching_active_scan()
    {
        var state = new SensorsState
        {
            ActiveScans = new[]
            {
                new SensorScan { ScanFor = "life forms", State = SensorScanState.Active },
                new SensorScan { ScanFor = "black holes", State = SensorScanState.Active },
                new SensorScan { ScanFor = "debris", State = SensorScanState.Active },
            }
        };
        var payload = new ScanResultPayload
        {
            ScanId = Guid.NewGuid(),
            Result = "The vessel is of unknown configuration."
        };

        var result = ClassUnderTest.SetScanResult(state, payload);

        Assert.That(result.ErrorMessage, Is.EqualTo($"No active scan with id {payload.ScanId}"));
    }

    [Test]
    public void When_canceling_an_active_scan()
    {
        var scanId = Guid.NewGuid();
        var state = new SensorsState
        {
            ActiveScans = new[]
            {
                new SensorScan { ScanId = scanId, ScanFor = "ice on Mars", State = SensorScanState.Active },
                new SensorScan { ScanId = Guid.NewGuid(), ScanFor = "diamonds on Jupiter", State = SensorScanState.Active },
                new SensorScan { ScanId = Guid.NewGuid(), ScanFor = "is Pluto a planet?", State = SensorScanState.Active }
            }
        };
        var payload = new CancelScanPayload { ScanId = scanId };
        var expected = state.ActiveScans[0] with { State = SensorScanState.Canceled };
        var expectedActive = new[] { state.ActiveScans[1], state.ActiveScans[2] };
        
        var result = ClassUnderTest.CancelScan(state, payload);
        
        var newState = result.NewState.Value;
        Assert.That(newState.LastUpdatedScan, Is.EqualTo(expected));
        Assert.That(newState.ActiveScans, Is.EqualTo(expectedActive));
    }
    
    [Test]
    public void When_canceling_an_active_scan_but_there_is_no_matching_active_scan()
    {
        var scanId = Guid.NewGuid();
        var state = new SensorsState
        {
            ActiveScans = new[]
            {
                new SensorScan { ScanId = Guid.NewGuid(), ScanFor = "ice on Mars", State = SensorScanState.Active },
                new SensorScan { ScanId = Guid.NewGuid(), ScanFor = "diamonds on Jupiter", State = SensorScanState.Active },
                new SensorScan { ScanId = Guid.NewGuid(), ScanFor = "is Pluto a planet?", State = SensorScanState.Active }
            }
        };
        var payload = new CancelScanPayload { ScanId = scanId };
        
        var result = ClassUnderTest.CancelScan(state, payload);
        
        Assert.That(result.ErrorMessage, Is.EqualTo($"No active scan with id {payload.ScanId}"));
    }

    [Test]
    public void When_adding_a_passive_scan()
    {
        var state = new SensorsState();
        var payload = new PassiveScanPayload { ScanFor = "Aliens", Result = "13 alien lifeforms detected"};

        var result = ClassUnderTest.PassiveScan(state, payload);
        
        var newState = result.NewState.Value;
        Assert.That(newState.LastUpdatedScan.ScanFor, Is.EqualTo(payload.ScanFor));
        Assert.That(newState.LastUpdatedScan.Result, Is.EqualTo(payload.Result));
        Assert.That(newState.LastUpdatedScan.State, Is.EqualTo(SensorScanState.Completed));
        Assert.That(newState.ActiveScans.Length, Is.EqualTo(state.ActiveScans.Length));
    }

    [Test]
    public void When_adding_a_new_sensor_contact()
    {
        var state = new SensorsState
        {
            Contacts = new []
            {
                new SensorContact(),
                new SensorContact()
            }
        };
        var payload = new NewSensorContactPayload
        {
            Name = "USS Odyssey",
            Icon = "Starship",
            Position = new Point { X = 3, Y = 4, Z = -5 },
            Destinations = new []
            {
                new Destination
                {
                    Position = new Point { X = -45, Y = 22, Z = 2 }
                }
            }
        };

        var result = ClassUnderTest.NewContact(state, payload);

        var newState = result.NewState.Value;
        Assert.That(newState.Contacts.Length, Is.EqualTo(3));
        Assert.That(newState.Contacts[2].Name, Is.EqualTo(payload.Name));
        Assert.That(newState.Contacts[2].Icon, Is.EqualTo(payload.Icon));
        Assert.That(newState.Contacts[2].Position, Is.EqualTo(payload.Position));
        Assert.That(newState.Contacts[2].Destinations, Is.EqualTo(payload.Destinations));
    }

    [Test]
    public void When_removing_a_sensor_contact()
    {
        var state = new SensorsState
        {
            Contacts = new [] { new SensorContact(), new SensorContact(), new SensorContact() }
        };
        var payload = new RemoveSensorContactPayload { ContactId = state.Contacts[1].ContactId };
        var expected = new[] { state.Contacts[0], state.Contacts[2] };

        var result = ClassUnderTest.RemoveContact(state, payload);

        var newState = result.NewState.Value;
        Assert.That(newState.Contacts, Is.EqualTo(expected));
    }
    
    [Test]
    public void When_removing_a_sensor_contact_but_the_contact_is_not_found()
    {
        var state = new SensorsState
        {
            Contacts = new [] { new SensorContact(), new SensorContact(), new SensorContact() }
        };
        var payload = new RemoveSensorContactPayload { ContactId = Guid.NewGuid() };

        var result = ClassUnderTest.RemoveContact(state, payload);
        
        Assert.That(result.ErrorMessage, Is.EqualTo($"No sensor contact found with id {payload.ContactId}"));
    }

    [Test]
    public void When_updating_a_sensor_contact()
    {
        var state = new SensorsState
        {
            Contacts = new [] { new SensorContact(), new SensorContact(), new SensorContact() }
        };
        var payload = new UpdateSensorContactPayload
        {
            ContactId = state.Contacts[1].ContactId,
            Name = "Asteroid",
            Icon = "asteroid",
            Position = new Point { X = 1, Y = 2, Z = 3 },
            Destinations = Array.Empty<Destination>()
        };
        var expected = new[]
        {
            state.Contacts[0], payload, state.Contacts[2]
        };

        var result = ClassUnderTest.UpdateContact(state, payload);
        
        Assert.That(result.NewState.Value.Contacts, Is.EqualTo(expected));
    }
    
    [Test]
    public void When_updating_a_sensor_contact_but_the_contact_is_not_found()
    {
        var state = new SensorsState();
        var payload = new UpdateSensorContactPayload
        {
            ContactId = Guid.NewGuid(),
            Name = "Asteroid",
            Icon = "asteroid",
            Position = new Point { X = 1, Y = 2, Z = 3 },
            Destinations = Array.Empty<Destination>()
        };

        var result = ClassUnderTest.UpdateContact(state, payload);
        
        Assert.That(result.ErrorMessage, Is.EqualTo($"No sensor contact found with id {payload.ContactId}"));
    }

    [Test]
    public void When_moving_contacts_but_none_have_destinations()
    {
        var state = new SensorsState
        {
            Contacts = new[] { new SensorContact(), new SensorContact(), new SensorContact(), }
        };
        var payload = new ChronometerPayload { ElapsedMilliseconds = 1000 };

        var result = ClassUnderTest.MoveContacts(state, payload);
        
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));
    }

    [Test]
    public void When_moving_contacts()
    {
        var state = new SensorsState
        {
            Contacts = new []
            {
                new SensorContact
                {
                    Name = "Asteroid",
                    Icon = "asteroid",
                    Position = new Point { X = 0, Y = 0, Z = 0 },
                    Destinations = new []
                    {
                        new Destination
                        {
                            Position = new Point { X = 10, Y = 20, Z = 30},
                            RemainingMilliseconds = 10000
                        }
                    }
                },
                new SensorContact
                {
                    Name = "Star",
                    Icon = "star",
                    Position = new Point { X = 123.45, Y = 234.56, Z = 345.67 }
                },
                new SensorContact
                {
                    Name = "USS Voyager",
                    Icon = "starship",
                    Position = new Point { X = 1, Y = 2, Z = 3 },
                    Destinations = new []
                    {
                        new Destination
                        {
                            Position = new Point { X = -1, Y = -2, Z = -3 },
                            RemainingMilliseconds = 300
                        },
                        new Destination
                        {
                            Position = new Point { X = -100, Y = -200, Z = -300 },
                            RemainingMilliseconds = 10000
                        }
                    }
                }
            }
        };
        var payload = new ChronometerPayload { ElapsedMilliseconds = 1000 };
        var expected = new[]
        {
            state.Contacts[0] with
            {
                Position = new Point { X = 1, Y = 2, Z = 3 },
                Destinations = new []
                {
                    new Destination
                    {
                        Position = new Point { X = 10, Y = 20, Z = 30},
                        RemainingMilliseconds = 9000,
                    }
                }
            },
            state.Contacts[1],
            state.Contacts[2] with
            {
                Position = new Point { X = -1, Y = -2, Z = -3 },
                Destinations = new []
                {
                    state.Contacts[2].Destinations[1]
                }
            }
        };

        var result = ClassUnderTest.MoveContacts(state, payload);

        var contacts = result.NewState.Value.Contacts;
        Assert.That(contacts[0].Position, Is.EqualTo(expected[0].Position));
        Assert.That(contacts[1].Position, Is.EqualTo(expected[1].Position));
        Assert.That(contacts[2].Position, Is.EqualTo(expected[2].Position));
        
        Assert.That(contacts[0].Destinations, Is.EqualTo(expected[0].Destinations));
        Assert.That(contacts[1].Destinations, Is.EqualTo(expected[1].Destinations));
        Assert.That(contacts[2].Destinations, Is.EqualTo(expected[2].Destinations));
    }
}