---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "src/durable.ts"
ref: HEAD
---

/**
 * Durable script authoring — the reliable way to add custom JASS/Lua that BOTH runs now AND
 * survives a re-save in the World Editor (and shows up there, editable by the user).
 *
 * Background: the World Editor recompiles war3map.j from the trigger tree (war3map.wtg) +
 * custom-text bodies (war3map.wct) on every save. So edits to war3map.j *alone* (what
 * wc3_add_trigger does) are wiped the first time the user re-saves in the editor. The durable
 * home for script is war3map.wct:
 *   - the map-header "Custom Script Code" block (slot: 'header'), or
 *   - a custom-text trigger's body (slot: 'trigger').
 *
 * This module writes the code into war3map.wct (durable + editor-visible) and ALSO mirrors it
 * into war3map.j (so it runs immediately, before the next editor save), while leaving
 * war3map.wtg byte-for-byte untouched — so the editor never rejects the map. The mirror is
 * idempotent: it is wrapped in sentinel comments and replaced in place on repeated calls.
 */
import { readScript, writeScript, readRawFile, writeRawFile } from './map.js';
import { readWct, writeWct } from './wct.js';
import type { War3Map } from './map.js';

const MODERN_WCT = 0x80000004 | 0;

export interface SetDurableOptions {
  /** Complete JASS/Lua function definition(s) (and optionally globals) to install. For
   *  slot:'trigger' this should define `function InitTrig_<name>` so the editor auto-runs it. */
  code: string;
  /** Where in war3map.wct to store it. 'trigger' = a custom-text trigger body (auto-runs at
   *  init, durably); 'header' = the map-header "Custom Script Code" (zero setup, JASS auto-run
   *  only via the immediate mirror). Default 'trigger'. */
  slot?: 'trigger' | 'header';
  /** Which custom-text trigger slot (0-based) to write when slot:'trigger'. Default 0 — the
   *  first custom-text trigger you created in the editor. */
  triggerIndex?: number;
  /** Name of a zero-arg function in `code` to also `call` at map init in the war3map.j mirror,
   *  so it runs immediately. For slot:'trigger', auto-detected as `InitTrig_<name>` if omitted. */
  entryFunction?: string;
  /** Also mirror into war3map.j for immediate effect (default true). Set false to write only
   *  the durable wct copy (runs after the user opens+saves the map in the editor once). */
  mirror?: boolean;
}

export function setDurableScript(map: War3Map, opts: SetDurableOptions): object {
  const slot = opts.slot ?? 'trigger';

  // --- 1) Durable copy in war3map.wct -------------------------------------------------------
  let wctBytes: Buffer;
  try {
    wctBytes = readRawFile(map, 'war3map.wct');
  } catch {
    throw new Error(
      'This map has no war3map.wct (no triggers yet). Open it in the World Editor, add one ' +
        'trigger or a line of Custom Script Code, Save once, then retry.',
    );
  }
  const wct = readWct(wctBytes);
  if ((wct.version | 0) !== MODERN_WCT) {
    throw new Error(
      `Unsupported war3map.wct format 0x${(wct.version >>> 0).toString(16)} — durable writes ` +
        'currently support only the modern (0x80000004) format.',
    );
  }

  let durableTarget: string;
  if (slot === 'header') {
    wct.header = opts.code;
    durableTarget = 'war3map.wct header ("Custom Script Code")';
  } else {
    const idx = opts.triggerIndex ?? 0;
    if (wct.triggers.length === 0) {
      throw new Error(
        'No custom-text trigger slot exists yet. In the World Editor → Trigger Editor: make a ' +
          'New Trigger, then Edit → Convert to Custom Text (or add a custom-script trigger), ' +
          'name it (e.g. "MCPDurable"), Save once, then retry. (Or pass slot:"header", which ' +
          'needs no setup.)',
      );
    }
    if (idx < 0 || idx >= wct.triggers.length) {
      throw new Error(
        `triggerIndex ${idx} is out of range — this map has ${wct.triggers.length} custom-text ` +
          `slot(s) (valid: 0..${wct.triggers.length - 1}).`,
      );
    }
    wct.triggers[idx] = opts.code;
    durableTarget = `war3map.wct custom-text trigger #${idx}`;
  }
  writeRawFile(map, 'war3map.wct', writeWct(wct));

  // --- 2) Immediate mirror into war3map.j ---------------------------------------------------
  let entryFunction = opts.entryFunction;
  if (slot === 'trigger' && !entryFunction) {
    const m = /function\s+(InitTrig_\w+)\s+takes/.exec(opts.code); // JASS InitTrig auto-detect
    if (m) entryFunction = m[1];
  }

  let mirror: object | null = null;
  if (opts.mirror !== false) {
    const label = slot === 'header' ? 'header' : `trigger${opts.triggerIndex ?? 0}`;
    mirror = mirrorIntoScript(map, opts.code, entryFunction, label);
  }

  return {
    durable: durableTarget,
    wtgUntouched: true,
    entryFunction: entryFunction ?? null,
    mirror,
    note:
      slot === 'header'
        ? 'Stored in "Custom Script Code" — durable and editable in the editor. In JASS maps ' +
          'it auto-runs only via the immediate mirror; for durable auto-run across editor saves ' +
          'use slot:"trigger".'
        : 'Stored as a custom-text trigger body — durable, editable, and auto-run at init by the ' +
          'editor. The mirror also makes it run right now (before any editor re-save).',
  };
}

