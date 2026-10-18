---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/mdx-parser-patch.ts"
ref: HEAD
---

// Monkey-patch mdx-m3-viewer's MDX parser to handle modern Reforged MDX
// formats. The library was last actively maintained around early Reforged
// (~v1.32), targeting MDX VERS=900/1000. WC3 Reforged 1.33+ shipped a new
// MDX wire format (VERS >= 1100) for Material/Layer chunks that the lib
// doesn't understand. Without these patches, the lib throws while reading
// the MTLS chunk and silently catches the exception in MdxModel's
// constructor (model.js wraps parser.load() in a no-op catch). The result
// is a model with 0 geosets / 0 bones / 0 batches — present in the viewer
// cache but rendering as nothing. This produces the "silent invisible
// units / cliffs / doodads" symptom across modern Reforged maps.
//
// **The new (post-v1100) MTLS / Layer wire format** — derived by reading
// HiveWE's `src/file_formats/mdx/mdx_reader.cpp::read_MTLS_texs_post_v1100`
// AND verified by hex-inspecting MDX files extracted from the user's local
// CASC install (units/human/footman/footman.mdx,
// doodads/terrain/cliffs/cliffsaacb0.mdx,
// doodads/lordaeronsummer/props/brazier/brazier.mdx — all VERS=1200):
//
//   Material chunk (per entry within MTLS):
//     uint32  size                       // includes itself
//     int32   priorityPlane
//     uint32  flags
//     // NO shader field — drops the 80-byte char[80] that pre-v1100
//     // versions 900/1000 carry between flags and LAYS.
//     "LAYS"  (uint32 tag)
//     uint32  numLayers
//     Layer[numLayers]
//
//   Layer:
//     uint32  size                       // includes itself
//     uint32  filterMode (a.k.a. blend_mode)
//     uint32  flags    (a.k.a. shading_flags)
//     uint32  textureId                  // unused in this format (textures
//                                        // are carried inline in the texs
//                                        // array below); lib must skip it
//     uint32  textureAnimationId
//     uint32  coordId
//     float32 alpha
//     float32 emissiveGain
//     float32[3] fresnelColor
//     float32 fresnelOpacity
//     float32 fresnelTeamColor
//     uint32  hd                         // NEW in post-v1100
//     uint32  texsCount                  // NEW in post-v1100
//     Tex[texsCount]:
//       uint32  id
//       uint32  slot                     // HiveWE comment: "always garbage"
//       [optional KMTF track for this tex]
//     // Then animation tracks: KMTA, KMTE, KFC3, KFCA, KFTC
//
// `size` covers the whole layer including the per-tex KMTF tracks AND the
// trailing animation tracks. After consuming the fixed fields + texs array,
// remaining bytes in `size` are animation tags read by the existing
// readAnimations machinery (which is correct for post-v1100 — only the
// PRE-animation layout changed).
//
// **Backward compatibility.** Files with VERS < 1100 keep the original
// readMdx logic (called via the saved `origLayerReadMdx` reference). The
// lib's readMdx is correct for VERS 800/900/1000. We dispatch on parser
// version via a `_currentVersion` capture in the Material monkey-patch.
//
// **Pre-v1100 fix (kept from the previous patch).** Some VERS > 800 files
// with version=900/1000 in pre-1.33 Reforged also drop the shader field.
// Peeking the next 4 bytes for "LAYS" detects which layout the file uses;
// the new Material patch unifies the dispatch.
//
// **Idempotent.** A flag on `(globalThis as any)` guards against double-
// patch if this module gets imported by multiple entry points.
//
// **Verification fixture.** node scripts/test-mdx-parse-with-patch.cjs
// (in the repo) loads extracted MDX files and reports the parsed-out
// model state (sequences/materials/geosets/bones counts). After this
// patch, the cliff + brazier + runeart + footman files all parse with
// non-zero geosets and bones — confirming geometry made it through the
// MTLS chunk and into GEOS.

