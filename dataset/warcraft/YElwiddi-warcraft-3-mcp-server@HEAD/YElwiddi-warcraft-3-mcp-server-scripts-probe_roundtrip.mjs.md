---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "scripts/probe_roundtrip.mjs"
ref: HEAD
---

// Verify warToJson -> jsonToWar -> warToJson round-trips for each translator.
// Usage: node scripts/probe_roundtrip.mjs <map.w3x>
import { createRequire } from 'node:module';
import { readFileSync } from 'node:fs';
const require = createRequire(import.meta.url);
const War3Map = require('mdx-m3-viewer/dist/cjs/parsers/w3x/map.js').default;
const wt = require('wc3maptranslator');

const map = new War3Map();
map.load(new Uint8Array(readFileSync(process.argv[2])), true);
const get = (n) => { const f = map.get(n); return f ? Buffer.from(f.bytes()) : null; };

function rt(label, file, T) {
  const buf = get(file);
  if (!buf) { console.log(`  ${label.padEnd(10)} MISSING`); return; }
  try {
    const json = T.warToJson(buf).json;
    const war = T.jsonToWar(json).buffer;
    const json2 = T.warToJson(Buffer.from(war)).json;
    const same = JSON.stringify(json) === JSON.stringify(json2);
    const byteSame = Buffer.from(war).equals(buf);
    console.log(`  ${label.padEnd(10)} json-stable=${same} byte-identical=${byteSame} (${buf.length}b -> ${war.byteLength}b)`);
  } catch (e) {
    console.log(`  ${label.padEnd(10)} FAIL: ${e.message}`);
  }
}

console.log('=== round-trip: warToJson -> jsonToWar -> warToJson ===');
rt('Strings', 'war3map.wts', wt.StringsTranslator);
rt('Regions', 'war3map.w3r', wt.RegionsTranslator);
rt('Cameras', 'war3map.w3c', wt.CamerasTranslator);
rt('Doodads', 'war3map.doo', wt.DoodadsTranslator);
rt('Units',   'war3mapUnits.doo', wt.UnitsTranslator);
rt('Imports', 'war3map.imp', wt.ImportsTranslator);

// Synthesize regions/cameras to prove the WRITE path even when source has none.
console.log('\n=== synth write: build JSON -> war -> reparse ===');
try {
  const regions = [{ position: [-100, -100, 100, 100], name: 'TestRegion', id: 0,
    weatherEffect: { enabled: false }, ambientSound: '', color: [255,0,0] }];
  const war = wt.RegionsTranslator.jsonToWar(regions).buffer;
  const back = wt.RegionsTranslator.warToJson(Buffer.from(war)).json;
  console.log('  Regions synth ->', JSON.stringify(back));
} catch (e) { console.log('  Regions synth FAIL:', e.message); }
