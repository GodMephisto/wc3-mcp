---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "src/wtg.ts"
ref: HEAD
---

/**
 * war3map.wtg — full parse/serialize of the GUI trigger tree (the Trigger Editor's data:
 * categories, variables, and every trigger's events/conditions/actions with parameters).
 *
 * Layout ported from War3Net.Build.Core (MIT) and cross-checked against HiveWE; verified
 * byte-for-byte against World Editor 2.0 output. Two layouts exist:
 *   - modern (magic "WTG!", then a 0x8000000x sub-version marker): Reforged / 1.31+ saves.
 *     Fully supported: parse AND serialize.
 *   - classic (plain format version 4 / 7): pre-1.31 saves. Parse only.
 *
 * The .wtg does NOT store per-function parameter counts — the World Editor resolves them
 * from UI\TriggerData.txt. We use the generated table in triggerdata.gen.ts; a function
 * missing there (custom editor data) aborts the parse with a clear error.
 *
 * SAFETY GATE: mutateWtg() refuses to write unless parse→serialize reproduces the original
 * file byte-for-byte first. A map that doesn't round-trip exactly is treated as read-only,
 * so a wtg we didn't fully understand can never be corrupted.
 *
 * Strings are kept as latin1 (raw bytes) internally so serialization is lossless even for
 * legacy non-UTF-8 maps; use displayString()/rawString() to convert at the API boundary.
 */
import {
  TRIGGER_ACTIONS,
  TRIGGER_CALLS,
  TRIGGER_CONDITIONS,
  TRIGGER_EVENTS,
  TRIGGER_PRESETS,
} from './triggerdata.gen.js';

// Item types as stored in the file (War3Net TriggerItemType / HiveWE Classifier).
export const ITEM = {
  root: 1,
  library: 2,
  category: 4,
  gui: 8,
  comment: 16,
  script: 32,
  variable: 64,
  unk7: 128,
} as const;
/** Header count-table order (one {count, deletedIds} entry per type). */
const ITEM_TYPE_ORDER = [1, 2, 4, 8, 16, 32, 64, 128];

export const FUNC_KIND = ['event', 'condition', 'action', 'call'] as const;

/** GUI function parameter. type: 0 preset | 1 variable | 2 function | 3 string literal. */
export interface WtgParam {
  type: number;
  value: string;
  /** has-sub-function flag as stored (nonzero when type is 2/function). */
  hasSub: number;
  /** Nested call (when hasSub). paramsFlag is the stored has-parameters/enabled u32. */
  sub?: { type: number; name: string; paramsFlag: number; params: WtgParam[] };
  /** Array-indexer flag as stored (variables only). */
  isArray: number;
  /** The index expression (when isArray). */
  index?: WtgParam;
}

/** One event/condition/action (or nested child under if/loop blocks). */
export interface WtgFunction {
  type: number; // 0 event | 1 condition | 2 action | 3 call
  /** Branch id within the parent (child functions only), e.g. if/then/else slot. */
  group?: number;
  name: string;
  enabled: number;
  params: WtgParam[];
  children: WtgFunction[];
}

export interface WtgCategory {
  kind: 'category';
  itemType: number; // 1 root | 2 library | 4 category
  id: number;
  name: string;
  isComment: number; // v7 only (absent from classic v4 files; kept 0 there)
  isExpanded: number; // modern only
  parentId: number; // modern only (-1 for the root)
}

export interface WtgTrigger {
  kind: 'trigger';
  itemType: number; // 8 gui | 16 comment | 32 script
  name: string;
  description: string;
  isComment: number;
  id: number; // -1 in classic files (no ids stored)
  enabled: number;
  isCustomText: number;
  initiallyOff: number;
  runOnInit: number;
  parentId: number;
  functions: WtgFunction[];
}

export interface WtgVariableRef {
  kind: 'variableRef';
  itemType: number; // 64 — tree placement of a variable (data lives in `variables`)
  id: number;
  name: string;
  parentId: number;
}

export type WtgItem = WtgCategory | WtgTrigger | WtgVariableRef;

export interface WtgVariable {
  name: string;
  type: string;
  unknown: number;
  isArray: number;
  arraySize: number; // v7 only (kept 0 for v4)
  isInitialized: number;
  initialValue: string;
  id: number; // modern only (-1 in classic)
  parentId: number; // modern only
}

