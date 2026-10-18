---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "src/gui-triggers.ts"
ref: HEAD
---

/**
 * GUI trigger editing — durable modify/create of real GUI triggers in war3map.wtg.
 *
 * Durability model: war3map.wtg IS the World Editor's source of truth for triggers. Edits
 * land there, so they appear in the Trigger Editor and survive every editor re-save. The
 * editor compiles the tree into the map script (war3map.j) on its next save — this module
 * deliberately does not touch war3map.j, so a GUI change becomes live in-game after the
 * user opens the map and saves once.
 *
 * Safety: every write goes through mutateWtg(), which first proves the serializer can
 * reproduce the map's existing wtg byte-for-byte and refuses otherwise. Creating a trigger
 * also inserts its positional slot in war3map.wct (one entry per non-comment trigger), and
 * refuses if the existing wct/wtg slot counts disagree rather than guessing positions.
 *
 * Input specs use the same JSON parameter shapes wc3_read_triggers emits:
 *   { preset: "PlayerUnitEventAttacked" } | { variable: "MyUnit", index? } |
 *   { value: "hfoo" } | { call: { function: "GetTriggerUnit", params?: [...] } }
 * Function names and presets are validated against the bundled TriggerData table
 * (argument counts are enforced; type mismatches are surfaced as warnings).
 */
import { readRawFile, writeRawFile } from './map.js';
import { readWct, writeWct } from './wct.js';
import {
  mutateWtg,
  triggersOf,
  wctIndexOf,
  nextId,
  displayString,
  rawString,
  argTypes,
  presetExists,
  presetType,
  ITEM,
  type WtgFunction,
  type WtgParam,
  type WtgTrigger,
} from './wtg.js';
import {
  TRIGGER_ACTIONS,
  TRIGGER_CALLS,
  TRIGGER_CONDITIONS,
  TRIGGER_EVENTS,
} from './triggerdata.gen.js';
import type { War3Map } from './map.js';

export interface GuiCallSpec {
  function: string;
  params?: GuiParamSpec[];
}

export interface GuiParamSpec {
  preset?: string;
  variable?: string;
  value?: string;
  call?: GuiCallSpec;
  index?: GuiParamSpec;
}

export interface GuiFunctionSpec {
  function: string;
  params?: GuiParamSpec[];
}

const KIND_LABEL = ['event', 'condition', 'action', 'call'] as const;
const KIND_TABLE = [TRIGGER_EVENTS, TRIGGER_CONDITIONS, TRIGGER_ACTIONS] as const;

/** Which section a name that can appear as a nested call belongs to (calls first — that is
 *  what parameters almost always reference). */
function callableKind(name: string): number | null {
  if (TRIGGER_CALLS[name]) return 3;
  if (TRIGGER_CONDITIONS[name]) return 1;
  if (TRIGGER_ACTIONS[name]) return 2;
  if (TRIGGER_EVENTS[name]) return 0;
  return null;
}

