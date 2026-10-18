---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/pathing.ts"
ref: HEAD
---

// Pathing-map overlay. Renders war3map.wpm as a semi-transparent colored
// grid sitting just above the terrain. Toggled on/off by the header pill;
// off by default.
//
// Per-cell flag bits (HiveWE convention, see internal/formats/wpm/wpm.go):
//   0x02 unwalkable  → red
//   0x04 unflyable   → blue
//   0x08 unbuildable → yellow
// Cells may carry multiple flags; bits are decoded independently and the
// tints additively blend, capped at full intensity.
//
// Resolution: the pathing map is 4× the terrain tile count along each axis
// (terrain tile = 128 studs → pathing cell = 32 studs). We do NOT tessellate
// the overlay mesh at pathing resolution though — we tessellate at TERRAIN
// resolution so the overlay tracks terrain elevation, and let the fragment
// shader sample the byte texture per-fragment with NEAREST filtering. This
// keeps the cell boundaries crisp while still following hills and ramps.
//
// Z bias: vertex shader lifts each vertex +2 studs above terrain to avoid
// z-fighting against the terrain mesh. Combined with depth-test ON +
// depth-write OFF, the overlay correctly occludes when behind cliffs while
// passing depth correctly over flat terrain.

import { flog } from './debuglog'

interface PathingDTO {
  width: number
  height: number
  cells: number[] // length = width*height; one byte per cell as uint32
}

// Mirrors the JS-side shape of App.GetTerrain — we only need the vertex
// heights + grid dims + center offset to build the follow-terrain mesh.
interface TerrainShape {
  width: number       // vertex count along X
  height: number      // vertex count along Y
  center_offset: [number, number]
  heights: number[]   // length = width*height, in studs (already FinalZ)
}

// Z bias to lift the overlay above terrain. Small enough that it doesn't
// look visibly floating from a normal RTS camera angle (~60° pitch); large
// enough to beat depth-precision wobble from the view matrix.
const Z_BIAS = 2.0

// Tints applied per-flag. Alpha is per-flag; the additive blend caps the
// composite alpha at 0.85 so even all-three-bits cells stay translucent
// (you should still be able to see the terrain underneath).
//
// Color picks roughly match HiveWE's pathing-brush palette: solid red for
// unwalkable, saturated blue for unflyable, warm yellow for unbuildable.
const VERT_SHADER = `
attribute vec3 a_position;
attribute vec2 a_pathingUV;
uniform mat4 u_viewProj;
uniform float u_zBias;
varying vec2 v_pathingUV;
void main() {
  v_pathingUV = a_pathingUV;
  vec3 pos = vec3(a_position.x, a_position.y, a_position.z + u_zBias);
  gl_Position = u_viewProj * vec4(pos, 1.0);
}
`.trim()

// GLSL ES 1.00 has no bitwise ops on uniforms/varyings; decompose the byte
// value via integer division (floor + mod). Texture returns 0..1; * 255
// recovers the raw cell byte at NEAREST-filtered fragments.
//
// We discard cells with zero flags entirely so the overlay doesn't darken
// blank quarter-cells (depth write is off, but discard avoids the alpha-
// blend cost on dense empty regions like the playable inside of an arena).
const FRAG_SHADER = `
precision highp float;
uniform sampler2D u_pathing;
varying vec2 v_pathingUV;
void main() {
  // Highp + +0.5 rounding nudge so mediump-grade hardware doesn't lose the
  // bit boundary at v ≈ 0xFE/0xFF. Texture sample is single-channel byte;
  // LUMINANCE/UNSIGNED_BYTE returns values quantized to k/255 for integer k.
  float v = floor(texture2D(u_pathing, v_pathingUV).r * 255.0 + 0.5);
  float bUnwalkable  = mod(floor(v / 2.0), 2.0);
  float bUnflyable   = mod(floor(v / 4.0), 2.0);
  float bUnbuildable = mod(floor(v / 8.0), 2.0);
  vec4 c = vec4(0.0);
  if (bUnwalkable  > 0.5) c += vec4(1.00, 0.10, 0.10, 0.55);
  if (bUnflyable   > 0.5) c += vec4(0.10, 0.30, 1.00, 0.50);
  if (bUnbuildable > 0.5) c += vec4(1.00, 0.85, 0.10, 0.50);
  if (c.a < 0.01) discard;
  c.rgb = min(c.rgb, vec3(1.0));
  c.a   = min(c.a, 0.85);
  gl_FragColor = c;
}
`.trim()

