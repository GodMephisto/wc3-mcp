---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/terrain.ts"
ref: HEAD
---

// Textured terrain renderer with HiveWE-style N-layer alpha-composite blending.
//
// ── Algorithm ───────────────────────────────────────────────────────────────
//
// Per cell (i, j) — defined by its 4 corner tilepoints (bl, br, tl, tr):
//
//  1. Collect the 4 corners' palette indices. Up to 4 unique values per cell;
//     uniform cells have 1, transition cells have 2-4.
//
//  2. Sort & dedupe the unique palette indices in ASCENDING order. Palette
//     ordering in war3map.w3e is the SLK "dir" priority order — lowest-index
//     ground textures (typically base grass) get painted FIRST, highest-index
//     (typically dirt / transitions) get painted LAST on top. HiveWE's
//     terrain.ixx does exactly this with `std::sort(u, u + 4)`.
//
//  3. The lowest-index palette is the BASE LAYER. It is rendered with its own
//     "pure variation" sub-tile (slot picked by `pickSubTile(extended, var)`):
//       - non-extended palettes: slot 0 (variation==0) or slot 15 (else)
//       - extended palettes:     slot 16+var (var<=15), slot 15 (var==16),
//                                slot 0 otherwise
//
//  4. Higher-priority palettes (k = 1..3) each carry a per-cell 4-bit
//     CORNER-COVERAGE MASK packing which of the 4 corners use that palette:
//       bit 0 → bottom-right, bit 1 → bottom-left,
//       bit 2 → top-right,    bit 3 → top-left.
//     The mask itself (range 0..15) IS the sub-tile slot inside that
//     palette's 4×4 alpha-edged-variant grid. WC3 tile textures hand-author
//     these 16 slots as pre-baked alpha-edge transitions: slot N's alpha
//     channel masks ONLY the corners whose bits are set in N. So sampling
//     `palette[k]` at slot=mask gives the correctly-faded coverage automatically.
//
//  5. The fragment shader composites layers in order. Starting from the base
//     layer (fully opaque), each higher layer is `mix(prev, layer, layer.a)`
//     where `layer.a` is the alpha-edged variant's per-pixel alpha. Cells with
//     fewer than 4 layers use a sentinel (negative atlas-U coord) to skip.
//
// ── Implementation notes ─────────────────────────────────────────────────────
//
// WebGL1: no `sampler2DArray`, no dynamic indexing into sampler arrays. To
// keep the algorithm in a single draw call against a single `sampler2D`, all
// loaded palette textures are pre-composited into one vertical-strip atlas
// (one palette per 256-pixel-tall row, atlas width = 256 for non-extended-only
// maps or 512 if any extended palette exists) via FBO-blit at build time. The
// frag shader samples that atlas at most 4 times — once per layer — at offsets
// derived from per-cell vertex attributes.
//
// Per-vertex inputs:
//   a_position  — world XYZ
//   a_shadowUV  — shadow-map sample coord (per vertex; unused if no shadow)
//   a_cellUV    — cell-local UV (0,0) at BL through (1,1) at TR
//   a_layerUV01 — base-layer subUV (xy) + layer-1 subUV (zw)
//   a_layerUV23 — layer-2 subUV (xy) + layer-3 subUV (zw)
//
// All 4 verts of a cell carry the same a_layerUV01 / a_layerUV23 (it's per-
// cell data); only a_position, a_shadowUV, a_cellUV vary per vertex.
//
// `subUV` is the BOTTOM-LEFT corner of the sub-tile in atlas-UV coordinates
// (v-up). Sub-tile size is uniform — every palette is 256×256 (non-extended,
// 4×4 sub-tiles) or 512×256 (extended, 8×4 sub-tiles), and every sub-tile is
// 64×64 pixels. So the size in atlas UV is `(64/atlasW, 64/atlasH)`, a single
// uniform shared across the whole draw.
//
// Sentinel for "no layer N" = `subUV.x < 0`. The shader compares and skips.
//
// Drawn between viewer.startFrame() and viewer.render() so it lands in the
// depth buffer before unit instances.

import { flog, flogWarn } from './debuglog'
import { registerDiag } from './diag-registry'

// --- Diagnostics counters (U6 + U5) --------------------------------------
// Terrain build has several quiet silent-failure sites: a palette .dds that
// 404s (cell falls back to flat color), an FBO-incomplete atlas blit (whole
// terrain aborts to null → invisible ground), and a dim/heights mismatch.
// These currently only flog. Surface them as precomputed integers + a small
// status string so diagnostics.get / the F9 overlay can flag a map whose
// terrain silently failed. *Fails suffix → overlay auto-red-tint. terrain
// atlasStatus is the U5 "scene-load stats" health field for the atlas blit.
const terrainDiag = {
  atlasStatus: 'none' as 'none' | 'ok' | 'fbo-incomplete' | 'failed',
  lastPalettes: 0,
  lastAtlasW: 0,
  lastAtlasH: 0,
  paletteFails: 0,    // cumulative: palette textures that failed to load
  fboIncompleteFails: 0, // cumulative: atlas FBO came back incomplete
}
registerDiag('terrain', () => ({ ...terrainDiag }))

// Matches the JSON shape App.GetTerrain returns. Field names are snake_case
// because the Go side carries `json:"..."` struct tags (Wails honors those
// when marshaling — not the Go field name).
interface TerrainDTO {
  width: number
  height: number
  center_offset: [number, number]
  heights: number[]
  ground_tex: number[]
  ground_var: number[]
  tileset: string
  palette: string[]
  palette_colors: [number, number, number][]
  palette_textures: string[]
  shadow_map: string
  shadow_map_width: number
  shadow_map_height: number
  // Per-cell skip mask (1 = don't render terrain quad). Mirrors HiveWE's
  // gpu_ground_exists == 0 test. Cliff cells (non-ramp-entrance) get
  // their vertical face covered by cliff MDX; rendering the terrain quad
  // on top produces diagonal Z-interpolated slopes that punch through
  // the cliff geometry. Length = (width-1)*(height-1), row-major.
  cell_skip?: number[]
}

// Mirrors HiveWE's `get_tile_variation`: the per-corner variation byte
// selects a sub-tile slot inside the texture's 4×4 (non-extended) or 8×4
// (extended) sub-tile layout. ONLY used for the BASE layer of a cell — higher
// layers use the corner-coverage mask directly as the slot index (slots 0..15
// are the alpha-edged variants for both extended and non-extended palettes).
function pickSubTile(extended: boolean, variation: number): number {
  if (extended) {
    if (variation <= 15) return 16 + variation
    if (variation === 16) return 15
    return 0
  }
  if (variation === 0) return 0
  return 15
}

