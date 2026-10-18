---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "scripts/sweep.mjs"
ref: HEAD
---

// Read-robustness sweep: run buildOverview across many real maps, each in an isolated child
// process (bounded heap) so one huge/pathological map can't blow up the run and memory can't
// accumulate. Mirrors how the MCP server handles one map per tool call.
// Usage: node scripts/sweep.mjs <dir> [limit]
import { readdirSync, statSync } from 'node:fs';
import { join } from 'node:path';
import { execFileSync } from 'node:child_process';

const DIR = process.argv[2];
const LIMIT = parseInt(process.argv[3] ?? '30', 10);

function findMaps(dir, acc = []) {
  let entries;
  try { entries = readdirSync(dir); } catch { return acc; }
  for (const e of entries) {
    const p = join(dir, e);
    let st;
    try { st = statSync(p); } catch { continue; }
    if (st.isDirectory()) findMaps(p, acc);
    else if (/\.w3[xm]$/i.test(e)) acc.push(p);
  }
  return acc;
}

const pool = findMaps(DIR);
const step = Math.max(1, Math.floor(pool.length / LIMIT));
const maps = pool.filter((_, i) => i % step === 0).slice(0, LIMIT);
console.log(`Found ${pool.length} maps; sampling ${maps.length}.`);

// Tiny program run per-map in a child so memory is fully reclaimed between maps.
const child = `
import { openMap } from './dist/map.js';
import { buildOverview } from './dist/overview.js';
const ov = buildOverview(await openMap(process.argv[1], true));
const errs = Object.entries(ov).filter(([,v]) => v && typeof v==='object' && 'error' in v).map(([k])=>k);
const name = (ov.info?.name ?? '?').toString().replace(/\\|c[0-9a-fA-F]{8}|\\|r/g,'').slice(0,26);
console.log(JSON.stringify({ name, v: ov.info?.formatVersion, t: ov.terrain?.tilepoints, u: ov.placedUnits, d: ov.placedDoodads, errs }));
`;

let ok = 0, degraded = 0, fail = 0;
for (const mapPath of maps) {
  try {
    const out = execFileSync(process.execPath, ['--max-old-space-size=1536', '--input-type=module', '-e', child, mapPath],
      { encoding: 'utf8', timeout: 30000, stdio: ['ignore', 'pipe', 'pipe'] });
    const r = JSON.parse(out.trim().split('\n').pop());
    const t = r.t ? `${r.t.cols}x${r.t.rows}` : '?';
    if (r.errs.length) { degraded++; console.log(`  ⚠️  ${r.name.padEnd(28)} v${r.v} ${t} u=${r.u} d=${r.d} — degraded: ${r.errs.join(',')}`); }
    else { ok++; console.log(`  ✅ ${r.name.padEnd(28)} v${r.v} ${t} u=${r.u} d=${r.d}`); }
  } catch (e) {
    fail++;
    const reason = /heap limit/.test(e.stderr || '') ? 'OOM' : (e.stderr || e.message || '').split('\n').find((l) => /Error|premature|range/.test(l)) || 'crash';
    console.log(`  ❌ ${(mapPath.split(/[\\/]/).pop()).slice(0,40).padEnd(40)} — ${reason.slice(0,60)}`);
  }
}
console.log(`\nSWEEP: ${ok} clean, ${degraded} degraded-but-ok, ${fail} hard failures (of ${maps.length})`);
