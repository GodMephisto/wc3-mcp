---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "src/authoring.ts"
ref: HEAD
---

/**
 * High-level "author content" helpers that turn an intent into the fiddly low-level edits:
 *   - createObject: add a custom object (unit/item/ability/…) derived from a base type.
 *   - addTrigger:   splice a JASS/Lua function into the map script and (optionally) wire it
 *                   to run during map initialization — the reliable way to make the map
 *                   *do* something new in-game.
 */
import { readObjects, writeObjects, readScript, writeScript } from './map.js';
import { objectFormatVersion, createObjectLegacy } from './objects-legacy.js';
import type { War3Map, ObjectKind } from './map.js';

export interface ObjectField {
  id: string; // 4-char field code, e.g. "unam" (name), "uhpm" (max HP)
  type: 'int' | 'real' | 'unreal' | 'string';
  value: string | number;
  level?: number; // for leveled fields (abilities/upgrades)
  column?: number;
}

/**
 * Add a custom object to one of the object-data tables, derived from `baseId`.
 * `newId` is the new 4-char raw code (e.g. "x000"). Returns the modification entry written.
 */
export function createObject(
  map: War3Map,
  kind: ObjectKind,
  baseId: string,
  newId: string,
  fields: ObjectField[],
): object {
  if (newId.length !== 4) throw new Error(`newId must be exactly 4 characters, got "${newId}"`);
  if (baseId.length !== 4) throw new Error(`baseId must be exactly 4 characters, got "${baseId}"`);

  // Classic version-1/2 tables aren't writable by wc3maptranslator — append via mdx instead,
  // which preserves every existing object byte-for-byte.
  const version = objectFormatVersion(map, kind);
  if (version !== null && version !== 3) {
    return createObjectLegacy(map, kind, baseId, newId, fields);
  }

  const table = readObjects(map, kind) as {
    original?: Record<string, unknown>;
    custom?: Record<string, unknown>;
  };
  table.custom ??= {};
  table.original ??= {};

  const key = `${newId}:${baseId}`;
  const mods = fields.map((f) => ({
    id: f.id,
    type: f.type,
    value: f.value,
    ...(f.level !== undefined ? { level: f.level } : {}),
    ...(f.column !== undefined ? { column: f.column } : {}),
  }));
  table.custom[key] = mods;

  writeObjects(map, kind, table);
  return { kind, created: key, fields: mods };
}

export interface AddTriggerOptions {
  /** One or more complete JASS/Lua function definitions to add to the script. */
  code: string;
  /** If set, a `call <entryFunction>()` (JASS) / `<entryFunction>()` (Lua) is inserted so it
   *  runs at map init. Must name a zero-arg function defined in `code`. Omit to only append. */
  entryFunction?: string;
}

/**
 * Splice custom script into war3map.j / war3map.lua.
 *
 * JASS is single-pass, so new functions are inserted *before* `function main`, and the
 * init call is inserted at the end of `main`'s body (after the stock initialization, where
 * all locals are already declared — a safe insertion point). Lua functions are appended and
 * the init call is added inside `main`.
 *
 * NOTE: if the map has a GUI trigger tree (war3map.wtg) and is later re-saved from the World
 * Editor, the editor regenerates the script and these edits are lost. For GUI maps, treat
 * this as an applied-now change; for pure custom-script maps it is fully durable.
 */
export function addTrigger(map: War3Map, opts: AddTriggerOptions): object {
  const { name, content } = readScript(map);
  const isLua = name.endsWith('.lua');
  // Match the file's dominant newline. Optimized/published maps frequently use bare \r.
  const eol = content.includes('\r\n') ? '\r\n' : content.includes('\r') ? '\r' : '\n';
  const out = isLua
    ? spliceLua(content, eol, opts)
    : spliceJass(content, eol, opts);
  if (out === content) throw new Error('Script unchanged — could not find an insertion point (no `main` function?).');
  const written = writeScript(map, out);
  return {
    script: written,
    language: isLua ? 'lua' : 'jass',
    wiredToInit: !!opts.entryFunction,
    bytesBefore: content.length,
    bytesAfter: out.length,
  };
}

function spliceJass(content: string, eol: string, opts: AddTriggerOptions): string {
  const mainDecl = 'function main takes nothing returns nothing';
  const mainIdx = content.indexOf(mainDecl);
  if (mainIdx === -1) return content;

  // Insert the new function(s) immediately before `function main`.
  let out = content.slice(0, mainIdx) + opts.code + eol + eol + content.slice(mainIdx);

  if (opts.entryFunction) {
    // JASS functions don't nest, so the first `endfunction` after the main declaration closes
    // main. Match it regardless of the preceding newline style (\r, \n, or \r\n) — optimized
    // maps often use bare \r.
    const newMainIdx = out.indexOf(mainDecl);
    const m = /[\r\n][ \t]*endfunction/.exec(out.slice(newMainIdx));
    if (!m) return content; // structure unexpected — abort cleanly
    const endIdx = newMainIdx + m.index; // at the newline that precedes main's endfunction
    const call = `${eol}    call ${opts.entryFunction}()`;
    out = out.slice(0, endIdx) + call + out.slice(endIdx);
  }
  return out;
}

function spliceLua(content: string, eol: string, opts: AddTriggerOptions): string {
  // Lua resolves names at call time and the last global assignment wins, so append the new
  // function(s) at the end of the file...
  let out = content.replace(/\s*$/, '') + eol + eol + opts.code + eol;

  if (opts.entryFunction) {
    // ...then wrap `main` at load time so the entry runs at init after the stock main. This
    // needs no brittle parsing of main's nested `end`s.
    out +=
      eol +
      [
        '-- wc3-mcp: run the added trigger at map init by wrapping main',
        'do',
        '    local __wc3mcp_oldmain = main',
        '    function main()',
        '        __wc3mcp_oldmain()',
        `        ${opts.entryFunction}()`,
        '    end',
        'end',
      ].join(eol) +
      eol;
  }
  return out;
}
