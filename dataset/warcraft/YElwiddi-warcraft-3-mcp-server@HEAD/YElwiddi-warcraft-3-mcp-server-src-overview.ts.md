---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "src/overview.ts"
ref: HEAD
---

/**
 * wc3_overview — a single-call snapshot of (almost) every editor subsystem, so the agent can
 * orient on a map without firing ten read tools. Each section is guarded independently: a
 * parser failing on one subsystem degrades that section to an error string, never the whole
 * report.
 */
import {
  readInfo,
  readPlacements,
  readScript,
  listFiles,
  OBJECT_KINDS,
} from './map.js';
import { objectTableInfo } from './objects-legacy.js';
import { readRegions } from './regions.js';
import { readCameras } from './cameras.js';
import { readSounds } from './sounds.js';
import { readTerrain } from './terrain.js';
import { readTriggers } from './triggers.js';
import { readImports } from './imports.js';
import type { War3Map } from './map.js';

function guard<T>(fn: () => T): T | { error: string } {
  try {
    return fn();
  } catch (e) {
    return { error: e instanceof Error ? e.message : String(e) };
  }
}

export function buildOverview(map: War3Map): object {
  const info = guard(() => readInfo(map)) as Record<string, unknown>;
  const placements = guard(() => readPlacements(map)) as { units?: unknown[]; doodads?: unknown[] };
  const script = guard(() => {
    const s = readScript(map);
    return { file: s.name, language: s.name.endsWith('.lua') ? 'lua' : 'jass', chars: s.content.length };
  });

  // Which object tables carry data, with their format version (cheap header probe — does NOT
  // materialize the full modification tables, so this stays light on very large maps). Use
  // wc3_read_objects for the actual entries.
  const objects = guard(() => {
    const tables: Record<string, { version: number; bytes: number }> = {};
    for (const kind of OBJECT_KINDS) {
      const t = objectTableInfo(map, kind);
      if (t && t.bytes > 8) tables[kind] = t;
    }
    return tables;
  });

  const regions = guard(() => readRegions(map));
  const cameras = guard(() => readCameras(map));
  const sounds = guard(() => readSounds(map));
  const triggers = guard(() => readTriggers(map)) as Record<string, unknown>;

  const names = <T extends { name: string }>(v: T[] | { error: string }) =>
    Array.isArray(v) ? v.map((x) => x.name) : v;

  return {
    info: {
      name: info.name,
      author: info.author,
      formatVersion: info.formatVersion ?? info.version,
      players: info.players ?? info.playerCount,
      playableSize: info.playableSize,
    },
    files: guard(() => listFiles(map).length),
    script,
    terrain: guard(() => readTerrain(map)),
    placedUnits: Array.isArray(placements.units) ? placements.units.length : (placements as { error?: string }).error ?? 'unavailable',
    placedDoodads: Array.isArray(placements.doodads) ? placements.doodads.length : undefined,
    customObjects: objects,
    regions: names(regions as { name: string }[]),
    cameras: names(cameras as { name: string }[]),
    sounds: Array.isArray(sounds) ? sounds.length : sounds,
    imports: guard(() => readImports(map).count),
    triggers: {
      hasGuiTriggers: triggers.hasGuiTriggers,
      triggerCount: triggers.triggerCount,
      categories: triggers.categories,
    },
  };
}