export interface WtgCounts {
  itemType: number;
  count: number;
  deletedIds: number[];
}

export interface Wtg {
  /** true = modern sub-version layout (serializable); false = classic (read-only). */
  modern: boolean;
  subVersion: number; // 0x80000004 / 0x80000001 (modern only; 0 for classic)
  formatVersion: number; // 7 (TFT+) or 4 (RoC)
  counts: WtgCounts[]; // modern only (8 entries in ITEM_TYPE_ORDER order)
  gameVersion: number; // 2 = Reforged-era saves
  variables: WtgVariable[];
  items: WtgItem[];
}

// ---------------------------------------------------------------------------------------------
// TriggerData lookups
// ---------------------------------------------------------------------------------------------

const TABLE_BY_TYPE: Record<number, Record<string, string[]> | null> = {
  0: TRIGGER_EVENTS,
  1: TRIGGER_CONDITIONS,
  2: TRIGGER_ACTIONS,
  3: null, // calls have a different shape; handled in argTypes()
};

/** Argument types (and thus count) for a GUI function, by its stored type. Throws with a
 *  clear message when the function is unknown — parsing cannot continue without the count. */
export function argTypes(fnType: number, name: string): string[] {
  if (fnType === 3) {
    const call = TRIGGER_CALLS[name];
    if (call) return call.args;
  } else {
    const args = TABLE_BY_TYPE[fnType]?.[name];
    if (args) return args;
  }
  // Editor files occasionally store comparisons/calls under a neighboring type — fall back
  // to any section that knows the name before giving up.
  for (const t of [3, 1, 2, 0]) {
    if (t === fnType) continue;
    const alt = t === 3 ? TRIGGER_CALLS[name]?.args : TABLE_BY_TYPE[t]?.[name];
    if (alt) return alt;
  }
  throw new Error(
    `Unknown GUI ${FUNC_KIND[fnType] ?? `type-${fnType}`} function "${name}" — not in the ` +
      'bundled TriggerData table (map may use custom World Editor trigger data).',
  );
}

export function presetExists(name: string): boolean {
  return Object.prototype.hasOwnProperty.call(TRIGGER_PRESETS, name);
}

export function presetType(name: string): string | undefined {
  return TRIGGER_PRESETS[name];
}

/** Raw (latin1) model string -> display text (decodes UTF-8 map strings). */
export function displayString(raw: string): string {
  return Buffer.from(raw, 'latin1').toString('utf8');
}

/** External text -> raw (latin1) model string for storage. */
export function rawString(text: string): string {
  return Buffer.from(text, 'utf8').toString('latin1');
}

// ---------------------------------------------------------------------------------------------
// Binary reader / writer
// ---------------------------------------------------------------------------------------------

class Reader {
  o = 0;
  constructor(private b: Buffer) {}
  private need(n: number): void {
    if (this.o + n > this.b.length) {
      throw new Error(`wtg: premature end of file at offset ${this.o} (need ${n} bytes)`);
    }
  }
  u32(): number {
    this.need(4);
    const v = this.b.readUInt32LE(this.o);
    this.o += 4;
    return v;
  }
  i32(): number {
    this.need(4);
    const v = this.b.readInt32LE(this.o);
    this.o += 4;
    return v;
  }
  cstr(): string {
    const s = this.o;
    while (this.o < this.b.length && this.b[this.o] !== 0) this.o++;
    if (this.o >= this.b.length) throw new Error(`wtg: unterminated string at offset ${s}`);
    const v = this.b.toString('latin1', s, this.o);
    this.o++; // null
    return v;
  }
  remaining(): number {
    return this.b.length - this.o;
  }
}

class Writer {
  private parts: Buffer[] = [];
  u32(v: number): void {
    const b = Buffer.alloc(4);
    b.writeUInt32LE(v >>> 0, 0);
    this.parts.push(b);
  }
  i32(v: number): void {
    const b = Buffer.alloc(4);
    b.writeInt32LE(v | 0, 0);
    this.parts.push(b);
  }
  cstr(s: string): void {
    this.parts.push(Buffer.from(s, 'latin1'), Buffer.from([0]));
  }
  raw(s: string): void {
    this.parts.push(Buffer.from(s, 'latin1'));
  }
  out(): Buffer {
    return Buffer.concat(this.parts);
  }
}

