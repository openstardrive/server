const damageChannel = new BroadcastChannel('damage-channel');

var currentViewedReport;
var currentSoftwarePanelSteps = [
    {
        systemName: 'warpEngines',
        stepName: 'warpEngineStep2',
        completionSquare: 'warpEnginesStep2Completion',
        switches: [true, false, true, false, false, true, false, false, true, false, false, true]
    }, {
        systemName: 'warpEngines',
        stepName: 'warpEngineStep4',
        completionSquare: 'warpEnginesStep4Completion',
        signalCalibration: {
            rcxI: {active: true, minValue: 10, maxValue: 20},
            gvdI: {active: true, minValue: 10, maxValue: 20},
            lecI: {active: true, minValue: 80, maxValue: 100},
            rcxE: {active: true, minValue: 10, maxValue: 30},
            lecE: {active: true, minValue: 10, maxValue: 30},
            gvdE: {active: true, minValue: 10, maxValue: 30}
        }
    }, {
        systemName: 'warpEngines',
        stepName: 'warpEngineStep5',
        completionSquare: 'warpEnginesStep5Completion',
        capacitorConduits: {
            alpha: {minValue: 35, maxValue: 35},
            bravo: {minValue: 14, maxValue: 14}
        }
    }
];

damageChannel.onmessage = (e) => {
    if (e.data.type === 'updateDamageReport') {
        if (e.data.switches) {
            var possibleSteps = [];
            currentSoftwarePanelSteps.forEach((step, index) => {
                if (step.switches) {
                    possibleSteps.push(step);
                }
            });
            possibleSteps.forEach((step) => {
                var matches = false;
                for (let i = 0; i < step.switches.length; i++) {
                    if (step.switches[i] !== e.data.switches[i]) {
                        matches = false;
                        break;
                    }
                    matches = true;
                }
                if (matches) {
                    const completionSquare = document.getElementById(step.completionSquare);
                    if(completionSquare.classList.contains('incomplete')) {
                        toggleComplete(completionSquare);
                    }
                }
            });
        }
        else if (e.data.calibratedSignals) {
            var possibleSteps = [];
            currentSoftwarePanelSteps.forEach((step, index) => {
                if (step.signalCalibration) {
                    possibleSteps.push(step);
                }
            });
            possibleSteps.forEach((step) => {
                var matches = true;
                var signalTypes = ['rcxI', 'lecI', 'gvdI', 'rcxE', 'lecE', 'gvdE'];
                signalTypes.forEach((signalType) => {
                    if ((step.signalCalibration[signalType].active === e.data.calibratedSignals[signalType].active) && (e.data.calibratedSignals[signalType].value < step.signalCalibration[signalType].minValue && e.data.calibratedSignals[signalType].value > step.signalCalibration[signalType].maxValue)) {
                        matches = false;
                    }
                })
                if (matches) {
                    const completionSquare = document.getElementById(step.completionSquare);
                    if(completionSquare.classList.contains('incomplete')) {
                        toggleComplete(completionSquare);
                    }
                }
            });
        }
        else if (e.data.configuredCapacitors) {
            console.log('configuredCapacitors', e.data.configuredCapacitors);
            var possibleSteps = [];
            currentSoftwarePanelSteps.forEach((step, index) => {
                if (step.capacitorConduits) {
                    possibleSteps.push(step);
                }
            });
            possibleSteps.forEach((step) => {
                console.log(step.capacitorConduits);
                var matches = true;
                var capacitorTypes = ['alpha', 'bravo'];
                capacitorTypes.forEach((capacitorType) => {
                    console.log(step.capacitorConduits[capacitorType].minValue, step.capacitorConduits[capacitorType].maxValue, e.data.configuredCapacitors[capacitorType]);
                    if ((e.data.configuredCapacitors[capacitorType] < step.capacitorConduits[capacitorType].minValue || e.data.configuredCapacitors[capacitorType] > step.capacitorConduits[capacitorType].maxValue)) {
                        matches = false;
                    }
                });
                if (matches) {
                    const completionSquare = document.getElementById(step.completionSquare);
                    if(completionSquare.classList.contains('incomplete')) {
                        toggleComplete(completionSquare);
                    }
                }
            });
        }
    }
};

