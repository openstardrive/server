const channel = new BroadcastChannel('panel-sync');

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

function toggleSwitch(element) {
  element.classList.toggle("on");
  if (element.classList.contains("on")) {
    switchesState.push(element.id);
  }
  else {
    switchesState = switchesState.filter(id => id !== element.id);
  }
  localStorage.setItem('switchesState', JSON.stringify(switchesState));
  channel.postMessage({ type: 'flipSwitch', id: element.id });
}

document.addEventListener('DOMContentLoaded', () => {
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
      const rcxLeds = [
        document.getElementById('rcx-i-1'),
        document.getElementById('rcx-i-2'),
        document.getElementById('rcx-i-3')
      ];
    
      const signalValue = rcxIState.value;
      console.log(signalValue);
    
      // Reset classes
      rcxLeds.forEach(led => {
        led.classList.remove('on', 'led-red', 'led-yellow', 'led-green', 'led-blue', 'led-purple');
      });
      
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
  }
  if (storedLecIState) {
    lecIState = JSON.parse(storedLecIState);
    if (lecIState.active) {
      const lecLeds = [
        document.getElementById('lec-i-1'),
        document.getElementById('lec-i-2'),
        document.getElementById('lec-i-3')
      ];
    
      const signalValue = lecIState.value;
    
      // Reset classes
      lecLeds.forEach(led => {
        led.classList.remove('on', 'led-red', 'led-yellow', 'led-green', 'led-blue', 'led-purple');
      });
      
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
  }
  if (storedGvdIState) {
    gvdIState = JSON.parse(storedGvdIState);
    if (gvdIState.active) {
      const gvdLeds = [
        document.getElementById('gvd-i-1'),
        document.getElementById('gvd-i-2'),
        document.getElementById('gvd-i-3')
      ];
    
      const signalValue = gvdIState.value;
    
      // Reset classes
      gvdLeds.forEach(led => {
        led.classList.remove('on', 'led-red', 'led-yellow', 'led-green', 'led-blue', 'led-purple');
      });
      
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
  }
  if (storedRcxEState) {
    rcxEState = JSON.parse(storedRcxEState);
    if (rcxEState.active) {
      const rcxLeds = [
        document.getElementById('rcx-e-1'),
        document.getElementById('rcx-e-2'),
        document.getElementById('rcx-e-3')
      ];
    
      const signalValue = rcxEState.value;
    
      // Reset classes
      rcxLeds.forEach(led => {
        led.classList.remove('on', 'led-red', 'led-yellow', 'led-green', 'led-blue', 'led-purple');
      });
      
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
  }
  if (storedLecEState) {
    lecEState = JSON.parse(storedLecEState);
    if (lecEState.active) {
      const lecLeds = [
        document.getElementById('lec-e-1'),
        document.getElementById('lec-e-2'),
        document.getElementById('lec-e-3')
      ];
    
      const signalValue = lecEState.value;
    
      // Reset classes
      lecLeds.forEach(led => {
        led.classList.remove('on', 'led-red', 'led-yellow', 'led-green', 'led-blue', 'led-purple');
      });
      
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
  }
  if (storedGvdEState) {
    gvdEState = JSON.parse(storedGvdEState);
    if (gvdEState.active) {
      const gvdLeds = [
        document.getElementById('gvd-e-1'),
        document.getElementById('gvd-e-2'),
        document.getElementById('gvd-e-3')
      ];
    
      const signalValue = gvdEState.value;
    
      // Reset classes
      gvdLeds.forEach(led => {
        led.classList.remove('on', 'led-red', 'led-yellow', 'led-green', 'led-blue', 'led-purple');
      });
      
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
  }
  if (storedAlphaGaugeValue) {
  }
  if (storedBravoGaugeValue) {
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

  document.getElementById('configurePositronicInterface').addEventListener('click', () => {
    positronicSwitches.forEach(switchElement => {
      if (switchElement.classList.contains("on")) {
        switchElement.classList.remove("on");
      }
    });
    switchesState = [];
    localStorage.setItem('switchesState', JSON.stringify(switchesState));
    channel.postMessage({ type: 'resetSwitches' });
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
      localStorage.setItem('internalSignalValue', JSON.stringify(value));
      channel.postMessage({ type: 'internalSignal', value: value });
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
      localStorage.setItem('externalSignalValue', JSON.stringify(value));
      channel.postMessage({ type: 'externalSignal', value: value });
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

        rcxIState = {
          active: true,
          value: signalValue
        }
      } 
      else {
        rcxIState = {
          active: false,
          value: 0
        };
      }

      localStorage.setItem('rcxIState', JSON.stringify(rcxIState));
      channel.postMessage({ type: 'rcx-i', value: signalValue });
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

        lecIState = {
          active: true,
          value: signalValue
        }
      }
      else {
        lecIState = {
          active: false,
          value: 0
        };
      }
      
      localStorage.setItem('lecIState', JSON.stringify(lecIState));
      channel.postMessage({ type: 'lec-i', value: signalValue });
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
        gvdIState = {
          active: true,
          value: signalValue
        }
      }
      else {
        gvdIState = {
          active: false,
          value: 0
        };
      }

      localStorage.setItem('gvdIState', JSON.stringify(gvdIState));
      channel.postMessage({ type: 'gvd-i', value: signalValue });
  });

  document.getElementById('rcx-e').addEventListener('click', () => {
      const rcxLeds = [
        document.getElementById('rcx-e-1'),
        document.getElementById('rcx-e-2'),
        document.getElementById('rcx-e-3')
      ];
    
      const signalValue = parseInt(document.getElementById('externalSignalSlider').value, 10);
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

        rcxEState = {
          active: true,
          value: signalValue
        }
      }
      else {
        rcxEState = {
          active: false,
          value: 0
        };
      }

      localStorage.setItem('rcxEState', JSON.stringify(rcxEState));
      channel.postMessage({ type: 'rcx-e', value: signalValue });
  });

  document.getElementById('lec-e').addEventListener('click', () => {
      const lecLeds = [
        document.getElementById('lec-e-1'),
        document.getElementById('lec-e-2'),
        document.getElementById('lec-e-3')
      ];
    
      const signalValue = parseInt(document.getElementById('externalSignalSlider').value, 10);
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
        lecEState = {
          active: true,
          value: signalValue
        }
      }
      else {
        lecEState = {
          active: false,
          value: 0
        };
      }

      localStorage.setItem('lecEState', JSON.stringify(lecEState));
      channel.postMessage({ type: 'lec-e', value: signalValue });
  });

  document.getElementById('gvd-e').addEventListener('click', () => {
      const gvdLeds = [
        document.getElementById('gvd-e-1'),
        document.getElementById('gvd-e-2'),
        document.getElementById('gvd-e-3')
      ];
    
      const signalValue = parseInt(document.getElementById('externalSignalSlider').value, 10);
      const isOn = gvdLeds.some(led => led.classList.contains('on'));
    
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
        gvdEState = {
          active: true,
          value: signalValue
        }
      }
      else {
        gvdEState = {
          active: false,
          value: 0
        };
      }
      
      localStorage.setItem('gvdEState', JSON.stringify(gvdEState));
      channel.postMessage({ type: 'gvd-e', value: signalValue });
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

  createArcTicks('alphaTicksArc');
  createArcTicks('bravoTicksArc');
    
  function updateGauge(needleId, gaugeId, value) {
    const needle = document.getElementById(needleId);
    const label = document.getElementById(gaugeId);
    const clamped = Math.max(0, Math.min(100, value));
    const angle = map(clamped, 0, 100, -90, 90);
    needle.style.transform = `rotate(${angle}deg)`;
    label.textContent = `${clamped}%`;
  }

  document.getElementById('configureSignalCalibration').addEventListener('click', () => {
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

    rcxIState = { active: false, value: 0 };
    lecIState = { active: false, value: 0 };
    gvdIState = { active: false, value: 0 };
    internalSignalValue = 0;
    localStorage.setItem('rcxIState', JSON.stringify(rcxIState));
    localStorage.setItem('lecIState', JSON.stringify(lecIState));
    localStorage.setItem('gvdIState', JSON.stringify(gvdIState));
    localStorage.setItem('internalSignalValue', JSON.stringify(internalSignalValue));
    channel.postMessage({ type: 'resetCalibration' });
  });

  document.getElementById('alphaMinus5').addEventListener('click', () => {
    if (alphaGaugeValue >= 5) {
      alphaGaugeValue -= 5;
      updateGauge('alphaNeedle', 'alphaGaugeValue', alphaGaugeValue);
      localStorage.setItem('alphaGaugeValue', JSON.stringify(alphaGaugeValue));
      channel.postMessage({ type: 'alphaGaugeUpdate', value: alphaGaugeValue });  }
  });

  document.getElementById('alphaMinus2.5').addEventListener('click', () => {
    if (alphaGaugeValue >= 2.5) {
      alphaGaugeValue -= 2.5;
      updateGauge('alphaNeedle', 'alphaGaugeValue', alphaGaugeValue);
      localStorage.setItem('alphaGaugeValue', JSON.stringify(alphaGaugeValue));
      channel.postMessage({ type: 'alphaGaugeUpdate', value: alphaGaugeValue });  }
  });

  document.getElementById('alphaPlus4').addEventListener('click', () => {
    if (alphaGaugeValue <= 96) {
      alphaGaugeValue += 4;
      updateGauge('alphaNeedle', 'alphaGaugeValue', alphaGaugeValue);
      localStorage.setItem('alphaGaugeValue', JSON.stringify(alphaGaugeValue));
      channel.postMessage({ type: 'alphaGaugeUpdate', value: alphaGaugeValue });  }
  });

  document.getElementById('alphaMinus4').addEventListener('click', () => {
    if (alphaGaugeValue >= 4) {
      alphaGaugeValue -= 4;
      updateGauge('alphaNeedle', 'alphaGaugeValue', alphaGaugeValue);
      localStorage.setItem('alphaGaugeValue', JSON.stringify(alphaGaugeValue));
      channel.postMessage({ type: 'alphaGaugeUpdate', value: alphaGaugeValue });  }
  });

  document.getElementById('alphaMinus1.5').addEventListener('click', () => {
    if (alphaGaugeValue >= 1.5) {
      alphaGaugeValue -= 1.5;
      updateGauge('alphaNeedle', 'alphaGaugeValue', alphaGaugeValue);
      localStorage.setItem('alphaGaugeValue', JSON.stringify(alphaGaugeValue));
      channel.postMessage({ type: 'alphaGaugeUpdate', value: alphaGaugeValue });
    }
  });

  document.getElementById('alphaPlus1.5').addEventListener('click', () => {
    if (alphaGaugeValue <= 98.5) {
      alphaGaugeValue += 1.5;
      updateGauge('alphaNeedle', 'alphaGaugeValue', alphaGaugeValue);
      localStorage.setItem('alphaGaugeValue', JSON.stringify(alphaGaugeValue));
      channel.postMessage({ type: 'alphaGaugeUpdate', value: alphaGaugeValue });
    }
  });

  document.getElementById('alphaReset').addEventListener('click', () => {
    alphaGaugeValue = 0;
    updateGauge('alphaNeedle', 'alphaGaugeValue', alphaGaugeValue);
    localStorage.setItem('alphaGaugeValue', JSON.stringify(alphaGaugeValue));
    channel.postMessage({ type: 'alphaGaugeUpdate', value: alphaGaugeValue });
  });

  document.getElementById('alphaPlus2.5').addEventListener('click', () => {
    if (alphaGaugeValue <= 97.5) {
      alphaGaugeValue += 2.5;
      updateGauge('alphaNeedle', 'alphaGaugeValue', alphaGaugeValue);
      localStorage.setItem('alphaGaugeValue', JSON.stringify(alphaGaugeValue));
      channel.postMessage({ type: 'alphaGaugeUpdate', value: alphaGaugeValue });  
    }
  });

  document.getElementById('alphaPlus5').addEventListener('click', () => {
    if (alphaGaugeValue <= 95) {
      alphaGaugeValue += 5;
      updateGauge('alphaNeedle', 'alphaGaugeValue', alphaGaugeValue);
      localStorage.setItem('alphaGaugeValue', JSON.stringify(alphaGaugeValue));
      channel.postMessage({ type: 'alphaGaugeUpdate', value: alphaGaugeValue });
    }
  });

  document.getElementById('bravoMinus5').addEventListener('click', () => {
    if (bravoGaugeValue >= 5) {
      bravoGaugeValue -= 5;
      updateGauge('bravoNeedle', 'bravoGaugeValue', bravoGaugeValue);
      localStorage.setItem('bravoGaugeValue', JSON.stringify(bravoGaugeValue));
      channel.postMessage({ type: 'bravoGaugeUpdate', value: bravoGaugeValue });
    }
  });

  document.getElementById('bravoMinus2.5').addEventListener('click', () => {
    if (bravoGaugeValue >= 2.5) {
      bravoGaugeValue -= 2.5;
      updateGauge('bravoNeedle', 'bravoGaugeValue', bravoGaugeValue);
      localStorage.setItem('bravoGaugeValue', JSON.stringify(bravoGaugeValue));
      channel.postMessage({ type: 'bravoGaugeUpdate', value: bravoGaugeValue });
    }
  });

  document.getElementById('bravoPlus4').addEventListener('click', () => {
    if (bravoGaugeValue <= 96) {
      bravoGaugeValue += 4;
      updateGauge('bravoNeedle', 'bravoGaugeValue', bravoGaugeValue);
      localStorage.setItem('bravoGaugeValue', JSON.stringify(bravoGaugeValue));
      channel.postMessage({ type: 'bravoGaugeUpdate', value: bravoGaugeValue });
    }
  });

  document.getElementById('bravoMinus4').addEventListener('click', () => {
    if (bravoGaugeValue >= 4) {
      bravoGaugeValue -= 4;
      updateGauge('bravoNeedle', 'bravoGaugeValue', bravoGaugeValue);
      localStorage.setItem('bravoGaugeValue', JSON.stringify(bravoGaugeValue));
      channel.postMessage({ type: 'bravoGaugeUpdate', value: bravoGaugeValue });
    }
  });

  document.getElementById('bravoMinus1.5').addEventListener('click', () => {
    if (bravoGaugeValue >= 1.5) {
      bravoGaugeValue -= 1.5;
      updateGauge('bravoNeedle', 'bravoGaugeValue', bravoGaugeValue);
      localStorage.setItem('bravoGaugeValue', JSON.stringify(bravoGaugeValue));
      channel.postMessage({ type: 'bravoGaugeUpdate', value: bravoGaugeValue });
    }
  });

  document.getElementById('bravoPlus1.5').addEventListener('click', () => {
    if (bravoGaugeValue <= 98.5) {
      bravoGaugeValue += 1.5;
      updateGauge('bravoNeedle', 'bravoGaugeValue', bravoGaugeValue);
      localStorage.setItem('bravoGaugeValue', JSON.stringify(bravoGaugeValue));
      channel.postMessage({ type: 'bravoGaugeUpdate', value: bravoGaugeValue });
    }
  });

  document.getElementById('bravoReset').addEventListener('click', () => {
    bravoGaugeValue = 0;
    updateGauge('bravoNeedle', 'bravoGaugeValue', bravoGaugeValue);
    localStorage.setItem('bravoGaugeValue', JSON.stringify(bravoGaugeValue));
    channel.postMessage({ type: 'bravoGaugeUpdate', value: bravoGaugeValue });
  });

  document.getElementById('bravoPlus2.5').addEventListener('click', () => {
    if (bravoGaugeValue <= 97.5) {
      bravoGaugeValue += 2.5;
      updateGauge('bravoNeedle', 'bravoGaugeValue', bravoGaugeValue);
      localStorage.setItem('bravoGaugeValue', JSON.stringify(bravoGaugeValue));
      channel.postMessage({ type: 'bravoGaugeUpdate', value: bravoGaugeValue });
    }
  });

  document.getElementById('bravoPlus5').addEventListener('click', () => {
    if (bravoGaugeValue <= 95) {
      bravoGaugeValue += 5;
      updateGauge('bravoNeedle', 'bravoGaugeValue', bravoGaugeValue);
      localStorage.setItem('bravoGaugeValue', JSON.stringify(bravoGaugeValue));
      channel.postMessage({ type: 'bravoGaugeUpdate', value: bravoGaugeValue });
    }
  });

  document.getElementById('capacitorChargeConfigure').addEventListener('click', () => {
    alphaGaugeValue = 0;
    bravoGaugeValue = 0;
    updateGauge('alphaNeedle', 'alphaGaugeValue', alphaGaugeValue);
    localStorage.setItem('alphaGaugeValue', JSON.stringify(alphaGaugeValue));
    channel.postMessage({ type: 'alphaGaugeUpdate', value: alphaGaugeValue });
    updateGauge('bravoNeedle', 'bravoGaugeValue', bravoGaugeValue);
    localStorage.setItem('bravoGaugeValue', JSON.stringify(bravoGaugeValue));
    channel.postMessage({ type: 'bravoGaugeUpdate', value: bravoGaugeValue });
  });
});