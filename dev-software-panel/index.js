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

function toggleSwitch(element) {
    element.classList.toggle("on");
}

document.getElementById('configurePositronicInterface').addEventListener('click', () => {
    positronicSwitches.forEach(switchElement => {
        if (switchElement.classList.contains("on")) {
            switchElement.classList.remove("on");
        }
    });
});

const internalSlider = document.getElementById('internalSignalSlider');
const internalCircleText = document.getElementById('internalCircleText');
const internalCircleDisplay = document.getElementById('internalCircleDisplay');

if (!internalSlider || !internalCircleText || !internalCircleDisplay) {
    console.error('Missing internal elements:', { internalSlider, internalCircleText, internalCircleDisplay });
} else {
    internalSlider.addEventListener('input', () => {
        const value = internalSlider.value;
        internalCircleText.textContent = `${value}%`;
        internalCircleDisplay.style.background = `conic-gradient(rgba(0, 255, 255, 0.6) ${value}%, #111 0%)`;
    });      
}

const externalSlider = document.getElementById('externalSignalSlider');
const externalCircleText = document.getElementById('externalCircleText');
const externalCircleDisplay = document.getElementById('externalCircleDisplay');

if (!externalSlider || !externalCircleText || !externalCircleDisplay) {
    console.error('Missing External elements:', { externalSlider, externalCircleText, externalCircleDisplay });
} else {
    externalSlider.addEventListener('input', () => {
        const value = externalSlider.value;
        externalCircleText.textContent = `${value}%`;
        externalCircleDisplay.style.background = `conic-gradient(rgba(0, 255, 255, 0.6) ${value}%, #111 0%)`;
    });      
}

document.getElementById('rcx-i').addEventListener('click', () => {
    const rcxLeds = [
      document.getElementById('rcx-i-1'),
      document.getElementById('rcx-i-2'),
      document.getElementById('rcx-i-3')
    ];
  
    const signalValue = parseInt(document.getElementById('internalSignalSlider').value, 10);
    const isOn = rcxLeds.some(led => led.classList.contains('on'));
  
    // Reset classes
    rcxLeds.forEach(led => {
      led.classList.remove('on', 'led-red', 'led-yellow', 'led-green', 'led-blue', 'led-purple');
    });
  
    if (!isOn) {
      let colorClass = 'led-red';
      if (signalValue < 10) colorClass = 'led-red';
      else if (signalValue < 30) colorClass = 'led-yellow';
      else if (signalValue < 60) colorClass = 'led-green';
      else if (signalValue < 90) colorClass = 'led-blue';
      else colorClass = 'led-purple';
  
      rcxLeds.forEach(led => {
        led.classList.add('on', colorClass);
      });
    }
});

document.getElementById('lec-i').addEventListener('click', () => {
    const lecLeds = [
      document.getElementById('lec-i-1'),
      document.getElementById('lec-i-2'),
      document.getElementById('lec-i-3')
    ];
  
    const signalValue = parseInt(document.getElementById('internalSignalSlider').value, 10);
    const isOn = lecLeds.some(led => led.classList.contains('on'));
  
    // Reset classes
    lecLeds.forEach(led => {
      led.classList.remove('on', 'led-red', 'led-yellow', 'led-green', 'led-blue', 'led-purple');
    });
  
    if (!isOn) {
      let colorClass = 'led-red';
      if (signalValue < 10) colorClass = 'led-red';
      else if (signalValue < 30) colorClass = 'led-yellow';
      else if (signalValue < 60) colorClass = 'led-green';
      else if (signalValue < 90) colorClass = 'led-blue';
      else colorClass = 'led-purple';
  
      lecLeds.forEach(led => {
        led.classList.add('on', colorClass);
      });
    }
});

document.getElementById('gvd-i').addEventListener('click', () => {
    const gvdLeds = [
      document.getElementById('gvd-i-1'),
      document.getElementById('gvd-i-2'),
      document.getElementById('gvd-i-3')
    ];
  
    const signalValue = parseInt(document.getElementById('internalSignalSlider').value, 10);
    const isOn = gvdLeds.some(led => led.classList.contains('on'));
  
    // Reset classes
    gvdLeds.forEach(led => {
      led.classList.remove('on', 'led-red', 'led-yellow', 'led-green', 'led-blue', 'led-purple');
    });
  
    if (!isOn) {
      let colorClass = 'led-red';
      if (signalValue < 10) colorClass = 'led-red';
      else if (signalValue < 30) colorClass = 'led-yellow';
      else if (signalValue < 60) colorClass = 'led-green';
      else if (signalValue < 90) colorClass = 'led-blue';
      else colorClass = 'led-purple';
  
      gvdLeds.forEach(led => {
        led.classList.add('on', colorClass);
      });
    }
});

