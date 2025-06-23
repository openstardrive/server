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
};

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
  