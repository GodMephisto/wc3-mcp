---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "src/doodads.ts"
ref: HEAD
---

/**
 * war3map.doo — preplaced doodads & destructables (trees, rocks, buildings-as-scenery,
 * destructible gates, …). Mirrors the unit-placement tools in map.ts. Uses mdx-m3-viewer's
 * doo parser (the same one wc3_read_placements reads with), so encoding matches the editor.
 */
import { createRequire } from 'node:module';
import { writeRawFile } from './map.js';
import type { War3Map } from './map.js';

const require = createRequire(import.meta.url);

type DoodadInstance = {
  id: string;
  variation: number;
  location: Float32Array;
  angle: number;
  scale: Float32Array;
  skin: string;
  flags: number;
  life: number;
  editorId: number;
};
const DoodadCtor = require('mdx-m3-viewer/dist/cjs/parsers/w3x/doo/doodad.js').default as {
  new (): DoodadInstance;
};
const War3MapDooCtor = require('mdx-m3-viewer/dist/cjs/parsers/w3x/doo/file.js').default as {
  new (): { load(b: Uint8Array, bv: number): void; save(bv: number): Uint8Array; doodads: DoodadInstance[] };
};

const DEG2RAD = Math.PI / 180;
const RAD2DEG = 180 / Math.PI;

/** Pick the build version whose load→save round-trips byte-for-byte (mirrors the units
 *  logic): >131 carries a per-doodad skin field (Reforged), otherwise classic. */
function detectBuildVersion(bytes: Buffer): number {
  for (const bv of [200, 1]) {
    try {
      const probe = new War3MapDooCtor();
      probe.load(bytes, bv);
      if (Buffer.from(probe.save(bv)).equals(bytes)) return bv;
    } catch {
      /* try next */
    }
  }
  return 200;
}

function load(map: War3Map): { doo: InstanceType<typeof War3MapDooCtor>; buildVersion: number } {
  const file = map.get('war3map.doo');
  if (!file) throw new Error('war3map.doo not found in archive');
  const bytes = Buffer.from(file.bytes());
  const buildVersion = detectBuildVersion(bytes);
  const doo = new War3MapDooCtor();
  doo.load(bytes, buildVersion);
  return { doo, buildVersion };
}

function summary(d: DoodadInstance, index: number) {
  return {
    index,
    editorId: d.editorId,
    type: d.id,
    x: d.location[0],
    y: d.location[1],
    z: d.location[2],
    angle: Math.round(d.angle * RAD2DEG * 1000) / 1000,
    scale: d.scale[0],
    variation: d.variation,
    life: d.life,
  };
}

export interface DoodadSelector {
  editorId?: number;
  index?: number;
  type?: string;
}

function matches(d: DoodadInstance, index: number, sel: DoodadSelector): boolean {
  if (sel.editorId === undefined && sel.index === undefined && sel.type === undefined) {
    throw new Error('Selector must specify at least one of: editorId, index, type');
  }
  if (sel.editorId !== undefined && d.editorId !== sel.editorId) return false;
  if (sel.index !== undefined && index !== sel.index) return false;
  if (sel.type !== undefined && d.id !== sel.type) return false;
  return true;
}

export function listDoodads(map: War3Map, type?: string): object[] {
  const { doo } = load(map);
  return doo.doodads
    .map((d, i) => summary(d, i))
    .filter((d) => type === undefined || d.type === type);
}

export interface DoodadPlacement {
  type: string;
  x: number;
  y: number;
  z?: number;
  angle?: number; // degrees
  scale?: number;
  variation?: number;
  life?: number; // percent, default 100
  skin?: string;
}

export function placeDoodad(map: War3Map, p: DoodadPlacement): object {
  const { doo, buildVersion } = load(map);
  const d = new DoodadCtor();
  d.id = p.type;
  d.variation = p.variation ?? 0;
  d.location = new Float32Array([p.x, p.y, p.z ?? 0]);
  d.angle = (p.angle ?? 270) * DEG2RAD;
  const s = p.scale ?? 1;
  d.scale = new Float32Array([s, s, s]);
  if (buildVersion > 131) d.skin = p.skin ?? p.type;
  d.flags = 2; // normal: visible + solid (World Editor default)
  d.life = p.life ?? 100;
  d.editorId = doo.doodads.reduce((m, x) => Math.max(m, x.editorId), 0) + 1;
  doo.doodads.push(d);
  writeRawFile(map, 'war3map.doo', doo.save(buildVersion));
  return summary(d, doo.doodads.length - 1);
}

export interface DoodadChanges {
  x?: number;
  y?: number;
  z?: number;
  angle?: number; // degrees
  scale?: number;
  type?: string;
  variation?: number;
  life?: number;
}

export function modifyDoodads(map: War3Map, sel: DoodadSelector, set: DoodadChanges): object[] {
  const { doo, buildVersion } = load(map);
  const updated: object[] = [];
  doo.doodads.forEach((d, i) => {
    if (!matches(d, i, sel)) return;
    if (set.x !== undefined) d.location[0] = set.x;
    if (set.y !== undefined) d.location[1] = set.y;
    if (set.z !== undefined) d.location[2] = set.z;
    if (set.angle !== undefined) d.angle = set.angle * DEG2RAD;
    if (set.scale !== undefined) d.scale = new Float32Array([set.scale, set.scale, set.scale]);
    if (set.type !== undefined) {
      d.id = set.type;
      if (buildVersion > 131) d.skin = set.type;
    }
    if (set.variation !== undefined) d.variation = set.variation;
    if (set.life !== undefined) d.life = set.life;
    updated.push(summary(d, i));
  });
  if (updated.length > 0) writeRawFile(map, 'war3map.doo', doo.save(buildVersion));
  return updated;
}

export function deleteDoodads(map: War3Map, sel: DoodadSelector): object[] {
  const { doo, buildVersion } = load(map);
  const removed: object[] = [];
  const kept: DoodadInstance[] = [];
  doo.doodads.forEach((d, i) => {
    if (matches(d, i, sel)) removed.push(summary(d, i));
    else kept.push(d);
  });
  if (removed.length > 0) {
    doo.doodads = kept;
    writeRawFile(map, 'war3map.doo', doo.save(buildVersion));
  }
  return removed;
}
