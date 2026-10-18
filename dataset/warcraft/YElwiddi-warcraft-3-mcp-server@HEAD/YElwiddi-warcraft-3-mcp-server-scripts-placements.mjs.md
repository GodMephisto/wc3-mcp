---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "scripts/placements.mjs"
ref: HEAD
---

/**
 * Dump the units & doodads placed on a map (war3mapUnits.doo / war3map.doo).
 * Usage: node scripts/placements.mjs <map.w3x>
 */
import { resolve } from 'node:path';
import { openMap, readPlacements } from '../dist/map.js';

const mapPath = resolve(process.argv[2] ?? 'tmp/TerenasStand.w3x');
const map = await openMap(mapPath, true);
const { units, doodads } = readPlacements(map);

console.log(`▶ ${mapPath}`);
console.log(`▶ ${units.length} placed units/items, ${doodads.length} doodads/destructables`);

const tally = (arr) => {
  const counts = {};
  for (const o of arr) counts[o.type] = (counts[o.type] ?? 0) + 1;
  return Object.entries(counts)
    .sort((a, b) => b[1] - a[1])
    .slice(0, 10)
    .map(([t, n]) => `${t}×${n}`)
    .join(', ');
};

console.log('▶ top unit types:   ', tally(units) || '(none)');
console.log('▶ top doodad types: ', tally(doodads) || '(none)');
console.log('▶ sample units:', JSON.stringify(units.slice(0, 3)));
console.log('▶ sample doodads:', JSON.stringify(doodads.slice(0, 3)));
