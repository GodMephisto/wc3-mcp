---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/model-thumbnail.ts"
ref: HEAD
---

// model-thumbnail.ts — shared offscreen renderer that turns a WC3 model path
// into a static 3D thumbnail image (a Blob URL an <img> can show).
//
// Why a SHARED renderer instead of one viewport per palette entry:
// mdx-m3-viewer is WebGL1-locked and each ModelViewer owns its own WebGL
// context (see AssetPreview.svelte). Browsers cap simultaneous WebGL contexts
// at ~8-16, so a grid of N live mini-viewports would blow the limit and crash.
// Instead we keep ONE offscreen ModelViewer, render each requested model to a
// single hero-pose frame, capture the canvas to a PNG Blob, and cache it by
// type. Entries become plain <img>s — cheap, scalable, and they don't each burn
// a RAF loop animating.
//
// Capture trick: the context is created with `preserveDrawingBuffer: true`
// (ModelViewer forwards its options straight to canvas.getContext('webgl', …)),
// so the drawing buffer survives until the next draw and `canvas.toBlob()` can
// read it asynchronously without coming back blank.
//
// Concurrency: one context can only render one thing at a time, so requests are
// serialized through a promise queue. A per-key inflight map dedupes concurrent
// requests for the same model, and a cache makes re-requests instant.

import * as MV_ns from 'mdx-m3-viewer'
import { pathSolver } from './scene-instances'
import { patchMdxParser } from './mdx-parser-patch'
import { registerDiag } from './diag-registry'

const MV: any = (MV_ns as any).default ?? MV_ns

// --- Diagnostics counters (U6) -------------------------------------------
// The thumbnail renderer is a quiet silent-failure site: a model that can't
// load / can't render just falls back to the 2D icon with no log. Count the
// outcomes (BEFORE the negative-cache write) so diagnostics.get / the F9
// overlay can surface a rash of unrenderable models. *Fails/*Miss suffix →
// overlay auto-red-tint.
const thumbDiag = {
  rendered: 0,        // produced a non-empty Blob URL
  loadMiss: 0,        // every candidate model path failed to load
  renderFails: 0,     // model loaded but capture/render threw or yielded ''
  viewerBuildFails: 0, // shared offscreen ModelViewer couldn't be built
}
registerDiag('thumbnails', () => ({
  ...thumbDiag,
  cached: cache.size,
  buildFailed,
}))

// Thumbnail resolution. 128² is plenty for a ~52px grid cell at 2× DPI and
// keeps per-capture cost (and the PNG size) low.
const SIZE = 128

// Hero-pose framing — mirrors AssetPreview's defaults so a doodad reads the
// same in the palette as it does in the Properties preview.
const YAW = Math.PI / 4
const PITCH = Math.PI / 7

let canvas: HTMLCanvasElement | null = null
let viewer: any = null
let scene: any = null
let builtReforged = false
let buildFailed = false

// key -> Blob URL ('' once we've confirmed the model can't be rendered, so we
// don't keep retrying a missing/broken model every time it scrolls into view).
const cache = new Map<string, string>()
// key -> in-flight promise so concurrent visibility triggers for the same model
// share one render.
const inflight = new Map<string, Promise<string>>()
// Serializes all renders onto the single shared context. Each request appends
// to the tail; failures are swallowed so the chain never wedges.
let queue: Promise<unknown> = Promise.resolve()

function teardown() {
  // mdx-m3-viewer has no clean dispose; drop refs and let GC reclaim the GL
  // context when the canvas is unreferenced (same approach AssetPreview uses).
  viewer = null
  scene = null
  canvas = null
}

