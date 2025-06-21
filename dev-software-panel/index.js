function toggleSwitch(element) {
    element.classList.toggle("on");
}

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