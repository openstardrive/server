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

// .switch.on .led {
//     background: #0f0;
//     box-shadow: 0 0 8px #0f0, inset 0 0 2px #0f0;
//     animation: ledPulse 2.0s infinite ease-in-out;
// }

// @keyframes ledPulse {
//     0%, 100% {
//       box-shadow: 0 0 5px #0f0, 0 0 10px #0f0, inset 0 0 3px #0f0;
//     }
//     50% {
//       box-shadow: 0 0 15px #0f0, 0 0 25px #0f0, inset 0 0 4px #0f0;
//     }
// }

const rcxIButton = document.getElementById('rcx-i');

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
  