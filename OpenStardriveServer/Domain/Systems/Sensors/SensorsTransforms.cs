using System;
using System.Linq;
using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Sensors;

public interface ISensorsTransforms : IStandardTransforms<SensorsState>
{
    TransformResult<SensorsState> NewScan(SensorsState state, NewScanPayload payload);
    TransformResult<SensorsState> SetScanResult(SensorsState state, ScanResultPayload payload);
    TransformResult<SensorsState> CancelScan(SensorsState state, CancelScanPayload payload);
    TransformResult<SensorsState> PassiveScan(SensorsState state, PassiveScanPayload payload);
    TransformResult<SensorsState> NewContact(SensorsState state, NewSensorContactPayload payload);
    TransformResult<SensorsState> RemoveContact(SensorsState state, RemoveSensorContactPayload payload);
    TransformResult<SensorsState> UpdateContact(SensorsState state, UpdateSensorContactPayload payload);
    TransformResult<SensorsState> MoveContacts(SensorsState state, ChronometerPayload payload);
}

public class SensorsTransforms : ISensorsTransforms 
{
    private readonly IStandardTransforms<SensorsState> standardTransforms;

    public SensorsTransforms(IStandardTransforms<SensorsState> standardTransforms)
    {
        this.standardTransforms = standardTransforms;
    }
    
    public TransformResult<SensorsState> SetDisabled(SensorsState state, string systemName, DisabledSystemsPayload payload)
    {
        return standardTransforms.SetDisabled(state, systemName, payload);
    }

    public TransformResult<SensorsState> SetDamaged(SensorsState state, string systemName, DamagedSystemsPayload payload)
    {
        return standardTransforms.SetDamaged(state, systemName, payload);
    }

    public TransformResult<SensorsState> SetCurrentPower(SensorsState state, string systemName, CurrentPowerPayload payload)
    {
        return standardTransforms.SetCurrentPower(state, systemName, payload);
    }

    public TransformResult<SensorsState> SetRequiredPower(SensorsState state, string systemName, RequiredPowerPayload payload)
    {
        return standardTransforms.SetRequiredPower(state, systemName, payload);
    }

    public TransformResult<SensorsState> NewScan(SensorsState state, NewScanPayload payload)
    {
        if (string.IsNullOrEmpty(payload.ScanId))
        {
            return TransformResult<SensorsState>.Error("You must specify a scanId");
        }

        if (state.ActiveScans.Any(x => x.ScanId == payload.ScanId))
        {
            return TransformResult<SensorsState>.Error("You must specify a unique scanId");
        }
        
        if (string.IsNullOrEmpty(payload.ScanFor))
        {
            return TransformResult<SensorsState>.Error("You must specify what to scan for");
        }
        
        var scan = new SensorScan
        {
            ScanId = payload.ScanId,
            ScanFor = payload.ScanFor,
            State = SensorScanState.Active
        };
        return TransformResult<SensorsState>.StateChanged(state with
        {
            ActiveScans = state.ActiveScans.Append(scan).ToArray(),
            LastUpdatedScan = scan
        });
    }

    public TransformResult<SensorsState> SetScanResult(SensorsState state, ScanResultPayload payload)
    {
        return state.ActiveScans.FirstOrNone(x => x.ScanId == payload.ScanId).Case(
            some: scan => TransformResult<SensorsState>.StateChanged(state with
            {
                LastUpdatedScan = scan with { Result = payload.Result, State = SensorScanState.Completed, LastUpdated = DateTimeOffset.UtcNow },
                ActiveScans = state.ActiveScans.Where(x => x.ScanId != payload.ScanId).ToArray()
            }),
            none: () => TransformResult<SensorsState>.Error($"No active scan with id {payload.ScanId}"));
    }

    public TransformResult<SensorsState> CancelScan(SensorsState state, CancelScanPayload payload)
    {
        return state.ActiveScans.FirstOrNone(x => x.ScanId == payload.ScanId).Case(
            some: scan => TransformResult<SensorsState>.StateChanged(state with
            {
                LastUpdatedScan = scan with { State = SensorScanState.Canceled, LastUpdated = DateTimeOffset.UtcNow },
                ActiveScans = state.ActiveScans.Where(x => x.ScanId != payload.ScanId).ToArray()
            }), 
            none: () => TransformResult<SensorsState>.Error($"No active scan with id {payload.ScanId}"));
    }

