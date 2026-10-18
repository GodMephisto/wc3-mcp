---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/gizmo.ts"
ref: HEAD
---

// gizmo.ts — transform gizmo renderer + picker + drag math.
//
// Phase B = move arrows.
// Phase C adds:
//   - rotate ring (Z-axis only — X/Y rotations have no on-disk storage)
//   - scale cubes (3 axes; visual feedback is uniform via scale[0] since
//     the render path uses uniformScale, but the on-disk per-axis values
//     are written by ScaleUnit/ScaleDoodad)
//   - mode switching: 'move' | 'rotate' | 'scale' via setMode()
//   - new drag math for rotate (plane angle) and scale (signed-distance ratio)
//
// Architecture: stateless renderer overlay, same shape as sloc-markers.ts,
// cell-highlight.ts, and path-blockers.ts. Owns its own WebGL1 shader.
// Drawn AFTER all other overlays in the RAF loop in scene-instances.ts.
//
// CRITICAL — shader-cache invalidation (see feedback-mdx-viewer-shader-cache):
// Always set `(viewer as any).webgl.currentShader = null` BEFORE our draw call
// so the lib's cache doesn't get confused. We reset it again AFTER as well.
//
// State restoration discipline: mirror path-blockers.ts:
//   - save DEPTH_TEST, BLEND, CULL_FACE state BEFORE our draw
//   - restore ALL of them AFTER our draw, unconditionally
//   - depthMask must be restored to true
// A missed restore = next-frame rendering breaks silently.

import {
  MoveUnit, MoveDoodad,
  RotateUnit, RotateDoodad,
  ScaleUnit, ScaleDoodad,
  BeginUndoGroup, EndUndoGroup,
} from '../wailsjs/go/main/App.js'
import { flog } from './debuglog'

// ─── Axis colors (design §3.7) ────────────────────────────────────────────
const X_COLOR: [number, number, number] = [1.0, 0.2, 0.2]  // red
const Y_COLOR: [number, number, number] = [0.2, 1.0, 0.2]  // green
const Z_COLOR: [number, number, number] = [0.3, 0.4, 1.0]  // blue

// ─── Screen-space size (design §1.2) ──────────────────────────────────────
const SCREEN_SIZE_CONSTANT = 0.115

// ─── Pick zone inflation factor (design §1.3) ──────────────────────────────
const PICK_RADIUS_INFLATE = 1.5

// ─── Move-arrow geometry (Phase B, unchanged) ──────────────────────────────
const CYL_SEGS = 12
const CONE_SEGS = 12
// Thinner stem + slightly trimmer cone per user feedback ("make the move
// gizmo thinner"). Pick zones are still inflated via PICK_RADIUS_INFLATE
// so the hit area stays generous even though the visible geometry shrinks.
const CYL_RADIUS = 0.022
const CONE_RADIUS = 0.07
const CYL_TOP = 0.75
const CONE_BASE = 0.75
const CONE_TIP = 1.00

function buildCylinder(): Float32Array {
  const verts: number[] = []
  const addTri = (ax:number,ay:number,az:number, bx:number,by:number,bz:number, cx:number,cy:number,cz:number) => {
    verts.push(ax,ay,az, bx,by,bz, cx,cy,cz)
  }
  for (let i = 0; i < CYL_SEGS; i++) {
    const a = (i / CYL_SEGS) * Math.PI * 2
    const b = ((i + 1) / CYL_SEGS) * Math.PI * 2
    const ax = Math.cos(a) * CYL_RADIUS, ay = Math.sin(a) * CYL_RADIUS
    const bx = Math.cos(b) * CYL_RADIUS, by = Math.sin(b) * CYL_RADIUS
    addTri(ax,ay,0,        bx,by,0,        bx,by,CYL_TOP)
    addTri(ax,ay,0,        bx,by,CYL_TOP,  ax,ay,CYL_TOP)
    addTri(0,0,CYL_TOP,    ax,ay,CYL_TOP,  bx,by,CYL_TOP)
  }
  return new Float32Array(verts)
}

function buildCone(): Float32Array {
  const verts: number[] = []
  const addTri = (ax:number,ay:number,az:number, bx:number,by:number,bz:number, cx:number,cy:number,cz:number) => {
    verts.push(ax,ay,az, bx,by,bz, cx,cy,cz)
  }
  for (let i = 0; i < CONE_SEGS; i++) {
    const a = (i / CONE_SEGS) * Math.PI * 2
    const b = ((i + 1) / CONE_SEGS) * Math.PI * 2
    const ax = Math.cos(a) * CONE_RADIUS, ay = Math.sin(a) * CONE_RADIUS
    const bx = Math.cos(b) * CONE_RADIUS, by = Math.sin(b) * CONE_RADIUS
    addTri(ax,ay,CONE_BASE,  bx,by,CONE_BASE,  0,0,CONE_TIP)
    addTri(0,0,CONE_BASE,    bx,by,CONE_BASE,  ax,ay,CONE_BASE)
  }
  return new Float32Array(verts)
}

const CYL_VERTS = buildCylinder()
const CONE_VERTS = buildCone()

// ─── Rotate-ring geometry (Phase C) ────────────────────────────────────────
// Flat annulus in the XY plane (z=0). Drawn with the identity rotation
// matrix (ROT_Z) so the visible ring orbits the world-Z axis through the
// gizmo origin. Edge-on viewing makes a flat ring disappear, which is fine
// for a top-down RTS editor — the dominant camera angle keeps it visible.
const RING_SEGS = 64
const RING_R_INNER = 0.78
const RING_R_OUTER = 0.92
const RING_R_MID = (RING_R_INNER + RING_R_OUTER) / 2
const RING_R_HALF_WIDTH = (RING_R_OUTER - RING_R_INNER) / 2

function buildRing(): Float32Array {
  const verts: number[] = []
  const push = (x:number, y:number) => verts.push(x, y, 0)
  for (let i = 0; i < RING_SEGS; i++) {
    const a = (i / RING_SEGS) * Math.PI * 2
    const b = ((i + 1) / RING_SEGS) * Math.PI * 2
    const ax = Math.cos(a), ay = Math.sin(a)
    const bx = Math.cos(b), by = Math.sin(b)
    // tri 1: outer-a, inner-a, outer-b
    push(ax * RING_R_OUTER, ay * RING_R_OUTER)
    push(ax * RING_R_INNER, ay * RING_R_INNER)
    push(bx * RING_R_OUTER, by * RING_R_OUTER)
    // tri 2: outer-b, inner-a, inner-b
    push(bx * RING_R_OUTER, by * RING_R_OUTER)
    push(ax * RING_R_INNER, ay * RING_R_INNER)
    push(bx * RING_R_INNER, by * RING_R_INNER)
  }
  return new Float32Array(verts)
}

const RING_VERTS = buildRing()

// ─── Scale-cube geometry (Phase C) ─────────────────────────────────────────
// Cube at the tip of the cylinder stem.
const BOX_R = 0.08
const BOX_BASE_Z = CYL_TOP
const BOX_TIP_Z = CYL_TOP + 2 * BOX_R

// Face-handle (Roblox-style scale) geometry — a small unit cube CENTERED
// at the origin. Drawn 6 times per face, positioned at the entity's
// bounding-sphere face centers in world space. Uses the same handleScale
// uniform as everything else, so visually it stays a fixed screen size.
const FACE_BOX_LOCAL_R = 0.07
function buildFaceBox(): Float32Array {
  const verts: number[] = []
  const v = (x: number, y: number, z: number) => verts.push(x, y, z)
  const a = FACE_BOX_LOCAL_R
  v(-a, -a,  a); v( a, -a,  a); v( a,  a,  a)
  v(-a, -a,  a); v( a,  a,  a); v(-a,  a,  a)
  v(-a, -a, -a); v( a,  a, -a); v( a, -a, -a)
  v(-a, -a, -a); v(-a,  a, -a); v( a,  a, -a)
  v( a, -a, -a); v( a,  a, -a); v( a,  a,  a)
  v( a, -a, -a); v( a,  a,  a); v( a, -a,  a)
  v(-a, -a, -a); v(-a,  a,  a); v(-a,  a, -a)
  v(-a, -a, -a); v(-a, -a,  a); v(-a,  a,  a)
  v(-a,  a, -a); v(-a,  a,  a); v( a,  a,  a)
  v(-a,  a, -a); v( a,  a,  a); v( a,  a, -a)
  v(-a, -a, -a); v( a, -a,  a); v(-a, -a,  a)
  v(-a, -a, -a); v( a, -a, -a); v( a, -a,  a)
  return new Float32Array(verts)
}
const FACE_BOX_VERTS = buildFaceBox()

function buildCube(): Float32Array {
  const verts: number[] = []
  const v = (x:number, y:number, z:number) => verts.push(x, y, z)
  const xMin = -BOX_R, xMax = BOX_R
  const yMin = -BOX_R, yMax = BOX_R
  const zMin = BOX_BASE_Z, zMax = BOX_TIP_Z
  // +Z face
  v(xMin, yMin, zMax); v(xMax, yMin, zMax); v(xMax, yMax, zMax)
  v(xMin, yMin, zMax); v(xMax, yMax, zMax); v(xMin, yMax, zMax)
  // -Z face
  v(xMin, yMin, zMin); v(xMax, yMax, zMin); v(xMax, yMin, zMin)
  v(xMin, yMin, zMin); v(xMin, yMax, zMin); v(xMax, yMax, zMin)
  // +X face
  v(xMax, yMin, zMin); v(xMax, yMax, zMin); v(xMax, yMax, zMax)
  v(xMax, yMin, zMin); v(xMax, yMax, zMax); v(xMax, yMin, zMax)
  // -X face
  v(xMin, yMin, zMin); v(xMin, yMax, zMax); v(xMin, yMax, zMin)
  v(xMin, yMin, zMin); v(xMin, yMin, zMax); v(xMin, yMax, zMax)
  // +Y face
  v(xMin, yMax, zMin); v(xMin, yMax, zMax); v(xMax, yMax, zMax)
  v(xMin, yMax, zMin); v(xMax, yMax, zMax); v(xMax, yMax, zMin)
  // -Y face
  v(xMin, yMin, zMin); v(xMax, yMin, zMax); v(xMin, yMin, zMax)
  v(xMin, yMin, zMin); v(xMax, yMin, zMin); v(xMax, yMin, zMax)
  return new Float32Array(verts)
}

const CUBE_VERTS = buildCube()

