---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/path-blockers.ts"
ref: HEAD
---

// path-blockers.ts — pink/black checker 3D box markers for "Pathing Blockers"
// category doodads.
//
// What a path blocker is: a doodad whose category in Doodads.slk is "P"
// (resolved through WESTRING_DTYPE_PATHING to "Pathing Blockers"). Their
// purpose in WC3 is to make terrain unwalkable / unbuildable / unflyable
// without showing any in-game art. Most of these rows ship with NO MDX file
// at all (info.file is empty) — the game just consumes their pathing
// footprint. That's correct for in-game, but in an editor the placements
// are invisible, which makes them impossible to inspect.
//
// HiveWE handles this two ways simultaneously: (1) substitutes
// Objects/Invalidmodel/Invalidmodel.mdx (the in-engine "broken model"
// placeholder, which is a magenta/black checker box) when the SLK row has
// no file, and (2) blits the row's pathtex texture into the editor's
// pathing-map debug view. We're not loading Invalidmodel.mdx here because
// the lib's resourceMap caching plus a missing-asset render path adds risk
// (the parser-patch shader-cache trap is the kind of bug that ate days of
// our render bring-up — see feedback-mdx-viewer-shader-cache). Instead we
// draw a small 3D box procedurally with the same pink/black aesthetic the
// user already recognises from HiveWE / the in-engine fallback.
//
// Why a 3D box (not a flat quad): the previous quad implementation read as
// "smear on the ground" at any angle — easy to mistake for a terrain tint
// rather than a placed entity, and impossible to see from a top-down angle
// when overlapping other doodads. A box silhouette is unmistakable at any
// editor pitch and at any zoom: vertical edges signal "thing here" even when
// the top face's checker is too small to resolve. Size choice: 64×64 studs
// XY footprint (= 1/4 of one 128×128 terrain tile, per the user spec) and
// 128 studs tall (one tile-edge — same scale the user sees in the cell-grid
// overlay, so the marker feels at-home in the editor's coordinate system).
//
// Approach: same shape as sloc-markers.ts and cell-highlight.ts — minimal
// standalone WebGL1 shader owned by this module, drawn AFTER viewer.render()
// in the main RAF loop. Each blocker is one box (12 triangles) at the
// doodad's world XY + a small Z lift so it sits visibly above terrain. The
// fragment shader generates the checker procedurally from world-space coords
// projected onto the per-face plane via the normal (so the pattern wraps
// consistently across all six faces). Semi-transparent so the user can still
// see what's underneath.
//
// Picking: pickInfos() returns the AABB of each placed blocker so the
// scene-instances ray-pick walker can include them alongside units, doodads,
// and slocs. The kind reported is 'doodad' so the Properties panel resolves
// through GetDoodad (path blockers ARE doodads — they're entries in
// war3map.doo with creation_numbers in the doodad namespace). The AABB now
// covers the box's full vertical extent so clicks anywhere on the silhouette
// register, not just on the ground footprint.

import { flog } from './debuglog'

// Footprint of a single path blocker. World-space center at (x, y, z) with
// half-extents (hx, hy). The Z lift is baked in below — the position the
// caller passes us is the raw doodad position; the box's base sits `Z_LIFT`
// studs above it.
//
// In a future revision we could read the per-type pathtex SLK column and
// shape the box to match (4x4Default = 1 cell wide, 8x8Default = 2 cells,
// etc.), but the current single-cell-quarter default is the right
// out-of-the-box look for "I want to see WHERE the blocker is" — and
// adjusting per-instance scale via `scale` on the Doodad DTO already handles
// the common case of a blocker that's been scaled up to cover a wider region.
export interface PathBlockerMarker {
  creationNumber: number
  position: [number, number, number]
  /** Per-side half-extent in studs. Default 32 = 1/4 of one 128-stud tile. */
  halfX: number
  halfY: number
}

export interface PathBlockerPickInfo {
  creationNumber: number
  center: [number, number, number]
  half: [number, number, number]
}

