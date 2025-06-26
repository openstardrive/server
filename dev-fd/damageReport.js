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
      })
      .catch(err => console.error('Error loading panel:', err));
});