// ─── Plane-handle geometry (Phase E) ───────────────────────────────────────
// Small quads at the corner between two axes for 2-axis simultaneous move.
// Each quad lies in the plane spanned by the two axes and is rendered with
// the identity rotation matrix (pre-rotated geometry in local space). Click
// fills the quad with a semi-transparent tinted patch; the user drags within
// the plane and both axes translate together.
const PLANE_LO = 0.18
const PLANE_HI = 0.40

// Y axis is rendered/dragged in -Y direction (see ROT_Y + AXIS_DIRS.y
// above). The XY and YZ plane patches must mirror their Y coords so the
// patches sit at the elbow between the X (resp. Z) arrow and the visible
// -Y arrow, not in the abandoned +Y quadrant.
function buildPlaneXY(): Float32Array {
  // Quad in z=0 plane, between +X arrow and -Y arrow.
  return new Float32Array([
    PLANE_LO, -PLANE_LO, 0,  PLANE_HI, -PLANE_LO, 0,  PLANE_HI, -PLANE_HI, 0,
    PLANE_LO, -PLANE_LO, 0,  PLANE_HI, -PLANE_HI, 0,  PLANE_LO, -PLANE_HI, 0,
  ])
}
function buildPlaneXZ(): Float32Array {
  // Quad in y=0 plane, between +X and +Z axes. No Y component → unaffected by the flip.
  return new Float32Array([
    PLANE_LO, 0, PLANE_LO,  PLANE_HI, 0, PLANE_LO,  PLANE_HI, 0, PLANE_HI,
    PLANE_LO, 0, PLANE_LO,  PLANE_HI, 0, PLANE_HI,  PLANE_LO, 0, PLANE_HI,
  ])
}
function buildPlaneYZ(): Float32Array {
  // Quad in x=0 plane, between -Y arrow and +Z axis.
  return new Float32Array([
    0, -PLANE_LO, PLANE_LO,  0, -PLANE_HI, PLANE_LO,  0, -PLANE_HI, PLANE_HI,
    0, -PLANE_LO, PLANE_LO,  0, -PLANE_HI, PLANE_HI,  0, -PLANE_LO, PLANE_HI,
  ])
}
const PLANE_XY_VERTS = buildPlaneXY()
const PLANE_XZ_VERTS = buildPlaneXZ()
const PLANE_YZ_VERTS = buildPlaneYZ()

// ─── Shader ───────────────────────────────────────────────────────────────
const VERT_SHADER = `
attribute vec3 a_position;
uniform mat4 u_viewProj;
uniform vec3 u_origin;
uniform float u_handleScale;
uniform vec3 u_rotRow0;
uniform vec3 u_rotRow1;
uniform vec3 u_rotRow2;
void main() {
  float x = dot(u_rotRow0, a_position);
  float y = dot(u_rotRow1, a_position);
  float z = dot(u_rotRow2, a_position);
  vec3 worldPos = u_origin + vec3(x, y, z) * u_handleScale;
  gl_Position = u_viewProj * vec4(worldPos, 1.0);
}
`.trim()

const FRAG_SHADER = `
precision mediump float;
uniform vec3 u_color;
uniform float u_hovered;
void main() {
  vec3 hoverCol = vec3(1.0, 0.95, 0.3);
  vec3 col = mix(u_color, hoverCol, u_hovered * 0.5);
  gl_FragColor = vec4(col, 1.0);
}
`.trim()

// ─── Rotation matrices (row-major 3x3, stored as three vec3 rows) ──────────
const ROT_X_R0 = new Float32Array([0, 0, 1])
const ROT_X_R1 = new Float32Array([0, 1, 0])
const ROT_X_R2 = new Float32Array([-1, 0, 0])
// Y axis flipped: arrow points in -Y direction (toward the camera in the
// default top-down view), which the user found easier to drag. The
// rotation matrix maps local +Z to world -Y; AXIS_DIRS.y is also flipped
// below so the drag math + pick math agree with the visible arrow.
const ROT_Y_R0 = new Float32Array([1, 0, 0])
const ROT_Y_R1 = new Float32Array([0, 0, -1])
const ROT_Y_R2 = new Float32Array([0, -1, 0])
const ROT_Z_R0 = new Float32Array([1, 0, 0])
const ROT_Z_R1 = new Float32Array([0, 1, 0])
const ROT_Z_R2 = new Float32Array([0, 0, 1])

// ─── Public types ──────────────────────────────────────────────────────────
// Single-axis handles ('x', 'y', 'z') for all 3 modes.
// Plane handles ('xy', 'xz', 'yz') for move mode only — 2-axis simultaneous
// translation. Defined here so move-mode's GizmoPickResult.axis can carry
// any of them.
export type GizmoAxis = 'x' | 'y' | 'z' | 'xy' | 'xz' | 'yz'
export type GizmoMode = 'move' | 'rotate' | 'scale'

const PLANE_AXES: GizmoAxis[] = ['xy', 'xz', 'yz']
function isPlaneAxis(a: GizmoAxis): a is 'xy' | 'xz' | 'yz' {
  return a === 'xy' || a === 'xz' || a === 'yz'
}
const PLANE_NORMAL: Record<'xy' | 'xz' | 'yz', [number, number, number]> = {
  xy: [0, 0, 1],  // plane normal for XY plane
  xz: [0, 1, 0],
  yz: [1, 0, 0],
}
// Plane fill colors — blend of the two axes' colors, less saturated so the
// handle reads as "secondary." A slight transparency would be ideal but our
// shader writes opaque colors only; the muted tint is the visual proxy.
const PLANE_COLOR: Record<'xy' | 'xz' | 'yz', [number, number, number]> = {
  xy: [0.85, 0.85, 0.30],  // red+green → yellow
  xz: [0.85, 0.30, 0.85],  // red+blue → magenta
  yz: [0.30, 0.85, 0.85],  // green+blue → cyan
}

export interface GizmoPickResult {
  axis: GizmoAxis | 'centroid'
  mode: GizmoMode
  // Scale mode only: which face of the bounding box was grabbed (+1 = +axis
  // face, -1 = -axis face). Undefined for centroid-fallback drag (multi-
  // select) and for non-scale modes. Used by the drag math to decide which
  // direction along the axis line means "grow."
  faceSign?: 1 | -1
}

// Single-axis records (no plane entries — plane handles use their own
// constants further up since the geometry is pre-rotated).
const AXIS_ROT: Record<'x' | 'y' | 'z', [Float32Array, Float32Array, Float32Array]> = {
  x: [ROT_X_R0, ROT_X_R1, ROT_X_R2],
  y: [ROT_Y_R0, ROT_Y_R1, ROT_Y_R2],
  z: [ROT_Z_R0, ROT_Z_R1, ROT_Z_R2],
}

const AXIS_COLOR: Record<'x' | 'y' | 'z', [number, number, number]> = {
  x: X_COLOR, y: Y_COLOR, z: Z_COLOR,
}

// Y axis direction is flipped to -Y so the drag math + pick zones agree
// with the visible arrow (which points in -Y per the flipped ROT_Y above).
// X and Z are conventional positive directions.
const AXIS_DIRS: Record<'x' | 'y' | 'z', [number,number,number]> = {
  x: [1, 0, 0],
  y: [0, -1, 0],
  z: [0, 0, 1],
}

interface EntityOrig {
  kind: 'unit' | 'doodad'
  cn: number
  posOrig: [number, number, number]
  rotOrig: number                 // Z-axis rotation in radians
  scaleOrig: [number, number, number]
  modelScale: number              // info.model_scale; for live-preview reconstruction with uniformScale
  inst: any
  moveHeight: number
}

interface MoveDrag {
  mode: 'move'
  axis: GizmoAxis
  origin: [number, number, number]
  entities: EntityOrig[]
  // For single-axis ('x'|'y'|'z'): signed distance along the axis line at mousedown.
  // For plane axes ('xy'|'xz'|'yz'): 3D world-space hit point on the plane at mousedown.
  anchorParam: number
  anchorPoint: [number, number, number]
}
interface RotateDrag {
  mode: 'rotate'
  axis: GizmoAxis
  origin: [number, number, number]
  entities: EntityOrig[]
  anchorAngle: number
  currentAngle: number
}
interface ScaleDrag {
  mode: 'scale'
  // 'x' | 'y' | 'z': which single axis the dragged face is on.
  // 'centroid': fallback for multi-select — uniform scale around centroid.
  axis: GizmoAxis | 'centroid'
  // Which face on that axis was grabbed: +1 = positive face, -1 = negative.
  // Both faces scale symmetrically around center; faceSign just decides which
  // direction "outward" is for the drag projection.
  faceSign: 1 | -1
  // Entity center (world coords) — projection target for the drag.
  origin: [number, number, number]
  entities: EntityOrig[]
  // |center → handle| at mousedown. Local (model-space) radius for single-
  // select Roblox-style handles, or centroid-distance for the multi-select
  // fallback. factor = currentHandleDist / origHandleDist.
  origHandleDist: number
  // Multi-select fallback only: 2D anchor for the cursor-delta math.
  anchorPx: number
  anchorPy: number
  screenAxisX: number
  screenAxisY: number
  // Per-axis live-preview scale factors. For single-axis face drag, only the
  // dragged axis changes; the others stay 1. For multi-select centroid drag,
  // all three change together (uniform).
  perAxisFactor: [number, number, number]
  // Local axis direction transformed to world space, captured at mousedown.
  // Face handles align with the entity's local frame, so dragging the +X
  // handle on a rotated entity scales the entity's LOCAL X (which renders
  // in whatever world direction the rotation pointed it). Drag projection
  // uses this direction so the cursor → factor math agrees with the visible
  // handle position regardless of entity rotation.
  axisWorldDir: [number, number, number]
}

export type GizmoDragState = MoveDrag | RotateDrag | ScaleDrag

export interface SelectionItem {
  kind: 'unit' | 'doodad'
  id: number
}

export interface UnitTypeInfo { move_height: number; model_scale?: number }

/**
 * Per-channel snap settings (design §3.2). Defaults are all OFF — freeform
 * editing is the v1 default. Snap value units:
 *   - move:   studs (game distance units; 128 studs = one terrain tile)
 *   - rotate: degrees (converted to radians internally)
 *   - scale:  multiplier (e.g. 0.1 rounds the factor to nearest 10%)
 *
 * Shift held during a drag INVERTS the channel's snap state for that drag
 * only (Blender convention). So Shift-drag with snap OFF snaps; Shift-drag
 * with snap ON is freeform. The invert is per-channel — switching modes
 * mid-session keeps each channel's preferred default intact.
 */
