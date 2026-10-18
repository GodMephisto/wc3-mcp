---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/icon-loader.ts"
ref: HEAD
---

// Browser-side BLP/DDS decoder + cache for the 2D icon thumbnails shown
// in the Explorer and Properties panel.
//
// Why: <img src=...> can't render BLP or DDS natively. The mdx-m3-viewer
// library ships JS parsers for both (since the 3D renderer already needs
// them for textures), so we reuse them here, decode to RGBA Uint8Array,
// draw into an offscreen canvas, and emit a Blob URL the <img> can load.
//
// Caching is critical: the Explorer renders one icon per row (hundreds of
// rows on real maps), the same FourCC often repeats, and decoding each
// time would burn CPU and trigger a re-fetch per request. We cache by the
// asset path so a row's icon reuses the previously decoded blob URL.
//
// Asset fetching goes through the existing /asset/<path> Wails handler
// (same one mdx-m3-viewer uses). That handler already does BLP <-> DDS
// sibling-extension fallback, so we don't have to try alternate paths
// ourselves — a single request always succeeds when a file exists at
// either extension.

// Vite's default-import interop for mdx-m3-viewer's CJS bundle is unreliable:
// the build sometimes leaves the default export wrapped, so `parsersModule.blp`
// is undefined while `parsersModule.default.blp` is the real thing. Same dance
// mdx-parser-patch.ts has to do for animationmap. Without this, decodeBLP/DDS
// throws "Cannot read properties of undefined (reading 'Image')" and the
// minimap shows "No minimap" even though Go returned the BLP bytes.
import parsersModule from 'mdx-m3-viewer/dist/cjs/parsers'
import { registerDiag } from './diag-registry'
const parsers: any = (parsersModule as any)?.default ?? parsersModule

// path -> data URL ("" once we've confirmed a 404 or decode failure, so
// repeated requests don't keep re-issuing the same XHR).
const cache = new Map<string, string>()
// path -> in-flight promise so concurrent requests for the same asset
// share a single fetch + decode pass.
const inflight = new Map<string, Promise<string>>()

// --- Diagnostics counters (U6) -------------------------------------------
// Precomputed integers bumped at the silent-failure sites below (BEFORE any
// negative-cache write, so a still-cached-as-"" failure is counted once on the
// first miss). The "scene" registry surfaces them via diagnostics.get + the F9
// overlay; the *Fails/*Miss suffix triggers the overlay's auto-red-tint.
const iconDiag = {
  decoded: 0,        // successful BLP/DDS/TGA decodes (non-empty data URL)
  fetchFails: 0,     // network/404 (resp not ok, or fetch threw)
  decodeFails: 0,    // got bytes but decode produced no image (bad magic, 0×0)
  unknownMagicMiss: 0, // bytes didn't match BLP1/BLP2/DDS magic
}
registerDiag('icons', () => ({
  ...iconDiag,
  cached: cache.size,
}))

/**
 * Resolve an icon asset path to a Blob URL suitable for <img src=...>.
 * Empty string is returned for any failure (missing file, decode error,
 * unsupported format). The result is cached for the session.
 *
 * @param assetPath e.g. "replaceabletextures/commandbuttons/btnfootman.blp"
 *                  (forward-slash, lowercase — same shape the Go normalizer
 *                  produces).
 */
export function loadIconURL(assetPath: string): Promise<string> {
  if (!assetPath) return Promise.resolve('')
  const cached = cache.get(assetPath)
  if (cached !== undefined) return Promise.resolve(cached)
  const existing = inflight.get(assetPath)
  if (existing) return existing
  const p = decodeIcon(assetPath)
    .then(url => {
      if (url) iconDiag.decoded++
      cache.set(assetPath, url)
      inflight.delete(assetPath)
      return url
    })
    .catch(_err => {
      // Log once-per-path so a missing icon doesn't spam the console for
      // every Explorer row that references it.
      // console.warn('icon decode failed:', assetPath, err)
      // Count BEFORE the negative-cache write so the swallowed throw shows up
      // in diagnostics.get instead of vanishing into cache.set('').
      iconDiag.decodeFails++
      cache.set(assetPath, '')
      inflight.delete(assetPath)
      return ''
    })
  inflight.set(assetPath, p)
  return p
}

