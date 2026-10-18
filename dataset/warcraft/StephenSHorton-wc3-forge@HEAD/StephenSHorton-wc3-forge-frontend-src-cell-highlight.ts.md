---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/cell-highlight.ts"
ref: HEAD
---

// cell-highlight.ts — yellow wireframe quad overlay for the currently-picked
// terrain cell. Draws a 4-corner line strip at the cell's per-corner Z
// values, rendered AFTER the main viewport pass so it sits on top of
// terrain/cliffs/units regardless of depth.
//
// Why a separate module: keeps the highlight isolated from terrain.ts (which
// is owned by a parallel cliff-height agent's work) and mirrors the
// minimal-shader-overlay pattern from sloc-markers.ts.
//
// Lifecycle: buildCellHighlight(gl) once per scene. setCell({col,row}) on
// pick; setCell(null) to hide. draw(viewProj) every frame. dispose() on
// teardown.
//
// Coordinate model: cell (col, row) spans 4 grid vertices:
//   BL = (col,   row  ) at world XY (centerOffset[0] + col*128, centerOffset[1] + row*128)
//   BR = (col+1, row  )
//   TR = (col+1, row+1)
//   TL = (col,   row+1)
// Each corner's Z comes from the TerrainDTO.heights array (already includes
// the layer_height contribution — see app.go TerrainDTO comments).

import { flog } from './debuglog'

const VERT_SHADER = `
attribute vec3 a_position;
uniform mat4 u_viewProj;
void main() {
  gl_Position = u_viewProj * vec4(a_position, 1.0);
}
`.trim()

const FRAG_SHADER = `
precision mediump float;
uniform vec3 u_color;
uniform float u_alpha;
void main() {
  gl_FragColor = vec4(u_color, u_alpha);
}
`.trim()

function compileShader(gl: WebGLRenderingContext, type: number, source: string): WebGLShader {
  const sh = gl.createShader(type)!
  gl.shaderSource(sh, source)
  gl.compileShader(sh)
  if (!gl.getShaderParameter(sh, gl.COMPILE_STATUS)) {
    const log = gl.getShaderInfoLog(sh)
    gl.deleteShader(sh)
    throw new Error('cell-highlight shader compile: ' + log)
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
    throw new Error('cell-highlight program link: ' + log)
  }
  return {
    program,
    aPosition: gl.getAttribLocation(program, 'a_position'),
    uViewProj: gl.getUniformLocation(program, 'u_viewProj')!,
    uColor: gl.getUniformLocation(program, 'u_color')!,
    uAlpha: gl.getUniformLocation(program, 'u_alpha')!,
  }
}

// Minimum TerrainDTO subset we read corner positions from. Stays in lockstep
// with main.TerrainDTO json tags (`width`, `center_offset`, `heights`).
export interface TerrainDataForHighlight {
  width: number
  height: number
  center_offset: [number, number]
  heights: number[] | Float32Array
}

export interface CellRef {
  col: number
  row: number
}

export interface CellHighlight {
  /** Set the highlighted cell (or null to hide). col/row = BL-corner indices. */
  setCell(cell: CellRef | null): void
  /** Update terrain data (called on map open). */
  setTerrain(t: TerrainDataForHighlight | null): void
  /** Draw the highlight (no-op when no cell or no terrain). */
  draw(viewProj: Float32Array): void
  /** Release GL resources. */
  dispose(): void
}

// Slight Z-lift so the wireframe doesn't z-fight with the underlying terrain
// quad. Tiny constant tuned for "consistently on top at any pitch / zoom"
// while not creating a visible floating gap.
const Z_LIFT = 4

