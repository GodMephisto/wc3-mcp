---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/pick-buffer.ts"
ref: HEAD
---

// pick-buffer.ts — pixel-accurate hit testing against an MDX instance's actual
// silhouette (not its bounding sphere). Renders ONE instance into an offscreen
// RGBA mask via the caller's render callback (the library's own render path —
// the exact mechanism instance-outline.ts uses, which is proven to produce a
// correct silhouette), then reads the single pixel under the cursor. If the
// model covers that pixel (alpha above threshold) it's a hit.
//
// Used by rayPick's narrow phase: the bounding-sphere test is a cheap broad
// phase that over-reports (a sphere swallows the empty space around tall/wide
// models); this refines each candidate to "is the cursor actually on the
// model?". Foliage gaps and concave shapes are respected because we test the
// real rendered coverage.

import { flog } from './debuglog'

// Alpha (0-255) above which a pixel counts as covered. Small but non-zero so
// anti-aliased / faintly-translucent edge pixels don't register as solid.
const COVERAGE_THRESHOLD = 8

export interface PickBuffer {
  /** Resize the offscreen mask to match the canvas. */
  setSize(w: number, h: number): void
  /**
   * Render `render` (which must draw exactly the candidate instance into the
   * currently-bound framebuffer — this binds the mask FBO first) and report
   * whether the model covers DOM pixel (px, py). Restores the default
   * framebuffer before returning.
   */
  testCoverage(render: () => void, px: number, py: number): boolean
  dispose(): void
}

export function buildPickBuffer(gl: WebGLRenderingContext): PickBuffer | null {
  const fbo = gl.createFramebuffer()
  const colorTex = gl.createTexture()
  const depthRb = gl.createRenderbuffer()
  if (!fbo || !colorTex) {
    flog('[pick-buffer] failed to allocate GL objects')
    return null
  }
  let texW = 0
  let texH = 0
  let ok = false
  const px4 = new Uint8Array(4)

  function setSize(w: number, h: number) {
    w = Math.max(1, w | 0)
    h = Math.max(1, h | 0)
    if (w === texW && h === texH) return
    texW = w
    texH = h
    gl.bindTexture(gl.TEXTURE_2D, colorTex)
    gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, w, h, 0, gl.RGBA, gl.UNSIGNED_BYTE, null)
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST)
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST)
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE)
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE)
    gl.bindRenderbuffer(gl.RENDERBUFFER, depthRb)
    gl.renderbufferStorage(gl.RENDERBUFFER, gl.DEPTH_COMPONENT16, w, h)
    gl.bindFramebuffer(gl.FRAMEBUFFER, fbo)
    gl.framebufferTexture2D(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.TEXTURE_2D, colorTex, 0)
    gl.framebufferRenderbuffer(gl.FRAMEBUFFER, gl.DEPTH_ATTACHMENT, gl.RENDERBUFFER, depthRb)
    ok = gl.checkFramebufferStatus(gl.FRAMEBUFFER) === gl.FRAMEBUFFER_COMPLETE
    if (!ok) flog('[pick-buffer] framebuffer incomplete; accurate picking disabled')
    gl.bindFramebuffer(gl.FRAMEBUFFER, null)
    gl.bindRenderbuffer(gl.RENDERBUFFER, null)
    gl.bindTexture(gl.TEXTURE_2D, null)
  }

  return {
    setSize,
    testCoverage(render: () => void, px: number, py: number): boolean {
      if (!ok || texW === 0) return false
      const prevFbo = gl.getParameter(gl.FRAMEBUFFER_BINDING) as WebGLFramebuffer | null
      gl.bindFramebuffer(gl.FRAMEBUFFER, fbo)
      gl.viewport(0, 0, texW, texH)
      gl.disable(gl.SCISSOR_TEST)
      gl.clearColor(0, 0, 0, 0)
      gl.depthMask(true)
      gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT)
      gl.enable(gl.DEPTH_TEST)
      render()
      // readPixels is bottom-left origin; the cursor is DOM top-left → flip Y.
      const gx = Math.min(texW - 1, Math.max(0, Math.round(px)))
      const gy = Math.min(texH - 1, Math.max(0, texH - 1 - Math.round(py)))
      gl.readPixels(gx, gy, 1, 1, gl.RGBA, gl.UNSIGNED_BYTE, px4)
      gl.bindFramebuffer(gl.FRAMEBUFFER, prevFbo)
      return px4[3] > COVERAGE_THRESHOLD
    },
    dispose() {
      gl.deleteFramebuffer(fbo)
      gl.deleteTexture(colorTex)
      if (depthRb) gl.deleteRenderbuffer(depthRb)
    },
  }
}
