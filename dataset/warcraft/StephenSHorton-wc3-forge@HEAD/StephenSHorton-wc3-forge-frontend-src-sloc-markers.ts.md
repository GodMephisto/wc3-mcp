---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/sloc-markers.ts"
ref: HEAD
---

// Sloc (start-location) marker renderer.
//
// In Warcraft III maps, a "sloc" is a unit Entity whose type_id is "sloc" —
// it's not a real unit, just a marker that says "player N starts here".
// Stock UnitData.slk has no row for "sloc" at all (verified via CASC probe),
// so the regular placeUnit path can't load an MDX for them.
//
// APPROACH: we draw OUR OWN marker for every start location and tint it by
// `teamColorFor(player)`, which reliably covers all 24 player slots (0..23)
// plus the neutral slots. We do NOT render the stock CircleOfPower disc.
//
// Why not the disc? The community reads CircleOfPower's glowing rune pad as
// "a player belongs here," and we used to render it tinted via the lib's
// setTeamColor(player). But in SD mode mdx-m3-viewer only ships team-color
// textures for slots 0..15, so players 16..23 (on 24-player Reforged maps)
// render as solid WHITE — and the disc's team-color glow layer ignores
// instance vertex color, so setVertexColor can't fix it. Rather than fight
// the disc's tint, every start marker is now drawn with the same WebGL
// shader-based marker, colored uniformly and correctly for every slot.
//
// We STILL create a hidden CircleOfPower MDX instance per marker. That
// instance is what registers in `unitInstances` and gives slocs their
// transform / gizmo / drag-move / selection / picking behavior in
// scene-instances.ts — all of which drive off the lib instance. We call
// `inst.hide()` so the white disc never draws (hide() stops rendering but
// keeps the node transform), then draw our own marker at the instance's
// live location each frame.
//
// The marker itself is a flat colored PAD (a short cylinder on the ground)
// plus a floating DIAMOND beacon above it, built procedurally in real stud
// units once at module load. Selection is shown via the shader's u_selected
// uniform rather than any lib-side tint.
//
// Picking integration: pickInfos() returns an axis-aligned bbox per marker
// (covering pad + floating diamond) so scene-instances.ts's ray-vs-AABB
// selection pipeline works uniformly across every marker.

import { flog } from './debuglog'

// WC3 player slot → team color, in RGB 0..1. Warcraft III Reforged supports
// 24 player slots (0..23), all of which are real player colors. The neutral
// slots come after them: 24 = Neutral Hostile, 25 = Neutral Victim,
// 26 = Neutral Extra, 27 = Neutral Passive. The mdx-m3-viewer ships
// TeamColorNN.dds with the same indices; we mirror those approximations as
// solid RGB.
export const TEAM_COLORS_RGB: Array<[number, number, number]> = [
  [1.00, 0.02, 0.02], //  0 Red
  [0.00, 0.26, 1.00], //  1 Blue
  [0.11, 0.93, 0.84], //  2 Teal
  [0.32, 0.00, 0.50], //  3 Purple
  [1.00, 1.00, 0.00], //  4 Yellow
  [1.00, 0.55, 0.00], //  5 Orange
  [0.13, 0.78, 0.06], //  6 Green
  [1.00, 0.50, 0.78], //  7 Pink
  [0.59, 0.59, 0.59], //  8 Gray
  [0.49, 0.74, 1.00], //  9 LightBlue
  [0.06, 0.39, 0.27], // 10 DarkGreen
  [0.30, 0.16, 0.00], // 11 Brown
  [0.61, 0.00, 0.00], // 12 Maroon
  [0.00, 0.00, 0.76], // 13 Navy
  [0.00, 0.92, 1.00], // 14 Turquoise
  [0.75, 0.00, 1.00], // 15 Violet
  [0.92, 0.80, 0.53], // 16 Wheat
  [0.97, 0.64, 0.55], // 17 Peach
  [0.75, 1.00, 0.50], // 18 Mint
  [0.86, 0.73, 0.92], // 19 Lavender
  [0.16, 0.16, 0.16], // 20 Coal
  [0.92, 0.94, 1.00], // 21 Snow
  [0.00, 0.47, 0.12], // 22 Emerald
  [0.64, 0.44, 0.20], // 23 Peanut
]