function buildParam(
  spec: GuiParamSpec,
  expectedType: string | undefined,
  where: string,
  warnings: string[],
): WtgParam {
  const given = (['preset', 'variable', 'value', 'call'] as const).filter(
    (k) => spec[k] !== undefined,
  );
  if (given.length !== 1) {
    throw new Error(
      `${where}: a parameter needs exactly one of preset | variable | value | call` +
        ` (got ${given.join(' + ') || 'nothing'})`,
    );
  }
  if (spec.index !== undefined && given[0] !== 'variable') {
    throw new Error(`${where}: "index" is only valid on variable parameters`);
  }

  if (spec.preset !== undefined) {
    if (!presetExists(spec.preset)) {
      throw new Error(
        `${where}: unknown preset "${spec.preset}" — must be a TriggerData preset name ` +
          '(e.g. "PlayerUnitEventAttacked", "OperatorEqualENE")',
      );
    }
    const pt = presetType(spec.preset);
    if (expectedType && pt && pt !== expectedType) {
      warnings.push(`${where}: preset "${spec.preset}" is a ${pt}, slot expects ${expectedType}`);
    }
    return { type: 0, value: rawString(spec.preset), hasSub: 0, isArray: 0 };
  }

  if (spec.variable !== undefined) {
    const p: WtgParam = { type: 1, value: rawString(spec.variable), hasSub: 0, isArray: spec.index ? 1 : 0 };
    if (spec.index) p.index = buildParam(spec.index, 'integer', `${where} index`, warnings);
    return p;
  }

  if (spec.value !== undefined) {
    return { type: 3, value: rawString(String(spec.value)), hasSub: 0, isArray: 0 };
  }

  // call
  const call = spec.call as GuiCallSpec;
  const kind = callableKind(call.function);
  if (kind === null) {
    throw new Error(
      `${where}: unknown function "${call.function}" (not in TriggerData events/conditions/actions/calls)`,
    );
  }
  const types = argTypes(kind, call.function);
  const got = call.params ?? [];
  if (got.length !== types.length) {
    throw new Error(
      `${where}: ${call.function} takes ${types.length} parameter(s)` +
        ` (${types.join(', ') || 'none'}) but got ${got.length}`,
    );
  }
  if (expectedType && kind === 3) {
    const ret = TRIGGER_CALLS[call.function].returns;
    if (ret !== expectedType) {
      warnings.push(`${where}: ${call.function} returns ${ret}, slot expects ${expectedType}`);
    }
  }
  return {
    type: 2,
    value: rawString(call.function),
    hasSub: 1,
    isArray: 0,
    sub: {
      type: kind,
      name: rawString(call.function),
      paramsFlag: got.length ? 1 : 0,
      params: got.map((s, i) =>
        buildParam(s, types[i], `${where} → ${call.function} arg ${i + 1}`, warnings),
      ),
    },
  };
}

function buildFunction(kind: 0 | 1 | 2, spec: GuiFunctionSpec, warnings: string[]): WtgFunction {
  const table = KIND_TABLE[kind];
  if (!table[spec.function]) {
    const actual = callableKind(spec.function);
    const hint = actual !== null && actual !== kind ? ` (it is a ${KIND_LABEL[actual]})` : '';
    throw new Error(
      `"${spec.function}" is not a GUI ${KIND_LABEL[kind]}${hint} — check the TriggerData name`,
    );
  }
  const types = table[spec.function];
  const got = spec.params ?? [];
  if (got.length !== types.length) {
    throw new Error(
      `${KIND_LABEL[kind]} ${spec.function} takes ${types.length} parameter(s)` +
        ` (${types.join(', ') || 'none'}) but got ${got.length}`,
    );
  }
  return {
    type: kind,
    name: rawString(spec.function),
    enabled: 1,
    params: got.map((s, i) =>
      buildParam(s, types[i], `${KIND_LABEL[kind]} ${spec.function} arg ${i + 1}`, warnings),
    ),
    children: [],
  };
}

function readWtgBytes(map: War3Map): Buffer {
  try {
    return readRawFile(map, 'war3map.wtg');
  } catch {
    throw new Error(
      'This map has no war3map.wtg (no trigger tree). Open it once in the World Editor and Save to create one.',
    );
  }
}

const DURABLE_NOTE = {
  durable:
    "Stored in war3map.wtg — the Trigger Editor's own data. The change is visible in the editor and survives its saves.",
  note:
    'The editor compiles GUI triggers into the map script on its next save: open the map, Save once, then playtest.',
};

// ---------------------------------------------------------------------------------------------
// Create
// ---------------------------------------------------------------------------------------------

export interface AddGuiTriggerSpec {
  name: string;
  description?: string;
  category?: string;
  enabled?: boolean;
  initiallyOff?: boolean;
  runOnInit?: boolean;
  events?: GuiFunctionSpec[];
  conditions?: GuiFunctionSpec[];
  actions?: GuiFunctionSpec[];
}

