---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/cliff-shader.ts"
ref: HEAD
---

// HiveWE-faithful cliff renderer. Replaces mdx-m3-viewer's stock MDX shader
// path for cliffs (which produced fixed-Z mesh tops that didn't conform to
// ramp surfaces). This file implements HiveWE's `cliff.vert` + `cliff.frag`
// in WebGL1 ESSL1, leveraging mdx-m3-viewer's MDX parser only for geometry
// upload (we reuse its `arrayBuffer` + `elementBuffer` GL buffers and the
// `geoset.positionOffset` / `normalOffset` / `uvOffset` / `faceOffset`
// metadata).
//
// ── HiveWE pipeline summary ──────────────────────────────────────────────
//
// CPU side (terrain.ixx):
//   gpu_final_ground_heights[i] = corner_height[i] + corner_layer_height[i] - 2
//     (with `+0.5` ramp-base bump per `update_ground_heights`)
//
// `gpu_final_ground_heights` becomes the `cliff_levels` SSBO buffer in
// cliff.vert. The cliff MDX vertex shader (line 35) per-vertex displaces:
//
//   vec3 rotated = vec3(vPos.y, -vPos.x, vPos.z) / 128 + vOffset.xyz
//   int2 hp = ivec2(rotated.xy)
//   float h = cliff_levels[hp.y * W + hp.x]
//   gl_Position = VP * vec4(rotated.xy, rotated.z + h, 1)
//
// Where `vOffset` is per-instance: (cellI, cellJ, min_layer - 2, palette_idx).
// The displacement makes the cliff mesh TOP edge sit exactly at the smooth
// ground-height of each corner — flat for pure cliffs, sloped for ramps.
//
// ── Port to wc3-forge stud space ─────────────────────────────────────────
//
// HiveWE uses cell-coordinates (1 cell = 1 unit) everywhere except inside
// MDX geometry (which is in studs, hence the `/128`). We use stud-space
// throughout (no /128). MDX vertices stay in studs; per-instance offset is
// in WORLD stud coordinates (with center_offset baked in). The shader
// re-derives the corner-grid index from world XY via:
//
//   vec2 cellLocal = (rotated.xy - centerOffset) / 128
//   int ix = floor(cellLocal.x)
//   int iy = floor(cellLocal.y)
//   float h = finalHeights[iy * W + ix]    // sampled from float texture
//
// `finalHeights` is the same FinalZ() array the terrain.ts mesh uses, with
// the ramp +64-stud bump pre-applied in app.go GetTerrain.
//
// ── Textures ─────────────────────────────────────────────────────────────
//
// HiveWE binds a 2D-texture-ARRAY (one layer per cliff palette) and picks
// via `vOffset.a`. WebGL1 has no sampler2DArray, so we batch draws PER
// (mesh × palette) — one draw per unique combination, binding the right
// palette texture each time. Number of combinations is small (typical map:
// ~20-100 unique cliff meshes × 1-2 palettes = 20-200 draw calls).
//
// ── What's REPLACED from the old path ────────────────────────────────────
//
// The old cliffs.ts used `viewer.load(path) → model.addInstance() →
// inst.rotateLocal(CLIFF_UNROTATE_QUAT) → inst.move(pos) → inst.setScene()`.
// The `rotateLocal` was a workaround for the `(y, -x, z)` swap that HiveWE's
// cliff.vert does INSIDE the shader. With this new shader, the swap lives
// in the shader; the per-instance rotateLocal is REMOVED. The lib
// instance path is bypassed entirely — cliffs no longer participate in the
// lib's instance lifecycle.

import { flog } from './debuglog'

interface FinalHeightsTex {
  tex: WebGLTexture
  width: number
  height: number
}

interface CliffPaletteTex {
  tex: WebGLTexture | null
  width: number
  height: number
}

interface CliffMeshGeom {
  // GL buffers we BIND (owned by mdx-m3-viewer's MdxModel — do NOT delete)
  vbo: WebGLBuffer
  ibo: WebGLBuffer
  // Geoset metadata from MdxModel.geosets[0]
  positionOffset: number
  normalOffset: number
  uvOffset: number      // byte offset to coord 0's UV set
  faceOffset: number    // byte offset into element buffer
  vertices: number      // vertex count
  elements: number      // index count
}

