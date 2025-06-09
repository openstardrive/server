var systems = {}

var processResults = (results, cursor) => {
    var systemsUpdated = new Set();
    results.forEach(result => {
        if (result.type === 'state-updated') {
            systemsUpdated.add(result.system);
        }
        systems[result.system] = {
            ...systems[result.system],
            ...result.payload
        };
    });
    if (systemsUpdated.has('ftl-engines') || systemsUpdated.has('sublight-engines')) {
        const sublightSpeed = systems['sublight-engines'] ? systems['sublight-engines'].currentSpeed : 0;
        const ftlSpeed = systems['ftl-engines'] ? systems['ftl-engines'].currentSpeed : 0;
        if (ftlSpeed == 0 && sublightSpeed == 0) {
            document.getElementById("speedValue").innerText = "Full Stop";
        }
        else if (ftlSpeed > 0 && sublightSpeed == 0) {
            document.getElementById("speedValue").innerText = `FTL - ${ftlSpeed}`;
        }
        else {
            document.getElementById("speedValue").innerText = `Sublight - ${sublightSpeed}`;
        }
    }
};

var onPollingPaused = () => {
    console.log('Polling paused');
};

var onPollingStarted = () => {
    console.log('Polling started');
};

var init = async () => {
    const api = await startApi(processResults, onPollingStarted, onPollingPaused);

    document.getElementById("changeSpeed").addEventListener("click", () => {
        console.log(api);
        const givenSpeed = document.getElementById("speedOptions").value;
        switch (givenSpeed) {
            case 'fullStop':
                api.sendCommand("set-ftl-engines-speed", { speed: 0 });
                api.sendCommand("set-sublight-engines-speed", { speed: 0 });
                break;
            case 'ftl-1':
                api.sendCommand("set-ftl-engines-speed", { speed: 1 });
                api.sendCommand("set-sublight-engines-speed", { speed: 0 });
                break;
            case 'ftl-2':
                api.sendCommand("set-ftl-engines-speed", { speed: 2 });
                api.sendCommand("set-sublight-engines-speed", { speed: 0 });
                break;
            case 'ftl-3':
                api.sendCommand("set-ftl-engines-speed", { speed: 3 });
                api.sendCommand("set-sublight-engines-speed", { speed: 0 });
                break;
            case 'ftl-4':
                api.sendCommand("set-ftl-engines-speed", { speed: 4 });
                api.sendCommand("set-sublight-engines-speed", { speed: 0 });
                break;
            case 'ftl-5':
                api.sendCommand("set-ftl-engines-speed", { speed: 5 });
                api.sendCommand("set-sublight-engines-speed", { speed: 0 });
                break;
            case 'ftl-6':
                api.sendCommand("set-ftl-engines-speed", { speed: 6 });
                api.sendCommand("set-sublight-engines-speed", { speed: 0 });
                break;
            case 'ftl-7':
                api.sendCommand("set-ftl-engines-speed", { speed: 7 });
                api.sendCommand("set-sublight-engines-speed", { speed: 0 });
                break;
            case 'ftl-8':
                api.sendCommand("set-ftl-engines-speed", { speed: 8 });
                api.sendCommand("set-sublight-engines-speed", { speed: 0 });
                break;
            case 'ftl-9':
                api.sendCommand("set-ftl-engines-speed", { speed: 9 });
                api.sendCommand("set-sublight-engines-speed", { speed: 0 });
                break;
            case 'ftl-10':
                api.sendCommand("set-ftl-engines-speed", { speed: 10 });
                api.sendCommand("set-sublight-engines-speed", { speed: 0 });
                break;
            case 'sublight-1':
                api.sendCommand("set-ftl-engines-speed", { speed: 0 });
                api.sendCommand("set-sublight-engines-speed", { speed: 1 });
                break;
            case 'sublight-2':
                api.sendCommand("set-ftl-engines-speed", { speed: 0 });
                api.sendCommand("set-sublight-engines-speed", { speed: 2 });
                break;
            case 'sublight-3':
                api.sendCommand("set-ftl-engines-speed", { speed: 0 });
                api.sendCommand("set-sublight-engines-speed", { speed: 3 });
                break;
            case 'sublight-4':
                api.sendCommand("set-ftl-engines-speed", { speed: 0 });
                api.sendCommand("set-sublight-engines-speed", { speed: 4 });
                break;
            case 'sublight-5':
                api.sendCommand("set-ftl-engines-speed", { speed: 0 });
                api.sendCommand("set-sublight-engines-speed", { speed: 5 });
                break;
            default:
                console.log('Unknown state.');
        }
    });
}

init();