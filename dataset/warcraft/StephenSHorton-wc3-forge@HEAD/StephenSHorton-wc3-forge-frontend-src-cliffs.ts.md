---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/cliffs.ts"
ref: HEAD
---

// Per-cell cliff mesh selection + placement. Mirrors HiveWE's
// `update_cliff_meshes` in `src/base/terrain.ixx`. Placement logic =
// computeCliffPlacements; rendering = createCliffRenderer in cliff-shader.ts.
//
// Algorithm sketch:
//   For each cell (i, j) whose four corners are NOT all at the same layer
//   height, render a cliff transition mesh. The mesh chosen depends on the
//   pattern of layer heights at the 4 corners — encoded as a 4-character
//   string of layer-height-offsets-from-the-min, with each char being
//   'A' + offset:
//     "AAAA" → no transition (filtered out)
//     "BAAA" → TL one layer above the rest
//     "BBAA" → top edge is one layer above bottom edge
//     "BCBA" → TR two above, BR one above, others at base
//   …and so on.
//
//   The filename is then `Doodads/Terrain/Cliffs/Cliffs<pattern><variation>.mdx`
//   where <variation> is the per-cell variation (0..N) chosen at map-author
//   time and stored in the bottom-left corner's `cliff_var`. We try the
//   stored variation; if the file 404s we fall back to variation 0.
//
//   Ramps (sloped transitions) are a 2-cell span with a clifftrans MDX.
//   See passes 1 + 2 below.
//
// ── Rendering pipeline (HiveWE-faithful) ─────────────────────────────────
//
// The OLD path used mdx-m3-viewer's instance system (model.addInstance() +
// inst.rotateLocal + inst.move + inst.setScene), but that path renders
// cliffs at a FIXED layer-Z mesh-top — ramps' sloped tops didn't conform
// to the actual ramp surface, leaving voids.
//
// The NEW path (this file + cliff-shader.ts) implements HiveWE's cliff.vert
// per-vertex displacement: each cliff vertex looks up the corresponding
// final-ground-height at its corner and the mesh top drapes over the
// terrain corners (vertical for pure cliffs, sloped for ramps).
//
// The 90° unrotation of the cliff MDX (HiveWE's vec3(vPos.y, -vPos.x, vPos.z)
// swap) lives INSIDE the new cliff vertex shader. The previous per-instance
// `inst.rotateLocal(CLIFF_UNROTATE_QUAT)` is REMOVED — the new path doesn't
// create lib instances, and the shader handles the rotation in-pipe.

import { flog, flogWarn } from './debuglog'
import { registerDiag } from './diag-registry'
// Deep import: the top-level package exports a namespace bundle whose
// `viewer.ModelViewer` is the class — there's no named `ModelViewer` re-export
// at the package root. The path mirrors how icon-loader.ts reaches into
// `mdx-m3-viewer/dist/cjs/parsers` for the same reason.
import type ModelViewer from 'mdx-m3-viewer/dist/cjs/viewer/viewer'
import {
  createCliffRenderer,
  uploadFinalHeights,
  type CliffInstance,
  type CliffRenderer,
} from './cliff-shader'

interface TerrainCliffData {
  width: number   // vertex count along X
  height: number  // vertex count along Y
  center_offset: [number, number]
  // Per-corner SMOOTH height in studs (Tilepoint.Z() only — the wobble,
  // WITHOUT the layer-step offset). This is what the cliff vertex shader
  // displaces by. Mirrors HiveWE's `corner_height` array, which is what
  // HiveWE binds to the cliff shader's SSBO 0 (terrain.ixx:652) — NOT the
  // full FinalGroundHeights. The cliff MDX geometry already encodes the
  // layer-step rise in vertex Z; adding it again here would double it.
  cliff_displacement: number[]
  layer_height: number[]
  cliff_tex: number[]
  cliff_var: number[]
  ramp_flags: number[]
  cliff_palette: string[]
  cliff_palette_textures: string[]
}

export interface CliffPlacement {
  /** MDX path to load. */
  path: string
  /** World position of the cell's bottom-left corner at the base layer's Z. */
  pos: [number, number, number]
  /** Cliff palette index (CornerCliffTexture[BL] from .w3e). */
  paletteIdx: number
}

