---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/bounds-debug.ts"
ref: HEAD
---

// bounds-debug.ts — DEBUG overlay that draws the bounding SPHERE used by
// rayPick (see instanceWorldBounds in scene-instances.ts) as a 3-great-circle
// wireframe. Toggled on to diagnose hover/pick accuracy: the sphere IS the hit
// volume, so seeing it explains why hovering near (but off) a tall/wide model
// still registers — the sphere covers the empty space around it.
//
// Mirrors the minimal-shader-overlay pattern of cell-highlight.ts: one static
// unit-sphere-wireframe VBO at the origin, scaled + translated per draw via
// u_center / u_radius uniforms. Always-on-top (depth test disabled) so the
// cage is visible through the model.

import { flog } from './debuglog'

const VERT_SHADER = `
attribute vec3 a_pos;
uniform mat4 u_viewProj;
uniform vec3 u_center;
uniform float u_radius;
void main() {
  gl_Position = u_viewProj * vec4(u_center + a_pos * u_radius, 1.0);
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
    throw new Error('bounds-debug shader compile: ' + log)
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
    throw new Error('bounds-debug program link: ' + log)
  }
  return {
    program,
    aPos: gl.getAttribLocation(program, 'a_pos'),
    uViewProj: gl.getUniformLocation(program, 'u_viewProj')!,
    uCenter: gl.getUniformLocation(program, 'u_center')!,
    uRadius: gl.getUniformLocation(program, 'u_radius')!,
    uColor: gl.getUniformLocation(program, 'u_color')!,
    uAlpha: gl.getUniformLocation(program, 'u_alpha')!,
  }
}

// Build a unit-radius wireframe sphere as three great circles (XY, XZ, YZ),
// emitted as discrete LINE segments (pairs of verts) so we never depend on
// LINE_LOOP, which some WebGL drivers render unevenly.
function buildUnitSphereLines(segments: number): Float32Array {
  const v: number[] = []
  const push = (x: number, y: number, z: number) => v.push(x, y, z)
  for (let i = 0; i < segments; i++) {
    const a0 = (i / segments) * Math.PI * 2
    const a1 = ((i + 1) / segments) * Math.PI * 2
    const c0 = Math.cos(a0), s0 = Math.sin(a0)
    const c1 = Math.cos(a1), s1 = Math.sin(a1)
    // XY plane (z=0)
    push(c0, s0, 0); push(c1, s1, 0)
    // XZ plane (y=0)
    push(c0, 0, s0); push(c1, 0, s1)
    // YZ plane (x=0)
    push(0, c0, s0); push(0, c1, s1)
  }
  return new Float32Array(v)
}

export interface BoundsDebug {
  /** Draw the pick sphere (center + radius, world space) as a wireframe cage. */
  draw(
    viewProj: Float32Array,
    cx: number, cy: number, cz: number, r: number,
    color?: readonly [number, number, number],
  ): void
  dispose(): void
}

const DEBUG_COLOR: readonly [number, number, number] = [1.0, 0.2, 0.9] // magenta

export function buildBoundsDebug(gl: WebGLRenderingContext): BoundsDebug | null {
  let prog: ReturnType<typeof buildProgram>
  try {
    prog = buildProgram(gl)
  } catch (e) {
    flog('[bounds-debug] program build failed:', e instanceof Error ? e.message : String(e))
    return null
  }

  const lines = buildUnitSphereLines(48)
  const vertCount = lines.length / 3
  const vbo = gl.createBuffer()!
  gl.bindBuffer(gl.ARRAY_BUFFER, vbo)
  gl.bufferData(gl.ARRAY_BUFFER, lines, gl.STATIC_DRAW)

  return {
    draw(viewProj, cx, cy, cz, r, color = DEBUG_COLOR) {
      gl.useProgram(prog.program)
      gl.bindBuffer(gl.ARRAY_BUFFER, vbo)
      const maxAttribs = gl.getParameter(gl.MAX_VERTEX_ATTRIBS) | 0
      for (let i = 0; i < maxAttribs; i++) gl.disableVertexAttribArray(i)
      gl.enableVertexAttribArray(prog.aPos)
      gl.vertexAttribPointer(prog.aPos, 3, gl.FLOAT, false, 12, 0)
      gl.uniformMatrix4fv(prog.uViewProj, false, viewProj)
      gl.uniform3f(prog.uCenter, cx, cy, cz)
      gl.uniform1f(prog.uRadius, r)
      gl.uniform3f(prog.uColor, color[0], color[1], color[2])
      gl.uniform1f(prog.uAlpha, 1.0)
      // Always-on-top, like cell-highlight: see the whole cage through the model.
      gl.disable(gl.DEPTH_TEST)
      gl.depthMask(false)
      gl.disable(gl.CULL_FACE)
      gl.disable(gl.BLEND)
      gl.lineWidth(1)
      gl.drawArrays(gl.LINES, 0, vertCount)
      gl.disableVertexAttribArray(prog.aPos)
      gl.enable(gl.DEPTH_TEST)
      gl.depthMask(true)
    },
    dispose() {
      gl.deleteBuffer(vbo)
      gl.deleteProgram(prog.program)
    },
  }
}
