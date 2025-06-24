document.addEventListener('DOMContentLoaded', () => {
    const toggleBtn = document.getElementById('toggleMonitor');
    const dock = document.getElementById('monitorDock');
  
    toggleBtn.addEventListener('click', () => {
      dock.classList.toggle('open');
      toggleBtn.textContent = dock.classList.contains('open') ? '▲ HIDE PANEL' : '▼ PANEL';
    });
  
    fetch('/dev-fd/miniPanel.html')
      .then(res => res.text())
      .then(html => {
        document.getElementById('slidePanel').innerHTML = html;
        createArcTicks('alphaTicksArc');
        createArcTicks('bravoTicksArc');
      })
      .catch(err => console.error('Error loading mini panel:', err));
});

const channel = new BroadcastChannel('panel-sync');
channel.onmessage = (e) => {
    if (e.data.type === 'flipSwitch') {
      document.getElementById(e.data.id).classList.toggle('on');
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
        console.log('Resetting switches');
        positronicSwitches.forEach(switchElement => {
            if (switchElement.classList.contains("on")) {
              switchElement.classList.remove("on");
            }
        });
    }
    else if (e.data.type === 'rcx-i') {
        signalCalibration('rcx-i', e.data.value);
    }
    else if (e.data.type === 'lec-i') {
        signalCalibration('lec-i', e.data.value);
    }
    else if (e.data.type === 'gvd-i') {
        signalCalibration('gvd-i', e.data.value);
    }
    else if (e.data.type === 'internalSignal') {
        updateInternalSignal(e.data.value);
    }
    else if (e.data.type === 'rcx-e') {
        signalCalibration('rcx-e', e.data.value);
    }
    else if (e.data.type === 'lec-e') {
        signalCalibration('lec-e', e.data.value);
    }
    else if (e.data.type === 'gvd-e') {
        signalCalibration('gvd-e', e.data.value);
    }
    else if (e.data.type === 'externalSignal') {
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
    }
    else if (e.data.type === 'alphaGaugeUpdate') {
        console.log('Updating alpha gauge:', e.data.value);
        updateGauge('alphaNeedle', 'alphaGaugeValue', e.data.value);
    }
    else if (e.data.type === 'bravoGaugeUpdate') {
        console.log('Updating bravo gauge:', e.data.value);
        updateGauge('bravoNeedle', 'bravoGaugeValue', e.data.value);
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