// --- Diagnostics counters (U6) -------------------------------------------
// Cliff placement has two quiet silent-failure sites: a cliff/clifftrans MDX
// that 404s (cell ends up with a hole) and a cliff-palette texture that fails
// to load (cliff renders untextured). Both currently only flog. Surface them
// as precomputed counters so diagnostics.get / the F9 overlay can show a map
// with broken cliff geometry without scraping the log. *Fails/*Miss suffix →
// overlay auto-red-tint. lastLoad is the most-recent renderCliffs() summary.
const cliffDiag = {
  lastPlaced: 0,
  lastFailedCells: 0,
  paletteFails: 0,    // cumulative: cliff palette textures that failed to load
  meshMissPaths: 0,   // cumulative: unique MDX paths that 404'd both variations
}
registerDiag('cliffs', () => ({ ...cliffDiag }))

export function computeCliffPlacements(t: TerrainCliffData): CliffPlacement[] {
  const out: CliffPlacement[] = []
  if (!t || !t.layer_height || t.layer_height.length === 0) return out

  const W = t.width
  const H = t.height
  if (W < 2 || H < 2) return out
  const cx = t.center_offset[0]
  const cy = t.center_offset[1]

  const idx = (i: number, j: number) => j * W + i
  const ramp = (k: number) => (t.ramp_flags[k] & 1) !== 0

  // `consumedByRamp` mirrors HiveWE's `corner_romp` — corners covered by a
  // ramp clifftrans piece. Regular-cliff placement skips cells whose BL is
  // in this set. (Go side computes this same flag for cell_skip; we
  // recompute here in parallel since we need the per-placement decision.)
  const consumedByRamp = new Set<number>()

  // Resolve cliff palette idx for a cell — clamped to non-negative, since
  // cliff_tex stores 0..15 and a value of 15 is "no cliff". HiveWE's
  // `texture == 15 ⇒ texture -= 14` quirk is applied for the palette
  // lookup (so 15 → 1).
  function resolvePaletteIdx(blIdx: number): number {
    const raw = t.cliff_tex[blIdx] & 0xF
    if (raw === 15) return 1
    return raw
  }

  // Filename char for one corner of a clifftrans piece. Non-ramp corners use
  // a normal 'A'+offset encoding; ramp corners use 'L' + offset * -4.
  function rampChar(isRamp: boolean, layerHeight: number, base: number): number {
    const baseChar = isRamp ? 76 /* 'L' */ : 65 /* 'A' */
    const scale = isRamp ? -4 : 1
    return baseChar + (layerHeight - base) * scale
  }

  // --- Pass 1: vertical ramps (3 corners tall on each side) ---
  for (let j = 0; j < H - 2; j++) {
    for (let i = 0; i < W - 1; i++) {
      const bl = idx(i, j), br = idx(i + 1, j)
      const tl = idx(i, j + 1), tr = idx(i + 1, j + 1)
      const ttl = idx(i, j + 2), ttr = idx(i + 1, j + 2)

      const lhBL = t.layer_height[bl], lhBR = t.layer_height[br]
      const lhTL = t.layer_height[tl], lhTR = t.layer_height[tr]
      const lhTTL = t.layer_height[ttl], lhTTR = t.layer_height[ttr]

      const ae = Math.min(lhBL, lhTTL)
      const cf = Math.min(lhBR, lhTTR)
      if (lhTL !== ae || lhTR !== cf) continue

      const rBL = ramp(bl), rTL = ramp(tl), rTTL = ramp(ttl)
      const rBR = ramp(br), rTR = ramp(tr), rTTR = ramp(ttr)
      if (rBL !== rTL || rBL !== rTTL) continue
      if (rBR !== rTR || rBR !== rTTR) continue
      if (rBL === rBR) continue

      if (lhBL === lhTTL && lhBL === lhBR && lhBL === lhTTR) continue

      const base = Math.min(ae, cf)
      const name = String.fromCharCode(
        rampChar(rTTL, lhTTL, base),
        rampChar(rTTR, lhTTR, base),
        rampChar(rBR, lhBR, base),
        rampChar(rBL, lhBL, base),
      )
      const path = `doodads/terrain/clifftrans/clifftrans${name}0.mdx`
      out.push({
        path,
        pos: [cx + i * 128, cy + j * 128, (base - 2) * 128],
        paletteIdx: resolvePaletteIdx(bl),
      })
      consumedByRamp.add(bl)
      consumedByRamp.add(tl)
    }
  }

  // --- Pass 2: horizontal ramps (symmetric, 3 corners wide on each side) ---
  for (let j = 0; j < H - 1; j++) {
    for (let i = 0; i < W - 2; i++) {
      const bl = idx(i, j), br = idx(i + 1, j), brr = idx(i + 2, j)
      const tl = idx(i, j + 1), tr = idx(i + 1, j + 1), trr = idx(i + 2, j + 1)

      const lhBL = t.layer_height[bl], lhBR = t.layer_height[br], lhBRR = t.layer_height[brr]
      const lhTL = t.layer_height[tl], lhTR = t.layer_height[tr], lhTRR = t.layer_height[trr]

      const ae = Math.min(lhBL, lhBRR)
      const bf = Math.min(lhTL, lhTRR)
      if (lhBR !== ae || lhTR !== bf) continue

      const rBL = ramp(bl), rBR = ramp(br), rBRR = ramp(brr)
      const rTL = ramp(tl), rTR = ramp(tr), rTRR = ramp(trr)
      if (rBL !== rBR || rBL !== rBRR) continue
      if (rTL !== rTR || rTL !== rTRR) continue
      if (rBL === rTL) continue

      if (lhBL === lhBRR && lhBL === lhTL && lhBL === lhTRR) continue

      const base = Math.min(ae, bf)
      const name = String.fromCharCode(
        rampChar(rTL, lhTL, base),
        rampChar(rTRR, lhTRR, base),
        rampChar(rBRR, lhBRR, base),
        rampChar(rBL, lhBL, base),
      )
      const path = `doodads/terrain/clifftrans/clifftrans${name}0.mdx`
      out.push({
        path,
        pos: [cx + i * 128, cy + j * 128, (base - 2) * 128],
        paletteIdx: resolvePaletteIdx(bl),
      })
      consumedByRamp.add(bl)
      consumedByRamp.add(br)
    }
  }

  // --- Pass 3: regular cliff transitions ---
  function isRampEntrance(i: number, j: number): boolean {
    if (i >= W - 1 || j >= H - 1) return false
    const bl = idx(i, j), br = idx(i + 1, j)
    const tl = idx(i, j + 1), tr = idx(i + 1, j + 1)
    if (!ramp(bl) || !ramp(br) || !ramp(tl) || !ramp(tr)) return false
    const symmetric =
      t.layer_height[bl] === t.layer_height[tr] &&
      t.layer_height[tl] === t.layer_height[br]
    return !symmetric
  }

  for (let j = 0; j < H - 1; j++) {
    for (let i = 0; i < W - 1; i++) {
      const bl = idx(i, j)
      if (consumedByRamp.has(bl)) continue
      if (isRampEntrance(i, j)) continue
      const br = idx(i + 1, j)
      const tl = idx(i, j + 1)
      const tr = idx(i + 1, j + 1)

      const lh_bl = t.layer_height[bl]
      const lh_br = t.layer_height[br]
      const lh_tl = t.layer_height[tl]
      const lh_tr = t.layer_height[tr]

      if (lh_bl === lh_br && lh_bl === lh_tl && lh_bl === lh_tr) continue

      const base = Math.min(lh_tl, lh_tr, lh_br, lh_bl)
      const fileName = String.fromCharCode(
        65 + lh_tl - base,
        65 + lh_tr - base,
        65 + lh_br - base,
        65 + lh_bl - base,
      )
      if (fileName === 'AAAA') continue

      const variation = t.cliff_var[bl] | 0
      const path = `Doodads/Terrain/Cliffs/Cliffs${fileName}${variation}.mdx`

      out.push({
        path,
        pos: [cx + i * 128, cy + j * 128, (base - 2) * 128],
        paletteIdx: resolvePaletteIdx(bl),
      })
    }
  }
  return out
}