// Player slot → color name, parallel to TEAM_COLORS_RGB (indices 0..23 are
// real Reforged player colors). Kept next to the RGB array so the names and
// colors can't drift out of sync.
export const PLAYER_COLOR_NAMES: string[] = [
  'Red', 'Blue', 'Teal', 'Purple', 'Yellow', 'Orange', 'Green', 'Pink',
  'Gray', 'LightBlue', 'DarkGreen', 'Brown', 'Maroon', 'Navy', 'Turquoise',
  'Violet', 'Wheat', 'Peach', 'Mint', 'Lavender', 'Coal', 'Snow', 'Emerald',
  'Peanut',
]

// The true neutral slots, which sit after the 24 player colors.
const NEUTRAL_NAMES: Record<number, string> = {
  24: 'Neutral Hostile',
  25: 'Neutral Victim',
  26: 'Neutral Extra',
  27: 'Neutral Passive',
}

/** Color name for a neutral slot (24..27), or undefined for any other index. */
export function neutralName(player: number): string | undefined {
  return NEUTRAL_NAMES[player]
}

function teamColorFor(player: number): [number, number, number] {
  if (player < TEAM_COLORS_RGB.length) return TEAM_COLORS_RGB[player]
  // Neutral slots (24 Hostile / 25 Victim / 26 Extra / 27 Passive) and any
  // unknown players ≥24 → dim red — distinct from any real player slot.
  return [0.55, 0.15, 0.15]
}

// CircleOfPower asset. Path is canonical mixed-case as it appears in
// CASC's listfile; pathSolver normalizes case before fetching. We still
// load + instance this model (hidden) so slocs inherit the lib instance's
// transform / gizmo / drag-move / selection behavior — but its disc never
// renders (see inst.hide() in spawnMdxInstance).
const CIRCLE_OF_POWER_PATH = 'Buildings/Other/CircleOfPower/CircleOfPower.mdx'

// Marker geometry dimensions, in real stud units. The marker sits at the
// instance's live (x,y,z) origin and is already full-scale — the shader's
// u_scale is [1,1,1] for it.
const PAD_RADIUS = 92      // flat ground pad radius
const PAD_HEIGHT = 16      // pad rises from z=0 to z=PAD_HEIGHT
const PAD_SEGMENTS = 20    // radial segments around the pad
const DIAMOND_RADIUS = 40  // octahedron beacon radius
const DIAMOND_Z = 120      // beacon center height above the ground

/**
 * Build the marker mesh once at module load. Returns interleaved vertices
 * (position xyz + normal xyz, 6 floats each, stride 6*4, normal at offset
 * 3*4 — the layout VERT_SHADER expects) and absolute Uint16 indices into
 * the combined vertex list. Geometry is in real stud units.
 *
 * Marker = a flat colored PAD (a short cylinder on the ground) + a floating
 * DIAMOND beacon (octahedron) centered at (0,0,DIAMOND_Z).
 */