export interface CliffInstance {
  /** World X of cell BL corner, in studs (includes centerOffset). */
  cellWorldX: number
  /** World Y of cell BL corner, in studs (includes centerOffset). */
  cellWorldY: number
  /** Base Z for the cliff foot, in studs: (min_layer - 2) * 128. */
  baseZ: number
  /** Cliff palette index (0..N-1). */
  paletteIdx: number
}

export interface CliffRenderer {
  /** Register an MDX model + instances for the given path. Idempotent on
   *  same-path calls — they accumulate into the existing per-mesh bucket. */
  addMesh(path: string, model: any, instances: CliffInstance[]): void
  /** Draw all registered meshes against the given view-projection. */
  draw(viewProj: Float32Array): void
  /** Release all GL resources OWNED BY THIS RENDERER (textures, buffers,
   *  program). Does NOT delete MdxModel geometry buffers — those are owned
   *  by mdx-m3-viewer. */
  dispose(): void
  /** Total instance count placed (for telemetry / diagnostics). */
  readonly instanceCount: number
}

// Vertex shader — HiveWE cliff.vert ported to ESSL 1.00.
//
// Key differences from HiveWE (which is GLSL 4.50 core):
// - No `layout(location = N) in` — uses `attribute` + getAttribLocation.
// - No SSBO for cliff_levels; we use a float-texture sampled by integer
//   lookup. The texture stores W*H float Z-values; sampled with NEAREST
//   filter so floor() math hits the right texel center.
// - No bitwise/integer-only paths; ESSL1's `int` is supported but division
//   is fp; we explicitly cast.
// - Per-instance `a_offset` (vec4) replaces `vOffset` from a divisor buffer.
const VERT_SHADER = `
precision highp float;
attribute vec3 a_position;
attribute vec3 a_normal;
attribute vec2 a_uv;
attribute vec4 a_offset;    // (cellWorldX, cellWorldY, baseZ_studs, paletteIdx)

uniform mat4 u_VP;
uniform vec2 u_centerOffset;
uniform vec2 u_mapSize;         // (W, H) in floats so we can /
uniform sampler2D u_finalHeights;   // R32F (or LUMINANCE float); float-texture; NEAREST
uniform vec2 u_finalHeightsTexel;   // (1/W, 1/H)

varying vec2 v_uv;
varying vec3 v_normal;
varying vec3 v_worldPos;

// Look up a height by integer corner index. Clamped to valid range.
float heightAt(float ix, float iy) {
  float cx = clamp(ix, 0.0, u_mapSize.x - 1.0);
  float cy = clamp(iy, 0.0, u_mapSize.y - 1.0);
  // texel center = (i + 0.5) / size
  vec2 uv = vec2((cx + 0.5) * u_finalHeightsTexel.x,
                 (cy + 0.5) * u_finalHeightsTexel.y);
  return texture2D(u_finalHeights, uv).r;
}

void main() {
  // HiveWE cliff.vert line 22 — unrotate the 90-degree-baked-in cliff MDX.
  // (x, y, z) → (y, -x, z). In our system we keep studs throughout — no /128.
  vec3 rotated = vec3(a_position.y, -a_position.x, a_position.z) + a_offset.xyz;

  // Derive cell-grid-coords from world XY. The mesh vertex at the cell's
  // BL corner has rotated.xy == (cellWorldX, cellWorldY) — divide by 128
  // and add the integer cell offset (encoded into a_offset.xy as world-space
  // BL position). We use floor() to find the corner whose final-height we
  // should sample. For verts exactly at a corner this is unambiguous; for
  // verts in the cell interior (cliff vert that sits at sub-cell XY) HiveWE
  // also floor()s — the per-vertex height lookup is the source of the slope.
  vec2 cellLocal = (rotated.xy - u_centerOffset) / 128.0;
  float ix = floor(cellLocal.x + 0.0001); // bias by epsilon to defeat (n - eps) → n-1 issues
  float iy = floor(cellLocal.y + 0.0001);
  float h = heightAt(ix, iy);

  // HiveWE: gl_Position = VP * vec4(rotated.xy, rotated.z + height, 1)
  // Our rotated.z = a_position.z + (min_layer - 2) * 128   (in studs)
  // h = finalHeight at corner (already in studs, in our system FinalZ()
  // = Z + (layer - 2) * 128 with ramp +64 bump).
  //
  // For a cliff vertex at mesh-Z = 128 (top of cliff face) at a layer-2
  // corner (h = 0 with smooth=0), z_world = 128 + (min-2)*128 + 0 — for
  // min=1 that's 128-128+0=0, matching the layer-2 ground. ✓
  // For a slope vertex on a ramp at a base-corner (h = base + 64), the
  // mesh-Z varies with slope position, and h provides the correct ramp
  // elevation at this corner. ✓
  vec3 world = vec3(rotated.xy, rotated.z + h);
  gl_Position = u_VP * vec4(world, 1.0);

  // HiveWE cliff.vert line 41 — unrotate normal too so it points outward
  // in world space, then blend with the terrain normal computed from h
  // neighbors. We skip the neighbor-blend for simplicity (the lib's stock
  // MDX shader doesn't do it either; the in-shader blend is HiveWE's
  // smoothing trick for ramps and not load-bearing for visual correctness
  // of cliff faces). Just unrotate.
  v_normal = vec3(a_normal.y, -a_normal.x, a_normal.z);
  v_uv = a_uv;
  v_worldPos = world;
}
`.trim()

