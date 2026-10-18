---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/sky.ts"
ref: HEAD
---

// SkyRenderer — draws the WC3 sky-dome MDX behind/around the terrain.
//
// The sky path is resolved Go-side (see sky.go ResolveSkyModel): SetSkyModel
// calls in the map script override the wc3-forge tileset-default table, which
// in turn beats "no sky". Empty string = render nothing here; the existing
// flat-clear remains visible.
//
// Render contract:
//   - Owns its own mdx-m3-viewer Scene so we can drive update/render manually,
//     keep it out of `viewer.scenes` (so viewer.render() doesn't auto-process
//     it under default GL state), and apply skybox-style depth state for the
//     pass.
//   - The sky scene's camera is the SAME object as the main scene's camera —
//     view/projection matrices stay in sync without copying.
//   - Per frame the sky instance's location is set to camera.eye so the dome
//     follows the viewer (matching the in-engine "sky never gets closer"
//     behavior).
//
// Depth state during the sky pass: depthMask=false, DEPTH_TEST disabled. The
// terrain pass that runs next re-enables both and writes the real scene depth,
// so units and doodads still occlude correctly against terrain — they just
// never occlude the sky (which is correct: the sky is conceptually at
// infinity).
//
// State-cache trap (memory: feedback_mdx_viewer_shader_cache.md): rendering
// MDX through scene.render() respects the lib's webgl.currentShader cache,
// but the *terrain* shader pass that runs right after touches gl.useProgram
// directly. Caller must null viewer.webgl.currentShader after the sky pass
// for the same reason terrain/cliffs/water do — keep that invariant in the
// integration point.

import { flog } from './debuglog'

export interface SkyRenderer {
  /** Run the sky pass for this frame. Caller has just cleared the framebuffer. */
  draw(dtMs: number): void
  /** Detach + dispose. Safe to call multiple times. */
  dispose(): void
}

/**
 * Build a sky renderer that draws the given MDX path under the given camera.
 *
 * Returns null when:
 *   - `skyPath` is empty (the map has no sky), or
 *   - the MDX load fails (missing CASC entry, bad path, etc).
 *
 * In both null cases the existing flat clear color stays visible — that's
 * the intended fallback per Phase 1 design.
 */
export async function buildSky(
  gl: WebGLRenderingContext,
  viewer: any,
  pathSolver: any,
  mainScene: any,
  skyPath: string,
): Promise<SkyRenderer | null> {
  if (!skyPath) return null

  let model: any
  try {
    model = await viewer.load(skyPath, pathSolver)
  } catch (e) {
    flog('[sky] load failed:', skyPath, e instanceof Error ? e.message : String(e))
    return null
  }
  if (!model) {
    flog('[sky] viewer.load returned null for', skyPath)
    return null
  }

  // Dedicated scene so we can drive its render lifecycle manually. We share
  // the main scene's camera object (not a copy) so view/projection track
  // automatically as the user pans/zooms.
  const skyScene = viewer.addScene()
  ;(skyScene as any).alpha = true
  skyScene.camera = mainScene.camera

  // Pull the sky scene out of viewer.scenes so viewer.update / viewer.render
  // don't auto-process it under default GL state. We still call the scene's
  // update + startFrame + renderOpaque + renderTranslucent ourselves below.
  const scenes: any[] = (viewer as any).scenes
  const idx = scenes.indexOf(skyScene)
  if (idx >= 0) scenes.splice(idx, 1)

  // Sky models are inside-out domes; force every layer twoSided=1 so the
  // lib disables CULL_FACE during their render pass (otherwise back-face
  // culling kills the inside-of-dome triangles for models authored without
  // the TwoSided flag — Outland_Sky has it, LordaeronSummerSky doesn't, so
  // without the patch one renders and the other doesn't).
  try {
    const mats = (model as any).materials ?? []
    for (const mat of mats) {
      const layers = mat?.layers ?? []
      for (const l of layers) {
        if (!l.twoSided) l.twoSided = 1
      }
    }
  } catch (e) {
    flog('[sky] twoSided patch failed:', e instanceof Error ? e.message : String(e))
  }

  let inst: any
  try {
    inst = model.addInstance()
    inst.setScene(skyScene)
  } catch (e) {
    flog('[sky] instance create failed:', e instanceof Error ? e.message : String(e))
    skyScene.detach?.()
    return null
  }

  let disposed = false

  return {
    draw(dtMs: number): void {
      if (disposed) return
      const eye = mainScene.camera.location as Float32Array | number[]
      // Lock the dome to the viewer's eye. Skybox MDX files are authored
      // huge (radius ~60k studs) so the geometry comfortably wraps the
      // playable area; without this lock the sky drifts as the user pans.
      inst.setLocation(eye)
      skyScene.update(dtMs)

      // Skybox depth state: don't read or write depth, so terrain (drawn
      // next) overwrites the sky color where the world exists, but the sky
      // shows through every "void" pixel that terrain doesn't touch.
      const prevDepthTest = gl.isEnabled(gl.DEPTH_TEST)
      gl.depthMask(false)
      gl.disable(gl.DEPTH_TEST)

      skyScene.startFrame()
      skyScene.renderOpaque()
      skyScene.renderTranslucent()

      // Restore depth state for the rest of the pipeline.
      if (prevDepthTest) gl.enable(gl.DEPTH_TEST)
      gl.depthMask(true)

      // Invalidate the lib's shader cache — terrain's direct gl.useProgram
      // call would otherwise leave the lib thinking an MDX shader is bound
      // when it isn't, breaking subsequent MDX uniform writes. See
      // memory: feedback_mdx_viewer_shader_cache.md
      ;(viewer as any).webgl.currentShader = null
    },
    dispose(): void {
      if (disposed) return
      disposed = true
      try { inst?.detach?.() } catch { /* swallow */ }
      try { skyScene?.detach?.() } catch { /* swallow */ }
    },
  }
}