import * as MV_ns from 'mdx-m3-viewer'
// AnimationMap is NOT re-exported from the lib's public `parsers.mdlx`
// surface (only Model / Sequence / Material / Layer / Texture / etc. live
// there). Import directly from the lib's internal deep path. Without this,
// the safety-belt readAnimations override below would treat EVERY animation
// tag as "unknown" and skip the rest of every animation chunk — bones end
// up with animations=[], so the runtime AnimatedObject's variants Map is
// empty, updateNodes skips per-frame transform lookup, and every skeletal
// model renders frozen (particles still work, since they tick independently).
// Was the root cause of the "doodads render but never animate" bug fixed
// in commit dc0aaa0 — that fix lived in scene-instances.ts as a re-patch;
// we now consolidate it here at the source.
import animationMapModule from 'mdx-m3-viewer/dist/cjs/parsers/mdlx/animationmap.js'
// Runtime handler classes (deep imports, same approach as animationmap above).
// We patch their render/bind so the HD path respects per-layer blending — the
// lib's HD batch render hardcodes blend-off + depth-write-on, which turns
// additive/blended HD layers (e.g. the Circle of Power's glow rings) into
// opaque dark quads. See patchHdBlending below.
import batchGroupModule from 'mdx-m3-viewer/dist/cjs/viewer/handlers/mdx/batchgroup.js'
import geosetModule from 'mdx-m3-viewer/dist/cjs/viewer/handlers/mdx/geoset.js'

const MV: any = (MV_ns as any).default ?? MV_ns
const animationMap: Record<string, [string, any]> | null =
  (animationMapModule as any)?.default ?? (animationMapModule as any) ?? null

const PATCHED_FLAG = '__wc3ForgeMdxParserPatched'