// ---------------------------------------------------------------------------------------------
// Parse
// ---------------------------------------------------------------------------------------------

function parseParam(r: Reader, v7: boolean): WtgParam {
  const p: WtgParam = { type: r.u32(), value: r.cstr(), hasSub: r.u32(), isArray: 0 };
  if (p.hasSub) {
    const type = r.u32();
    const name = r.cstr();
    const paramsFlag = r.u32();
    const params: WtgParam[] = [];
    if (paramsFlag) {
      for (let i = 0; i < argTypes(type, name).length; i++) params.push(parseParam(r, v7));
    }
    p.sub = { type, name, paramsFlag, params };
  }
  if (v7) {
    if (p.hasSub) {
      const tail = r.u32(); // War3Net: nested child count; HiveWE: "unknown, always 0"
      if (tail !== 0) {
        throw new Error(`wtg: nested call with unsupported trailing count ${tail}`);
      }
    }
    p.isArray = r.u32();
  } else {
    // v4: function-type params carry an unknown u32 instead of the array flag
    if (p.type === 2) r.u32();
    else p.isArray = r.u32();
  }
  if (p.isArray) p.index = parseParam(r, v7);
  return p;
}

function parseFunction(r: Reader, v7: boolean, isChild: boolean): WtgFunction {
  const type = r.u32();
  const f: WtgFunction = { type, name: '', enabled: 0, params: [], children: [] };
  if (isChild) f.group = r.u32();
  f.name = r.cstr();
  f.enabled = r.u32();
  for (let i = 0; i < argTypes(type, f.name).length; i++) f.params.push(parseParam(r, v7));
  if (v7) {
    const childCount = r.u32();
    for (let i = 0; i < childCount; i++) f.children.push(parseFunction(r, v7, true));
  }
  return f;
}

function parseTriggerBody(
  r: Reader,
  itemType: number,
  v7: boolean,
  modern: boolean,
): WtgTrigger {
  const t: WtgTrigger = {
    kind: 'trigger',
    itemType,
    name: r.cstr(),
    description: r.cstr(),
    isComment: v7 ? r.u32() : 0,
    id: modern ? r.i32() : -1,
    enabled: r.u32(),
    isCustomText: r.u32(),
    initiallyOff: r.u32(),
    runOnInit: r.u32(),
    parentId: r.i32(),
    functions: [],
  };
  const count = r.u32();
  for (let i = 0; i < count; i++) t.functions.push(parseFunction(r, v7, false));
  return t;
}

