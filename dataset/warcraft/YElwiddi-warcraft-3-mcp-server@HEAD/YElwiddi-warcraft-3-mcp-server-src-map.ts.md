---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "src/map.ts"
ref: HEAD
---

/**
 * Core engine: open a Warcraft III .w3x/.w3m map, read/edit its inner files, and repack it.
 *
 * Two libraries do the heavy lifting:
 *   - mdx-m3-viewer  -> the MPQ/W3X container (load, list, get/set inner files, save)
 *   - wc3maptranslator -> convert the inner `war3map.*` binaries <-> JSON
 *
 * Both are CommonJS; this file is ESM (NodeNext). We pull runtime values through
 * `createRequire` (robust against CJS/ESM default-interop quirks) and types through
 * `import type` (erased at compile time).
 */
import { readFile, writeFile } from 'node:fs/promises';
import { createRequire } from 'node:module';
import { readStrings, resolveTrigStr } from './strings.js';
import { readObjectsLegacy } from './objects-legacy.js';

const require = createRequire(import.meta.url);

// mdx-m3-viewer: MPQ/W3X container (CJS; default export is the War3Map class).
// Types come from the .d.ts via `import(...)` type queries; runtime values come from
// require() — this sidesteps CJS/ESM default-interop quirks (a plain default type-import
// resolves to the module namespace, not the class).
type War3MapInstance = InstanceType<
  typeof import('mdx-m3-viewer/dist/cjs/parsers/w3x/map.js').default
>;
const War3MapCtor = require('mdx-m3-viewer/dist/cjs/parsers/w3x/map.js').default as {
  new (): War3MapInstance;
};

// mdx placement-file parsers: preplaced units/items and doodads/destructables.
type War3MapUnitsDooInstance = InstanceType<
  typeof import('mdx-m3-viewer/dist/cjs/parsers/w3x/unitsdoo/file.js').default
>;
const War3MapUnitsDooCtor = require('mdx-m3-viewer/dist/cjs/parsers/w3x/unitsdoo/file.js')
  .default as { new (): War3MapUnitsDooInstance };

type War3MapDooInstance = InstanceType<
  typeof import('mdx-m3-viewer/dist/cjs/parsers/w3x/doo/file.js').default
>;
const War3MapDooCtor = require('mdx-m3-viewer/dist/cjs/parsers/w3x/doo/file.js').default as {
  new (): War3MapDooInstance;
};

// The per-unit struct inside war3mapUnits.doo — needed to *author* placements,
// not just read them.
type UnitInstance = InstanceType<
  typeof import('mdx-m3-viewer/dist/cjs/parsers/w3x/unitsdoo/unit.js').default
>;
const UnitCtor = require('mdx-m3-viewer/dist/cjs/parsers/w3x/unitsdoo/unit.js').default as {
  new (): UnitInstance;
};

// wc3maptranslator: object-data war3map.* <-> JSON (CJS named exports).
// (Map info is read via mdx-m3-viewer instead — it tracks Reforged w3i format versions
// that wc3maptranslator 5.0.0 rejects.)
const { ObjectsTranslator } = require('wc3maptranslator') as {
  ObjectsTranslator: typeof import('wc3maptranslator').ObjectsTranslator;
};

export type War3Map = War3MapInstance;

/** Object-data categories that share the war3map "modification table" format. */
export type ObjectKind =
  | 'units'
  | 'items'
  | 'abilities'
  | 'buffs'
  | 'destructables'
  | 'doodads'
  | 'upgrades';

/** war3map data file backing each object-data category. */
const OBJECT_FILE: Record<ObjectKind, string> = {
  units: 'war3map.w3u',
  items: 'war3map.w3t',
  abilities: 'war3map.w3a',
  buffs: 'war3map.w3h',
  destructables: 'war3map.w3b',
  doodads: 'war3map.w3d',
  upgrades: 'war3map.w3q',
};

export const OBJECT_KINDS = Object.keys(OBJECT_FILE) as ObjectKind[];

/** Node Buffer/Uint8Array -> exact ArrayBuffer slice (what mdx `set()` expects). */
export function toArrayBuffer(buf: Uint8Array): ArrayBuffer {
  return buf.buffer.slice(buf.byteOffset, buf.byteOffset + buf.byteLength) as ArrayBuffer;
}

