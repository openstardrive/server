var systems = {}

var previousEnergyFired;

var warheadInventory = [];

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
        const ftlEngines = systems['ftl-engines'];
        const sublightEngines = systems['sublight-engines'];
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

        document.getElementById("ftlCurrentPower").innerText = ftlEngines.currentPower;
        document.getElementById("ftlRequiredPower").innerText = ftlEngines.requiredPower;
        document.getElementById("sublightCurrentPower").innerText = sublightEngines.currentPower;
        document.getElementById("sublightRequiredPower").innerText = sublightEngines.requiredPower;

        if (ftlEngines.damaged === true) {
            document.getElementById("ftlName").style.color = "#f44336";
        }
        else {
            document.getElementById("ftlName").style.color = "white";
        }

        if (sublightEngines.damaged === true) {
            document.getElementById("sublightName").style.color = "#f44336";
        }
        else {
            document.getElementById("sublightName").style.color = "white";
        }

        enginesSpeedCheck();
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

        document.getElementById("sensorCurrentPower").innerText = sensors.currentPower;
        document.getElementById("sensorRequiredPower").innerText = sensors.requiredPower;

        if (sensors.damaged === true) {
            document.getElementById("sensorsName").style.color = "#f44336";
        }
        else {
            document.getElementById("sensorsName").style.color = "white";
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

        document.getElementById("navigationCurrentPower").innerText = navigation.currentPower;
        document.getElementById("navigationRequiredPower").innerText = navigation.requiredPower;

        if (navigation.damaged === true) {
            document.getElementById("navigationName").style.color = "#f44336";
        }
        else {
            document.getElementById("navigationName").style.color = "white";
        }
    }
    if (systemsUpdated.has('energy-beams')) {
        const energyBeams = systems['energy-beams'];
        const forwardBank = energyBeams.banks[0];
        const aftBank = energyBeams.banks[1];

        document.getElementById("forwardCharged").innerText = `${Math.round(forwardBank.percentCharged * 100)}%`;
        document.getElementById("forwardArc").innerText = forwardBank.arcDegrees;
        document.getElementById("forwardGHz").innerText = forwardBank.frequency;

        document.getElementById("aftCharged").innerText = `${Math.round(aftBank.percentCharged * 100)}%`;
        document.getElementById("aftArc").innerText = aftBank.arcDegrees;
        document.getElementById("aftGHz").innerText = aftBank.frequency;

        document.getElementById("energyBeamsCurrentPower").innerText = energyBeams.currentPower;
        document.getElementById("energyBeamsRequiredPower").innerText = energyBeams.requiredPower;

        if (energyBeams.damaged === true) {
            document.getElementById("energyBeamsName").style.color = "#f44336";
        }
        else {
            document.getElementById("energyBeamsName").style.color = "white";
        }

        const lastFired = energyBeams.lastFiredEnergyBeam;
        if (lastFired && (!previousEnergyFired || previousEnergyFired.firedAt !== lastFired.firedAt)) {
            document.getElementById("lastFiredEnergyBeam").innerText = `${lastFired.name} fired at ${lastFired.target ? lastFired.target : "no target"} at ${new Date(lastFired.firedAt).toLocaleTimeString()}`;
            const lastFiredContainer = document.getElementById("lastEnergyBeamFiredContainer");
            lastFiredContainer.classList.add("flash-red");
            previousEnergyFired = lastFired;

            setTimeout(() => {
                lastFiredContainer.classList.remove("flash-red");
            }, 3000);
        }
    }
    if (systemsUpdated.has('warhead-launcher')) {
        const warhead = systems['warhead-launcher'];
        document.getElementById("warheadCurrentPower").innerText = warhead.currentPower;
        document.getElementById("warheadRequiredPower").innerText = warhead.requiredPower;

        if (warhead.damaged === true) {
            document.getElementById("warheadLauncherName").style.color = "#f44336";
        }
        else {
            document.getElementById("warheadLauncherName").style.color = "white";
        }

        const availableTorpedosContainer = document.getElementById("availableTorpedosContainer");
        const inventory = warhead.inventory || [];
        warheadInventory = inventory;
        availableTorpedosContainer.innerHTML = '';
        inventory.forEach(torpedo => {
            const torpedoElement = document.createElement('div');
            torpedoElement.innerHTML = `<span class="typeOfTorpedo">${torpedo.kind} - </span><div id="num${torpedo.kind}Torpedos">${torpedo.number}</div>`;
            availableTorpedosContainer.appendChild(torpedoElement);
        });
    }
    if (systemsUpdated.has('shields')) {
        const shields = systems['shields'];
        document.getElementById("shieldsCurrentPower").innerText = shields.currentPower;
        document.getElementById("shieldsRequiredPower").innerText = shields.requiredPower;

        if (shields.damaged === true) {
            document.getElementById("shieldsName").style.color = "#f44336";
        }
        else {
            document.getElementById("shieldsName").style.color = "white";
        }
    }
    if (systemsUpdated.has('thrusters')) {
        const thrusters = systems['thrusters'];
        document.getElementById("thrustersCurrentPower").innerText = thrusters.currentPower;
        document.getElementById("thrustersRequiredPower").innerText = thrusters.requiredPower;

        if (thrusters.damaged === true) {
            document.getElementById("thrustersName").style.color = "#f44336";
        }
        else {
            document.getElementById("thrustersName").style.color = "white";
        }
    }
    if (systemsUpdated.has('short-range-comms')) {
        const shortRangeComms = systems['short-range-comms'];
        document.getElementById("shortRangeCommsCurrentPower").innerText = shortRangeComms.currentPower;
        document.getElementById("shortRangeCommsRequiredPower").innerText = shortRangeComms.requiredPower;

        if (shortRangeComms.damaged === true) {
            document.getElementById("shortRangeCommsName").style.color = "#f44336";
        }
        else {
            document.getElementById("shortRangeCommsName").style.color = "white";
        }
    }
    if (systemsUpdated.has('long-range-comms')) {
        const longRangeComms = systems['long-range-comms'];
        document.getElementById("longRangeCommsCurrentPower").innerText = longRangeComms.currentPower;
        document.getElementById("longRangeCommsRequiredPower").innerText = longRangeComms.requiredPower;

        if (longRangeComms.damaged === true) {
            document.getElementById("longRangeCommsName").style.color = "#f44336";
        }
        else {
            document.getElementById("longRangeCommsName").style.color = "white";
        }
    }
    if (systemsUpdated.has('alert')) {
        const alertSystem = systems['alert'];
        const alertLevel = alertSystem.current.level;
        const alertName = alertSystem.current.name;
        const alertMessage = `${alertLevel} - ${alertName}` || "No current Level";
        document.getElementById("currentAlert").innerText = alertMessage;
    }

    if(systemsUpdated.has('json-plugin-viewscreen')){
        const viewScreenContainer=document.getElementById("viewscreenDropdownContainer");
        viewScreenContainer.innerHTML="";
        systems["json-plugin-viewscreen"].jsonState.Cards.forEach((item)=>{
            let itemButton=document.createElement("button");

            itemButton.setAttribute("class","alertButton");
            itemButton.innerText=item;
            itemButton.addEventListener("click",()=>{
                api.sendCommand("update-viewscreen-CurrentImage",{
                    value: item
                });
                console.log(item);
            })

            viewScreenContainer.appendChild(itemButton);
        })
    }
};