export function patchMdxParser(): void {
  if ((globalThis as any)[PATCHED_FLAG]) return

  const Layer = MV?.parsers?.mdlx?.Layer
  const Material = MV?.parsers?.mdlx?.Material
  if (!Layer || !Material) {
    // Lib shape changed — bail loud so the failure mode is visible (a thrown
    // error in console) rather than silently leaving the patch off and
    // reproducing the "invisible geometry" symptom.
    throw new Error('mdx-parser-patch: cannot find Layer/Material classes')
  }

  // Save the lib's original Layer.readMdx so files with VERS < 1100 still
  // go through the lib's well-tested path.
  const origLayerReadMdx = Layer.prototype.readMdx

  // Post-v1100 Layer reader. Parses the new wire format (see the doc block
  // at top). Mutates `this` the same way the lib's reader does, so the
  // surrounding viewer code (which treats Layer the same regardless of
  // version) keeps working.
  function readLayerPostV1100(this: any, stream: any, version: number): void {
    const start = stream.index
    const size = stream.readUint32()
    this.filterMode = stream.readUint32()
    this.flags = stream.readUint32()
    // texture_id present in file but unused in the post-v1100 format — each
    // layer carries its own per-texture struct in the texs array below.
    // HiveWE skips it via `reader.advance(4)`. The lib's pre-v1100 model
    // reads it into this.textureId; we set -1 here so downstream code that
    // would look at it sees a sentinel rather than stale data.
    stream.readInt32() // ignored
    this.textureId = -1
    this.textureAnimationId = stream.readInt32()
    this.coordId = stream.readUint32()
    this.alpha = stream.readFloat32()
    this.emissiveGain = stream.readFloat32()
    stream.readFloat32Array(this.fresnelColor)
    this.fresnelOpacity = stream.readFloat32()
    this.fresnelTeamColor = stream.readFloat32()
    // NEW in post-v1100:
    const hd = stream.readUint32()
    const texsCount = stream.readUint32()
    // Stash for any downstream consumer that wants to know whether the
    // layer was authored as HD-only. The mdx-m3-viewer Layer class doesn't
    // declare these, so use bracket access via `as any` semantics. The viewer
    // pipeline doesn't read these — the texture binding has already routed
    // through `viewer.load(path)` for both SD and HD paths.
    ;(this as any).hd = hd
    // For each inline tex entry: read (id, slot). The id indexes the model-
    // level Textures chunk; the slot is the PBR role (0 diffuse, 1 normals,
    // 2 orm, 3 emissive, 4 teamColor, 5 environment). Pre-v1100 HD models
    // carried each of these as a SEPARATE layer; post-v1100 collapses them
    // into this one layer's texs array. Stash the whole array so the Material
    // patch can expand it back into the per-slot layers mdx-m3-viewer's HD
    // renderer expects (see patchedMaterialReadMdx). We also set this.textureId
    // to the first (diffuse) id so the pre-v1100 single-texture codepath keeps
    // working for any layer that doesn't get expanded.
    const texs: Array<{ id: number; slot: number }> = []
    for (let t = 0; t < texsCount; t++) {
      const id = stream.readInt32()
      const slot = stream.readInt32() // HiveWE: "usually slot order, sometimes garbage"
      texs.push({ id, slot })
      // The per-tex inline KMTF track is technically possible per HiveWE's
      // reader (peek the next 4 bytes, if 'KMTF' consume the track). We
      // skip that for now — if subsequent track tags appear in the remaining
      // animations area, readAnimations will skip them via the 'K' filter.
    }
    ;(this as any).texs = texs
    if (texs.length > 0 && texs[0].id >= 0) this.textureId = texs[0].id
    // Whatever bytes remain in this layer's size are KMTA/KMTE/KFC3/KFCA/
    // KFTC animation tracks. Use the lib's readAnimations (animatedobject
    // protocol). The patched-for-safety version (see below) handles
    // unknown tags by skipping to `end` instead of crashing on
    // `animationmap[undefined][1]`.
    this.readAnimations(stream, size - (stream.index - start))
  }

  // The version dispatcher. We need to know the file's VERS to choose between
  // the lib's original Layer.readMdx (good for VERS 800/900/1000) and our
  // new post-v1100 reader. The Material chunk's readMdx receives `version`
  // — capture it there and stash on `this` (the layer being constructed)
  // via a closure variable set on the Material's iteration loop.
  Layer.prototype.readMdx = function patchedLayerReadMdx(stream: any, version: number): void {
    if (version >= 1100) {
      readLayerPostV1100.call(this, stream, version)
    } else {
      origLayerReadMdx.call(this, stream, version)
    }
  }

  // Material.readMdx — for VERS >= 1100 the shader field is dropped (LAYS
  // appears immediately after flags). For VERS 900/1000 the lib's
  // readMdx is correct; we still peek-LAYS to handle pre-1.33 stock assets
  // that drop the shader field (the previous patch handled that case).
  Material.prototype.readMdx = function patchedMaterialReadMdx(stream: any, version: number): void {
    stream.readUint32() // size — lib's reader also ignores this
    this.priorityPlane = stream.readInt32()
    this.flags = stream.readUint32()
    // Peek-LAYS: detects shader-dropped layouts regardless of declared
    // version. Reforged 1.33+ (VERS >= 1100) ALWAYS drops shader; some
    // pre-1.33 stock assets (VERS = 900/1000 with hand-stripped shader)
    // also drop it. The peek decides per-material rather than per-file.
    const u = stream.uint8array as Uint8Array
    const i = stream.index
    let peekLAYS = false
    if (i + 4 <= u.length) {
      peekLAYS = u[i] === 0x4C /* L */ && u[i+1] === 0x41 /* A */
              && u[i+2] === 0x59 /* Y */ && u[i+3] === 0x53 /* S */
    }
    if (!peekLAYS && version > 800) {
      this.shader = stream.read(80)
    } else {
      this.shader = ''
    }
    stream.skip(4) // LAYS
    const numLayers = stream.readUint32()
    for (let n = 0; n < numLayers; n++) {
      const layer = new Layer()
      // Dispatches via patched Layer.readMdx → readLayerPostV1100 for
      // VERS >= 1100, origLayerReadMdx otherwise. For HD (post-v1100)
      // materials the single layer stashes its texs[] + hd flag; the actual
      // expansion into the lib's six-layer HD layout happens AFTER the whole
      // model parses (see reconstructHdMaterials, wired into Model.load),
      // because it needs the TEXS chunk — which comes after MTLS — to clamp
      // each slot's texture id to a valid index.
      layer.readMdx(stream, version)
      this.layers.push(layer)
    }
  }

  // Reforged "tree" replaceable-texture bases (mirror of mdx-m3-viewer's
  // handlers/mdx/replaceableids). In the SD era each id mapped to ONE texture;
  // the HD assets split it into per-slot files (<base>_Diffuse/_Normal/_ORM).
  const TREE_REPLACEABLE_BASE: Record<number, string> = {
    31: 'LordaeronTree/LordaeronSummerTree',
    32: 'AshenvaleTree/AshenTree',
    33: 'BarrensTree/BarrensTree',
    34: 'NorthrendTree/NorthTree',
    35: 'Mushroom/MushroomTree',
    36: 'RuinsTree/RuinsTree',
    37: 'OutlandMushroomTree/MushroomTree',
  }
  // HD per-slot filename suffixes, indexed by the lib's HD layer slot order.
  const SLOT_SUFFIX: Record<number, string> = { 0: '_Diffuse', 1: '_Normal', 2: '_ORM', 3: '_Emissive' }

  // ---- HD material reconstruction (the load-bearing part) ----
  //
  // mdx-m3-viewer predates the post-v1100 MDX format. Its HD render path
  // (handlers/mdx/setupgeosets.js + batchgroup.js) is gated on
  // `material.shader === 'Shader_HD_DefaultUnit'` and destructures EXACTLY six
  // layers per material — `[diffuse, normals, orm, emissive, teamColor,
  // environment]` — the pre-v1100 layout. The post-v1100 file drops the shader
  // field (material.shader === '') and folds all six PBR textures into ONE
  // layer's `texs` array.
  //
  // Left as-is, `isHd` is false, so an HD geoset (whose skinning lives ENTIRELY
  // in its SKIN chunk — the legacy vertexGroups are empty) is bound through the
  // SD vertex-group path. That reads the 8-byte per-vertex skin data (4 bone
  // ids + 4 weights) with the wrong stride, so bone indices come out as garbage
  // and every vertex snaps toward bone 0 / the world origin → doodads and trees
  // stretch from the origin to infinity on Reforged maps. (geoset/bone COUNTS
  // look fine — only the on-screen skinning is wrong — which is why a
  // parse-success check never caught it.)
  //
  // Fix: translate the post-v1100 representation forward into the pre-v1100 one
  // the lib understands — expand the single layer into six slot-ordered layers
  // (clamping each slot's texture id to a real index so the lib's UNGUARDED
  // `textures[id].texture` deref can't crash the whole render loop) and restore
  // the shader marker. This routes HD geosets through the correct hdSkinShader
  // (#define SKIN) + bindSkin (stride-8) path.
  function reconstructHdMaterials(model: any): void {
    const texCount = Array.isArray(model.textures) ? model.textures.length : 0
    // No textures → we cannot safely feed the HD path (it dereferences every
    // slot's texture). Leave such degenerate materials alone.
    if (texCount === 0) return
    const valid = (id: number): boolean => id >= 0 && id < texCount
    for (const material of model.materials || []) {
      if (material.shader !== '') continue // already HD (pre-v1100) or SD
      const layers = material.layers
      if (!layers || layers.length === 0) continue
      if (!layers.some((l: any) => l.hd === 1)) continue // not an HD material
      const base = layers[0]
      const texs: Array<{ id: number; slot: number }> = base.texs || []
      // Resolve Reforged "tree" replaceable textures (ids 31–37) to their real
      // per-slot HD files. The lib's replaceable-id table only knows the bare
      // base name, so without this it resolves diffuse/normal/orm ALL to the SD
      // texture: the HD shader then samples the diffuse as the normal+orm maps,
      // and orm.a (team-color factor) reads the foliage diffuse's alpha (~1), so
      // `color *= teamColor` blackens the whole tree. Each tree slot ships as a
      // separate file (<base>_Diffuse/_Normal/_ORM.dds), so point each
      // empty-path tree-replaceable texture at the right one (literal path,
      // replaceableId cleared so the lib uses it verbatim). Non-tree-replaceable
      // slots (emissive=Black32, teamColor, environment) already have real paths
      // or ids and are left untouched.
      for (const { id, slot } of texs) {
        if (!valid(id)) continue
        const tex = model.textures[id]
        if (!tex || tex.path !== '') continue
        const baseName = TREE_REPLACEABLE_BASE[tex.replaceableId]
        const suffix = SLOT_SUFFIX[slot]
        if (!baseName || !suffix) continue
        tex.path = `ReplaceableTextures\\${baseName}${suffix}.dds`
        tex.replaceableId = 0
      }
      // Diffuse fallback for any slot we can't resolve — always a valid index.
      const diffuse = (() => {
        const d = texs.find(t => t.slot === 0) || texs[0]
        return d && valid(d.id) ? d.id : 0
      })()
      // Resolve a slot's texture id: by slot field (usually authoritative),
      // then positionally, then the diffuse fallback. Every return is clamped
      // valid, so the lib never dereferences an undefined texture.
      const pick = (slot: number): number => {
        const bySlot = texs.find(t => t.slot === slot)
        if (bySlot && valid(bySlot.id)) return bySlot.id
        const byPos = texs[slot]
        if (byPos && valid(byPos.id)) return byPos.id
        return diffuse
      }
      // Slot 0 (diffuse) reuses the parsed layer — it carries the material's
      // filterMode/coordId/alpha and any KMTA/KMTF animation tracks, which the
      // HD render path reads from `diffuseLayer` only.
      base.textureId = pick(0)
      const six: any[] = [base]
      for (let s = 1; s <= 5; s++) {
        const L: any = new Layer()
        L.filterMode = base.filterMode
        L.flags = base.flags
        L.coordId = base.coordId
        L.alpha = base.alpha
        L.textureAnimationId = -1
        L.textureId = pick(s)
        L.hd = 1
        six.push(L)
      }
      material.layers = six
      material.shader = 'Shader_HD_DefaultUnit'
    }
  }

  // Run the HD reconstruction after a full model parse, when the TEXS chunk is
  // available. Wrap the mdlx Model's load so both the runtime MdxModel handler
  // and any direct parser use get fixed materials.
  const MdlxModel = MV?.parsers?.mdlx?.Model
  if (MdlxModel && !(MdlxModel.prototype as any).__wc3ForgeHdFixup) {
    const origModelLoad = MdlxModel.prototype.load
    MdlxModel.prototype.load = function patchedModelLoad(this: any, ...loadArgs: any[]): any {
      const result = origModelLoad.apply(this, loadArgs)
      try { reconstructHdMaterials(this) } catch { /* never let the fixup break a parse */ }
      return result
    }
    ;(MdlxModel.prototype as any).__wc3ForgeHdFixup = true
  }

  // Safety-belt readAnimations. The lib's original implementation crashes
  // when it encounters an animation tag missing from the hardcoded
  // AnimationMap (`new animationmap[name][1]()` → TypeError on missing key).
  // Our override does two things:
  //   1. Tolerates unknown K-prefixed tags by skipping to chunk end rather
  //      than throwing — defense-in-depth for any future MDX revision.
  //   2. Tolerates non-K leading bytes the same way — a misaligned read
  //      (e.g. from a downstream parser bug) skips clean to the chunk end
  //      instead of taking down the whole model parse.
  // Pushed parsed animations onto `this.animations`, same shape the lib's
  // downstream handler code expects (handlers/mdx/animatedobject.js
  // constructor walks `object.animations` to build the runtime Map of
  // sequences by tag).
  //
  // **CRITICAL**: AnimationMap MUST be sourced from the lib's internal
  // module path (see the import comment at the top of this file). The lib
  // does NOT re-export it through `parsers.mdlx`, so a previous version
  // of this patch that read `MV.parsers.mdlx.AnimationMap` silently got
  // `undefined` → every tag matched the "no entry" branch → animations
  // were always dropped → bones froze. That was commit dc0aaa0's
  // diagnosis; consolidated here.
  if (!animationMap || typeof animationMap !== 'object' || !animationMap['KGTR']) {
    throw new Error('mdx-parser-patch: failed to import animation map (KGTR not found) — skeletal animation would be broken')
  }
  const AnimatedObjectProto = Object.getPrototypeOf(Layer.prototype)
  if (AnimatedObjectProto && typeof AnimatedObjectProto.readAnimations === 'function') {
    AnimatedObjectProto.readAnimations = function patchedReadAnimations(stream: any, size: number): void {
      const end = stream.index + size
      while (stream.index < end) {
        const name = stream.readBinary(4)
        // Every legitimate animation tag starts with 'K' (KGTR, KMTA, KMTF, …)
        // — a non-K byte is either a future format we don't handle, or a
        // misaligned read. Either way: skip the rest of the animation block.
        // The chunk-level `size` field already told us where it ends so this
        // is a clean recovery without losing chunk alignment.
        if (typeof name !== 'string' || name.length < 1 || name.charCodeAt(0) !== 0x4B /* K */) {
          stream.index = end
          return
        }
        const entry = animationMap[name]
        if (entry) {
          const Ctor = entry[1]
          const animation = new Ctor()
          animation.readMdx(stream, name)
          this.animations.push(animation)
        } else {
          // Unknown but K-prefixed tag — recovery is the same. Animations
          // are visual polish; geometry/bones are what we strictly need
          // and they've already been read by this point.
          stream.index = end
          return
        }
      }
    }
  }

  // ---- HD per-layer blending (Circle-of-Power glow rings, etc.) ----
  //
  // The lib's HD batch render (handlers/mdx/batchgroup.js) hardcodes, once per
  // batch group: `gl.disable(BLEND); gl.depthMask(true)` — then draws every HD
  // batch with that state, ignoring each layer's filterMode for blending (it
  // only uses filterMode in the shader, for the 1-bit-alpha discard). So HD
  // materials authored as Blend/Additive (filterMode >= 2) — the Circle of
  // Power's AuraRune glow rings, magic effects, etc. — render as OPAQUE, depth-
  // writing quads. A dark-with-alpha glow texture then paints big black quads
  // over the model and terrain. (The SD path is fine: it calls layer.bind(),
  // which sets per-layer blend.)
  //
  // The runtime Layer already precomputes `.blended/.blendSrc/.blendDst` and
  // `.depthMaskValue` from filterMode. So: in BatchGroup.render, before the lib
  // draws, stash the diffuse layer's blend state on each HD batch's geoset;
  // then in Geoset.bindHd (called per batch immediately before the draw) apply
  // it. Opaque/1-bit materials (filterMode 0/1 — including trees) resolve to
  // blended=false + depthMask=true, i.e. exactly the lib's current behavior, so
  // only genuinely-blended HD layers change.
  const BatchGroup: any = (batchGroupModule as any)?.default ?? batchGroupModule
  const Geoset: any = (geosetModule as any)?.default ?? geosetModule
  if (BatchGroup?.prototype && Geoset?.prototype && !(Geoset.prototype as any).__wc3ForgeHdBlend) {
    const origBindHd = Geoset.prototype.bindHd
    Geoset.prototype.bindHd = function patchedBindHd(this: any, shader: any, skinningType: any, coordId: any): void {
      origBindHd.call(this, shader, skinningType, coordId)
      const b = this.__wc3HdBlend
      if (b) {
        const gl = this.model.viewer.gl
        if (b.blended) {
          gl.enable(gl.BLEND)
          gl.blendFunc(b.src, b.dst)
        } else {
          gl.disable(gl.BLEND)
        }
        gl.depthMask(b.depthMask)
      }
    }

    const origRender = BatchGroup.prototype.render
    BatchGroup.prototype.render = function patchedRender(this: any, instance: any): void {
      // Only HD batch groups need the fix; SD already blends via layer.bind().
      if (this.isHd) {
        const batches = this.model?.batches || []
        for (const idx of this.objects) {
          const batch = batches[idx]
          const dl = batch?.material?.layers?.[0] // runtime diffuse layer
          if (batch?.geoset && dl) {
            batch.geoset.__wc3HdBlend = {
              blended: !!dl.blended,
              src: dl.blendSrc,
              dst: dl.blendDst,
              depthMask: !!dl.depthMaskValue,
            }
          }
        }
      }
      return origRender.call(this, instance)
    }
    ;(Geoset.prototype as any).__wc3ForgeHdBlend = true
  }

  ;(globalThis as any)[PATCHED_FLAG] = true
}