function ensureViewer(reforged: boolean): boolean {
  if (viewer && builtReforged === reforged) return true
  // Reforged flips the MDX handler's asset mode, so a change needs a rebuild.
  if (viewer) teardown()
  buildFailed = false
  try {
    patchMdxParser() // idempotent
    const v = MV?.viewer
    if (!v || !v.ModelViewer || !v.handlers) throw new Error('mdx-m3-viewer namespace shape unexpected')
    canvas = document.createElement('canvas')
    canvas.width = SIZE
    canvas.height = SIZE
    // preserveDrawingBuffer so toBlob can read the frame after the fact;
    // alpha so the cleared background can be composited; antialias for clean
    // silhouettes at thumbnail scale.
    viewer = new v.ModelViewer(canvas, { alpha: true, antialias: true, preserveDrawingBuffer: true })
    // EventEmitter throws on emit('error') with no listener — every load
    // failure would otherwise crash the render path (see AssetPreview).
    viewer.on('error', () => { /* swallowed; load() resolves undefined */ })
    viewer.addHandler(v.handlers.blp)
    viewer.addHandler(v.handlers.dds)
    viewer.addHandler(v.handlers.tga)
    viewer.addHandler(v.handlers.mdx, pathSolver, reforged)
    try { v.handlers.mdx.loadTeamTextures(viewer) } catch { /* non-fatal */ }
    scene = viewer.addScene()
    // Subtle dark background, matching the AssetPreview / app chrome.
    ;(scene as any).color = [0.07, 0.07, 0.09]
    ;(scene as any).viewport[2] = SIZE
    ;(scene as any).viewport[3] = SIZE
    builtReforged = reforged
    return true
  } catch {
    buildFailed = true
    teardown()
    return false
  }
}

function applyCamera(center: [number, number, number], distance: number) {
  const cp = Math.cos(PITCH), sp = Math.sin(PITCH)
  const cy = Math.cos(YAW), sy = Math.sin(YAW)
  const [cx, cyC, cz] = center
  const eye = [
    cx - sy * cp * distance,
    cyC - cy * cp * distance,
    cz + sp * distance,
  ]
  scene.camera.perspective(Math.PI / 3, 1, Math.max(1, distance * 0.001), Math.max(5000, distance * 10))
  scene.camera.moveToAndFace(eye, [cx, cyC, cz], [0, 0, 1])
  scene.camera.update()
}

function canvasToBlobURL(c: HTMLCanvasElement): Promise<string> {
  return new Promise((resolve) => {
    try {
      c.toBlob((blob) => resolve(blob ? URL.createObjectURL(blob) : ''), 'image/png')
    } catch {
      resolve('')
    }
  })
}

async function renderOne(paths: string[], reforged: boolean): Promise<string> {
  if (!ensureViewer(reforged) || !viewer) { thumbDiag.viewerBuildFails++; return '' }
  // First model path that loads wins (variant → unsuffixed → other-extension,
  // same chain placeDoodad walks).
  let model: any = null
  for (const p of paths) {
    if (!p) continue
    try {
      const m = await viewer.load(p, pathSolver)
      if (m && typeof m.addInstance === 'function') { model = m; break }
    } catch { /* try next */ }
  }
  if (!model) { thumbDiag.loadMiss++; return '' }

  const inst = model.addInstance()
  inst.setScene(scene)
  inst.move([0, 0, 0])
  inst.uniformScale(1)
  // 45° around Z → 3/4 view instead of dead-on (AssetPreview convention).
  try { inst.setRotation?.([0, 0, 0.38268343, 0.92387953]) } catch { /* lib quirk */ }
  // Always-loop so whichever sequence we land on is mid-motion, not frozen.
  try { inst.setSequenceLoopMode?.(2) } catch { /* lib quirk */ }
  const seqs: Array<{ name: string }> = model.sequences || []
  let idx = seqs.findIndex((s) => /^\s*stand/i.test(s.name))
  if (idx < 0 && seqs.length > 0) idx = 0
  if (idx >= 0) { try { inst.setSequence(idx) } catch { /* lib quirk */ } }

  // Frame the camera on the model. bounds.r can be 0 for emitter-only dummies;
  // fall back so the camera isn't inside the model. Look-target lifted to ~40%
  // radius keeps humanoid/tree models vertically centered (AssetPreview note).
  const b = model.bounds
  const r = (b && b.r > 0) ? b.r : 200
  applyCamera([0, 0, r * 0.4], Math.max(80, r * 1.6))

  // Wait for textures (fetched async over /asset) before capturing, else the
  // thumbnail bakes in the lib's 2×2 white placeholder texture.
  try { await viewer.whenAllLoaded() } catch { /* render anyway */ }
  // Two ticks: the first binds freshly-loaded textures + advances the anim off
  // frame 0; the second renders the settled frame we capture.
  try {
    viewer.updateAndRender(0)
    viewer.updateAndRender(16)
  } catch { /* fall through to capture whatever's there */ }

  const url = canvas ? await canvasToBlobURL(canvas) : ''
  if (url) thumbDiag.rendered++; else thumbDiag.renderFails++
  // Detach the instance but keep the model + textures in the viewer's resource
  // cache — re-rendering a shared model/texture later is then nearly free.
  try { inst.detach() } catch { /* already gone */ }
  return url
}