function toggleComplete(element) {
    if (element.classList.contains('completed')) {
        element.textContent = "";
        element.classList.remove('completed');
        element.classList.add('incomplete');
    } else {
        element.textContent = "✓";
        element.classList.remove('incomplete');
        element.classList.add('completed');
    }
    console.log(element);
}

document.addEventListener('DOMContentLoaded', () => {
    const toggleBtn = document.getElementById('toggleDamageReport');
    const dock = document.getElementById('monitorDock');
  
    toggleBtn.addEventListener('click', () => {
      dock.classList.toggle('damageReportOpen');
      toggleBtn.textContent = dock.classList.contains('damageReportOpen') ? '▲ HIDE DAMAGE REPORTS' : '▼ DAMAGE REPORTS';
    });
  
    fetch('/dev-fd/damageReport.html')
      .then(res => res.text())
      .then(html => {
        document.getElementById('damageReportContent').innerHTML = html;

        document.getElementById('warpEnginesButton').addEventListener('click', () => {
            currentViewedReport = 'warpEngines';
            const currentReport = document.getElementById('currentReport');
            currentReport.innerHTML = `
                <h4>Warp Engines Damage Report</h4>
                <div class="damageReportStepContainer">
                    <h5>Step 1</h5>
                    <p class="damageReportStep">This is a step that doesn't have to do with the software panel already made. You can mark it as complete by clicking on the status box or conversely mark the step as incomplete.</p>
                    <div class="completionStatusContainer">
                        <span>Status:</span>
                        <div id="warpEnginesStep1Completion" class="completionStatusBox incomplete" onclick="toggleComplete(this)"></div>
                    </div>
                </div>
                <div class="damageReportStepContainer">
                    <h5>Step 2</h5>
                    <p class="damageReportStep">Turn on the following switches in the Positronic Switches, then press the "configure" button for the Positronic Interface. Alpha - 1, Alpha - 2, Alpha - 5, Bravo - 3, Bravo - 6</p>
                    <div class="completionStatusContainer">
                        <span>Status:</span>
                        <div id="warpEnginesStep2Completion" class="completionStatusBox incomplete" onclick="toggleComplete(this)"></div>
                    </div>
                </div>
                <div class="damageReportStepContainer">
                    <h5>Step 3</h5>
                    <p class="damageReportStep">This is a step that doesn't have to do with the software panel already made. You can mark it as complete by clicking on the status box or conversely mark the step as incomplete.</p>
                    <div class="completionStatusContainer">
                        <span>Status:</span>
                        <div id="warpEnginesStep3Completion" class="completionStatusBox incomplete" onclick="toggleComplete(this)"></div>
                    </div>
                </div>
                <div class="damageReportStepContainer">
                    <h5>Step 4</h5>
                    <p class="damageReportStep">Adjust the internal signal calibration slider to between 10-20%, then press the RCX-I and GVD-I button. Then adjust the signal Calibration slider to between 80-100%, and press the LEC-I button. Adjust RCX-E, LEC-E, and GVD-E to all be Yellow using the external signal calibration. Then press the configure button.</p>
                    <div class="completionStatusContainer">    
                        <span>Status:</span>
                        <div id="warpEnginesStep4Completion" class="completionStatusBox incomplete" onclick="toggleComplete(this)"></div>
                    </div>
                </div>
                <div class="damageReportStepContainer">
                    <h5>Step 5</h5>
                    <p class="damageReportStep">Adjust the Alpha Capacitor Conduits to 35% and the Bravo Capacitor Conduits to 14% and then press the configure button.</p>
                    <div class="completionStatusContainer">
                        <span>Status:</span>
                        <div id="warpEnginesStep5Completion" class="completionStatusBox incomplete" onclick="toggleComplete(this)"></div>
                    </div>
                </div>
                <div class="damageReportStepContainer">
                    <h5>Step 6</h5>
                    <p class="damageReportStep">This is a step that doesn't have to do with the software panel already made. You can mark it as complete by clicking on the status box or conversely mark the step as incomplete.</p>
                    <div class="completionStatusContainer">
                        <span>Status:</span>
                        <div id="warpEnginesStep6Completion" class="completionStatusBox incomplete" onclick="toggleComplete(this)"></div>
                    </div>
                </div>
            `;       
        });
      })
      .catch(err => console.error('Error loading panel:', err));
});