const getRenderFunctions = () => {
    const renderSystems = systems => {
        systems.sort()
        return systems.map(x => `<option value="${x}">${x}</option>`).join('')
    }

    const renderClients = (data) => {
        const now = new Date().valueOf()
        return data.clients.map(x => {
            const clientId = x.clientId
            const lastSeen = x.lastSeen || 1200000
            const elapsedSeconds = (now - lastSeen) / 1000
            let color = 'active'
            if (elapsedSeconds > 60) {
                color = 'inactive'
            }
            if (elapsedSeconds > 300) {
                color = 'gone'
            }
            return `<span class="client ${color}">${x.name}</span>`
        }).join('')
    }

    const renderTime = timestamp => {
        const x = new Date(timestamp)
        const pad = i => i < 10 ? '0' + i : i
        return `${pad(x.getHours())}:${pad(x.getMinutes())}:${pad(x.getSeconds())}`
    }

    const renderDamagedAndDisabled = data => {
        var damaged = data.damaged ? '<span class="tag damaged">Damaged</span>' : ''
        var disabled = data.disabled ? '<span class="tag disabled">Disabled</span>' : ''
        return `<div>${damaged}${disabled}</div>`
    }

    const renderPower = data => {
        return `Power: ${data.currentPower}/${data.requiredPower}`
    }

    const renderEngines = data => {
        return renderDamagedAndDisabled(data) +
            `Speed: ${data.currentSpeed}/${data.speedConfig.maxSpeed}` +
            `, Heat: ${data.currentHeat}/${data.heatConfig.maxHeat}` +
            `, ${renderPower(data)}`
    }

    const renderThrusters = data => {
        return renderDamagedAndDisabled(data) +
            `Velocity: X${data.velocity.x} Y${data.velocity.y} Z${data.velocity.z}` +
            `, Attitude: Y${data.attitude.yaw} P${data.attitude.pitch} R${data.attitude.roll}` +
            `, ${renderPower(data)}`
    }

    const renderShields = data => {
        var state = data.raised ? 'up' : 'down'
        var strengths = ['forward', 'starboard', 'aft', 'port']
            .map(x => x + ' ' + (data.sectionStrengths[x + 'Percent'] * 100) + '%')
            .join(', ')
        return renderDamagedAndDisabled(data) +
            `State: ${state}, ${data.modulationFrequency} GHz` +
            `, Strength: ${strengths}` +
            `, ${renderPower(data)}`
    }

    const renderWarheadLauncher = data => {
        var fired = data.lastFiredWarhead
        return renderDamagedAndDisabled(data) +
            `# of Launchers: ${data.numberOfLaunchers}` +
            `, Loaded: [${data.loaded.join(', ')}]` +
            `<br>Inventory:  ${data.inventory.map(x => `${x.kind} - ${x.number}`).join(', ')}` +
            '<br>Last fired: ' + (fired ? `${fired.kind} at ${fired.target} [${renderTime(fired.firedAt)}]` : 'none') +
            `, ${renderPower(data)}`
    }

    const toPercent = number => (Math.round(number * 100)) + '%'

    const renderEnergyBeams = data => {
        var fired = data.lastFiredEnergyBeam
        return renderDamagedAndDisabled(data) +
            `Banks: ${data.banks.map(x => `[${x.name} charged: ${toPercent(x.percentCharged)}, arc: ${x.arcDegrees}, ${x.frequency} GHz]`).join(' ')}` +
            '<br>Last fired: ' + (fired ? `${fired.name} beams ${toPercent(fired.percentDischarged)} at ${fired.target} [${renderTime(fired.firedAt)}]` : 'none') +
            `, ${renderPower(data)}`
    }

    const roundToPrecision = (value, precision) => {
        const exponent = Math.pow(10, precision);
        return Math.round(value * exponent) / exponent;
    }

    const renderPosition = p => {
        return `${roundToPrecision(p.x, 5)},${roundToPrecision(p.y, 5)},${roundToPrecision(p.z, 5)}`
    }

    const renderContact = c => {
        return `<div class="contact">[${c.contactId}] ${c.name} (${c.icon}) ${renderPosition(c.position)}` +
        `<br/>${c.destinations.map(d => `=> ${renderPosition(d.position)} (${d.remainingMilliseconds})`).join(' ')}</div>`
    }

    const renderSensors = data => {
        var last = data.lastUpdatedScan
        return renderDamagedAndDisabled(data) +
            `Active scans: ${data.activeScans.map(x => `<div>[${x.scanId} at ${renderTime(x.lastUpdated)}] ${x.scanFor}</div>`).join('')}` +
            `<br/>Last Updated Scan: `+ (last ? `[${last.scanId} at ${renderTime(last.lastUpdated)}] ${last.scanFor} : ${last.result}` : 'none') +
            `<br/>Contacts: ${data.contacts.map(x => renderContact(x)).join('')}` +
            renderPower(data)
    }

    const renderEtaMs = ms => {
        const pad = (i) => i < 10 ? `0${i}` : i
        var min = Math.floor(ms / 60000)
        var sec = Math.floor((ms % 60000) / 1000)
        var remain = ms % 1000
        return `${min}:${pad(sec)}.${remain}`
    }

    const renderCourse = (c) => c
        ? `[${c.courseId}] ${c.destination} since ${renderTime(c.courseSetAt)}` +
            (c.eta ? `<br/>ETA (${c.eta.engineSystem}):<br/>${c.eta.travelTimes.map(x => `[${x.speed}] ${renderEtaMs(x.arriveInMilliseconds)}`).join('<br/>')}` : '')
        : 'none'

    const renderNavigation = data => {
        return renderDamagedAndDisabled(data) +
            `Calculating courses for: ${data.requestedCourseCalculations.map(x => `[${x.courseId}] ${x.destination}`).join(', ')}` +
            `<br/><br/>Calculated courses: ${data.calculatedCourses.map(x => `<br/>[${x.courseId}] ${x.destination}: ${renderPosition(x.coordinates)}`).join('')}` +
            `<br/><br/>Current course: ${renderCourse(data.currentCourse)}` +
            `<br/><br/>${renderPower(data)}`
    }

    const renderCypher = cypher => {
        if (!cypher) return 'null'
        return `[${cypher.cypherId}] ${cypher.name} (${cypher.description})` +
            `<br/>Encoding: ${cypher.encodeSubstitutions.map(x => `${x.change}->${x.to}`).join(', ')}` +
            `<br/>Decoding: ${cypher.decodeSubstitutions.map(x => `${x.change}->${x.to}`).join(', ')}`
    }

    const renderMessage = (message, cyphers) => {
        if (!message) return 'null'
        let encoded = ''
        const cypher = cyphers.find(x => x.cypherId === message.cypherId)
        if (cypher) {
            encoded = ' | ' + message.message
                .split('')
                .map(letter => {
                    const encode = cypher.encodeSubstitutions.find(x => x.change === letter) || {to: letter}
                    const decode = cypher.decodeSubstitutions.find(x => x.change === encode.to) || {to: encode.to}
                    return decode.to
                })
                .join('')
        }

        return `<br/>[${message.messageId}] ${message.sender} -> ${message.recipient}` +
            `: ${message.message}${encoded}`
    }

    const renderLongRangeComms = data => {
        return renderDamagedAndDisabled(data) +
            `Cyphers: ${data.cyphers.map(x => `<br/>[${x.cypherId}] ${x.name} (${toPercent(x.percentDecoded)} decoded)`).join('')}` +
            `<br/><br/>Last updated cypher: ${renderCypher(data.lastUpdatedCypher)}` +
            `<br/><br/>Messages: ${data.messages.map(x => renderMessage(x, data.cyphers)).join('')}` +
            `<br/><br/>Last updated message: ${renderMessage(data.lastUpdatedLongRangeMessage, data.cyphers)}` +
            `<br/><br/>${renderPower(data)}`
    }

    return {
        'systems': renderSystems,
        'clients': renderClients,
        'ftl-engines': renderEngines,
        'sublight-engines': renderEngines,
        'thrusters': renderThrusters,
        'shields': renderShields,
        'warhead-launcher': renderWarheadLauncher,
        'energy-beams': renderEnergyBeams,
        'sensors': renderSensors,
        'navigation': renderNavigation,
        'long-range-comms': renderLongRangeComms
    }
}