export function parseWtg(bytes: Uint8Array): Wtg {
  const b = Buffer.from(bytes.buffer, bytes.byteOffset, bytes.byteLength);
  if (b.length < 8 || b.toString('latin1', 0, 4) !== 'WTG!') {
    throw new Error('wtg: missing WTG! magic');
  }
  const r = new Reader(b);
  r.o = 4;
  const first = r.u32();

  if (first === 0x80000004 || first === 0x80000001) {
    // Modern (1.31+/Reforged) layout
    const formatVersion = r.u32();
    if (formatVersion !== 7 && formatVersion !== 4) {
      throw new Error(`wtg: unknown format version ${formatVersion} (expected 7 or 4)`);
    }
    const v7 = formatVersion === 7;
    const counts: WtgCounts[] = ITEM_TYPE_ORDER.map((itemType) => {
      const count = r.u32();
      const deletedCount = r.u32();
      const deletedIds: number[] = [];
      for (let i = 0; i < deletedCount; i++) deletedIds.push(r.i32());
      return { itemType, count, deletedIds };
    });
    const gameVersion = r.u32();
    const variables: WtgVariable[] = [];
    const varCount = r.u32();
    for (let i = 0; i < varCount; i++) {
      variables.push({
        name: r.cstr(),
        type: r.cstr(),
        unknown: r.u32(),
        isArray: r.u32(),
        arraySize: v7 ? r.u32() : 0,
        isInitialized: r.u32(),
        initialValue: r.cstr(),
        id: r.i32(),
        parentId: r.i32(),
      });
    }
    const items: WtgItem[] = [];
    const itemCount = r.u32();
    for (let i = 0; i < itemCount; i++) {
      const itemType = r.u32();
      switch (itemType) {
        case ITEM.root:
        case ITEM.library:
        case ITEM.category:
          items.push({
            kind: 'category',
            itemType,
            id: r.i32(),
            name: r.cstr(),
            isComment: v7 ? r.u32() : 0,
            isExpanded: r.u32(),
            parentId: r.i32(),
          });
          break;
        case ITEM.gui:
        case ITEM.comment:
        case ITEM.script:
          items.push(parseTriggerBody(r, itemType, v7, true));
          break;
        case ITEM.variable:
          items.push({ kind: 'variableRef', itemType, id: r.i32(), name: r.cstr(), parentId: r.i32() });
          break;
        default:
          throw new Error(`wtg: unknown item type ${itemType} at offset ${r.o - 4}`);
      }
    }
    if (r.remaining() !== 0) {
      throw new Error(`wtg: ${r.remaining()} unparsed trailing bytes`);
    }
    return { modern: true, subVersion: first, formatVersion, counts, gameVersion, variables, items };
  }

  if (first === 3 || first === 4 || first === 6 || first === 7) {
    // Classic (pre-1.31) layout — read-only. v3/v6 are beta-era variants of v4/v7.
    const formatVersion = first;
    const v7 = formatVersion === 7;
    const items: WtgItem[] = [];
    const categoryCount = r.u32();
    for (let i = 0; i < categoryCount; i++) {
      items.push({
        kind: 'category',
        itemType: ITEM.category,
        id: r.i32(),
        name: r.cstr(),
        isComment: v7 ? r.u32() : 0,
        isExpanded: 1,
        parentId: 0,
      });
    }
    const gameVersion = r.u32();
    const variables: WtgVariable[] = [];
    const varCount = r.u32();
    for (let i = 0; i < varCount; i++) {
      variables.push({
        name: r.cstr(),
        type: r.cstr(),
        unknown: r.u32(),
        isArray: r.u32(),
        arraySize: v7 ? r.u32() : 0,
        isInitialized: r.u32(),
        initialValue: r.cstr(),
        id: -1,
        parentId: -1,
      });
    }
    const triggerCount = r.u32();
    for (let i = 0; i < triggerCount; i++) {
      const t = parseTriggerBody(r, ITEM.gui, v7, false);
      if (t.isComment) t.itemType = ITEM.comment;
      else if (t.isCustomText) t.itemType = ITEM.script;
      items.push(t);
    }
    return { modern: false, subVersion: 0, formatVersion, counts: [], gameVersion, variables, items };
  }

  throw new Error(`wtg: unknown version 0x${(first >>> 0).toString(16)}`);
}

// ---------------------------------------------------------------------------------------------
// Serialize (modern only)
// ---------------------------------------------------------------------------------------------

function writeParam(w: Writer, p: WtgParam, v7: boolean): void {
  w.u32(p.type);
  w.cstr(p.value);
  w.u32(p.hasSub);
  if (p.hasSub && p.sub) {
    w.u32(p.sub.type);
    w.cstr(p.sub.name);
    w.u32(p.sub.paramsFlag);
    for (const sp of p.sub.params) writeParam(w, sp, v7);
  }
  if (v7) {
    if (p.hasSub) w.u32(0);
    w.u32(p.isArray);
  } else {
    if (p.type === 2) w.u32(0);
    else w.u32(p.isArray);
  }
  if (p.isArray && p.index) writeParam(w, p.index, v7);
}

function writeFunction(w: Writer, f: WtgFunction, v7: boolean, isChild: boolean): void {
  w.u32(f.type);
  if (isChild) w.u32(f.group ?? 0);
  w.cstr(f.name);
  w.u32(f.enabled);
  for (const p of f.params) writeParam(w, p, v7);
  if (v7) {
    w.u32(f.children.length);
    for (const c of f.children) writeFunction(w, c, v7, true);
  }
}

