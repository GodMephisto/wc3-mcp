---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "src/imports.ts"
ref: HEAD
---

/**
 * war3map.imp — the import table: the list of custom files (models, textures, sounds, …)
 * bundled into the map. Read-only listing; the actual file bytes live in the archive and
 * are reachable with wc3_read_file / wc3_replace_file.
 */
import { createRequire } from 'node:module';
import type { War3Map } from './map.js';

const require = createRequire(import.meta.url);
const { ImportsTranslator } = require('wc3maptranslator') as {
  ImportsTranslator: typeof import('wc3maptranslator').ImportsTranslator;
};

export function readImports(map: War3Map): { count: number; files: string[] } {
  const file = map.get('war3map.imp');
  if (!file) return { count: 0, files: [] };
  const json = ImportsTranslator.warToJson(Buffer.from(file.bytes())).json as unknown;
  const files = Array.isArray(json) ? (json as string[]) : ((json as { files?: string[] }).files ?? []);
  return { count: files.length, files };
}
