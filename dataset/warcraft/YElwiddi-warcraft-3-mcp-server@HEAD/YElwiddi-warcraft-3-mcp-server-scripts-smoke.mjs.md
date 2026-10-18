---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "scripts/smoke.mjs"
ref: HEAD
---

/**
 * End-to-end smoke test for the wc3-mcp engine (dist/map.js).
 * Usage: node scripts/smoke.mjs <path-to-map.w3x>
 *
 * Exercises: MPQ container read (mdx) + WC3MapTranslator (war3map.* <-> JSON) at
 * runtime on this Node version, plus a write/save/reload round-trip.
 */
import { fileURLToPath } from 'node:url';
import { dirname, join } from 'node:path';
import {
  openMap,
  listFiles,
  readInfo,
  readScript,
  readObjects,
  writeObjects,
  saveMap,
  OBJECT_KINDS,
} from '../dist/map.js';

const here = dirname(fileURLToPath(import.meta.url));
const src = process.argv[2];
if (!src) {
  console.error('usage: node scripts/smoke.mjs <map.w3x>');
  process.exit(1);
}

const log = (...a) => console.log(...a);

log('▶ opening', src);
const map = await openMap(src, false);

const files = listFiles(map);
log(`▶ ${files.length} internal files; sample:`, files.slice(0, 12).join(', '));

try {
  const info = readInfo(map);
  log('▶ info: name=%o author=%o players=%o w3iVersion=%o', info?.name, info?.author, info?.players?.length, info?.version);
} catch (e) {
  log('  readInfo error:', e.message);
}

try {
  const { name, content } = readScript(map);
  log(`▶ script: ${name} (${content.length} chars)`);
} catch (e) {
  log('  readScript error:', e.message);
}

for (const kind of OBJECT_KINDS) {
  try {
    const t = readObjects(map, kind);
    const o = Object.keys(t.original ?? {}).length;
    const c = Object.keys(t.custom ?? {}).length;
    log(`▶ objects ${kind}: original=${o} custom=${c}`);
  } catch (e) {
    log(`  objects ${kind} error:`, e.message);
  }
}

// Round-trip with REAL data: inject a custom unit (renamed Footman with 9999 HP),
// save, reload, and verify the edit survived the full serialize/repack cycle.
const units = readObjects(map, 'units');
units.custom = units.custom ?? {};
units.custom['x000:hfoo'] = [
  { id: 'unam', type: 'string', value: 'Smoke Test Footman' },
  { id: 'uhpm', type: 'int', value: 9999 },
];
writeObjects(map, 'units', units);
const out = join(here, '..', 'tmp', 'roundtrip.w3x');
await saveMap(map, out);
log('▶ saved round-trip map ->', out);

const reopened = await openMap(out, true);
const got = readObjects(reopened, 'units').custom?.['x000:hfoo'] ?? [];
const name = got.find((m) => m.id === 'unam')?.value;
const hp = got.find((m) => m.id === 'uhpm')?.value;
log(`▶ reload custom unit x000:hfoo -> name=${JSON.stringify(name)} hp=${hp}`);

if (name === 'Smoke Test Footman' && hp === 9999) {
  log('✅ SMOKE OK (object round-trip verified)');
} else {
  log('❌ object round-trip mismatch');
  process.exit(1);
}