function compileShader(gl: WebGLRenderingContext, type: number, src: string): WebGLShader {
  const sh = gl.createShader(type)!
  gl.shaderSource(sh, src)
  gl.compileShader(sh)
  if (!gl.getShaderParameter(sh, gl.COMPILE_STATUS)) {
    const log = gl.getShaderInfoLog(sh)
    gl.deleteShader(sh)
    throw new Error('pathing shader compile: ' + log)
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
    throw new Error('pathing program link: ' + log)
  }
  return {
    program,
    aPosition: gl.getAttribLocation(program, 'a_position'),
    aPathingUV: gl.getAttribLocation(program, 'a_pathingUV'),
    uViewProj: gl.getUniformLocation(program, 'u_viewProj')!,
    uPathing: gl.getUniformLocation(program, 'u_pathing')!,
    uZBias: gl.getUniformLocation(program, 'u_zBias')!,
  }
}

// Pack uint32 cells back into a Uint8Array for gl.texImage2D. We sent them
// as uint32 (4× the wire size) only to dodge encoding/json's []byte → base64
// special case. JS-side the cost is one tight loop.
function packCells(cells: number[]): Uint8Array {
  const out = new Uint8Array(cells.length)
  for (let i = 0; i < cells.length; i++) out[i] = cells[i] & 0xff
  return out
}

function uploadPathingTexture(
  gl: WebGLRenderingContext, w: number, h: number, bytes: Uint8Array,
): WebGLTexture | null {
  const tex = gl.createTexture()
  if (!tex) return null
  gl.bindTexture(gl.TEXTURE_2D, tex)
  gl.pixelStorei(gl.UNPACK_ALIGNMENT, 1)
  // LUMINANCE is the WebGL1-portable choice for single-channel byte data
  // (GL_RED needs WebGL2 or EXT_texture_rg). The shader reads .r, which
  // works the same on a LUMINANCE texture (luminance is replicated into
  // RGB; A is 1).
  gl.texImage2D(gl.TEXTURE_2D, 0, gl.LUMINANCE, w, h, 0, gl.LUMINANCE, gl.UNSIGNED_BYTE, bytes)
  // NEAREST keeps cell boundaries crisp — the brief calls this out
  // explicitly. LINEAR would smear the unwalkable/unflyable bit edges
  // across half-cells and the discard-when-zero path would alias.
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST)
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST)
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE)
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE)
  return tex
}

export interface PathingOverlay {
  draw(viewProj: Float32Array): void
  dispose(): void
}

