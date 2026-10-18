---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/sky-gradient.ts"
ref: HEAD
---

// SkyGradient — the always-on base layer that paints the void with a soft
// 3-stop horizon gradient: deep slate-blue zenith, warm gray at the horizon,
// dark slate below. Drawn BEFORE terrain so the rest of the pipeline overlays
// it; drawn BEFORE any SetSkyModel mdx so the script-defined dome sits on top.
//
// Implementation: a 2-triangle fullscreen quad in NDC. The vertex shader uses
// the camera's `inverseViewProjectionMatrix` to convert each NDC corner into
// a world-space far-plane point, then yields a world-space ray-from-eye as a
// varying. The fragment shader takes the ray's Z component (world up) to pick
// a gradient stop. Result: the gradient follows the camera's pitch, so the
// horizon-color band sits where the actual world horizon is regardless of
// how you tilt the view.
//
// State for the pass: depth-test off, depth-write off, blending off — the
// gradient fully overwrites the framebuffer. terrain.draw() then runs with
// normal depth state and paints the map on top.

import { flog } from './debuglog'

const VERT_SHADER = `
attribute vec2 a_ndc;
uniform mat4 u_invViewProj;
uniform vec3 u_eye;
varying vec3 v_rayDir;
void main() {
  // Unproject the corner at the far plane (clip-space z=1) back to world.
  vec4 farWorld = u_invViewProj * vec4(a_ndc, 1.0, 1.0);
  vec3 farXYZ = farWorld.xyz / farWorld.w;
  v_rayDir = farXYZ - u_eye;
  // Draw at clip-space z=0 — depth-test is off for this pass anyway, this
  // just keeps the quad on the near side of the depth range so any later
  // depth-test pass that misses our intent won't auto-discard.
  gl_Position = vec4(a_ndc, 0.0, 1.0);
}
`.trim()

const FRAG_SHADER = `
precision mediump float;
uniform vec3 u_zenith;
uniform vec3 u_horizon;
uniform vec3 u_ground;
varying vec3 v_rayDir;
void main() {
  vec3 dir = normalize(v_rayDir);
  // dir.z in WC3 world space: +1 = straight up, 0 = horizon, -1 = straight down.
  float h = dir.z;
  vec3 c;
  if (h > 0.0) {
    // Above horizon: ease from horizon-color into zenith.
    c = mix(u_horizon, u_zenith, smoothstep(0.0, 0.45, h));
  } else {
    // Below horizon: ease from horizon-color into ground. Widened to 0..-0.9
    // (was -0.4) because the default RTS-overview camera tilts the view down
    // ~45°, so most visible void pixels sit at h ≈ -0.4..-0.7. With the
    // narrower transition those pixels landed firmly in the ground zone and
    // the gradient looked indistinguishable from the old flat black clear.
    c = mix(u_horizon, u_ground, smoothstep(0.0, -0.9, h));
  }
  gl_FragColor = vec4(c, 1.0);
}
`.trim()

// 3-stop palette: deep slate-blue zenith, warm gray horizon, soft slate ground.
// Values normalized 0..1. Ground is intentionally brighter than pitch black —
// the void should read as "below the horizon" rather than "screen blanker".
// Tunable; if you tweak these, keep horizon as the lightest stop so the
// gradient reads as "atmosphere".
const ZENITH:  [number, number, number] = [0x1a / 255, 0x1d / 255, 0x2a / 255]
const HORIZON: [number, number, number] = [0x5a / 255, 0x52 / 255, 0x58 / 255]
const GROUND:  [number, number, number] = [0x2a / 255, 0x25 / 255, 0x30 / 255]

function compileShader(gl: WebGLRenderingContext, type: number, src: string): WebGLShader {
  const sh = gl.createShader(type)!
  gl.shaderSource(sh, src)
  gl.compileShader(sh)
  if (!gl.getShaderParameter(sh, gl.COMPILE_STATUS)) {
    const log = gl.getShaderInfoLog(sh)
    gl.deleteShader(sh)
    throw new Error('sky-gradient shader compile: ' + log)
  }
  return sh
}

export interface SkyGradient {
  /** Run the gradient pass for this frame. Caller has cleared/setup the framebuffer. */
  draw(mainScene: any): void
  /** Release GL resources. */
  dispose(): void
}