/**
 * Splice `code` into war3map.j / war3map.lua so it runs immediately. Wrapped in sentinel
 * comments and replaced in place on repeat calls (idempotent). New functions go *before*
 * `function main` (JASS is single-pass); the optional init `call` goes at the end of main.
 */
function mirrorIntoScript(
  map: War3Map,
  code: string,
  entryFunction: string | undefined,
  label: string,
): object {
  const { name, content } = readScript(map);
  const isLua = name.endsWith('.lua');
  const eol = content.includes('\r\n') ? '\r\n' : content.includes('\r') ? '\r' : '\n';
  const cmt = isLua ? '--' : '//';
  const start = `${cmt} wc3-mcp:durable:${label}:start`;
  const end = `${cmt} wc3-mcp:durable:${label}:end`;
  const block = `${start}${eol}${code}${eol}${end}`;

  let out: string;
  const s = content.indexOf(start);
  if (s !== -1) {
    const e = content.indexOf(end, s);
    if (e === -1) {
      throw new Error('Found a durable start marker without its end marker in the script — ' +
        'refusing to edit (it may have been hand-modified). Remove the stray marker and retry.');
    }
    out = content.slice(0, s) + block + content.slice(e + end.length);
  } else {
    out = isLua ? insertLua(content, eol, block) : insertJass(content, eol, block);
  }

  if (entryFunction) {
    out = isLua ? wireLuaInit(out, eol, entryFunction) : wireJassInit(out, eol, entryFunction);
  }

  const written = writeScript(map, out);
  return {
    script: written,
    language: isLua ? 'lua' : 'jass',
    wiredToInit: !!entryFunction,
    bytesBefore: content.length,
    bytesAfter: out.length,
  };
}

function insertJass(content: string, eol: string, block: string): string {
  const mainDecl = 'function main takes nothing returns nothing';
  const i = content.indexOf(mainDecl);
  if (i === -1) {
    throw new Error('Could not find `function main` in war3map.j to mirror before. The durable ' +
      'war3map.wct copy still applies — open and Save the map in the editor once to compile it.');
  }
  return content.slice(0, i) + block + eol + eol + content.slice(i);
}

function wireJassInit(content: string, eol: string, fn: string): string {
  const callStr = `call ${fn}()`;
  if (content.includes(callStr)) return content; // already wired (idempotent)
  const mainDecl = 'function main takes nothing returns nothing';
  const mi = content.indexOf(mainDecl);
  if (mi === -1) return content;
  // First `endfunction` after main's declaration closes main; match any newline style.
  const m = /[\r\n][ \t]*endfunction/.exec(content.slice(mi));
  if (!m) return content;
  const endIdx = mi + m.index;
  return content.slice(0, endIdx) + `${eol}    ${callStr}` + content.slice(endIdx);
}

function insertLua(content: string, eol: string, block: string): string {
  return content.replace(/\s*$/, '') + eol + eol + block + eol;
}

function wireLuaInit(content: string, eol: string, fn: string): string {
  const marker = `-- wc3-mcp:durable-init:${fn}`;
  if (content.includes(marker)) return content; // already wired (idempotent)
  return (
    content +
    eol +
    [
      marker,
      'do',
      '    local __wc3mcp_oldmain = main',
      '    function main()',
      '        __wc3mcp_oldmain()',
      `        ${fn}()`,
      '    end',
      'end',
    ].join(eol) +
    eol
  );
}
