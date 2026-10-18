---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "scripts/verify_phase2.mjs"
ref: HEAD
---

// End-to-end exercise of ALL phase-2 subsystems through the compiled engine.
// Chains edits onto a copy of the source map, reopening after each save to prove the
// archive stays valid and the change persisted. Usage: node scripts/verify_phase2.mjs <src> <out>
import { copyFileSync } from 'node:fs';
import { openMap, saveMap, readInfo, readObjects, readPlacements } from '../dist/map.js';
import { readStrings, resolveTrigStr, setString } from '../dist/strings.js';
import { readRegions, addRegion, modifyRegions, deleteRegions } from '../dist/regions.js';
import { readCameras, addCamera, deleteCameras } from '../dist/cameras.js';
import { readSounds } from '../dist/sounds.js';
import { readTerrain } from '../dist/terrain.js';
import { readTriggers } from '../dist/triggers.js';
import { readImports } from '../dist/imports.js';
import { listDoodads, placeDoodad, modifyDoodads, deleteDoodads } from '../dist/doodads.js';
import { createObject, addTrigger } from '../dist/authoring.js';
import { buildOverview } from '../dist/overview.js';

const [, , SRC, OUT] = process.argv;
copyFileSync(SRC, OUT);
let pass = 0, fail = 0;
const check = (label, cond, detail) => {
  if (cond) { pass++; console.log(`  ✅ ${label}${detail ? ' — ' + detail : ''}`); }
  else { fail++; console.log(`  ❌ ${label}${detail ? ' — ' + detail : ''}`); }
};
const ro = async (fn) => fn(await openMap(OUT, true));
const rw = async (fn) => { const m = await openMap(OUT, false); const r = fn(m); await saveMap(m, OUT); return r; };

console.log('\n=== READS ===');
const info = await ro(readInfo);
check('map_info resolves TRIGSTR name', !String(info.name).startsWith('TRIGSTR_'), `name="${info.name}"`);
const strings = await ro(readStrings);
check('read_strings returns entries', Object.keys(strings).length > 0, `${Object.keys(strings).length} strings`);
const terrain = await ro(readTerrain);
check('read_terrain summary', terrain.tilepoints?.cols > 0, `${terrain.tilepoints?.cols}x${terrain.tilepoints?.rows} tileset=${terrain.tileset}`);
const triggers = await ro(readTriggers);
check('read_triggers', 'hasGuiTriggers' in triggers, `gui=${triggers.hasGuiTriggers} cats=${triggers.categories?.length}`);
const imports = await ro(readImports);
check('read_imports', imports !== undefined, `${imports.files?.length ?? '?'} files`);
const sounds = await ro(readSounds);
check('read_sounds', Array.isArray(sounds), `${sounds.length} sounds`);
const overview = await ro(buildOverview);
check('overview assembled', !!overview.info && !!overview.terrain, `units=${overview.placedUnits} doodads=${overview.placedDoodads}`);

console.log('\n=== REGIONS (add/read/modify/delete) ===');
const added = await rw((m) => addRegion(m, { left: -512, right: 512, bottom: -512, top: 512, name: 'TestArena', color: [255, 0, 0] }));
check('add_region', added.name === 'TestArena', `id=${added.id}`);
let regions = await ro(readRegions);
check('region persisted after reopen', regions.some((r) => r.name === 'TestArena'), `${regions.length} regions`);
await rw((m) => modifyRegions(m, { name: 'TestArena' }, { name: 'RenamedArena', top: 1024 }));
regions = await ro(readRegions);
check('modify_region persisted', regions.some((r) => r.name === 'RenamedArena' && r.position.top === 1024));
await rw((m) => deleteRegions(m, { name: 'RenamedArena' }));
regions = await ro(readRegions);
check('delete_region persisted', !regions.some((r) => r.name === 'RenamedArena'), `${regions.length} regions`);

console.log('\n=== CAMERAS (add/read/delete) ===');
const cam = await rw((m) => addCamera(m, { x: 100, y: 200, name: 'Intro', distance: 2000 }));
check('add_camera', cam.name === 'Intro', `dist=${cam.distance}`);
let cams = await ro(readCameras);
check('camera persisted', cams.some((c) => c.name === 'Intro' && c.target.x === 100));
await rw((m) => deleteCameras(m, { name: 'Intro' }));
cams = await ro(readCameras);
check('delete_camera persisted', !cams.some((c) => c.name === 'Intro'), `${cams.length} cameras`);

console.log('\n=== DOODADS (list/place/modify/delete) ===');
const beforeD = (await ro(readPlacements)).doodads.length;
const pd = await rw((m) => placeDoodad(m, { type: 'LTlt', x: 0, y: 0, scale: 2 }));
check('place_doodad', pd.type === 'LTlt', `editorId=${pd.editorId}`);
const afterD = (await ro(readPlacements)).doodads.length;
check('doodad count increased', afterD === beforeD + 1, `${beforeD} -> ${afterD}`);
await rw((m) => modifyDoodads(m, { editorId: pd.editorId }, { x: 256, scale: 3 }));
let placedD = await ro((m) => listDoodads(m, 'LTlt'));
check('modify_doodad persisted', placedD.some((d) => d.editorId === pd.editorId && d.x === 256 && d.scale === 3));
await rw((m) => deleteDoodads(m, { editorId: pd.editorId }));
check('delete_doodad persisted', (await ro(readPlacements)).doodads.length === beforeD);

console.log('\n=== OBJECT DATA (create_object) ===');
await rw((m) => createObject(m, 'units', 'hfoo', 'x999', [
  { id: 'unam', type: 'string', value: 'Test Champion' },
  { id: 'uhpm', type: 'int', value: 5000 },
]));
const units = await ro((m) => readObjects(m, 'units'));
check('create_object persisted', !!units.custom?.['x999:hfoo'], `custom keys: ${Object.keys(units.custom || {}).join(',')}`);

console.log('\n=== SCRIPT (add_trigger, JASS) ===');
const jass = [
  'function Verify_Init takes nothing returns nothing',
  '    call BJDebugMsg("phase2 trigger online")',
  'endfunction',
].join('\n');
const at = await rw((m) => addTrigger(m, { code: jass, entryFunction: 'Verify_Init' }));
check('add_trigger applied', at.bytesAfter > at.bytesBefore, `+${at.bytesAfter - at.bytesBefore} bytes, wired=${at.wiredToInit}`);
const script = await ro((m) => { const s = openScript(m); return s; });
function openScript(m) { return m.getScriptFile().text(); }
check('script contains new function', script.includes('function Verify_Init'));
check('script wired into main', /call Verify_Init\(\)/.test(script));

console.log('\n=== STRINGS (set_string) ===');
await rw((m) => setString(m, 1, 'Renamed By Test'));
check('set_string persisted', (await ro(readStrings))['1'] === 'Renamed By Test');

console.log('\n=== FINAL INTEGRITY: reopen + full overview ===');
const finalOv = await ro(buildOverview);
check('final map reopens & overview builds', !!finalOv.info && typeof finalOv.files === 'number', `files=${finalOv.files}`);

console.log(`\n${fail === 0 ? '✅ ALL PASS' : '❌ FAILURES'}: ${pass} passed, ${fail} failed`);
process.exit(fail === 0 ? 0 : 1);
