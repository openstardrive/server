const startApi = async (processResults, onPollingStarted, onPollingPaused) => {
    const apiBaseUrl = 'http://localhost:5002'
    
    let isInPoll = false;
    let cursor = 0;

    let clientId = window.localStorage.getItem('clientId')
    let clientSecret = window.localStorage.getItem('clientSecret')

    const tryFetch = async (url, data) => {
        try {
            return await fetch(url, data)
        }
        catch (error) {
            console.error('Unexpected error', error)
            return {status: 0}
        }
    }

    const registerClient = async () => {
        console.log('Registering client...')
        const response = await tryFetch(`${apiBaseUrl}/register`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json;charset=utf-8'
            },
            body: JSON.stringify({name: 'dev-client', clientType: 'development'})
        });
    
        if (response.status == 200) {
            const json = await response.json()
            window.localStorage.setItem('clientId', json.clientId)
            window.localStorage.setItem('clientSecret', json.clientSecret)
            clientId = json.clientId
            clientSecret = json.clientSecret
        }
    }

    const pollApi = async () => {
        if (isInPoll) {
            return
        }
        isInPoll = true
        const response = await tryFetch(`${apiBaseUrl}/results?cursor=${cursor}`)
        isInPoll = false

        if (response.status == 200) {
            const json = await response.json()
            const diff = json.nextCursor - cursor
            cursor = json.nextCursor
        
            processResults(json.results, cursor)

            if (diff > 90) {
                pollApi()
            }
        } else {
            console.error('Unexpected response from server', response)
            pausePolling()
        }
    }
    
    const startPolling = () => {
        const seconds = parseInt(document.getElementById('pollingSeconds').value)
        pollInterval = setInterval(pollApi, seconds * 1000)
        onPollingStarted()
    }
    
    const pausePolling = () => {
        clearInterval(pollInterval)
        onPollingPaused()
    }

    const sendCommand = async (command, payload) => {
        const response = await tryFetch(`${apiBaseUrl}/command`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json;charset=utf-8'
            },
            body: JSON.stringify({
                clientSecret: clientSecret,
                type: command,
                payload
            })
        });

        if (response.status == 200) {
            const json = await response.json()
            return json.commandId
        }
        if (response.status == 401)
        {
            registerClient()
            return
        }

        console.error('Unexpected failure', response)
    }

    if (!clientId || !clientSecret)
    {
        await registerClient()
    }
    startPolling()

    return {
        startPolling,
        pausePolling,
        sendCommand
    }
}





