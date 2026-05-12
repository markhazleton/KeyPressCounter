'use strict';

/* ==========================================
   SCROLL REVEAL
   ========================================== */
const revealObserver = new IntersectionObserver((entries) => {
  entries.forEach(e => {
    if (e.isIntersecting) {
      e.target.classList.add('visible');
      revealObserver.unobserve(e.target);
    }
  });
}, { threshold: 0.1, rootMargin: '0px 0px -40px 0px' });

document.querySelectorAll('.reveal').forEach(el => revealObserver.observe(el));

/* ==========================================
   NAV SCROLL EFFECT
   ========================================== */
const nav = document.getElementById('nav');
window.addEventListener('scroll', () => {
  nav.classList.toggle('scrolled', window.scrollY > 32);
}, { passive: true });

/* ==========================================
   WIDGET TABS
   ========================================== */
document.querySelectorAll('.widget-tab').forEach(tab => {
  tab.addEventListener('click', () => {
    const target = tab.dataset.tab;
    document.querySelectorAll('.widget-tab').forEach(t => t.classList.remove('active'));
    tab.classList.add('active');
    document.querySelectorAll('.widget-panel').forEach(p => p.classList.add('hidden'));
    document.getElementById('panel-' + target).classList.remove('hidden');
  });
});

/* ==========================================
   INSTALL TABS
   ========================================== */
document.querySelectorAll('.itab').forEach(tab => {
  tab.addEventListener('click', () => {
    const target = tab.dataset.panel;
    document.querySelectorAll('.itab').forEach(t => t.classList.remove('active'));
    tab.classList.add('active');
    document.querySelectorAll('.install-panel').forEach(p => p.classList.add('hidden'));
    document.getElementById('panel-' + target).classList.remove('hidden');
  });
});

/* ==========================================
   COPY BUTTONS
   ========================================== */
document.querySelectorAll('.copy-btn').forEach(btn => {
  btn.addEventListener('click', () => {
    const code = btn.dataset.code || '';
    navigator.clipboard.writeText(code).then(() => {
      btn.textContent = 'Copied!';
      btn.classList.add('copied');
      setTimeout(() => {
        btn.textContent = 'Copy';
        btn.classList.remove('copied');
      }, 2000);
    });
  });
});

/* ==========================================
   DEMO WIDGET — LIVE SIMULATION
   ========================================== */
let demoKeys = 0;
let demoClicks = 0;
let demoPeak = 0;
let demoSessionSeconds = 0;
let demoRecentKeys = 0; // keys this minute
const history = new Array(28).fill(0);

const elKeys = document.getElementById('demo-keys');
const elClicks = document.getElementById('demo-clicks');
const elPeak = document.getElementById('demo-peak');
const elUptime = document.getElementById('demo-uptime');
const canvas = document.getElementById('mini-chart');
const ctx = canvas ? canvas.getContext('2d') : null;

function formatTime(secs) {
  const m = Math.floor(secs / 60).toString().padStart(2, '0');
  const s = (secs % 60).toString().padStart(2, '0');
  return `${m}:${s}`;
}

function drawMiniChart() {
  if (!ctx) return;
  const w = canvas.width;
  const h = canvas.height;
  const max = Math.max(...history, 10);

  ctx.clearRect(0, 0, w, h);

  // Gradient fill
  const grad = ctx.createLinearGradient(0, 0, 0, h);
  grad.addColorStop(0, 'rgba(99,102,241,0.35)');
  grad.addColorStop(1, 'rgba(99,102,241,0)');

  const step = w / (history.length - 1);

  ctx.beginPath();
  history.forEach((v, i) => {
    const x = i * step;
    const y = h - (v / max) * (h - 6) - 3;
    if (i === 0) ctx.moveTo(x, y);
    else ctx.lineTo(x, y);
  });

  // Close for fill
  ctx.lineTo((history.length - 1) * step, h);
  ctx.lineTo(0, h);
  ctx.closePath();
  ctx.fillStyle = grad;
  ctx.fill();

  // Line
  ctx.beginPath();
  history.forEach((v, i) => {
    const x = i * step;
    const y = h - (v / max) * (h - 6) - 3;
    if (i === 0) ctx.moveTo(x, y);
    else ctx.lineTo(x, y);
  });
  ctx.strokeStyle = 'rgba(129,140,248,0.9)';
  ctx.lineWidth = 1.5;
  ctx.lineJoin = 'round';
  ctx.stroke();
}