export interface PathBlockerRenderer {
  /**
   * Draw all blockers. `selected` gets the white selection wash; `hovered`
   * (a single creation_number, or null) and any cn in `preview` (the live
   * rubber-band set, or null) get the cyan hover wash — matching the white/cyan
   * outline used for MDX units & doodads, which a procedural box can't receive.
   */
  draw(viewProj: Float32Array, selected: Set<number>, hovered: number | null, preview: Set<number> | null): void
  /** Replace the marker list (called on map open). */
  setMarkers(ms: PathBlockerMarker[]): void
  /** AABB info for ray-vs-AABB picking — one entry per marker. */
  pickInfos(): PathBlockerPickInfo[]
  /** Release GL resources. */
  dispose(): void
}

// Z lift in studs. Small bump above the ground so the base of the box doesn't
// z-fight terrain on a tilted camera. Same rationale as cell-highlight.ts.
const Z_LIFT = 2

// Vertical extent of the box in studs. Set to 64 so the box reads as a cube
// matching the default XY footprint (half-extent 32 → 64-stud width). User
// feedback: a 128-stud box reads as ~2× too tall vs the footprint and looks
// like a column rather than a marker block. Half-extent (32) is what the AABB
// picker uses for its Z half.
const BOX_HEIGHT = 64
const BOX_HALF_Z = BOX_HEIGHT * 0.5

// Checker pattern scale: each square in the checker is CHECKER_STUDS wide in
// world space. 16 studs gives a 4×4 = 16-cell checker on each face of the
// default 64-stud cube — denser than the previous 2×2 grid, more legible as
// a "hazard" texture at every editor zoom.
const CHECKER_STUDS = 16

// Unit cube from (-1,-1,0) to (+1,+1,+1) in local space. The vertex shader
// scales X/Y by per-marker half-extents (`u_half`) and Z by box height
// (`u_height`), so the base sits on the lifted ground at the marker's XYZ
// and the top is at z + BOX_HEIGHT.
//
// Vertices duplicated per-face so each face gets its own outward normal for
// the fragment shader's per-face lighting + checker-projection axis pick.
// Layout: vec3 position + vec3 normal interleaved (6 floats * 24 verts).
const BOX_VERTS = new Float32Array([
  // +X face
  +1, -1, 0,   1, 0, 0,
  +1, +1, 0,   1, 0, 0,
  +1, +1, 1,   1, 0, 0,
  +1, -1, 1,   1, 0, 0,
  // -X face
  -1, +1, 0,  -1, 0, 0,
  -1, -1, 0,  -1, 0, 0,
  -1, -1, 1,  -1, 0, 0,
  -1, +1, 1,  -1, 0, 0,
  // +Y face
  +1, +1, 0,   0, 1, 0,
  -1, +1, 0,   0, 1, 0,
  -1, +1, 1,   0, 1, 0,
  +1, +1, 1,   0, 1, 0,
  // -Y face
  -1, -1, 0,   0, -1, 0,
  +1, -1, 0,   0, -1, 0,
  +1, -1, 1,   0, -1, 0,
  -1, -1, 1,   0, -1, 0,
  // +Z face (top)
  -1, -1, 1,   0, 0, 1,
  +1, -1, 1,   0, 0, 1,
  +1, +1, 1,   0, 0, 1,
  -1, +1, 1,   0, 0, 1,
  // -Z face (bottom) — rarely visible from editor pitch but cheap to draw
  -1, +1, 0,   0, 0, -1,
  +1, +1, 0,   0, 0, -1,
  +1, -1, 0,   0, 0, -1,
  -1, -1, 0,   0, 0, -1,
])
const BOX_INDICES = new Uint16Array([
  0,1,2, 0,2,3,
  4,5,6, 4,6,7,
  8,9,10, 8,10,11,
  12,13,14, 12,14,15,
  16,17,18, 16,18,19,
  20,21,22, 20,22,23,
])

const VERT_SHADER = `
attribute vec3 a_position;
attribute vec3 a_normal;
uniform mat4 u_viewProj;
uniform vec3 u_origin;
uniform vec2 u_half;
uniform float u_height;
varying vec3 v_worldPos;
varying vec3 v_normal;
void main() {
  // Scale the unit cube into world space: XY by per-marker half-extent,
  // Z by full box height (vertex z is 0..1 so this lifts the top face by
  // u_height exactly).
  vec3 worldPos = u_origin + vec3(a_position.xy * u_half, a_position.z * u_height);
  v_worldPos = worldPos;
  v_normal = a_normal;
  gl_Position = u_viewProj * vec4(worldPos, 1.0);
}
`.trim()

