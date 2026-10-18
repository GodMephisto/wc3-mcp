---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "src/triggers.ts"
ref: HEAD
---

/**
 * war3map.wtg — read the GUI trigger tree in full detail: categories, variables, and each
 * trigger's events/conditions/actions with decoded parameters. Uses the native parser in
 * wtg.ts (modern AND classic layouts); the parameter shapes emitted here are exactly the
 * shapes wc3_add_gui_trigger / wc3_modify_gui_trigger accept as input, so what you read is
 * what you can write back.
 *
 * Custom-text bodies come from war3map.wct via wct.ts. Every non-comment trigger owns one
 * positional wct slot (its `wctIndex`) — the same index wc3_set_durable_script targets.
 */
import { readRawFile } from './map.js';
import { readStrings, resolveTrigStr } from './strings.js';
import { readWct } from './wct.js';
import {
  parseWtg,
  triggersOf,
  wctIndexOf,
  displayString,
  ITEM,
  type Wtg,
  type WtgFunction,
  type WtgParam,
} from './wtg.js';
import type { War3Map } from './map.js';

/** Cap per-trigger custom-text bodies in the read output (full bodies via wc3_read_file). */
const CUSTOM_TEXT_CAP = 4000;

function renderParam(p: WtgParam): Record<string, unknown> {
  let out: Record<string, unknown>;
  if (p.type === 0) out = { preset: displayString(p.value) };
  else if (p.type === 1) out = { variable: displayString(p.value) };
  else if (p.type === 2) {
    const name = p.sub ? displayString(p.sub.name) : displayString(p.value);
    const call: Record<string, unknown> = { function: name };
    if (p.sub && p.sub.params.length) call.params = p.sub.params.map(renderParam);
    out = { call };
  } else out = { value: displayString(p.value) };
  if (p.isArray && p.index) out.index = renderParam(p.index);
  return out;
}

function renderFunction(f: WtgFunction): Record<string, unknown> {
  const out: Record<string, unknown> = { function: displayString(f.name) };
  if (!f.enabled) out.enabled = false;
  if (f.params.length) out.params = f.params.map(renderParam);
  if (f.children.length) {
    out.children = f.children.map((c) => ({ group: c.group ?? 0, ...renderFunction(c) }));
  }
  return out;
}

export function readTriggers(map: War3Map): object {
  let bytes: Buffer;
  try {
    bytes = readRawFile(map, 'war3map.wtg');
  } catch {
    return {
      hasGuiTriggers: false,
      note: 'No war3map.wtg — this map has no GUI trigger tree. Its logic (if any) lives entirely in the map script (wc3_read_script).',
    };
  }

  const strings = (() => { try { return readStrings(map); } catch { return {}; } })();
  const resolve = (raw: string) => resolveTrigStr(strings, displayString(raw));

  let wtg: Wtg;
  try {
    wtg = parseWtg(bytes);
  } catch (e) {
    return {
      hasGuiTriggers: true,
      parseError: e instanceof Error ? e.message : String(e),
      note: 'Could not decode the GUI trigger tree (custom trigger data or an unknown format variant). The compiled logic is still readable via wc3_read_script.',
    };
  }

  let wctEntries: string[] | null = null;
  try {
    wctEntries = readWct(readRawFile(map, 'war3map.wct')).triggers;
  } catch {
    /* no or unreadable wct — custom-text bodies just won't be attached */
  }

  const cats = wtg.items.filter((i) => i.kind === 'category');
  const categoryName = (parentId: number): string | undefined => {
    const c = cats.find((x) => x.id === parentId);
    return c ? resolve(c.name) : undefined;
  };

  const triggers = triggersOf(wtg).map((t) => {
    const kind = t.isComment ? 'comment' : t.isCustomText ? 'custom-text' : 'gui';
    const o: Record<string, unknown> = { name: resolve(t.name), kind, enabled: !!t.enabled };
    const description = resolve(t.description);
    if (description) o.description = description;
    const category = categoryName(t.parentId);
    if (category) o.category = category;
    if (t.initiallyOff) o.initiallyOff = true;
    if (t.runOnInit) o.runOnInit = true;

    const events = t.functions.filter((f) => f.type === 0).map(renderFunction);
    const conditions = t.functions.filter((f) => f.type === 1).map(renderFunction);
    const actions = t.functions.filter((f) => f.type === 2).map(renderFunction);
    if (events.length) o.events = events;
    if (conditions.length) o.conditions = conditions;
    if (actions.length) o.actions = actions;

    if (!t.isComment && wtg.modern) {
      const wctIndex = wctIndexOf(wtg, t);
      o.wctIndex = wctIndex;
      if (t.isCustomText && wctEntries && wctIndex >= 0 && wctIndex < wctEntries.length) {
        const body = wctEntries[wctIndex];
        o.customText = body.length > CUSTOM_TEXT_CAP ? body.slice(0, CUSTOM_TEXT_CAP) : body;
        if (body.length > CUSTOM_TEXT_CAP) o.customTextTruncated = true;
      }
    }
    return o;
  });

  return {
    hasGuiTriggers: true,
    format: wtg.modern ? 'modern' : 'classic (read-only)',
    formatVersion: wtg.formatVersion,
    triggerCount: triggers.length,
    categories: cats
      .filter((c) => c.itemType === ITEM.category)
      .map((c) => ({ id: c.id, name: resolve(c.name) })),
    variables: wtg.variables.map((v) => {
      const out: Record<string, unknown> = { name: displayString(v.name), type: displayString(v.type) };
      if (v.isArray) { out.isArray = true; out.arraySize = v.arraySize; }
      if (v.isInitialized) out.initialValue = displayString(v.initialValue);
      return out;
    }),
    triggers,
    note:
      'Edit GUI triggers durably with wc3_modify_gui_trigger / wc3_add_gui_trigger (param shapes ' +
      'above are exactly their input format). wctIndex = the trigger\'s war3map.wct slot ' +
      '(custom-text bodies; the triggerIndex used by wc3_set_durable_script).',
  };
}
