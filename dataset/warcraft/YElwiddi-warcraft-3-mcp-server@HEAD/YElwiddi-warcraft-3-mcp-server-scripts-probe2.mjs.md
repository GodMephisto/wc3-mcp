---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "scripts/probe2.mjs"
ref: HEAD
---

/**
 * Extended MCP integration probe: spawn the built server over stdio and exercise the new
 * tools the way Claude would. Usage: node scripts/probe2.mjs <srcMap> <outMap>
 */
import { Client } from '@modelcontextprotocol/sdk/client/index.js';
import { StdioClientTransport } from '@modelcontextprotocol/sdk/client/stdio.js';
import { resolve } from 'node:path';
import { copyFileSync } from 'node:fs';

const SRC = resolve(process.argv[2] ?? 'tmp/TerenasStand.w3x');
const OUT = resolve(process.argv[3] ?? 'tmp/probe2.w3x');
copyFileSync(SRC, OUT);

const transport = new StdioClientTransport({ command: process.execPath, args: ['dist/index.js'] });
const client = new Client({ name: 'wc3-probe2', version: '0.0.0' });
await client.connect(transport);
const text = (r) => r.content?.map((c) => c.text).join('') ?? '';
const callJson = async (name, args) => {
  const r = await client.callTool({ name, arguments: args });
  if (r.isError) throw new Error(`${name} errored: ${text(r)}`);
  try { return JSON.parse(text(r)); } catch { return text(r); }
};

let pass = 0, fail = 0;
const check = (l, c, d) => { c ? pass++ : fail++; console.log(`  ${c ? '✅' : '❌'} ${l}${d ? ' — ' + d : ''}`); };

const { tools } = await client.listTools();
console.log(`▶ ${tools.length} tools registered`);
const names = tools.map((t) => t.name);
for (const expected of ['wc3_overview', 'wc3_read_terrain', 'wc3_read_regions', 'wc3_add_region', 'wc3_create_object', 'wc3_add_trigger', 'wc3_open_in_editor', 'wc3_test_map', 'wc3_read_strings', 'wc3_place_doodad']) {
  check(`tool registered: ${expected}`, names.includes(expected));
}

console.log('\n▶ wc3_overview');
const ov = await callJson('wc3_overview', { mapPath: OUT });
check('overview has resolved name', ov.info?.name === 'Terenas Stand LV', `name=${ov.info?.name}`);
check('overview has terrain', !!ov.terrain?.tilepoints, `${ov.terrain?.tilepoints?.cols}x${ov.terrain?.tilepoints?.rows}`);
check('overview placed counts', ov.placedUnits === 107 && ov.placedDoodads === 1680, `units=${ov.placedUnits} doodads=${ov.placedDoodads}`);

console.log('\n▶ write round-trip via MCP: add_region -> read_regions');
const add = await callJson('wc3_add_region', { mapPath: OUT, left: -256, right: 256, bottom: -256, top: 256, name: 'McpRegion', outPath: OUT });
check('add_region returned saved path', !!add.saved, `id=${add.result?.id}`);
const regions = await callJson('wc3_read_regions', { mapPath: OUT });
check('region readable through MCP after save', regions.some((r) => r.name === 'McpRegion'), `${regions.length} region(s)`);

console.log('\n▶ create_object via MCP');
const co = await callJson('wc3_create_object', { mapPath: OUT, kind: 'units', baseId: 'hfoo', newId: 'x123',
  fields: [{ id: 'unam', type: 'string', value: 'MCP Footman' }], outPath: OUT });
check('create_object saved', !!co.saved);
const objs = await callJson('wc3_read_objects', { mapPath: OUT, kind: 'units' });
check('custom object present', !!objs.custom?.['x123:hfoo']);

await client.close();
console.log(`\n${fail === 0 ? '✅ MCP PROBE OK' : '❌ MCP PROBE FAILED'}: ${pass} passed, ${fail} failed`);
process.exit(fail === 0 ? 0 : 1);
