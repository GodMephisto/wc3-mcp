---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "scripts/add_trigger_demo.mjs"
ref: HEAD
---

// Inject a self-contained "footman grows 2x in the north region" trigger into
// a map's war3map.j, editing the map IN PLACE. Drives the same engine the MCP
// wc3_write_script tool uses.
// Usage: node scripts/add_trigger_demo.mjs <srcMap> [outMap]   (outMap defaults to in-place)
import { openMap, readScript, writeRawFile, saveMap } from '../dist/map.js';

const MAP = process.argv[2];
const OUT = process.argv[3] || MAP;
const map = await openMap(MAP, false);
const { content } = readScript(map);
const eol = content.includes('\r\n') ? '\r\n' : '\n';

const L = [
  '//===========================================================================',
  '// Custom demo trigger: a Footman doubles in size when it enters the north region.',
  '//===========================================================================',
  'function Demo_GrowActions takes nothing returns nothing',
  '    local unit u = GetTriggerUnit()',
  "    if GetUnitTypeId(u) == 'hfoo' then",
  '        call SetUnitScale( u, 2.00, 2.00, 2.00 )',
  '        call DisplayTimedTextToPlayer( GetOwningPlayer(u), 0, 0, 8.00, "The footman entered the north and grew twice as large!" )',
  '    endif',
  '    set u = null',
  'endfunction',
  '',
  'function Demo_Init takes nothing returns nothing',
  '    local trigger t = CreateTrigger()',
  '    local region reg = CreateRegion()',
  '    local rect re = Rect( -2048.0, 768.0, -768.0, 2048.0 )',
  "    local unit foot = CreateUnit( Player(0), 'hfoo', -1408.0, -1024.0, 90.0 )",
  '    call RegionAddRect( reg, re )',
  '    call TriggerRegisterEnterRegion( t, reg, null )',
  '    call TriggerAddAction( t, function Demo_GrowActions )',
  '    call IssuePointOrder( foot, "move", -1408.0, 1664.0 )',
  '    set t = null',
  '    set foot = null',
  'endfunction',
  '',
  '',
];
const FUNCS = L.join(eol);

const mainAnchor =
  '//===========================================================================' + eol + 'function main takes nothing returns nothing';
const endAnchor =
  '    call RunInitializationTriggers(  )' + eol + eol + 'endfunction';

if (!content.includes(mainAnchor) || !content.includes(endAnchor)) {
  throw new Error('injection anchors not found — script structure differs from expected');
}

const out = content
  .replace(mainAnchor, FUNCS + mainAnchor)
  .replace(endAnchor, '    call RunInitializationTriggers(  )' + eol + '    call Demo_Init(  )' + eol + eol + 'endfunction');

writeRawFile(map, 'war3map.j', new TextEncoder().encode(out));
await saveMap(map, OUT);
console.log(`injected trigger; war3map.j ${content.length} -> ${out.length} bytes, saved to ${OUT}`);
