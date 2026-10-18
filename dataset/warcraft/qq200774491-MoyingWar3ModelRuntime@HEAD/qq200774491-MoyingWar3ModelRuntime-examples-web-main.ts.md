---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "examples/web/main.ts"
ref: HEAD
---

import { createWar3BabylonViewer } from '../../src/index';
import type { War3Viewer, War3ViewerWarning } from '../../src/types';

// ---- 资源清单：把候选路径映射到真实文件，逻辑与 Node 冒烟测试一致 ----
interface Manifest { files: string[]; models: string[]; }

const manifest: Manifest = await (await fetch('/model-manifest.json')).json();
const byRel = new Map<string, string>();
const byBase = new Map<string, string>();
for (const rel of manifest.files) {
  const low = rel.toLowerCase();
  byRel.set(low, rel);
  const base = low.split('/').pop() ?? low;
  if (!byBase.has(base)) byBase.set(base, rel);
}

function resolveRel(candidatePaths: readonly string[], requestedPath: string): string | undefined {
  for (const raw of [...candidatePaths, requestedPath]) {
    const norm = raw.replace(/\\/g, '/').toLowerCase();
    const rel = byRel.get(norm)
      ?? byRel.get(norm.replace(/^.*?模型\//, ''))
      ?? byBase.get(norm.split('/').pop() ?? '');
    if (rel) return rel;
  }
  return undefined;
}

function assetUrl(rel: string): string {
  return '/model-assets/' + rel.split('/').map(encodeURIComponent).join('/');
}

// ---- DOM ----
const $ = <T extends HTMLElement>(id: string) => document.getElementById(id) as T;
const canvas = $('viewer') as HTMLCanvasElement;
const statusEl = $('status');
const listEl = $('modelList');
const playPause = $('playPause') as HTMLButtonElement;
const seqSel = $('sequences') as HTMLSelectElement;
const speed = $('speed') as HTMLInputElement;
const speedVal = $('speedVal');
const team = $('team') as HTMLInputElement;
const teamSwatch = $('teamSwatch');
const resetCam = $('resetCam') as HTMLButtonElement;
const webgpuToggle = $('webgpu') as HTMLInputElement;

const TEAM_HEX = ['#cc0d0d', '#1a40f2', '#1abfbf', '#7319a6', '#f2e60d', '#f27309', '#1ab333', '#f266a6',
  '#8c8c8c', '#73bff2', '#194d19', '#734019', '#731919', '#0d1973', '#0d6666', '#3f0d59',
  '#807319', '#803319', '#0d5919', '#803359', '#404040', '#33668c', '#0d330d', '#402619'];

let viewer: War3Viewer | null = null;
let currentBackend = 'webgl2';
let playing = false;
let activeItem: HTMLElement | null = null;

function setStatus(html: string) { statusEl.innerHTML = html; }

async function ensureViewer(): Promise<War3Viewer> {
  const wantGpu = webgpuToggle.checked;
  if (viewer && (currentBackend === 'webgpu') === wantGpu) return viewer;
  viewer?.dispose();
  viewer = await createWar3BabylonViewer(canvas, {
    preferWebGPU: wantGpu,
    onWarning: (w) => reportWarning(w),
    resolveResource: async ({ candidatePaths, requestedPath, signal }) => {
      const rel = resolveRel(candidatePaths, requestedPath);
      if (!rel) return undefined;
      const res = await fetch(assetUrl(rel), { signal });
      if (!res.ok) return undefined;
      return { bytes: await res.arrayBuffer(), resolvedPath: rel };
    },
  });
  currentBackend = (viewer as unknown as { backend: string }).backend ?? 'webgl2';
  return viewer;
}

const warnings: War3ViewerWarning[] = [];
function reportWarning(w: War3ViewerWarning) { warnings.push(w); }

async function loadModel(rel: string, item: HTMLElement) {
  activeItem?.classList.remove('active');
  item.classList.add('active');
  activeItem = item;
  warnings.length = 0;
  setStatus(`<span class="spin">加载 ${rel} …</span>`);
  playPause.disabled = true;
  seqSel.disabled = true;

  try {
    const v = await ensureViewer();
    const t0 = performance.now();
    const bytes = await (await fetch(assetUrl(rel))).arrayBuffer();
    const result = await v.loadModel({ path: rel, bytes, autoPlay: true });
    const ms = (performance.now() - t0).toFixed(0);

    seqSel.innerHTML = '';
    for (const s of result.sequences) {
      const o = document.createElement('option');
      o.value = String(s.index);
      o.textContent = `${s.name} (${s.duration}ms)`;
      seqSel.appendChild(o);
    }
    seqSel.disabled = result.sequences.length === 0;
    playPause.disabled = result.sequences.length === 0;
    playing = result.sequences.length > 0;
    playPause.textContent = playing ? '⏸ 暂停' : '▶ 播放';

    const missing = warnings.filter((w) => w.code === 'missing-resource').length;
    const feats = [...new Set(warnings.filter((w) => w.code === 'unsupported-feature').map((w) => w.feature))];
    const b = result.bounds;
    const size = Math.max(...[0, 1, 2].map((i) => b.maximum[i] - b.minimum[i])).toFixed(0);
    setStatus(
      `<span class="ok">✓ ${rel}</span> · ${result.format} · ${currentBackend}<br>` +
      `动画 ${result.sequences.length} · 请求 ${result.timings.requestCount} · 尺寸 ${size} · ${ms}ms` +
      (missing ? `<br><span class="warn">缺失贴图 ${missing}（已用占位色）</span>` : '') +
      (feats.length ? `<br><span class="warn">未渲染特性: ${feats.join(', ')}</span>` : ''),
    );
  } catch (err) {
    const e = err as Error;
    setStatus(`<span class="err">✗ 加载失败: ${e.name}: ${e.message}</span>`);
    console.error(err);
  }
}

// ---- 控件 ----
playPause.addEventListener('click', () => {
  if (!viewer) return;
  if (playing) { viewer.pause(); playing = false; playPause.textContent = '▶ 播放'; }
  else { viewer.playSequence(Number(seqSel.value) || 0); playing = true; playPause.textContent = '⏸ 暂停'; }
});
seqSel.addEventListener('change', () => {
  viewer?.playSequence(Number(seqSel.value) || 0);
  playing = true; playPause.textContent = '⏸ 暂停';
});
speed.addEventListener('input', () => {
  speedVal.textContent = Number(speed.value).toFixed(1) + '×';
  viewer?.setPlaybackSpeed(Number(speed.value));
});
team.addEventListener('input', () => {
  const idx = Number(team.value);
  teamSwatch.style.background = TEAM_HEX[idx] ?? '#cc0d0d';
  viewer?.setTeamColor(idx);
});
resetCam.addEventListener('click', () => viewer?.resetCamera());
webgpuToggle.addEventListener('change', () => {
  if (activeItem) { const rel = activeItem.dataset.rel!; loadModel(rel, activeItem); }
});
window.addEventListener('resize', () => viewer?.resize());
teamSwatch.style.background = TEAM_HEX[0];

// ---- 模型列表 ----
listEl.innerHTML = '';
if (manifest.models.length === 0) {
  listEl.innerHTML = '<div class="spin" style="padding:10px">未找到模型（test/模型 下无 .mdx/.mdl）</div>';
}
for (const rel of manifest.models) {
  const item = document.createElement('div');
  item.className = 'item';
  item.textContent = rel;
  item.title = rel;
  item.dataset.rel = rel;
  item.addEventListener('click', () => loadModel(rel, item));
  listEl.appendChild(item);
}

// 自动加载第一个模型
if (listEl.firstElementChild instanceof HTMLElement) {
  listEl.firstElementChild.click();
}
