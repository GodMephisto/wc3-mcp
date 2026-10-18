---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "scripts/probe_translators.mjs"
ref: HEAD
---

// Probe which wc3maptranslator translators + mdx parsers actually work on a given map.
// Usage: node scripts/probe_translators.mjs <map.w3x>
import { createRequire } from 'node:module';
import { readFileSync } from 'node:fs';
const require = createRequire(import.meta.url);

const War3Map = require('mdx-m3-viewer/dist/cjs/parsers/w3x/map.js').default;
const wt = require('wc3maptranslator');

const map = new War3Map();
map.load(new Uint8Array(readFileSync(process.argv[2])), true);
const get = (n) => { const f = map.get(n); return f ? Buffer.from(f.bytes()) : null; };

function tryTranslator(label, file, fn) {
  const buf = get(file);
  if (!buf) { console.log(`  ${label.padEnd(12)} ${file.padEnd(18)} MISSING`); return; }
  try {
    const r = fn(buf);
    const j = r.json ?? r;
    const count = Array.isArray(j) ? j.length : Object.keys(j).length;
    console.log(`  ${label.padEnd(12)} ${file.padEnd(18)} OK  (${buf.length}b -> ${count} entries)`);
  } catch (e) {
    console.log(`  ${label.padEnd(12)} ${file.padEnd(18)} FAIL: ${e.message}`);
  }
}

console.log('=== wc3maptranslator (warToJson) ===');
tryTranslator('Strings',  'war3map.wts', (b) => wt.StringsTranslator.warToJson(b));
tryTranslator('Regions',  'war3map.w3r', (b) => wt.RegionsTranslator.warToJson(b));
tryTranslator('Cameras',  'war3map.w3c', (b) => wt.CamerasTranslator.warToJson(b));
tryTranslator('Sounds',   'war3map.w3s', (b) => wt.SoundsTranslator.warToJson(b));
tryTranslator('Terrain',  'war3map.w3e', (b) => wt.TerrainTranslator.warToJson(b));
tryTranslator('Doodads',  'war3map.doo', (b) => wt.DoodadsTranslator.warToJson(b));
tryTranslator('Units',    'war3mapUnits.doo', (b) => wt.UnitsTranslator.warToJson(b));
tryTranslator('Imports',  'war3map.imp', (b) => wt.ImportsTranslator.warToJson(b));
tryTranslator('Info',     'war3map.w3i', (b) => wt.InfoTranslator.warToJson(b));

console.log('\n=== mdx-m3-viewer parsers ===');
function tryMdx(label, file, modPath) {
  const f = map.get(file);
  if (!f) { console.log(`  ${label.padEnd(12)} ${file.padEnd(18)} MISSING`); return; }
  try {
    const Ctor = require(modPath).default;
    const p = new Ctor();
    const bv = (() => { try { return map.getMapInformation().getBuildVersion(); } catch { return 200; } })();
    // some parsers take buildVersion, some don't; try both
    try { p.load(f.bytes(), bv); } catch { p.load(f.bytes()); }
    console.log(`  ${label.padEnd(12)} ${file.padEnd(18)} OK  keys: ${Object.keys(p).join(',')}`);
  } catch (e) {
    console.log(`  ${label.padEnd(12)} ${file.padEnd(18)} FAIL: ${e.message}`);
  }
}
tryMdx('w3r',  'war3map.w3r', 'mdx-m3-viewer/dist/cjs/parsers/w3x/w3r/file.js');
tryMdx('w3c',  'war3map.w3c', 'mdx-m3-viewer/dist/cjs/parsers/w3x/w3c/file.js');
tryMdx('w3s',  'war3map.w3s', 'mdx-m3-viewer/dist/cjs/parsers/w3x/w3s/file.js');
tryMdx('w3e',  'war3map.w3e', 'mdx-m3-viewer/dist/cjs/parsers/w3x/w3e/file.js');
tryMdx('wtg',  'war3map.wtg', 'mdx-m3-viewer/dist/cjs/parsers/w3x/wtg/file.js');
tryMdx('imp',  'war3map.imp', 'mdx-m3-viewer/dist/cjs/parsers/w3x/imp/file.js');
tryMdx('wpm',  'war3map.wpm', 'mdx-m3-viewer/dist/cjs/parsers/w3x/wpm/file.js');