document.getElementById('rcx-e').addEventListener('click', () => {
    const rcxLeds = [
      document.getElementById('rcx-e-1'),
      document.getElementById('rcx-e-2'),
      document.getElementById('rcx-e-3')
    ];
  
    const signalValue = parseInt(document.getElementById('internalSignalSlider').value, 10);
    const isOn = rcxLeds.some(led => led.classList.contains('on'));
  
    // Reset classes
    rcxLeds.forEach(led => {
      led.classList.remove('on', 'led-red', 'led-yellow', 'led-green', 'led-blue', 'led-purple');
    });
  
    if (!isOn) {
      let colorClass = 'led-red';
      if (signalValue < 10) colorClass = 'led-red';
      else if (signalValue < 30) colorClass = 'led-yellow';
      else if (signalValue < 60) colorClass = 'led-green';
      else if (signalValue < 90) colorClass = 'led-blue';
      else colorClass = 'led-purple';
  
      rcxLeds.forEach(led => {
        led.classList.add('on', colorClass);
      });
    }
});

document.getElementById('lec-e').addEventListener('click', () => {
    const lecLeds = [
      document.getElementById('lec-e-1'),
      document.getElementById('lec-e-2'),
      document.getElementById('lec-e-3')
    ];
  
    const signalValue = parseInt(document.getElementById('internalSignalSlider').value, 10);
    const isOn = lecLeds.some(led => led.classList.contains('on'));
  
    // Reset classes
    lecLeds.forEach(led => {
      led.classList.remove('on', 'led-red', 'led-yellow', 'led-green', 'led-blue', 'led-purple');
    });
  
    if (!isOn) {
      let colorClass = 'led-red';
      if (signalValue < 10) colorClass = 'led-red';
      else if (signalValue < 30) colorClass = 'led-yellow';
      else if (signalValue < 60) colorClass = 'led-green';
      else if (signalValue < 90) colorClass = 'led-blue';
      else colorClass = 'led-purple';
  
      lecLeds.forEach(led => {
        led.classList.add('on', colorClass);
      });
    }
});

document.getElementById('gvd-e').addEventListener('click', () => {
    const gvdLeds = [
      document.getElementById('gvd-e-1'),
      document.getElementById('gvd-e-2'),
      document.getElementById('gvd-e-3')
    ];
  
    const signalValue = parseInt(document.getElementById('internalSignalSlider').value, 10);
    const isOn = gvdLeds.some(led => led.classList.contains('on'));
  
    // Reset classes
    gvdLeds.forEach(led => {
      led.classList.remove('on', 'led-red', 'led-yellow', 'led-green', 'led-blue', 'led-purple');
    });
  
    if (!isOn) {
      let colorClass = 'led-red';
      if (signalValue < 10) colorClass = 'led-red';
      else if (signalValue < 30) colorClass = 'led-yellow';
      else if (signalValue < 60) colorClass = 'led-green';
      else if (signalValue < 90) colorClass = 'led-blue';
      else colorClass = 'led-purple';
  
      gvdLeds.forEach(led => {
        led.classList.add('on', colorClass);
      });
    }
});

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

createArcTicks('ticksArc');

let gaugeValue = 0;
  
function updateGauge(value) {
    const needle = document.getElementById('needle');
    const label = document.getElementById('gaugeValue');
    const clamped = Math.max(0, Math.min(100, value));
    const angle = map(clamped, 0, 100, -90, 90);
    needle.style.transform = `rotate(${angle}deg)`;
    label.textContent = `${clamped}%`;
}
  
document.addEventListener('keydown', (e) => {
    if (e.key === 'ArrowRight') {
      gaugeValue += 10;
      updateGauge(gaugeValue);
    } else if (e.key === 'ArrowLeft') {
      gaugeValue -= 10;
      updateGauge(gaugeValue);
    }
});
  