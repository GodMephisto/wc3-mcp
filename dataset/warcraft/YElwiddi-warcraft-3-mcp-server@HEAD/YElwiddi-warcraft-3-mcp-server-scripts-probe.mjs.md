---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "scripts/probe.mjs"
ref: HEAD
---

/**
 * MCP integration probe: spawn the *built* server over stdio with the SDK client,
 * list tools, and call a few against a real map.
 * Usage: node scripts/probe.mjs [mapPath]
 */
import { Client } from '@modelcontextprotocol/sdk/client/index.js';
import { StdioClientTransport } from '@modelcontextprotocol/sdk/client/stdio.js';
import { resolve } from 'node:path';

const mapPath = resolve(process.argv[2] ?? 'tmp/TerenasStand.w3x');

const transport = new StdioClientTransport({ command: process.execPath, args: ['dist/index.js'] });
const client = new Client({ name: 'wc3-probe', version: '0.0.0' });
await client.connect(transport);

const text = (r) => r.content?.map((c) => c.text).join('') ?? '';

const { tools } = await client.listTools();
console.log(`▶ ${tools.length} tools: ${tools.map((t) => t.name).join(', ')}`);

const files = await client.callTool({ name: 'wc3_list_files', arguments: { mapPath } });
console.log('▶ wc3_list_files ->', text(files).split('\n').slice(0, 6).join(', '), '…');

const info = await client.callTool({ name: 'wc3_map_info', arguments: { mapPath } });
const infoJson = JSON.parse(text(info));
console.log('▶ wc3_map_info -> name=%o players=%o version=%o', infoJson.name, infoJson.players?.length, infoJson.version);

const script = await client.callTool({ name: 'wc3_read_script', arguments: { mapPath } });
console.log('▶ wc3_read_script ->', text(script).slice(0, 64).replace(/\s+/g, ' '), '…');

await client.close();
console.log('✅ PROBE OK');