// Fragment shader — HiveWE cliff.frag (lighting only; we skip pathing-map
// + brush overlays since wc3-forge doesn't have those tools yet).
const FRAG_SHADER = `
precision mediump float;
uniform sampler2D u_paletteTex;
uniform bool u_showLighting;
uniform vec3 u_lightDir;     // normalized; matches HiveWE's normalize(1,1,-3)
varying vec2 v_uv;
varying vec3 v_normal;
varying vec3 v_worldPos;

void main() {
  vec4 c = texture2D(u_paletteTex, v_uv);
  if (u_showLighting) {
    // HiveWE cliff.frag line 24: (dot(-light, normal) + 1) * 0.5
    // Half-Lambert so backfaces aren't fully black.
    vec3 n = normalize(v_normal);
    float contribution = (dot(-u_lightDir, n) + 1.0) * 0.5;
    c.rgb *= clamp(contribution, 0.0, 1.0);
  }
  gl_FragColor = c;
}
`.trim()

function compileShader(gl: WebGLRenderingContext, type: number, source: string): WebGLShader {
  const sh = gl.createShader(type)!
  gl.shaderSource(sh, source)
  gl.compileShader(sh)
  if (!gl.getShaderParameter(sh, gl.COMPILE_STATUS)) {
    const log = gl.getShaderInfoLog(sh)
    gl.deleteShader(sh)
    throw new Error('cliff shader compile: ' + log)
  }
  return sh
}

function buildProgram(gl: WebGLRenderingContext) {
  const prog = gl.createProgram()!
  gl.attachShader(prog, compileShader(gl, gl.VERTEX_SHADER, VERT_SHADER))
  gl.attachShader(prog, compileShader(gl, gl.FRAGMENT_SHADER, FRAG_SHADER))
  gl.linkProgram(prog)
  if (!gl.getProgramParameter(prog, gl.LINK_STATUS)) {
    const log = gl.getProgramInfoLog(prog)
    gl.deleteProgram(prog)
    throw new Error('cliff program link: ' + log)
  }
  return {
    prog,
    aPosition: gl.getAttribLocation(prog, 'a_position'),
    aNormal: gl.getAttribLocation(prog, 'a_normal'),
    aUV: gl.getAttribLocation(prog, 'a_uv'),
    aOffset: gl.getAttribLocation(prog, 'a_offset'),
    uVP: gl.getUniformLocation(prog, 'u_VP')!,
    uCenterOffset: gl.getUniformLocation(prog, 'u_centerOffset')!,
    uMapSize: gl.getUniformLocation(prog, 'u_mapSize')!,
    uFinalHeights: gl.getUniformLocation(prog, 'u_finalHeights')!,
    uFinalHeightsTexel: gl.getUniformLocation(prog, 'u_finalHeightsTexel')!,
    uPaletteTex: gl.getUniformLocation(prog, 'u_paletteTex')!,
    uShowLighting: gl.getUniformLocation(prog, 'u_showLighting')!,
    uLightDir: gl.getUniformLocation(prog, 'u_lightDir')!,
  }
}