export interface SnapSettings {
  moveOn:   boolean; moveStep:   number   // studs
  rotateOn: boolean; rotateStep: number   // degrees
  scaleOn:  boolean; scaleStep:  number   // multiplier
}

export interface GizmoRenderer {
  /** Set the active mode. Cancels any in-progress drag. */
  setMode(mode: GizmoMode): void
  getMode(): GizmoMode
  /** Update snap settings. Pass partial — undefined keys keep current values. */
  setSnap(s: Partial<SnapSettings>): void
  getSnap(): SnapSettings

  draw(
    gl: WebGLRenderingContext,
    viewer: any,
    scene: any,
    canvas: HTMLCanvasElement,
    selectionItems: SelectionItem[],
    unitInstances: Map<number, any>,
    doodadInstances: Map<number, any>,
    eyePos: [number, number, number],
  ): void

  rayPick(
    px: number,
    py: number,
    scene: any,
    canvas: HTMLCanvasElement,
  ): GizmoPickResult | null

  beginDrag(
    pick: GizmoPickResult,
    px: number,
    py: number,
    scene: any,
    canvas: HTMLCanvasElement,
    selectionItems: SelectionItem[],
    unitInstances: Map<number, any>,
    doodadInstances: Map<number, any>,
    unitTypeIndexCache: Record<string, UnitTypeInfo> | null,
  ): void

  onDrag(
    px: number,
    py: number,
    scene: any,
    canvas: HTMLCanvasElement,
    shiftKey?: boolean,
  ): void

  onDragEnd(
    px: number,
    py: number,
    scene: any,
    canvas: HTMLCanvasElement,
    shiftKey?: boolean,
  ): void

  cancelDrag(): void
  isDragging(): boolean
  dragAxis(): GizmoAxis | 'centroid' | null
  dragMode(): GizmoMode | null
  /**
   * Returns the world-space arrow-tip positions for the X/Y/Z move arrows,
   * or null when the gizmo isn't currently visible in move mode. Callers
   * project these into screen space to position DOM overlay labels.
   * The Y arrow uses its flipped (-Y) direction so the label sits at the
   * visible cone tip.
   */
  getMoveArrowTips(): { x: [number, number, number]; y: [number, number, number]; z: [number, number, number] } | null
  dispose(): void
}

// ─── GL helpers ──────────────────────────────────────────────────────────
function compileShader(gl: WebGLRenderingContext, type: number, src: string): WebGLShader {
  const sh = gl.createShader(type)!
  gl.shaderSource(sh, src)
  gl.compileShader(sh)
  if (!gl.getShaderParameter(sh, gl.COMPILE_STATUS)) {
    const log = gl.getShaderInfoLog(sh)
    gl.deleteShader(sh)
    throw new Error('gizmo shader compile: ' + log)
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
    throw new Error('gizmo program link: ' + log)
  }
  return {
    program,
    aPosition: gl.getAttribLocation(program, 'a_position'),
    uViewProj: gl.getUniformLocation(program, 'u_viewProj')!,
    uOrigin: gl.getUniformLocation(program, 'u_origin')!,
    uHandleScale: gl.getUniformLocation(program, 'u_handleScale')!,
    uRotRow0: gl.getUniformLocation(program, 'u_rotRow0')!,
    uRotRow1: gl.getUniformLocation(program, 'u_rotRow1')!,
    uRotRow2: gl.getUniformLocation(program, 'u_rotRow2')!,
    uColor: gl.getUniformLocation(program, 'u_color')!,
    uHovered: gl.getUniformLocation(program, 'u_hovered')!,
  }
}

// ─── Ray helpers ──────────────────────────────────────────────────────────
function screenRay(px: number, py: number, scene: any, _canvas: HTMLCanvasElement): Float32Array | null {
  const vp = (scene as any).viewport as Float32Array
  if (!vp) return null
  const out = new Float32Array(6)
  const screen = new Float32Array([px, py])
  scene.camera.screenToWorldRay(out, screen, vp)
  const dx = out[3] - out[0], dy = out[4] - out[1], dz = out[5] - out[2]
  const len = Math.hypot(dx, dy, dz)
  if (len < 1e-10) return null
  return new Float32Array([out[0], out[1], out[2], dx/len, dy/len, dz/len])
}

function rayLineNearest(
  rox: number, roy: number, roz: number,
  rdx: number, rdy: number, rdz: number,
  lox: number, loy: number, loz: number,
  ldx: number, ldy: number, ldz: number,
): { s: number; t: number } | null {
  const wx = rox - lox, wy = roy - loy, wz = roz - loz
  const b = rdx*ldx + rdy*ldy + rdz*ldz
  const d = rdx*rdx + rdy*rdy + rdz*rdz
  const e = ldx*ldx + ldy*ldy + ldz*ldz
  const denom = d * e - b * b
  if (Math.abs(denom) < 1e-10) return null
  const c = rdx*wx + rdy*wy + rdz*wz
  const f = ldx*wx + ldy*wy + ldz*wz
  const sc = (b*f - e*c) / denom
  const tc = (d*f - b*c) / denom
  return { s: tc, t: sc }
}

/** Ray-plane intersection. Returns t (ray parameter) + hit point, or null. */
function rayPlane(
  rox: number, roy: number, roz: number,
  rdx: number, rdy: number, rdz: number,
  px: number, py: number, pz: number,
  nx: number, ny: number, nz: number,
): { t: number; hx: number; hy: number; hz: number } | null {
  const denom = rdx*nx + rdy*ny + rdz*nz
  if (Math.abs(denom) < 1e-10) return null
  const t = ((px - rox)*nx + (py - roy)*ny + (pz - roz)*nz) / denom
  if (t < 0) return null
  return { t, hx: rox + rdx * t, hy: roy + rdy * t, hz: roz + rdz * t }
}

function rayCylinder(
  rox: number, roy: number, roz: number,
  rdx: number, rdy: number, rdz: number,
  cox: number, coy: number, coz: number,
  cax: number, cay: number, caz: number,
  radius: number,
  minT: number,
  maxT: number,
): number | null {
  const wx = rox - cox, wy = roy - coy, wz = roz - coz
  const wdotA = wx*cax + wy*cay + wz*caz
  const wpx = wx - wdotA*cax, wpy = wy - wdotA*cay, wpz = wz - wdotA*caz
  const ddotA = rdx*cax + rdy*cay + rdz*caz
  const dpx = rdx - ddotA*cax, dpy = rdy - ddotA*cay, dpz = rdz - ddotA*caz
  const a = dpx*dpx + dpy*dpy + dpz*dpz
  if (a < 1e-10) return null
  const b2 = wpx*dpx + wpy*dpy + wpz*dpz
  const c = wpx*wpx + wpy*wpy + wpz*wpz - radius*radius
  const disc = b2*b2 - a*c
  if (disc < 0) return null
  const sq = Math.sqrt(disc)
  let t = (-b2 - sq) / a
  if (t < 0) t = (-b2 + sq) / a
  if (t < 0) return null
  const hitAx = wdotA + t * ddotA
  if (hitAx < minT || hitAx > maxT) return null
  return t
}

/**
 * Ray vs sphere. Returns the nearest positive t (front-face hit) or null
 * if no intersection. Used by face-handle picking.
 */
function raySphere(
  rox: number, roy: number, roz: number,
  rdx: number, rdy: number, rdz: number,
  cx: number, cy: number, cz: number,
  radius: number,
): number | null {
  const ox = rox - cx, oy = roy - cy, oz = roz - cz
  const b = ox*rdx + oy*rdy + oz*rdz
  const c = ox*ox + oy*oy + oz*oz - radius*radius
  const disc = b*b - c
  if (disc < 0) return null
  const sq = Math.sqrt(disc)
  let t = -b - sq
  if (t < 0) t = -b + sq
  if (t < 0) return null
  return t
}

/** Build a unit-axis Z-rotation quaternion [x, y, z, w]. */
function quatZ(angle: number): [number, number, number, number] {
  const h = angle / 2
  return [0, 0, Math.sin(h), Math.cos(h)]
}

/**
 * Rotate vec3 v by quaternion q ([x, y, z, w]). Used to transform an
 * entity's local axes into world space so the Roblox-style scale face
 * handles sit on the entity's actual faces (not on world axes), and so
 * the drag math projects the cursor onto the LOCAL axis direction in
 * world space — matching how inst.setScale operates in local space.
 *
 * Formula: vr = v + 2 * cross(q.xyz, cross(q.xyz, v) + q.w * v)
 */
function rotateVecByQuat(v: [number, number, number], q: ArrayLike<number>): [number, number, number] {
  const vx = v[0], vy = v[1], vz = v[2]
  const qx = q[0], qy = q[1], qz = q[2], qw = q[3]
  // t = 2 * cross(q.xyz, v)
  const tx = 2 * (qy * vz - qz * vy)
  const ty = 2 * (qz * vx - qx * vz)
  const tz = 2 * (qx * vy - qy * vx)
  // v + qw * t + cross(q.xyz, t)
  return [
    vx + qw * tx + (qy * tz - qz * ty),
    vy + qw * ty + (qz * tx - qx * tz),
    vz + qw * tz + (qx * ty - qy * tx),
  ]
}

/** Normalize an angle to (-π, π]. */
function wrapAngle(a: number): number {
  while (a >  Math.PI) a -= 2 * Math.PI
  while (a <= -Math.PI) a += 2 * Math.PI
  return a
}

/** Round to nearest multiple of `step`. Returns `v` unchanged when step<=0. */
function snap(v: number, step: number): number {
  if (!isFinite(step) || step <= 0) return v
  return Math.round(v / step) * step
}

/**
 * World-space bounds of one entity instance. Returns the world-space center
 * of the model's bounding sphere + the LOCAL (unscaled) sphere radius and
 * the current per-axis scales — the gizmo's face-handle code needs these
 * separately so it can position each handle at center + axis_unit * r_local
 * * scale[axis] (i.e. honoring per-axis stretching).
 *
 * Returns null when the instance has no MDX bounds (e.g. sloc pillars,
 * doodads whose model failed to load) — caller should fall back to the
 * centroid+arrow-cube path used by multi-select.
 */
