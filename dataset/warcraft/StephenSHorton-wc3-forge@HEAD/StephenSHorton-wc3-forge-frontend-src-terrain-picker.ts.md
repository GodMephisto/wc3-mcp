---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/terrain-picker.ts"
ref: HEAD
---

// terrain-picker.ts — cell-level terrain picking.
//
// When terrain-pick mode is active, a canvas click should return the (col,
// row) of the terrain cell under the cursor and a bundle of per-cell facts
// (palette FourCC, layer-height, cliff data, water, shadow). The data is
// already in the TerrainDTO that scene-instances.ts fetched at loadMap time,
// so we read straight from that — no Go round-trip per click.
//
// Algorithm:
//   1. screenToWorldRay → ray origin + dir in world space (via camera).
//   2. Intersect with the Z=0 ground plane: t = -origin.z / dir.z.
//      (We use Z=0 rather than per-cell Z because per-cell Z lookups require
//       solving for which cell the ray hits FIRST — chicken/egg with what
//       we're picking. The error vs per-cell Z is small at normal pitch
//       angles, and the click target visually anchored to ground is fine
//       for "tell me about this cell" purposes.)
//   3. col = floor((worldX - centerOffset[0]) / 128)
//      row = floor((worldY - centerOffset[1]) / 128)
//   4. Clamp to [0, width-1] × [0, height-1]. Return null if outside the map.
//
// Indexing convention matches w3e: vertex grid (Width × Height), each cell
// is bounded by 4 vertices. We return the BL-corner vertex index for "cell"
// lookups. The cell-cliff/cell-skip arrays in TerrainDTO are sized
// (Width-1) × (Height-1) and indexed by the BL corner's i,j; "primary palette"
// here means GroundTex[BL].

export interface TerrainCellInfo {
  col: number                 // BL-corner i (0..width-1)
  row: number                 // BL-corner j (0..height-1)
  worldX: number              // ray-vs-ground intersection X (game coords)
  worldY: number              // ray-vs-ground intersection Y (game coords)
  paletteIdx: number          // GroundTex[BL] — primary palette index
  paletteFourCC: string       // Palette[paletteIdx]
  paletteTexture: string      // PaletteTextures[paletteIdx] (no extension)
  layerHeight: number         // LayerHeight[BL] — integer cliff step
  cliffTexIdx: number         // CliffTex[BL] — index into CliffPalette
  cliffFourCC: string         // CliffPalette[cliffTexIdx] (or '' if out of range)
  cliffVar: number            // CliffVar[BL]
  groundVar: number           // GroundVar[BL]
  rampFlags: number           // RampFlags[BL] — bit 0 ramp, bit 1 boundary
  waterZ: number              // WaterZ[BL]
  hasWater: boolean           // HasWater[BL] != 0
  // 4 corner ground-texture palette indices (BL, BR, TL, TR). Useful for
  // tracing "grass appears where there should be none" bugs — adjacent corners
  // can differ when cliff-palette remap happened on some but not all corners.
  cornerPalettes: [number, number, number, number]
  // Cell-skip flag (true = HiveWE-style "skip terrain quad here, the cliff
  // mesh covers it"). Only valid when col<W-1 and row<H-1.
  cellSkip: boolean
  // Per-cell shadow byte (0 = lit, ≥0x80 = shadowed). Shadow map is sized
  // (W-1)*4 × (H-1)*4 in HiveWE convention — we map the cell corner to its
  // nearest shadow sample. -1 when no shadow map is loaded.
  shadow: number
  // Cell index for callers that want to look up further attributes.
  cellIndex: number
}

// Minimal TerrainDTO subset we need — declared locally so consumers don't
// have to import the full thing.
export interface TerrainDataForPicking {
  width: number
  height: number
  center_offset: [number, number]
  ground_tex: number[] | Uint32Array
  palette: string[]
  palette_textures?: string[]
  layer_height: number[] | Uint32Array
  cliff_tex: number[] | Uint32Array
  cliff_var: number[] | Uint32Array
  ground_var: number[] | Uint32Array
  ramp_flags: number[] | Uint32Array
  cliff_palette: string[]
  water_z: number[] | Float32Array
  has_water: number[] | Uint32Array
  shadow_map?: Uint8Array | string  // bytes or base64 string
  shadow_map_width?: number
  shadow_map_height?: number
  cell_skip?: number[] | Uint32Array
}

// Intersect a ray with the Z=0 plane. Returns [worldX, worldY] or null when
// the ray is parallel to the plane (camera looking horizontally — guarded by
// the camera's PITCH_MIN clamp in practice).
export function rayGroundPlane(
  originX: number, originY: number, originZ: number,
  dirX: number, dirY: number, dirZ: number,
): [number, number] | null {
  if (Math.abs(dirZ) < 1e-6) return null
  const t = -originZ / dirZ
  if (!isFinite(t)) return null
  return [originX + dirX * t, originY + dirY * t]
}

