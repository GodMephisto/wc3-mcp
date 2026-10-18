---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "src/wct.ts"
ref: HEAD
---

/**
 * war3map.wct — the custom-text (raw JASS/Lua) bodies for the map's triggers, plus the
 * map-header "Custom Script Code" block. This is the DURABLE home for script: the World
 * Editor recompiles war3map.j from the trigger tree on every save, so anything written here
 * (and registered in war3map.wtg) survives, while edits to war3map.j alone do not.
 *
 * Modern (Reforged, 0x80000004) layout — reverse-engineered byte-for-byte from World Editor
 * output and verified to round-trip exactly:
 *
 *   int32  version        // 0x80000004
 *   int32  one            // always 1 (format marker)
 *   cstr   comment        // null-terminated placeholder/comment text
 *   int32  headerLen      // bytes of the map-header custom script (incl trailing null); 0 = empty
 *   byte[headerLen] header
 *   -- then, in trigger-tree order, one entry per custom-text trigger, until EOF:
 *   int32  textLen        // bytes incl trailing null; 0 = empty
 *   byte[textLen] text
 *
 * IMPORTANT: the per-trigger entries map POSITIONALLY to the custom-text triggers listed in
 * war3map.wtg. This module never changes their count — it only rewrites the text of an
 * existing entry — so the wtg can be left untouched (and thus uncorrupted).
 */

const MODERN_VERSION = 0x80000004 | 0; // -2147483644 as int32

export interface Wct {
  version: number;
  one: number;
  comment: string;
  header: string;
  triggers: string[];
}

class Reader {
  private o = 0;
  constructor(private b: Buffer) {}
  i32(): number { const v = this.b.readInt32LE(this.o); this.o += 4; return v; }
  remaining(): number { return this.b.length - this.o; }
  cstr(): string {
    const s = this.o;
    while (this.o < this.b.length && this.b[this.o] !== 0) this.o++;
    const v = this.b.toString('utf8', s, this.o);
    this.o++; // skip null
    return v;
  }
  /** Length-prefixed text (length includes the trailing null). Empty when len === 0. */
  lenText(): string {
    const len = this.i32();
    if (len <= 0) return '';
    const v = this.b.toString('utf8', this.o, this.o + len - 1); // drop trailing null
    this.o += len;
    return v;
  }
}

export function readWct(bytes: Uint8Array): Wct {
  const r = new Reader(Buffer.from(bytes));
  const version = r.i32();
  const one = r.i32();
  const comment = r.cstr();
  const header = r.lenText();
  const triggers: string[] = [];
  while (r.remaining() >= 4) triggers.push(r.lenText());
  return { version, one, comment, header, triggers };
}

function lenTextBytes(text: string): Buffer {
  if (!text) return Buffer.from([0, 0, 0, 0]);
  const body = Buffer.from(text, 'utf8');
  const len = Buffer.alloc(4);
  len.writeInt32LE(body.length + 1, 0); // +1 for the trailing null
  return Buffer.concat([len, body, Buffer.from([0])]);
}

export function writeWct(wct: Wct): Buffer {
  const head = Buffer.alloc(8);
  head.writeInt32LE(wct.version, 0);
  head.writeInt32LE(wct.one, 4);
  const comment = Buffer.concat([Buffer.from(wct.comment, 'utf8'), Buffer.from([0])]);
  const parts = [head, comment, lenTextBytes(wct.header)];
  for (const t of wct.triggers) parts.push(lenTextBytes(t));
  return Buffer.concat(parts);
}

/** Build a fresh, empty modern wct (used if a map somehow lacks one). */
export function emptyWct(): Wct {
  return { version: MODERN_VERSION, one: 1, comment: '', header: '', triggers: [] };
}