export function buildSkyGradient(gl: WebGLRenderingContext, viewer: any): SkyGradient {
  const program = gl.createProgram()!
  gl.attachShader(program, compileShader(gl, gl.VERTEX_SHADER, VERT_SHADER))
  gl.attachShader(program, compileShader(gl, gl.FRAGMENT_SHADER, FRAG_SHADER))
  gl.linkProgram(program)
  if (!gl.getProgramParameter(program, gl.LINK_STATUS)) {
    const log = gl.getProgramInfoLog(program)
    gl.deleteProgram(program)
    throw new Error('sky-gradient link: ' + log)
  }
  const aNdc = gl.getAttribLocation(program, 'a_ndc')
  const uInvVP = gl.getUniformLocation(program, 'u_invViewProj')!
  const uEye = gl.getUniformLocation(program, 'u_eye')!
  const uZenith = gl.getUniformLocation(program, 'u_zenith')!
  const uHorizon = gl.getUniformLocation(program, 'u_horizon')!
  const uGround = gl.getUniformLocation(program, 'u_ground')!

  // Fullscreen quad: NDC corners as a triangle strip.
  const quad = new Float32Array([
    -1, -1,
     1, -1,
    -1,  1,
     1,  1,
  ])
  const vbo = gl.createBuffer()
  gl.bindBuffer(gl.ARRAY_BUFFER, vbo)
  gl.bufferData(gl.ARRAY_BUFFER, quad, gl.STATIC_DRAW)

  // Number of vertex-attribute slots the driver exposes — used to scrub leftover
  // enabled arrays before our draw (see draw()).
  const maxAttribs = gl.getParameter(gl.MAX_VERTEX_ATTRIBS) as number

  flog('[sky-gradient] built')

  let disposed = false

  return {
    draw(mainScene: any): void {
      if (disposed) return
      const cam = mainScene.camera

      // Use our program, set uniforms.
      gl.useProgram(program)
      gl.uniformMatrix4fv(uInvVP, false, cam.inverseViewProjectionMatrix)
      gl.uniform3fv(uEye, cam.location)
      gl.uniform3fv(uZenith, ZENITH)
      gl.uniform3fv(uHorizon, HORIZON)
      gl.uniform3fv(uGround, GROUND)

      // GL state: opaque, no depth, no blending. The gradient is the "void"
      // backdrop; everything else renders on top via normal depth state.
      const prevDepthTest = gl.isEnabled(gl.DEPTH_TEST)
      const prevBlend = gl.isEnabled(gl.BLEND)
      gl.disable(gl.DEPTH_TEST)
      gl.disable(gl.BLEND)
      gl.depthMask(false)

      // CRITICAL: scrub leftover vertex-attribute array state before drawing.
      // mdx-m3-viewer's model render leaves several attribute arrays ENABLED
      // (bound to per-model buffers). This pass runs first each frame and
      // inherits that state; under ANGLE/D3D11 a draw with an enabled array
      // whose buffer is stale/freed raises GL_INVALID_OPERATION on drawArrays —
      // observed as a per-frame 0x502 that froze the viewport on maps with
      // placed doodads. Disabling all slots, then enabling only ours, gives a
      // clean, valid attribute set. Downstream passes re-enable what they need.
      for (let i = 0; i < maxAttribs; i++) gl.disableVertexAttribArray(i)
      gl.bindBuffer(gl.ARRAY_BUFFER, vbo)
      gl.enableVertexAttribArray(aNdc)
      gl.vertexAttribPointer(aNdc, 2, gl.FLOAT, false, 0, 0)
      gl.drawArrays(gl.TRIANGLE_STRIP, 0, 4)
      gl.disableVertexAttribArray(aNdc)

      // Restore state for the rest of the pipeline.
      gl.depthMask(true)
      if (prevDepthTest) gl.enable(gl.DEPTH_TEST)
      if (prevBlend) gl.enable(gl.BLEND)

      // Invalidate the lib's shader cache — we touched gl.useProgram directly.
      // See memory: feedback_mdx_viewer_shader_cache.md
      ;(viewer as any).webgl.currentShader = null
    },
    dispose(): void {
      if (disposed) return
      disposed = true
      try { gl.deleteBuffer(vbo) } catch { /* swallow */ }
      try { gl.deleteProgram(program) } catch { /* swallow */ }
    },
  }
}