function buildMarkerGeometry(): { verts: Float32Array; indices: Uint16Array } {
  const verts: number[] = []
  const indices: number[] = []

  // --- PAD: a short cylinder, radius PAD_RADIUS, z=0..PAD_HEIGHT. ---
  // Top cap: a center vertex + a ring of PAD_SEGMENTS vertices, all with
  // up normals (0,0,1), wound CCW into a triangle fan.
  const topCenterIdx = verts.length / 6
  verts.push(0, 0, PAD_HEIGHT, 0, 0, 1)
  const topRingStart = verts.length / 6
  for (let i = 0; i < PAD_SEGMENTS; i++) {
    const a = (i / PAD_SEGMENTS) * Math.PI * 2
    verts.push(Math.cos(a) * PAD_RADIUS, Math.sin(a) * PAD_RADIUS, PAD_HEIGHT, 0, 0, 1)
  }
  for (let i = 0; i < PAD_SEGMENTS; i++) {
    const a = topRingStart + i
    const b = topRingStart + ((i + 1) % PAD_SEGMENTS)
    indices.push(topCenterIdx, a, b)
  }

  // Side wall: per segment, two ring verts (top + bottom) at the same angle
  // with a radial normal (cos,sin,0). Two triangles per segment quad.
  // Skip the bottom cap — it sits on the terrain and is never visible.
  for (let i = 0; i < PAD_SEGMENTS; i++) {
    const a0 = (i / PAD_SEGMENTS) * Math.PI * 2
    const a1 = ((i + 1) / PAD_SEGMENTS) * Math.PI * 2
    const c0 = Math.cos(a0), s0 = Math.sin(a0)
    const c1 = Math.cos(a1), s1 = Math.sin(a1)
    const base = verts.length / 6
    // 0: top @ a0, 1: bottom @ a0, 2: bottom @ a1, 3: top @ a1
    verts.push(c0 * PAD_RADIUS, s0 * PAD_RADIUS, PAD_HEIGHT, c0, s0, 0)
    verts.push(c0 * PAD_RADIUS, s0 * PAD_RADIUS, 0, c0, s0, 0)
    verts.push(c1 * PAD_RADIUS, s1 * PAD_RADIUS, 0, c1, s1, 0)
    verts.push(c1 * PAD_RADIUS, s1 * PAD_RADIUS, PAD_HEIGHT, c1, s1, 0)
    indices.push(base + 0, base + 1, base + 2)
    indices.push(base + 0, base + 2, base + 3)
  }

  // --- DIAMOND: an octahedron centered at (0,0,DIAMOND_Z), radius R. ---
  // 6 axis-offset points; 8 triangular faces. Flat-shaded: 3 unique verts
  // per face with a computed per-face normal.
  const R = DIAMOND_RADIUS
  const cz = DIAMOND_Z
  const px: [number, number, number] = [+R, 0, cz]
  const nx: [number, number, number] = [-R, 0, cz]
  const py: [number, number, number] = [0, +R, cz]
  const ny: [number, number, number] = [0, -R, cz]
  const pz: [number, number, number] = [0, 0, cz + R]
  const nz: [number, number, number] = [0, 0, cz - R]
  const faces: Array<[[number, number, number], [number, number, number], [number, number, number]]> = [
    // top 4 faces (apex pz)
    [px, py, pz],
    [py, nx, pz],
    [nx, ny, pz],
    [ny, px, pz],
    // bottom 4 faces (apex nz)
    [py, px, nz],
    [nx, py, nz],
    [ny, nx, nz],
    [px, ny, nz],
  ]
  for (const [v0, v1, v2] of faces) {
    // Flat normal = normalize(cross(v1-v0, v2-v0)).
    const e1x = v1[0] - v0[0], e1y = v1[1] - v0[1], e1z = v1[2] - v0[2]
    const e2x = v2[0] - v0[0], e2y = v2[1] - v0[1], e2z = v2[2] - v0[2]
    let nxn = e1y * e2z - e1z * e2y
    let nyn = e1z * e2x - e1x * e2z
    let nzn = e1x * e2y - e1y * e2x
    const len = Math.hypot(nxn, nyn, nzn) || 1
    nxn /= len; nyn /= len; nzn /= len
    const base = verts.length / 6
    verts.push(v0[0], v0[1], v0[2], nxn, nyn, nzn)
    verts.push(v1[0], v1[1], v1[2], nxn, nyn, nzn)
    verts.push(v2[0], v2[1], v2[2], nxn, nyn, nzn)
    indices.push(base + 0, base + 1, base + 2)
  }

  return { verts: new Float32Array(verts), indices: new Uint16Array(indices) }
}

const { verts: MARKER_VERTS, indices: MARKER_INDICES } = buildMarkerGeometry()

const VERT_SHADER = `
attribute vec3 a_position;
attribute vec3 a_normal;
uniform mat4 u_viewProj;
uniform vec3 u_origin;
uniform vec3 u_scale;
varying vec3 v_normal;
varying float v_topMask;
varying vec3 v_local;
void main() {
  vec3 worldPos = u_origin + a_position * u_scale;
  gl_Position = u_viewProj * vec4(worldPos, 1.0);
  v_normal = a_normal;
  v_topMask = a_normal.z > 0.5 ? 1.0 : 0.0;
  v_local = a_position;
}
`.trim()

