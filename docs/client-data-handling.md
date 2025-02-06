# Client Data Handling

This page has a few examples of how a client of the OpenStardrive server might utilize data from
[command results](./overview.md) obtained from the `/results` [API endpoint](./api.md).

These examples are in JavaScript to keep the code lightweight.
A typed language follow the same process, but would need to deserialize to types.

> Note: these code examples have not been fully tested, but are only intended to provide the general ideas.


## Example 1: Simple State Handling

The client could initialize the server / simulation state with a simple empty dictionary (hash):

```javascript

const state = {
    systems: {}
}
```

The client can then poll the server periodically (perhaps once per second), for each response:

```javascript
response.results.forEach(result => {
    if (result.type === 'state-updated') {
        state.systems[result.system] = result.payload
    }
})
```

Now the state contains the latest information about each system. For example:

```javascript
const sublight = state.systems['sublight-engines']
renderSublightSpeed(sublight.currentSpeed)
if (sublight.currentHeat > sublight.poweredHeat.cruisingHeat) {
    displaySublightHeatWarning()
}
```


## Example 2: Tracking Transient Data

Some pieces of data do not appear in every command result.
For example, the set of all individual text-based sensor scans that were ever made
do not appear in every `sensors` system result, so you may have to capture them for later reference.

```javascript
const sensorScans = []

const processCommandResults = (commandResults) => {
    commandResults.forEach(result => {
        if (result.system !== 'sensors' || type !== 'state-updated') {
            return
        }
        if (!result.lastUpdatedScan) {
            return
        }
        
        const scan = result.lastUpdatedScan
        const index = sensorScans.findIndex(x => x.scanId === scan.scanId);
        if (index >= 0) {
            sensorScans[index] = scan
        } else {
            sensorScans.push(scan)
        }
    })
}
```


## Example 3: Tracking Time Spent In Each Alert Condition

```javascript
const alertTime = {
    currentLevel: null,
    started: new Date().toISOString(),
    timeInLevel: {}
}

const processCommandResults = (commandResults) => {
    commandResults.forEach(result => {
        if (result.system !== 'alert' || type !== 'state-updated') {
            return
        }
        
        const alertLevel = result.payload.current.level
        if (alertTime.currentLevel !== alertLevel) {
            if (alertTime.currentLevel != null) {
                const stopped = result.timestamp
                const totalMs = new Date(stopped).valueOf() - new Date(alertTime.started).valueOf()
                alertTime.timeInLevel[alertTime.currentLevel] = totalMs
            }

            alertLevel.currentLevel = alertLevel
            alertTime.started = result.timestamp
        }
    })
}
```