var onPollingPaused = () => {
    console.log('Polling paused');
};

var onPollingStarted = () => {
    console.log('Polling started');
};

var enginesSpeedCheck = () => {
    const ftlEngines = systems['ftl-engines'];
    const sublightEngines = systems['sublight-engines'];
    const ftlCurrentPower = systems['ftl-engines'].currentPower;
    const sublightCurrentPower = systems['sublight-engines'].currentPower;
    const select = document.getElementById("speedOptions");
    if (ftlCurrentPower < 10 || ftlEngines.damaged === true) {
        for (let i = 1; i <= 10; i++) {
            Array.from(select.options).find(opt => opt.value === `ftl-${i}`).disabled = true;
        }
    }
    else {
        for (let i = 1; i <= 6; i++) {
            Array.from(select.options).find(opt => opt.value === `ftl-${i}`).disabled = false;
        }

        if (ftlCurrentPower < 12 || ftlEngines.damaged === true) {
            for (let i = 7; i <= 10; i++) {
                Array.from(select.options).find(opt => opt.value === `ftl-${i}`).disabled = true;
            }
        }
        else {
            Array.from(select.options).find(opt => opt.value === 'ftl-7').disabled = false;
            if (ftlCurrentPower < 15 || ftlEngines.damaged === true) {
                for (let i = 8; i <= 10; i++) {
                    Array.from(select.options).find(opt => opt.value === `ftl-${i}`).disabled = true;
                }
            }
            else {
                Array.from(select.options).find(opt => opt.value === 'ftl-8').disabled = false;
                if (ftlCurrentPower < 18 || ftlEngines.damaged === true) {
                    Array.from(select.options).find(opt => opt.value === 'ftl-9').disabled = true;
                    Array.from(select.options).find(opt => opt.value === 'ftl-10').disabled = true;
                }
                else {
                    Array.from(select.options).find(opt => opt.value === 'ftl-9').disabled = false;
                    if (ftlCurrentPower < 20 || ftlEngines.damaged === true) {
                        Array.from(select.options).find(opt => opt.value === 'ftl-10').disabled = true;
                    }
                    else {
                        Array.from(select.options).find(opt => opt.value === 'ftl-10').disabled = false;
                    }
                }
            }
        }
    }

    if (sublightCurrentPower < 5 || sublightEngines.damaged === true) {
        for (let i = 1; i <= 5; i++) {
            Array.from(select.options).find(opt => opt.value === `sublight-${i}`).disabled = true;
        }
    }
    else {
        for (let i = 1; i <= 4; i++) {
            Array.from(select.options).find(opt => opt.value === `sublight-${i}`).disabled = false;
        }
        if (sublightCurrentPower < 8 || sublightEngines.damaged === true) {
            Array.from(select.options).find(opt => opt.value === 'sublight-5').disabled = true;
        }
        else {
            Array.from(select.options).find(opt => opt.value === 'sublight-5').disabled = false;
        }
    }
}