/** Recursively convert typed arrays (Int32Array, Float32Array, ...) to plain number
 *  arrays so parsed structures serialize to clean JSON. */
export function normalizeTypedArrays(value: unknown): object {
  return JSON.parse(
    JSON.stringify(value, (_key, v) =>
      ArrayBuffer.isView(v) && !(v instanceof DataView)
        ? Array.from(v as unknown as ArrayLike<number>)
        : v,
    ),
  );
}

/** mdx's War3Map plus the original-header stash we attach in openMap. */
type MapWithHeader = War3MapInstance & { __originalHeader?: Uint8Array | null };

/** Read a map file from disk and parse its archive. */
export async function openMap(mapPath: string, readonly = false): Promise<War3MapInstance> {
  // IMPORTANT: convert to a *plain* Uint8Array, not a Node Buffer. mdx-m3-viewer relies on
  // Uint8Array.prototype.slice() copying when it reads the MPQ hash/block tables, but
  // Buffer.prototype.slice() returns a view — which corrupts those reads (RangeError /
  // wrong table sizes). `new Uint8Array(buf)` copies into a fresh zero-offset buffer.
  const bytes = new Uint8Array(await readFile(mapPath));
  const map = new War3MapCtor();
  map.load(bytes, readonly);
  // Stash the original 512-byte HM3W header (if present) so saveMap can repack
  // resiliently when mdx's w3i-dependent save() path throws (see saveMap).
  const hasHeader =
    bytes.length >= 4 && String.fromCharCode(bytes[0], bytes[1], bytes[2], bytes[3]) === 'HM3W';
  (map as MapWithHeader).__originalHeader = hasHeader ? bytes.slice(0, 512) : null;
  return map;
}

/**
 * Repack the (mutated) archive and write it to disk.
 *
 * mdx's `War3Map.save()` unconditionally parses war3map.w3i (only to decide whether to
 * prepend the HM3W header). On formats newer than mdx knows — e.g. Reforged 2.0's w3i
 * version 33 — that parse throws and takes the whole save down. We try mdx's save first
 * (best fidelity: it also refreshes the imports list), and on failure fall back to
 * repacking at the archive level and re-attaching the *original* header verbatim.
 */
export async function saveMap(map: War3MapInstance, outPath: string): Promise<void> {
  let out: Uint8Array | null = null;
  try {
    out = map.save();
  } catch {
    out = null; // w3i parser blew up — fall back to a w3i-free repack below
  }

  if (!out) {
    const archive = (map as unknown as { archive: { save(): Uint8Array | null } }).archive;
    const archiveBuffer = archive.save();
    if (!archiveBuffer) {
      throw new Error('Could not repack archive — opened readonly, or the archive is unrepackable.');
    }
    const header = (map as MapWithHeader).__originalHeader;
    if (header) {
      const combined = new Uint8Array(header.length + archiveBuffer.byteLength);
      combined.set(header, 0);
      combined.set(archiveBuffer, header.length);
      out = combined;
    } else {
      out = archiveBuffer;
    }
  }

  await writeFile(outPath, Buffer.from(out));
}

export function listFiles(map: War3MapInstance): string[] {
  return map.getFileNames();
}

/** Parse war3map.w3i: name, author, players, playable size, script mode, etc.
 *  Uses mdx-m3-viewer's full parser; if that throws on a w3i version newer than mdx
 *  knows (e.g. Reforged 2.0 = version 33), falls back to a tolerant header read that
 *  recovers the fields living before the version-divergent region. */
export function readInfo(map: War3MapInstance): object {
  let info: Record<string, unknown>;
  try {
    info = normalizeTypedArrays(map.getMapInformation()) as Record<string, unknown>;
  } catch (e) {
    const file = map.get('war3map.w3i');
    if (!file) throw e;
    const msg = e instanceof Error ? e.message : String(e);
    info = {
      partial: true,
      note: `Full w3i parse failed (${msg}) — likely a format version newer than mdx-m3-viewer supports. Returning header fields only (players/forces/tech tables omitted).`,
      ...parseW3iHeader(Buffer.from(file.bytes())),
    };
  }
  // Resolve TRIGSTR_### references (name/author/description/recommendedPlayers) against
  // the string table so the agent sees the real text, not "TRIGSTR_001".
  return resolveInfoStrings(map, info);
}

