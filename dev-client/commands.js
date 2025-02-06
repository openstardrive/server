const getCommands = (api, state) => {
    const randomFloat = (min, max) => Math.random() * (max - min) + min
    const randomInt = (min, max) => Math.floor(Math.random() * (max - min) + min)
    const randomPosition = () => ({x: randomFloat(10, 100), y: randomFloat(10, 100), z: randomFloat(10, 100)})
    const randomId = () => `${randomInt(100000, 999999)}`

    return {
        logCurrentState: () => state.logCurrentState(),
        updateSystem: (system, action) => {
            const parts = action.split(':')
            api.sendCommand(`set-${parts[0]}`, {[system]: parts[1] === "true"})
        },
        speedDown: (system) => api.sendCommand(`set-${system}-speed`, {speed: state.getSystemState(system).currentSpeed - 1 }),
        speedUp: (system) => api.sendCommand(`set-${system}-speed`, {speed: state.getSystemState(system).currentSpeed + 1 }),
        powerDown: (system) => api.sendCommand(`set-power`, {[system]: state.getSystemState(system).currentPower - 1 }),
        powerUp: (system) => api.sendCommand(`set-power`, {[system]: state.getSystemState(system).currentPower + 1 }),
        setThrusterVelocity: data => {
            const parts = data.split(',').map(x => parseInt(x))
            api.sendCommand(`set-thrusters-velocity`, {x: parts[0], y: parts[1], z: parts[2]})
        },
        setThrusterAttitude: (axis, amount) => {
            const currentState = state.getSystemState('thrusters').attitude
            api.sendCommand('set-thrusters-attitude', {...currentState, [axis]: currentState[axis] + amount})
        },
        toggleShields: () => {
            const command = state.getSystemState('shields').raised ? 'lower-shields' : 'raise-shields'
            api.sendCommand(command, null)
        },
        loadWarhead: () => api.sendCommand(`load-warhead`, {kind: 'torpedo'}),
        fireWarhead: () => api.sendCommand('fire-warhead', {kind: 'torpedo', target: 'none'}),
        chargeEnergyBeam: (bankName) => api.sendCommand('charge-energy-beam', {bankName, newCharge: Math.random()}),
        fireEnergyBeam: (bankName) => api.sendCommand('fire-energy-beam', {bankName, dischargePercent: Math.random(), target: 'asteroid'}),
        newSensorScan: text => api.sendCommand('new-sensor-scan', {scanId: randomId(), scanFor: text}),
        sensorScanResult: text => api.sendCommand('set-sensor-scan-result', {scanId: state.getSystemState('sensors').activeScans[0].scanId, result: text}),
        passiveScan: text => api.sendCommand('passive-sensor-scan', {scanFor: '[passive]', result: text}),
        addContact: () => {
            api.sendCommand('new-sensor-contact', {
                contactId: randomId(),
                name: 'asteroid',
                'icon': 'asteroid',
                position: randomPosition(),
                destinations: [
                    {
                        position: {x: 0, y: 0, z: 0},
                        remainingMilliseconds: 1000 * 30
                    }
                ]
            })
        },
        moveContact: () => {
            const contacts = state.getSystemState('sensors').contacts
            const contact = contacts[randomInt(0, contacts.length)]
            api.sendCommand('update-sensor-contact', {
                ...contact,
                destinations: [
                    ...contact.destinations,
                    {position: randomPosition(), remainingMilliseconds: 1000 * 30}
                ]
            })
        },
        requestCourse: destination => api.sendCommand('request-course-calculation', {courseId: randomId(), destination}),
        calculateCourse: () => {
            const requested = state.getSystemState('navigation').requestedCourseCalculations[0]
            api.sendCommand('course-calculated', {
                courseId: requested.courseId,
                destination: requested.destination,
                coordinates: randomPosition()
            })
        },
        randomCourse: text => {
            api.sendCommand('course-calculated', {
                courseId: randomId(),
                destination: text,
                coordinates: randomPosition()
            })
        },
        setCourse: text => api.sendCommand('set-course', {courseId: randomId(), destination: text, coordinates: randomPosition()}),
        clearCourse: () => api.sendCommand('clear-course', {}),
        setEta: () => api.sendCommand('update-eta', {engineSystem: 'ftl-engines', speed: 4, arriveInMilliseconds: randomInt(30000, 60000)}),
        clearEta: () => api.sendCommand('clear-eta', {}),
        addRandomCypher: () => {
            const letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".split('')
            const mixed = letters.concat().sort((a, b) => randomInt(0, 3) - 1)
            api.sendCommand('set-long-range-cypher', {
                cypherId: randomId(),
                name: 'Random Cypher ' + randomInt(0, 10000),
                description: 'A random cypher (A-Z)',
                encodeSubstitutions: letters.map((x, i) => ({change: x, to: mixed[i]})),
                decodeSubstitutions: letters.map(x => ({change: x, to: x}))
            })
        },
        sendLongRangeMessage: message => {
            const cypher = state.getSystemState('long-range-comms').cyphers[0] || {cypherId: null}
            api.sendCommand('send-long-range-message', {
                messageId: randomId(),
                sender: 'Starbase',
                recipient: 'Odyssey',
                message,
                cypherId: cypher.cypherId,
                outbound: false
            })
        },
        decode: () => {
            const cyphers = state.getSystemState('long-range-comms').cyphers
            const cypher = cyphers[randomInt(0, cyphers.length)]
            let solved = 0
            const updated = cypher.decodeSubstitutions.map(x => {
                const match = cypher.encodeSubstitutions.find(y => y.to == x.change) || {change: x.change}
                const isSolved = x.to === match.change
                if (!isSolved && solved < 3) {
                    solved++
                    return {change: x.change, to: match.change}
                }
                return x
            })
            api.sendCommand('update-long-range-substitutions', {
                cypherId: cypher.cypherId,
                decodeSubstitutions: updated
            })
        },
        addRandomFrequencyRange: () => {
            const frequencyRanges = state.getSystemState('short-range-comms').frequencyRanges
            const a = randomFloat(1000, 9999)
            const b = randomFloat(1000, 9999)
            frequencyRanges.push({name: 'Band ' + frequencyRanges.length, min: Math.min(a, b), max: Math.max(a, b)})
            api.sendCommand('configure-short-range-frequencies', { frequencyRanges })
        },
        addRandomSignal: () => {
            const activeSignals = state.getSystemState('short-range-comms').activeSignals
            activeSignals.push({name: `Signal ${randomInt(0, 9999)}`, frequency: randomFloat(1000, 9999)})
            api.sendCommand('set-short-range-signals', { activeSignals })
        },
        removeRandomSignal: () => {
            const activeSignals = state.getSystemState('short-range-comms').activeSignals
            const index = randomInt(0, activeSignals.length)
            activeSignals = activeSignals.filter((_, i) => i !== index)
            api.sendCommand('set-short-range-signals', { activeSignals })
        },
        setRandomFrequency: () => {
            api.sendCommand('set-short-range-frequency', {frequency: randomFloat(1000, 9999)})
        },
        toggleBroadcast: () => {
            const isBroadcasting = state.getSystemState('short-range-comms').isBroadcasting
            api.sendCommand('set-short-range-broadcasting', {isBroadcasting: !isBroadcasting})
        },
        addDebugEntry: text => {
            api.sendCommand('debug', { debugId: randomId(), description: text })
        },
        disableClient: text => {
            const clients = state.getSystemState('clients').clients
            const clientId = clients[randomInt(0, clients.length)].clientId
            api.sendCommand('set-client-disabled', { clientId, disabled: true, disabledMessage: text })
        },
        enableClient: () => {
            const clients = state.getSystemState('clients').clients
            const clientId = clients[randomInt(0, clients.length)].clientId
            api.sendCommand('set-client-disabled', { clientId, disabled: false })
        },
        setOperator: text => {
            const clients = state.getSystemState('clients').clients
            const clientId = clients[randomInt(0, clients.length)].clientId
            api.sendCommand('set-client-operator', { clientId, operator: text })
        },
        setClientScreen: text => {
            const clients = state.getSystemState('clients').clients
            const clientId = clients[randomInt(0, clients.length)].clientId
            api.sendCommand('set-client-screen', { clientId, currentScreen: text })
        },
        configurePower: () => {
            api.sendCommand('configure-power', {
                targetOutput: 68,
                reactorDrift: 5,
                numberOfBatteries: 4,
                maxBatteryCharge: 250,
                currentBatteryCharge: 200,
                updateRateInMilliseconds: 5000
            })
            const powerPayload = {
                'navigation': 3,
                'thrusters': 4,
                'shields': 10,
                'warhead-launcher': 5,
                'energy-beams': 10,
                'sensors': 7,
                'long-range-comms': 3,
                'short-range-comms': 5,
                'life-support': 5
            }
            api.sendCommand('set-required-power', powerPayload)
            api.sendCommand('set-power', powerPayload)
        },
        damageBattery: () => {
            const batteries = state.getSystemState('power').batteries
                .map((x, i) => ({...x, i})).filter(x => !x.damaged)
            const index = randomInt(0, batteries.length)
            api.sendCommand('set-battery-damage', { batteryIndex: batteries[index].i, isDamaged: true })
        },
        repairBattery: () => {
            const batteries = state.getSystemState('power').batteries
                .map((x, i) => ({...x, i})).filter(x => x.damaged)
            const index = randomInt(0, batteries.length)
            api.sendCommand('set-battery-damage', { batteryIndex: batteries[index].i, isDamaged: false })
        },
        randomBatteryCharge: () => {
            const batteries = state.getSystemState('power').batteries
            const batteryIndex = randomInt(0, batteries.length)
            const charge = randomInt(0, 250)
            api.sendCommand('set-battery-charge', { batteryIndex, charge })
        },
        bumpReactorOutput: amount => {
            const config = state.getSystemState('power').config
            api.sendCommand('configure-power', {...config, targetOutput: config.targetOutput + amount})
        },
        bumpReactorDrift: amount => {
            const config = state.getSystemState('power').config
            api.sendCommand('configure-power', {...config, reactorDrift: config.reactorDrift + amount})
        },
        setAlert: change => {
            const level = state.getSystemState('alert').current.level + change
            api.sendCommand('set-alert-level', { level })
        }
    }
}