export function buildCellHighlight(gl: WebGLRenderingContext): CellHighlight | null {
  let prog: ReturnType<typeof buildProgram>
  try {
    prog = buildProgram(gl)
  } catch (e) {
    flog('[cell-highlight] program build failed:', e instanceof Error ? e.message : String(e))
    return null
  }

  // Dynamic VBO — 4 corner positions, updated whenever setCell+setTerrain
  // changes which cell is highlighted. Uploaded via STREAM_DRAW so the GL
  // implementation can optimize for "small buffer, frequent rewrites".
  const vbo = gl.createBuffer()!
  // Pre-allocate 4 verts × 3 floats × 4 bytes = 48 bytes.
  gl.bindBuffer(gl.ARRAY_BUFFER, vbo)
  gl.bufferData(gl.ARRAY_BUFFER, 4 * 3 * 4, gl.STREAM_DRAW)

  // LINE_LOOP would walk 0→1→2→3→0 naturally, but in practice WebGL impls
  // can have spotty support / non-uniform widths for LINE_LOOP. Drawing as
  // 4 LINES via an index buffer is portable and dependable.
  const ibo = gl.createBuffer()!
  gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, ibo)
  gl.bufferData(gl.ELEMENT_ARRAY_BUFFER,
    new Uint16Array([0, 1, 1, 2, 2, 3, 3, 0]),
    gl.STATIC_DRAW)

  let terrain: TerrainDataForHighlight | null = null
  let cell: CellRef | null = null
  let dirty = false

  function rebuildVertices() {
    if (!cell || !terrain) return
    const W = terrain.width
    const H = terrain.height
    const c = cell.col
    const r = cell.row
    // Guard against stale (col,row) referring to a previous map's grid.
    if (c < 0 || r < 0 || c > W - 2 || r > H - 2) return
    const cx = terrain.center_offset[0]
    const cy = terrain.center_offset[1]
    const heights = terrain.heights
    const bl = r * W + c
    const br = bl + 1
    const tl = bl + W
    const tr = tl + 1
    const x0 = cx + c * 128
    const x1 = cx + (c + 1) * 128
    const y0 = cy + r * 128
    const y1 = cy + (r + 1) * 128
    const zBL = Number(heights[bl] ?? 0) + Z_LIFT
    const zBR = Number(heights[br] ?? 0) + Z_LIFT
    const zTR = Number(heights[tr] ?? 0) + Z_LIFT
    const zTL = Number(heights[tl] ?? 0) + Z_LIFT
    // Order: BL → BR → TR → TL (matches the line index buffer).
    const verts = new Float32Array([
      x0, y0, zBL,
      x1, y0, zBR,
      x1, y1, zTR,
      x0, y1, zTL,
    ])
    gl.bindBuffer(gl.ARRAY_BUFFER, vbo)
    gl.bufferSubData(gl.ARRAY_BUFFER, 0, verts)
    dirty = false
  }

  return {
    setCell(c: CellRef | null) {
      cell = c ? { col: c.col, row: c.row } : null
      dirty = true
    },
    setTerrain(t: TerrainDataForHighlight | null) {
      terrain = t
      dirty = true
    },
    draw(viewProj: Float32Array) {
      if (!cell || !terrain) return
      if (dirty) rebuildVertices()
      // Re-establish every bit of state we touch — the lib's render path
      // leaves attribs/programs in unknown states (same playbook as
      // sloc-markers).
      gl.useProgram(prog.program)
      gl.bindBuffer(gl.ARRAY_BUFFER, vbo)
      gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, ibo)
      const maxAttribs = gl.getParameter(gl.MAX_VERTEX_ATTRIBS) | 0
      for (let i = 0; i < maxAttribs; i++) gl.disableVertexAttribArray(i)
      gl.enableVertexAttribArray(prog.aPosition)
      gl.vertexAttribPointer(prog.aPosition, 3, gl.FLOAT, false, 12, 0)
      gl.uniformMatrix4fv(prog.uViewProj, false, viewProj)
      // Bright yellow, fully opaque. Alpha kept as a uniform in case future
      // work wants to pulse it for "I see you" emphasis.
      gl.uniform3f(prog.uColor, 1.0, 0.95, 0.15)
      gl.uniform1f(prog.uAlpha, 1.0)
      // Always-on-top: disable depth-test so the outline shows through any
      // cliff/unit that visually occludes the cell. Per project memory, this
      // is the pattern the user expects for "highlight the picked cell" —
      // not a faintly-buried wireframe but a confident marker.
      gl.disable(gl.DEPTH_TEST)
      gl.depthMask(false)
      gl.disable(gl.CULL_FACE)
      gl.disable(gl.BLEND)
      // WebGL's max line width is implementation-defined (most drivers clamp
      // to 1.0). Request the widest the driver supports without breaking on
      // platforms that ignore it.
      gl.lineWidth(2)
      gl.drawElements(gl.LINES, 8, gl.UNSIGNED_SHORT, 0)
      gl.disableVertexAttribArray(prog.aPosition)
      // Restore depth-test for the next pass.
      gl.enable(gl.DEPTH_TEST)
      gl.depthMask(true)
    },
    dispose() {
      gl.deleteBuffer(vbo)
      gl.deleteBuffer(ibo)
      gl.deleteProgram(prog.program)
      cell = null
      terrain = null
    },
  }
}