/**
 * Resolve a model to a thumbnail Blob URL, rendering it on the shared offscreen
 * viewer if not already cached. `key` is the cache identity (use the doodad
 * type_id). `paths` is the ordered candidate model-path list (variant →
 * unsuffixed → other-extension). Returns '' when the model can't be rendered
 * (no model / load failure / renderer unavailable) — callers fall back to the
 * 2D icon. The result is cached for the session.
 */
export function loadModelThumbnail(key: string, paths: string[], reforged: boolean): Promise<string> {
  const cached = cache.get(key)
  if (cached !== undefined) return Promise.resolve(cached)
  if (buildFailed && !viewer) return Promise.resolve('')
  const existing = inflight.get(key)
  if (existing) return existing
  const p = (queue = queue.then(() =>
    renderOne(paths, reforged)
      .catch(() => '')
      .then((url) => {
        cache.set(key, url)
        inflight.delete(key)
        return url
      }),
  )) as Promise<string>
  inflight.set(key, p)
  return p
}

/**
 * Build the ordered candidate model paths for a doodad type from its SLK
 * `file` stem (which may already carry a .mdl/.mdx extension) and variation
 * count. Mirrors placeDoodad / AssetPreview's path chain so the thumbnail loads
 * the same model the viewport will place.
 */
export function doodadModelPaths(file: string, numVar: number): string[] {
  if (!file) return []
  const extMatch = file.match(/\.(mdl|mdx)$/i)
  const declaredExt = extMatch ? extMatch[0] : '.mdx'
  const stem = extMatch ? file.slice(0, -extMatch[0].length) : file
  const otherExt = declaredExt.toLowerCase() === '.mdx' ? '.mdl' : '.mdx'
  const variantPath = (numVar > 1 ? stem + '0' : stem) + declaredExt
  const fallbackPath = stem + declaredExt
  const otherExtPath = stem + otherExt
  return [...new Set([variantPath, fallbackPath, otherExtPath])]
}

/**
 * Build the ordered candidate model paths for a unit type from its SLK `file`
 * stem. Mirrors scene-instances.ts mdxPath() (the path placeUnit loads) so the
 * Unit Palette thumbnail renders the same model the viewport will place. Units
 * have no per-variation suffix (unlike doodads), so this is just the stem with
 * its declared extension plus the other-extension fallback.
 */
export function unitModelPaths(file: string): string[] {
  if (!file) return []
  const extMatch = file.match(/\.(mdl|mdx)$/i)
  const declaredExt = extMatch ? extMatch[0] : '.mdx'
  const stem = extMatch ? file.slice(0, -extMatch[0].length) : file
  const otherExt = declaredExt.toLowerCase() === '.mdx' ? '.mdl' : '.mdx'
  return [...new Set([stem + declaredExt, stem + otherExt])]
}
