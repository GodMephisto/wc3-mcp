---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "src/objects-legacy.ts"
ref: HEAD
---

/**
 * Legacy (format version 1/2) object-data support.
 *
 * wc3maptranslator only parses the modern version-3 object tables (current WC3 / Reforged).
 * Classic custom maps (RoC/TFT) store object data as version 1/2, which mdx-m3-viewer parses
 * and re-saves byte-for-byte. This module bridges that: it reads v1/v2 tables into the *same*
 * `{ original, custom }` JSON shape the v3 path uses, and appends new custom objects in place
 * (preserving every existing object exactly — only new modifications are added).
 *
 * Field code split (which kinds carry per-level/variation ints):
 *   - w3u(units)/w3t(items)/w3b(destructables)/w3h(buffs): no optional ints  -> W3u parser
 *   - w3a(abilities)/w3d(doodads)/w3q(upgrades):           level+column ints -> W3d parser
 */
import { createRequire } from 'node:module';
import type { War3Map, ObjectKind } from './map.js';
import type { ObjectField } from './authoring.js';

const require = createRequire(import.meta.url);

/** Local copy of the engine's Uint8Array -> exact ArrayBuffer slice (kept here so this module
 *  has no value-import of map.ts, which would create a require cycle). */
function toArrayBuffer(buf: Uint8Array): ArrayBuffer {
  return buf.buffer.slice(buf.byteOffset, buf.byteOffset + buf.byteLength) as ArrayBuffer;
}

interface Modification {
  id: string;
  variableType: number;
  levelOrVariation: number;
  dataPointer: number;
  value: number | string;
  u1: number;
}
interface ModifiedObject {
  oldId: string;
  newId: string;
  modifications: Modification[];
}
interface ModTable {
  objects: ModifiedObject[];
}
interface W3oFile {
  load(b: Uint8Array): void;
  save(): Uint8Array;
  version: number;
  originalTable: ModTable;
  customTable: ModTable;
}

const W3uCtor = require('mdx-m3-viewer/dist/cjs/parsers/w3x/w3u/file.js').default as { new (): W3oFile };
const W3dCtor = require('mdx-m3-viewer/dist/cjs/parsers/w3x/w3d/file.js').default as { new (): W3oFile };
const ModifiedObjectCtor = require('mdx-m3-viewer/dist/cjs/parsers/w3x/w3u/modifiedobject.js').default as {
  new (): ModifiedObject;
};
const ModificationCtor = require('mdx-m3-viewer/dist/cjs/parsers/w3x/w3u/modification.js').default as {
  new (): Modification;
};

/** war3map data file + which mdx parser (carries the useOptionalInts flag) backs each kind. */
const KIND_INFO: Record<ObjectKind, { file: string; Ctor: { new (): W3oFile }; optionalInts: boolean }> = {
  units: { file: 'war3map.w3u', Ctor: W3uCtor, optionalInts: false },
  items: { file: 'war3map.w3t', Ctor: W3uCtor, optionalInts: false },
  destructables: { file: 'war3map.w3b', Ctor: W3uCtor, optionalInts: false },
  buffs: { file: 'war3map.w3h', Ctor: W3uCtor, optionalInts: false },
  abilities: { file: 'war3map.w3a', Ctor: W3dCtor, optionalInts: true },
  doodads: { file: 'war3map.w3d', Ctor: W3dCtor, optionalInts: true },
  upgrades: { file: 'war3map.w3q', Ctor: W3dCtor, optionalInts: true },
};

const TYPE_BY_VARIABLE = ['int', 'real', 'unreal', 'string'] as const;
const VARIABLE_BY_TYPE: Record<string, number> = { int: 0, real: 1, unreal: 2, string: 3 };

/** Read the object-table format version (int32 at offset 0), or null if the file is absent. */
export function objectFormatVersion(map: War3Map, kind: ObjectKind): number | null {
  const file = map.get(KIND_INFO[kind].file);
  if (!file) return null;
  return Buffer.from(file.bytes()).readInt32LE(0);
}

/** Cheap object-table probe: presence, format version, and byte size — WITHOUT parsing the
 *  (possibly huge) modification tables. Used by the overview so it stays memory-light on big
 *  maps. An empty table is 8 bytes (version + count); >8 means it carries data. */
export function objectTableInfo(map: War3Map, kind: ObjectKind): { version: number; bytes: number } | null {
  const file = map.get(KIND_INFO[kind].file);
  if (!file) return null;
  const bytes = file.bytes();
  if (bytes.length < 8) return null;
  return { version: Buffer.from(bytes).readInt32LE(0), bytes: bytes.length };
}

function modToJson(m: Modification, optionalInts: boolean) {
  return {
    id: m.id,
    type: TYPE_BY_VARIABLE[m.variableType] ?? 'int',
    value: m.value,
    ...(optionalInts ? { level: m.levelOrVariation, column: m.dataPointer } : {}),
  };
}

/** Read a v1/v2 object table into the unified `{ original, custom }` shape. */
export function readObjectsLegacy(map: War3Map, kind: ObjectKind): object {
  const { file, Ctor, optionalInts } = KIND_INFO[kind];
  const f = map.get(file);
  if (!f) return { original: {}, custom: {} };
  const w3o = new Ctor();
  w3o.load(f.bytes());

  const original: Record<string, unknown> = {};
  const custom: Record<string, unknown> = {};
  for (const o of w3o.originalTable.objects) {
    original[o.oldId] = o.modifications.map((m) => modToJson(m, optionalInts));
  }
  for (const o of w3o.customTable.objects) {
    custom[`${o.newId}:${o.oldId}`] = o.modifications.map((m) => modToJson(m, optionalInts));
  }
  return { original, custom, _formatVersion: w3o.version };
}

/** Append a custom object to a v1/v2 table via mdx and write it back (existing objects are
 *  preserved byte-for-byte; only the new object is added). Caller saves the map. */
export function createObjectLegacy(
  map: War3Map,
  kind: ObjectKind,
  baseId: string,
  newId: string,
  fields: ObjectField[],
): object {
  const { file, Ctor, optionalInts } = KIND_INFO[kind];
  const f = map.get(file);
  if (!f) throw new Error(`${file} not found — cannot add a legacy ${kind} object to a map without that table.`);
  const w3o = new Ctor();
  w3o.load(f.bytes());

  const obj = new ModifiedObjectCtor();
  obj.oldId = baseId;
  obj.newId = newId;
  obj.modifications = fields.map((field) => {
    const m = new ModificationCtor();
    m.id = field.id;
    m.variableType = VARIABLE_BY_TYPE[field.type] ?? 0;
    m.levelOrVariation = optionalInts ? field.level ?? 0 : 0;
    m.dataPointer = optionalInts ? field.column ?? 0 : 0;
    m.value = field.value;
    m.u1 = 0; // end-of-modification sentinel for a freshly authored mod
    return m;
  });
  w3o.customTable.objects.push(obj);

  map.set(file, toArrayBuffer(w3o.save()));
  return { kind, created: `${newId}:${baseId}`, formatVersion: w3o.version, fields: fields.length };
}
