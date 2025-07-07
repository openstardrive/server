var positronicSwitches = [];
var switchesState = [];
var rcxIState;
var lecIState;
var gvdIState;
var rcxEState;
var lecEState;
var gvdEState;
var internalSignalValue = 0;
var externalSignalValue = 0;
var alphaGaugeValue = 0;
var bravoGaugeValue = 0;

document.addEventListener('DOMContentLoaded', () => {
    const toggleBtn = document.getElementById('togglePanel');
    const dock = document.getElementById('monitorDock');
  
    toggleBtn.addEventListener('click', () => {
      dock.classList.toggle('panelOpen');
      toggleBtn.textContent = dock.classList.contains('panelOpen') ? '▲ HIDE PANEL' : '▼ PANEL';
    });
  
    fetch('/dev-fd/miniPanel.html')
      .then(res => res.text())
      .then(html => {
        document.getElementById('panelContent').innerHTML = html;
        createArcTicks('alphaTicksArc');
        createArcTicks('bravoTicksArc');

        function addPositronicSwitches() {
            for (let i = 1; i<= 6; i++) {
                const alphaSwitch = document.getElementById(`alpha-${i}`);
                const betaSwitch = document.getElementById(`bravo-${i}`);
                if (!alphaSwitch || !betaSwitch) {
                    console.error("not all switches found for pair", i);
                }
                else {
                    positronicSwitches.push(alphaSwitch);
                    positronicSwitches.push(betaSwitch);
                }
            }
        }
      
        addPositronicSwitches();

        var storedSwitchesState = localStorage.getItem('switchesState');
        var storedRcxIState = localStorage.getItem('rcxIState');
        var storedLecIState = localStorage.getItem('lecIState');
        var storedGvdIState = localStorage.getItem('gvdIState');
        var storedRcxEState = localStorage.getItem('rcxEState');
        var storedLecEState = localStorage.getItem('lecEState');
        var storedGvdEState = localStorage.getItem('gvdEState');
        var storedAlphaGaugeValue = localStorage.getItem('alphaGaugeValue');
        var storedBravoGaugeValue = localStorage.getItem('bravoGaugeValue');
        var storedInternalSignalValue = localStorage.getItem('internalSignalValue');
        var storedExternalSignalValue = localStorage.getItem('externalSignalValue');
        var storedAlphaGaugeValue = localStorage.getItem('alphaGaugeValue');
        var storedBravoGaugeValue = localStorage.getItem('bravoGaugeValue');
        if (storedSwitchesState) {
            switchesState = JSON.parse(storedSwitchesState);
            positronicSwitches.forEach(switchElement => {
                if (switchesState.includes(switchElement.id)) {
                switchElement.classList.add("on");
                } else {
                    switchElement.classList.remove("on");
                }
            });
        }
        if (storedRcxIState) {
            rcxIState = JSON.parse(storedRcxIState);
            if (rcxIState.active) {
                signalCalibration('rcx-i', rcxIState.value); 
            }
        }
        if (storedLecIState) {
            lecIState = JSON.parse(storedLecIState);
            if (lecIState.active) {
                signalCalibration('lec-i', lecIState.value);
            }
        }
        if (storedGvdIState) {
            gvdIState = JSON.parse(storedGvdIState);
            if (gvdIState.active) {
                signalCalibration('gvd-i', gvdIState.value);
            }
        }
        if (storedRcxEState) {
            rcxEState = JSON.parse(storedRcxEState);
            if (rcxEState.active) {
                signalCalibration('rcx-e', gvdIState.value);
            }
        }
        if (storedLecEState) {
            lecEState = JSON.parse(storedLecEState);
            if (lecEState.active) {
                signalCalibration('lec-e', gvdIState.value);
            }
        }
        if (storedGvdEState) {
            gvdEState = JSON.parse(storedGvdEState);
            if (gvdEState.active) {
                signalCalibration('gvd-e', gvdIState.value);
            }
        }   
        if (storedInternalSignalValue) {
            internalSignalValue = JSON.parse(storedInternalSignalValue);
            const internalCircleText = document.getElementById('internalCircleText');
            const internalCircleDisplay = document.getElementById('internalCircleDisplay');
            document.getElementById('internalSignalSlider').value = internalSignalValue;
            internalCircleText.textContent = `${internalSignalValue}%`;
            internalCircleDisplay.style.background = `conic-gradient(rgba(0, 255, 255, 0.6) ${internalSignalValue}%, #111 0%)`;
        }
        if (storedExternalSignalValue) {
            externalSignalValue = JSON.parse(storedExternalSignalValue);
            const externalCircleText = document.getElementById('externalCircleText');
            const externalCircleDisplay = document.getElementById('externalCircleDisplay');
            document.getElementById('externalSignalSlider').value = externalSignalValue;
            externalCircleText.textContent = `${externalSignalValue}%`;
            externalCircleDisplay.style.background = `conic-gradient(rgba(0, 255, 255, 0.6) ${externalSignalValue}%, #111 0%)`;
        }
        if (storedAlphaGaugeValue) {
            alphaGaugeValue = JSON.parse(storedAlphaGaugeValue);
            updateGauge('alphaNeedle', 'alphaGaugeValue', alphaGaugeValue);
        }
        if (storedBravoGaugeValue) {
            bravoGaugeValue = JSON.parse(storedBravoGaugeValue);
            updateGauge('bravoNeedle', 'bravoGaugeValue', bravoGaugeValue);
        }
      })
      .catch(err => console.error('Error loading mini panel:', err));
});

