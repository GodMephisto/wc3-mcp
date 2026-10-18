---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "src/strings.ts"
ref: HEAD
---

/**
 * war3map.wts — the trigger-string table. Many fields across the map (map name, unit names,
 * descriptions, quest text, …) are stored as `TRIGSTR_###` references that index into this
 * table. Reading it resolves those to real text; writing it changes that text.
 *
 * We parse/serialize the (simple, text) wts format ourselves rather than via wc3maptranslator,
 * which re-converts the entire buffer to a string on every regex iteration — that's O(entries ×
 * filesize) and OOM-crashes on the large string tables common in published custom maps.
 */
import type { War3Map } from './map.js';

/** STRING <id> [// comment] { <value...> } — `[\s\S]*?` matches across newlines, lazily. */
const STRING_BLOCK = /STRING\s+(\d+)\s*\r?\n(\/\/[^\r\n]*\r?\n)?\{\r?\n([\s\S]*?)\r?\n\}/g;

export interface WtsEntry {
  value: string;
  comment?: string;
}

/** Parse a war3map.wts buffer into { id: { value, comment? } }. Converts to text exactly once. */
function parseWts(buf: Uint8Array): Record<string, WtsEntry> {
  let text = Buffer.from(buf).toString('utf8');
  if (text.charCodeAt(0) === 0xfeff) text = text.slice(1); // strip UTF-8 BOM
  const out: Record<string, WtsEntry> = {};
  STRING_BLOCK.lastIndex = 0;
  let m: RegExpExecArray | null;
  while ((m = STRING_BLOCK.exec(text)) !== null) {
    const [, id, comment, value] = m;
    out[id] = comment ? { value, comment: comment.trim() } : { value };
  }
  return out;
}

/** Serialize { id: { value, comment? } } back to wts text (UTF-8 bytes). */
function serializeWts(entries: Record<string, WtsEntry>): Buffer {
  const ids = Object.keys(entries).sort((a, b) => Number(a) - Number(b));
  const parts: string[] = [];
  for (const id of ids) {
    const e = entries[id];
    const comment = e.comment ? `${e.comment}\r\n` : '';
    parts.push(`STRING ${id}\r\n${comment}{\r\n${e.value}\r\n}\r\n`);
  }
  return Buffer.from(parts.join('\r\n'), 'utf8');
}

/** Read war3map.wts into a flat { "1": "text", ... } map. Empty if the file is absent. */
export function readStrings(map: War3Map): Record<string, string> {
  const file = map.get('war3map.wts');
  if (!file) return {};
  const parsed = parseWts(file.bytes());
  const out: Record<string, string> = {};
  for (const [id, e] of Object.entries(parsed)) out[id] = e.value;
  return out;
}

/** Resolve a possibly-`TRIGSTR_###` value against a strings map. Returns the input unchanged
 *  when it isn't a TRIGSTR reference (or the index is missing). */
export function resolveTrigStr(strings: Record<string, string>, value: string): string {
  const m = /^TRIGSTR_(\d+)$/.exec(value.trim());
  if (!m) return value;
  const idx = String(parseInt(m[1], 10)); // "001" -> "1"
  return strings[idx] ?? value;
}

/** Set/replace one string-table entry by numeric id, then write war3map.wts back into the
 *  archive (caller saves the map). Returns the id written. */
export function setString(map: War3Map, id: number | string, value: string): string {
  const file = map.get('war3map.wts');
  const entries = file ? parseWts(file.bytes()) : {};
  const key = String(typeof id === 'string' ? parseInt(id, 10) : id);
  entries[key] = { value };
  map.set('war3map.wts', toArrayBuffer(serializeWts(entries)));
  return key;
}

/** Allocate the next free string id, store `value`, and return the `TRIGSTR_###` token that
 *  now points at it — handy when authoring new named content. */
export function addString(map: War3Map, value: string): { id: number; trigStr: string } {
  const file = map.get('war3map.wts');
  const entries = file ? parseWts(file.bytes()) : {};
  const maxId = Object.keys(entries).reduce((m, k) => Math.max(m, parseInt(k, 10) || 0), 0);
  const id = maxId + 1;
  entries[String(id)] = { value };
  map.set('war3map.wts', toArrayBuffer(serializeWts(entries)));
  return { id, trigStr: `TRIGSTR_${String(id).padStart(3, '0')}` };
}

/** Uint8Array -> exact ArrayBuffer slice (kept local so this module doesn't value-import
 *  map.ts, which would create a require cycle with readInfo's string resolution). */
function toArrayBuffer(buf: Uint8Array): ArrayBuffer {
  return buf.buffer.slice(buf.byteOffset, buf.byteOffset + buf.byteLength) as ArrayBuffer;
}