// Fast tick — simulate keystrokes/clicks (every 200ms)
setInterval(() => {
  if (Math.random() < 0.75) {
    const burst = Math.floor(Math.random() * 4) + 1;
    demoKeys += burst;
    demoRecentKeys += burst;
  }
  if (Math.random() < 0.18) {
    demoClicks += 1;
  }

  if (elKeys) elKeys.textContent = demoKeys.toLocaleString();
  if (elClicks) elClicks.textContent = demoClicks.toLocaleString();
}, 200);

// Slow tick — update peak + chart (every 1s)
setInterval(() => {
  demoSessionSeconds++;
  if (elUptime) elUptime.textContent = formatTime(demoSessionSeconds);

  // Update history every second with scaled value
  const rate = Math.round(demoRecentKeys * (60 / 1));
  history.push(Math.min(rate, 420));
  if (history.length > 28) history.shift();
  demoRecentKeys = 0;

  // Update peak (uses 60s rate estimate)
  const estRate = Math.round(demoKeys / Math.max(demoSessionSeconds, 1) * 60);
  if (estRate > demoPeak) demoPeak = estRate;
  if (elPeak) elPeak.textContent = demoPeak;

  drawMiniChart();
}, 1000);

// System metrics simulation
const cpuBase = 22 + Math.random() * 20;
const ramBase = 48 + Math.random() * 20;

function lerp(a, b, t) { return a + (b - a) * t; }

let cpuCur = cpuBase;
let ramCur = ramBase;
let diskCur = 0;
let netCur = 0;

function animateSystemMetrics() {
  // Drift values with gentle noise
  cpuCur = Math.max(5, Math.min(95, cpuCur + (Math.random() - 0.5) * 8));
  ramCur = Math.max(30, Math.min(88, ramCur + (Math.random() - 0.5) * 2));
  diskCur = Math.max(0, Math.min(100, diskCur + (Math.random() - 0.48) * 15));
  netCur = Math.max(0, Math.min(100, netCur + (Math.random() - 0.46) * 12));

  const cpuBar = document.getElementById('cpu-bar');
  const ramBar = document.getElementById('ram-bar');
  const diskBar = document.getElementById('disk-bar');
  const netBar = document.getElementById('net-bar');
  const cpuVal = document.getElementById('cpu-val');
  const ramVal = document.getElementById('ram-val');
  const diskVal = document.getElementById('disk-val');
  const netVal = document.getElementById('net-val');
  const uptimeVal = document.getElementById('sys-uptime-val');

  if (cpuBar) cpuBar.style.width = cpuCur.toFixed(0) + '%';
  if (ramBar) ramBar.style.width = ramCur.toFixed(0) + '%';
  if (diskBar) diskBar.style.width = Math.min(diskCur, 100).toFixed(0) + '%';
  if (netBar) netBar.style.width = Math.min(netCur, 100).toFixed(0) + '%';

  if (cpuVal) cpuVal.textContent = cpuCur.toFixed(0) + '%';
  if (ramVal) ramVal.textContent = ramCur.toFixed(0) + '%';
  if (diskVal) diskVal.textContent = (diskCur * 2.4).toFixed(0) + ' KB/s';
  if (netVal) netVal.textContent = (netCur * 0.8).toFixed(0) + ' KB/s';

  if (uptimeVal) {
    const baseHours = 4;
    const totalSecs = baseHours * 3600 + demoSessionSeconds;
    const h = Math.floor(totalSecs / 3600);
    const m = Math.floor((totalSecs % 3600) / 60);
    uptimeVal.textContent = `${h}h ${m}m`;
  }
}

setInterval(animateSystemMetrics, 1500);
animateSystemMetrics();
drawMiniChart();

/* ==========================================
   SMOOTH ANCHOR SCROLL
   ========================================== */
document.querySelectorAll('a[href^="#"]').forEach(a => {
  a.addEventListener('click', e => {
    const target = document.querySelector(a.getAttribute('href'));
    if (target) {
      e.preventDefault();
      target.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  });
});