function instanceBounds(inst: any): { cx: number; cy: number; cz: number; r: number; sx: number; sy: number; sz: number } | null {
  const b = inst?.getBounds?.()
  if (!b || b.r <= 0) return null
  const sx = inst.worldScale?.[0] ?? 1
  const sy = inst.worldScale?.[1] ?? 1
  const sz = inst.worldScale?.[2] ?? 1
  // The bounds offset (b.x/y/z) is in LOCAL space. To get the world-space
  // sphere center it has to be SCALED by the entity's worldScale then
  // ROTATED by the entity's worldRotation, THEN translated by worldLocation.
  // Without the rotation step, a rotated entity's bounds offset stays on
  // the world axes — so a banner doodad with bounds offset (30, 0, 50)
  // and a 90° yaw rotation would have its scale gizmo offset by world +X
  // when it should be offset by world +Y.
  const wr = inst.worldRotation ?? [0, 0, 0, 1]
  const localOffset: [number, number, number] = [b.x * sx, b.y * sy, b.z * sz]
  const worldOffset = rotateVecByQuat(localOffset, wr)
  return {
    cx: inst.worldLocation[0] + worldOffset[0],
    cy: inst.worldLocation[1] + worldOffset[1],
    cz: inst.worldLocation[2] + worldOffset[2],
    r: b.r, sx, sy, sz,
  }
}

/**
 * Project a world point through the camera's view-projection matrix into
 * canvas-pixel space (DOM-Y, origin top-left). Returns null if the point
 * is behind the camera (w<=0). Used by the screen-space scale-drag math.
 *
 * Mirrors scene-instances.ts::worldToCanvasPx — kept private to gizmo.ts
 * to avoid taking a public API dependency on the SceneAPI surface.
 */
function worldToScreen(scene: any, canvas: HTMLCanvasElement, world: [number, number, number]): { x: number; y: number } | null {
  const m = scene.camera.viewProjectionMatrix as Float32Array
  const wx = world[0], wy = world[1], wz = world[2]
  const x = m[0]*wx + m[4]*wy + m[8]*wz + m[12]
  const y = m[1]*wx + m[5]*wy + m[9]*wz + m[13]
  const w = m[3]*wx + m[7]*wy + m[11]*wz + m[15]
  if (w <= 0) return null
  const nx = x / w, ny = y / w
  const sx = (nx * 0.5 + 0.5) * canvas.clientWidth
  const syBottom = (ny * 0.5 + 0.5) * canvas.clientHeight
  return { x: sx, y: canvas.clientHeight - syBottom }
}