const channel = new BroadcastChannel('panel-sync');
channel.onmessage = (e) => {
    if (e.data.type === 'flipSwitch') {
      const switchElement = document.getElementById(e.data.id);
      switchElement.classList.toggle("on");
      if (switchElement.classList.contains("on")) {
        switchesState.push(switchElement.id);
      }
      else {
        switchesState = switchesState.filter(id => id !== switchElement.id);
      }
      localStorage.setItem('switchesState', JSON.stringify(switchesState));
    }
    else if (e.data.type === 'resetSwitches') {
        const positronicSwitches = [];

        function addPositronicSwitches() {
            for (let i = 1; i<= 6; i++) {
            const alphaSwitch = document.getElementById(`alpha-${i}`);
            const betaSwitch = document.getElementById(`bravo-${i}`);
            if (!alphaSwitch || !betaSwitch) {
                console.error("not all switches found for pair", i);
            }
            else {
                positronicSwitches.push(alphaSwitch);
                positronicSwitches.push(betaSwitch);
            }
            }
        }

        addPositronicSwitches();
        switchesState = [];
        localStorage.setItem('switchesState', JSON.stringify(switchesState));
        positronicSwitches.forEach(switchElement => {
            if (switchElement.classList.contains("on")) {
              switchElement.classList.remove("on");
            }
        });
    }
    else if (e.data.type === 'rcx-i') {
        const led = document.getElementById('rcx-i-1');
        if (led.classList.contains('on')) {
            rcxIState = {
                active: false,
                value: 0
            }
        }
        else {
            rcxIState = {
                active: true,
                value: e.data.value
            };
        }
        localStorage.setItem('rcxIState', JSON.stringify(rcxIState));
        signalCalibration('rcx-i', e.data.value);
    }
    else if (e.data.type === 'lec-i') {
        const led = document.getElementById('lec-i-1');
        if (led.classList.contains('on')) {
            lecIState = {
                active: false,
                value: 0
            }
        }
        else {
            lecIState = {
                active: true,
                value: e.data.value
            };
        }
        localStorage.setItem('lecIState', JSON.stringify(lecIState));
        signalCalibration('lec-i', e.data.value);
    }
    else if (e.data.type === 'gvd-i') {
        const led = document.getElementById('gvd-i-1');
        if (led.classList.contains('on')) {
            gvdIState = {
                active: false,
                value: 0
            }
        }
        else {
            gvdIState = {
                active: true,
                value: e.data.value
            };
        }
        localStorage.setItem('gvdIState', JSON.stringify(gvdIState));
        signalCalibration('gvd-i', e.data.value);
    }
    else if (e.data.type === 'internalSignal') {
        internalSignalValue = JSON.stringify(e.data.value);
        localStorage.setItem('internalSignalValue', internalSignalValue);
        updateInternalSignal(e.data.value);
    }
    else if (e.data.type === 'rcx-e') {
        const led = document.getElementById('rcx-e-1');
        if (led.classList.contains('on')) {
            rcxEState = {
                active: false,
                value: 0
            }
        }
        else {
            rcxEState = {
                active: true,
                value: e.data.value
            };
        }
        localStorage.setItem('rcxEState', JSON.stringify(rcxEState));
        signalCalibration('rcx-e', e.data.value);
    }
    else if (e.data.type === 'lec-e') {
        const led = document.getElementById('lec-e-1');
        if (led.classList.contains('on')) {
            lecEState = {
                active: false,
                value: 0
            }
        }
        else {
            lecEState = {
                active: true,
                value: e.data.value
            };
        }
        localStorage.setItem('lecEState', JSON.stringify(lecEState));
        signalCalibration('lec-e', e.data.value);
    }
    else if (e.data.type === 'gvd-e') {
        const led = document.getElementById('gvd-e-1');
        if (led.classList.contains('on')) {
            gvdEState = {
                active: false,
                value: 0
            }
        }
        else {
            gvdEState = {
                active: true,
                value: e.data.value
            };
        }
        localStorage.setItem('gvdEState', JSON.stringify(gvdEState));
        signalCalibration('gvd-e', e.data.value);
    }
    else if (e.data.type === 'externalSignal') {
        externalSignalValue = JSON.stringify(e.data.value);
        localStorage.setItem('externalSignalValue', externalSignalValue);
        updateExternalSignal(e.data.value);
    }
    else if (e.data.type === 'resetCalibration') {
        const leds = [
            document.getElementById('rcx-i-1'),
            document.getElementById('rcx-i-2'),
            document.getElementById('rcx-i-3'),
            document.getElementById('lec-i-1'),
            document.getElementById('lec-i-2'),
            document.getElementById('lec-i-3'),
            document.getElementById('gvd-i-1'),
            document.getElementById('gvd-i-2'),
            document.getElementById('gvd-i-3'),
            document.getElementById('rcx-e-1'),
            document.getElementById('rcx-e-2'),
            document.getElementById('rcx-e-3'),
            document.getElementById('lec-e-1'),
            document.getElementById('lec-e-2'),
            document.getElementById('lec-e-3'),
            document.getElementById('gvd-e-1'),
            document.getElementById('gvd-e-2'),
            document.getElementById('gvd-e-3')
        ];
        
        leds.forEach(led => {
            if (led.classList.contains('on')) {
              led.classList.remove('on');
              led.classList.remove('led-red', 'led-yellow', 'led-green', 'led-blue', 'led-purple');
            }
        });
        
        document.getElementById('internalSignalSlider').value = 0;
        document.getElementById('externalSignalSlider').value = 0;
        document.getElementById('internalCircleText').textContent = '0%';
        document.getElementById('externalCircleText').textContent = '0%';
        document.getElementById('internalCircleDisplay').style.background = 'conic-gradient(rgba(0, 255, 255, 0.6) 0%, #111 0%)';
        document.getElementById('externalCircleDisplay').style.background = 'conic-gradient(rgba(0, 255, 255, 0.6) 0%, #111 0%)';
        internalSignalValue = 0;
        externalSignalValue = 0;
        rcxIState = { active: false, value: 0 };
        lecIState = { active: false, value: 0 };
        gvdIState = { active: false, value: 0 };
        rcxEState = { active: false, value: 0 };
        lecEState = { active: false, value: 0 };
        gvdEState = { active: false, value: 0 };
        localStorage.setItem('rcxIState', JSON.stringify(rcxIState));
        localStorage.setItem('lecIState', JSON.stringify(lecIState));
        localStorage.setItem('gvdIState', JSON.stringify(gvdIState));
        localStorage.setItem('rcxEState', JSON.stringify(rcxEState));
        localStorage.setItem('lecEState', JSON.stringify(lecEState));
        localStorage.setItem('gvdEState', JSON.stringify(gvdEState));
        localStorage.setItem('internalSignalValue', JSON.stringify(internalSignalValue));
        localStorage.setItem('externalSignalValue', JSON.stringify(externalSignalValue));
    }
    else if (e.data.type === 'alphaGaugeUpdate') {
        updateGauge('alphaNeedle', 'alphaGaugeValue', e.data.value);
        alphaGaugeValue = e.data.value;
        localStorage.setItem('alphaGaugeValue', JSON.stringify(alphaGaugeValue));
    }
    else if (e.data.type === 'bravoGaugeUpdate') {
        updateGauge('bravoNeedle', 'bravoGaugeValue', e.data.value);
        bravoGaugeValue = e.data.value;
        localStorage.setItem('bravoGaugeValue', JSON.stringify(bravoGaugeValue));
    }
};