export function buildPathingOverlay(
  gl: WebGLRenderingContext,
  pathing: PathingDTO,
  terrain: TerrainShape,
): PathingOverlay | null {
  if (!pathing || pathing.width <= 0 || pathing.height <= 0) return null
  if (!pathing.cells || pathing.cells.length !== pathing.width * pathing.height) {
    flog(`[pathing] cell-count mismatch: ${pathing.cells?.length} ≠ ${pathing.width}*${pathing.height}`)
    return null
  }
  if (!terrain || terrain.width < 2 || terrain.height < 2 || !terrain.heights?.length) {
    flog('[pathing] missing terrain shape — cannot tessellate overlay')
    return null
  }

  const tW = terrain.width
  const tH = terrain.height
  const N = tW * tH
  if (terrain.heights.length !== N) {
    flog(`[pathing] terrain.heights length ${terrain.heights.length} ≠ ${N}`)
    return null
  }

  // Pathing covers world XY [cx .. cx + pathingW*32] × [cy .. cy + pathingH*32].
  // For well-formed maps that equals [cx .. cx + (tW-1)*128] (since pathingW
  // = (tW-1)*4). If the wpm declares different dims we still anchor the
  // overlay at the terrain origin and scale UVs to the wpm dims — the
  // mismatch case shouldn't happen on real maps.
  const cx = terrain.center_offset[0]
  const cy = terrain.center_offset[1]
  const pW = pathing.width
  const pH = pathing.height
  // World extent the pathing texture spans. Pathing cell = 32 studs.
  const pathingSpanX = pW * 32
  const pathingSpanY = pH * 32

  // Per-vertex layout: pos(3) + pathingUV(2) = 5 floats.
  const STRIDE_F = 5
  const verts = new Float32Array(N * STRIDE_F)
  for (let j = 0; j < tH; j++) {
    for (let i = 0; i < tW; i++) {
      const k = j * tW + i
      const o = k * STRIDE_F
      const x = cx + i * 128
      const y = cy + j * 128
      verts[o] = x
      verts[o + 1] = y
      verts[o + 2] = terrain.heights[k]
      // UV is "fraction of pathing texture spanned by this vertex's world XY".
      // Half-texel inset is unnecessary with NEAREST + CLAMP_TO_EDGE.
      verts[o + 3] = (x - cx) / pathingSpanX
      verts[o + 4] = (y - cy) / pathingSpanY
    }
  }

  const numCells = (tW - 1) * (tH - 1)
  const need32 = N > 65535
  const uintExt = need32 ? gl.getExtension('OES_element_index_uint') : null
  if (need32 && !uintExt) {
    flog(`[pathing] needs uint32 indices (${N} verts) but OES_element_index_uint missing`)
    return null
  }
  const indices: Uint16Array | Uint32Array = need32
    ? new Uint32Array(numCells * 6)
    : new Uint16Array(numCells * 6)
  const indexType = need32 ? gl.UNSIGNED_INT : gl.UNSIGNED_SHORT
  let iOff = 0
  for (let j = 0; j < tH - 1; j++) {
    for (let i = 0; i < tW - 1; i++) {
      const bl = j * tW + i
      const br = bl + 1
      const tl = bl + tW
      const tr = tl + 1
      indices[iOff++] = bl
      indices[iOff++] = br
      indices[iOff++] = tr
      indices[iOff++] = bl
      indices[iOff++] = tr
      indices[iOff++] = tl
    }
  }

  const vbo = gl.createBuffer()!
  gl.bindBuffer(gl.ARRAY_BUFFER, vbo)
  gl.bufferData(gl.ARRAY_BUFFER, verts, gl.STATIC_DRAW)
  const ibo = gl.createBuffer()!
  gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, ibo)
  gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, indices, gl.STATIC_DRAW)

  const tex = uploadPathingTexture(gl, pW, pH, packCells(pathing.cells))
  if (!tex) {
    gl.deleteBuffer(vbo)
    gl.deleteBuffer(ibo)
    flog('[pathing] texture upload failed')
    return null
  }

  const prog = buildProgram(gl)
  const STRIDE_B = STRIDE_F * 4
  const idxCount = numCells * 6

  flog(`[pathing] built overlay: ${pW}×${pH} cells, ${numCells} terrain quads`)

  return {
    draw(viewProj: Float32Array) {
      gl.useProgram(prog.program)
      gl.uniformMatrix4fv(prog.uViewProj, false, viewProj)
      gl.uniform1f(prog.uZBias, Z_BIAS)

      gl.activeTexture(gl.TEXTURE0)
      gl.bindTexture(gl.TEXTURE_2D, tex)
      gl.uniform1i(prog.uPathing, 0)

      gl.bindBuffer(gl.ARRAY_BUFFER, vbo)
      gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, ibo)

      gl.enableVertexAttribArray(prog.aPosition)
      gl.vertexAttribPointer(prog.aPosition, 3, gl.FLOAT, false, STRIDE_B, 0)
      gl.enableVertexAttribArray(prog.aPathingUV)
      gl.vertexAttribPointer(prog.aPathingUV, 2, gl.FLOAT, false, STRIDE_B, 3 * 4)

      // Same translucent-overlay state as water.ts: depth-test on so cliffs
      // occlude the overlay, depth-write off so multiple translucent passes
      // don't fight, alpha blend on for the per-flag transparency.
      gl.enable(gl.DEPTH_TEST)
      gl.depthFunc(gl.LEQUAL)
      gl.depthMask(false)
      gl.enable(gl.BLEND)
      gl.blendFunc(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA)
      gl.disable(gl.CULL_FACE)

      gl.drawElements(gl.TRIANGLES, idxCount, indexType, 0)

      gl.depthMask(true)
      gl.disable(gl.BLEND)
      gl.disableVertexAttribArray(prog.aPosition)
      gl.disableVertexAttribArray(prog.aPathingUV)
    },
    dispose() {
      gl.deleteBuffer(vbo)
      gl.deleteBuffer(ibo)
      gl.deleteTexture(tex)
      gl.deleteProgram(prog.program)
    },
  }
}