export interface CliffRendering {
  /** Draw all cliff instances. Call from the per-frame render loop AFTER
   *  terrain.draw and BEFORE viewer.render() so cliffs sit at the right
   *  depth relative to MDX-rendered units/doodads. */
  draw(viewProj: Float32Array): void
  /** Number of cliff instances actually placed (some MDXs may 404). */
  placed: number
  /** Disposal: release all GL resources for this load. */
  dispose(): void
}

export async function renderCliffs(
  gl: WebGLRenderingContext,
  viewer: ModelViewer,
  pathSolver: any,
  placements: CliffPlacement[],
  t: TerrainCliffData,
): Promise<CliffRendering | null> {
  // 1. Upload the per-corner SMOOTH-height array as a float texture for the
  //    cliff vertex shader's per-vertex displacement lookup. CRITICAL: this
  //    must be Tilepoint.Z() only (the per-corner wobble), NOT FinalZ
  //    (Z + (layer-2)*128). The cliff MDX vertex Z already encodes the full
  //    layer-step rise (a 1-step cliff MDX spans vPos.z = 0..128 studs);
  //    if we also displaced by (layer-2)*128 we'd double the cliff height.
  //    This matches HiveWE's `ground_height_buffer` binding on the cliff
  //    shader (terrain.ixx:652) which carries `corner_height` (the wobble).
  const heights = new Float32Array(t.cliff_displacement)
  const finalHeights = uploadFinalHeights(gl, heights, t.width, t.height)
  if (!finalHeights) {
    flog('[cliffs] finalHeights upload failed; cliffs disabled')
    return null
  }

  // 2. Load cliff-palette textures (one per CliffPalette entry). Skip
  //    silently when a palette has no SLK path — that palette won't be
  //    used by any instance with the right `paletteIdx`, or if it is, the
  //    cliff-shader.ts draw loop skips it (better silent than magenta).
  const paletteTextures: { tex: WebGLTexture | null; width: number; height: number }[] =
    new Array(t.cliff_palette.length).fill(null).map(() => ({ tex: null, width: 0, height: 0 }))
  const palettePromises: Promise<void>[] = []
  for (let p = 0; p < t.cliff_palette.length; p++) {
    const stem = t.cliff_palette_textures?.[p]
    if (!stem) {
      flog(`[cliffs] cliff palette[${p}] (${t.cliff_palette[p]}) has no SLK texture path`)
      continue
    }
    // Try .dds first (Reforged CASC ships these as DDS), the asset handler's
    // BLP↔DDS swap covers SD-installs too.
    const path = stem + '.dds'
    palettePromises.push(
      viewer.load(path, pathSolver).then((res: any) => {
        if (!res || !res.webglResource) {
          cliffDiag.paletteFails++
          flogWarn(`[cliffs] cliff palette[${p}] (${t.cliff_palette[p]}) failed to load: ${path}`)
          return
        }
        paletteTextures[p] = {
          tex: res.webglResource as WebGLTexture,
          width: res.width as number,
          height: res.height as number,
        }
      }).catch((e: unknown) => {
        cliffDiag.paletteFails++
        flogWarn(`[cliffs] cliff palette[${p}] load threw:`, e instanceof Error ? e.message : String(e))
      }),
    )
  }
  await Promise.all(palettePromises)

  // 3. Create the renderer.
  const renderer = createCliffRenderer(gl, {
    finalHeights,
    centerOffset: t.center_offset,
    paletteTextures,
  })
  if (!renderer) {
    gl.deleteTexture(finalHeights.tex)
    return null
  }

  // 4. Load each unique MDX (via mdx-m3-viewer's parser — we only borrow
  //    the parsed geometry buffers, not the instance system).
  const byPath = new Map<string, CliffPlacement[]>()
  for (const p of placements) {
    const list = byPath.get(p.path) ?? []
    list.push(p)
    byPath.set(p.path, list)
  }

  let placed = 0
  let failed = 0
  for (const [path, group] of byPath) {
    let model: any
    try { model = await viewer.load(path, pathSolver) } catch { /* fall through */ }
    if (!model || !model.geosets || model.geosets.length === 0) {
      const fallback = path.replace(/(\d+)\.mdx$/i, '0.mdx')
      if (fallback !== path) {
        try { model = await viewer.load(fallback, pathSolver) } catch { /* ignore */ }
      }
    }
    if (!model || !model.geosets || model.geosets.length === 0) {
      failed += group.length
      cliffDiag.meshMissPaths++
      if (failed === group.length || failed < 10) {
        flogWarn(`[cliffs miss] ${path} (×${group.length} cells)`)
      }
      continue
    }
    const insts: CliffInstance[] = group.map(p => ({
      cellWorldX: p.pos[0],
      cellWorldY: p.pos[1],
      baseZ: p.pos[2],
      paletteIdx: p.paletteIdx,
    }))
    renderer.addMesh(path, model, insts)
    placed += group.length
  }
  cliffDiag.lastPlaced = placed
  cliffDiag.lastFailedCells = failed
  if (failed > 0) {
    flogWarn(`[cliffs] placed=${placed} failed=${failed} unique-paths=${byPath.size}`)
  } else {
    flog(`[cliffs] placed=${placed} unique-paths=${byPath.size}`)
  }

  return {
    placed,
    draw(viewProj) { renderer.draw(viewProj) },
    dispose() {
      renderer.dispose()
      gl.deleteTexture(finalHeights.tex)
      // Palette textures are owned by mdx-m3-viewer's resourceMap — don't
      // delete; the next loadMap may reuse them.
    },
  }
}
