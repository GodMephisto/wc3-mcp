---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "src/terrain.ts"
ref: HEAD
---

/**
 * war3map.w3e — the terrain/environment file: tilepoint grid with ground & cliff heights,
 * textures, water and flags. Full corner data is large (cols×rows structs), so we summarize:
 * dimensions, tilesets, and height ranges. Parsed with mdx-m3-viewer (wc3maptranslator's
 * terrain parser rejects current/Reforged versions).
 */
import { createRequire } from 'node:module';
import type { War3Map } from './map.js';

const require = createRequire(import.meta.url);
const War3MapW3e = require('mdx-m3-viewer/dist/cjs/parsers/w3x/w3e/file.js').default as {
  new (): {
    load(b: Uint8Array): void;
    version: number;
    tileset: string;
    groundTilesets: string[];
    cliffTilesets: string[];
    mapSize: ArrayLike<number>;
    centerOffset: ArrayLike<number>;
    corners: { groundHeight: number; waterHeight: number; layerHeight: number; water: number }[][];
  };
};

/** Convert a tilepoint's raw ground/layer heights to a world-space Z (WC3 formula). */
function worldHeight(groundHeight: number, layerHeight: number): number {
  return (groundHeight - 0x2000) / 4 + (layerHeight - 2) * 128;
}

export function readTerrain(map: War3Map): object {
  const file = map.get('war3map.w3e');
  if (!file) throw new Error('war3map.w3e not found in archive');
  const w3e = new War3MapW3e();
  w3e.load(file.bytes());

  const [cols, rows] = [w3e.mapSize[0], w3e.mapSize[1]];
  let minH = Infinity, maxH = -Infinity, minWater = Infinity, maxWater = -Infinity, waterTiles = 0;
  for (const row of w3e.corners) {
    for (const c of row) {
      const h = worldHeight(c.groundHeight, c.layerHeight);
      if (h < minH) minH = h;
      if (h > maxH) maxH = h;
      if (c.water) {
        waterTiles++;
        if (c.waterHeight < minWater) minWater = c.waterHeight;
        if (c.waterHeight > maxWater) maxWater = c.waterHeight;
      }
    }
  }

  return {
    version: w3e.version,
    tileset: w3e.tileset,
    groundTilesets: w3e.groundTilesets,
    cliffTilesets: w3e.cliffTilesets,
    tilepoints: { cols, rows },
    // Playable terrain spans (cols-1)*128 world units (tilepoints are 128 apart).
    worldSize: { width: (cols - 1) * 128, height: (rows - 1) * 128 },
    centerOffset: [w3e.centerOffset[0], w3e.centerOffset[1]],
    heightRange: { min: Math.round(minH), max: Math.round(maxH) },
    water: { tilepoints: waterTiles, heightRange: waterTiles ? { min: minWater, max: maxWater } : null },
  };
}