async function decodeIcon(assetPath: string): Promise<string> {
  const url = '/asset/' + assetPath
  const resp = await fetch(url)
  if (!resp.ok) { iconDiag.fetchFails++; return '' }
  const buf = new Uint8Array(await resp.arrayBuffer())
  if (buf.byteLength < 4) { iconDiag.decodeFails++; return '' }
  // Magic-byte dispatch. The asset handler swaps BLP <-> DDS siblings on
  // miss, so what we asked for may not be what we got. Always sniff bytes.
  const magic = (buf[0] | (buf[1] << 8) | (buf[2] << 16) | (buf[3] << 24)) >>> 0
  if (magic === 0x31504c42 /* 'BLP1' */) {
    return decodeBLP(buf)
  }
  if (magic === 0x32504c42 /* 'BLP2' */) {
    return decodeBLP(buf)
  }
  if (magic === 0x20534444 /* 'DDS ' */) {
    return decodeDDS(buf)
  }
  iconDiag.unknownMagicMiss++
  return ''
}

/**
 * Decode raw image bytes (already known format) into a PNG data URL. Used by
 * non-asset-handler callers — e.g. the Minimap panel, which fetches its bytes
 * through App.GetMinimapBytes (the source file lives at the map root, not in
 * the CASC asset namespace, so going through /asset/ would 404).
 *
 * extHint disambiguates TGA from BLP/DDS (TGA has no magic bytes). Empty
 * string falls through to magic-byte sniffing identical to decodeIcon's.
 */
export function decodeImageBytes(buf: Uint8Array, extHint: string): string {
  if (buf.byteLength < 4) return ''
  if (extHint === 'tga') return decodeTGA(buf)
  const magic = (buf[0] | (buf[1] << 8) | (buf[2] << 16) | (buf[3] << 24)) >>> 0
  if (magic === 0x31504c42 || magic === 0x32504c42) return decodeBLP(buf)
  if (magic === 0x20534444) return decodeDDS(buf)
  // Fallback to extHint when magic didn't match (rare formats).
  if (extHint === 'blp') return decodeBLP(buf)
  if (extHint === 'dds') return decodeDDS(buf)
  return ''
}

function decodeBLP(buf: Uint8Array): string {
  const img = new parsers.blp.Image()
  img.load(buf)
  // Mipmap 0 is the full-resolution image. ImageData is browser-native,
  // putImageData works directly.
  const data = img.getMipmap(0)
  const canvas = document.createElement('canvas')
  canvas.width = data.width
  canvas.height = data.height
  const ctx = canvas.getContext('2d')
  if (!ctx) return ''
  ctx.putImageData(data, 0, 0)
  return canvas.toDataURL('image/png')
}

function decodeDDS(buf: Uint8Array): string {
  const img = new parsers.dds.Image()
  img.load(buf)
  // getMipmap returns a Uint8Array of RGBA pixels (already decoded from
  // DXT1/3/5/RGTC) when raw=false (default).
  const mip = img.getMipmap(0)
  const canvas = document.createElement('canvas')
  canvas.width = mip.width
  canvas.height = mip.height
  const ctx = canvas.getContext('2d')
  if (!ctx) return ''
  // Wrap the RGBA bytes in a Uint8ClampedArray for ImageData. The mip
  // buffer is owned by the parser; we don't outlive this call so no
  // copy needed.
  const imageData = new ImageData(
    new Uint8ClampedArray(mip.data.buffer, mip.data.byteOffset, mip.data.byteLength),
    mip.width,
    mip.height,
  )
  ctx.putImageData(imageData, 0, 0)
  return canvas.toDataURL('image/png')
}

// Decode a TGA buffer via mdx-m3-viewer's parsers.tga.Image. Used for legacy
// maps' war3mapPreview.tga minimap. Most modern Reforged maps ship .blp/.dds,
// so this is a fallback path — but a cheap one (the lib already includes it
// for unit-portrait textures so there's no extra dep cost).
function decodeTGA(buf: Uint8Array): string {
  const img = new parsers.tga.Image()
  img.load(buf)
  if (!img.data) return ''
  const canvas = document.createElement('canvas')
  canvas.width = img.width
  canvas.height = img.height
  const ctx = canvas.getContext('2d')
  if (!ctx) return ''
  ctx.putImageData(img.data, 0, 0)
  return canvas.toDataURL('image/png')
}