export function addGuiTrigger(map: War3Map, spec: AddGuiTriggerSpec): object {
  const warnings: string[] = [];
  let info: Record<string, unknown> = {};
  let wctIndex = -1;

  const newWtg = mutateWtg(readWtgBytes(map), (wtg) => {
    if (wtg.formatVersion !== 7) {
      throw new Error(`Creating triggers requires trigger format v7 (this map uses v${wtg.formatVersion}).`);
    }
    if (triggersOf(wtg).some((t) => displayString(t.name) === spec.name)) {
      throw new Error(`A trigger named "${spec.name}" already exists — use wc3_modify_gui_trigger or pick another name.`);
    }
    const cats = wtg.items.filter((i) => i.kind === 'category');
    let parent;
    if (spec.category !== undefined) {
      parent = cats.find((c) => c.itemType === ITEM.category && displayString(c.name) === spec.category);
      if (!parent) {
        const names = cats.filter((c) => c.itemType === ITEM.category).map((c) => displayString(c.name));
        throw new Error(`Category "${spec.category}" not found. Available: ${names.join(', ') || '(none)'}`);
      }
    } else {
      parent = cats.find((c) => c.itemType === ITEM.category) ?? cats.find((c) => c.itemType === ITEM.root);
    }
    if (!parent) throw new Error('No category or root found in the trigger tree to attach the trigger to.');

    const functions = [
      ...(spec.events ?? []).map((s) => buildFunction(0, s, warnings)),
      ...(spec.conditions ?? []).map((s) => buildFunction(1, s, warnings)),
      ...(spec.actions ?? []).map((s) => buildFunction(2, s, warnings)),
    ];
    if (!(spec.events ?? []).length) {
      warnings.push('Trigger has no events — it will never fire on its own (fine if run from other triggers).');
    }

    const item: WtgTrigger = {
      kind: 'trigger',
      itemType: ITEM.gui,
      name: rawString(spec.name),
      description: rawString(spec.description ?? ''),
      isComment: 0,
      id: nextId(wtg),
      enabled: spec.enabled === false ? 0 : 1,
      isCustomText: 0,
      initiallyOff: spec.initiallyOff ? 1 : 0,
      runOnInit: spec.runOnInit ? 1 : 0,
      parentId: parent.id,
      functions,
    };
    wtg.items.push(item);
    const guiCount = wtg.counts.find((c) => c.itemType === ITEM.gui);
    if (guiCount) guiCount.count += 1;

    wctIndex = wctIndexOf(wtg, item);
    info = {
      created: spec.name,
      id: item.id,
      category: displayString(parent.name),
      events: (spec.events ?? []).length,
      conditions: (spec.conditions ?? []).length,
      actions: (spec.actions ?? []).length,
      wctIndex,
    };
  });

  // war3map.wct holds one positional entry per non-comment trigger — register the new slot.
  let wctBytes: Buffer;
  try {
    wctBytes = readRawFile(map, 'war3map.wct');
  } catch {
    throw new Error('This map has no war3map.wct — open it once in the World Editor and Save, then retry.');
  }
  const wct = readWct(wctBytes);
  if ((wct.version | 0) !== (0x80000004 | 0)) {
    throw new Error(
      `Unsupported war3map.wct version 0x${(wct.version >>> 0).toString(16)} — cannot register the trigger's custom-text slot.`,
    );
  }
  if (wct.triggers.length !== wctIndex) {
    throw new Error(
      `war3map.wct has ${wct.triggers.length} trigger slot(s) but the trigger tree expects ${wctIndex} — ` +
        'refusing to guess slot positions. (Open + Save the map in the World Editor once to normalize it, then retry.)',
    );
  }
  wct.triggers.push('');
  writeRawFile(map, 'war3map.wct', writeWct(wct));
  writeRawFile(map, 'war3map.wtg', newWtg);

  return { ...info, ...(warnings.length ? { warnings } : {}), ...DURABLE_NOTE };
}

// ---------------------------------------------------------------------------------------------
// Modify
// ---------------------------------------------------------------------------------------------

export interface ModifyGuiTriggerSpec {
  trigger: string | number;
  rename?: string;
  description?: string;
  enabled?: boolean;
  initiallyOff?: boolean;
  runOnInit?: boolean;
  replaceParamValue?: { from: string; to: string };
  setEvents?: GuiFunctionSpec[];
  setConditions?: GuiFunctionSpec[];
  setActions?: GuiFunctionSpec[];
}

