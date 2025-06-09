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
    if (systemsUpdated.has('sensors')) {
        const sensors = systems['sensors'];
        const activeScans = sensors ? sensors.activeScans : [];
        if (activeScans.length > 0) {
            document.getElementById("incomingScanContainer").style.backgroundColor = "#f44336";
            document.getElementById("incomingScanContainer").innerText = activeScans[0].scanFor;
        }
        else if (activeScans.length === 0) {
            document.getElementById("incomingScanContainer").style.backgroundColor = "#2c5364";
            document.getElementById("incomingScanContainer").innerText = "No active scans";
        }
    }
    if (systemsUpdated.has('navigation')) {
        const navigation = systems['navigation'];
        document.getElementById("courseValue").innerText = navigation.currentCourse ? navigation.currentCourse.destination : "None";
        const etaMilliseconds = navigation.currentCourse && navigation.currentCourse.eta ? navigation.currentCourse.eta.travelTimes[0].arriveInMilliseconds : null;
        if (etaMilliseconds !== null) {
            const seconds = Math.floor((etaMilliseconds / 1000) % 60);
            const minutes = Math.floor((etaMilliseconds / (1000 * 60)) % 60);
            const hours = Math.floor((etaMilliseconds / (1000 * 60 * 60)) % 24);
            document.getElementById("etaValue").innerText = `${hours}h ${minutes}m ${seconds}s`;
        } else {
            document.getElementById("etaValue").innerText = "No ETA available";
        }
        const requestedCourses = navigation.requestedCourseCalculations ? navigation.requestedCourseCalculations : [];
        if (requestedCourses.length > 0) {
            document.getElementById("requestedCourse").style.backgroundColor = "#f44336";
            document.getElementById("requestedCourse").innerText = requestedCourses[0].destination;
        }
        else {
            document.getElementById("requestedCourse").style.backgroundColor = "#2c5364";
            document.getElementById("requestedCourse").innerText = "None";
        }

        console.log(navigation);
        document.getElementById("navgiationRequiredPower").innerHTML = navigation.requiredPower;
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

    document.getElementById("sendResponse").addEventListener("click", () => {
        const sensors = systems['sensors'];
        const givenResponse = document.getElementById("scanResponse").value;
        if (sensors.activeScans.length > 0) {
            const scan = sensors.activeScans[0];
            const scanID = scan.scanId;
            api.sendCommand("set-sensor-scan-result", {
                scanId: scanID,
                result: givenResponse
            });
            document.getElementById("incomingScanContainer").style.backgroundColor = "#2c5364";
        }
    });

    document.getElementById("setCourse").addEventListener("click", () => {
        const navigation = systems['navigation'];
        const x = document.getElementById("xCoordinate").value;
        const y = document.getElementById("yCoordinate").value;
        const z = document.getElementById("zCoordinate").value;
        if (!x || !y || !z) {
            alert("Please enter coordinates.");
            return;
        }

        const requestedCourses = navigation.requestedCourseCalculations || [];
        if (requestedCourses.length == 0) {
            alert("No course calculations in progress.");
            return;
        }

        const course = requestedCourses[0];
        const destination = course.destination;
        const courseId = course.courseId;

        const coordinates = {
            x: parseFloat(x),
            y: parseFloat(y),
            z: parseFloat(z)
        };

        let engineSystems;
        let arriveInMilliseconds;

        if (document.getElementById("courseEtaInput").value === '') {
            arriveInMilliseconds = 5 * 60000;
        }
        else {
            arriveInMilliseconds = parseFloat(document.getElementById("courseEtaInput").value * 60000);
        }

        if (document.getElementById("navigationEtaEngineOptions").value == 'ftl') {
            engineSystems = 'ftl-engines';
        }
        else {
            engineSystems = 'sublight-engines';
        }

        const etaPayload = {
            engineSystem: engineSystems,
            speed: 1,
            arriveInMilliseconds: arriveInMilliseconds
        };

        api.sendCommand("course-calculated", {
            courseId: courseId,
            destination: destination,
            coordinates: coordinates,
            eta: etaPayload
        });
        document.getElementById("requestedCourse").style.backgroundColor = "#2c5364";
    });

    document.getElementById("randomCourse").addEventListener("click", () => {
        var randomX = (Math.random() * 1000).toFixed(3);
        var randomY = (Math.random() * 1000).toFixed(3);
        var randomZ = (Math.random() * 1000).toFixed(3);
        document.getElementById("xCoordinate").value = randomX;
        document.getElementById("yCoordinate").value = randomY;
        document.getElementById("zCoordinate").value = randomZ;
    });

    document.getElementById("clearCourse").addEventListener("click", () => {
        const navigation = systems['navigation'];
        api.sendCommand("clear-course", navigation);
    });

    document.getElementById("clearEta").addEventListener("click", () => {
        const navigation = systems['navigation'];
        api.sendCommand("clear-eta", navigation);
    });

    document.getElementById("setEta").addEventListener("click", () => {
        const hours = document.getElementById("hours").value;
        const minutes = document.getElementById("minutes").value;
        const seconds = document.getElementById("seconds").value;
        if (!hours && !minutes && !seconds) {
            alert("Please enter at least one time unit.");
            return;
        }
        const totalMilliseconds = (parseInt(hours) || 0) * 3600000 + (parseInt(minutes) || 0) * 60000 + (parseInt(seconds) || 0) * 1000;
        
        var engineSystems;
        if (document.getElementById("etaEngineOptions").value == 'ftl') {
            engineSystems = 'ftl-engines';
        }
        else {
            engineSystems = 'sublight-engines';
        }

        api.sendCommand("update-eta" , {
            engineSystem: engineSystems,
            speed: 1,
            arriveInMilliseconds: totalMilliseconds
        })
    });
}

init();