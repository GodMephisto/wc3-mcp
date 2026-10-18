---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/camera-bounds-overlay.ts"
ref: HEAD
---

// camera-bounds-overlay.ts — darkens the area OUTSIDE the map's CAMERA BOUNDS
// (the playable / camera-movement limit stored in war3map.w3i:
// CameraLeftBottom..CameraRightTop), the way WC3 masks the unplayable border
// in-game. The playable rectangle is left untouched; everything outside it gets
// a translucent black "shadow" that fades IN over a short feather distance so
// the edge is soft rather than a hard cut.
//
// Implementation: a single large quad covers the whole visible area; the
// fragment shader computes each pixel's distance OUTSIDE the camera-bounds
// rectangle (0 inside) and ramps the shadow alpha from 0 → max across `u_feather`
// world units. Distance-to-rectangle gives correctly rounded, evenly-feathered
// corners for free. Drawn AFTER the main viewport pass with depth-test off so it
// darkens terrain/units beneath it regardless of their height (same always-on-
// top playbook as region-overlay.ts, alpha-blended).
//
// Coordinate model: bounds are WC3 world coords (minX/minY = left/bottom,
// maxX/maxY = right/top), the SAME space the camera renders in — no transform.

import { flog } from './debuglog'

const VERT_SHADER = `
attribute vec3 a_position;
uniform mat4 u_viewProj;
varying vec2 v_world;
void main() {
  v_world = a_position.xy;
  gl_Position = u_viewProj * vec4(a_position, 1.0);
}
`.trim()

// highp so the distance math stays accurate near the bounds edge at large world
// coordinates (desktop WebGL supports fragment highp universally).
const FRAG_SHADER = `
precision highp float;
uniform vec3 u_color;
uniform float u_maxAlpha;
uniform float u_feather;
uniform vec2 u_min;
uniform vec2 u_max;
varying vec2 v_world;
void main() {
  // Distance from the camera-bounds rectangle: 0 inside, growing outward.
  float dx = max(max(u_min.x - v_world.x, v_world.x - u_max.x), 0.0);
  float dy = max(max(u_min.y - v_world.y, v_world.y - u_max.y), 0.0);
  float dist = length(vec2(dx, dy));
  float a = clamp(dist / u_feather, 0.0, 1.0) * u_maxAlpha;
  gl_FragColor = vec4(u_color, a);
}
`.trim()

function compileShader(gl: WebGLRenderingContext, type: number, source: string): WebGLShader {
  const sh = gl.createShader(type)!
  gl.shaderSource(sh, source)
  gl.compileShader(sh)
  if (!gl.getShaderParameter(sh, gl.COMPILE_STATUS)) {
    const log = gl.getShaderInfoLog(sh)
    gl.deleteShader(sh)
    throw new Error('camera-bounds-overlay shader compile: ' + log)
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
    throw new Error('camera-bounds-overlay program link: ' + log)
  }
  return {
    program,
    aPosition: gl.getAttribLocation(program, 'a_position'),
    uViewProj: gl.getUniformLocation(program, 'u_viewProj')!,
    uColor: gl.getUniformLocation(program, 'u_color')!,
    uMaxAlpha: gl.getUniformLocation(program, 'u_maxAlpha')!,
    uFeather: gl.getUniformLocation(program, 'u_feather')!,
    uMin: gl.getUniformLocation(program, 'u_min')!,
    uMax: gl.getUniformLocation(program, 'u_max')!,
  }
}

// The camera-bounds rectangle in WC3 world coords (min = left/bottom corner,
// max = right/top corner). null when no map is loaded.
export interface CameraBoundsRect {
  minX: number
  minY: number
  maxX: number
  maxY: number
}

export interface CameraBoundsOverlay {
  /** Set the bounds rectangle (or null to draw nothing). Called on map open. */
  setBounds(b: CameraBoundsRect | null): void
  /** Draw the outside-bounds shadow (no-op when unset). */
  draw(viewProj: Float32Array): void
  /** Release GL resources. */
  dispose(): void
}