// Vertex shader: pure pass-through of per-cell layer attributes plus per-
// vertex normal for lighting. Normals are computed CPU-side from neighbor-
// corner heights (same algorithm as HiveWE terrain.vert):
//   normal = normalize((hL - hR, hD - hU, 2.0))
// where hL/hR/hD/hU are the heights of the 4 cardinal neighbors (one tile-
// width = 128 studs away, but the 2.0 in the Z slot already bakes the
// implicit 2*128-wide finite-difference into the slope). HiveWE uses
// vertex-shader SSBO lookup; we precompute on the CPU since per-vertex
// attribute upload is cheap and WebGL1 doesn't have SSBOs anyway.
const VERT_SHADER = `
attribute vec3 a_position;
attribute vec2 a_shadowUV;
attribute vec2 a_cellUV;
attribute vec4 a_layerUV01; // base.uv, layer1.uv
attribute vec4 a_layerUV23; // layer2.uv, layer3.uv
attribute vec3 a_normal;
uniform mat4 u_viewProj;
varying vec2 v_shadowUV;
varying vec2 v_cellUV;
varying vec4 v_layerUV01;
varying vec4 v_layerUV23;
varying vec3 v_normal;
void main() {
  v_shadowUV = a_shadowUV;
  v_cellUV = a_cellUV;
  v_layerUV01 = a_layerUV01;
  v_layerUV23 = a_layerUV23;
  v_normal = a_normal;
  gl_Position = u_viewProj * vec4(a_position, 1.0);
}
`.trim()

// Fragment shader: alpha-composite up to 4 layers.
//
// For each layer L with sub-tile origin `lo` in atlas UV, sample the atlas at
// `lo + (cellUV.x, 1.0 - cellUV.y) * u_subSize`. The v-flip aligns the image-
// space (y-down) sub-tile interior with cellUV (y-up): cellUV.y=1 (top of
// cell) → sample row 0 of the sub-tile.
//
// The base layer is opaque (its alpha is ignored — it covers the cell fully).
// Each higher layer is `mix(prev, layer, layer.a)` so the layer's alpha-edged
// transparency reveals what's underneath.
//
// Sentinel: a layer with `lo.x < -0.5` is "no layer here" and gets skipped.
// Using -1 in the attribute and a generous -0.5 threshold dodges any
// interpolation/precision drift across the cell (the attribute is flat-shaded
// in spirit but varying-interpolated by the GL pipeline — for all 4 verts of
// a cell the attribute value is identical, so interpolation is a no-op, but
// keeping the threshold generous costs nothing).
const FRAG_SHADER = `
precision mediump float;
uniform sampler2D u_atlas;
uniform sampler2D u_shadowTex;
uniform bool u_hasShadow;
uniform vec2 u_subSize;     // (sub-tile width, sub-tile height) in atlas UV
uniform vec3 u_lightDir;    // unit vector; matches HiveWE map.ixx (normalize(1,1,-3))
varying vec2 v_shadowUV;
varying vec2 v_cellUV;
varying vec4 v_layerUV01;
varying vec4 v_layerUV23;
varying vec3 v_normal;

vec4 sampleLayer(vec2 lo) {
  // lo.x < -0.5 = sentinel = no layer here.
  if (lo.x < -0.5) {
    return vec4(0.0, 0.0, 0.0, 0.0);
  }
  // Sub-tile-local UV: image v-down within the sub-tile.
  vec2 atlasUV = vec2(lo.x + v_cellUV.x * u_subSize.x,
                      lo.y + (1.0 - v_cellUV.y) * u_subSize.y);
  return texture2D(u_atlas, atlasUV);
}

void main() {
  // Layer 0 = base (lowest-index palette). Its sub-tile is the per-corner
  // variation slot — fully opaque on the interior. Alpha is IGNORED for base:
  // base color covers the entire cell.
  vec4 base = sampleLayer(v_layerUV01.xy);
  vec3 col = base.rgb;

  // Layers 1..3 — each may be a sentinel (skipped).
  vec4 l1 = sampleLayer(v_layerUV01.zw);
  col = mix(col, l1.rgb, l1.a);

  vec4 l2 = sampleLayer(v_layerUV23.xy);
  col = mix(col, l2.rgb, l2.a);

  vec4 l3 = sampleLayer(v_layerUV23.zw);
  col = mix(col, l3.rgb, l3.a);

  // Half-Lambert lighting, exact port of HiveWE terrain.frag line 42-44:
  //   contribution = (dot(-light_direction, normal) + 1) * 0.5
  //   color *= clamp(contribution, 0, 1)
  // The +1 then *0.5 maps a [-1..1] dot to [0..1] (so backfaces aren't
  // fully black). With HiveWE's default light (normalize(1,1,-3)) a flat
  // ground normal (0,0,1) yields contribution = (0.905 + 1)/2 = 0.95.
  // This is the missing piece that gave wc3-forge over-saturated raw-DDS
  // teal where HiveWE shows neutral lit ground.
  vec3 n = normalize(v_normal);
  float contribution = (dot(-u_lightDir, n) + 1.0) * 0.5;
  col *= clamp(contribution, 0.0, 1.0);

  if (u_hasShadow) {
    float shadow = texture2D(u_shadowTex, v_shadowUV).r;
    col *= 1.0 - shadow * 0.45;
  }
  gl_FragColor = vec4(col, 1.0);
}
`.trim()

function compileShader(gl: WebGLRenderingContext, type: number, source: string): WebGLShader {
  const sh = gl.createShader(type)!
  gl.shaderSource(sh, source)
  gl.compileShader(sh)
  if (!gl.getShaderParameter(sh, gl.COMPILE_STATUS)) {
    const log = gl.getShaderInfoLog(sh)
    gl.deleteShader(sh)
    throw new Error('terrain shader compile: ' + log)
  }
  return sh
}

