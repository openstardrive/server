console.log("index.js loaded");
var systems = {

}

var processResults = (results, cursor) => {
    var systemsUpdated = new Set();
    results.forEach(result => {
        //todo: we only want to add the systems updated if the result type is a system update => if result.type == 'state-updated'
        systemsUpdated.add(result.system);
        systems[result.system] = {
            ...systems[result.system],
            ...result.payload
        };
    });
    if (systemsUpdated.has('ftl-engines')) {
        document.getElementById("speedValue").innerText = systems['ftl-engines'].currentSpeed || 'no speed found';
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
        api.sendCommand("set-ftl-engines-speed", {
            speed: 3
        });
    });
}

init();