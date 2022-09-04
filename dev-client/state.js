const initState = () => {
    const currentState = {
        'clients': { clients: [] },
        'debug': { entries: [] },
        'power': { systems: {} }
    }

    const findClient = clientId => {
        return currentState['clients'].clients.find(x => x.clientId === clientId)
    }

    const trackClientLastSeen = event => {
        const client = findClient(event.clientId)
        if (client) {
            client.lastSeen = new Date(event.timestamp)
        }
    }

    const systemProcessors = {
        'default': event => currentState[event.system] = event.payload,
        'clients': event => {
            currentState['clients'] = {
                clients: event.payload.clients.map(c => {
                    const old = findClient(c.clientId) || {}
                    return {...c, lastSeen: old.lastSeen}
                })
            }
        },
        'long-range-comms': event => {
            const old = currentState['long-range-comms'] || {}

            let cyphers = old.cyphers || []
            if (event.payload.lastUpdatedCypher) {
                cyphers = cyphers.filter(c => c.cypherId !== event.payload.lastUpdatedCypher.cypherId)
                cyphers.push(event.payload.lastUpdatedCypher)
            }
            
            let messages = old.messages || []
            if (event.payload.lastUpdatedLongRangeMessage) {
                messages = messages.filter(m => m.messageId !== event.payload.lastUpdatedLongRangeMessage.messageId)
                messages.push(event.payload.lastUpdatedLongRangeMessage)
            }

            currentState['long-range-comms'] = {...event.payload, cyphers, messages}
        },
        'debug': event => {
            currentState['debug'].entries.unshift({...event.payload.lastEntry, timestamp: event.timestamp})
        },
        'power': event => {
            currentState['power'] = {...event.payload, systems: currentState['power'].systems}
        }
    }
    
    const trackPowerUsage = event => {
        if (event.payload && (event.payload.currentPower || event.payload.requiredPower))
        {
            currentState['power'].systems[event.system] = {
                currentPower: event.payload.currentPower,
                requiredPower: event.payload.requiredPower
            }
        }
    }

    return {
        findClient,
        processEvent: event => {
            trackClientLastSeen(event)
            trackPowerUsage(event)
            const processor = systemProcessors[event.system] || systemProcessors['default']
            processor(event)
        },
        getSystemState: system => currentState[system],
        logCurrentState: () => console.log(currentState),
        allSystems: () => Object.keys(currentState)
    }
}