function buildProgram(gl: WebGLRenderingContext) {
  const program = gl.createProgram()!
  gl.attachShader(program, compileShader(gl, gl.VERTEX_SHADER, VERT_SHADER))
  gl.attachShader(program, compileShader(gl, gl.FRAGMENT_SHADER, FRAG_SHADER))
  gl.linkProgram(program)
  if (!gl.getProgramParameter(program, gl.LINK_STATUS)) {
    const log = gl.getProgramInfoLog(program)
    gl.deleteProgram(program)
    throw new Error('terrain program link: ' + log)
  }
  return {
    program,
    aPosition: gl.getAttribLocation(program, 'a_position'),
    aShadowUV: gl.getAttribLocation(program, 'a_shadowUV'),
    aCellUV: gl.getAttribLocation(program, 'a_cellUV'),
    aLayerUV01: gl.getAttribLocation(program, 'a_layerUV01'),
    aLayerUV23: gl.getAttribLocation(program, 'a_layerUV23'),
    aNormal: gl.getAttribLocation(program, 'a_normal'),
    uViewProj: gl.getUniformLocation(program, 'u_viewProj')!,
    uAtlas: gl.getUniformLocation(program, 'u_atlas')!,
    uShadowTex: gl.getUniformLocation(program, 'u_shadowTex')!,
    uHasShadow: gl.getUniformLocation(program, 'u_hasShadow')!,
    uSubSize: gl.getUniformLocation(program, 'u_subSize')!,
    uLightDir: gl.getUniformLocation(program, 'u_lightDir')!,
  }
}

// Decode the base64 shadow map and upload as a single-channel GL texture.
function uploadShadowTexture(
  gl: WebGLRenderingContext, b64: string, w: number, h: number,
): WebGLTexture | null {
  if (!b64 || w <= 0 || h <= 0) return null
  const bin = atob(b64)
  if (bin.length !== w * h) {
    flog(`[terrain shadow] size mismatch (got ${bin.length}, want ${w}*${h}=${w * h})`)
    return null
  }
  const bytes = new Uint8Array(bin.length)
  for (let i = 0; i < bin.length; i++) bytes[i] = bin.charCodeAt(i)

  const tex = gl.createTexture()
  if (!tex) return null
  gl.bindTexture(gl.TEXTURE_2D, tex)
  gl.pixelStorei(gl.UNPACK_ALIGNMENT, 1)
  gl.texImage2D(gl.TEXTURE_2D, 0, gl.LUMINANCE, w, h, 0, gl.LUMINANCE, gl.UNSIGNED_BYTE, bytes)
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR)
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR)
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE)
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE)
  return tex
}