// Slight Z-lift so the shadow sits just above the ground plane.
const Z_LIFT = 6
// How far the covering quad extends past the camera bounds. Comfortably exceeds
// the terrain edge of any WC3 map (max ~480 tiles ≈ 61k units); the bounds-
// limited camera never sees the far extent, so over-extending is free.
const MARGIN = 60000
// Soft-edge width in world units (½ tile): the shadow ramps 0 → full across
// this narrow band just outside the camera bounds — just enough to take the
// aliasing off the edge without reading as blurry.
const FEATHER = 64
const SHADOW_COLOR: [number, number, number] = [0, 0, 0]
const SHADOW_ALPHA = 0.6
// One quad = 2 triangles = 6 verts, 3 floats each.
const VERT_COUNT = 6

export function buildCameraBoundsOverlay(gl: WebGLRenderingContext): CameraBoundsOverlay | null {
  let prog: ReturnType<typeof buildProgram>
  try {
    prog = buildProgram(gl)
  } catch (e) {
    flog('[camera-bounds-overlay] program build failed:', e instanceof Error ? e.message : String(e))
    return null
  }

  const vbo = gl.createBuffer()!
  gl.bindBuffer(gl.ARRAY_BUFFER, vbo)
  gl.bufferData(gl.ARRAY_BUFFER, VERT_COUNT * 3 * 4, gl.STREAM_DRAW)

  let bounds: CameraBoundsRect | null = null
  let verts: Float32Array | null = null

  function buildQuad(b: CameraBoundsRect): Float32Array {
    const x0 = b.minX - MARGIN, x1 = b.maxX + MARGIN
    const y0 = b.minY - MARGIN, y1 = b.maxY + MARGIN
    // Two triangles covering the whole area; the shader masks the inside.
    return new Float32Array([
      x0, y0, Z_LIFT, x1, y0, Z_LIFT, x1, y1, Z_LIFT,
      x0, y0, Z_LIFT, x1, y1, Z_LIFT, x0, y1, Z_LIFT,
    ])
  }

  return {
    setBounds(b: CameraBoundsRect | null) {
      bounds = b ? { ...b } : null
      verts = b ? buildQuad(b) : null
    },
    draw(viewProj: Float32Array) {
      if (!verts || !bounds) return
      gl.useProgram(prog.program)
      gl.bindBuffer(gl.ARRAY_BUFFER, vbo)
      gl.bufferSubData(gl.ARRAY_BUFFER, 0, verts)
      // Re-establish every bit of state we touch — the lib's render path leaves
      // attribs/programs in unknown states (same playbook as region-overlay).
      const maxAttribs = gl.getParameter(gl.MAX_VERTEX_ATTRIBS) | 0
      for (let i = 0; i < maxAttribs; i++) gl.disableVertexAttribArray(i)
      gl.enableVertexAttribArray(prog.aPosition)
      gl.vertexAttribPointer(prog.aPosition, 3, gl.FLOAT, false, 12, 0)
      gl.uniformMatrix4fv(prog.uViewProj, false, viewProj)
      gl.uniform3f(prog.uColor, SHADOW_COLOR[0], SHADOW_COLOR[1], SHADOW_COLOR[2])
      gl.uniform1f(prog.uMaxAlpha, SHADOW_ALPHA)
      gl.uniform1f(prog.uFeather, FEATHER)
      gl.uniform2f(prog.uMin, bounds.minX, bounds.minY)
      gl.uniform2f(prog.uMax, bounds.maxX, bounds.maxY)
      // Translucent fill: blend over whatever's beneath, always-on-top so it
      // darkens terrain/units regardless of their depth.
      gl.disable(gl.DEPTH_TEST)
      gl.depthMask(false)
      gl.disable(gl.CULL_FACE)
      gl.enable(gl.BLEND)
      gl.blendFunc(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA)
      gl.drawArrays(gl.TRIANGLES, 0, VERT_COUNT)
      // Restore the state the main pass expects.
      gl.disable(gl.BLEND)
      gl.disableVertexAttribArray(prog.aPosition)
      gl.enable(gl.DEPTH_TEST)
      gl.depthMask(true)
    },
    dispose() {
      gl.deleteBuffer(vbo)
      gl.deleteProgram(prog.program)
      bounds = null
      verts = null
    },
  }
}