function updateInternalSignal(value) {
    document.getElementById('internalCircleText').innerText = value + '%';
    document.getElementById('internalCircleDisplay').style.background = `conic-gradient(rgba(0, 255, 255, 0.6) ${value}%, #111 0%)`;
}

function updateExternalSignal(value) {
    document.getElementById('externalCircleText').innerText = value + '%';
    document.getElementById('externalCircleDisplay').style.background = `conic-gradient(rgba(0, 255, 255, 0.6) ${value}%, #111 0%)`;
}

function signalCalibration(id, signalValue) {
    const leds = [
        document.getElementById(`${id}-1`),
        document.getElementById(`${id}-2`),
        document.getElementById(`${id}-3`),
    ];

    const isOn = leds.some(led => led.classList.contains('on'));
    
    leds.forEach(led => {
        led.classList.remove('on', 'led-red', 'led-yellow', 'led-green', 'led-blue', 'led-purple');
    });
    
    if (!isOn) {
        let colorClass = 'led-red';
        if (signalValue < 10) colorClass = 'led-red';
        else if (signalValue < 30) colorClass = 'led-yellow';
        else if (signalValue < 60) colorClass = 'led-green';
        else if (signalValue < 90) colorClass = 'led-blue';
        else colorClass = 'led-purple';
    
        leds.forEach(led => {
          led.classList.add('on', colorClass);
        });
    }
}