function replaceInParams(params: WtgParam[], from: string, to: string): number {
  let n = 0;
  for (const p of params) {
    if (p.value === from) { p.value = to; n++; }
    if (p.sub) n += replaceInParams(p.sub.params, from, to);
    if (p.index) n += replaceInParams([p.index], from, to);
  }
  return n;
}

function replaceInFunctions(fns: WtgFunction[], from: string, to: string): number {
  let n = 0;
  for (const f of fns) {
    n += replaceInParams(f.params, from, to);
    n += replaceInFunctions(f.children, from, to);
  }
  return n;
}

export function modifyGuiTrigger(map: War3Map, spec: ModifyGuiTriggerSpec): object {
  const warnings: string[] = [];
  const applied: string[] = [];
  let name = '';

  const newWtg = mutateWtg(readWtgBytes(map), (wtg) => {
    const list = triggersOf(wtg);
    let t: WtgTrigger;
    if (typeof spec.trigger === 'number') {
      if (spec.trigger < 0 || spec.trigger >= list.length) {
        throw new Error(`Trigger index ${spec.trigger} is out of range — the map has ${list.length} trigger(s).`);
      }
      t = list[spec.trigger];
    } else {
      const matches = list.filter((x) => displayString(x.name) === spec.trigger);
      if (matches.length === 0) {
        throw new Error(
          `No trigger named "${spec.trigger}". Triggers: ${list.map((x) => displayString(x.name)).join(', ') || '(none)'}`,
        );
      }
      if (matches.length > 1) {
        throw new Error(`${matches.length} triggers are named "${spec.trigger}" — select by index instead (wc3_read_triggers order).`);
      }
      t = matches[0];
    }
    name = displayString(t.name);

    const ecaEdit = spec.setEvents || spec.setConditions || spec.setActions || spec.replaceParamValue;
    if (t.isCustomText && ecaEdit) {
      throw new Error(
        `"${name}" is a custom-text trigger (no GUI functions) — edit its code with wc3_set_durable_script, wct slot ${wctIndexOf(wtg, t)}.`,
      );
    }

    if (spec.rename !== undefined) {
      t.name = rawString(spec.rename);
      applied.push(`renamed "${name}" → "${spec.rename}"`);
      name = spec.rename;
    }
    if (spec.description !== undefined) { t.description = rawString(spec.description); applied.push('description set'); }
    if (spec.enabled !== undefined) { t.enabled = spec.enabled ? 1 : 0; applied.push(spec.enabled ? 'enabled' : 'disabled'); }
    if (spec.initiallyOff !== undefined) { t.initiallyOff = spec.initiallyOff ? 1 : 0; applied.push(`initiallyOff = ${spec.initiallyOff}`); }
    if (spec.runOnInit !== undefined) { t.runOnInit = spec.runOnInit ? 1 : 0; applied.push(`runOnInit = ${spec.runOnInit}`); }

    const sets: Array<[0 | 1 | 2, GuiFunctionSpec[] | undefined]> = [
      [0, spec.setEvents],
      [1, spec.setConditions],
      [2, spec.setActions],
    ];
    for (const [kind, specs] of sets) {
      if (!specs) continue;
      const rebuilt = specs.map((s) => buildFunction(kind, s, warnings));
      t.functions = [...t.functions.filter((f) => f.type !== kind), ...rebuilt];
      applied.push(`${KIND_LABEL[kind]}s replaced (now ${rebuilt.length})`);
    }

    if (spec.replaceParamValue) {
      const { from, to } = spec.replaceParamValue;
      const n = replaceInFunctions(t.functions, rawString(from), rawString(to));
      if (n === 0) throw new Error(`No parameter with value "${from}" found in "${name}".`);
      applied.push(`replaced value "${from}" → "${to}" in ${n} place(s)`);
    }

    if (applied.length === 0) {
      throw new Error(
        'No changes requested — pass one of: rename, description, enabled, initiallyOff, runOnInit, replaceParamValue, setEvents, setConditions, setActions.',
      );
    }
  });

  writeRawFile(map, 'war3map.wtg', newWtg);
  return { trigger: name, applied, ...(warnings.length ? { warnings } : {}), ...DURABLE_NOTE };
}
