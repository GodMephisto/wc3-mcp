---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "scripts/verify_phase1.mjs"
ref: HEAD
---

// Exercise the compiled server engine exactly as the new MCP tools do.
// Usage: node scripts/verify_phase1.mjs <srcMap> <outMap>
import { openMap, saveMap, readInfo, listUnits, placeUnit, modifyUnits, deleteUnits } from '../dist/map.js';

const [, , SRC, OUT] = process.argv;
const line = (label, v) => console.log(`\n=== ${label} ===\n` + JSON.stringify(v, null, 2));

// 1) map_info must no longer crash (graceful partial on Reforged 2.0 w3i v33)
const info = await openMap(SRC, true).then(readInfo);
line('wc3_map_info (was crashing)', { partial: info.partial, name: info.name, formatVersion: info.formatVersion, gameBuildVersion: info.gameBuildVersion, playableSize: info.playableSize });

// 2) place a ghoul at centre, then SAVE (this is the write path that used to crash)
let map = await openMap(SRC, false);
const placed = placeUnit(map, { type: 'ugho', x: 0, y: 0, facing: 270, player: 0 });
await saveMap(map, OUT);
line('wc3_place_unit -> saved', placed);

// 3) read it back
map = await openMap(OUT, true);
line('wc3_list_units after place', listUnits(map));

// 4) move + re-own + re-face the ghoul, overwrite in place
map = await openMap(OUT, false);
const moved = modifyUnits(map, { type: 'ugho' }, { x: 512, y: -256, player: 1, facing: 90 });
await saveMap(map, OUT);
line('wc3_modify_unit', moved);

// 5) delete it, overwrite in place
map = await openMap(OUT, false);
const removed = deleteUnits(map, { type: 'ugho' });
await saveMap(map, OUT);
line('wc3_delete_unit', removed);

map = await openMap(OUT, true);
line('final units (ghoul gone, originals intact)', listUnits(map));
