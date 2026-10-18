---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "scripts/verify_wtg.mjs"
ref: HEAD
---

// Corpus check for the wtg parser: every modern-format war3map.wtg must parse and
// re-serialize BYTE-IDENTICALLY (the same invariant the runtime safety gate enforces).
// Also verifies the wct mapping (entries == non-comment triggers) and reports which
// conventions the World Editor uses inside function-parameters (informs authoring code).
// Usage: node scripts/verify_wtg.mjs <dir-or-map> [more...]
import { readdirSync, readFileSync, statSync } from 'node:fs';
import { join } from 'node:path';
import { createRequire } from 'node:module';
import { parseWtg, writeWtg, triggersOf } from '../dist/wtg.js';
import { readWct } from '../dist/wct.js';

const require = createRequire(import.meta.url);
const War3Map = require('mdx-m3-viewer/dist/cjs/parsers/w3x/map.js').default;

function findMaps(p, acc = []) {
  let st;
  try { st = statSync(p); } catch { return acc; }
  if (st.isFile()) {
    if (/\.(w3[xm]|wtg)$/i.test(p)) acc.push(p);
    return acc;
  }
  for (const e of readdirSync(p)) findMaps(join(p, e), acc);
  return acc;
}

const maps = process.argv.slice(2).flatMap((p) => findMaps(p));
const stats = { total: 0, noWtg: 0, loadFail: 0, classic: 0, modernOk: 0, roundtripFail: 0, parseErr: 0 };
const conv = { subFlag0ArgZero: 0, subFlag0ArgOne: 0, valueEqSubName: 0, valueNeSubName: 0 };
const parseErrors = new Map();

function scanParams(params) {
  for (const p of params ?? []) {
    if (p.hasSub && p.sub) {
      if (p.sub.params.length === 0) {
        if (p.sub.paramsFlag === 0) conv.subFlag0ArgZero++;
        else conv.subFlag0ArgOne++;
      }
      if (p.value === p.sub.name) conv.valueEqSubName++;
      else conv.valueNeSubName++;
      scanParams(p.sub.params);
    }
    if (p.index) scanParams([p.index]);
  }
}

for (const mapPath of maps) {
  stats.total++;
  const short = mapPath.split(/[\\/]/).pop().slice(0, 38).padEnd(38);
  let bytes;
  let m = null;
  if (/\.wtg$/i.test(mapPath)) {
    bytes = readFileSync(mapPath); // bare war3map.wtg fixture
  } else {
    try {
      m = new War3Map();
      m.load(new Uint8Array(readFileSync(mapPath)), true);
    } catch (e) {
      stats.loadFail++;
      console.log(`${short} MAP-LOAD-FAIL ${e.message.slice(0, 50)}`);
      continue;
    }
    const wtgFile = m.get('war3map.wtg');
    if (!wtgFile) { stats.noWtg++; console.log(`${short} no-wtg`); continue; }
    bytes = Buffer.from(wtgFile.bytes());
  }
  let wtg;
  try {
    wtg = parseWtg(bytes);
  } catch (e) {
    stats.parseErr++;
    const key = e.message.slice(0, 70);
    parseErrors.set(key, (parseErrors.get(key) ?? 0) + 1);
    console.log(`${short} PARSE-ERR ${key}`);
    continue;
  }
  if (!wtg.modern) {
    stats.classic++;
    console.log(`${short} classic v${wtg.formatVersion} (read-only) triggers=${triggersOf(wtg).length}`);
    continue;
  }
  const re = writeWtg(wtg);
  const triggers = triggersOf(wtg);
  for (const t of triggers) for (const f of t.functions) { scanParams(f.params); (function walk(fs){ for (const c of fs) { scanParams(c.params); walk(c.children); } })(f.children); }

  // wct mapping check
  let wctNote = 'no-wct';
  const wctFile = m?.get('war3map.wct');
  if (wctFile) {
    try {
      const wct = readWct(Buffer.from(wctFile.bytes()));
      const expected = triggers.filter((t) => !t.isComment).length;
      wctNote = wct.triggers.length === expected ? `wct=${wct.triggers.length}✓` : `WCT-MISMATCH entries=${wct.triggers.length} expected=${expected}`;
    } catch (e) {
      wctNote = `wct-parse-err(${e.message.slice(0, 25)})`;
    }
  }

  if (re.equals(bytes)) {
    stats.modernOk++;
    console.log(`${short} OK ${bytes.length}b triggers=${triggers.length} vars=${wtg.variables.length} ${wctNote}`);
  } else {
    stats.roundtripFail++;
    let at = -1;
    for (let i = 0; i < Math.min(re.length, bytes.length); i++) if (re[i] !== bytes[i]) { at = i; break; }
    console.log(`${short} ROUNDTRIP-FAIL at=${at} ${bytes.length}->${re.length} ${wctNote}`);
  }
}

console.log('\n==== summary ====');
console.log(stats);
console.log('editor conventions:', conv);
if (parseErrors.size) {
  console.log('parse errors:');
  for (const [msg, n] of parseErrors) console.log(`  ${n}x ${msg}`);
}