// Pink/black checker, alpha-blended. The checker is projected onto the plane
// perpendicular to the face's normal so all six faces show consistent square
// cells (no UV stretching, no per-face math mismatch). For the top/bottom
// (Z-axis normal) the cells come from XY; for the +X/-X faces from YZ; for
// +Y/-Y from XZ. Picking 2D coords by zeroing the largest |normal| component
// is a single dominant-axis compare without branching surprises.
//
// Lighting: half-Lambert dot of normal vs a fixed "sun" direction, biased
// upward so the top face reads brighter than the sides. This sells the box
// as a 3D form rather than a flat sticker.
//
// `u_selected` / `u_hover` wash the box white / cyan to mirror the white
// selection outline and cyan hover outline that MDX units & doodads get (a
// procedural box can't go through the silhouette-outline pass, so it carries
// its own highlight).
const FRAG_SHADER = `
precision mediump float;
uniform float u_selected;
uniform float u_hover;
varying vec3 v_worldPos;
varying vec3 v_normal;
void main() {
  // Project world position onto the plane perpendicular to this face's
  // normal: pick the two non-dominant axes by inspecting |normal|. Cheap
  // dominant-axis compare keeps the checker square on every face.
  vec3 an = abs(v_normal);
  vec2 uv;
  if (an.z > 0.5) {
    uv = v_worldPos.xy;
  } else if (an.x > 0.5) {
    uv = v_worldPos.yz;
  } else {
    uv = v_worldPos.xz;
  }
  vec2 cell = floor(uv / ${CHECKER_STUDS.toFixed(1)});
  float odd = mod(cell.x + cell.y, 2.0);
  vec3 pink = vec3(0.55, 0.20, 0.70);
  vec3 dark = vec3(0.02, 0.02, 0.02);
  vec3 col = mix(dark, pink, odd);
  // Half-Lambert lighting: hard-coded sun coming from +X +Y +Z, biased so
  // even back faces stay legible. Top face brightens noticeably, sides
  // darker — the silhouette reads as a box, not a flat sticker.
  vec3 n = normalize(v_normal);
  vec3 light = normalize(vec3(0.4, 0.4, 1.0));
  float diffuse = max(0.55, dot(n, light));
  col *= diffuse;
  // Extra brighten on the top face so the cap reads at any zoom.
  if (n.z > 0.5) col += vec3(0.08);
  // Selection / hover wash so picked or pointed-at blockers are obvious in
  // dense areas. Selected = white (matches the white selection outline),
  // hover = cyan (matches the cyan hover outline). A blocker is never both
  // (hover defers to selection upstream), so selection wins if ever both set.
  if (u_selected > 0.5) {
    col = mix(col, vec3(1.0, 1.0, 1.0), 0.6) + vec3(0.08);
  } else if (u_hover > 0.5) {
    col = mix(col, vec3(0.35, 0.95, 1.0), 0.55) + vec3(0.04);
  }
  gl_FragColor = vec4(col, 0.7);
}
`.trim()

function compileShader(gl: WebGLRenderingContext, type: number, src: string): WebGLShader {
  const sh = gl.createShader(type)!
  gl.shaderSource(sh, src)
  gl.compileShader(sh)
  if (!gl.getShaderParameter(sh, gl.COMPILE_STATUS)) {
    const log = gl.getShaderInfoLog(sh)
    gl.deleteShader(sh)
    throw new Error('path-blockers shader compile: ' + log)
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
    throw new Error('path-blockers program link: ' + log)
  }
  return {
    program,
    aPosition: gl.getAttribLocation(program, 'a_position'),
    aNormal: gl.getAttribLocation(program, 'a_normal'),
    uViewProj: gl.getUniformLocation(program, 'u_viewProj')!,
    uOrigin: gl.getUniformLocation(program, 'u_origin')!,
    uHalf: gl.getUniformLocation(program, 'u_half')!,
    uHeight: gl.getUniformLocation(program, 'u_height')!,
    uSelected: gl.getUniformLocation(program, 'u_selected')!,
    uHover: gl.getUniformLocation(program, 'u_hover')!,
  }
}