// Decode the shadow byte for a (col, row) cell. Maps the cell's BL-corner to
// the shadow grid (W-1)*4 × (H-1)*4. Returns -1 when no shadow data exists.
function shadowAtCell(
  col: number, row: number,
  t: TerrainDataForPicking,
): number {
  if (!t.shadow_map || !t.shadow_map_width || !t.shadow_map_height) return -1
  // BL-corner of cell (col, row) sits at shadow (col*4, row*4) approximately.
  const sx = col * 4
  const sy = row * 4
  if (sx < 0 || sy < 0 || sx >= t.shadow_map_width || sy >= t.shadow_map_height) return -1
  const idx = sy * t.shadow_map_width + sx
  let bytes: Uint8Array
  if (typeof t.shadow_map === 'string') {
    // Base64 (Go encoding/json default for []byte). Decode lazily.
    try {
      const bin = atob(t.shadow_map)
      bytes = new Uint8Array(bin.length)
      for (let i = 0; i < bin.length; i++) bytes[i] = bin.charCodeAt(i)
    } catch { return -1 }
  } else {
    bytes = t.shadow_map
  }
  if (idx >= bytes.length) return -1
  return bytes[idx]
}

export function pickTerrainCell(
  px: number, py: number,
  canvas: HTMLCanvasElement,
  scene: any,
  terrain: TerrainDataForPicking,
): TerrainCellInfo | null {
  // screenToWorldRay → near + far points (6 floats: vec3 + vec3).
  const out = new Float32Array(6)
  const vp = (scene as any).viewport as Float32Array
  // mdx-m3-viewer's screenToWorldRay → unproject() expects DOM-top-origin Y
  // (its `y_ndc = 1 - 2*v[1]/h` formula gives y_ndc=+1 for v[1]=0). The
  // earlier "flip with canvas.clientHeight - py" mirrored cell picks around
  // the canvas midpoint — same bug as the entity ray-pick. Pass DOM coords
  // through directly.
  const screen = new Float32Array([px, py])
  scene.camera.screenToWorldRay(out, screen, vp)
  const ox = out[0], oy = out[1], oz = out[2]
  const dx = out[3] - ox, dy = out[4] - oy, dz = out[5] - oz
  const hit = rayGroundPlane(ox, oy, oz, dx, dy, dz)
  if (!hit) return null
  const [worldX, worldY] = hit

  const W = terrain.width
  const H = terrain.height
  if (W <= 0 || H <= 0) return null

  // Convert world XY → cell (col, row). center_offset is the game-space
  // position of vertex (0,0); each cell is 128 studs.
  const col = Math.floor((worldX - terrain.center_offset[0]) / 128)
  const row = Math.floor((worldY - terrain.center_offset[1]) / 128)

  // Bounds-check against the cell grid (W-1) × (H-1); the BL corner of a
  // valid cell ranges 0..W-2 / 0..H-2. Out-of-range = clicked outside map.
  if (col < 0 || row < 0 || col > W - 2 || row > H - 2) return null

  const bl = row * W + col
  const br = bl + 1
  const tl = bl + W
  const tr = tl + 1

  const paletteIdx = Number(terrain.ground_tex[bl] ?? 0)
  const cliffTexIdx = Number(terrain.cliff_tex[bl] ?? 0)
  const cellSkipIdx = row * (W - 1) + col
  const cellSkip = !!(terrain.cell_skip && terrain.cell_skip[cellSkipIdx])
  return {
    col,
    row,
    worldX,
    worldY,
    paletteIdx,
    paletteFourCC: terrain.palette[paletteIdx] ?? '',
    paletteTexture: terrain.palette_textures?.[paletteIdx] ?? '',
    layerHeight: Number(terrain.layer_height[bl] ?? 0),
    cliffTexIdx,
    cliffFourCC: terrain.cliff_palette[cliffTexIdx] ?? '',
    cliffVar: Number(terrain.cliff_var[bl] ?? 0),
    groundVar: Number(terrain.ground_var[bl] ?? 0),
    rampFlags: Number(terrain.ramp_flags[bl] ?? 0),
    waterZ: Number(terrain.water_z[bl] ?? 0),
    hasWater: Number(terrain.has_water[bl] ?? 0) !== 0,
    cornerPalettes: [
      Number(terrain.ground_tex[bl] ?? 0),
      Number(terrain.ground_tex[br] ?? 0),
      Number(terrain.ground_tex[tl] ?? 0),
      Number(terrain.ground_tex[tr] ?? 0),
    ],
    cellSkip,
    shadow: shadowAtCell(col, row, terrain),
    cellIndex: bl,
  }
}