    public TransformResult<SensorsState> PassiveScan(SensorsState state, PassiveScanPayload payload)
    {
        return TransformResult<SensorsState>.StateChanged(state with
        {
            LastUpdatedScan = new SensorScan
            {
                ScanFor = payload.ScanFor,
                Result = payload.Result,
                State = SensorScanState.Completed
            }
        });
    }

    public TransformResult<SensorsState> NewContact(SensorsState state, NewSensorContactPayload payload)
    {
        if (string.IsNullOrEmpty(payload.ContactId))
        {
            return TransformResult<SensorsState>.Error("You must specify a contactId");    
        }
        
        if (state.Contacts.Any(x => x.ContactId == payload.ContactId))
        {
            return TransformResult<SensorsState>.Error("You must specify a unique contactId");    
        }
        
        var newContact = new SensorContact
        {
            ContactId = payload.ContactId,
            Name = payload.Name,
            Icon = payload.Icon,
            Position = payload.Position,
            Destinations = payload.Destinations
        };
        return TransformResult<SensorsState>.StateChanged(state with
        {
            Contacts = state.Contacts.Append(newContact).ToArray()
        });
    }

    public TransformResult<SensorsState> RemoveContact(SensorsState state, RemoveSensorContactPayload payload)
    {
        return state.Contacts.FirstOrNone(x => x.ContactId == payload.ContactId).Case(
            some: _ => TransformResult<SensorsState>.StateChanged(state with
            {
                Contacts = state.Contacts.Where(x => x.ContactId != payload.ContactId).ToArray()
            }),
            none: () => TransformResult<SensorsState>.Error($"No sensor contact found with id {payload.ContactId}"));
    }

    public TransformResult<SensorsState> UpdateContact(SensorsState state, UpdateSensorContactPayload payload)
    {
        return state.Contacts.FirstOrNone(x => x.ContactId == payload.ContactId).Case(
            some: _ => TransformResult<SensorsState>.StateChanged(state with
            {
                Contacts = state.Contacts
                    .Replace(x => x.ContactId == payload.ContactId, _ => payload)
                    .ToArray()
            }),
            none: () => TransformResult<SensorsState>.Error($"No sensor contact found with id {payload.ContactId}"));
    }

    public TransformResult<SensorsState> MoveContacts(SensorsState state, ChronometerPayload payload)
    {
        if (state.Contacts.None(x => x.Destinations.Any()))
        {
            return TransformResult<SensorsState>.NoChange();    
        }
        return TransformResult<SensorsState>.StateChanged(state with
        {
            Contacts = state.Contacts.Select(x => MoveContact(x, payload.ElapsedMilliseconds)).ToArray()
        });
    }

    private SensorContact MoveContact(SensorContact contact, long elapsed)
    {
        if (!contact.Destinations.Any())
        {
            return contact;    
        }

        var destination = contact.Destinations.First();
        var remaining = destination.RemainingMilliseconds;
        var target = destination.Position;
        
        return remaining <= elapsed
            ? ReachDestination(contact, target)
            : MoveTowardDestination(contact, target, remaining, elapsed);
    }

    private static SensorContact ReachDestination(SensorContact contact, Point target)
    {
        return contact with
        {
            Position = target,
            Destinations = contact.Destinations
                .Where((_, i) => i != 0)
                .ToArray()
        };
    }

    private SensorContact MoveTowardDestination(SensorContact contact, Point target, int remaining, long elapsed)
    {
        var origin = contact.Position;
        return contact with
        {
            Position = new Point
            {
                X = origin.X + AmountToMove(origin.X, target.X, remaining, elapsed),
                Y = origin.Y + AmountToMove(origin.Y, target.Y, remaining, elapsed),
                Z = origin.Z + AmountToMove(origin.Z, target.Z, remaining, elapsed)
            },
            Destinations = contact.Destinations
                .Replace((_, i) => i == 0, x => x with
                {
                    RemainingMilliseconds = (int) (x.RemainingMilliseconds - elapsed)
                })
                .ToArray()
        };
    }

    private double AmountToMove(double origin, double destination, int remainingMilliseconds, long elapsedMilliseconds)
    {
        var distance = (destination - origin) / remainingMilliseconds;
        return distance * elapsedMilliseconds;
    }
}