/** Replace top-level `TRIGSTR_###` string fields in a parsed w3i with their war3map.wts
 *  values. Keeps the original under `<field>Raw` when a substitution happens. */
function resolveInfoStrings(map: War3MapInstance, info: Record<string, unknown>): object {
  let strings: Record<string, string> | null;
  try {
    strings = readStrings(map);
  } catch {
    return info; // no/unreadable wts — return as-is
  }
  for (const key of ['name', 'author', 'description', 'recommendedPlayers']) {
    const v = info[key];
    if (typeof v === 'string') {
      const resolved = resolveTrigStr(strings, v);
      if (resolved !== v) {
        info[`${key}Raw`] = v;
        info[key] = resolved;
      }
    }
  }
  return info;
}

/** Read the deterministic prefix of war3map.w3i — everything up to the tileset byte,
 *  which precedes the loading-screen/fog fields whose layout changes between versions. */
function parseW3iHeader(b: Buffer): object {
  let o = 0;
  const i32 = () => { const v = b.readInt32LE(o); o += 4; return v; };
  const u32 = () => { const v = b.readUInt32LE(o); o += 4; return v; };
  const f32 = () => { const v = b.readFloatLE(o); o += 4; return v; };
  const str = () => { const s = o; while (o < b.length && b[o] !== 0) o++; const v = b.toString('utf8', s, o); o++; return v; };

  const formatVersion = i32();
  const saves = i32();
  const editorVersion = i32();
  const buildVersion = formatVersion > 27 ? [u32(), u32(), u32(), u32()] : [0, 0, 0, 0];
  const name = str();
  const author = str();
  const description = str();
  const recommendedPlayers = str();
  const cameraBounds = Array.from({ length: 8 }, f32);
  const cameraBoundsComplements = Array.from({ length: 4 }, i32);
  const playableSize = [i32(), i32()];
  const flags = u32();
  const tileset = String.fromCharCode(b[o]);

  return {
    formatVersion,
    saves,
    editorVersion,
    buildVersion,
    gameBuildVersion: buildVersion[0] * 100 + buildVersion[1],
    name,
    author,
    description,
    recommendedPlayers,
    cameraBounds,
    cameraBoundsComplements,
    playableSize,
    flags,
    tileset,
  };
}

/** Read what's placed on the map in the World Editor: preplaced units/items
 *  (war3mapUnits.doo) and doodads/destructables (war3map.doo). */
export function readPlacements(map: War3MapInstance): {
  units: object[];
  doodads: object[];
} {
  const units: object[] = [];
  const doodads: object[] = [];

  const unitsFile = map.get('war3mapUnits.doo');
  if (unitsFile) {
    const bytes = Buffer.from(unitsFile.bytes());
    const doo = new War3MapUnitsDooCtor();
    doo.load(bytes, detectUnitsBuildVersion(bytes));
    for (const u of doo.units) {
      units.push({
        type: u.id,
        player: u.player,
        x: u.location[0],
        y: u.location[1],
        z: u.location[2],
        angle: u.angle,
        hitpoints: u.hitpoints,
        mana: u.mana,
      });
    }
  }

  const dooFile = map.get('war3map.doo');
  if (dooFile) {
    const bytes = Buffer.from(dooFile.bytes());
    const doo = new War3MapDooCtor();
    doo.load(bytes, detectDooBuildVersion(bytes, War3MapDooCtor));
    for (const d of doo.doodads) {
      doodads.push({
        type: d.id,
        x: d.location[0],
        y: d.location[1],
        z: d.location[2],
        angle: d.angle,
        variation: d.variation,
      });
    }
  }

  return { units, doodads };
}

// ---------------------------------------------------------------------------
// Unit/item placement editing (war3mapUnits.doo)
// ---------------------------------------------------------------------------