// Upload the per-corner final-heights array as a float texture so the cliff
// vertex shader can do `texture2D(finalHeights, uv).r` for the per-vertex
// displacement lookup. Requires OES_texture_float (which mdx-m3-viewer also
// requires, so it's effectively guaranteed in our environment).
//
// Layout: width = W (vertex-count along X), height = H. Single channel R.
// LUMINANCE works on WebGL1+OES_texture_float; OES_texture_float_linear is
// NOT required since we sample with NEAREST.
export function uploadFinalHeights(
  gl: WebGLRenderingContext,
  heights: Float32Array,
  W: number,
  H: number,
): FinalHeightsTex | null {
  if (heights.length !== W * H) {
    flog(`[cliff-shader] finalHeights size mismatch: got ${heights.length}, want ${W * H}`)
    return null
  }
  const floatExt = gl.getExtension('OES_texture_float')
  if (!floatExt) {
    flog('[cliff-shader] OES_texture_float missing — cannot upload float heights texture')
    return null
  }
  const tex = gl.createTexture()
  if (!tex) return null
  gl.bindTexture(gl.TEXTURE_2D, tex)
  gl.pixelStorei(gl.UNPACK_ALIGNMENT, 4)
  // LUMINANCE + FLOAT is the broad-compat path. Some implementations are
  // picky; if this fails on a target machine we'd switch to RGBA-packed
  // floats via internalFormat = RGBA, but every modern GPU supports the
  // LUMINANCE+FLOAT combo via the float ext.
  gl.texImage2D(gl.TEXTURE_2D, 0, gl.LUMINANCE, W, H, 0, gl.LUMINANCE, gl.FLOAT, heights)
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST)
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST)
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE)
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE)
  return { tex, width: W, height: H }
}

interface MeshBucket {
  geom: CliffMeshGeom
  // Per-palette instance lists. Each palette gets its own instance VBO + a
  // separate draw call (WebGL1 has no sampler2DArray).
  byPalette: Map<number, {
    instances: CliffInstance[]
    instVBO: WebGLBuffer
    // True once instances have been uploaded to instVBO. Set false on
    // any addMesh that appends more instances — flush via uploadIfDirty.
    uploaded: boolean
  }>
}