export function buildPathBlockerRenderer(gl: WebGLRenderingContext): PathBlockerRenderer | null {
  let prog: ReturnType<typeof buildProgram>
  try {
    prog = buildProgram(gl)
  } catch (e) {
    flog('[path-blockers] program build failed:', e instanceof Error ? e.message : String(e))
    return null
  }

  const vbo = gl.createBuffer()!
  gl.bindBuffer(gl.ARRAY_BUFFER, vbo)
  gl.bufferData(gl.ARRAY_BUFFER, BOX_VERTS, gl.STATIC_DRAW)

  const ibo = gl.createBuffer()!
  gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, ibo)
  gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, BOX_INDICES, gl.STATIC_DRAW)

  let markers: PathBlockerMarker[] = []

  return {
    setMarkers(ms: PathBlockerMarker[]) {
      markers = ms.slice()
    },

    pickInfos(): PathBlockerPickInfo[] {
      const out: PathBlockerPickInfo[] = []
      for (const m of markers) {
        // AABB centered vertically on the BOX so picks hit the silhouette,
        // not just the ground footprint. Box spans z+Z_LIFT..z+Z_LIFT+H, so
        // center sits at z + Z_LIFT + BOX_HALF_Z and Z half-extent is the
        // box's half-height.
        out.push({
          creationNumber: m.creationNumber,
          center: [m.position[0], m.position[1], m.position[2] + Z_LIFT + BOX_HALF_Z],
          half: [m.halfX, m.halfY, BOX_HALF_Z],
        })
      }
      return out
    },

    draw(viewProj: Float32Array, selected: Set<number>, hovered: number | null, preview: Set<number> | null) {
      if (markers.length === 0) return
      // Re-establish every bit of GL state we touch — the lib's render loop
      // leaves attribs / programs / blend state in unknown configuration.
      // Same playbook as sloc-markers + cell-highlight. The shader-cache
      // invalidation in scene-instances.ts covers the lib's side.
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
      gl.uniform1f(prog.uHeight, BOX_HEIGHT)
      // Alpha-blended marker: depth-test ON so terrain / cliffs / models can
      // occlude / be occluded correctly. Depth-write ON so blockers occlude
      // EACH OTHER — without this, a blocker behind another reads through
      // the front one (you "see" the far blocker's silhouette inside the
      // near one's, which looks like x-ray vision rather than two solid
      // markers in space). The end-of-pass `depthMask(true)` restore below
      // is now a no-op but kept for symmetry with the other state restores.
      gl.enable(gl.DEPTH_TEST)
      gl.depthFunc(gl.LEQUAL)
      gl.depthMask(true)
      // Back-face culling ON. The box is convex, so only the 3 camera-facing
      // faces ever rasterize — that's enough to silhouette the marker without
      // the user seeing the opposite-side back faces blending through the
      // front (which read as "this is hollow / wireframe-y" rather than "this
      // is a translucent solid"). Alpha blend with the scene behind the box
      // is unchanged.
      gl.enable(gl.CULL_FACE)
      gl.cullFace(gl.BACK)
      gl.frontFace(gl.CCW)
      gl.enable(gl.BLEND)
      gl.blendFunc(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA)

      for (const m of markers) {
        gl.uniform3f(
          prog.uOrigin,
          m.position[0],
          m.position[1],
          m.position[2] + Z_LIFT,
        )
        gl.uniform2f(prog.uHalf, m.halfX, m.halfY)
        gl.uniform1f(prog.uSelected, selected.has(m.creationNumber) ? 1 : 0)
        gl.uniform1f(prog.uHover, (hovered === m.creationNumber || preview?.has(m.creationNumber)) ? 1 : 0)
        gl.drawElements(gl.TRIANGLES, BOX_INDICES.length, gl.UNSIGNED_SHORT, 0)
      }

      // Don't leak attribs / state into the next pass. Restore depth-write
      // (a number of downstream passes assume depth-write is back on).
      gl.disableVertexAttribArray(prog.aPosition)
      gl.disableVertexAttribArray(prog.aNormal)
      gl.depthMask(true)
      gl.disable(gl.BLEND)
    },

    dispose() {
      gl.deleteBuffer(vbo)
      gl.deleteBuffer(ibo)
      gl.deleteProgram(prog.program)
      markers = []
    },
  }
}
