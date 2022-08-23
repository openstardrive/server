const start = async () => {
    console.log('Starting dev client...')
    let allResults = []
    let lastCursor
    let currentStates = {
        'clients': { clients: [] }
    }
    const additionalData = {
        'clients': []
    }
    const renderMap = getRenderFunctions()

    const processResults = (results, cursor) => {
        results.forEach(processResult)
        allResults = allResults.concat(results)
        if (lastCursor != cursor)
        {
            lastCursor = cursor
            document.getElementById('cursor').innerHTML = cursor
        }

        results.map(x => x.system).filter((v, i, a) => a.indexOf(v) === i)
            .filter(x => !!x && x !== 'unknown')
            .concat('clients')
            .forEach(system => {
                if (renderMap[system]) {
                    document.getElementById(system).innerHTML = renderMap[system](currentStates[system], additionalData[system])
                } else {
                    console.log('unknown system:', system, currentStates[system])
                }
            })
    }

    const processResult = (result) => {
        if (result.type == 'unrecognized-command')
        {
            var client = currentStates.clients.clients.find(x => x.clientId == result.clientId)
            console.warn(`Client ${client.name} sent a bad command`, result)
            return
        }
        if (result.type == 'error')
        {
            var client = currentStates.clients.clients.find(x => x.clientId == result.clientId)
            console.error(`Client ${client.name} had an error using ${result.system} system: ${result.payload}`)
            return
        }
        if (result.type !== 'state-updated')
        {
            console.log('unknown result type:', result.type)
            return
        }

        trackClientLastSeen(result)
        
        var system = result.system
        currentStates[system] = result.payload
    }

    const trackClientLastSeen = (result) => {
        const clientId = result.clientId
        const clientsLastSeen = additionalData['clients']
        if (clientId) {
            var client = clientsLastSeen.find(x => x.id == clientId)
            if (!client) {
                client = {id: clientId, lastSeen: new Date(result.timestamp).valueOf()}
                clientsLastSeen.push(client)
            }
            else {
                client.lastSeen = new Date(result.timestamp).valueOf()
            }
        }
    }

    const startPollingButton = document.getElementById('startPolling')
    const pausePollingButton = document.getElementById('pausePolling')

    startPollingButton.onclick = () => api.startPolling()
    pausePollingButton.onclick = () => api.pausePolling()

    const onPollingStarted = () => {
        startPollingButton.disabled = true
        pausePollingButton.disabled = false
    }

    const onPollingPaused = () => {
        startPollingButton.disabled = false
        pausePollingButton.disabled = true
    }

    let api = await startApi(processResults, onPollingStarted, onPollingPaused)

    setTimeout(() => document.getElementById('system-select').innerHTML = renderMap.systems(Object.keys(currentStates)), 3000)

    return getCommands(api, system => currentStates[system])
}
