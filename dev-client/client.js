const start = async () => {
    console.log('Starting dev client...')
    let lastCursor

    const renderMap = getRenderFunctions()
    const state = initState()

    const processResults = (results, cursor) => {
        results.forEach(processResult)
        if (lastCursor != cursor)
        {
            lastCursor = cursor
            document.getElementById('cursor').innerHTML = cursor
        }

        results.map(x => x.system).filter((v, i, a) => a.indexOf(v) === i)
            .filter(x => !!x && x !== 'unknown')
            .concat('clients', 'power')
            .forEach(system => {
                if (renderMap[system]) {
                    document.getElementById(system).innerHTML = renderMap[system](state.getSystemState(system))
                } else {
                    console.log('unknown system:', system, state.getSystemState(system))
                }
            })
    }

    const processResult = (result) => {
        if (result.type == 'unrecognized-command') {
            const client = state.findClient(result.clientId) || {name: 'UNKNOWN'}
            console.warn(`Client ${client.name} sent a bad command`, result)
            return
        }
        if (result.type == 'error') {
            const client = state.findClient(result.clientId) || {name: 'UNKNOWN'}
            console.warn(`Client ${client.name} had an error using ${result.system} system: ${result.payload}`)
            return
        }
        if (result.type !== 'state-updated') {
            console.log('unknown result type:', result.type)
            return
        }

        state.processEvent(result)
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

    setTimeout(() => document.getElementById('system-select').innerHTML = renderMap.systems(state.allSystems()), 3000)

    return getCommands(api, state)
}