// Composite each loaded palette texture into a single vertical-strip atlas
// via FBO-blit. Atlas layout: width = max palette width (256 or 512),
// height = numPalettes * 256, palette p occupies row [p*256 .. (p+1)*256).
//
// Extended palettes (512×256) fill the row width; non-extended (256×256)
// occupy the left half and the right half is left clear (sub-tile picks for
// non-extended palettes never index beyond u < 0.5 — pickSubTile only
// returns slots 0..15 there, and the per-corner subUV computed at build time
// stays inside that half).
//
// Returns the atlas texture handle plus per-palette metadata the geometry
// build step needs to map (paletteIdx, subSlot) → atlas UV.
interface PaletteRow {
  extended: boolean
  loaded: boolean
  fallbackColor: [number, number, number]
}
function buildAtlas(
  gl: WebGLRenderingContext,
  palettes: { glTex: WebGLTexture | null; extended: boolean; width: number; height: number; fallback: [number, number, number] }[],
): { atlas: WebGLTexture; atlasW: number; atlasH: number; rows: PaletteRow[] } | null {
  const numPalettes = palettes.length
  if (numPalettes === 0) return null

  // Atlas width = widest palette (256 for non-extended-only, 512 if any
  // extended). Atlas height = numPalettes * 256, rounded UP to the next
  // power-of-two so WebGL1's mipmap-completeness rule is satisfied. WebGL1
  // requires PoT dimensions for textures sampled with LINEAR_MIPMAP_LINEAR;
  // a non-PoT atlas with that filter samples as black. The padding sits at
  // the BOTTOM of the atlas (atlas pixel y in [0, atlasH - atlasH_data));
  // sub-tile UV math never targets it, but high-LOD mips will average
  // padding into the bottom palette's lowest sub-tile row — see the
  // inter-palette-bleed note on `insetV` below.
  let atlasW = 256
  for (const p of palettes) {
    if (p.glTex && p.width > atlasW) atlasW = p.width
  }
  const rowH = 256
  const atlasH_data = numPalettes * rowH
  let atlasH = 1
  while (atlasH < atlasH_data) atlasH <<= 1
  const padded = atlasH !== atlasH_data

  const atlas = gl.createTexture()
  if (!atlas) return null
  gl.bindTexture(gl.TEXTURE_2D, atlas)
  gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, atlasW, atlasH, 0, gl.RGBA, gl.UNSIGNED_BYTE, null)
  // Initial MIN_FILTER is LINEAR; we flip to LINEAR_MIPMAP_LINEAR after the
  // FBO blits + generateMipmap below. WebGL1 rejects mipmap filters on a
  // texture whose mip chain hasn't been generated yet.
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR)
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR)
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE)
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE)

  // FBO with the atlas as color attachment; we draw each palette into its
  // row via a fullscreen-style textured quad. Viewport is set to the row's
  // sub-rect of the atlas, so the quad covers exactly that row.
  const fbo = gl.createFramebuffer()
  gl.bindFramebuffer(gl.FRAMEBUFFER, fbo)
  gl.framebufferTexture2D(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.TEXTURE_2D, atlas, 0)
  const fbStatus = gl.checkFramebufferStatus(gl.FRAMEBUFFER)
  if (fbStatus !== gl.FRAMEBUFFER_COMPLETE) {
    terrainDiag.atlasStatus = 'fbo-incomplete'
    terrainDiag.fboIncompleteFails++
    flogWarn(`[terrain atlas] FBO incomplete: 0x${fbStatus.toString(16)}`)
    gl.bindFramebuffer(gl.FRAMEBUFFER, null)
    gl.deleteFramebuffer(fbo)
    gl.deleteTexture(atlas)
    return null
  }

  // Build a tiny throwaway program: textured quad (positions in clip space,
  // UVs 0..1) sampling u_src. We use it once per palette row, then dispose.
  const QV = `
attribute vec2 a_p;
attribute vec2 a_uv;
varying vec2 v_uv;
void main() {
  v_uv = a_uv;
  gl_Position = vec4(a_p, 0.0, 1.0);
}
`.trim()
  const QF = `
precision mediump float;
uniform sampler2D u_src;
varying vec2 v_uv;
void main() {
  gl_FragColor = texture2D(u_src, v_uv);
}
`.trim()
  const vs = compileShader(gl, gl.VERTEX_SHADER, QV)
  const fs = compileShader(gl, gl.FRAGMENT_SHADER, QF)
  const prog = gl.createProgram()!
  gl.attachShader(prog, vs)
  gl.attachShader(prog, fs)
  gl.linkProgram(prog)
  gl.useProgram(prog)
  const aP = gl.getAttribLocation(prog, 'a_p')
  const aUV = gl.getAttribLocation(prog, 'a_uv')
  const uSrc = gl.getUniformLocation(prog, 'u_src')!

  const quadVbo = gl.createBuffer()
  gl.bindBuffer(gl.ARRAY_BUFFER, quadVbo)
  // Quad covering clip-space [-1..1] in both axes. UV (0,0) at bottom-left,
  // (1,1) at top-right. The blit will be drawn into a viewport rect that
  // selects the destination row inside the atlas.
  gl.bufferData(gl.ARRAY_BUFFER, new Float32Array([
    -1, -1, 0, 0,
     1, -1, 1, 0,
    -1,  1, 0, 1,
     1,  1, 1, 1,
  ]), gl.STATIC_DRAW)

  // Some lib state is globally enabled by mdx-m3-viewer at boot — most
  // importantly SCISSOR_TEST (viewer.js line 102), with a scissor rect
  // pinned to the canvas size by scene.render(). That clips our FBO writes
  // to the canvas region (e.g. 768×768) and silently drops writes outside
  // it — the cause of mysteriously-empty atlas rows when atlasH > canvasH.
  // Disable scissor for the duration of the blit; the caller re-enables
  // via its own state setup (terrain.draw doesn't toggle it, but
  // viewer.update/render will set the scissor box again next frame).
  gl.disable(gl.SCISSOR_TEST)
  gl.disable(gl.BLEND)
  gl.disable(gl.DEPTH_TEST)
  gl.disable(gl.CULL_FACE)

  const rows: PaletteRow[] = new Array(numPalettes)
  for (let p = 0; p < numPalettes; p++) {
    const pal = palettes[p]
    rows[p] = { extended: pal.extended, loaded: !!pal.glTex, fallbackColor: pal.fallback }
    if (!pal.glTex) continue

    // Destination viewport rect in the atlas for palette p. v=0 is atlas
    // bottom; we lay palettes from top→bottom so palette 0 sits at the
    // visually-top of the atlas. But sampling convention only requires that
    // we agree with ourselves — pick top-to-bottom so row v-coord =
    // (atlasH - (p+1)*rowH) / atlasH .. (atlasH - p*rowH) / atlasH.
    //
    // CANONICAL atlas pitch is 64 atlas-pixels per sub-tile regardless of
    // the SOURCE palette's pixel dimensions. Sub-tile picker math below
    // (subTileAtlasOrigin) assumes this — slot col c → atlas pixel `c * 64`.
    //
    // - non-extended: 4 columns × 64 = 256 dst-px wide; right half of atlas
    //   row left blank (subUVs never reach u >= 0.5).
    // - extended:     8 columns × 64 = 512 dst-px wide; fills the full row.
    //
    // Source palette dims vary in the wild: stock CASC ships non-extended at
    // 256×256 (16 sub-tiles × 64×64) but ALSO at 512×256 extended (32 ×
    // 64×64). Custom-map BLP overrides are often 512×512 (16 sub-tiles ×
    // 128×128). All three are equally valid and HiveWE handles them via
    // `tile_size = max(height * 0.25, 1)` + `extended = (width == height*2)`.
    // We rasterize them all into the SAME canonical 64-px-per-sub-tile atlas
    // by scaling-down at blit time (the UV-textured-quad blit covers source
    // [0..1]×[0..1] regardless of source size, and the destination viewport
    // is fixed to 256 or 512 px wide × 256 px tall). This collapses all four
    // size variants into a single rendering path.
    //
    // PREVIOUSLY this line was `dstW = pal.extended ? atlasW : Math.min(pal.width, atlasW)`,
    // which gave dstW=512 for a 512×512 non-extended source when paired with
    // any extended palette in the same map (atlasW=512). That scaled the
    // source's 128-px sub-tiles to 128 atlas-px wide × 64 atlas-px tall
    // (squashed!) while the sub-tile picker still indexed at 64-px pitch —
    // sampling the wrong region for every slot but slot 0 and looking
    // visually like "rotated/wrong" sub-tiles. Fixed by anchoring dstW to
    // the canonical 256/512 pitch.
    const dstW = pal.extended ? 512 : 256
    const dstH = rowH
    const dstX = 0
    const dstY = atlasH - (p + 1) * rowH

    gl.viewport(dstX, dstY, dstW, dstH)
    gl.activeTexture(gl.TEXTURE0)
    gl.bindTexture(gl.TEXTURE_2D, pal.glTex)
    // The source palette texture was uploaded by mdx-m3-viewer with
    // its own filter/wrap state. We need CLAMP + LINEAR for a clean copy.
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE)
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE)
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR)
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR)
    gl.uniform1i(uSrc, 0)

    gl.bindBuffer(gl.ARRAY_BUFFER, quadVbo)
    gl.enableVertexAttribArray(aP)
    gl.vertexAttribPointer(aP, 2, gl.FLOAT, false, 16, 0)
    gl.enableVertexAttribArray(aUV)
    gl.vertexAttribPointer(aUV, 2, gl.FLOAT, false, 16, 8)
    gl.drawArrays(gl.TRIANGLE_STRIP, 0, 4)
  }

  // Re-enable scissor test for the viewer's per-frame pipeline.
  gl.enable(gl.SCISSOR_TEST)
  // Cleanup of the throwaway program. The atlas + rows live on.
  gl.bindFramebuffer(gl.FRAMEBUFFER, null)
  gl.deleteFramebuffer(fbo)
  gl.deleteBuffer(quadVbo)
  gl.deleteProgram(prog)
  gl.deleteShader(vs)
  gl.deleteShader(fs)

  // Generate the mip chain and switch to trilinear minification. HiveWE's
  // ground_texture.ixx loads each slot into a sampler2DArray with
  // GL_LINEAR_MIPMAP_LINEAR; we mirror that on the atlas so that at zoom
  // levels where each sub-tile renders at 1-4 screen pixels, WC3's
  // hand-baked diagonal alpha-variant slots (mask 6 = BL+TR, mask 9 =
  // TL+BR) blur into smooth frames instead of sharp "diamonds". Sub-tile
  // sampling is inset by `insetU/insetV` (5%) to keep neighbor-slot bleed
  // out of the sampled rect at the LOD levels that matter for those zooms.
  gl.bindTexture(gl.TEXTURE_2D, atlas)
  gl.generateMipmap(gl.TEXTURE_2D)
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR_MIPMAP_LINEAR)
  terrainDiag.atlasStatus = 'ok'
  terrainDiag.lastPalettes = numPalettes
  terrainDiag.lastAtlasW = atlasW
  terrainDiag.lastAtlasH = atlasH
  flog(`[terrain atlas] built ${atlasW}×${atlasH} (data ${atlasH_data}, padded=${padded}); mipmaps on, MIN=LINEAR_MIPMAP_LINEAR`)

  return { atlas, atlasW, atlasH, rows }
}

