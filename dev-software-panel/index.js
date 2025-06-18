function toggleSwitch(element) {
    element.classList.toggle("on");
}

const slider = document.getElementById('powerSlider');
const circleText = document.getElementById('circleText');
const circleDisplay = document.getElementById('circleDisplay');

if (!slider || !circleText || !circleDisplay) {
    console.error('Missing elements:', { slider, circleText, circleDisplay });
} else {
    slider.addEventListener('input', () => {
        const value = slider.value;
        circleText.textContent = `${value}%`;
        circleDisplay.style.background = `conic-gradient(rgba(0, 255, 255, 0.6) ${value}%, #111 0%)`;
    });      
}