const FRAG_SHADER = `
precision mediump float;
uniform vec3 u_color;
uniform float u_selected;
varying vec3 v_normal;
varying float v_topMask;
varying vec3 v_local;
void main() {
  vec3 light = normalize(vec3(0.4, 0.4, 1.0));
  float diffuse = max(0.55, dot(normalize(v_normal), light));
  vec3 col = u_color * diffuse;
  col += vec3(0.12) * v_topMask;

  // Procedural "circle of power" rune pattern, drawn only on the flat pad top
  // (top-facing AND low in Z — excludes the floating diamond at z~120). The
  // pad is a disc of radius 92 centred on the marker, so v_local.xy gives a
  // clean radial coordinate with no UVs needed. The pad base is darkened and
  // the rim / inner ring / 8 radial runes / core are brightened in the player
  // color, reading as a glowing rune pad. All tinted by u_color, so it can
  // never render white and needs no texture asset.
  if (v_topMask > 0.5 && v_local.z < 30.0) {
    float r = length(v_local.xy) / 92.0;
    float theta = atan(v_local.y, v_local.x);
    float rim    = smoothstep(0.80, 0.94, r);
    float ring   = 1.0 - smoothstep(0.0, 0.05, abs(r - 0.55));
    float band   = smoothstep(0.30, 0.42, r) - smoothstep(0.66, 0.80, r);
    float spokes = pow(0.5 + 0.5 * cos(theta * 8.0), 8.0) * max(band, 0.0);
    float core   = 1.0 - smoothstep(0.0, 0.28, r);
    float glow = clamp(rim * 1.1 + ring * 0.7 + spokes * 0.7 + core * 0.8, 0.0, 1.4);
    vec3 base = u_color * diffuse * 0.45;
    col = base + u_color * glow + vec3(0.12) * glow;
  }

  if (u_selected > 0.5) {
    col = mix(col, vec3(1.0, 0.95, 0.4), 0.55);
  }
  gl_FragColor = vec4(col, 1.0);
}
`.trim()

function compileShader(gl: WebGLRenderingContext, type: number, source: string): WebGLShader {
  const sh = gl.createShader(type)!
  gl.shaderSource(sh, source)
  gl.compileShader(sh)
  if (!gl.getShaderParameter(sh, gl.COMPILE_STATUS)) {
    const log = gl.getShaderInfoLog(sh)
    gl.deleteShader(sh)
    throw new Error('sloc shader compile: ' + log)
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
    throw new Error('sloc program link: ' + log)
  }
  return {
    program,
    aPosition: gl.getAttribLocation(program, 'a_position'),
    aNormal: gl.getAttribLocation(program, 'a_normal'),
    uViewProj: gl.getUniformLocation(program, 'u_viewProj')!,
    uOrigin: gl.getUniformLocation(program, 'u_origin')!,
    uScale: gl.getUniformLocation(program, 'u_scale')!,
    uColor: gl.getUniformLocation(program, 'u_color')!,
    uSelected: gl.getUniformLocation(program, 'u_selected')!,
  }
}

// One sloc marker.
export interface SlocMarker {
  creationNumber: number
  player: number
  position: [number, number, number]
  rotation: number
}

export interface SlocPickInfo {
  creationNumber: number
  /** World-space center of the marker AABB. */
  center: [number, number, number]
  /** Half-extents of the marker AABB. */
  half: [number, number, number]
}

export interface SlocRenderer {
  /** Draw our own colored marker for every start location. */
  draw(viewProj: Float32Array, selected: Set<number>): void
  /** Replace the marker list (called on map open). */
  setMarkers(ms: SlocMarker[]): void
  /** Picking info for ray-vs-AABB tests — one entry per marker. */
  pickInfos(): SlocPickInfo[]
  /** Release GL resources and detach lib instances. */
  dispose(): void
  /** Toggle viewport visibility of all start-location markers. */
  setVisible(visible: boolean): void
}

/**
 * Path solver matching scene-instances.ts pathSolver — kept inline so this
 * module doesn't pull in the full scene module. mdx-m3-viewer hands raw
 * MDX asset names (and texture names) to its solver; we route them through
 * Wails' embedded HTTP server to Go's assetHandler.
 */
