const getCommands = (api, getCurrentState) => {
    const randomFloat = (min, max) => Math.random() * (max - min) + min
    const randomInt = (min, max) => Math.floor(Math.random() * (max - min) + min)
    const randomPosition = () => ({x: randomFloat(10, 100), y: randomFloat(10, 100), z: randomFloat(10, 100)})
    const randomId = () => `${randomInt(100000, 999999)}`

    return {
        updateSystem: (system, action) => {
            var parts = action.split(':')
            api.sendCommand(`set-${parts[0]}`, {[system]: parts[1] === "true"})
        },
        speedDown: (system) => api.sendCommand(`set-${system}-speed`, {speed: getCurrentState(system).currentSpeed - 1 }),
        speedUp: (system) => api.sendCommand(`set-${system}-speed`, {speed: getCurrentState(system).currentSpeed + 1 }),
        powerDown: (system) => api.sendCommand(`set-power`, {[system]: getCurrentState(system).currentPower - 1 }),
        powerUp: (system) => api.sendCommand(`set-power`, {[system]: getCurrentState(system).currentPower + 1 }),
        setThrusterVelocity: data => {
            const parts = data.split(',').map(x => parseInt(x))
            api.sendCommand(`set-thrusters-velocity`, {x: parts[0], y: parts[1], z: parts[2]})
        },
        setThrusterAttitude: (axis, amount) => {
            const state = getCurrentState('thrusters').attitude
            api.sendCommand('set-thrusters-attitude', {...state, [axis]: state[axis] + amount})
        },
        toggleShields: () => {
            var command = getCurrentState('shields').raised ? 'lower-shields' : 'raise-shields'
            api.sendCommand(command, null)
        },
        loadWarhead: () => api.sendCommand(`load-warhead`, {kind: 'torpedo'}),
        fireWarhead: () => api.sendCommand('fire-warhead', {kind: 'torpedo', target: 'none'}),
        chargeEnergyBeam: (bankName) => api.sendCommand('charge-energy-beam', {bankName, newCharge: Math.random()}),
        fireEnergyBeam: (bankName) => api.sendCommand('fire-energy-beam', {bankName, dischargePercent: Math.random(), target: 'asteroid'}),
        newSensorScan: text => api.sendCommand('new-sensor-scan', {scanId: randomId(), scanFor: text}),
        sensorScanResult: text => api.sendCommand('set-sensor-scan-result', {scanId: getCurrentState('sensors').activeScans[0].scanId, result: text}),
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
            var contacts = getCurrentState('sensors').contacts
            var contact = contacts[randomInt(0, contacts.length)]
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
            var requested = getCurrentState('navigation').requestedCourseCalculations[0]
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
        clearEta: () => api.sendCommand('clear-eta', {})
    }
}