// ─── Main builder ──────────────────────────────────────────────────────────
export function buildGizmo(gl: WebGLRenderingContext): GizmoRenderer | null {
  let prog: ReturnType<typeof buildProgram>
  try {
    prog = buildProgram(gl)
  } catch (e) {
    flog('[gizmo] program build failed:', e instanceof Error ? e.message : String(e))
    return null
  }

  const cylVBO = gl.createBuffer()!
  gl.bindBuffer(gl.ARRAY_BUFFER, cylVBO)
  gl.bufferData(gl.ARRAY_BUFFER, CYL_VERTS, gl.STATIC_DRAW)

  const coneVBO = gl.createBuffer()!
  gl.bindBuffer(gl.ARRAY_BUFFER, coneVBO)
  gl.bufferData(gl.ARRAY_BUFFER, CONE_VERTS, gl.STATIC_DRAW)

  const ringVBO = gl.createBuffer()!
  gl.bindBuffer(gl.ARRAY_BUFFER, ringVBO)
  gl.bufferData(gl.ARRAY_BUFFER, RING_VERTS, gl.STATIC_DRAW)

  const cubeVBO = gl.createBuffer()!
  gl.bindBuffer(gl.ARRAY_BUFFER, cubeVBO)
  gl.bufferData(gl.ARRAY_BUFFER, CUBE_VERTS, gl.STATIC_DRAW)

  const planeXYVBO = gl.createBuffer()!
  gl.bindBuffer(gl.ARRAY_BUFFER, planeXYVBO)
  gl.bufferData(gl.ARRAY_BUFFER, PLANE_XY_VERTS, gl.STATIC_DRAW)

  const planeXZVBO = gl.createBuffer()!
  gl.bindBuffer(gl.ARRAY_BUFFER, planeXZVBO)
  gl.bufferData(gl.ARRAY_BUFFER, PLANE_XZ_VERTS, gl.STATIC_DRAW)

  const planeYZVBO = gl.createBuffer()!
  gl.bindBuffer(gl.ARRAY_BUFFER, planeYZVBO)
  gl.bufferData(gl.ARRAY_BUFFER, PLANE_YZ_VERTS, gl.STATIC_DRAW)

  // Face-handle cube (Phase: roblox-scale). Centered at origin in local
  // space; drawn 6 times per entity at the bounding-sphere face centers.
  const faceBoxVBO = gl.createBuffer()!
  gl.bindBuffer(gl.ARRAY_BUFFER, faceBoxVBO)
  gl.bufferData(gl.ARRAY_BUFFER, FACE_BOX_VERTS, gl.STATIC_DRAW)

  gl.bindBuffer(gl.ARRAY_BUFFER, null)
  const PLANE_VBO: Record<'xy' | 'xz' | 'yz', WebGLBuffer> = {
    xy: planeXYVBO, xz: planeXZVBO, yz: planeYZVBO,
  }

  let mode: GizmoMode = 'move'
  // Per-channel snap settings. Defaults all OFF (freeform), per design §3.2
  // user-preference note. Step sizes use the same units the user picks in
  // the UI (move=studs, rotate=degrees, scale=multiplier).
  const snapSettings: SnapSettings = {
    moveOn: false,   moveStep: 32,    // 1/4 tile by default if enabled
    rotateOn: false, rotateStep: 15,  // 15° by default if enabled
    scaleOn: false,  scaleStep: 0.1,  // 10% steps by default if enabled
  }
  let lastOrigin: [number, number, number] = [0, 0, 0]
  let lastHandleScale: number = 1
  let lastVisible: boolean = false
  let hoverAxis: GizmoAxis | null = null
  let dragState: GizmoDragState | null = null
  // Face-handle positions for single-select scale mode. Cached by draw,
  // read by pick. Null when not in scale mode or the entity has no bounds
  // (e.g. sloc), in which case the centroid+arrow-cube fallback applies.
  let lastFacePositions: {
    xp: [number, number, number]
    xn: [number, number, number]
    yp: [number, number, number]
    yn: [number, number, number]
    zp: [number, number, number]
    zn: [number, number, number]
  } | null = null
  // Local (unscaled) bounding-sphere radius captured by draw for use in
  // pick + drag math. Null when no single-select bounds available.
  let lastFaceRadius: number = 0
  // Center of the single-selected entity's bounding sphere (world coords)
  // captured by draw, used for the drag-axis projection.
  let lastFaceCenter: [number, number, number] = [0, 0, 0]

  function computeOrigin(
    selectionItems: SelectionItem[],
    unitInstances: Map<number, any>,
    doodadInstances: Map<number, any>,
  ): [number, number, number] | null {
    if (selectionItems.length === 0) return null
    let sx = 0, sy = 0, sz = 0, cnt = 0
    for (const item of selectionItems) {
      const inst = item.kind === 'unit' ? unitInstances.get(item.id) : doodadInstances.get(item.id)
      if (!inst) continue
      const wl = inst.worldLocation
      if (!wl) continue
      sx += wl[0]; sy += wl[1]; sz += wl[2]; cnt++
    }
    if (cnt === 0) return null
    return [sx/cnt, sy/cnt, sz/cnt]
  }

  function bindVBO(vbo: WebGLBuffer) {
    gl.bindBuffer(gl.ARRAY_BUFFER, vbo)
    gl.vertexAttribPointer(prog.aPosition, 3, gl.FLOAT, false, 12, 0)
  }

  function setAxisUniforms(
    origin: [number, number, number],
    handleScale: number,
    r0: Float32Array, r1: Float32Array, r2: Float32Array,
    color: [number, number, number],
    hovered: boolean,
    viewProj: Float32Array,
  ) {
    gl.uniform3f(prog.uOrigin, origin[0], origin[1], origin[2])
    gl.uniform1f(prog.uHandleScale, handleScale)
    gl.uniform3fv(prog.uRotRow0, r0)
    gl.uniform3fv(prog.uRotRow1, r1)
    gl.uniform3fv(prog.uRotRow2, r2)
    gl.uniform3f(prog.uColor, color[0], color[1], color[2])
    gl.uniform1f(prog.uHovered, hovered ? 1.0 : 0.0)
    gl.uniformMatrix4fv(prog.uViewProj, false, viewProj)
  }

  function drawMoveArrow(
    viewProj: Float32Array,
    origin: [number, number, number], scale: number,
    axis: GizmoAxis, hovered: boolean,
  ) {
    const [r0, r1, r2] = AXIS_ROT[axis]
    setAxisUniforms(origin, scale, r0, r1, r2, AXIS_COLOR[axis], hovered, viewProj)
    bindVBO(cylVBO);  gl.drawArrays(gl.TRIANGLES, 0, CYL_VERTS.length / 3)
    bindVBO(coneVBO); gl.drawArrays(gl.TRIANGLES, 0, CONE_VERTS.length / 3)
  }

  function drawScaleHandle(
    viewProj: Float32Array,
    origin: [number, number, number], scale: number,
    axis: GizmoAxis, hovered: boolean,
  ) {
    const [r0, r1, r2] = AXIS_ROT[axis]
    setAxisUniforms(origin, scale, r0, r1, r2, AXIS_COLOR[axis], hovered, viewProj)
    bindVBO(cylVBO);  gl.drawArrays(gl.TRIANGLES, 0, CYL_VERTS.length / 3)
    bindVBO(cubeVBO); gl.drawArrays(gl.TRIANGLES, 0, CUBE_VERTS.length / 3)
  }

  function drawRotateRing(
    viewProj: Float32Array,
    origin: [number, number, number], scale: number,
    axis: GizmoAxis, hovered: boolean,
  ) {
    // Z ring uses identity rotation. (Future X/Y rings would use AXIS_ROT[axis]
    // but they have no on-disk destination, so we don't draw them.)
    const [r0, r1, r2] = AXIS_ROT[axis]
    setAxisUniforms(origin, scale, r0, r1, r2, AXIS_COLOR[axis], hovered, viewProj)
    bindVBO(ringVBO); gl.drawArrays(gl.TRIANGLES, 0, RING_VERTS.length / 3)
  }

  function drawFaceHandle(
    viewProj: Float32Array,
    facePos: [number, number, number], scale: number,
    axisColor: [number, number, number], hovered: boolean,
  ) {
    // Face-handle cube — centered at the face position, identity rotation
    // since the geometry is symmetric. Sized via handleScale for fixed
    // screen size like every other gizmo handle.
    const [r0, r1, r2] = AXIS_ROT['z']
    setAxisUniforms(facePos, scale, r0, r1, r2, axisColor, hovered, viewProj)
    bindVBO(faceBoxVBO)
    gl.drawArrays(gl.TRIANGLES, 0, FACE_BOX_VERTS.length / 3)
  }

  function drawPlaneHandle(
    viewProj: Float32Array,
    origin: [number, number, number], scale: number,
    plane: 'xy' | 'xz' | 'yz', hovered: boolean,
  ) {
    // Plane geometry is pre-rotated into world axes, so use identity rotation.
    const [r0, r1, r2] = AXIS_ROT['z']
    setAxisUniforms(origin, scale, r0, r1, r2, PLANE_COLOR[plane], hovered, viewProj)
    bindVBO(PLANE_VBO[plane])
    gl.drawArrays(gl.TRIANGLES, 0, 6)
  }

  function pickMove(
    rox: number, roy: number, roz: number, rdx: number, rdy: number, rdz: number,
    origin: [number, number, number], scale: number,
  ): GizmoAxis | null {
    let bestT = Infinity
    let bestAxis: GizmoAxis | null = null
    // Plane handles FIRST (they sit between the axis arrows and overlap them
    // visually; testing planes first gives them ergonomic priority when a
    // click lands in the overlap zone).
    for (const plane of PLANE_AXES) {
      const p = plane as 'xy' | 'xz' | 'yz'
      const [nx, ny, nz] = PLANE_NORMAL[p]
      const hit = rayPlane(
        rox, roy, roz, rdx, rdy, rdz,
        origin[0], origin[1], origin[2], nx, ny, nz,
      )
      if (!hit) continue
      // Convert hit to local space relative to origin (no rotation since
      // plane geometry is identity-rotated; verts are in world-axis frame).
      const lx = hit.hx - origin[0], ly = hit.hy - origin[1], lz = hit.hz - origin[2]
      // Check the two axes in the plane are both within [PLANE_LO, PLANE_HI]*scale.
      const lo = PLANE_LO * scale, hi = PLANE_HI * scale
      let inPlane = false
      // Y bounds are negated because the Y axis is rendered/dragged in -Y
      // (the patches sit in the -Y quadrant; see buildPlaneXY / buildPlaneYZ).
      if (p === 'xy') inPlane = lx >= lo && lx <= hi && ly <= -lo && ly >= -hi
      if (p === 'xz') inPlane = lx >= lo && lx <= hi && lz >= lo && lz <= hi
      if (p === 'yz') inPlane = ly <= -lo && ly >= -hi && lz >= lo && lz <= hi
      if (!inPlane) continue
      if (hit.t > 0 && hit.t < bestT) { bestT = hit.t; bestAxis = plane }
    }
    // Then the single-axis arrows.
    for (const axis of ['x', 'y', 'z'] as GizmoAxis[]) {
      const [adx, ady, adz] = AXIS_DIRS[axis]
      const cylR = CYL_RADIUS * scale * PICK_RADIUS_INFLATE
      const cylT = rayCylinder(
        rox, roy, roz, rdx, rdy, rdz,
        origin[0], origin[1], origin[2],
        adx, ady, adz, cylR, 0, CYL_TOP * scale,
      )
      if (cylT !== null && cylT > 0 && cylT < bestT) { bestT = cylT; bestAxis = axis }
      const coneBaseX = origin[0] + adx * CONE_BASE * scale
      const coneBaseY = origin[1] + ady * CONE_BASE * scale
      const coneBaseZ = origin[2] + adz * CONE_BASE * scale
      const coneR = CONE_RADIUS * scale * PICK_RADIUS_INFLATE
      const coneLen = (CONE_TIP - CONE_BASE) * scale
      const coneT = rayCylinder(
        rox, roy, roz, rdx, rdy, rdz,
        coneBaseX, coneBaseY, coneBaseZ,
        adx, ady, adz, coneR, 0, coneLen,
      )
      if (coneT !== null && coneT > 0 && coneT < bestT) { bestT = coneT; bestAxis = axis }
    }
    return bestAxis
  }

  function pickScale(
    rox: number, roy: number, roz: number, rdx: number, rdy: number, rdz: number,
    origin: [number, number, number], scale: number,
  ): { axis: 'x' | 'y' | 'z' | 'centroid'; faceSign?: 1 | -1 } | null {
    // Face-handle pick when single-select gave us bounds. Direct-manipulation
    // mode: pick the closest face cube. Falls through to the centroid+arrow
    // pick when no face positions were cached (multi-select or bounds-less
    // entity like a sloc).
    if (lastFacePositions) {
      const pickR = FACE_BOX_LOCAL_R * scale * PICK_RADIUS_INFLATE * 1.5
      let bestT = Infinity
      let bestAxis: 'x' | 'y' | 'z' | null = null
      let bestSign: 1 | -1 = 1
      const tryFace = (axis: 'x' | 'y' | 'z', sign: 1 | -1, pos: [number, number, number]) => {
        const t = raySphere(rox, roy, roz, rdx, rdy, rdz, pos[0], pos[1], pos[2], pickR)
        if (t !== null && t > 0 && t < bestT) { bestT = t; bestAxis = axis; bestSign = sign }
      }
      tryFace('x', +1, lastFacePositions.xp)
      tryFace('x', -1, lastFacePositions.xn)
      tryFace('y', +1, lastFacePositions.yp)
      tryFace('y', -1, lastFacePositions.yn)
      tryFace('z', +1, lastFacePositions.zp)
      tryFace('z', -1, lastFacePositions.zn)
      if (bestAxis !== null) return { axis: bestAxis, faceSign: bestSign }
      return null
    }
    // Centroid fallback (multi-select etc.): the old per-axis cubes at the
    // gizmo origin. faceSign omitted — outer dispatch treats this as the
    // uniform-around-centroid drag.
    let bestT = Infinity
    let bestAxis: 'x' | 'y' | 'z' | null = null
    for (const axis of ['x', 'y', 'z'] as Array<'x' | 'y' | 'z'>) {
      const [adx, ady, adz] = AXIS_DIRS[axis]
      const cylR = CYL_RADIUS * scale * PICK_RADIUS_INFLATE
      const cylT = rayCylinder(
        rox, roy, roz, rdx, rdy, rdz,
        origin[0], origin[1], origin[2],
        adx, ady, adz, cylR, 0, CYL_TOP * scale,
      )
      if (cylT !== null && cylT > 0 && cylT < bestT) { bestT = cylT; bestAxis = axis }
      const cubeCenterT = (BOX_BASE_Z + BOX_TIP_Z) / 2
      const cubeBaseX = origin[0] + adx * (cubeCenterT - (BOX_TIP_Z - BOX_BASE_Z)/2) * scale
      const cubeBaseY = origin[1] + ady * (cubeCenterT - (BOX_TIP_Z - BOX_BASE_Z)/2) * scale
      const cubeBaseZ = origin[2] + adz * (cubeCenterT - (BOX_TIP_Z - BOX_BASE_Z)/2) * scale
      const cubeR = BOX_R * scale * PICK_RADIUS_INFLATE * 1.2
      const cubeLen = (BOX_TIP_Z - BOX_BASE_Z) * scale
      const cubeT = rayCylinder(
        rox, roy, roz, rdx, rdy, rdz,
        cubeBaseX, cubeBaseY, cubeBaseZ,
        adx, ady, adz, cubeR, 0, cubeLen,
      )
      if (cubeT !== null && cubeT > 0 && cubeT < bestT) { bestT = cubeT; bestAxis = axis }
    }
    return bestAxis !== null ? { axis: bestAxis } : null
  }

  function pickRotate(
    rox: number, roy: number, roz: number, rdx: number, rdy: number, rdz: number,
    origin: [number, number, number], scale: number,
  ): GizmoAxis | null {
    const hit = rayPlane(
      rox, roy, roz, rdx, rdy, rdz,
      origin[0], origin[1], origin[2],
      0, 0, 1,
    )
    if (!hit) return null
    const dx = hit.hx - origin[0]
    const dy = hit.hy - origin[1]
    const r = Math.hypot(dx, dy)
    const minR = (RING_R_MID - RING_R_HALF_WIDTH * PICK_RADIUS_INFLATE) * scale
    const maxR = (RING_R_MID + RING_R_HALF_WIDTH * PICK_RADIUS_INFLATE) * scale
    if (r < minR || r > maxR) return null
    return 'z'
  }

  function restoreEntity(ent: EntityOrig) {
    const inst = ent.inst as any
    inst.setLocation([ent.posOrig[0], ent.posOrig[1], ent.posOrig[2]])
    if (typeof inst.setRotation === 'function') {
      inst.setRotation(quatZ(ent.rotOrig))
    }
    // Per-axis restore so a cancel after a Roblox-style per-axis scale
    // drag truly reverts to original (uniformScale here would clobber Y/Z
    // back to scaleOrig[0]).
    const ms = ent.modelScale
    if (typeof inst.setScale === 'function') {
      inst.setScale([
        ent.scaleOrig[0] * ms,
        ent.scaleOrig[1] * ms,
        ent.scaleOrig[2] * ms,
      ])
    } else if (typeof inst.uniformScale === 'function') {
      inst.uniformScale(ent.scaleOrig[0] * ms)
    }
  }

  return {
    setMode(m: GizmoMode) {
      if (m === mode) return
      // Cancel any active drag on mode switch.
      if (dragState) {
        for (const ent of dragState.entities) restoreEntity(ent)
        dragState = null
      }
      mode = m
      hoverAxis = null
    },

    getMode(): GizmoMode { return mode },

    setSnap(s: Partial<SnapSettings>) {
      if (s.moveOn   !== undefined) snapSettings.moveOn   = s.moveOn
      if (s.moveStep !== undefined && s.moveStep > 0) snapSettings.moveStep = s.moveStep
      if (s.rotateOn !== undefined) snapSettings.rotateOn = s.rotateOn
      if (s.rotateStep !== undefined && s.rotateStep > 0) snapSettings.rotateStep = s.rotateStep
      if (s.scaleOn  !== undefined) snapSettings.scaleOn  = s.scaleOn
      if (s.scaleStep !== undefined && s.scaleStep > 0) snapSettings.scaleStep = s.scaleStep
    },
    getSnap(): SnapSettings { return { ...snapSettings } },

    draw(
      _gl, viewer, scene, _canvas,
      selectionItems, unitInstances, doodadInstances,
      eyePos,
    ) {
      // Always recompute the visible origin from the CURRENT entity positions.
      // For move drags this makes the gizmo track the entities as they move
      // (otherwise it would stick to the mousedown snapshot — the bug the
      // user filed). For rotate/scale drags the centroid is mathematically
      // preserved (rotation around centroid keeps the mean position fixed;
      // axis-scale from centroid keeps it fixed too), so always-recompute
      // gives the same result as the snapshot would for those modes.
      //
      // dragState.origin remains the PIVOT for the drag math (orbit center
      // for rotate, scale center for scale, anchor reference for move) —
      // that's separate from the visible position. Render position is
      // recomputed every frame; drag math uses the mousedown snapshot.
      const origin = computeOrigin(selectionItems, unitInstances, doodadInstances)
      lastVisible = origin !== null
      if (!origin) return
      lastOrigin = origin

      const dx = eyePos[0] - origin[0]
      const dy = eyePos[1] - origin[1]
      const dz = eyePos[2] - origin[2]
      const dist = Math.hypot(dx, dy, dz) || 6000
      const handleScale = Math.max(10, dist * SCREEN_SIZE_CONSTANT)
      lastHandleScale = handleScale

      const wasDepthTest = gl.isEnabled(gl.DEPTH_TEST)
      const wasBlend = gl.isEnabled(gl.BLEND)
      const wasCull = gl.isEnabled(gl.CULL_FACE)

      ;(viewer as any).webgl.currentShader = null
      gl.useProgram(prog.program)

      const maxAttribs = gl.getParameter(gl.MAX_VERTEX_ATTRIBS) | 0
      for (let i = 0; i < maxAttribs; i++) gl.disableVertexAttribArray(i)
      gl.enableVertexAttribArray(prog.aPosition)

      gl.disable(gl.DEPTH_TEST)
      gl.disable(gl.BLEND)
      gl.disable(gl.CULL_FACE)

      const vp = scene.camera.viewProjectionMatrix as Float32Array
      const activeAxis = dragState?.axis ?? hoverAxis

      try {
        if (mode === 'move') {
          drawMoveArrow(vp, origin, handleScale, 'x', activeAxis === 'x')
          drawMoveArrow(vp, origin, handleScale, 'y', activeAxis === 'y')
          drawMoveArrow(vp, origin, handleScale, 'z', activeAxis === 'z')
          drawPlaneHandle(vp, origin, handleScale, 'xy', activeAxis === 'xy')
          drawPlaneHandle(vp, origin, handleScale, 'xz', activeAxis === 'xz')
          drawPlaneHandle(vp, origin, handleScale, 'yz', activeAxis === 'yz')
        } else if (mode === 'scale') {
          // Roblox-style: 6 face handles around the entity's bounding sphere,
          // each tracking the current per-axis scale so the handle follows
          // the cursor as the model grows. Single-select only — multi-select
          // falls back to the centroid+arrow-cube path below.
          let usedFaceHandles = false
          if (selectionItems.length === 1) {
            const item = selectionItems[0]
            const inst = item.kind === 'unit' ? unitInstances.get(item.id) : doodadInstances.get(item.id)
            const wb = inst ? instanceBounds(inst) : null
            if (wb) {
              // Transform the entity's local axes into world space so face
              // handles align with the entity's own frame (Roblox style).
              // Without this, a 90°-rotated doodad shows red on world +X
              // but dragging scales LOCAL +X which renders as world +Y —
              // the "scaled on opposite axis" bug.
              // Prefer the lib's worldRotation when present (includes any
              // composed pitch/roll for doodads); fall back to the stashed
              // yaw-only rotation when not.
              const instAny = inst as any
              const wr: ArrayLike<number> = instAny.worldRotation
                ?? quatZ(instAny.__wc3ForgeRotation ?? 0)
              const lxw = rotateVecByQuat([1, 0, 0], wr)
              const lyw = rotateVecByQuat([0, 1, 0], wr)
              const lzw = rotateVecByQuat([0, 0, 1], wr)
              const rx = wb.r * wb.sx
              const ry = wb.r * wb.sy
              const rz = wb.r * wb.sz
              lastFaceCenter = [wb.cx, wb.cy, wb.cz]
              lastFaceRadius = wb.r
              lastFacePositions = {
                xp: [wb.cx + lxw[0]*rx, wb.cy + lxw[1]*rx, wb.cz + lxw[2]*rx],
                xn: [wb.cx - lxw[0]*rx, wb.cy - lxw[1]*rx, wb.cz - lxw[2]*rx],
                yp: [wb.cx + lyw[0]*ry, wb.cy + lyw[1]*ry, wb.cz + lyw[2]*ry],
                yn: [wb.cx - lyw[0]*ry, wb.cy - lyw[1]*ry, wb.cz - lyw[2]*ry],
                zp: [wb.cx + lzw[0]*rz, wb.cy + lzw[1]*rz, wb.cz + lzw[2]*rz],
                zn: [wb.cx - lzw[0]*rz, wb.cy - lzw[1]*rz, wb.cz - lzw[2]*rz],
              }
              const hov = activeAxis  // 'x'|'y'|'z' lights both faces on that axis
              drawFaceHandle(vp, lastFacePositions.xp, handleScale, AXIS_COLOR.x, hov === 'x')
              drawFaceHandle(vp, lastFacePositions.xn, handleScale, AXIS_COLOR.x, hov === 'x')
              drawFaceHandle(vp, lastFacePositions.yp, handleScale, AXIS_COLOR.y, hov === 'y')
              drawFaceHandle(vp, lastFacePositions.yn, handleScale, AXIS_COLOR.y, hov === 'y')
              drawFaceHandle(vp, lastFacePositions.zp, handleScale, AXIS_COLOR.z, hov === 'z')
              drawFaceHandle(vp, lastFacePositions.zn, handleScale, AXIS_COLOR.z, hov === 'z')
              usedFaceHandles = true
            }
          }
          if (!usedFaceHandles) {
            // Multi-select OR entity without bounds (sloc, broken model) —
            // fall back to the centroid+arrow-cubes UX so the user always
            // has SOME scale gizmo to interact with.
            lastFacePositions = null
            drawScaleHandle(vp, origin, handleScale, 'x', activeAxis === 'x')
            drawScaleHandle(vp, origin, handleScale, 'y', activeAxis === 'y')
            drawScaleHandle(vp, origin, handleScale, 'z', activeAxis === 'z')
          }
        } else if (mode === 'rotate') {
          // Z-axis ring only (design §5 #9: X/Y rotations have no on-disk storage).
          drawRotateRing(vp, origin, handleScale, 'z', activeAxis === 'z')
        }
      } finally {
        gl.disableVertexAttribArray(prog.aPosition)
        if (wasDepthTest) gl.enable(gl.DEPTH_TEST); else gl.disable(gl.DEPTH_TEST)
        if (wasBlend) gl.enable(gl.BLEND); else gl.disable(gl.BLEND)
        if (wasCull) gl.enable(gl.CULL_FACE); else gl.disable(gl.CULL_FACE)
        gl.depthMask(true)
        gl.bindBuffer(gl.ARRAY_BUFFER, null)
        ;(viewer as any).webgl.currentShader = null
      }
    },

    rayPick(px, py, scene, canvas): GizmoPickResult | null {
      if (!lastVisible) return null
      const ray = screenRay(px, py, scene, canvas)
      if (!ray) return null
      const [rox, roy, roz, rdx, rdy, rdz] = ray

      if (mode === 'move') {
        const axis = pickMove(rox, roy, roz, rdx, rdy, rdz, lastOrigin, lastHandleScale)
        if (axis === null) { hoverAxis = null; return null }
        hoverAxis = axis
        return { axis, mode }
      } else if (mode === 'scale') {
        const hit = pickScale(rox, roy, roz, rdx, rdy, rdz, lastOrigin, lastHandleScale)
        if (!hit) { hoverAxis = null; return null }
        // hoverAxis is the single-axis label ('x'|'y'|'z' or 'centroid'); the
        // draw block uses it to light both faces on the hovered axis.
        hoverAxis = hit.axis === 'centroid' ? null : hit.axis
        return { axis: hit.axis, mode, faceSign: hit.faceSign }
      } else if (mode === 'rotate') {
        const axis = pickRotate(rox, roy, roz, rdx, rdy, rdz, lastOrigin, lastHandleScale)
        if (axis === null) { hoverAxis = null; return null }
        hoverAxis = axis
        return { axis, mode }
      }
      hoverAxis = null
      return null
    },

    beginDrag(pick, px, py, scene, canvas, selectionItems, unitInstances, doodadInstances, unitTypeIndexCache) {
      const origin = computeOrigin(selectionItems, unitInstances, doodadInstances)
      if (!origin) return

      const ray = screenRay(px, py, scene, canvas)
      if (!ray) return
      const [rox, roy, roz, rdx, rdy, rdz] = ray

      const entities: EntityOrig[] = []
      for (const item of selectionItems) {
        const inst = item.kind === 'unit' ? unitInstances.get(item.id) : doodadInstances.get(item.id)
        if (!inst) continue
        const wl = inst.worldLocation
        if (!wl) continue
        let moveHeight = 0
        let modelScale = 1
        if (item.kind === 'unit') {
          const typeId = (inst as any).__wc3ForgeTypeId as string | undefined
          const info = typeId ? unitTypeIndexCache?.[typeId] : undefined
          moveHeight = info?.move_height ?? 0
          modelScale = info?.model_scale ?? 1
        } else {
          modelScale = (inst as any).__wc3ForgeModelScale ?? 1
        }
        const rotOrig = (inst as any).__wc3ForgeRotation ?? 0
        const scaleOrig = ((inst as any).__wc3ForgeScale ?? [1, 1, 1]) as [number, number, number]
        entities.push({
          kind: item.kind,
          cn: item.id,
          posOrig: [wl[0], wl[1], wl[2]],
          rotOrig,
          scaleOrig,
          modelScale,
          inst,
          moveHeight,
        })
      }
      if (entities.length === 0) return

      if (pick.mode === 'move') {
        // pickMove only returns GizmoAxis (no 'centroid'); narrow so the
        // MoveDrag literal below typechecks against MoveDrag.axis: GizmoAxis.
        if (pick.axis === 'centroid') return
        if (isPlaneAxis(pick.axis)) {
          // Plane move: project ray onto plane → 3D anchor point.
          const [nx, ny, nz] = PLANE_NORMAL[pick.axis]
          const hit = rayPlane(
            rox, roy, roz, rdx, rdy, rdz,
            origin[0], origin[1], origin[2], nx, ny, nz,
          )
          if (!hit) return
          dragState = {
            mode: 'move', axis: pick.axis, origin, entities,
            anchorParam: 0,
            anchorPoint: [hit.hx, hit.hy, hit.hz],
          }
        } else {
          const [adx, ady, adz] = AXIS_DIRS[pick.axis]
          const nearest = rayLineNearest(
            rox, roy, roz, rdx, rdy, rdz,
            origin[0], origin[1], origin[2], adx, ady, adz,
          )
          if (!nearest) return
          dragState = {
            mode: 'move', axis: pick.axis, origin, entities,
            anchorParam: nearest.s,
            anchorPoint: [0, 0, 0],
          }
        }
      } else if (pick.mode === 'scale') {
        if (pick.faceSign !== undefined && (pick.axis === 'x' || pick.axis === 'y' || pick.axis === 'z')) {
          // Roblox-style direct manipulation. The handle follows the cursor
          // along the axis through the entity center; the face position IS
          // the new scale edge.
          if (!lastFacePositions || lastFaceRadius <= 0) return
          const center = lastFaceCenter
          const ent0 = entities[0]
          if (!ent0) return
          const axisIdx = pick.axis === 'x' ? 0 : pick.axis === 'y' ? 1 : 2
          // World-space distance from center to face at mousedown. This is
          // what the cursor-projected distance is divided by to get the
          // per-axis factor — natural 1:1 mapping (face at orig distance →
          // factor=1; face at 2× orig distance → factor=2; face at 0.5× →
          // factor=0.5).
          const origHandleDist = lastFaceRadius * ent0.scaleOrig[axisIdx] * ent0.modelScale
          if (origHandleDist <= 1e-3) return
          // Compute the entity's local axis direction in world space —
          // the drag projection line. setScale operates in LOCAL space, so
          // we measure the cursor's movement along that same local axis
          // (transformed to world for ray-line intersection).
          const instAny = ent0.inst as any
          const wr: ArrayLike<number> = instAny.worldRotation
            ?? quatZ(instAny.__wc3ForgeRotation ?? 0)
          const localAxis: [number, number, number] =
            pick.axis === 'x' ? [1, 0, 0] : pick.axis === 'y' ? [0, 1, 0] : [0, 0, 1]
          const axisWorldDir = rotateVecByQuat(localAxis, wr)
          dragState = {
            mode: 'scale', axis: pick.axis, faceSign: pick.faceSign,
            origin: [center[0], center[1], center[2]], entities,
            origHandleDist,
            anchorPx: 0, anchorPy: 0, screenAxisX: 0, screenAxisY: 0,
            perAxisFactor: [1, 1, 1],
            axisWorldDir,
          }
        } else if (pick.axis === 'x' || pick.axis === 'y' || pick.axis === 'z') {
          // Centroid fallback (multi-select). Keep the previous signed-axis
          // cursor-delta math — there's no single bounding box to anchor on.
          const sp0 = worldToScreen(scene, canvas, origin)
          if (!sp0) return
          const [adx, ady, adz] = AXIS_DIRS[pick.axis]
          const ref = lastHandleScale > 0 ? lastHandleScale : 100
          const sp1 = worldToScreen(scene, canvas, [
            origin[0] + adx * ref,
            origin[1] + ady * ref,
            origin[2] + adz * ref,
          ])
          if (!sp1) return
          let sax = sp1.x - sp0.x
          let say = sp1.y - sp0.y
          const len = Math.hypot(sax, say)
          if (len < 1e-3) return
          sax /= len; say /= len
          dragState = {
            mode: 'scale', axis: 'centroid', faceSign: 1,
            origin, entities,
            origHandleDist: 1,
            anchorPx: px, anchorPy: py,
            screenAxisX: sax, screenAxisY: say,
            perAxisFactor: [1, 1, 1],
            axisWorldDir: [1, 0, 0],  // unused for centroid path
          }
        } else {
          return
        }
      } else if (pick.mode === 'rotate') {
        // Rotate pick uses pickMove (axis arrows), which never returns
        // 'centroid'. Narrow so RotateDrag.axis: GizmoAxis typechecks.
        if (pick.axis === 'centroid') return
        const hit = rayPlane(
          rox, roy, roz, rdx, rdy, rdz,
          origin[0], origin[1], origin[2],
          0, 0, 1,
        )
        if (!hit) return
        const anchorAngle = Math.atan2(hit.hy - origin[1], hit.hx - origin[0])
        dragState = {
          mode: 'rotate', axis: pick.axis, origin, entities,
          anchorAngle, currentAngle: anchorAngle,
        }
      }
    },

    onDrag(px, py, scene, canvas, shiftKey) {
      if (!dragState) return
      const ray = screenRay(px, py, scene, canvas)
      if (!ray) return
      const [rox, roy, roz, rdx, rdy, rdz] = ray

      const ds = dragState
      const origin = ds.origin
      const shift = !!shiftKey

      if (ds.mode === 'move') {
        if (isPlaneAxis(ds.axis)) {
          // Plane move: project cursor onto same plane, delta = current - anchor.
          const [nx0, ny0, nz0] = PLANE_NORMAL[ds.axis]
          const hit = rayPlane(
            rox, roy, roz, rdx, rdy, rdz,
            origin[0], origin[1], origin[2], nx0, ny0, nz0,
          )
          if (!hit) return
          let dx = hit.hx - ds.anchorPoint[0]
          let dy = hit.hy - ds.anchorPoint[1]
          let dz = hit.hz - ds.anchorPoint[2]
          if (snapSettings.moveOn) {
            dx = snap(dx, snapSettings.moveStep)
            dy = snap(dy, snapSettings.moveStep)
            dz = snap(dz, snapSettings.moveStep)
          }
          for (const ent of ds.entities) {
            ;(ent.inst as any).setLocation([
              ent.posOrig[0] + dx,
              ent.posOrig[1] + dy,
              ent.posOrig[2] + dz,
            ])
          }
        } else {
          const [adx, ady, adz] = AXIS_DIRS[ds.axis]
          const nearest = rayLineNearest(
            rox, roy, roz, rdx, rdy, rdz,
            origin[0], origin[1], origin[2], adx, ady, adz,
          )
          if (!nearest) return
          let delta = nearest.s - ds.anchorParam
          if (snapSettings.moveOn) delta = snap(delta, snapSettings.moveStep)
          for (const ent of ds.entities) {
            const nx = ent.posOrig[0] + adx * delta
            const ny = ent.posOrig[1] + ady * delta
            const nz = ent.posOrig[2] + adz * delta
            ;(ent.inst as any).setLocation([nx, ny, nz])
          }
        }
      } else if (ds.mode === 'scale') {
        if (ds.axis === 'centroid') {
          // Multi-select fallback: signed cursor-delta projection onto the
          // screen-space axis direction. Same as before; uniform across all
          // entities since there's no single bounding box to anchor on.
          const SCALE_DRAG_PIXELS = 400
          const dpx = px - ds.anchorPx
          const dpy = py - ds.anchorPy
          const signed = dpx * ds.screenAxisX + dpy * ds.screenAxisY
          let factor = Math.pow(2, signed / SCALE_DRAG_PIXELS)
          if (snapSettings.scaleOn) factor = snap(factor, snapSettings.scaleStep)
          if (factor < 0.1) factor = 0.1
          if (factor > 10) factor = 10
          ds.perAxisFactor = [factor, factor, factor]
          for (const ent of ds.entities) {
            const newScale = ent.scaleOrig[0] * factor
            if (typeof (ent.inst as any).uniformScale === 'function') {
              ;(ent.inst as any).uniformScale(newScale * ent.modelScale)
            }
          }
        } else {
          // Single-select Roblox-style: direct manipulation. Project cursor
          // ray onto the world-space axis line through the entity center.
          // The signed projection distance from center, scaled by faceSign,
          // divided by the orig handle distance, IS the new per-axis factor.
          const axisIdx = ds.axis === 'x' ? 0 : ds.axis === 'y' ? 1 : 2
          // Use the LOCAL axis direction transformed to world (captured at
          // mousedown). This is what the visible face handle aligns with,
          // so the cursor projection direction matches what the user sees.
          // Falls back to world axes if axisWorldDir wasn't captured (legacy
          // dragState shapes — shouldn't happen with current beginDrag).
          const axisUnit: [number, number, number] = (ds.axisWorldDir && ds.axisWorldDir.length === 3)
            ? ds.axisWorldDir
            : (ds.axis === 'x' ? [1, 0, 0] : ds.axis === 'y' ? [0, 1, 0] : [0, 0, 1])
          const nearest = rayLineNearest(
            rox, roy, roz, rdx, rdy, rdz,
            origin[0], origin[1], origin[2],
            axisUnit[0], axisUnit[1], axisUnit[2],
          )
          if (!nearest) return
          // Signed distance from center along the axis (positive in +axis,
          // negative in -axis). faceSign decides which direction means "grow."
          const signedDist = nearest.s * ds.faceSign
          let factor = signedDist / ds.origHandleDist
          if (snapSettings.scaleOn) factor = snap(factor, snapSettings.scaleStep)
          if (factor < 0.1) factor = 0.1
          if (factor > 10) factor = 10
          // Shift held = uniform: the dragged axis's factor applies to all
          // three. Released mid-drag snaps the non-dragged axes back to 1
          // immediately so the user sees the modifier toggle live (Blender's
          // shift-scale convention). Without shift = per-axis: only the
          // dragged axis changes, others stay at 1.
          const f: [number, number, number] = shift
            ? [factor, factor, factor]
            : [1, 1, 1]
          if (!shift) f[axisIdx] = factor
          ds.perAxisFactor = f
          for (const ent of ds.entities) {
            const inst = ent.inst as any
            const sx = ent.scaleOrig[0] * f[0]
            const sy = ent.scaleOrig[1] * f[1]
            const sz = ent.scaleOrig[2] * f[2]
            const ms = ent.modelScale
            if (typeof inst.setScale === 'function') {
              inst.setScale([sx * ms, sy * ms, sz * ms])
            } else if (typeof inst.uniformScale === 'function') {
              // Fall back to uniform; will be visually wrong for non-uniform
              // scaling but the on-disk commit still writes per-axis.
              inst.uniformScale(sx * ms)
            }
          }
        }
      } else if (ds.mode === 'rotate') {
        const hit = rayPlane(
          rox, roy, roz, rdx, rdy, rdz,
          origin[0], origin[1], origin[2],
          0, 0, 1,
        )
        if (!hit) return
        const angle = Math.atan2(hit.hy - origin[1], hit.hx - origin[0])
        let delta = wrapAngle(angle - ds.anchorAngle)
        // Snap in DEGREES — rotateStep is in degrees, convert to radians for math.
        if (snapSettings.rotateOn) {
          const step = (snapSettings.rotateStep * Math.PI) / 180
          delta = snap(delta, step)
        }
        ds.currentAngle = ds.anchorAngle + delta
        const cosD = Math.cos(delta), sinD = Math.sin(delta)
        for (const ent of ds.entities) {
          const newRot = ent.rotOrig + delta
          const q = quatZ(newRot)
          if (typeof (ent.inst as any).setRotation === 'function') {
            ;(ent.inst as any).setRotation(q)
          }
          // Multi-select: orbit entity around centroid in XY plane (Z unchanged
          // since we only rotate around Z axis). Single-select reduces to
          // entity.pos == origin → delta=0 → no position change.
          const dx = ent.posOrig[0] - origin[0]
          const dy = ent.posOrig[1] - origin[1]
          const nx = dx * cosD - dy * sinD + origin[0]
          const ny = dx * sinD + dy * cosD + origin[1]
          ;(ent.inst as any).setLocation([nx, ny, ent.posOrig[2]])
        }
      }
    },

    onDragEnd(px, py, scene, canvas, shiftKey) {
      if (!dragState) return
      const ds = dragState
      const shift = !!shiftKey

      if (ds.mode === 'move') {
        let dx = 0, dy = 0, dz = 0
        const ray = screenRay(px, py, scene, canvas)
        if (isPlaneAxis(ds.axis)) {
          if (ray) {
            const [rox, roy, roz, rdx, rdy, rdz] = ray
            const [nx0, ny0, nz0] = PLANE_NORMAL[ds.axis]
            const hit = rayPlane(
              rox, roy, roz, rdx, rdy, rdz,
              ds.origin[0], ds.origin[1], ds.origin[2], nx0, ny0, nz0,
            )
            if (hit) {
              dx = hit.hx - ds.anchorPoint[0]
              dy = hit.hy - ds.anchorPoint[1]
              dz = hit.hz - ds.anchorPoint[2]
            }
          }
          if (snapSettings.moveOn) {
            dx = snap(dx, snapSettings.moveStep)
            dy = snap(dy, snapSettings.moveStep)
            dz = snap(dz, snapSettings.moveStep)
          }
        } else {
          let delta = 0
          if (ray) {
            const [rox, roy, roz, rdx, rdy, rdz] = ray
            const [adx, ady, adz] = AXIS_DIRS[ds.axis]
            const nearest = rayLineNearest(
              rox, roy, roz, rdx, rdy, rdz,
              ds.origin[0], ds.origin[1], ds.origin[2], adx, ady, adz,
            )
            if (nearest) delta = nearest.s - ds.anchorParam
          }
          if (snapSettings.moveOn) delta = snap(delta, snapSettings.moveStep)
          const [adx, ady, adz] = AXIS_DIRS[ds.axis]
          dx = adx * delta; dy = ady * delta; dz = adz * delta
        }
        const ents = ds.entities
        // Wrap the per-entity commit loop in an undo group so the whole drag
        // (1 or N entities) collapses to a single Ctrl+Z. Single-entity drags
        // benefit too — the group label reads cleaner ("Move doodad ×1" still
        // beats untagged "Move doodad" in a list).
        const label = ents.length > 1 ? `Move ${ents.length} entities` : ''
        ;(async () => {
          await BeginUndoGroup(label)
          try {
            for (const ent of ents) {
              const nx = ent.posOrig[0] + dx
              const ny = ent.posOrig[1] + dy
              const nz = ent.posOrig[2] + dz
              ;(ent.inst as any).setLocation([nx, ny, nz])
              const gameZ = nz - ent.moveHeight
              try {
                if (ent.kind === 'unit') await MoveUnit(ent.cn, nx, ny, gameZ)
                else                     await MoveDoodad(ent.cn, nx, ny, gameZ)
              } catch (err) {
                flog(`[gizmo] move commit ${ent.kind} cn=${ent.cn} failed:`, err instanceof Error ? err.message : String(err))
              }
            }
          } finally {
            try { await EndUndoGroup() } catch (err) {
              flog(`[gizmo] EndUndoGroup failed:`, err instanceof Error ? err.message : String(err))
            }
          }
        })()
      } else if (ds.mode === 'rotate') {
        // Re-apply snap to the final delta so commit matches what the user
        // saw at the moment of mouseup (the last onDrag may have been one
        // mouse-tick stale; ds.currentAngle reflects that tick's snapped value).
        let delta = ds.currentAngle - ds.anchorAngle
        if (snapSettings.rotateOn) {
          const step = (snapSettings.rotateStep * Math.PI) / 180
          delta = snap(delta, step)
        }
        const cosD = Math.cos(delta), sinD = Math.sin(delta)
        const origin = ds.origin
        const ents = ds.entities
        const rotLabel = ents.length > 1 ? `Rotate ${ents.length} entities` : ''
        ;(async () => {
          await BeginUndoGroup(rotLabel)
          try {
            for (const ent of ents) {
              const newRot = ent.rotOrig + delta
              // Orbit position around centroid (no-op for single-select).
              const dx = ent.posOrig[0] - origin[0]
              const dy = ent.posOrig[1] - origin[1]
              const nx = dx * cosD - dy * sinD + origin[0]
              const ny = dx * sinD + dy * cosD + origin[1]
              const nz = ent.posOrig[2]
              const positionChanged = (Math.abs(nx - ent.posOrig[0]) > 1e-3
                                    || Math.abs(ny - ent.posOrig[1]) > 1e-3)
              const gameZ = nz - ent.moveHeight
              try {
                if (ent.kind === 'unit') {
                  await RotateUnit(ent.cn, newRot)
                  if (positionChanged) await MoveUnit(ent.cn, nx, ny, gameZ)
                } else {
                  await RotateDoodad(ent.cn, newRot)
                  if (positionChanged) await MoveDoodad(ent.cn, nx, ny, gameZ)
                }
              } catch (err) {
                flog(`[gizmo] rotate commit ${ent.kind} cn=${ent.cn} failed:`, err instanceof Error ? err.message : String(err))
              }
            }
          } finally {
            try { await EndUndoGroup() } catch (err) {
              flog(`[gizmo] EndUndoGroup failed:`, err instanceof Error ? err.message : String(err))
            }
          }
        })()
      } else if (ds.mode === 'scale') {
        const f: [number, number, number] = [
          ds.perAxisFactor[0], ds.perAxisFactor[1], ds.perAxisFactor[2],
        ]
        // Apply scale-snap to whichever axes changed.
        if (snapSettings.scaleOn) {
          for (let i = 0; i < 3; i++) {
            if (Math.abs(f[i] - 1) > 1e-6) f[i] = snap(f[i], snapSettings.scaleStep)
          }
        }
        // Final clamp guards.
        for (let i = 0; i < 3; i++) {
          if (f[i] < 0.1) f[i] = 0.1
          if (f[i] > 10) f[i] = 10
        }
        const origin = ds.origin
        const ents = ds.entities
        const isCentroid = ds.axis === 'centroid'
        const scaleLabel = ents.length > 1 ? `Scale ${ents.length} entities` : ''
        ;(async () => {
          await BeginUndoGroup(scaleLabel)
          try {
            for (const ent of ents) {
              const newSx = ent.scaleOrig[0] * f[0]
              const newSy = ent.scaleOrig[1] * f[1]
              const newSz = ent.scaleOrig[2] * f[2]
              // Position update only for the centroid (multi-select) path —
              // single-select Roblox-style scales symmetrically around the
              // entity's pivot, no position change. For centroid we keep the
              // existing orbit-distance math but only on the dragged axis.
              let nx = ent.posOrig[0], ny = ent.posOrig[1], nz = ent.posOrig[2]
              if (isCentroid) {
                const factor = f[0]   // uniform for centroid
                const dx = ent.posOrig[0] - origin[0]
                const dy = ent.posOrig[1] - origin[1]
                const dz = ent.posOrig[2] - origin[2]
                nx = ent.posOrig[0] + dx * (factor - 1)
                ny = ent.posOrig[1] + dy * (factor - 1)
                nz = ent.posOrig[2] + dz * (factor - 1)
              }
              const positionChanged = (Math.abs(nx - ent.posOrig[0]) > 1e-3
                                    || Math.abs(ny - ent.posOrig[1]) > 1e-3
                                    || Math.abs(nz - ent.posOrig[2]) > 1e-3)
              const gameZ = nz - ent.moveHeight
              try {
                if (ent.kind === 'unit') {
                  await ScaleUnit(ent.cn, newSx, newSy, newSz)
                  if (positionChanged) await MoveUnit(ent.cn, nx, ny, gameZ)
                } else {
                  await ScaleDoodad(ent.cn, newSx, newSy, newSz)
                  if (positionChanged) await MoveDoodad(ent.cn, nx, ny, gameZ)
                }
              } catch (err) {
                flog(`[gizmo] scale commit ${ent.kind} cn=${ent.cn} failed:`, err instanceof Error ? err.message : String(err))
              }
            }
          } finally {
            try { await EndUndoGroup() } catch (err) {
              flog(`[gizmo] EndUndoGroup failed:`, err instanceof Error ? err.message : String(err))
            }
          }
        })()
      }

      dragState = null
      hoverAxis = null
    },

    cancelDrag() {
      if (!dragState) return
      for (const ent of dragState.entities) restoreEntity(ent)
      dragState = null
      hoverAxis = null
    },

    isDragging() { return dragState !== null },
    dragAxis()   { return dragState?.axis ?? null },
    dragMode()   { return dragState?.mode ?? null },

    getMoveArrowTips() {
      if (mode !== 'move' || !lastVisible) return null
      const o = lastOrigin
      const s = lastHandleScale
      // CONE_TIP * s = world distance from origin to the arrow's tip. Add a
      // small visual outset so the label sits just past the cone instead of
      // intersecting it. Y axis uses its flipped direction so the label is
      // at the VISIBLE cone tip (matches AXIS_DIRS.y = [0, -1, 0]).
      const t = CONE_TIP * s * 1.15
      return {
        x: [o[0] + t, o[1], o[2]] as [number, number, number],
        y: [o[0], o[1] - t, o[2]] as [number, number, number],
        z: [o[0], o[1], o[2] + t] as [number, number, number],
      }
    },

    dispose() {
      gl.deleteBuffer(cylVBO)
      gl.deleteBuffer(coneVBO)
      gl.deleteBuffer(ringVBO)
      gl.deleteBuffer(cubeVBO)
      gl.deleteBuffer(planeXYVBO)
      gl.deleteBuffer(planeXZVBO)
      gl.deleteBuffer(planeYZVBO)
      gl.deleteProgram(prog.program)
      dragState = null
    },
  }
}
