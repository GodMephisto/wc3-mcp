---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/region-overlay.ts"
ref: HEAD
---

// region-overlay.ts — flat rectangle outlines for the map's regions
// (war3map.w3r). Each region is drawn as a 4-corner LINE-loop on the z=0 ground
// plane (regions are 2D world-XY rects; WC3 stores no Z for them), rendered
// AFTER the main viewport pass so the outlines sit on top of terrain/units
// regardless of depth.
//
// Mirrors the minimal-shader-overlay pattern from cell-highlight.ts /
// sloc-markers.ts: build once per scene, setRegions(...) on data change,
// draw(viewProj) every frame, dispose() on teardown. The selected region (by
// creation_number) is drawn in a brighter highlight color.
//
// Coordinate model: region bounds are WC3 world coords (min_x/min_y = left/
// bottom, max_x/max_y = right/top), the SAME space the camera renders in
// (units/doodads store the same coords). No center-offset transform is needed —
// they're already world-space.

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
    throw new Error('region-overlay shader compile: ' + log)
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
    throw new Error('region-overlay program link: ' + log)
  }
  return {
    program,
    aPosition: gl.getAttribLocation(program, 'a_position'),
    uViewProj: gl.getUniformLocation(program, 'u_viewProj')!,
    uColor: gl.getUniformLocation(program, 'u_color')!,
    uAlpha: gl.getUniformLocation(program, 'u_alpha')!,
  }
}

// One region rectangle the overlay renders. min/max are WC3 world coords;
// creationNumber is the stable id used to flag the selected region. color is
// the region's stored [r,g,b] (0..255) — used as the outline color so the
// overlay reflects what the user set in the panel; it's brightened for the
// selected region.
export interface RegionRect {
  creationNumber: number
  minX: number
  minY: number
  maxX: number
  maxY: number
  color: [number, number, number]
}

export interface RegionOverlay {
  /** Replace the region list (called on map open + after any region edit). */
  setRegions(rs: RegionRect[]): void
  /** Set the highlighted region by creation_number (or null for none). */
  setSelected(cn: number | null): void
  /** Draw all region rectangles (no-op when none). */
  draw(viewProj: Float32Array): void
  /** Release GL resources. */
  dispose(): void
}

// Slight Z-lift so the outlines float just above the ground plane rather than
// z-fighting flat terrain. Matches cell-highlight's lift intent.
const Z_LIFT = 6

export function buildRegionOverlay(gl: WebGLRenderingContext): RegionOverlay | null {
  let prog: ReturnType<typeof buildProgram>
  try {
    prog = buildProgram(gl)
  } catch (e) {
    flog('[region-overlay] program build failed:', e instanceof Error ? e.message : String(e))
    return null
  }

  // Dynamic VBO holding 4 corner positions for ONE rect at a time — we draw
  // each region in its own draw call (region counts are tiny, typically < 100,
  // so per-rect draws are cheap and keep the buffer logic trivial). STREAM_DRAW
  // because we rewrite it once per region per frame.
  const vbo = gl.createBuffer()!
  gl.bindBuffer(gl.ARRAY_BUFFER, vbo)
  gl.bufferData(gl.ARRAY_BUFFER, 4 * 3 * 4, gl.STREAM_DRAW)

  // 4 LINES via an index buffer (BL→BR, BR→TR, TR→TL, TL→BL) — portable across
  // WebGL impls with spotty LINE_LOOP support (same rationale as cell-highlight).
  const ibo = gl.createBuffer()!
  gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, ibo)
  gl.bufferData(gl.ELEMENT_ARRAY_BUFFER,
    new Uint16Array([0, 1, 1, 2, 2, 3, 3, 0]),
    gl.STATIC_DRAW)

  let regions: RegionRect[] = []
  let selectedCN: number | null = null
  const scratch = new Float32Array(4 * 3)

  return {
    setRegions(rs: RegionRect[]) {
      regions = rs.map((r) => ({ ...r, color: [...r.color] as [number, number, number] }))
    },
    setSelected(cn: number | null) {
      selectedCN = cn
    },
    draw(viewProj: Float32Array) {
      if (regions.length === 0) return
      gl.useProgram(prog.program)
      gl.bindBuffer(gl.ARRAY_BUFFER, vbo)
      gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, ibo)
      // Re-establish every bit of state we touch — the lib's render path leaves
      // attribs/programs in unknown states (same playbook as cell-highlight).
      const maxAttribs = gl.getParameter(gl.MAX_VERTEX_ATTRIBS) | 0
      for (let i = 0; i < maxAttribs; i++) gl.disableVertexAttribArray(i)
      gl.enableVertexAttribArray(prog.aPosition)
      gl.vertexAttribPointer(prog.aPosition, 3, gl.FLOAT, false, 12, 0)
      gl.uniformMatrix4fv(prog.uViewProj, false, viewProj)
      // Always-on-top: disable depth-test so outlines show through occluders,
      // matching the cell-highlight "confident marker" convention.
      gl.disable(gl.DEPTH_TEST)
      gl.depthMask(false)
      gl.disable(gl.CULL_FACE)
      gl.disable(gl.BLEND)
      gl.lineWidth(2)

      for (const r of regions) {
        // Order: BL → BR → TR → TL (matches the line index buffer).
        scratch[0] = r.minX; scratch[1] = r.minY; scratch[2] = Z_LIFT
        scratch[3] = r.maxX; scratch[4] = r.minY; scratch[5] = Z_LIFT
        scratch[6] = r.maxX; scratch[7] = r.maxY; scratch[8] = Z_LIFT
        scratch[9] = r.minX; scratch[10] = r.maxY; scratch[11] = Z_LIFT
        gl.bufferSubData(gl.ARRAY_BUFFER, 0, scratch)

        const sel = selectedCN !== null && r.creationNumber === selectedCN
        if (sel) {
          // Selected: bright cyan, fully opaque + a touch wider so it stands out.
          gl.uniform3f(prog.uColor, 0.25, 0.95, 1.0)
          gl.uniform1f(prog.uAlpha, 1.0)
        } else {
          // Unselected: the region's stored color, normalized to 0..1. Fall back
          // to a neutral green-ish tint if the stored color is pure black (a
          // common "unset" default that would be invisible against dark terrain).
          let cr = r.color[0] / 255, cg = r.color[1] / 255, cb = r.color[2] / 255
          if (cr < 0.06 && cg < 0.06 && cb < 0.06) { cr = 0.3; cg = 0.9; cb = 0.4 }
          gl.uniform3f(prog.uColor, cr, cg, cb)
          gl.uniform1f(prog.uAlpha, 0.85)
        }
        gl.drawElements(gl.LINES, 8, gl.UNSIGNED_SHORT, 0)
      }

      gl.disableVertexAttribArray(prog.aPosition)
      // Restore depth-test for the next pass.
      gl.enable(gl.DEPTH_TEST)
      gl.depthMask(true)
    },
    dispose() {
      gl.deleteBuffer(vbo)
      gl.deleteBuffer(ibo)
      gl.deleteProgram(prog.program)
      regions = []
      selectedCN = null
    },
  }
}