export function writeWtg(wtg: Wtg): Buffer {
  if (!wtg.modern) {
    throw new Error('wtg: classic (pre-1.31) format is read-only — cannot serialize');
  }
  const v7 = wtg.formatVersion === 7;
  const w = new Writer();
  w.raw('WTG!');
  w.u32(wtg.subVersion);
  w.u32(wtg.formatVersion);
  for (const c of wtg.counts) {
    w.u32(c.count);
    w.u32(c.deletedIds.length);
    for (const id of c.deletedIds) w.i32(id);
  }
  w.u32(wtg.gameVersion);
  w.u32(wtg.variables.length);
  for (const v of wtg.variables) {
    w.cstr(v.name);
    w.cstr(v.type);
    w.u32(v.unknown);
    w.u32(v.isArray);
    if (v7) w.u32(v.arraySize);
    w.u32(v.isInitialized);
    w.cstr(v.initialValue);
    w.i32(v.id);
    w.i32(v.parentId);
  }
  w.u32(wtg.items.length);
  for (const item of wtg.items) {
    w.u32(item.itemType);
    if (item.kind === 'category') {
      w.i32(item.id);
      w.cstr(item.name);
      if (v7) w.u32(item.isComment);
      w.u32(item.isExpanded);
      w.i32(item.parentId);
    } else if (item.kind === 'trigger') {
      w.cstr(item.name);
      w.cstr(item.description);
      if (v7) w.u32(item.isComment);
      w.i32(item.id);
      w.u32(item.enabled);
      w.u32(item.isCustomText);
      w.u32(item.initiallyOff);
      w.u32(item.runOnInit);
      w.i32(item.parentId);
      w.u32(item.functions.length);
      for (const f of item.functions) writeFunction(w, f, v7, false);
    } else {
      w.i32(item.id);
      w.cstr(item.name);
      w.i32(item.parentId);
    }
  }
  return w.out();
}

// ---------------------------------------------------------------------------------------------
// Safety gate + shared helpers
// ---------------------------------------------------------------------------------------------

function firstDiff(a: Buffer, b: Buffer): number {
  const n = Math.min(a.length, b.length);
  for (let i = 0; i < n; i++) if (a[i] !== b[i]) return i;
  return a.length === b.length ? -1 : n;
}

/** Parse, PROVE the serializer reproduces this exact file, then mutate and re-serialize.
 *  This is what makes wtg writes safe: a map we can't reproduce byte-for-byte is rejected
 *  before any change is attempted. */
export function mutateWtg(bytes: Uint8Array, mutate: (wtg: Wtg) => void): Buffer {
  const original = Buffer.from(bytes.buffer, bytes.byteOffset, bytes.byteLength);
  const wtg = parseWtg(original);
  if (!wtg.modern) {
    throw new Error(
      'This map uses the classic (pre-1.31) trigger format, which this server reads but does ' +
        'not write. Open and re-save the map in the current World Editor to upgrade it first.',
    );
  }
  const check = writeWtg(wtg);
  if (!check.equals(original)) {
    const at = firstDiff(original, check);
    throw new Error(
      `Refusing to edit war3map.wtg: the round-trip self-check failed (first difference at ` +
        `byte ${at}, ${original.length} vs ${check.length} bytes). Editing without an exact ` +
        'round-trip could corrupt every trigger, so this map is treated as read-only. ' +
        'Please report this map so the parser can be extended.',
    );
  }
  mutate(wtg);
  return writeWtg(wtg);
}

export function triggersOf(wtg: Wtg): WtgTrigger[] {
  return wtg.items.filter((i): i is WtgTrigger => i.kind === 'trigger');
}

export function categoriesOf(wtg: Wtg): WtgCategory[] {
  return wtg.items.filter((i): i is WtgCategory => i.kind === 'category');
}

/** Index of a trigger among the map's war3map.wct entries, or -1 for comments (comments get
 *  no wct slot). Entry order = non-comment triggers in item order. */
export function wctIndexOf(wtg: Wtg, trigger: WtgTrigger): number {
  let idx = 0;
  for (const t of triggersOf(wtg)) {
    if (t.isComment) continue;
    if (t === trigger) return idx;
    idx++;
  }
  return -1;
}

/** Next free item id (ids are shared across categories, triggers, and variables). */
export function nextId(wtg: Wtg): number {
  let max = 0;
  for (const item of wtg.items) if (item.id > max) max = item.id;
  for (const v of wtg.variables) if (v.id > max) max = v.id;
  for (const c of wtg.counts) for (const id of c.deletedIds) if (id > max) max = id;
  return max + 1;
}