var init = async () => {
    const api = await startApi(processResults, onPollingStarted, onPollingPaused);

    function displayAllPowerLevels() {
        const ftlEngines = systems['ftl-engines'];
        const sublightEngines = systems['sublight-engines'];
        const sensors = systems['sensors'];
        const navigation = systems['navigation'];
        const energyBeams = systems['energy-beams'];
        const warhead = systems['warhead-launcher'];
        const shields = systems['shields'];
        const thrusters = systems['thrusters'];
        const shortRangeComms = systems['short-range-comms'];
        const longRangeComms = systems['long-range-comms'];

        document.getElementById("ftlCurrentPower").innerText = ftlEngines ? ftlEngines.currentPower : 0;
        document.getElementById("ftlRequiredPower").innerText = ftlEngines ? ftlEngines.requiredPower : 0;
        document.getElementById("sublightCurrentPower").innerText = sublightEngines ? sublightEngines.currentPower : 0;
        document.getElementById("sublightRequiredPower").innerText = sublightEngines ? sublightEngines.requiredPower : 0;
        document.getElementById("sensorCurrentPower").innerText = sensors ? sensors.currentPower : 0;
        document.getElementById("sensorRequiredPower").innerText = sensors ? sensors.requiredPower : 0;
        document.getElementById("navigationCurrentPower").innerText = navigation ? navigation.currentPower : 0;
        document.getElementById("navigationRequiredPower").innerText = navigation ? navigation.requiredPower : 0;
        document.getElementById("energyBeamsCurrentPower").innerText = energyBeams ? energyBeams.currentPower : 0;
        document.getElementById("energyBeamsRequiredPower").innerText = energyBeams ? energyBeams.requiredPower : 0;
        document.getElementById("warheadCurrentPower").innerText = warhead ? warhead.currentPower : 0;
        document.getElementById("warheadRequiredPower").innerText = warhead ? warhead.requiredPower : 0;
        document.getElementById("shieldsCurrentPower").innerText = shields ? shields.currentPower : 0;
        document.getElementById("shieldsRequiredPower").innerText = shields ? shields.requiredPower : 0;
        document.getElementById("thrustersCurrentPower").innerText = thrusters ? thrusters.currentPower : 0;
        document.getElementById("thrustersRequiredPower").innerText = thrusters ? thrusters.requiredPower : 0;
        document.getElementById("shortRangeCommsCurrentPower").innerText = shortRangeComms ? shortRangeComms.currentPower : 0;
        document.getElementById("shortRangeCommsRequiredPower").innerText = shortRangeComms ? shortRangeComms.requiredPower : 0;
        document.getElementById("longRangeCommsCurrentPower").innerText = longRangeComms ? longRangeComms.currentPower : 0;
        document.getElementById("longRangeCommsRequiredPower").innerText = longRangeComms ? longRangeComms.requiredPower : 0;
    }

    displayAllPowerLevels();

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

        api.sendCommand("set-course", {
            courseId: courseId,
            destination: destination,
            coordinates: coordinates,
            eta: etaPayload
        });
        api.sendCommand("cancel-course-calculation",{
            courseId: courseId
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

    document.getElementById("ftlPowerUp").addEventListener("click", () => {
        const ftlEngines = systems['ftl-engines'];
        const currentPower = ftlEngines.currentPower;
        const increasedPower = currentPower + 1;
        api.sendCommand(`set-power`, {'ftl-engines': increasedPower});
    });

    document.getElementById("ftlPowerDown").addEventListener("click", () => {
        const ftlEngines = systems['ftl-engines'];
        const currentPower = ftlEngines.currentPower;
        if (currentPower == 0) {
            alert("FTL engines power is already at minimum.");
            return;
        }
        const decreasedPower = currentPower - 1;
        api.sendCommand(`set-power`, {'ftl-engines': decreasedPower});
    });

    document.getElementById("sublightPowerUp").addEventListener("click", () => {
        const sublightEngines = systems['sublight-engines'];
        const currentPower = sublightEngines.currentPower;
        const increasedPower = currentPower + 1;
        api.sendCommand(`set-power`, {'sublight-engines': increasedPower});
    });

    document.getElementById("sublightPowerDown").addEventListener("click", () => {
        const sublightEngines = systems['sublight-engines'];
        const currentPower = sublightEngines.currentPower;
        if (currentPower == 0) {
            alert("FTL engines power is already at minimum.");
            return;
        }
        const decreasedPower = currentPower - 1;
        api.sendCommand(`set-power`, {'sublight-engines': decreasedPower});
    });

    document.getElementById("navigationPowerUp").addEventListener("click", () => {
        const navigation = systems['navigation'];
        const currentPower = navigation.currentPower;
        const increasedPower = currentPower + 1;
        api.sendCommand(`set-power`, {'navigation': increasedPower});
    });

    document.getElementById("navigationPowerDown").addEventListener("click", () => {
        const navigation = systems['navigation'];
        const currentPower = navigation.currentPower;
        if (currentPower == 0) {
            alert("Navigation power is already at minimum.");
            return;
        }
        const decreasedPower = currentPower - 1;
        api.sendCommand(`set-power`, {'navigation': decreasedPower});
    });

    document.getElementById("sensorPowerUp").addEventListener("click", () => {
        const sensors = systems['sensors'];
        const currentPower = sensors.currentPower;
        const increasedPower = currentPower + 1;
        api.sendCommand(`set-power`, {'sensors': increasedPower});
    });

    document.getElementById("sensorPowerDown").addEventListener("click", () => {
        const sensors = systems['sensors'];
        const currentPower = sensors.currentPower;
        if (currentPower == 0) {
            alert("Sensors power is already at minimum.");
            return;
        }
        const decreasedPower = currentPower - 1;
        api.sendCommand(`set-power`, {'sensors': decreasedPower});
    });

    document.getElementById("energyBeamsPowerUp").addEventListener("click", () => {
        const energyBeams = systems['energy-beams'];
        const currentPower = energyBeams.currentPower;
        const increasedPower = currentPower + 1;
        api.sendCommand(`set-power`, {'energy-beams': increasedPower});
    });

    document.getElementById("energyBeamsPowerDown").addEventListener("click", () => {
        const energyBeams = systems['energy-beams'];
        const currentPower = energyBeams.currentPower;
        if (currentPower == 0) {
            alert("Energy beams power is already at minimum.");
            return;
        }
        const decreasedPower = currentPower - 1;
        api.sendCommand(`set-power`, {'energy-beams': decreasedPower});
    });

    document.getElementById("warheadPowerUp").addEventListener("click", () => {
        const warhead = systems['warhead-launcher'];
        const currentPower = warhead.currentPower;
        const increasedPower = currentPower + 1;
        api.sendCommand(`set-power`, {'warhead': increasedPower});
    });

    document.getElementById("warheadPowerDown").addEventListener("click", () => {
        const warhead = systems['warhead-launcher'];
        const currentPower = warhead.currentPower;
        if (currentPower == 0) {
            alert("Warhead power is already at minimum.");
            return;
        }
        const decreasedPower = currentPower - 1;
        api.sendCommand(`set-power`, {'warhead': decreasedPower});
    });

    document.getElementById("shieldsPowerUp").addEventListener("click", () => {
        const shields = systems['shields'];
        const currentPower = shields.currentPower;
        const increasedPower = currentPower + 1;
        api.sendCommand(`set-power`, {'shields': increasedPower});
    });

    document.getElementById("shieldsPowerDown").addEventListener("click", () => {
        const shields = systems['shields'];
        const currentPower = shields.currentPower;
        if (currentPower == 0) {
            alert("Shields power is already at minimum.");
            return;
        }
        const decreasedPower = currentPower - 1;
        api.sendCommand(`set-power`, {'shields': decreasedPower});
    });

    document.getElementById("thrustersPowerUp").addEventListener("click", () => {
        const thrusters = systems['thrusters'];
        const currentPower = thrusters.currentPower;
        const increasedPower = currentPower + 1;
        api.sendCommand(`set-power`, {'thrusters': increasedPower});
    });

    document.getElementById("thrustersPowerDown").addEventListener("click", () => {
        const thrusters = systems['thrusters'];
        const currentPower = thrusters.currentPower;
        if (currentPower == 0) {
            alert("Thrusters power is already at minimum.");
            return;
        }
        const decreasedPower = currentPower - 1;
        api.sendCommand(`set-power`, {'thrusters': decreasedPower});
    });

    document.getElementById("shortRangeCommsPowerUp").addEventListener("click", () => {
        const shortRangeComms = systems['short-range-comms'];
        const currentPower = shortRangeComms.currentPower;
        const increasedPower = currentPower + 1;
        api.sendCommand(`set-power`, {'short-range-comms': increasedPower});
    });

    document.getElementById("shortRangeCommsPowerDown").addEventListener("click", () => {
        const shortRangeComms = systems['short-range-comms'];
        const currentPower = shortRangeComms.currentPower;
        if (currentPower == 0) {
            alert("Short range comms power is already at minimum.");
            return;
        }
        const decreasedPower = currentPower - 1;
        api.sendCommand(`set-power`, {'short-range-comms': decreasedPower});
    });

    document.getElementById("longRangeCommsPowerUp").addEventListener("click", () => {
        const longRangeComms = systems['long-range-comms'];
        const currentPower = longRangeComms.currentPower;
        const increasedPower = currentPower + 1;
        api.sendCommand(`set-power`, {'long-range-comms': increasedPower});
    });

    document.getElementById("longRangeCommsPowerDown").addEventListener("click", () => {
        const longRangeComms = systems['long-range-comms'];
        const currentPower = longRangeComms.currentPower;
        if (currentPower == 0) {
            alert("Long range comms power is already at minimum.");
            return;
        }
        const decreasedPower = currentPower - 1;
        api.sendCommand(`set-power`, {'long-range-comms': decreasedPower});
    });

    document.getElementById("navigationRepairSystem").addEventListener("click", () => {
        const navigation = systems['navigation'];
        if (navigation.damaged) {
            api.sendCommand("set-damaged", {'navigation': false});
        } else {
            alert("Navigation system is not damaged.");
        }
    });

    document.getElementById("navigationDamageSystem").addEventListener("click", () => {
        const navigation = systems['navigation'];
        if (!navigation.damaged) {
            api.sendCommand("set-damaged", {'navigation': true});
        } else {
            alert("Navigation system is already damaged.");
        }
    });

    document.getElementById("sensorRepairSystem").addEventListener("click", () => {
        const sensors = systems['sensors'];
        if (sensors.damaged) {
            api.sendCommand("set-damaged", {'sensors': false});
        } else {
            alert("Sensors system is not damaged.");
        }
    });

    document.getElementById("sensorDamageSystem").addEventListener("click", () => {
        const sensors = systems['sensors'];
        if (!sensors.damaged) {
            api.sendCommand("set-damaged", {'sensors': true});
        } else {
            alert("Sensors system is already damaged.");
        }
    });

    document.getElementById("ftlRepairSystem").addEventListener("click", () => {
        const ftlEngines = systems['ftl-engines'];
        if (ftlEngines.damaged) {
            api.sendCommand("set-damaged", {'ftl-engines': false});
        } else {
            alert("FTL engines system is not damaged.");
        }
    });

    document.getElementById("ftlDamageSystem").addEventListener("click", () => {
        const ftlEngines = systems['ftl-engines'];
        if (!ftlEngines.damaged) {
            api.sendCommand("set-damaged", {'ftl-engines': true});
        } else {
            alert("FTL engines system is already damaged.");
        }
    });

    document.getElementById("sublightRepairSystem").addEventListener("click", () => {
        const sublightEngines = systems['sublight-engines'];
        if (sublightEngines.damaged) {
            api.sendCommand("set-damaged", {'sublight-engines': false});
        } else {
            alert("Sublight engines system is not damaged.");
        }
    });

    document.getElementById("sublightDamageSystem").addEventListener("click", () => {
        const sublightEngines = systems['sublight-engines'];
        if (!sublightEngines.damaged) {
            api.sendCommand("set-damaged", {'sublight-engines': true});
        } else {
            alert("Sublight engines system is already damaged.");
        }
    });

    document.getElementById("energyBeamsRepairSystem").addEventListener("click", () => {
        const energyBeams = systems['energy-beams'];
        if (energyBeams.damaged) {
            api.sendCommand("set-damaged", {'energy-beams': false});
        } else {
            alert("Energy beams system is not damaged.");
        }
    });

    document.getElementById("energyBeamsDamageSystem").addEventListener("click", () => {
        const energyBeams = systems['energy-beams'];
        if (!energyBeams.damaged) {
            api.sendCommand("set-damaged", {'energy-beams': true});
        } else {
            alert("Energy beams system is already damaged.");
        }
    });

    document.getElementById("warheadRepairSystem").addEventListener("click", () => {
        const warhead = systems['warhead-launcher'];
        if (warhead.damaged) {
            api.sendCommand("set-damaged", {'warhead-launcher': false});
        } else {
            alert("Warhead system is not damaged.");
        }
    });

    document.getElementById("warheadDamageSystem").addEventListener("click", () => {
        const warhead = systems['warhead-launcher'];
        if (!warhead.damaged) {
            api.sendCommand("set-damaged", {'warhead-launcher': true});
        } else {
            alert("Warhead system is already damaged.");
        }
    });

    document.getElementById("shieldsRepairSystem").addEventListener("click", () => {
        const shields = systems['shields'];
        if (shields.damaged) {
            api.sendCommand("set-damaged", {'shields': false});
        } else {
            alert("Shields system is not damaged.");
        }
    });

    document.getElementById("shieldsDamageSystem").addEventListener("click", () => {
        const shields = systems['shields'];
        if (!shields.damaged) {
            api.sendCommand("set-damaged", {'shields': true});
        } else {
            alert("Shields system is already damaged.");
        }
    });

    document.getElementById("thrustersRepairSystem").addEventListener("click", () => {
        const thrusters = systems['thrusters'];
        if (thrusters.damaged) {
            api.sendCommand("set-damaged", {'thrusters': false});
        } else {
            alert("Thrusters system is not damaged.");
        }
    });

    document.getElementById("thrustersDamageSystem").addEventListener("click", () => {
        const thrusters = systems['thrusters'];
        if (!thrusters.damaged) {
            api.sendCommand("set-damaged", {'thrusters': true});
        } else {
            alert("Thrusters system is already damaged.");
        }
    });

    document.getElementById("shortRangeCommsRepairSystem").addEventListener("click", () => {
        const shortRangeComms = systems['short-range-comms'];
        if (shortRangeComms.damaged) {
            api.sendCommand("set-damaged", {'short-range-comms': false});
        } else {
            alert("Short range comms system is not damaged.");
        }
    });

    document.getElementById("shortRangeCommsDamageSystem").addEventListener("click", () => {
        const shortRangeComms = systems['short-range-comms'];
        if (!shortRangeComms.damaged) {
            api.sendCommand("set-damaged", {'short-range-comms': true});
        } else {
            alert("Short range comms system is already damaged.");
        }
    });

    document.getElementById("longRangeCommsRepairSystem").addEventListener("click", () => {
        const longRangeComms = systems['long-range-comms'];
        if (longRangeComms.damaged) {
            api.sendCommand("set-damaged", {'long-range-comms': false});
        } else {
            alert("Long range comms system is not damaged.");
        }
    });

    document.getElementById("longRangeCommsDamageSystem").addEventListener("click", () => {
        const longRangeComms = systems['long-range-comms'];
        if (!longRangeComms.damaged) {
            api.sendCommand("set-damaged", {'long-range-comms': true});
        } else {
            alert("Long range comms system is already damaged.");
        }
    });

    document.getElementById("alert1").addEventListener("click", () => {
        const alertSystem = systems['alert'];
        api.sendCommand("set-alert-level", {level: 1});
    });

    document.getElementById("alert2").addEventListener("click", () => {
        const alertSystem = systems['alert'];
        api.sendCommand("set-alert-level", {level: 2});
    });

    document.getElementById("alert3").addEventListener("click", () => {
        const alertSystem = systems['alert'];
        api.sendCommand("set-alert-level", {level: 3});
    });

    document.getElementById("addTorpedoButton").addEventListener("click", () => {
        const torpedoType = document.getElementById("torpedoType").value;
        const numTorpedos = document.getElementById("numTorpedos").value;
        if (!torpedoType || !numTorpedos) {
            alert("Please select a torpedo type and enter the number of torpedos.");
            return;
        }

        const inventoryAddition = {
            kind: torpedoType,
            number: parseInt(numTorpedos)
        }

        warheadInventory.push(inventoryAddition);

        api.sendCommand("set-warhead-inventory", {
            inventory: warheadInventory
        });
    });


    return api;
}


let api;

async function setupApi(){
    api=await init();
}

setupApi();
