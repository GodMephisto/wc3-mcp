---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "src/regions.ts"
ref: HEAD
---

/**
 * war3map.w3r — rectangular regions placed in the World Editor (the things JASS calls
 * `gg_rct_<Name>`). Round-trips byte-for-byte through wc3maptranslator, so read/edit/write
 * is lossless.
 */
import { createRequire } from 'node:module';
import { toArrayBuffer } from './map.js';
import type { War3Map } from './map.js';

const require = createRequire(import.meta.url);
const { RegionsTranslator } = require('wc3maptranslator') as {
  RegionsTranslator: typeof import('wc3maptranslator').RegionsTranslator;
};
// mdx's w3r parser is more tolerant than wc3maptranslator's (which throws on some empty/edge
// w3r files); used as a read-only fallback.
const War3MapW3r = require('mdx-m3-viewer/dist/cjs/parsers/w3x/w3r/file.js').default as {
  new (): {
    load(b: Uint8Array): void;
    regions: {
      left: number; right: number; bottom: number; top: number;
      name: string; creationNumber: number; weatherEffectId: string;
      ambientSound: string; color: ArrayLike<number>;
    }[];
  };
};

export interface Region {
  position: { left: number; bottom: number; right: number; top: number };
  name: string;
  id: number;
  weatherEffect: string; // 4-char code, or '' for none
  ambientSound: string; // sound variableName, or ''
  color: [number, number, number]; // r,g,b 0-255
}

function read(map: War3Map): Region[] {
  const file = map.get('war3map.w3r');
  if (!file) return [];
  try {
    return RegionsTranslator.warToJson(Buffer.from(file.bytes())).json as unknown as Region[];
  } catch (primary) {
    // wc3maptranslator chokes on some w3r files — try mdx, which is more tolerant.
    try {
      const w3r = new War3MapW3r();
      w3r.load(file.bytes());
      return w3r.regions.map((r) => ({
        position: { left: r.left, right: r.right, bottom: r.bottom, top: r.top },
        name: r.name,
        id: r.creationNumber,
        weatherEffect: r.weatherEffectId === '\0\0\0\0' ? '' : r.weatherEffectId,
        ambientSound: r.ambientSound,
        color: [r.color[2] ?? 0, r.color[1] ?? 0, r.color[0] ?? 0],
      }));
    } catch {
      // Both parsers failed — the table's header disagrees with its data (commonly a
      // protected/corrupted w3r). Report clearly instead of guessing.
      const bytes = file.bytes().length;
      throw new Error(
        `war3map.w3r appears malformed (${bytes} bytes; header/data mismatch — often map protection). Regions unreadable.`,
      );
    }
  }
}

function write(map: War3Map, regions: Region[]): void {
  const { buffer } = RegionsTranslator.jsonToWar(regions as never);
  map.set('war3map.w3r', toArrayBuffer(buffer));
}

export function readRegions(map: War3Map): Region[] {
  return read(map);
}

export interface NewRegion {
  left: number;
  right: number;
  bottom: number;
  top: number;
  name?: string;
  weatherEffect?: string;
  ambientSound?: string;
  color?: [number, number, number];
}

/** Append a region; returns the created region (with its assigned id). Caller saves. */
export function addRegion(map: War3Map, r: NewRegion): Region {
  const regions = read(map);
  const id = regions.reduce((m, x) => Math.max(m, x.id), -1) + 1;
  const region: Region = {
    position: { left: r.left, bottom: r.bottom, right: r.right, top: r.top },
    name: r.name ?? `Region_${id}`,
    id,
    weatherEffect: r.weatherEffect ?? '',
    ambientSound: r.ambientSound ?? '',
    color: r.color ?? [255, 255, 255],
  };
  regions.push(region);
  write(map, regions);
  return region;
}

export interface RegionSelector {
  id?: number;
  name?: string;
  index?: number;
}

function matches(r: Region, i: number, sel: RegionSelector): boolean {
  if (sel.id === undefined && sel.name === undefined && sel.index === undefined) {
    throw new Error('Region selector must specify one of: id, name, index');
  }
  if (sel.id !== undefined && r.id !== sel.id) return false;
  if (sel.name !== undefined && r.name !== sel.name) return false;
  if (sel.index !== undefined && i !== sel.index) return false;
  return true;
}

export interface RegionChanges {
  left?: number;
  right?: number;
  bottom?: number;
  top?: number;
  name?: string;
  weatherEffect?: string;
  ambientSound?: string;
  color?: [number, number, number];
}

export function modifyRegions(map: War3Map, sel: RegionSelector, set: RegionChanges): Region[] {
  const regions = read(map);
  const updated: Region[] = [];
  regions.forEach((r, i) => {
    if (!matches(r, i, sel)) return;
    if (set.left !== undefined) r.position.left = set.left;
    if (set.right !== undefined) r.position.right = set.right;
    if (set.bottom !== undefined) r.position.bottom = set.bottom;
    if (set.top !== undefined) r.position.top = set.top;
    if (set.name !== undefined) r.name = set.name;
    if (set.weatherEffect !== undefined) r.weatherEffect = set.weatherEffect;
    if (set.ambientSound !== undefined) r.ambientSound = set.ambientSound;
    if (set.color !== undefined) r.color = set.color;
    updated.push(r);
  });
  if (updated.length > 0) write(map, regions);
  return updated;
}

export function deleteRegions(map: War3Map, sel: RegionSelector): Region[] {
  const regions = read(map);
  const removed: Region[] = [];
  const kept = regions.filter((r, i) => {
    if (matches(r, i, sel)) {
      removed.push(r);
      return false;
    }
    return true;
  });
  if (removed.length > 0) write(map, kept);
  return removed;
}