function slocPathSolver(src: unknown): unknown {
  if (typeof src !== 'string') return undefined
  return '/asset/' + src.toLowerCase().replace(/\\/g, '/')
}

function quatZ(angle: number): number[] {
  const h = angle * 0.5
  return [0, 0, Math.sin(h), Math.cos(h)]
}

/**
 * Build the sloc renderer. `viewer` and `scene` are optional — when both
 * are present we create one (hidden) CircleOfPower MDX instance per marker
 * to carry its transform / gizmo / drag-move / selection behavior, then
 * draw our own colored marker at the instance's live location. When either
 * is missing or the model load fails, the marker still draws at the stored
 * position; only the gizmo / drag-move integration is unavailable.
 *
 * `unitInstances` is the scene's master cn→MdxModelInstance map. When
 * provided, the hidden sloc MDX instances are registered there alongside
 * real units
 * so the gizmo, drag-to-move, and entity-changed handlers in
 * scene-instances.ts all find them by creation_number without needing
 * sloc-specific branches. WC3 itself stores slocs as units (type_id
 * "sloc") in war3mapUnits.doo with the same position/rotation/scale
 * fields as any other unit, so they're transformable like anything else.
 */
export function buildSlocRenderer(
  gl: WebGLRenderingContext,
  viewer: any | null = null,
  scene: any | null = null,
  unitInstances: Map<number, any> | null = null,
): SlocRenderer | null {
  let prog: ReturnType<typeof buildProgram>
  try {
    prog = buildProgram(gl)
  } catch (e) {
    flog('[slocs] program build failed:', e instanceof Error ? e.message : String(e))
    return null
  }

  const vbo = gl.createBuffer()!
  gl.bindBuffer(gl.ARRAY_BUFFER, vbo)
  gl.bufferData(gl.ARRAY_BUFFER, MARKER_VERTS, gl.STATIC_DRAW)

  const ibo = gl.createBuffer()!
  gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, ibo)
  gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, MARKER_INDICES, gl.STATIC_DRAW)

  let markers: SlocMarker[] = []
  // Viewport visibility for all start-location markers, toggled from the
  // Explorer "Markers" section eye button. Hidden markers neither draw nor pick.
  let visible = true
  // Per-marker hidden CircleOfPower MdxModelInstance, or absent if the model
  // failed to load / the load is still in flight when setMarkers was called.
  // These instances never render (inst.hide()); they exist only to carry the
  // transform / gizmo / drag-move / selection behavior. We read their live
  // location each frame to position our own marker.
  const mdxInstances = new Map<number, any>()

  // CircleOfPower model — loaded lazily on first setMarkers when viewer+scene
  // are both available. `null` means "load not yet attempted"; a Promise
  // means "load in flight"; a resolved model means success; `circleModelFailed`
  // means a prior attempt failed permanently and we shouldn't retry.
  let circleModelP: Promise<any> | null = null
  let circleModel: any = null
  let circleModelFailed = false

  function loadCircleModel(): Promise<any | null> {
    if (circleModel) return Promise.resolve(circleModel)
    if (circleModelFailed) return Promise.resolve(null)
    if (!viewer || typeof viewer.load !== 'function') return Promise.resolve(null)
    if (!circleModelP) {
      circleModelP = (async () => {
        try {
          const m = await viewer.load(CIRCLE_OF_POWER_PATH, slocPathSolver)
          if (!m || typeof m.addInstance !== 'function') {
            circleModelFailed = true
            flog('[slocs] CircleOfPower load returned no addInstance — falling back to pillars')
            return null
          }
          circleModel = m
          flog('[slocs] CircleOfPower loaded')
          return m
        } catch (e) {
          circleModelFailed = true
          flog('[slocs] CircleOfPower load failed, using pillar fallback:', e instanceof Error ? e.message : String(e))
          return null
        }
      })()
    }
    return circleModelP
  }

  // Create one CircleOfPower instance for a marker. Safe to call only after
  // loadCircleModel has resolved with a non-null model.
  function spawnMdxInstance(m: SlocMarker): any | null {
    if (!circleModel || !scene) return null
    let inst: any
    try {
      inst = circleModel.addInstance()
      if (!inst) return null
      inst.move([m.position[0], m.position[1], m.position[2]])
      inst.rotateLocal(quatZ(m.rotation))
      inst.setTeamColor(m.player)
      inst.setScene(scene)
      // Tag the instance with the same convenience props placeUnit writes
      // (scene-instances.ts) so the gizmo, drag-move arming, and
      // entity-changed rotation/scale handlers can treat slocs identically
      // to real units. Stock UnitData.slk has no row for "sloc" — there is
      // no SLK model_scale or move_height to bake in — so the multipliers
      // are 1× / 0 by definition.
      ;(inst as any).__wc3ForgeTypeId = 'sloc'
      ;(inst as any).__wc3ForgeRotation = m.rotation
      ;(inst as any).__wc3ForgeScale = [1, 1, 1]
      ;(inst as any).__wc3ForgeModelScale = 1
      // Best-effort: kick the model into its "stand" sequence so the runes
      // animate. CircleOfPower's stand is index 0 in practice but we look
      // it up by name to stay robust to MDX edits.
      const seqs = inst?.model?.sequences
      if (Array.isArray(seqs)) {
        let standIdx = -1
        for (let i = 0; i < seqs.length; i++) {
          const name = String(seqs[i]?.name ?? '')
          if (name.split('-')[0].replace(/\d/g, '').trim().toLowerCase() === 'stand') {
            standIdx = i
            break
          }
        }
        if (standIdx >= 0) inst.setSequence(standIdx)
      }
      // Register in the scene's master unit-instance map so the gizmo +
      // drag-move + entity-changed flows in scene-instances.ts find slocs
      // by creation_number with no sloc-specific branches.
      if (unitInstances) unitInstances.set(m.creationNumber, inst)
      // Hide the disc's rendering — we draw our own colored marker instead
      // (the disc tints white for players 16..23 in SD mode). hide() stops
      // the instance from rendering but keeps its node transform intact, so
      // the gizmo / drag-move / selection / picking integration above all
      // keep working off the live instance location.
      try { inst.hide() } catch { /* lib path; swallow */ }
    } catch (e) {
      flog('[slocs] spawn instance failed:', e instanceof Error ? e.message : String(e))
      return null
    }
    return inst
  }

  function clearMdxInstances() {
    for (const [cn, inst] of mdxInstances) {
      if (!inst) continue
      // detach() is the lib's "remove from scene" op (scene.removeInstance).
      // Matches what scene-instances.clearInstances() does on real units.
      // Idempotent: returns false (no-op) if the instance is already
      // detached, so double-cleanup from a clearInstances → setMarkers([])
      // sequence is safe.
      try { inst.detach() } catch { /* swallow */ }
      // Mirror in unitInstances. Safe when unitInstances was already
      // cleared by scene-instances.clearInstances() — delete on a missing
      // key is a no-op.
      if (unitInstances) unitInstances.delete(cn)
    }
    mdxInstances.clear()
  }

  // Live world location for a marker: prefer the hidden instance's
  // localLocation (the un-offset node position the gizmo/drag-move writes),
  // then its worldLocation, then the marker's stored position when no
  // instance exists yet (model load in flight / failed).
  function liveLoc(m: SlocMarker): [number, number, number] {
    const inst = mdxInstances.get(m.creationNumber)
    const loc = (inst && inst.localLocation) ? inst.localLocation
      : ((inst && inst.worldLocation) ? inst.worldLocation : m.position)
    return [loc[0], loc[1], loc[2]]
  }

  return {
    setMarkers(ms: SlocMarker[]) {
      // Render slocs at their on-disk positions verbatim, even when a map
      // (e.g. Enfo's FFB) places every sloc at the same coordinate. The
      // map author chose to stack them; the editor should show that truth
      // and let the author see what they authored. Coincident markers will
      // z-fight and only one player's color will read — that's correct.
      markers = ms.slice()

      // Tear down any instances from a previous map.
      clearMdxInstances()

      // If we already have the model cached, spawn (hidden) instances
      // immediately; otherwise kick off the async load and spawn when it
      // resolves. draw() positions our own marker via each instance's live
      // location (or the stored marker position before the instance exists).
      if (!viewer || !scene) return
      if (circleModel) {
        for (const m of markers) {
          const inst = spawnMdxInstance(m)
          if (inst) mdxInstances.set(m.creationNumber, inst)
        }
        return
      }
      if (circleModelFailed) return
      // Capture markers at-call-time so a fast subsequent setMarkers (e.g.
      // user opens map B while map A's load is mid-flight) doesn't spawn
      // stale markers into the new state.
      const myMarkers = markers
      loadCircleModel().then((m) => {
        if (!m) return
        if (myMarkers !== markers) return // superseded by a later setMarkers
        for (const mk of markers) {
          if (mdxInstances.has(mk.creationNumber)) continue
          const inst = spawnMdxInstance(mk)
          if (inst) mdxInstances.set(mk.creationNumber, inst)
        }
      })
    },

    pickInfos(): SlocPickInfo[] {
      if (!visible) return []
      const out: SlocPickInfo[] = []
      for (const m of markers) {
        // One AABB per marker, centered on the live location. The box
        // covers the ground pad and the floating diamond beacon — its
        // vertical span is ~0..160 studs (center +80, half-extent 90),
        // and ±96 horizontally to give a comfortable click target. Reading
        // the live location keeps the click target tracking the gizmo /
        // drag-to-move without waiting for a setMarkers() rebuild.
        const loc = liveLoc(m)
        out.push({
          creationNumber: m.creationNumber,
          center: [loc[0], loc[1], loc[2] + 80],
          half: [96, 96, 90],
        })
      }
      return out
    },

    draw(viewProj: Float32Array, selected: Set<number>) {
      if (!visible) return
      if (markers.length === 0) return

      // The lib's render path leaves an unknown WebGL state behind: vertex
      // attribs 2+ may still be enabled and pointing into model buffers,
      // depth mask may be off, blend func may be set to translucent-merge,
      // current program changes per batch. We explicitly re-establish every
      // bit of state we care about — cheap and avoids ghost-rendering.
      gl.useProgram(prog.program)
      gl.bindBuffer(gl.ARRAY_BUFFER, vbo)
      gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, ibo)
      const maxAttribs = gl.getParameter(gl.MAX_VERTEX_ATTRIBS) | 0
      for (let i = 0; i < maxAttribs; i++) gl.disableVertexAttribArray(i)
      gl.enableVertexAttribArray(prog.aPosition)
      gl.enableVertexAttribArray(prog.aNormal)
      const stride = 6 * 4
      gl.vertexAttribPointer(prog.aPosition, 3, gl.FLOAT, false, stride, 0)
      gl.vertexAttribPointer(prog.aNormal, 3, gl.FLOAT, false, stride, 3 * 4)
      gl.uniformMatrix4fv(prog.uViewProj, false, viewProj)
      gl.enable(gl.DEPTH_TEST)
      gl.depthFunc(gl.LEQUAL)
      gl.depthMask(true)
      gl.disable(gl.CULL_FACE)
      gl.disable(gl.BLEND)

      // Draw our own marker for EVERY start location, at its live location,
      // tinted by player slot (reliable for 0..23 + neutrals — never white).
      // The geometry is already in stud units, so u_scale is unity.
      for (const m of markers) {
        const loc = liveLoc(m)
        gl.uniform3f(prog.uOrigin, loc[0], loc[1], loc[2])
        gl.uniform3f(prog.uScale, 1, 1, 1)
        const c = teamColorFor(m.player)
        gl.uniform3f(prog.uColor, c[0], c[1], c[2])
        gl.uniform1f(prog.uSelected, selected.has(m.creationNumber) ? 1 : 0)
        gl.drawElements(gl.TRIANGLES, MARKER_INDICES.length, gl.UNSIGNED_SHORT, 0)
      }

      gl.disableVertexAttribArray(prog.aPosition)
      gl.disableVertexAttribArray(prog.aNormal)
    },

    dispose() {
      clearMdxInstances()
      // Release the marker mesh buffers + shader program.
      gl.deleteBuffer(vbo)
      gl.deleteBuffer(ibo)
      gl.deleteProgram(prog.program)
      markers = []
    },
    setVisible(b: boolean) {
      visible = b
    },
  }
}