export function createCliffRenderer(
  gl: WebGLRenderingContext,
  opts: {
    finalHeights: FinalHeightsTex
    centerOffset: [number, number]
    paletteTextures: CliffPaletteTex[]   // index by paletteIdx
  },
): CliffRenderer | null {
  let program: ReturnType<typeof buildProgram>
  try {
    program = buildProgram(gl)
  } catch (e) {
    flog('[cliff-shader] program build failed:', e instanceof Error ? e.message : String(e))
    return null
  }

  // ANGLE_instanced_arrays — mdx-m3-viewer also hard-requires this.
  const instExt = gl.getExtension('ANGLE_instanced_arrays')
  if (!instExt) {
    flog('[cliff-shader] ANGLE_instanced_arrays missing — cliff rendering disabled')
    gl.deleteProgram(program.prog)
    return null
  }

  const meshes = new Map<string, MeshBucket>()
  let instanceCount = 0

  // Pre-computed HiveWE light direction. We don't recompute per-frame.
  const lightDir = (() => {
    const x = 1, y = 1, z = -3
    const len = Math.sqrt(x * x + y * y + z * z)
    return new Float32Array([x / len, y / len, z / len])
  })()

  const renderer: CliffRenderer = {
    get instanceCount() { return instanceCount },

    addMesh(path, model, instances) {
      // Extract geoset 0 from MdxModel (HiveWE convention: cliff MDX has
      // a single geoset). The model has already uploaded vbo/ibo via
      // setupgeosets.ts; we just borrow the handles.
      const geosets = model.geosets
      if (!geosets || geosets.length === 0) {
        flog(`[cliff-shader] ${path}: no geosets`)
        return
      }
      const g = geosets[0]
      const vbo = model.arrayBuffer
      const ibo = model.elementBuffer
      if (!vbo || !ibo) {
        flog(`[cliff-shader] ${path}: model has no GL buffers`)
        return
      }
      let bucket = meshes.get(path)
      if (!bucket) {
        bucket = {
          geom: {
            vbo,
            ibo,
            positionOffset: g.positionOffset,
            normalOffset: g.normalOffset,
            // coordId=0 for cliff MDXs (single UV set)
            uvOffset: g.uvOffset,
            faceOffset: g.faceOffset,
            vertices: g.vertices,
            elements: g.elements,
          },
          byPalette: new Map(),
        }
        meshes.set(path, bucket)
      }
      // Group new instances by palette (HiveWE binds per-palette texture).
      for (const inst of instances) {
        let palBucket = bucket.byPalette.get(inst.paletteIdx)
        if (!palBucket) {
          const instVBO = gl.createBuffer()!
          palBucket = { instances: [], instVBO, uploaded: false }
          bucket.byPalette.set(inst.paletteIdx, palBucket)
        }
        palBucket.instances.push(inst)
        palBucket.uploaded = false
        instanceCount++
      }
    },

    draw(viewProj) {
      if (meshes.size === 0) return

      gl.useProgram(program.prog)
      gl.uniformMatrix4fv(program.uVP, false, viewProj)
      gl.uniform2f(program.uCenterOffset, opts.centerOffset[0], opts.centerOffset[1])
      gl.uniform2f(program.uMapSize, opts.finalHeights.width, opts.finalHeights.height)
      gl.uniform2f(program.uFinalHeightsTexel,
        1 / opts.finalHeights.width, 1 / opts.finalHeights.height)
      gl.uniform1i(program.uShowLighting, 1)
      gl.uniform3fv(program.uLightDir, lightDir)

      gl.activeTexture(gl.TEXTURE0)
      gl.bindTexture(gl.TEXTURE_2D, opts.finalHeights.tex)
      gl.uniform1i(program.uFinalHeights, 0)

      gl.enable(gl.DEPTH_TEST)
      gl.depthFunc(gl.LEQUAL)
      gl.depthMask(true)
      gl.disable(gl.BLEND)
      // Cliff meshes have a single visible side; HiveWE enables CULL_FACE
      // (line 593 in terrain.ixx). MDX winding for stock cliffs is CCW
      // (front=GL_CCW which is GL's default). Enable CULL_FACE here to
      // match. If a cliff renders backward we'd see no face from the
      // outside — switch to BACK culling explicitly.
      gl.enable(gl.CULL_FACE)
      gl.cullFace(gl.BACK)
      gl.frontFace(gl.CCW)
      // Defensive Z-pushback for cliff geometry so its TOP face never
      // accidentally wins the Z-fight against a ramp's terrain ground quad
      // (both are drawn at the same world Z after the +0.5-layer / +64-stud
      // ramp-base bump in update_ground_heights). Pure cliffs (cell_skip=1,
      // no ground quad) are unaffected — nothing to Z-fight with. Vertical
      // cliff faces are perpendicular to the ground plane so the offset
      // just nudges depth without creating visible gaps.
      gl.enable(gl.POLYGON_OFFSET_FILL)
      gl.polygonOffset(1.0, 1.0)

      for (const bucket of meshes.values()) {
        const geom = bucket.geom
        gl.bindBuffer(gl.ARRAY_BUFFER, geom.vbo)
        gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, geom.ibo)

        if (program.aPosition >= 0) {
          gl.enableVertexAttribArray(program.aPosition)
          gl.vertexAttribPointer(program.aPosition, 3, gl.FLOAT, false, 0, geom.positionOffset)
        }
        if (program.aNormal >= 0) {
          gl.enableVertexAttribArray(program.aNormal)
          gl.vertexAttribPointer(program.aNormal, 3, gl.FLOAT, false, 0, geom.normalOffset)
        }
        if (program.aUV >= 0) {
          gl.enableVertexAttribArray(program.aUV)
          gl.vertexAttribPointer(program.aUV, 2, gl.FLOAT, false, 0, geom.uvOffset)
        }

        for (const [palIdx, palBucket] of bucket.byPalette) {
          if (palBucket.instances.length === 0) continue
          if (!palBucket.uploaded) {
            const data = new Float32Array(palBucket.instances.length * 4)
            for (let i = 0; i < palBucket.instances.length; i++) {
              const inst = palBucket.instances[i]
              data[i * 4 + 0] = inst.cellWorldX
              data[i * 4 + 1] = inst.cellWorldY
              data[i * 4 + 2] = inst.baseZ
              data[i * 4 + 3] = inst.paletteIdx
            }
            gl.bindBuffer(gl.ARRAY_BUFFER, palBucket.instVBO)
            gl.bufferData(gl.ARRAY_BUFFER, data, gl.STATIC_DRAW)
            palBucket.uploaded = true
            // Restore vertex VBO binding for the per-vertex attribs.
            gl.bindBuffer(gl.ARRAY_BUFFER, geom.vbo)
          }

          // Bind the palette texture for this draw. Falls back to a
          // 1×1 magenta if a palette is missing (rare; signals bad SLK).
          const pt = opts.paletteTextures[palIdx]
          if (!pt || !pt.tex) {
            // Skip rather than draw with magenta — better silent than
            // visibly wrong in the common case (palette index outside
            // the cliffset's loaded textures, which means this instance
            // shouldn't have been queued in the first place).
            continue
          }
          gl.activeTexture(gl.TEXTURE1)
          gl.bindTexture(gl.TEXTURE_2D, pt.tex)
          gl.uniform1i(program.uPaletteTex, 1)

          gl.bindBuffer(gl.ARRAY_BUFFER, palBucket.instVBO)
          if (program.aOffset >= 0) {
            gl.enableVertexAttribArray(program.aOffset)
            gl.vertexAttribPointer(program.aOffset, 4, gl.FLOAT, false, 0, 0)
            instExt.vertexAttribDivisorANGLE(program.aOffset, 1)
          }
          // Switch back to vertex VBO before the draw — IBO binding stays.
          gl.bindBuffer(gl.ARRAY_BUFFER, geom.vbo)

          instExt.drawElementsInstancedANGLE(
            gl.TRIANGLES,
            geom.elements,
            gl.UNSIGNED_SHORT,
            geom.faceOffset,
            palBucket.instances.length,
          )

          if (program.aOffset >= 0) {
            instExt.vertexAttribDivisorANGLE(program.aOffset, 0)
          }
        }
      }

      if (program.aPosition >= 0) gl.disableVertexAttribArray(program.aPosition)
      if (program.aNormal >= 0) gl.disableVertexAttribArray(program.aNormal)
      if (program.aUV >= 0) gl.disableVertexAttribArray(program.aUV)
      if (program.aOffset >= 0) gl.disableVertexAttribArray(program.aOffset)

      // Restore default GL state so subsequent passes (MDX, water, slocs) don't
      // inherit the cliff-only polygon offset. mdx-m3-viewer's render path
      // assumes POLYGON_OFFSET_FILL is disabled and reset to (0, 0); leaving
      // it on would push every subsequent triangle backward too.
      gl.disable(gl.POLYGON_OFFSET_FILL)
      gl.polygonOffset(0.0, 0.0)
    },

    dispose() {
      for (const bucket of meshes.values()) {
        for (const palBucket of bucket.byPalette.values()) {
          gl.deleteBuffer(palBucket.instVBO)
        }
      }
      meshes.clear()
      gl.deleteProgram(program.prog)
      // Final heights texture + palette textures are owned by the caller
      // (created by uploadFinalHeights / viewer.load; caller disposes).
    },
  }

  return renderer
}