function toggleSwitch(element) {
    element.classList.toggle("on");
}

function createArcTicks(containerId, count = 10) {
    const container = document.getElementById(containerId);
    const radius = 80;
    
    for (let i = 0; i <= count; i++) {
      const angle = map(i, 0, count, -90, 90);
      const percent = Math.round((i / count) * 100);
      const isLabeled = percent % 20 === 0;
    
      const tick = document.createElement('div');
      tick.classList.add('tick');
      if (isLabeled) tick.classList.add('tick-labeled');
      tick.style.transform = `rotate(${angle}deg) translateY(-${radius}px)`;
      container.appendChild(tick);
    
      if (isLabeled) {
        const label = document.createElement('div');
        label.classList.add('tick-label');
        label.innerText = `${percent}%`;
        label.style.transform = `rotate(${angle}deg) translateY(-${radius + 20}px) rotate(${-angle}deg)`;
        container.appendChild(label);
      }
    }
}
    
function map(val, inMin, inMax, outMin, outMax) {
    return ((val - inMin) * (outMax - outMin)) / (inMax - inMin) + outMin;
}

function updateGauge(needleId, gaugeId, value) {
    const needle = document.getElementById(needleId);
    const label = document.getElementById(gaugeId);
    const clamped = Math.max(0, Math.min(100, value));
    const angle = map(clamped, 0, 100, -90, 90);
    needle.style.transform = `rotate(${angle}deg)`;
    label.textContent = `${clamped}%`;
}