/** Detect the build version a .doo file (units or doodads) was written with, by finding the
 *  value whose load→save round-trips byte-for-byte. Build version only changes the per-entry
 *  skin field (present when > 131, i.e. Reforged). Defaults to 200 (Reforged 2.0) when the
 *  file is empty or ambiguous. This avoids depending on getMapInformation(), whose w3i parse
 *  throws on format versions newer than mdx-m3-viewer knows. */
function detectDooBuildVersion(
  bytes: Buffer,
  Ctor: { new (): { load(b: Uint8Array, bv: number): void; save(bv: number): Uint8Array } },
): number {
  for (const bv of [200, 1]) {
    try {
      const probe = new Ctor();
      probe.load(bytes, bv);
      if (Buffer.from(probe.save(bv)).equals(bytes)) return bv;
    } catch {
      /* wrong format for this candidate — try the next */
    }
  }
  return 200;
}

function detectUnitsBuildVersion(bytes: Buffer): number {
  return detectDooBuildVersion(bytes, War3MapUnitsDooCtor);
}

export interface UnitSelector {
  creationNumber?: number;
  index?: number;
  type?: string;
}

export interface UnitPlacement {
  type: string;
  x: number;
  y: number;
  z?: number;
  facing?: number; // degrees (World Editor convention); stored as radians
  player?: number;
  scale?: number;
  variation?: number;
  hitpointsPercent?: number; // -1 = default
  manaPercent?: number; // -1 = default
  skin?: string; // Reforged only; defaults to `type`
}

export interface UnitChanges {
  x?: number;
  y?: number;
  z?: number;
  facing?: number; // degrees
  player?: number;
  scale?: number;
  type?: string;
  hitpointsPercent?: number;
  manaPercent?: number;
}

const DEG2RAD = Math.PI / 180;
const RAD2DEG = 180 / Math.PI;

function loadUnitsDoo(map: War3MapInstance): { doo: War3MapUnitsDooInstance; buildVersion: number } {
  const file = map.get('war3mapUnits.doo');
  if (!file) throw new Error('war3mapUnits.doo not found in archive');
  const bytes = Buffer.from(file.bytes());
  const buildVersion = detectUnitsBuildVersion(bytes);
  const doo = new War3MapUnitsDooCtor();
  doo.load(bytes, buildVersion);
  return { doo, buildVersion };
}

function unitSummary(u: UnitInstance, index: number) {
  return {
    index,
    creationNumber: u.creationNumber,
    type: u.id,
    player: u.player,
    x: u.location[0],
    y: u.location[1],
    z: u.location[2],
    facing: Math.round(u.angle * RAD2DEG * 1000) / 1000,
  };
}

function matchesSelector(u: UnitInstance, index: number, sel: UnitSelector): boolean {
  if (sel.creationNumber === undefined && sel.index === undefined && sel.type === undefined) {
    throw new Error('Selector must specify at least one of: creationNumber, index, type');
  }
  if (sel.creationNumber !== undefined && u.creationNumber !== sel.creationNumber) return false;
  if (sel.index !== undefined && index !== sel.index) return false;
  if (sel.type !== undefined && u.id !== sel.type) return false;
  return true;
}

/** List the preplaced units/items in war3mapUnits.doo. */
export function listUnits(map: War3MapInstance): object[] {
  const { doo } = loadUnitsDoo(map);
  return doo.units.map((u, i) => unitSummary(u, i));
}

/** Append a unit/item placement to war3mapUnits.doo (writes into the archive; caller saves). */
export function placeUnit(map: War3MapInstance, p: UnitPlacement): object {
  const { doo, buildVersion } = loadUnitsDoo(map);
  const u = new UnitCtor();
  u.id = p.type;
  u.variation = p.variation ?? 0;
  u.location = new Float32Array([p.x, p.y, p.z ?? 0]);
  u.angle = (p.facing ?? 270) * DEG2RAD;
  const s = p.scale ?? 1;
  u.scale = new Float32Array([s, s, s]);
  if (buildVersion > 131) u.skin = p.skin ?? p.type;
  u.player = p.player ?? 0;
  u.hitpoints = p.hitpointsPercent ?? -1;
  u.mana = p.manaPercent ?? -1;
  u.creationNumber = doo.units.reduce((m, x) => Math.max(m, x.creationNumber), -1) + 1;
  doo.units.push(u);
  writeRawFile(map, 'war3mapUnits.doo', doo.save(buildVersion));
  return unitSummary(u, doo.units.length - 1);
}

