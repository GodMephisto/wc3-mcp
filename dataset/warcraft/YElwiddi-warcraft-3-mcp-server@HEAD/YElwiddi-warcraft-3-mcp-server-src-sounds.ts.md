---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "src/sounds.ts"
ref: HEAD
---

/**
 * war3map.w3s — sound definitions (JASS `gg_snd_<Name>`). Read-only: surfaces which sounds
 * the map declares, their files and playback settings.
 */
import { createRequire } from 'node:module';
import type { War3Map } from './map.js';

const require = createRequire(import.meta.url);
const { SoundsTranslator } = require('wc3maptranslator') as {
  SoundsTranslator: typeof import('wc3maptranslator').SoundsTranslator;
};

export function readSounds(map: War3Map): object[] {
  const file = map.get('war3map.w3s');
  if (!file) return [];
  return SoundsTranslator.warToJson(Buffer.from(file.bytes())).json as unknown as object[];
}