export interface TerrainMesh {
  draw(viewProj: Float32Array): void
  /**
   * Re-mesh the grid from a new TerrainDTO in place, re-uploading the geometry
   * buffers but REUSING the baked palette atlas + shader program. Use for live
   * brush edits (paint / height / cliff) where the palette is unchanged — it
   * skips the render-to-texture atlas bake that otherwise flickers per dab. If
   * the palette FourCCs actually change (tileset swap), build a fresh mesh
   * instead so the atlas matches.
   */
  update(t: TerrainDTO): void
  dispose(): void
  drawCallCount: number
}

export async function buildTerrain(
  gl: WebGLRenderingContext,
  viewer: any,
  pathSolver: any,
  t: TerrainDTO,
): Promise<TerrainMesh | null> {
  if (!t || !t.width || !t.height || !t.heights?.length) return null

  const W = t.width
  const H = t.height
  const N = W * H
  if (t.heights.length !== N) {
    flog(`[terrain] dim mismatch: W*H=${N}, heights=${t.heights.length}`)
    return null
  }

  const numCells = (W - 1) * (H - 1)
  if (numCells === 0) {
    flog(`[terrain] no cells: ${W}×${H}`)
    return null
  }

  // ---- Load palette textures (parallel) ----
  type PaletteTex = {
    glTex: WebGLTexture | null
    extended: boolean
    width: number
    height: number
    fallback: [number, number, number]
  }
  const paletteTextures: PaletteTex[] = new Array(t.palette.length)
  const palettePromises: Promise<void>[] = []
  for (let p = 0; p < t.palette.length; p++) {
    const fb = t.palette_colors?.[p]
    const fallback: [number, number, number] = fb
      ? [fb[0] / 255, fb[1] / 255, fb[2] / 255]
      : [0.4, 0.4, 0.4]
    paletteTextures[p] = { glTex: null, extended: false, width: 256, height: 256, fallback }
    const stem = t.palette_textures?.[p]
    if (!stem) {
      flog(`[terrain] palette[${p}] (${t.palette[p]}) has no Terrain.slk path; using fallback color`)
      continue
    }
    const path = stem + '.dds'
    palettePromises.push(
      viewer.load(path, pathSolver).then((res: any) => {
        if (!res || !res.webglResource) {
          terrainDiag.paletteFails++
          flogWarn(`[terrain] palette[${p}] (${t.palette[p]}) failed to load: ${path}`)
          return
        }
        const w = res.width as number
        const h = res.height as number
        const extended = w >= 2 * h
        paletteTextures[p] = {
          glTex: res.webglResource as WebGLTexture,
          extended, width: w, height: h, fallback,
        }
        flog(`[terrain] palette[${p}] (${t.palette[p]}) loaded ${w}×${h} extended=${extended} from ${path}`)
      }).catch((e: unknown) => {
        terrainDiag.paletteFails++
        flogWarn(`[terrain] palette[${p}] (${t.palette[p]}) load threw:`,
          e instanceof Error ? e.message : String(e))
      }),
    )
  }
  await Promise.all(palettePromises)

  // ---- Composite atlas ----
  const atlasInfo = buildAtlas(gl, paletteTextures)
  if (!atlasInfo) {
    // buildAtlas already set atlasStatus to 'fbo-incomplete' on the FBO path;
    // mark the generic failure otherwise so the overlay shows SOMETHING went
    // wrong rather than a stale 'ok' from a prior map.
    if (terrainDiag.atlasStatus !== 'fbo-incomplete') terrainDiag.atlasStatus = 'failed'
    flogWarn('[terrain] atlas build failed; aborting')
    return null
  }
  const { atlas, atlasW, atlasH, rows } = atlasInfo
  const numPalettes = rows.length

  // ---- Pre-compute per-palette atlas mapping helpers ----
  //
  // For palette p, sub-tile slot k, the sub-tile origin (bottom-left in
  // atlas UV, v-up) is (u0, v0). All sub-tiles in the atlas are 64×64 pixels
  // regardless of whether the source palette is extended (256×256, 4×4 grid)
  // or extended (512×256, 8×4 grid). Sub-tile size in atlas UV:
  //   subW = 64 / atlasW   (= 0.25 when atlasW=256, 0.125 when atlasW=512)
  //   subH = 64 / atlasH   (= 0.25/numPalettes etc.)
  //
  // Slot layout within a row:
  //   non-extended (4×4):  col c = slot % 4, row r = slot / 4   (slot 0..15)
  //   extended    (8×4):   col c = slot % 8, row r = slot / 8   (slot 0..31)
  //
  // Row image-y-down: r=0 is the top of the sub-tile grid; r=3 is the
  // bottom. We blit each palette so that the source image's TOP appears at
  // the destination row's TOP (in atlas-pixel-y, "top" = higher pixel-y =
  // higher atlas-UV v in v-up GL convention). So source sub-tile row r in
  // image-y-down maps to atlas pixel-y range
  //   [GL_top - (r+1)*64, GL_top - r*64)
  // where GL_top = atlasH - p*256 (the row's upper edge in atlas pixel-y).
  //
  // The bottom-left of sub-tile (c, r) in atlas pixel coords:
  //   xPx = c * 64
  //   yPx = GL_top - (r + 1) * 64 = atlasH - p*256 - (r+1)*64
  // → atlas UV (v-up):
  //   u0 = xPx / atlasW
  //   v0 = yPx / atlasH
  const subW = 64 / atlasW
  const subH = 64 / atlasH
  // Inset the sampling rect by 5% of a sub-tile on every side to absorb mip-
  // level bleed from neighbor slots. With sub-tile = 64 px, 5% = ~3.2 atlas
  // pixels of margin — enough to keep LOD-1 (2 px) and LOD-2 (4 px) sampling
  // strictly inside the slot's own pixels. Higher LODs (LOD-3 = 8 px, LOD-4
  // = 16 px) will bleed across slots and palette-row boundaries; that is the
  // SAME blur HiveWE applies via its mipmap chain, so the cross-bleed at
  // those LODs is what produces the smooth far-zoom appearance instead of
  // sharp transitions. Was 0.01 (= 0.64 px) before this commit, which was
  // tight enough for LINEAR (no mipmaps) but leaked at LOD-1+ once mipmaps
  // were enabled.
  const insetU = subW * 0.05
  const insetV = subH * 0.05
  // Slot → (sub-tile column, row) in the SOURCE image, per HiveWE's
  // GroundTexture atlas layout (ground_texture.ixx). Non-extended (4×4 grid,
  // 256×256 image): slot ∈ 0..15, c = slot%4, r = slot/4. Extended (slots 0..31
  // in a 512×256 image laid out as two horizontal halves of 4×4 each): slots
  // 0..15 live in the LEFT half (image cols 0..3); slots 16..31 in the RIGHT
  // half (image cols 4..7). Both halves keep the same row layout (slot &= 15
  // for the row within each half).
  //
  //   slot 0..15   → c = slot % 4,        r = slot / 4
  //   slot 16..31  → c = (slot-16) % 4 + 4, r = (slot-16) / 4
  //
  // The v-mapping here matches the existing (pre-refactor) terrain.ts that the
  // alpha-weighted-bilinear renderer used and confirmed visually correct: the
  // FBO blit stores image-row-0 at the LOWER atlas-v end of each palette's
  // strip (i.e. row r maps to atlas-v = baseV + r*subH, growing with r), and
  // the fragment shader compensates with (1 - cellUV.y) * subSize.y so that
  // cellUV.y=1 (cell TL) samples the image's TOP of the sub-tile.
  function subTileAtlasOrigin(p: number, slot: number): { u0: number; v0: number } {
    const half = slot >> 4              // 0 for slots 0..15, 1 for 16..31
    const k = slot & 0xF                // slot index within the half (0..15)
    const sc = (k & 0x3) + half * 4
    const sr = k >> 2
    const xPx = sc * 64
    const baseV = (atlasH - (p + 1) * 256) / atlasH
    const v0 = baseV + sr * (64 / atlasH)
    return { u0: xPx / atlasW + insetU, v0: v0 + insetV }
  }
  // The shader's effective sub-tile sampling rect is `[u0, u0 + subWEff]` ×
  // `[v0, v0 + subHEff]` after the insets. Pass the inset size to the shader
  // as the uniform.
  const subWEff = subW - 2 * insetU
  const subHEff = subH - 2 * insetV
  // Sentinel: a layer that doesn't exist for this cell gets atlas-U = -1.
  // The shader compares against -0.5 to skip the sample.
  const SENTINEL_U = -1.0
  const SENTINEL_V = -1.0

  // ---- Pre-compute per-corner normals from neighboring heights ----
  //
  // Port of HiveWE terrain.vert:
  //   normal = normalize(vec3(hL - hR, hD - hU, 2.0))
  // where hL/hR/hD/hU are heights of the 4 cardinal neighbors. HiveWE
  // does this in the vertex shader from an SSBO of CORNER_HEIGHT (the
  // SMOOTH height without the cliff-layer step), so cliff steps don't
  // make the lighting fight itself. We don't have a smooth-only buffer
  // — t.heights is FinalZ which includes the cliff step. Using FinalZ
  // for lighting produces sensible results except at exactly-on-cliff
  // corners (where the normal points sideways and the cell darkens),
  // but those cells are exactly the ones we SKIP in the cell_skip pass.
  // So in practice the visible cells all have neighbor pairs at the
  // same layer-height and the normal computation is well-behaved.
  //
  // Z slope baked-in factor: HiveWE's "2.0" in the Z slot represents
  // 2 tile-widths (left-to-right = 2 tiles apart) without dividing the
  // dh by 2*tile. In HiveWE units 1 tile = 1, in our units 1 tile = 128
  // studs. To keep the slope-to-height ratio the same as HiveWE:
  //   normal = normalize((hL - hR) / 256, (hD - hU) / 256, 1.0)
  // which simplifies to
  //   normal = normalize(hL - hR, hD - hU, 256.0)
  // and after normalize gives identical direction to HiveWE's per-tile
  // formula. Verified: flat ground (all heights equal) yields (0,0,1).

  // STRIDE_F (floats per vertex) lives in the outer scope because draw() reads
  // it for the attribute stride too.
  const STRIDE_F = 18
  // Geometry buffers are created ONCE and re-uploaded by rebuildGeometry on each
  // terrain edit. The palette atlas + shader program (built above) are reused,
  // so a brush dab re-meshes the grid WITHOUT re-baking the atlas. The bake is a
  // render-to-texture pass that binds an FBO and disables SCISSOR_TEST (relying
  // on "next frame" to restore it) — running it per-dab was the visible flicker.
  // triCount / indexType are set by rebuildGeometry and read live by draw().
  const vbo = gl.createBuffer()!
  const ibo = gl.createBuffer()!
  let triCount = 0
  let indexType: number = gl.UNSIGNED_SHORT

  // Rebuild the whole-map geometry from `t` and upload it into vbo/ibo. Reused
  // for the initial build and every in-place update(). Returns false (leaving
  // the prior buffers intact) when the map can't be meshed.
  function rebuildGeometry(t: TerrainDTO): boolean {
  const NORMAL_Z_SCALE = 256.0
  const normals = new Float32Array(N * 3) // per-corner (x,y,z)
  for (let j = 0; j < H; j++) {
    for (let i = 0; i < W; i++) {
      const idx = j * W + i
      const hL = t.heights[j * W + Math.max(i - 1, 0)]
      const hR = t.heights[j * W + Math.min(i + 1, W - 1)]
      const hD = t.heights[Math.max(j - 1, 0) * W + i]
      const hU = t.heights[Math.min(j + 1, H - 1) * W + i]
      const nx = hL - hR
      const ny = hD - hU
      const nz = NORMAL_Z_SCALE
      const len = Math.sqrt(nx * nx + ny * ny + nz * nz) || 1
      normals[idx * 3 + 0] = nx / len
      normals[idx * 3 + 1] = ny / len
      normals[idx * 3 + 2] = nz / len
    }
  }

  // ---- Pre-count cells we'll actually emit (cliff cells get skipped) ----
  //
  // Per HiveWE update_ground_exists: a cell is skipped when corner_cliff
  // is set AND it's not a ramp entrance (and not a special-doodad cell —
  // unimplemented here, low signal in test maps). The Go side computed
  // this into t.cell_skip already; we just have to honor it. Pre-counting
  // lets us size the typed arrays exactly so we don't carry sentinel
  // verts/indices that the GPU would skip anyway.
  const cellSkip = t.cell_skip || []
  let emittedCells = 0
  for (let k = 0; k < numCells; k++) {
    if (!cellSkip[k]) emittedCells++
  }
  if (emittedCells === 0) {
    flog(`[terrain] all ${numCells} cells skipped (entire map is cliff?) — aborting`)
    return false
  }
  flog(`[terrain] cell-skip: rendering ${emittedCells}/${numCells} cells (skipped ${numCells - emittedCells} cliff cells)`)

  // ---- Build whole-map mesh (single VBO/IBO) ----
  //
  // Per-vertex: pos(3) + shadowUV(2) + cellUV(2) + layerUV01(4) + layerUV23(4)
  //           + normal(3) = 18 floats.
  //
  // 4 verts per cell × emittedCells cells. uint32 indices when vCount > 65535
  // (needs OES_element_index_uint, ships universally in modern browsers).
  const uintExt = gl.getExtension('OES_element_index_uint')
  const has32Bit = !!uintExt

  const vCount = emittedCells * 4
  const need32 = vCount > 65535
  if (need32 && !has32Bit) {
    flog(`[terrain] needs uint32 indices (${vCount} verts) but OES_element_index_uint missing`)
    return false
  }
  const verts = new Float32Array(vCount * STRIDE_F)
  const indices = need32
    ? new Uint32Array(emittedCells * 6)
    : new Uint16Array(emittedCells * 6)

  const cx = t.center_offset[0]
  const cy = t.center_offset[1]
  const invWm1 = 1 / Math.max(1, W - 1)
  const invHm1 = 1 / Math.max(1, H - 1)

  let vOff = 0
  let iOff = 0
  let baseVert = 0

  // Scratch arrays for the per-cell unique-palette sort. Reused across all
  // cells to avoid GC pressure (4096 cells × 4 entries = 16k allocs/build
  // otherwise).
  const cellPals = new Int32Array(4)

  for (let j = 0; j < H - 1; j++) {
    for (let i = 0; i < W - 1; i++) {
      const cellIdx = j * (W - 1) + i
      if (cellSkip[cellIdx]) continue
      const bl = j * W + i
      const br = bl + 1
      const tl = bl + W
      const tr = tl + 1

      const xL = cx + i * 128
      const xR = cx + (i + 1) * 128
      const yB = cy + j * 128
      const yT = cy + (j + 1) * 128

      // 4 corners' palette indices, clamped to the loaded palette range.
      const pBL = Math.min(t.ground_tex[bl] | 0, numPalettes - 1)
      const pBR = Math.min(t.ground_tex[br] | 0, numPalettes - 1)
      const pTR = Math.min(t.ground_tex[tr] | 0, numPalettes - 1)
      const pTL = Math.min(t.ground_tex[tl] | 0, numPalettes - 1)

      // HiveWE convention (terrain.ixx update_ground_textures): 4-corner
      // mask uses bits assigned to {bottom_right=0, bottom_left=1,
      // top_right=2, top_left=3}. We collect them in the same order so the
      // mask matches what the texture's alpha-edged sub-tile slots expect.
      cellPals[0] = pBR
      cellPals[1] = pBL
      cellPals[2] = pTR
      cellPals[3] = pTL

      // Sort + dedupe. With only 4 entries, an in-place insertion sort is
      // faster than calling Array.prototype.sort (it's also reusable across
      // an Int32Array). After sort, walk the sorted array writing unique
      // values to the front.
      // Tiny insertion sort (n=4):
      for (let a = 1; a < 4; a++) {
        const v = cellPals[a]
        let b = a - 1
        while (b >= 0 && cellPals[b] > v) {
          cellPals[b + 1] = cellPals[b]
          b--
        }
        cellPals[b + 1] = v
      }
      // Inline dedupe — count unique values; the 4 cellPals stay accessible
      // for the per-layer mask computation that follows.
      // u0 = sorted unique values
      let u0 = cellPals[0]
      let u1 = -1, u2 = -1, u3 = -1
      let nUnique = 1
      if (cellPals[1] !== u0) { u1 = cellPals[1]; nUnique = 2 }
      const next1 = cellPals[2]
      if (nUnique === 1) {
        if (next1 !== u0) { u1 = next1; nUnique = 2 }
      } else {
        if (next1 !== u1) { u2 = next1; nUnique = 3 }
      }
      const next2 = cellPals[3]
      if (nUnique === 1) {
        if (next2 !== u0) { u1 = next2; nUnique = 2 }
      } else if (nUnique === 2) {
        if (next2 !== u1) { u2 = next2; nUnique = 3 }
      } else {
        if (next2 !== u2) { u3 = next2; nUnique = 4 }
      }

      // ---- Layer 0 (base): variation-driven slot of u0 ----
      // HiveWE: out.x = u[0] | get_tile_variation(u[0], variation_at_cell_origin) << 16
      // The "variation_at_cell_origin" is the ground_var of the BL corner
      // (cell origin in HiveWE's `ci(tx, ty)` indexing — see
      // `corner_ground_variation[ci(tx, ty)]` in terrain.ixx update_ground_textures).
      const vBase = t.ground_var[bl] & 0x1F
      const slotBase = pickSubTile(rows[u0].extended, vBase)
      const orig0 = subTileAtlasOrigin(u0, slotBase)
      const l0u = orig0.u0, l0v = orig0.v0

      // ---- Layers 1..3: higher-priority palettes, slot = corner mask ----
      // The mask bits are { bottom_right=0, bottom_left=1, top_right=2,
      // top_left=3 } — same as HiveWE.
      let l1u = SENTINEL_U, l1v = SENTINEL_V
      let l2u = SENTINEL_U, l2v = SENTINEL_V
      let l3u = SENTINEL_U, l3v = SENTINEL_V

      if (nUnique >= 2) {
        let m = 0
        if (pBR === u1) m |= 0x1
        if (pBL === u1) m |= 0x2
        if (pTR === u1) m |= 0x4
        if (pTL === u1) m |= 0x8
        // Mask 0..15 IS the alpha-edged sub-tile slot.
        const o = subTileAtlasOrigin(u1, m)
        l1u = o.u0; l1v = o.v0
      }
      if (nUnique >= 3) {
        let m = 0
        if (pBR === u2) m |= 0x1
        if (pBL === u2) m |= 0x2
        if (pTR === u2) m |= 0x4
        if (pTL === u2) m |= 0x8
        const o = subTileAtlasOrigin(u2, m)
        l2u = o.u0; l2v = o.v0
      }
      if (nUnique >= 4) {
        let m = 0
        if (pBR === u3) m |= 0x1
        if (pBL === u3) m |= 0x2
        if (pTR === u3) m |= 0x4
        if (pTL === u3) m |= 0x8
        const o = subTileAtlasOrigin(u3, m)
        l3u = o.u0; l3v = o.v0
      }

      // Shadow UVs.
      const sBLu = i * invWm1, sBLv = j * invHm1
      const sBRu = (i + 1) * invWm1, sBRv = sBLv
      const sTLu = sBLu, sTLv = (j + 1) * invHm1
      const sTRu = sBRu, sTRv = sTLv

      // Per-cell layer attributes are the same across all 4 verts.
      // Vert layout helper: write one vertex's STRIDE_F floats.
      // cellUV per vertex: bl=(0,0), br=(1,0), tl=(0,1), tr=(1,1).
      // Normal is per-corner (precomputed in `normals` from neighbor heights).
      function writeVert(
        x: number, y: number, z: number,
        cellU: number, cellV: number,
        shU: number, shV: number,
        cornerIdx: number,
      ) {
        verts[vOff++] = x; verts[vOff++] = y; verts[vOff++] = z
        verts[vOff++] = shU; verts[vOff++] = shV
        verts[vOff++] = cellU; verts[vOff++] = cellV
        verts[vOff++] = l0u; verts[vOff++] = l0v; verts[vOff++] = l1u; verts[vOff++] = l1v
        verts[vOff++] = l2u; verts[vOff++] = l2v; verts[vOff++] = l3u; verts[vOff++] = l3v
        verts[vOff++] = normals[cornerIdx * 3 + 0]
        verts[vOff++] = normals[cornerIdx * 3 + 1]
        verts[vOff++] = normals[cornerIdx * 3 + 2]
      }

      writeVert(xL, yB, t.heights[bl], 0, 0, sBLu, sBLv, bl) // BL
      writeVert(xR, yB, t.heights[br], 1, 0, sBRu, sBRv, br) // BR
      writeVert(xL, yT, t.heights[tl], 0, 1, sTLu, sTLv, tl) // TL
      writeVert(xR, yT, t.heights[tr], 1, 1, sTRu, sTRv, tr) // TR

      indices[iOff++] = baseVert + 0
      indices[iOff++] = baseVert + 1
      indices[iOff++] = baseVert + 3
      indices[iOff++] = baseVert + 0
      indices[iOff++] = baseVert + 3
      indices[iOff++] = baseVert + 2
      baseVert += 4
    }
  }

  gl.bindBuffer(gl.ARRAY_BUFFER, vbo)
  gl.bufferData(gl.ARRAY_BUFFER, verts, gl.STATIC_DRAW)
  gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, ibo)
  gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, indices, gl.STATIC_DRAW)
  triCount = emittedCells * 2
  indexType = need32 ? gl.UNSIGNED_INT : gl.UNSIGNED_SHORT
  return true
  } // end rebuildGeometry

  // Initial geometry upload. A false here means the map can't be meshed.
  if (!rebuildGeometry(t)) {
    gl.deleteBuffer(vbo)
    gl.deleteBuffer(ibo)
    gl.deleteTexture(atlas)
    return null
  }

  const prog = buildProgram(gl)
  const STRIDE_B = STRIDE_F * 4

  // HiveWE map.ixx: light_direction = normalize(vec3(1, 1, -3)). Pre-computed
  // so the per-frame draw doesn't redo the sqrt.
  const lightDir = (() => {
    const x = 1, y = 1, z = -3
    const len = Math.sqrt(x * x + y * y + z * z)
    return new Float32Array([x / len, y / len, z / len])
  })()

  // war3map.shd is the baked doodad-cast shadow map. HiveWE skips rendering it;
  // we follow suit because at game-scale viewport zooms the shadows appear as
  // floating cell-aligned dark blocks without their visible casters.
  const shadowTex: WebGLTexture | null = null

  flog(`[terrain] built layered mesh: ${numCells} cells, ${numPalettes} palettes, atlas ${atlasW}×${atlasH}, subSize=(${subWEff.toFixed(4)}, ${subHEff.toFixed(4)})`)

  return {
    drawCallCount: 1,
    update(t: TerrainDTO) {
      // Geometry-only re-upload; atlas + program stay put. No-op-safe: a false
      // return leaves the prior buffers (and the last good frame) intact.
      rebuildGeometry(t)
    },
    draw(viewProj: Float32Array) {
      gl.useProgram(prog.program)
      gl.uniformMatrix4fv(prog.uViewProj, false, viewProj)
      gl.uniform2f(prog.uSubSize, subWEff, subHEff)
      gl.uniform3fv(prog.uLightDir, lightDir)

      gl.activeTexture(gl.TEXTURE0)
      gl.bindTexture(gl.TEXTURE_2D, atlas)
      gl.uniform1i(prog.uAtlas, 0)

      if (shadowTex) {
        gl.activeTexture(gl.TEXTURE1)
        gl.bindTexture(gl.TEXTURE_2D, shadowTex)
        gl.uniform1i(prog.uShadowTex, 1)
        gl.uniform1i(prog.uHasShadow, 1)
      } else {
        gl.uniform1i(prog.uHasShadow, 0)
      }

      gl.enable(gl.DEPTH_TEST)
      gl.depthFunc(gl.LEQUAL)
      gl.depthMask(true)
      gl.disable(gl.BLEND)
      gl.disable(gl.CULL_FACE)

      gl.bindBuffer(gl.ARRAY_BUFFER, vbo)
      gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, ibo)

      let off = 0
      function setAttr(loc: number, size: number) {
        if (loc >= 0) {
          gl.enableVertexAttribArray(loc)
          gl.vertexAttribPointer(loc, size, gl.FLOAT, false, STRIDE_B, off)
        }
        off += size * 4
      }
      setAttr(prog.aPosition, 3)
      setAttr(prog.aShadowUV, 2)
      setAttr(prog.aCellUV, 2)
      setAttr(prog.aLayerUV01, 4)
      setAttr(prog.aLayerUV23, 4)
      setAttr(prog.aNormal, 3)

      gl.drawElements(gl.TRIANGLES, triCount * 3, indexType, 0)

      if (prog.aPosition >= 0) gl.disableVertexAttribArray(prog.aPosition)
      if (prog.aShadowUV >= 0) gl.disableVertexAttribArray(prog.aShadowUV)
      if (prog.aCellUV >= 0) gl.disableVertexAttribArray(prog.aCellUV)
      if (prog.aLayerUV01 >= 0) gl.disableVertexAttribArray(prog.aLayerUV01)
      if (prog.aLayerUV23 >= 0) gl.disableVertexAttribArray(prog.aLayerUV23)
      if (prog.aNormal >= 0) gl.disableVertexAttribArray(prog.aNormal)
    },
    dispose() {
      gl.deleteBuffer(vbo)
      gl.deleteBuffer(ibo)
      gl.deleteTexture(atlas)
      if (shadowTex) gl.deleteTexture(shadowTex)
      gl.deleteProgram(prog.program)
    },
  }
}