/** Modify matching units in war3mapUnits.doo. Returns the updated unit summaries. */
export function modifyUnits(map: War3MapInstance, sel: UnitSelector, changes: UnitChanges): object[] {
  const { doo, buildVersion } = loadUnitsDoo(map);
  const updated: object[] = [];
  doo.units.forEach((u, i) => {
    if (!matchesSelector(u, i, sel)) return;
    if (changes.x !== undefined) u.location[0] = changes.x;
    if (changes.y !== undefined) u.location[1] = changes.y;
    if (changes.z !== undefined) u.location[2] = changes.z;
    if (changes.facing !== undefined) u.angle = changes.facing * DEG2RAD;
    if (changes.player !== undefined) u.player = changes.player;
    if (changes.scale !== undefined) u.scale = new Float32Array([changes.scale, changes.scale, changes.scale]);
    if (changes.type !== undefined) {
      u.id = changes.type;
      if (buildVersion > 131) u.skin = changes.type;
    }
    if (changes.hitpointsPercent !== undefined) u.hitpoints = changes.hitpointsPercent;
    if (changes.manaPercent !== undefined) u.mana = changes.manaPercent;
    updated.push(unitSummary(u, i));
  });
  if (updated.length > 0) writeRawFile(map, 'war3mapUnits.doo', doo.save(buildVersion));
  return updated;
}

/** Delete matching units from war3mapUnits.doo. Returns the removed unit summaries. */
export function deleteUnits(map: War3MapInstance, sel: UnitSelector): object[] {
  const { doo, buildVersion } = loadUnitsDoo(map);
  const removed: object[] = [];
  const kept: UnitInstance[] = [];
  doo.units.forEach((u, i) => {
    if (matchesSelector(u, i, sel)) removed.push(unitSummary(u, i));
    else kept.push(u);
  });
  if (removed.length > 0) {
    doo.units = kept;
    writeRawFile(map, 'war3mapUnits.doo', doo.save(buildVersion));
  }
  return removed;
}

/** Parse an object-data table to JSON. Returns an empty table if the file is absent.
 *  Modern (version-3) tables go through wc3maptranslator; classic version-1/2 tables (older
 *  custom maps) are read via mdx-m3-viewer into the same { original, custom } shape. */
export function readObjects(map: War3MapInstance, kind: ObjectKind): object {
  const file = map.get(OBJECT_FILE[kind]);
  if (!file) return { original: {}, custom: {} };
  const version = Buffer.from(file.bytes()).readInt32LE(0);
  if (version !== 3) return readObjectsLegacy(map, kind);
  return ObjectsTranslator.warToJson(kind, Buffer.from(file.bytes())).json;
}

/** Serialize an object-data JSON table back into the archive (does not save to disk). */
export function writeObjects(map: War3MapInstance, kind: ObjectKind, json: object): void {
  const { buffer } = ObjectsTranslator.jsonToWar(kind, json as never);
  map.set(OBJECT_FILE[kind], toArrayBuffer(buffer));
}

/** Read the map script (war3map.j JASS or war3map.lua). */
export function readScript(map: War3MapInstance): { name: string; content: string } {
  const file = map.getScriptFile();
  if (!file) throw new Error('No script file (war3map.j / war3map.lua) found in archive');
  return { name: file.name, content: file.text() };
}

/** Replace the map script. Keeps the existing script filename, defaulting to war3map.j. */
export function writeScript(map: War3MapInstance, content: string): string {
  const name = map.getScriptFile()?.name ?? 'war3map.j';
  map.set(name, content);
  return name;
}

/** Read any internal file's raw bytes. */
export function readRawFile(map: War3MapInstance, internalName: string): Buffer {
  const file = map.get(internalName);
  if (!file) throw new Error(`File not found in archive: ${internalName}`);
  return Buffer.from(file.bytes());
}

/** Write/replace any internal file's raw bytes (does not save to disk). */
export function writeRawFile(map: War3MapInstance, internalName: string, data: Uint8Array): void {
  map.set(internalName, toArrayBuffer(data));
}
