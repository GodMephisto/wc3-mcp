---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "docs/metal-renderer-status.md"
ref: HEAD
---

# macOS Metal Renderer Status

This branch adds a native macOS Metal renderer beside the existing OpenGL renderer. Metal is enabled by default for the macOS CMake presets and is used by the map canvas, model editor, and object-grid preview surfaces. Non-macOS builds keep the existing OpenGL path.

## Current PR Evidence State

The named evidence sections later in this document are historical snapshots from earlier implementation commits unless they explicitly match the commit under review. For current review work, regenerate the source-current artifacts from a clean tree and use the reports written under the local build directories:

- Debug package validation: `build/macOS-Debug-WithTests/package-validation/debug-package-report.json` and `.md`. This should be run with `--require-full-smoke --warcraft-dir "<Warcraft III>"` when proprietary Warcraft content is available, so the report records `full_installed_smoke: true` and points at `full-metal-smoke.png` plus `full-metal-smoke-artifacts/metal-smoke-provenance.json`.
- Release package validation: `build/macOS-Release/package-validation/release-package-report.json` and `.md`, covering the Release installed app, Warcraft-free save/reload, mutated save/reload, MPQ export/reload, renderer probes, preview probes, same-app zero-diff parity gates, CPack DMG creation, DMG checksum verification, and mounted-DMG bundle validation.
- Metal candidate PR capture: `build/macOS-Debug-WithTests/pr-evidence/metal/hivewe-metal-artifacts.zip`, `build/macOS-Debug-WithTests/pr-evidence/metal/hivewe-metal-mpq-map-artifacts.zip`, and `renderer-pr-capture-summary.md`, captured from the installed Debug app and source-pinned with `--require-source-head <reviewed sha>`.
- OpenGL fallback preservation on macOS: `cmake --build --preset macOS-Debug-OpenGL` followed by `build/macOS-Debug-OpenGL/HiveWE.app/Contents/MacOS/HiveWE --print-render-backend`. This is only a compile/backend-identity gate on macOS because Apple's OpenGL runtime cannot provide the OpenGL 4.5 profile required for visual parity capture.

Final maintainer-facing parity still requires Windows/Linux OpenGL 4.5 reference capture bundles for the same reviewed source revision, default and MPQ OpenGL-vs-Metal parity reports, and a completed manual visual-review note. Until those artifacts exist, local macOS Metal evidence can prove the Metal package/runtime side and OpenGL fallback compilation, but not final cross-backend visual parity.

### Current Source Checkpoint

The latest source commits after the full local evidence refresh are focused
runtime-hardening and renderer-abstraction cleanup:

- `f0ad3cc7` reports model-load failures with both attempted `.mdx` and `.mdl`
  paths instead of throwing `bad access to std::expected` when neither fallback
  exists.
- `e23889bf` makes the preview load-failure smoke reject that expected-access
  regression and require the missing model path in the diagnostic.
- `494b2298` guards legacy OpenGL preview/widget headers behind
  `HIVEWE_BUILD_OPENGL_RENDERER`, so Metal-only macOS builds cannot accidentally
  include those fallback declarations.
- `7872784b` renames the upload profiling bucket from `GL upload` to
  `Renderer upload`, matching the backend-neutral texture upload path.
- `be7e53d5` records the newer evidence state here and documents that
  `GPUTexture` remains the historical resource name, but it is decoded CPU
  texture data only; concrete OpenGL/Metal texture objects and residency are
  renderer-cache state.
- `de161d65` removes the unused global `GLWidget* context` pointer and drops the
  `Globals` module dependency on the map widget header.
- `d733c01b` renames the main promoted map wrapper from `GLWidget` to
  `MapCanvasWidget`; the concrete fallback canvas remains `OpenGLMapCanvas`.
- `00d96d7a` makes the wrapper `makeCurrent()` and `doneCurrent()` API
  OpenGL-only, so Metal builds no longer expose no-op current-context methods.
- `b90966ca` renames the main map-canvas source/header files to
  `map_canvas_widget.*`, updates the promoted Qt widget header, keeps both old
  and new map-canvas paths in the renderer-PR evidence matcher, centralizes
  Metal runtime selection through `runtime_config.h`, and keys HD water texture
  loading off that runtime capability.
- `73c4f9ac` applies the shared Metal runtime config to model-editor/object-grid
  widget selection and the legacy OpenGL terrain resource modules, removing the
  remaining local runtime-selection macros outside Metal-only widget files.
- `15845cf4` switches the Metal-only model-editor and object-grid widget file
  guards to the same shared runtime config.
- `c4f9cabe` moves `Map`'s render-manager ownership from a by-value member to
  an owned pointer, while keeping concrete renderer state behind
  `RenderManager` and the abstract `IMapRenderer`.

Focused validation for these commits completed with the Debug Metal app build,
targeted Metal preview probes, and the macOS OpenGL fallback build/backend
identity check. After the map-wrapper cleanup, the Debug Metal app and macOS
OpenGL fallback app were rebuilt through `73c4f9ac`, the fallback executable
reported backend `opengl`, `python3 tools/prepare_renderer_pr_evidence.py
--self-test` passed with explicit `map_canvas_widget` path coverage, and the
Debug Metal app was rebuilt again through `15845cf4` for the Metal-only widget
guard cleanup.

The broad local evidence snapshot below was regenerated from a clean tree for
`05403e5ad6a853ba3d44f8ffbd5d7588e514ecb2`. A later render-manager ownership
cleanup (`c4f9cabe`) has focused validation: the Metal Debug app target builds,
the macOS OpenGL fallback app target builds, the rebuilt apps report backends
`metal` and `opengl`, and the current Metal app passed a targeted Warcraft-backed
map smoke covering save/reload, render modes, terrain/cliff material smoke, and
picking. For maintainer review, the broad package/capture evidence must still be
regenerated from a clean tree for the final reviewed `HEAD`, then checked with
`python3 tools/check_renderer_evidence_current.py`.

### Latest Full Local Validation Snapshot

For implementation revision `05403e5ad6a853ba3d44f8ffbd5d7588e514ecb2`
(`Document Metal widget guard cleanup`), the local macOS Metal evidence set was
regenerated from a clean source tree on 2026-06-02:

- Debug package validation passed with 24/24 checks true, including
  source/build provenance, bundled resources, Metal backend identity,
  forbidden-dylib checks, codesign, CPack DMG build and mounted-DMG
  verification, save/reload, mutated save/reload, MPQ export/reload, renderer
  probes, same-app renderer-probe parity, map-canvas smoke, same-app map-render
  parity, MPQ map-render parity, preview probes, same-app preview parity,
  shared preview probes, same-app shared preview parity, and full installed
  Metal runtime smoke with Warcraft content. The installed full-smoke capture
  was `1920x897` with `1720521` varied pixels, and the map-render smoke
  artifacts were `1920x897` with 31 validated PNG artifacts under
  `build/macOS-Debug-WithTests/package-validation/full-metal-smoke-artifacts`.
- Release package validation passed 23/24 checks true: the same installed-app,
  smoke/probe, same-app parity, CPack DMG, checksum, mounted-DMG, and bundle
  validation gates passed, while full installed Warcraft-backed runtime smoke
  was intentionally skipped for Release because the Debug package run supplied
  the required full-smoke artifact set.
- The installed Debug app produced and packaged the default Metal PR capture
  archive `build/macOS-Debug-WithTests/pr-evidence/metal/hivewe-metal-artifacts.zip`
  and the MPQ export/reload map-render archive
  `build/macOS-Debug-WithTests/pr-evidence/metal/hivewe-metal-mpq-map-artifacts.zip`.
  The capture manifests record clean source `05403e5ad6a8`, backend `metal`,
  executable SHA-256 `202416f58d1f9c19cc5980f923f6d49ceed93ec60fd3364bd1201161c181c141`,
  picking coverage, the default saved/reloaded map+preview mode with 25 PNG
  artifacts, and the MPQ map-render-only export/reload mode with 9 PNG
  artifacts.
- The local macOS OpenGL fallback build completed for the same clean source
  revision. The fallback executable reported `opengl`, and the fallback build
  provenance records Debug `enable_metal=0` and `build_opengl_renderer=1`.
- The local audit with `--allow-missing-opengl` and
  `--allow-missing-manual-review` reported 9 passes, 4 warnings, and 0 failures
  for that Metal-side evidence set. The warnings are the expected missing
  Windows/Linux OpenGL 4.5 reference capture, missing default parity report,
  missing MPQ map-render parity report, and missing manual visual-review note.
- `tools/create_renderer_pr_handoff.py` assembled the source-pinned Metal-side
  handoff bundle
  `build/macOS-Debug-WithTests/pr-evidence/final-metal-handoff.zip`, containing
  the Metal default and MPQ archives, Debug and Release package reports,
  installed full-smoke frame/artifacts, and the local Metal-only audit.

Committed documentation can only record a snapshot. Every later commit, including a documentation-only commit, changes `HEAD` and makes previously source-current reports stale. To determine whether local evidence is current for the commit under review, run the lightweight source-revision checker before spending time on a full package or parity audit:

```shell
python3 tools/check_renderer_evidence_current.py
```

That command checks the default local Debug package report, Release package report, Metal default capture, Metal MPQ capture, full Metal smoke provenance, and macOS OpenGL fallback build provenance. Each must record the requested `source_revision.head` with `dirty=false`; the fallback provenance must also record `enable_metal=0` and `build_opengl_renderer=1`. After regenerating current local evidence, run `tools/audit_renderer_pr_evidence.py` with `--allow-missing-opengl --allow-missing-manual-review` for a macOS Metal-side progress audit. A strict final audit must omit those allowances and supply the Windows/Linux OpenGL capture bundles, default and MPQ parity reports, and completed manual review note.

## Current Coverage

- Terrain, cliffs, ground textures, water textures, pathing overlays, brush overlays, wireframe, and lighting are rendered through a Metal frame-renderer interface backed by `MetalMapRenderer`; the Metal map canvas depends on `IMapFrameRenderer`, while the model editor and object-grid preview widgets depend on the narrower `IModelFrameRenderer` instead of constructing the concrete renderer directly. Metal water blending now uses the same source-alpha factors as the OpenGL water pass, Metal builds can load complete HD water texture sets without forcing the legacy SD water assets, map-wide wireframe state remains active for overlay and water passes like OpenGL polygon mode, terrain material uploads track frame source signatures and preserve `Terrain::texture_revision` through the map canvas so fallback-to-real and real-to-fallback texture transitions cannot reuse stale texture arrays, the shared map-frame validator rejects corrupt ground texture IDs/layers and water layer indices before Metal uniform upload, and cliff material uploads normalize out-of-range cliff texture IDs before the shader sees them.
- Metal terrain CPU frames consume `Terrain`'s maintained render-height, ground-texture, tile-sized ground-exists, and water-exists data instead of duplicating that derivation, and terrain, pathing, brush frame validation, terrain/water draw instance counts, plus optional fallback-buffer generation now check oversized dimensions without signed overflow before upload or encoding.
- Metal terrain material frames preserve valid ground, water, or cliff texture categories when another category is malformed, letting the renderer use per-category fallback resources instead of dropping unrelated valid material uploads.
- Metal terrain material validation now rejects malformed ground texture layer-offset tables and overflowing texture dimensions before upload, and the terrain texture-frame builder plus R8 texture upload path use checked source-size/tile-span arithmetic so corrupt offsets or sizes cannot index outside Metal texture data or force oversized fallback/upload allocations.
- Metal map-canvas texture uploads retry when expected ground, water, or cliff categories are still using fallback resources, so a late fallback-to-real texture transition cannot remain stuck just because the ground category was already valid.
- Metal cliff texture-array expansion repeats source texels into padded areas instead of leaving transparent black padding, and cliff pathing overlays now combine static and dynamic pathing masks like the terrain pass.
- Metal cliff-frame uploads now reject invalid cliff mesh buffers, indexed vertex references, partial-triangle draw index counts, and instance ranges before encoding.
- The OpenGL cliff path now mirrors the Metal frame builder for malformed maps by clamping out-of-range cliff texture IDs to layer 0, and its cliff pathing shader combines static and dynamic pathing masks like the terrain pass.
- The OpenGL terrain and cliff shaders now clamp top/right border normal samples to the last valid map row/column, matching the Metal shader bounds behavior and avoiding out-of-range height-buffer reads in reference captures.
- SD and HD skinned model layers, team colors, alpha testing, blend modes, transparency sorting, particles, selection overlays, and click helpers are rendered through the shared Metal model path. Metal model draw frames now resolve MDX `KMTF` animated material texture IDs plus `KMTE` emissive gain, `KFC3` fresnel color, `KFCA` fresnel opacity, `KFTC` fresnel team-color, and sphere-environment material state per instance for HD draws and picking, skip malformed geosets whose material IDs are invalid or whose materials have no layers, and `SkeletalModelInstance` initializes/advances material alpha, emissive, fresnel, and texture-ID tracks together.
- Metal model and editable-preview mesh constructors now also skip malformed geosets with missing vertex, UV, normal, or index data, keep geoset-animation pointers aligned to the original MDX geoset IDs after filtering, and fall back to bone-0 skin weights when SD matrix-group data is incomplete.
- Metal model-frame uploads now reject invalid mesh buffers, draw index ranges, indexed vertex references, partial-triangle draw index counts, out-of-slice skin bone IDs, invalid blend/depth/cull draw state, material flags, picking IDs, team-color indexes, instance offsets, bone ranges, layer-color ranges, invalid texture wrap flags, and overflowing texture-source dimensions before encoding, and Metal RGBA texture uploads plus model-picking readbacks use checked row/frame sizing before allocation or GPU transfer.
- Meshless particle-only MDX models keep their texture CPU data in Metal builds, are accepted by the Metal particle frame builder for map, model editor, and object-grid rendering, and emit extent-based picking bounds for unit/doodad selection when no base mesh draw exists.
- Metal model and particle texture uploads preserve the same sRGB treatment used by the OpenGL SOIL path for non-BLP textures, and missing model/particle texture slots now use the same opaque magenta fallback texel as the OpenGL texture cache. Unknown replaceable model texture IDs and failed model texture loads leave null CPU texture slots instead of throwing during `SkinnedMesh` or `EditableMesh` construction, so the renderer fallback texture can be used for malformed or incomplete assets in map and model-editor surfaces.
- Metal model and particle texture caches track CPU texture source signatures separately from mesh geometry revisions, so late texture availability, texture-count changes, wrap-mode changes, and fallback-to-real texture transitions refresh Metal textures without forcing a mesh buffer rebuild.
- `MetalMapRenderer` records per-render model and particle texture fallback counts, and the Warcraft-free renderer probe now asserts the intentional missing-texture warmup uses fallbacks while the following late-texture frame has returned to real Metal texture uploads.
- Metal placement and paste brush previews append queued model PE2, PE1 TGA, PE1 spawned-child, and ribbon particle data into the map particle frame through the shared queued-preview frame builder, then re-run particle draw sorting so particle-only or particle-heavy unit/doodad previews render through the same Metal particle path and transparency ordering as placed map objects.
- Metal map, model-editor, and object-grid particle frames emit `RibbonEmitter` trails through the shared particle renderer using a CPU ribbon history buffer with ribbon material texture, atlas slot, blend mode, color, alpha, visibility, height, lifetime, emission-rate, and gravity state. A current-position quad remains as the first-frame fallback until a moving trail has enough samples.
- Metal texture-style `ParticleEmitter1` emitters marked `EmitterUsesTga` now run through a CPU simulation and the shared particle renderer. TGA sprite quads consume the simulated spawn orientation, transformed through the rendered instance, while synthetic pools without orientation data retain the camera-billboard fallback. Skinned meshes load their PE1 TGA/BLP/DDS paths into the particle texture table, and the synthetic model-editor/object-grid particle preview smoke includes a PE1 TGA emitter beside the PE2 emitter. Metal PE1 TGA, PE2, ribbon, and CORN particle frame builders fall back to an origin pivot when malformed models have a valid node matrix but no matching pivot entry, keep already-simulated particle data renderable when the instance has no active sequence, and validate vertex-buffer consistency, texture-source dimensions, filter modes, every draw range, and complete triangle-list batches before upload so stale or malformed particle draws cannot issue invalid GPU work.
- Metal `ParticleEmitter2` rendering resolves emitter-level replaceable texture IDs through the same default/override replaceable texture path as model materials, then falls back to the emitter's normal texture slot when a replaceable source is unavailable.
- Metal particle frames sort whole draw batches by MDX priority plane and then camera distance while preserving per-emitter `SortPrimsFarZ` primitive ordering, so PE2, PE1 TGA, and ribbon particle batches no longer depend on map/object append order for transparency.
- Metal model-spawning `ParticleEmitter1` emitters now load non-TGA emitter paths as child `SkinnedMesh` resources and append one child model instance per complete live PE1 position/age record through the shared Metal model frame. Child models inherit the PE1 emitter basis captured at spawn time, then sample their own birth/stand sequence by particle age for bone, layer-alpha, and geoset-animation state. Nested child model resources load with bounded depth- and ancestor-specific cache keys, so child PE1 model-spawn emitters simulate through their animated child poses by child particle age and append grandchild instances without unbounded resource recursion or self-wait cycles. Spawned child and grandchild models also append their own PE2, PE1 TGA, and ribbon emitters into the shared Metal particle frame, with origin-pivot fallback for malformed child models whose node matrices exist but pivot entries are missing. Already-live parent PE1 model-spawn pools keep rendering their spawned child model and child particles even when the parent instance has no active sequence. PE1 spawned child and grandchild model picking inherits the parent unit/doodad pick ID, and sparse/nonzero or missing bone node IDs keep a matching Metal bone-matrix stride for shader skinning while preserving available valid matrices. Map objects, placement/brush previews, model editor previews, and object-grid cells all use the same PE1 model-spawn append path; the Warcraft-free particle preview smoke writes a parent/child/grandchild MDX chain and validates the spawned child plus nested grandchild model draws alongside parent and spawned-child PE1/PE2/ribbon particle draws.
- CORN/PopcornFX model chunks now decode CornEmitter static fields and animation tracks for MDL export/import while preserving the exact raw MDX payload for untouched binary roundtrips; the MDL roundtrip tests cover folder and MDX save/load conversion for the decoded fields, and CPU extent calculation includes conservative CORN fallback billboard bounds so CORN-only models are not culled by empty extents. `SkeletalModelInstance` now initializes and advances CORN alpha, emission-rate, visibility, speed, lifetime, and color tracks, and the shared Metal particle-frame builder emits a bounded CORN fallback billboard with sampled color/alpha through the existing particle renderer so CORN-only models no longer disappear completely in Metal map, model-editor, or object-grid surfaces. Non-animating instances without an active valid sequence now use static CORN fields and model extents instead of dropping the fallback billboard entirely.
- `SkeletalModelInstance` now advances and initializes ribbon animation tracks, guards keyframe-table access against out-of-range track IDs, evaluates node parents before children even when MDX node IDs are not declaration ordered, falls back to an origin pivot when malformed models omit pivot rows for valid particle nodes, and updates ribbon history after node transforms so map objects, model editor previews, and object-grid cells share the same animated trail path.
- Metal clip-space depth conversion is applied consistently for the map canvas, picking, model editor, and object grid. Cliff textures use the same source-alpha blending as OpenGL, and Metal no-depth-test model states bypass depth writes like disabled OpenGL depth testing.
- Metal surface capture readback for model editor and object-grid previews uses checked BGRA row and buffer sizing before allocation or CPU copy.
- Unit and doodad picking use a Metal offscreen color target. The Metal map canvas now renders and picks Warcraft starting-location (`sloc`) units through the same shared model/picking path using the editor invalid-object marker mesh, so starting locations are visible and selectable even though they have no normal unit model; the reusable Metal unit-picking frame builder can now include the same marker mesh instead of relying on a canvas-local append path. The OpenGL fallback now renders and color-picks the same starting-location marker, so OpenGL reference capture and Metal candidate capture use matching visible/pickable starting locations. Doodad picking now mirrors the OpenGL ray/AABB candidate filter, includes click-helper bounds, includes live PE1 spawned child model instances with the parent doodad pick ID, and can pick helper-enabled doodads even when the base doodad mesh is absent or non-renderable. Metal and OpenGL picking frames emit the first visible renderable material layer per geoset while preserving that layer's depth/cull state, animated layer/geoset alpha, and alpha-test texture rejection; visible layers without draw state, or whose resolved layer alpha is below the layer alpha-test cutoff, fall through to the next renderable layer. The OpenGL reference picker now uses the requested mouse position for doodad rays and accepts helper-only doodad bounds without dereferencing an absent base mesh.
- Brush selection overlays are emitted through a backend-neutral CPU overlay frame and consumed by `OpenGLSelectionOverlayRenderer` and the Metal canvas; Metal validates overlay vertex-buffer consistency, draw ranges, primitive kinds, and style IDs before issuing draw calls.
- Brush masks are CPU-owned with a revision counter; OpenGL uploads them through the terrain render cache instead of storing a GL texture on `Brush`, and Metal reuses the brush mask texture until the brush source, size, or revision changes.
- Brush shape changes, object-brush model changes, and doodad/destructible object-field updates no longer force a current OpenGL context.
- Unit and doodad mesh preload/instance initialization now use a generic CPU task pool instead of shared OpenGL worker contexts.
- macOS Metal Play Test saves the map and launches the native Warcraft III app binary from `_retail_/x86_64/Warcraft III.app/Contents/MacOS/Warcraft III` when present, while preserving the existing Windows executable fallback for non-macOS/OpenGL builds.
- `Map::render()` delegates map-frame OpenGL state setup, terrain/cliff drawing, and water drawing to `RenderManager` instead of issuing those GL calls directly.
- `RenderManager` owns only the backend-neutral `IMapRenderer`; concrete OpenGL construction is isolated in the rendering-layer factory.
- `RenderBackend` owns only backend-neutral lifecycle contracts; OpenGL-specific backend lifecycle code is isolated in `OpenGLRenderBackend` under the guarded OpenGL renderer source set.
- Pathing map cells are CPU-owned with static/dynamic revision counters; OpenGL pathing textures are uploaded through the terrain render cache, and Metal pathing textures are reused until the matching revision changes.
- Shadow maps are CPU-owned with a revision counter; the previous unused OpenGL texture owner was removed from `ShadowMap`.
- Terrain edit paths update CPU render data and revision counters; cell-based water/boundary edits now bounds-check terrain width and height independently so rectangular maps do not skip valid rows or touch invalid cells before Metal frame generation. OpenGL terrain buffers are synchronized lazily by the terrain render cache during rendering, and Metal terrain buffers are reused until the terrain source, dimensions, inclusion flags, or matching revision changes.
- OpenGL terrain SSBO and ground-texture handle upload resources live in `OpenGLTerrainBufferResources` under the rendering layer.
- OpenGL cliff/water texture-array upload resources live in `OpenGLTerrainTextureArrayResources` under the rendering layer.
- OpenGL terrain, cliff, brush/pathing overlay, and water draw orchestration lives in `OpenGLTerrainRenderer` under the rendering layer instead of `Terrain`.
- Ground texture resources are CPU-decoded only; OpenGL ground texture arrays and bindless handles are owned by the terrain render cache.
- Cliff and water texture resources are CPU-decoded only; OpenGL terrain texture arrays are owned by the terrain render cache.
- CPU image decoding for texture resources and heightmap import is centralized in `ImageDecoding`; renderer/resource callers consume decoded pixel vectors instead of depending directly on SOIL2 allocation APIs.
- Cliff mesh resources are CPU-owned; OpenGL cliff vertex, index, and instance buffers are owned by the terrain render cache.
- Color-picking bone upload state is owned by the OpenGL map renderer instead of individual `SkinnedMesh` resources.
- Color-picking mesh draw state is owned by the OpenGL map renderer instead of individual `SkinnedMesh` resources.
- Per-frame skinned mesh instance/color/skeleton queues are owned by the OpenGL map renderer instead of `SkinnedMesh` resources.
- Static skinned mesh buffer ranges and uploads are owned by the OpenGL map renderer instead of `SkinnedMesh` construction.
- OpenGL skinned-model texture residency tracking is owned by the OpenGL map renderer instead of `SkinnedMesh` resources.
- OpenGL skinned-model bindless texture-handle uploads are owned by the OpenGL map renderer instead of `SkinnedMesh` construction.
- Shared skinned OpenGL mega-buffers live in `OpenGLSkinnedMeshResources` under the rendering layer instead of the resource layer.
- Shared skinned OpenGL allocation state is reset by `OpenGLMapRenderer` construction instead of the main window reload path.
- Model textures are CPU-decoded in `GPUTexture`; OpenGL texture IDs and bindless residency are owned by renderer-side texture caches.
- OpenGL model texture cache resources live in `OpenGLTextureResources` under the rendering layer.
- OpenGL shader programs are represented by `OpenGLShader` under the rendering layer instead of the resource layer.
- Terrain and trigger-editor icon conversion lives in backend-neutral `IconUtilities`; the legacy OpenGL quad helper now lives in `OpenGLShapeResources` under the rendering layer.
- CPU physics state no longer carries the unused OpenGL Bullet debug-draw helper.
- Editable model preview meshes keep CPU-side geometry in `EditableMesh`; OpenGL preview VAOs and buffers are owned by `OpenGLEditableMeshResources` under the rendering layer, and the legacy OpenGL model preview renderer skips invalid/empty material geosets and unbinds missing texture slots instead of dereferencing malformed material layers.
- The legacy OpenGL particle-emitter preview renderer lives in `OpenGLParticleEmitter2Renderer` under the rendering layer.
- Skinned mesh draw-state blend factors are backend-neutral and are translated to OpenGL or Metal constants inside the respective renderers.
- Shared skinned indirect draw command data uses fixed-width CPU integer fields instead of OpenGL typedefs.
- Model editor and object-grid previews use the shared Metal model renderer. The object grid clears depth per preview cell while preserving accumulated color output, the Metal renderer probe exercises that color-load/depth-clear contract with overlapping model passes, empty filtered grids present a valid cleared Metal frame instead of treating capture as a renderer failure, the Metal preview probe now filters a populated synthetic grid to zero cells and asserts the cleared frame has no loaded/rendered model work, the scrolled object-grid preview smoke captures a partially clipped row/header frame at a nonzero scroll offset, and Metal object-grid smoke readbacks paint the category-header overlay into the captured drawable so parity artifacts include the same headers as the visible widget.
- Metal model-editor and object-grid preview cameras clamp degenerate/zero extents to a usable minimum radius so particle-only or malformed preview models do not collapse the view matrix, and object-grid previews now frame sequence-less/static models from the model extent just like the model editor instead of using a hardcoded fallback distance.
- Metal preview smoke captures suppress Qt child controls and header labels while rendering the Metal frame, disable automatic preview frame timers during smoke capture, use a fixed smoke delta with a warm-up frame before capture for deterministic particle/model-spawn state, and preserve the captured object-grid header overlay.
- Map save/reload smoke validation is available through `--smoke-save-reload` and is registered as an always-on macOS Metal CTest; it verifies source-file preservation, explicitly byte-checks minimap, conversation metadata, and imported payload files that the save path is not expected to rewrite, validates required generated map files, then compares pre-save and post-reload terrain, static and dynamic pathing, shadows, units/items, doodads, object modification tables, triggers, imports, regions, cameras, sounds, and full `MapInfo` metadata including players, forces, tech/upgrades, random tables, flags, bounds, loading/prologue, fog, weather, and camera settings. After the final reload, the Metal path also builds the post-reload terrain, pathing, ground/water/cliff material texture, cliff, model, particle, unit-picking, and doodad-picking CPU frames, runs the shared Metal cliff/model renderer validators, and rejects incomplete texture slots, draw ranges, per-frame model buffers, cliff instances, picking draw buffers, or particle texture sources before reporting success. `--smoke-mutated-save-reload` stages a temporary map copy, mutates serialized terrain/water/cliff/pathing data, serialized shadow-map data, available unit/item/doodad placements, region/camera/sound data, object-table shadow data, trigger text, imported payloads, and `MapInfo`, then saves once to normalize serialized formats, reloads, proves the persisted map changed, saves/reloads again, compares the stronger post-mutation snapshot, and runs the same Metal renderer-frame validation; it is registered as a separate macOS Metal CTest. `--smoke-mpq-export-reload` stages the same in-memory map to a temporary folder, exports it as a `.w3x` MPQ archive with StormLib, unpacks it, reloads the extracted archive contents, compares the same map-state snapshot, and runs the same Metal renderer-frame validation; this is registered as a separate macOS Metal CTest and is also run before map-canvas render/picking smoke so Metal renders the post-archive-reload map. The bundled smoke map includes a non-reserved imported text asset so the import manifest exercises real payload preservation, not just reserved Warcraft map files.
- Map frame capture smoke validation is available through `--capture-frame <path>` with Metal drawable readback and OpenGL framebuffer capture.
- Render-mode smoke validation is available through `--smoke-render-modes`; it captures and rejects a flat deterministic terrain-only baseline, pins the smoke camera distance/orientation before each capture, stages temporary water and red/green/blue pathing patches around the camera focus, validates pathing, a deliberately colored brush overlay, lighting, water, wireframe, and click-helper modes independently, and renders a synthetic Metal map frame with readback checks for multi-layer ground texture variation, cliff texture-array layers, selected water texture layers, and particle colors.
- Rendered unit/doodad picking smoke validation is available through `--smoke-picking`; it proves unit, doodad, helper-only doodad, and starting-location picking independently on each renderer path when the startup map contains the matching object, and adds transient probes when the startup map has no visible pickable object of the required class.
- Warcraft-free Metal map canvas smoke validation is registered in CTest; when no explicit `--warcraft-dir` is provided, non-model smoke modes load bundled fixture data from `data/warcraft/` instead of saved Qt Warcraft settings, then load `data/test map/`, save/reload it, mutate serialized map data and save/reload again, export/unpack/reload it through MPQ, run render-mode and picking smoke through the real Metal map canvas startup path, render synthetic model editor/object-grid previews after the map is loaded, and write map and preview artifacts for CI upload. The bundled fixture contains minimal terrain/water/cliff, object-table, trigger-script, and editor-string data for deterministic CI startup; the map-canvas smoke now proves the Metal renderer, unit/doodad/helper-only picking, click-helper render toggle, and Warcraft-free preview widgets still work after save/reload, mutation save/reload, and MPQ export/reload cycles with scaled fixture probes. Full Footman/model-preview parity remains covered by the Warcraft-backed full runtime smoke.
- CTest fixtures clear smoke artifact directories before the artifact-producing Metal smokes run and register dependent artifact-manifest validators for the Warcraft-free Metal map canvas, renderer probe, backend-neutral shared preview probe, and Metal-specific preview probe smokes, so filtered and full CTest runs fail on missing, wrongly sized, invalid, or dominant-color-flat required PNG outputs. The synthetic renderer and preview artifact validator also rejects known probe PNGs that lack their expected material, picking, particle, spawned-model, ribbon, multicell, scrolled-cell, or header-overlay color buckets, so archived artifacts cannot satisfy the manifest with only dimensions and generic non-flatness. Strict map/probe manifests also reject stale or extra PNGs; shared/parity manifests allow intentional backend-specific extras while still validating all required outputs.
- Model preview smoke validation is available through `--smoke-model-previews`; it renders a known Footman model in the model editor preview and object-grid preview on both backends, rejects flat/non-textured real preview frames by checking changed-pixel color diversity, now also emits shared synthetic SD/HD/team-color/transparency/alpha-key material and meshless PE2 particle preview artifacts from both OpenGL and Metal, verifies the Metal widget-built model frames contain real meshes, textures, SD draw calls, alpha-tested layers, team-color layers, and an animated `KMTF` HD albedo slot, checks the widget captures for team-color, HD material, transparent SD, transparent HD, alpha-key, and particle color buckets, verifies the Metal particle-only widget frame also contains a PE1 spawned child model, renders synthetic SD/HD/team-color/transparency/alpha-key model, model color-load/depth-clear isolation, model-overlapped selection-overlay, and particle-only frames through the Metal renderer, captures readback pixels, and verifies targeted model/material, depth-isolation, overlay, and particle readbacks where applicable.
- Warcraft-free shared preview probe smoke validation is available through `--smoke-shared-preview-probes`; it runs the synthetic material-heavy and PE2 particle model editor/object-grid previews without loading a map, CASC, or bundled Warcraft fixture data. OpenGL uses the shared synthetic MDX/TGA assets directly, while Metal reuses the stronger Metal preview-probe path and emits the same shared artifacts plus Metal-only multicell replaceable-cache coverage.
- Warcraft-free Metal renderer probe smoke validation is available through `--smoke-metal-renderer-probes`; it runs the synthetic terrain/cliff/water/particle, pathing-overlay, cliff dynamic-pathing overlay, SD/HD material, model color-load/depth-clear isolation, alpha-tested picking, PE1 spawned-model picking, visible PE1 spawned child/grandchild rendering, meshless bounds picking, selection-overlay, and particle-only Metal renderer readbacks before CASC or map loading so CI can gate Metal GPU behavior without a Warcraft III installation. The model-material probe now renders lit HD draws that isolate normal-map lighting, ORM/team-mask blending, emissive-gain contribution, and fresnel material color instead of only proving a generic HD albedo bucket. The alpha-tested picking probe forces a 2x drawable scale, samples separate top/bottom quads so Metal pick readback covers logical-to-pixel scaling and top-left UI coordinates, and writes a colorized picking-target artifact from the same offscreen pass. The visible PE1 spawned-model render probe builds the same nested synthetic parent/child/grandchild chain used by the preview probes, emits unit and doodad particles through the map-object CPU PE1 model-spawn path, requires parent and spawned-child PE2, PE1 TGA, and ribbon particle frame data, then renders visible magenta PE1 TGA particles plus green child and blue grandchild model draws through the Metal map renderer; the PE1 spawned-model picking probe feeds child-model geometry through the same Metal offscreen picking pass and verifies the returned index still maps to the parent object pick ID. The meshless bounds picking probe feeds an extent-only CPU frame through that same pass and verifies particle-only unit/doodad fallback bounds return the expected pick index. The terrain pathing probe samples deterministic static red, dynamic green, and static blue pathing flags at known terrain cells; the cliff pathing probe renders a cliff-only frame over dynamic pathing with no terrain underneath so green pixels prove the cliff fragment combines dynamic pathing masks. The terrain/material, model-material, and particle probes also warm the renderer with missing textures and then capture after the same sources become available, and the terrain/material probe renders a final missing-texture frame after real cliff textures to exercise real-to-fallback cache refresh.
- Warcraft-free Metal preview probe smoke validation is available through `--smoke-metal-preview-probes`; it runs the synthetic SD/HD/team-color/transparency/alpha-key, meshless particle-only, and ribbon-only MDX models through the real Metal model editor and object-grid widgets before CASC or map loading, validates object-grid extent-based camera fitting for a sequence-less/static material model, validates distinct PE2, PE1 TGA, PE1 spawned-child, and nested spawned-grandchild pixel buckets in both particle preview surfaces, validates parent plus spawned-child PE2/PE1-TGA/ribbon particle draw counts, validates cyan ribbon pixels in dedicated ribbon preview surfaces, validates a nested PE1 model-spawn parent/child/grandchild resource chain with Birth-to-Stand child sequences, validates a filtered-empty object-grid frame with no loaded/rendered cells or model work, renders a two-cell object-grid material probe using the same MDX path with different replaceable texture overrides so the global `SkinnedMesh` cache cannot collapse distinct preview cells, and captures a scrolled object-grid frame with a clipped first cell plus later category rows to exercise negative viewport origins and clamped scissors.
- Smoke artifact capture is available through `--smoke-artifact-dir <dir>`; it writes PNGs for the validated map render-mode frames plus Metal terrain/cliff/particle, terrain pathing-overlay, cliff pathing-overlay, alpha-picking, PE1 spawned-model picking, PE1 spawned-model render, meshless bounds picking, model-material, model depth-isolation, selection-overlay, particle-only, model-editor, object-grid, model-editor-material, object-grid-material, object-grid-empty-filter, object-grid-load-failure, object-grid-multicell-material, object-grid-scrolled-material, model-editor-particle, object-grid-particle, model-editor-ribbon, and object-grid-ribbon readbacks so manual PR review can inspect the exact frames behind the automated smoke checks. Metal smoke artifact captures force deterministic 1x drawable output for fixed-size manifest validation while normal interactive widgets keep device-pixel-ratio rendering, except the picking probes intentionally capture the forced 2x drawable.
- Smoke artifact manifests and PNG validation live in `tools/compare_smoke_artifacts.py`; `tools/run_metal_smoke.py` and `tools/check_macos_package.py` import the same expected artifact names, dimensions, dominant-color flat-image checks, and real model-preview color-diversity checks so CI, package validation, and manual parity reports cannot drift independently. The wrapper tools and CTest fixtures clean artifact directories before grouped captures, the Python wrappers suppress bytecode output so self-tests do not dirty the source tree, and the app-side PNG writers create directories as needed before each save.
- Smoke validation code now lives outside the app entry point: save/reload hashing is in `src/smoke/smoke_save_reload.cpp`, render/preview probes are in `src/smoke/smoke_render.cpp`, and `src/main.cpp` only coordinates command-line options and scheduling. Save/reload smoke frame validation now applies the shared Metal map-frame texture-index validator to terrain and material frames before accepting post-reload renderer state.
- Backend-independent Metal model-frame tests cover SD/HD texture-slot propagation, animated `KMTF` material texture IDs, out-of-order skeletal parent transforms, material blend/depth/cull/team-color state, invalid blend/depth/cull/material-flag/picking-ID/team-color-index draw-state rejection, invalid texture wrap-flag rejection, unknown replaceable texture IDs falling through to renderer fallbacks for both `SkinnedMesh` and `EditableMesh`, malformed blend-factor normalization, no-depth-test draw state, transparent draw sorting, malformed-geoset material skipping, PE1 spawned child model instance placement, PE1 spawned child rendering from already-live parent pools without an active parent sequence, PE1 spawned child picking draw emission, meshless unit/doodad and PE1 child extent-bounds picking, clamped incomplete PE1 model-spawn pool records, model-spawn simulation basis normalization under nonuniform scale, orthonormal spawn bases from sheared nested transforms, child transform-track sampling, PE1 child-model out-of-order parent transforms, sparse child bone-node matrix uploads, missing skin-bone identity padding that preserves available matrices, child spawn-basis preservation, child Birth-to-Stand sequence transitions, child global-sequence sampling from total particle age, nested PE1 model-spawn simulation from selected-sequence local age, bounded nested PE1 model-spawn recursion including self-referential frame links at the depth boundary, PE1 child/grandchild picking ID propagation, first-visible-layer picking behavior across single- and multi-geoset meshes, non-renderable visible-layer fallback, alpha-key picking fallback below the alpha-test cutoff, invisible-layer picking behavior before frames reach the GPU renderer, and object-grid replaceable-texture cache-key separation before widget smoke reaches the renderer.
- Backend-independent Metal particle-frame tests cover meshless particle-only models, missing-pivot fallback for PE1/PE2/ribbon `SkeletalModelInstance` simulation, PE2/ribbon/CORN particle draws, PE1/PE2/ribbon rendering without an active sequence, and PE1 spawned-child particle simulation, appending and sorting queued brush/preview particles after an existing map particle frame, `ParticleEmitter1` TGA sprite emission, spawned PE1 child-model PE2/PE1-TGA/ribbon particle emission, instance-transformed TGA sprite spawn orientation, and simulation with stable particle IDs and spawn bases for model-spawn consumers, RibbonEmitter first-frame quads, simulated RibbonEmitter trail history, CORN fallback billboard emission with sampled color/alpha, ParticleEmitter2 replaceable texture preference and fallback, ParticleEmitter2 filter-mode propagation, invalid filter-mode rejection, atlas UV selection, segment color/alpha interpolation, model-space and XY-quad flags, far-Z primitive sorting, priority-plane/distance draw-batch sorting, draw-range validation, clamped incomplete particle/ribbon records, and tail geometry before frames reach the GPU renderer.
- Backend-independent Metal brush and selection-overlay tests cover the shared brush-mask and brush-overlay to Metal CPU-frame bridges plus malformed overlay draw-range rejection before frames reach the GPU renderer.
- Backend-independent Metal terrain-frame tests cover terrain heights, boolean ground/water existence masks, water/pathing CPU data, terrain material texture revision propagation, ground/water/cliff texture expansion, map-frame ground/water texture index rejection, and cliff mesh instance filtering before frames reach the GPU renderer.
- macOS builds produce a `HiveWE.app` bundle with `data` and `hivewe-build-provenance.json` in `Contents/Resources`, install rules, and a CPack DragNDrop DMG preset. The build-provenance JSON records the git source revision, dirty state, and renderer build flags so package and PR-evidence gates can reject stale bundles.
- The default macOS Metal build excludes the legacy OpenGL renderer modules, OpenGL preview widgets, QtImGui OpenGL bridge, glad, imgui, and Qt OpenGL link targets from the app target. macOS configures still pin transitive QtGui/SOIL2 OpenGL lookups to Apple's framework so a Metal build never selects XQuartz `libGL` from `/usr/X11R6`; install packaging also strips unused XQuartz `libGL`/X11 dylibs and fails if any remaining bundle binary links them.
- The main map widget wrapper exposes backend-neutral operations only; the `OpenGLMapCanvas` type is visible only inside the guarded OpenGL fallback declaration and implementation.
- The `macOS-Debug-OpenGL` preset builds the legacy OpenGL app target with `HIVEWE_ENABLE_METAL=OFF`; OpenGL fallback headers only use a temporary `QT_NO_OPENGL` include guard around `QOpenGLWidget` to avoid macOS glad/System OpenGL header collisions, then undefine it so the macro does not leak through the fallback build. At runtime, the macOS fallback now fails early with a clear OpenGL 4.5 requirement message before GLAD startup because Apple's OpenGL runtime exposes at most OpenGL 4.1.
- Generated JASS helper inputs/outputs are staged in a temporary directory instead of writing below packaged `data/tools`.

## Verification Commands

```shell
cmake --build --preset macOS-Debug-App
cmake --build --preset macOS-Debug-Verify
cmake --build --preset macOS-Debug-OpenGL
cmake --build --preset macOS-Release-App
./build/macOS-Debug-WithTests/HiveWE.app/Contents/MacOS/HiveWE --map "data/test map/" --smoke-exit-ms 3000
./build/macOS-Debug-WithTests/HiveWE.app/Contents/MacOS/HiveWE --map "data/test map/" --smoke-save-reload --smoke-exit-ms 3000
./build/macOS-Debug-WithTests/HiveWE.app/Contents/MacOS/HiveWE --map "data/test map/" --smoke-mutated-save-reload --smoke-exit-ms 3000
./build/macOS-Debug-WithTests/HiveWE.app/Contents/MacOS/HiveWE --map "data/test map/" --smoke-mpq-export-reload --smoke-exit-ms 3000
./build/macOS-Debug-WithTests/HiveWE.app/Contents/MacOS/HiveWE --map "data/test map/" --capture-frame /tmp/hivewe-capture.png --capture-delay-ms 1000 --smoke-exit-ms 10000
./build/macOS-Debug-WithTests/HiveWE.app/Contents/MacOS/HiveWE --map "data/test map/" --smoke-render-modes --capture-delay-ms 1000
./build/macOS-Debug-WithTests/HiveWE.app/Contents/MacOS/HiveWE --map "data/test map/" --smoke-save-reload --smoke-picking --capture-delay-ms 1000
./build/macOS-Debug-WithTests/HiveWE.app/Contents/MacOS/HiveWE --map "data/test map/" --smoke-model-previews --smoke-artifact-dir /tmp/hivewe-metal-smoke-artifacts --capture-delay-ms 1000
./build/macOS-Debug-WithTests/HiveWE.app/Contents/MacOS/HiveWE --smoke-metal-renderer-probes --smoke-artifact-dir /tmp/hivewe-metal-renderer-probes
./build/macOS-Debug-WithTests/HiveWE.app/Contents/MacOS/HiveWE --smoke-metal-preview-probes --smoke-artifact-dir /tmp/hivewe-metal-preview-probes
python3 tools/compare_smoke_artifacts.py --self-test
python3 tools/compare_smoke_artifacts.py --validate-manifest /tmp/hivewe-metal-renderer-probes --manifest-preset metal-renderer-probes
python3 tools/run_metal_smoke.py build/macOS-Debug-WithTests/HiveWE.app --warcraft-dir "/Applications/Warcraft III" --require-current-source --require-clean-source
cmake --install build/macOS-Debug-WithTests --prefix build/macOS-Debug-WithTests/install
python3 tools/run_metal_smoke.py build/macOS-Debug-WithTests/install/HiveWE.app --warcraft-dir "/Applications/Warcraft III" --capture-frame /tmp/hivewe-installed-metal-smoke.png --artifact-dir /tmp/hivewe-installed-metal-smoke-artifacts --require-current-source --require-clean-source
python3 tools/check_macos_package.py --build-dir build/macOS-Debug-WithTests --warcraft-dir "/Applications/Warcraft III"
python3 tools/check_macos_package.py --build-dir build/macOS-Debug-WithTests --skip-smoke --require-current-source --require-clean-source --require-build-type Debug --save-reload-smoke --mutated-save-reload-smoke --mpq-export-reload-smoke --renderer-probe-smoke --renderer-probe-artifact-dir /tmp/hivewe-installed-metal-renderer-probes --renderer-probe-parity-smoke --renderer-probe-parity-artifact-dir /tmp/hivewe-installed-metal-renderer-parity --map-canvas-smoke --map-canvas-artifact-dir /tmp/hivewe-installed-metal-map-canvas-smoke --map-render-parity-smoke --map-render-parity-artifact-dir /tmp/hivewe-installed-map-render-parity --mpq-map-render-parity-smoke --mpq-map-render-parity-artifact-dir /tmp/hivewe-installed-mpq-map-render-parity --preview-probe-smoke --preview-probe-artifact-dir /tmp/hivewe-installed-metal-preview-probes --preview-probe-parity-smoke --preview-probe-parity-artifact-dir /tmp/hivewe-installed-metal-preview-parity --shared-preview-probe-smoke --shared-preview-probe-artifact-dir /tmp/hivewe-installed-shared-preview-probes --shared-preview-parity-smoke --shared-preview-parity-artifact-dir /tmp/hivewe-installed-shared-preview-parity --run-cpack --package-report-json /tmp/hivewe-debug-package-report.json --package-report-markdown /tmp/hivewe-debug-package-report.md
python3 tools/check_macos_package.py --build-dir build/macOS-Release --skip-smoke --require-current-source --require-clean-source --require-build-type Release --save-reload-smoke --mutated-save-reload-smoke --mpq-export-reload-smoke --renderer-probe-smoke --renderer-probe-parity-smoke --map-canvas-smoke --map-render-parity-smoke --mpq-map-render-parity-smoke --preview-probe-smoke --preview-probe-parity-smoke --shared-preview-probe-smoke --shared-preview-parity-smoke --run-cpack --package-report-json /tmp/hivewe-release-package-report.json --package-report-markdown /tmp/hivewe-release-package-report.md

ctest --test-dir build/macOS-Debug-WithTests --output-on-failure
```

`macOS-Debug-App`, `macOS-Debug-Tests`, and `macOS-Debug-Verify` all build from one tests-enabled build tree. Use the app-only or test-binary-only presets for focused iteration, then run the full verify preset before smoke tests and commits that affect renderer behavior. `macOS-Release-App` builds only the shipping Release app target for package validation.

On the local macOS test host, a representative content edit to `src/main.cpp` rebuilt the app in 5.97s after the smoke split, down from the 15.78s baseline measured immediately before it. A representative content edit isolated to `src/smoke/smoke_render.cpp` rebuilt in 4.52s, which is the path used by most Metal smoke-validation changes.
The brush/selection overlay and terrain/cliff/pathing/material frame builders now live in `src/rendering/metal_map_frame_builders.h` instead of the broad renderer type header. A helper-header timestamp rebuild of the full `macOS-Debug-Verify` target rebuilt only the map-canvas translation unit, now `src/main_window/map_canvas_widget.cpp`, plus the overlay and terrain tests in 2.68s, compared with 135.16s for helper work and 296.52s for the one-time terrain-builder move while they still touched `src/rendering/metal_map_renderer.h`.
Metal widget headers no longer expose the full `MetalMapRenderer` type; the public model-preview smoke stats live in `src/rendering/metal_model_frame_stats.h`, while the map canvas holds an `IMapFrameRenderer` from `make_map_frame_renderer()` and preview widgets hold `IModelFrameRenderer` pointers from `make_model_frame_renderer()`. The concrete renderer is owned only by `src/rendering/metal_map_frame_renderer.cpp`, so future app-surface work can mock or replace the frame renderer without including the broad Metal renderer type. On the local macOS test host, a timestamp-only edit to `src/rendering/metal_map_renderer.h` rebuilt the focused `HiveWE_tests` target in 3.47s after the original header split, down from 233.36s immediately before it.
`IModelPreviewRenderer` now lives in a backend-neutral header without `glad` or
OpenGL resource imports. The legacy OpenGL implementation lives in
`src/rendering/opengl_model_preview_renderer.h` and is included only by the
OpenGL model editor/object-grid widgets, so Metal builds do not see the OpenGL
preview implementation while the fallback preview path remains available.
Avoid extending `src/rendering/metal_map_renderer.h` during normal renderer iteration when an implementation-local cache or helper is enough. A pathing metadata change in that shared renderer type header forced a 435.71s `macOS-Debug-Verify` rebuild on this host, and the intentionally batched terrain/brush/pathing source-key metadata change forced a 386.71s rebuild. The follow-up no-op verify build was 0.24s and the focused Metal terrain-frame test was 0.09s.
The macOS presets route compiler launches through `tools/ccache-cxx-modules.sh`, which keeps ccache enabled while hashing imported C++ module PCM files from CMake's `.modmap` response files. This prevents stale cached objects when a module interface changes. Reconfiguring to the wrapper caused a one-time 448.00s app-target rebuild on this host; afterward, a no-op app build was 0.19s, a cache-hit PE1 helper-header rebuild was 1.49s, and a forced cache-miss PE1 helper-header rebuild was 13.46s versus 18.67s for the old broad `metal_map_renderer.h` path. The macOS CI job now persists `CCACHE_DIR` with branch and OS restore keys, zeros stats before configure/build, and prints final ccache stats so repeated PR evidence runs can prove cache reuse instead of starting from a cold compiler cache.

The current macOS Metal CTest baseline includes the smoke artifact comparison self-test, Metal full-runtime smoke wrapper self-test, renderer parity report self-test, renderer capture package self-test, renderer PR evidence capture-helper self-test, renderer PR evidence preparation self-test, renderer PR evidence audit self-test, macOS package validator self-test covering required bundle resources, stale bundled data, forbidden OpenGL/XQuartz dylibs, installed-app backend identity, and installed parity command construction, a build-tree backend identity fingerprint that requires `--print-render-backend` to report `metal`, save/reload smoke, mutated save/reload smoke, MPQ export/reload smoke, a labeled aggregate Metal unit-frame gate that also covers object-grid replaceable-texture cache identifiers, artifact-directory cleanup, the Warcraft-free save/reload-, mutation-, and MPQ-backed Metal map canvas plus synthetic runtime preview smoke, renderer, shared preview, and Metal-specific preview probe smokes, their dependent PNG artifact-manifest validators, a strict same-app Metal renderer-probe parity gate, a strict same-app Metal preview-probe parity gate, strict same-app save/reload and MPQ export/reload map-render parity gates, and a strict same-app shared-preview parity gate that requires zero pixel differences across repeated captures from the current source revision and rejects mismatched source revisions between the paired captures. Those same-app gates use `--same-app-determinism`, so they are recorded as deterministic Metal smoke checks instead of OpenGL-vs-Metal evidence. Tests are labeled for focused maintainer runs (`unit`, `metal`, `smoke`, `gpu`, `artifact`, `map-canvas`, `preview`, `renderer`, `picking`, `runtime`, `warcraft`, `parity`, `mutation`, `mpq`, and `save-reload`), and `ctest -L metal` now includes the backend-independent Metal unit-frame aggregate plus Metal-scoped artifact/parity/package/audit wrapper self-tests instead of only app-level smokes/artifact validators. `metal_full_runtime_smoke` is always registered and runs when `HIVEWE_WARCRAFT_DIR` is set at test time; without that proprietary content it reports a CTest skip instead of disappearing from the inventory. When enabled, it runs the full Warcraft-backed Metal smoke helper and validates real map render modes, picking, Footman previews, synthetic model/material/particle readbacks, and the full artifact manifest.
The picking smoke includes a helper-only doodad probe with a fixed skeleton scale, so real Warcraft SLK `defScale` values cannot shrink the click-helper mesh out of the sampled pixels while proving helper-enabled doodad picking.
The `macos-metal` GitHub Actions job runs on the pinned `macos-15` hosted image, prints the macOS/Xcode/CMake/Ninja/Homebrew LLVM/ccache/vcpkg versions used for provenance, configures `macOS-Debug-WithTests`, builds `macOS-Debug-Verify`, runs the CTest suite including the build-tree Metal backend identity fingerprint, smoke artifact comparison self-test, save/reload smoke, mutated save/reload smoke, MPQ export/reload smoke, the Warcraft-free save/reload-, mutation-, and MPQ-backed Metal map canvas, renderer, shared preview, strict same-app Metal renderer-probe parity, strict same-app Metal preview-probe parity, strict same-app save/reload and MPQ export/reload map-render parity, strict same-app shared-preview parity, and Metal-specific preview probes, their PNG artifact validators, and the optional full Metal runtime smoke, uploads their PNG/parity artifacts, validates the installed macOS bundle with installed-app save/reload, mutated save/reload, and MPQ export/reload smoke plus the same Warcraft-free save/reload-, mutation-, and MPQ-backed Metal map canvas/renderer/renderer-probe parity/map-render parity/shared-preview/strict shared-preview parity/Metal-preview probes/strict Metal-preview parity captured from a clean source tree, builds and verifies a DragNDrop DMG, uploads those installed-app PNG/parity and DMG artifacts, optionally runs installed-app full Metal runtime smoke when `HIVEWE_WARCRAFT_DIR` is configured, uploads the installed full-smoke artifact set, captures and packages source-pinned Metal PR renderer evidence plus Metal MPQ export/reload map-render evidence and a local Metal-only `--allow-missing-opengl` audit report when Warcraft content is configured, configures Release, builds `macOS-Release-Verify`, runs `ctest --preset macOS-Release`, validates the Release install tree and DragNDrop DMG with the installed-app save/reload, mutated save/reload, and MPQ export/reload smoke plus the same Warcraft-free save/reload-, mutation-, and MPQ-backed Metal map canvas/renderer/renderer-probe parity/map-render parity/shared-preview/strict shared-preview parity/Metal-preview probes/strict Metal-preview parity captured from a clean source tree, uploads the Release probe/parity artifacts, then configures, builds, and fingerprint-checks the `macOS-Debug-OpenGL` fallback with `--print-render-backend`. The Windows job now resolves and fingerprints the built OpenGL executable after tests, and when `HIVEWE_WINDOWS_WARCRAFT_DIR` is configured or supplied through the manual `workflow_dispatch` input it uses `tools/capture_renderer_pr_evidence.py --backend opengl` to upload the default OpenGL PR bundle, MPQ export/reload map-render bundle, and source-pinned capture summary as `windows-opengl-pr-renderer-evidence`; this is intended for a self-hosted Windows/OpenGL 4.5 runner with Warcraft III content, while hosted Windows CI normally skips the evidence step. The CI audit artifact is a macOS Metal-side progress audit, not PR-complete evidence, until a Windows/Linux OpenGL 4.5 reference bundle and the final cross-backend parity reports are attached.
The `renderer-pr-evidence` GitHub Actions job waits for both platform jobs, downloads `windows-opengl-pr-renderer-evidence`, `macos-metal-pr-renderer-evidence-final`, and `macos-installed-metal-smoke`, uses `tools/prepare_renderer_pr_evidence.py` to verify the expected downloaded artifact layout, then runs the final default and MPQ `tools/run_renderer_parity.py --skip-capture --pr-gate` comparisons when every required archive/report is present. It writes a source-pinned manual-review template prefilled with the submitted capture paths, default parity metrics, MPQ parity metrics, and highest-diff artifact rows, then writes an automated audit report with `--allow-missing-manual-review`, so CI can prove the cross-backend bundles, package reports, installed full-smoke artifacts, Debug package executable fingerprint, and MPQ parity are internally consistent before the human visual-review note is added for the strict final audit. Pull requests that touch renderer, runtime, smoke, package, or evidence-sensitive paths now fail this final job when either side's proprietary/GPU-backed evidence is absent; unrelated PRs and pushes still upload a skip note instead of hiding the missing input, and manual dispatch can force the same required-evidence behavior with `renderer_pr_evidence_required`.
`tools/run_metal_smoke.py` fingerprints the target app with `--print-render-backend`, requires `metal`, runs the full Metal runtime smoke command, writes and validates `metal-smoke-provenance.json` with host, source revision, app executable SHA-256, capture-frame SHA-256, command, and per-PNG artifact fingerprints, and then verifies the artifact manifest: eight map-render frames that match the main capture size, two Warcraft-backed preview frames, ten synthetic preview-probe widget frames, eight deterministic 256x256 Metal renderer readbacks, and three deterministic 512x512 picking readbacks. The provenance validator also rejects missing or mismatched full-smoke command flags/values, so archived evidence must prove save/reload, mutated save/reload, MPQ export/reload, render-mode, picking, model-preview, map, Warcraft-content, capture-frame, artifact-dir, and capture-delay arguments matched the recorded run. The full runtime path invokes the same Metal renderer-probe producer used by the standalone Warcraft-free renderer gate so this manifest cannot drift from the full-smoke artifact set. It rejects extra, missing, dimension-mismatched, invalid, stale-provenance, dirty-source provenance when requested, and unexpected flat PNGs so a wrong-but-sized artifact cannot satisfy the gate; the filtered-empty object-grid artifact is the only intentionally flat preview artifact. Full runtime smoke now requires `--warcraft-dir <path>` or `HIVEWE_WARCRAFT_DIR`, runs folder save/reload, mutated save/reload, and MPQ export/unpack/reload before render capture, accepts `--require-current-source --require-clean-source` or `--require-source-head <sha> --require-clean-source` for maintainer-facing full-smoke evidence, and accepts `--skip-if-missing-warcraft-dir` for optional CI jobs that should report a CTest skip when proprietary content is unavailable.
`tools/check_macos_package.py` wraps the installed-bundle gate: it runs `cmake --install`, validates required bundle resources, verifies the bundled `Contents/Resources/hivewe-build-provenance.json` source revision with `--require-current-source` or `--require-source-head <sha>`, clean-source state with `--require-clean-source`, and CMake configuration with `--require-build-type Debug|Release|RelWithDebInfo`, verifies the bundled `Contents/Resources/data/test map/`, `Contents/Resources/data/warcraft/` smoke fixture, and `Contents/Resources/data/shaders/metal_map_renderer.metal` are byte-identical to the repository sources even when runtime smoke is skipped, checks bundled and linked dylibs for forbidden XQuartz/OpenGL dependencies, fingerprints the installed app with `--print-render-backend` and requires `metal`, can run installed-app save/reload smoke with `--save-reload-smoke`, can run installed-app mutated save/reload smoke with `--mutated-save-reload-smoke`, can run installed-app MPQ export/unpack/reload smoke with `--mpq-export-reload-smoke`, can run installed-app Warcraft-free renderer probes with `--renderer-probe-smoke` and validates all eleven required renderer-probe PNGs including the forced-scale alpha-picking, PE1 spawned-model picking, PE1 spawned-model visible render, meshless bounds picking, and model depth-isolation readbacks, can run strict same-installed-app renderer-probe parity with `--renderer-probe-parity-smoke`, can run installed-app Warcraft-free save/reload-, mutation-, MPQ-backed map canvas, and synthetic runtime-preview smoke with `--map-canvas-smoke` and validates the required map/probe/preview PNGs, can run strict same-installed-app save/reload map-render parity with `--map-render-parity-smoke`, can run strict same-installed-app MPQ export/reload map-render parity with `--mpq-map-render-parity-smoke`, can run installed-app Warcraft-free shared preview probes with `--shared-preview-probe-smoke` and validates the four shared preview PNGs, can run strict same-installed-app shared-preview parity with `--shared-preview-parity-smoke`, can run installed-app Metal-specific preview probes with `--preview-probe-smoke` and validates the ten required widget preview PNGs, can run strict same-installed-app Metal preview-probe parity with `--preview-probe-parity-smoke`, always requires paired installed-app parity captures to share the same source revision, can pass `--require-current-source --require-clean-source` or `--require-source-head <sha> --require-clean-source` through to full installed Metal smoke provenance so PR artifacts cannot come from stale or dirty source, runs the full installed-app Metal smoke helper against the bundled test map when `--warcraft-dir` or `HIVEWE_WARCRAFT_DIR` is available unless `--skip-smoke` is passed, skips that full installed smoke when Warcraft content is unavailable, can use `--require-full-smoke` to fail instead of skipping when full installed-smoke evidence is mandatory, can write installed full-smoke artifacts to explicit paths for CI upload, can write JSON/Markdown package validation reports for PR evidence audits, and builds/verifies the current CPack DragNDrop DMG with `--run-cpack`, then mounts the verified DMG read-only and applies the same app bundle, build-provenance, byte-identical test-map, byte-identical Warcraft-free fixture, byte-identical Metal shader, dylib-link, and Metal backend checks to the app inside it.

Package reports now include a required `codesign` check; the validator verifies both the installed app and the mounted DMG app with strict deep bundle signature verification before evidence can pass the final audit.
The final evidence-preparation step and Metal-side handoff packager also reject stale package-report schemas, dirty-source package reports, dirty or cross-revision installed full-smoke source provenance, stale installed full-smoke capture/artifact fingerprints, Metal captures or full-smoke runs from a different Debug package executable, missing required package checks, missing `built_dmg`/`verified_dmg` artifact metadata, mismatched Debug/Release build types, wrong-backend capture archives, wrong default-vs-MPQ capture modes, missing picking coverage, missing capture commands, and missing PNG fingerprints before producing final parity inputs or a handoff zip.

## Visual Parity Artifacts

Use `tools/run_renderer_parity.py` on a host where both the legacy OpenGL 4.5 renderer and Metal renderer can be run, or use `--capture-only opengl|metal` to collect one backend at a time and later compare the two artifact directories with `--skip-capture`. It captures `--smoke-render-modes`, `--smoke-picking`, and `--smoke-model-previews` for both backends, validates the required map-render and shared-preview artifacts for missing, wrongly-sized, invalid, or flat PNG output, writes amplified diff PNGs, and emits a PR-friendly Markdown/JSON report plus `renderer-parity-provenance.json`. `--pr-gate` reports include a maintainer review checklist for backend identity, clean matching source revisions, save/reload-backed picking captures, terrain/cliff/water overlays, SD/HD materials, particles including PE1 model spawns, and model-editor/object-grid previews.

Captured artifact directories also receive `renderer-parity-capture.json` with the capture commands, host metadata, reported renderer backend from `--print-render-backend`, source Git revision/dirty state when available, executable size/SHA-256, PNG dimensions, PNG SHA-256 fingerprints, whether `--smoke-picking` ran for map captures, and whether map captures were taken after folder save/reload or MPQ export/unpack/reload; later comparisons reject changed captured PNGs when that fingerprint data is present. `--capture-only` validates the single backend directory and its manifest before exiting. `--skip-capture` comparisons also reject present provenance manifests when their preview-only, map-render-only, metal-preview-probes-only, renderer-probes-only, save/reload, or MPQ export/reload mode does not match the requested comparison, and can consume zipped capture bundles directly with `--opengl-artifact-archive <zip>` or `--metal-artifact-archive <zip>`. Default parity capture runs model-preview smoke and therefore requires Warcraft III content via `--warcraft-dir` or `HIVEWE_WARCRAFT_DIR`; use `--map-render-only`, `--preview-probes-only`, `--metal-renderer-probes-only`, or `--metal-preview-probes-only` for Warcraft-free focused comparisons. The capture packager self-test now validates default map/preview, shared preview-probe, Metal renderer-probe, and Metal preview-probe bundles so cross-host artifact transfer covers both full and focused capture modes; every zip also includes `renderer-capture-summary.md` with the expected artifact table, captured dimensions/SHA-256 prefixes, host/app binary provenance, a mode-specific `--skip-capture --pr-gate` command, and PR-gate review reminders. Packaging, evidence preparation, handoff assembly, and final PR-gate validation require schema-8 capture manifests with PNG fingerprints for every required artifact in the capture mode plus command provenance proving the requested render, preview, probe, save/reload, MPQ, and picking smoke flags; the preparation and handoff tools also verify the packaged PNG payload bytes against those fingerprints, so stale or hand-edited bundles cannot satisfy PR evidence. Default zip prefixes include the capture mode so map, renderer-probe, and preview-probe bundles from the same commit do not collide when unpacked or attached for review. Use `--pr-gate` for maintainer-facing PR evidence; it requires capture manifests, current clean source provenance, matching OpenGL/Metal source revisions when comparing, OpenGL-reference/Metal-candidate backend identities, post-save/reload map captures for default map parity modes unless `--map-after-mpq-export-reload` is explicitly requested, recorded picking smoke coverage for every map-mode capture, command provenance, and required PNG fingerprints, and it refuses `--keep-existing-artifacts`. Fresh comparisons remove prior parity JSON/Markdown/provenance files and the previous `diffs/` directory before writing new report evidence while preserving extracted capture archives. Use `--same-app-determinism` only for intentional same-backend smoke gates. Use `--require-source-head <sha> --require-clean-source` when validating artifacts for a specific reviewed commit, or `--require-matching-source` directly to fail whenever OpenGL and Metal captures were produced from different commit SHAs.

`tools/audit_renderer_pr_evidence.py` audits the PR evidence set before handoff: it validates a Metal candidate capture directory or zip, an OpenGL reference capture directory or zip, optional Metal/OpenGL MPQ export/reload map-render capture directories or zips, the final `renderer-parity` report directory, matching clean source revisions, required saved/reloaded map + preview capture mode, recorded picking coverage, backend identity, parity-report provenance that embeds the submitted capture manifests, passing parity result rows with finite numeric metrics and diff PNG dimensions tied to the embedded capture fingerprints, and recomputed parity metrics plus diff PNG bytes from the submitted PNGs, installed full-Metal smoke provenance/artifacts, installed-app executable identity and build-source provenance from either the local installed app or the Debug package report executable fingerprint, Debug and Release package validation reports from `tools/check_macos_package.py --package-report-json`, and a manual visual-review note for the final parity diffs. When MPQ capture bundles are supplied, the auditor requires map-render-only MPQ export/reload mode with picking coverage and checks that the MPQ parity report embeds the submitted MPQ capture manifests. The default and MPQ Metal capture manifests must match the Debug package report's installed app path, executable path, executable size, executable SHA-256, and source head, so final PR evidence should be captured from `build/macOS-Debug-WithTests/install/HiveWE.app` after package validation. The manual note must reference the reviewed source revision, final parity report, OpenGL and Metal capture evidence, and at least one numeric RMS, mean-abs, or changed-percent diff metric while covering the required visual topics. `--write-manual-review-template` now accepts the same parity-report and capture arguments as the final audit so the generated note can prefill source, evidence paths, worst RMS/mean/changed-percent metrics, and the highest-diff artifact rows while still retaining TODO review prompts. Add `--require-mpq-parity-report` to the final audit command when MPQ export/reload map-render parity is required PR evidence; with that flag the MPQ parity report and both submitted MPQ capture bundles are required, while without it missing MPQ evidence stays a warning for local progress checks. Use `--allow-missing-opengl` only for local macOS progress checks; the final audit fails until the OpenGL reference capture, final parity report, installed full-smoke app identity, Debug/Release package reports, and manual review note are present.

For cross-host PR evidence collection, prefer the capture helper when a host owns only one backend. It runs the default PR capture, packages that archive, runs the MPQ export/reload map-render capture, packages that archive, then writes `renderer-pr-capture-summary.md` with source-pinned opposite-backend capture, comparison, and final audit commands:

```shell
python3 tools/capture_renderer_pr_evidence.py \
  --backend opengl \
  --app ./build/<opengl-build>/HiveWE \
  --map "data/test map/" \
  --warcraft-dir "<Warcraft III>" \
  --output-dir /tmp/hivewe-opengl-pr-evidence

python3 tools/capture_renderer_pr_evidence.py \
  --backend metal \
  --app ./build/macOS-Debug-WithTests/install/HiveWE.app \
  --map "data/test map/" \
  --warcraft-dir "<Warcraft III>" \
  --output-dir /tmp/hivewe-metal-pr-evidence \
  --require-source-head <OpenGL capture source sha>
```

The generated opposite-backend command defaults to the repository map path `data/test map/` and a relative `hivewe-<backend>-pr-evidence` directory so a summary produced from a macOS installed app bundle does not tell the OpenGL host to use a macOS `.app/Contents/Resources/...` path or POSIX-only temp directory. Use `--counterpart-map <path>` or `--counterpart-output-dir <path>` when the other host needs a different map checkout path or evidence directory. Use `--summary-only` to rewrite `renderer-pr-capture-summary.md` from existing capture manifests after adjusting handoff paths, without rerunning the expensive capture steps.

Pass `--map-after-save-reload`, or `--pr-gate`, when capturing default PR review artifacts to save, reload, and then capture map-render parity frames from the post-reload map state. For a Warcraft-free map-canvas-only sanity check, pass `--map-render-only --map-after-save-reload`; it skips model preview captures and compares only the map-render artifacts. Pass `--map-render-only --map-after-mpq-export-reload` when the artifact pair needs to prove render parity after MPQ export/unpack/reload; `--pr-gate` preserves that explicit MPQ mode instead of adding a separate folder save/reload mode. For a Warcraft-free Metal renderer-probe sanity check, pass `--metal-renderer-probes-only`; it runs `--smoke-metal-renderer-probes` on both apps and compares only the Metal terrain/material/pathing/picking/model/selection/particle probe artifacts. For a Warcraft-free Metal preview-probe sanity check, pass `--metal-preview-probes-only`; it runs `--smoke-metal-preview-probes` on both apps and compares the Metal model-editor/object-grid material, multicell replaceable-cache, scrolled-grid, particle, and nested PE1 model-spawn probe artifacts. For a Warcraft-free preview-only sanity check, pass `--preview-probes-only`; it runs `--smoke-shared-preview-probes` on both apps, compares only the synthetic shared preview artifacts, and still writes the same Markdown/JSON/diff report. On this macOS host, the OpenGL fallback is only a compile gate, so capture the OpenGL reference artifacts from a Windows/Linux OpenGL runtime or another host where the legacy OpenGL path starts reliably.

```shell
python3 tools/run_renderer_parity.py \
  --opengl-app ./build/<opengl-build>/HiveWE \
  --metal-app ./build/macOS-Debug-WithTests/install/HiveWE.app \
  --map "data/test map/" \
  --warcraft-dir "<Warcraft III>" \
  --pr-gate \
  --report-dir /tmp/hivewe-renderer-parity

# Or capture artifacts on different hosts and compare them afterward.
python3 tools/run_renderer_parity.py \
  --capture-only opengl \
  --opengl-app ./build/<opengl-build>/HiveWE \
  --opengl-artifact-dir /tmp/hivewe-opengl-artifacts \
  --map "data/test map/" \
  --warcraft-dir "<Warcraft III>" \
  --pr-gate

python3 tools/run_renderer_parity.py \
  --capture-only metal \
  --metal-app ./build/macOS-Debug-WithTests/install/HiveWE.app \
  --metal-artifact-dir /tmp/hivewe-metal-artifacts \
  --map "data/test map/" \
  --warcraft-dir "<Warcraft III>" \
  --pr-gate

python3 tools/package_renderer_capture.py \
  --artifact-dir /tmp/hivewe-opengl-artifacts \
  --expected-backend opengl \
  --require-current-source \
  --require-clean-source \
  --output /tmp/hivewe-opengl-artifacts.zip

python3 tools/package_renderer_capture.py \
  --artifact-dir /tmp/hivewe-metal-artifacts \
  --expected-backend metal \
  --require-current-source \
  --require-clean-source \
  --output /tmp/hivewe-metal-artifacts.zip

# MPQ export/reload map-render bundles for the final PR audit.
python3 tools/run_renderer_parity.py \
  --capture-only opengl \
  --opengl-app ./build/<opengl-build>/HiveWE \
  --opengl-artifact-dir /tmp/hivewe-opengl-mpq-map-artifacts \
  --map-render-only \
  --map-after-mpq-export-reload \
  --map "data/test map/" \
  --warcraft-dir "<Warcraft III>" \
  --pr-gate

python3 tools/run_renderer_parity.py \
  --capture-only metal \
  --metal-app ./build/macOS-Debug-WithTests/install/HiveWE.app \
  --metal-artifact-dir /tmp/hivewe-metal-mpq-map-artifacts \
  --map-render-only \
  --map-after-mpq-export-reload \
  --map "data/test map/" \
  --warcraft-dir "<Warcraft III>" \
  --pr-gate

python3 tools/package_renderer_capture.py \
  --artifact-dir /tmp/hivewe-opengl-mpq-map-artifacts \
  --expected-backend opengl \
  --require-current-source \
  --require-clean-source \
  --output /tmp/hivewe-opengl-mpq-map-artifacts.zip

python3 tools/package_renderer_capture.py \
  --artifact-dir /tmp/hivewe-metal-mpq-map-artifacts \
  --expected-backend metal \
  --require-current-source \
  --require-clean-source \
  --output /tmp/hivewe-metal-mpq-map-artifacts.zip

python3 tools/run_renderer_parity.py \
  --skip-capture \
  --pr-gate \
  --map-render-only \
  --map-after-mpq-export-reload \
  --opengl-artifact-archive /tmp/hivewe-opengl-mpq-map-artifacts.zip \
  --metal-artifact-archive /tmp/hivewe-metal-mpq-map-artifacts.zip \
  --report-dir /tmp/hivewe-renderer-mpq-parity

python3 tools/run_renderer_parity.py \
  --skip-capture \
  --pr-gate \
  --opengl-artifact-archive /tmp/hivewe-opengl-artifacts.zip \
  --metal-artifact-archive /tmp/hivewe-metal-artifacts.zip \
  --report-dir /tmp/hivewe-renderer-parity

python3 tools/audit_renderer_pr_evidence.py \
  --write-manual-review-template /tmp/hivewe-renderer-manual-review.md \
  --opengl-artifact-archive /tmp/hivewe-opengl-artifacts.zip \
  --metal-artifact-archive /tmp/hivewe-metal-artifacts.zip \
  --opengl-mpq-artifact-archive /tmp/hivewe-opengl-mpq-map-artifacts.zip \
  --metal-mpq-artifact-archive /tmp/hivewe-metal-mpq-map-artifacts.zip \
  --parity-report-dir /tmp/hivewe-renderer-parity \
  --mpq-parity-report-dir /tmp/hivewe-renderer-mpq-parity \
  --require-current-source

python3 tools/audit_renderer_pr_evidence.py \
  --opengl-artifact-archive /tmp/hivewe-opengl-artifacts.zip \
  --metal-artifact-archive /tmp/hivewe-metal-artifacts.zip \
  --opengl-mpq-artifact-archive /tmp/hivewe-opengl-mpq-map-artifacts.zip \
  --metal-mpq-artifact-archive /tmp/hivewe-metal-mpq-map-artifacts.zip \
  --parity-report-dir /tmp/hivewe-renderer-parity \
  --mpq-parity-report-dir /tmp/hivewe-renderer-mpq-parity \
  --installed-smoke-artifact-dir /tmp/hivewe-installed-full-metal-smoke-artifacts \
  --installed-smoke-capture-frame /tmp/hivewe-installed-full-metal-smoke.png \
  --installed-smoke-app build/macOS-Debug-WithTests/install/HiveWE.app \
  --debug-package-report-json /tmp/hivewe-debug-package-report.json \
  --release-package-report-json /tmp/hivewe-release-package-report.json \
  --manual-review-note /tmp/hivewe-renderer-manual-review.md \
  --require-current-source \
  --require-clean-source \
  --require-mpq-parity-report \
  --markdown-output /tmp/hivewe-renderer-pr-evidence-audit.md

# Warcraft-free shared preview probes only.
python3 tools/run_renderer_parity.py \
  --preview-probes-only \
  --opengl-app ./build/<opengl-build>/HiveWE \
  --metal-app ./build/macOS-Debug-WithTests/HiveWE.app \
  --report-dir /tmp/hivewe-preview-parity

# Warcraft-free map-render parity only.
python3 tools/run_renderer_parity.py \
  --map-render-only \
  --map-after-save-reload \
  --same-app-determinism \
  --opengl-app ./build/macOS-Debug-WithTests/HiveWE.app \
  --metal-app ./build/macOS-Debug-WithTests/HiveWE.app \
  --report-dir /tmp/hivewe-map-render-parity

# Warcraft-free Metal renderer probe parity only.
python3 tools/run_renderer_parity.py \
  --metal-renderer-probes-only \
  --same-app-determinism \
  --opengl-app ./build/macOS-Debug-WithTests/HiveWE.app \
  --metal-app ./build/macOS-Debug-WithTests/HiveWE.app \
  --report-dir /tmp/hivewe-metal-renderer-parity

# Warcraft-free Metal preview probe parity only.
python3 tools/run_renderer_parity.py \
  --metal-preview-probes-only \
  --same-app-determinism \
  --opengl-app ./build/macOS-Debug-WithTests/HiveWE.app \
  --metal-app ./build/macOS-Debug-WithTests/HiveWE.app \
  --report-dir /tmp/hivewe-metal-preview-parity
```

The parity runner compares only artifacts shared by both backends in its default mode: map render modes, real model editor/object-grid preview frames, synthetic material-heavy model editor/object-grid preview frames, and synthetic PE2 particle preview frames. Metal additionally writes multicell replaceable-cache and scrolled object-grid widget captures plus synthetic material, selection, and particle readbacks; the Metal-only renderer probes are covered by `--metal-renderer-probes-only` and their own smoke gates, and the Metal-specific widget previews are covered by `--metal-preview-probes-only` and their own smoke gates. Optional `--max-rms`, `--max-mean-abs`, and `--max-changed-percent` thresholds can turn the parity report into a stricter gate once acceptable backend variance is known. `python3 tools/compare_smoke_artifacts.py --self-test` exercises the PNG codec, diff writing, missing-artifact reporting, dimension mismatch reporting, threshold-failure paths, and shared manifest validation without requiring a renderer.

## Latest Local Smoke Evidence

On 2026-06-01 (validated `2026-05-31T23:30:16.308350+00:00`), the local macOS Debug Metal installed app for implementation revision `a57b2a8d2365a9c3d3d84c380bab2fcdbfe95a33` completed full package validation, including the Warcraft-backed installed-app smoke against `/Applications/Warcraft III`:

```shell
python3 tools/check_macos_package.py \
  --build-dir build/macOS-Debug-WithTests \
  --require-current-source \
  --require-clean-source \
  --require-build-type Debug \
  --require-full-smoke \
  --warcraft-dir "/Applications/Warcraft III" \
  --installed-smoke-capture-frame /tmp/hivewe-debug-a57b2a8-full-metal-smoke.png \
  --installed-smoke-artifact-dir /tmp/hivewe-debug-a57b2a8-full-metal-smoke-artifacts \
  --save-reload-smoke \
  --mutated-save-reload-smoke \
  --mpq-export-reload-smoke \
  --renderer-probe-smoke \
  --renderer-probe-parity-smoke \
  --map-canvas-smoke \
  --map-render-parity-smoke \
  --mpq-map-render-parity-smoke \
  --preview-probe-smoke \
  --preview-probe-parity-smoke \
  --shared-preview-probe-smoke \
  --shared-preview-parity-smoke \
  --run-cpack \
  --package-report-json /tmp/hivewe-debug-a57b2a8-package-report.json \
  --package-report-markdown /tmp/hivewe-debug-a57b2a8-package-report.md
```

That run passed all 24 recorded package checks: build provenance, bundled resources and Metal shader, forbidden-dylib rejection, codesign, installed backend identity, renderer probes, preview probes, map-canvas smoke, save/reload, mutated save/reload, MPQ export/reload, same-installed-app determinism gates, CPack, DMG checksum verification, and mounted-DMG app validation. The installed full-smoke run captured `2026-05-31T23:29:52.696083+00:00`, reported backend `metal`, validated all 30 required PNG artifacts in `/tmp/hivewe-debug-a57b2a8-full-metal-smoke-artifacts`, and wrote `/tmp/hivewe-debug-a57b2a8-full-metal-smoke.png` at 1920x897 with 1,720,498 varied pixels and SHA-256 `6dae3d209e9b9851b4f1fa3a8cc0c6c543686e669023255e1ca2b86faff4c0ac`. The installed executable was `build/macOS-Debug-WithTests/install/HiveWE.app/Contents/MacOS/HiveWE` with SHA-256 `0d68a626fe0edee33698adbfbc105b5cc0e800f240448a48b6aeb7f343976991` and size 78,887,648 bytes. Detached validation should use `--require-source-head a57b2a8d2365a9c3d3d84c380bab2fcdbfe95a33 --require-clean-source`.

On 2026-06-01 (captured `2026-05-31T23:19:38.719791+00:00`), the local macOS Debug Metal build-tree app for implementation revision `dc592d8156a85448bc200591d81bd85ab1cef6d3` completed the Warcraft-backed full runtime smoke against `/Applications/Warcraft III`:

```shell
python3 tools/run_metal_smoke.py \
  build/macOS-Debug-WithTests/HiveWE.app \
  --warcraft-dir "/Applications/Warcraft III" \
  --artifact-dir /tmp/hivewe-current-metal-smoke-artifacts \
  --capture-frame /tmp/hivewe-current-metal-smoke-frame.png \
  --timeout 180 \
  --require-current-source \
  --require-clean-source
```

That run reported backend `metal`, validated save/reload, mutated save/reload, MPQ export/reload, render-mode toggles, picking, real Footman model-editor/object-grid previews, synthetic SD/HD material previews, particles, ribbons, selection overlays, and renderer probes, then wrote and validated `metal-smoke-provenance.json` plus all 30 required PNG artifacts in `/tmp/hivewe-current-metal-smoke-artifacts`. The capture frame was 1920x897 with 1,720,498 varied pixels and SHA-256 `d5d8b99ac884ada65aa546dcc7e22c0a0a810653ebcf77fc27f4c61985f2cf61`. The app executable was `build/macOS-Debug-WithTests/HiveWE.app/Contents/MacOS/HiveWE` with SHA-256 `f2217a48a778c36d2113d28a8b46e5a58b0563fc541fafc88024590404a8cd40` and size 79,330,712 bytes. Detached validation of those artifacts should require `--require-source-head dc592d8156a85448bc200591d81bd85ab1cef6d3 --require-clean-source`. The same run with the wrapper's default 60-second timeout reached the model-depth-isolation artifact and then timed out, so this host should use an explicit longer `--timeout` for full Warcraft-backed smoke runs.

On 2026-05-31, the installed local macOS Debug Metal app for implementation revision `837c9d6afa8e260b2f5a05906a92b199b602a36d` completed the Warcraft-backed full runtime smoke against `/Applications/Warcraft III` as part of Debug package validation:

```shell
python3 tools/check_macos_package.py \
  --build-dir build/macOS-Debug-WithTests \
  --require-current-source \
  --require-clean-source \
  --require-build-type Debug \
  --require-full-smoke \
  --warcraft-dir "/Applications/Warcraft III" \
  --installed-smoke-capture-frame /tmp/hivewe-debug-837c9d6-full-metal-smoke.png \
  --installed-smoke-artifact-dir /tmp/hivewe-debug-837c9d6-full-metal-smoke-artifacts \
  --save-reload-smoke \
  --mutated-save-reload-smoke \
  --mpq-export-reload-smoke \
  --renderer-probe-smoke \
  --renderer-probe-parity-smoke \
  --map-canvas-smoke \
  --map-render-parity-smoke \
  --mpq-map-render-parity-smoke \
  --preview-probe-smoke \
  --preview-probe-parity-smoke \
  --shared-preview-probe-smoke \
  --shared-preview-parity-smoke \
  --run-cpack \
  --package-report-json /tmp/hivewe-debug-837c9d6-package-report.json \
  --package-report-markdown /tmp/hivewe-debug-837c9d6-package-report.md
```

That run reported backend `metal`, validated save/reload, mutated save/reload, MPQ export/reload, render-mode toggles, picking, real Footman model-editor/object-grid previews, synthetic SD/HD material previews, particles, ribbons, selection overlays, renderer probes, same-installed-app parity gates, and DMG packaging, then validated all 30 required full-smoke PNG artifacts plus `metal-smoke-provenance.json`. The capture frame was 1920x897 with 1,720,499 varied pixels, and the artifact manifest validator accepted the 30 full-smoke PNGs in `/tmp/hivewe-debug-837c9d6-full-metal-smoke-artifacts`. The full Debug package/full-smoke run completed in 330.07 seconds. The generated Debug executable was `build/macOS-Debug-WithTests/install/HiveWE.app/Contents/MacOS/HiveWE` with SHA-256 `b5087102d92e4d773ab408ceadd8e8c821f15becf71d93f32eaa0e179e36c727` and size 78,650,960 bytes. Detached validation of these artifacts should use `--require-source-head 837c9d6afa8e260b2f5a05906a92b199b602a36d --require-clean-source`.

## Current HEAD Focused Validation

On 2026-06-01, focused local validation completed for source revision
`f5b69970622d8457c7a49ea74124cd932807b00f` after adding OpenGL
starting-location render/picking parity with the Metal marker path:

```shell
cmake --build --preset macOS-Debug-WithTests --target HiveWE
cmake --build --preset macOS-Debug-OpenGL
build/macOS-Debug-WithTests/HiveWE.app/Contents/MacOS/HiveWE \
  --map "data/test map/" \
  --smoke-picking \
  --capture-delay-ms 1000
build/macOS-Debug-OpenGL/HiveWE.app/Contents/MacOS/HiveWE \
  --print-render-backend
ctest --preset macOS-Debug-WithTests \
  -R 'metal_map_canvas_smoke|metal_map_canvas_artifacts' \
  --output-on-failure
git diff --check
```

The Metal app target and the macOS OpenGL fallback target both built
successfully. The direct Metal smoke-picking run selected unit `0 (sloc)` from
the bundled test map, then proved doodad and helper-only doodad picking. The
OpenGL fallback app reported backend `opengl`, and `git diff --check` reported
no whitespace errors. The selected CTest map-canvas smoke and artifact
validators passed all 3 tests in 17.61 seconds, covering save/reload, mutation
save/reload, MPQ export/reload, render modes, picking, synthetic previews, and
map-canvas artifact validation through the Metal startup path. The rebuilt Metal
executable was `build/macOS-Debug-WithTests/HiveWE.app/Contents/MacOS/HiveWE`
with SHA-256
`f883a7b1e2f4072c8586579ac0a0ae5a50f64609f311e6708f3ef42e3a59666c` and size
79,351,064 bytes. The rebuilt OpenGL fallback executable was
`build/macOS-Debug-OpenGL/HiveWE.app/Contents/MacOS/HiveWE` with SHA-256
`253b055e66aa8f1d6a11e919cd7d67e624fdb78e00c9746cdb4a59bd1ec3e2cd` and size
80,504,152 bytes. This remains local compile/backend-identity evidence for
OpenGL; OpenGL visual reference capture still needs a Windows/Linux OpenGL 4.5
runtime host.

On 2026-06-01, build-tree Metal-side PR evidence was refreshed for reviewed
source revision `b94a3a5d29031a69f9e346069c745e656ea119bb`:

- Default Metal PR bundle: `/tmp/hivewe-metal-pr-evidence-b94a3a5/hivewe-metal-artifacts.zip`
- MPQ export/reload Metal map-render bundle: `/tmp/hivewe-metal-pr-evidence-b94a3a5/hivewe-metal-mpq-map-artifacts.zip`
- Capture summary: `/tmp/hivewe-metal-pr-evidence-b94a3a5/renderer-pr-capture-summary.md`
- Full Metal smoke artifacts: `/tmp/hivewe-full-metal-smoke-b94a3a5-artifacts`
- Full Metal smoke frame: `/tmp/hivewe-full-metal-smoke-b94a3a5.png`
- Local Metal-side audit: `/tmp/hivewe-b94a3a5-local-metal-audit.md`

The default capture passed `--pr-gate`, packaged 24 PNG artifacts, and covered
saved/reloaded map render modes, picking, real model-editor/object-grid
previews, synthetic SD/HD material previews, particles, ribbons, selection
overlays, and Metal renderer probes. The MPQ capture passed
`--pr-gate --map-render-only --map-after-mpq-export-reload`, packaged 9 PNG
map-render/picking artifacts, and recorded source `b94a3a5d2903`, backend
`metal`, and app `build/macOS-Debug-WithTests/HiveWE.app`. The full Metal smoke
validated save/reload, mutated save/reload, MPQ export/reload, render modes,
picking, model previews, renderer probes, `metal-smoke-provenance.json`, and all
30 required PNG artifacts; its frame was 1920x897 with 1,720,521 varied pixels.

The local Metal-side audit was:

```shell
python3 tools/audit_renderer_pr_evidence.py \
  --metal-artifact-archive /tmp/hivewe-metal-pr-evidence-b94a3a5/hivewe-metal-artifacts.zip \
  --metal-mpq-artifact-archive /tmp/hivewe-metal-pr-evidence-b94a3a5/hivewe-metal-mpq-map-artifacts.zip \
  --installed-smoke-artifact-dir /tmp/hivewe-full-metal-smoke-b94a3a5-artifacts \
  --installed-smoke-capture-frame /tmp/hivewe-full-metal-smoke-b94a3a5.png \
  --installed-smoke-app build/macOS-Debug-WithTests/HiveWE.app \
  --allow-missing-opengl \
  --allow-missing-manual-review \
  --allow-missing-installed-app-identity \
  --require-source-head b94a3a5d29031a69f9e346069c745e656ea119bb \
  --require-clean-source \
  --markdown-output /tmp/hivewe-b94a3a5-local-metal-audit.md
```

It reported 5 passing checks, 8 warnings, and 0 failures. The passing checks
prove the current build-tree Metal default capture, Metal MPQ capture, default
and MPQ executable identity, full Metal smoke provenance/artifacts, and full
smoke app identity. The warnings are the remaining final-evidence boundary for
this build-tree audit: capture OpenGL reference bundles on a Windows/Linux
OpenGL 4.5 runtime, generate default and MPQ OpenGL-vs-Metal parity reports,
run current Debug/Release package validation if packaged-app identity evidence
is required for this revision, and complete the manual visual-review note over
the final diffs.

On 2026-06-01, current installed-app Metal-side PR evidence was refreshed for
reviewed source revision `a03d45d23fe5d6b5bfd34adb453e4ac7ff062b6f` after the
OpenGL starting-location parity and evidence documentation commits:

- Installed Debug default Metal PR bundle: `/tmp/hivewe-installed-metal-pr-evidence-a03d45d/hivewe-metal-artifacts.zip`
- Installed Debug MPQ export/reload Metal map-render bundle: `/tmp/hivewe-installed-metal-pr-evidence-a03d45d/hivewe-metal-mpq-map-artifacts.zip`
- Installed Debug capture summary: `/tmp/hivewe-installed-metal-pr-evidence-a03d45d/renderer-pr-capture-summary.md`
- Installed Debug full Metal smoke artifacts: `/tmp/hivewe-debug-a03d45d-full-metal-smoke-artifacts`
- Installed Debug full Metal smoke frame: `/tmp/hivewe-debug-a03d45d-full-metal-smoke.png`
- Debug package report: `/tmp/hivewe-debug-a03d45d-package-report.json`
- Release package report: `/tmp/hivewe-release-a03d45d-package-report.json`
- Local installed-app audit: `/tmp/hivewe-a03d45d-local-installed-metal-audit.md`
- Metal-side handoff bundle: `/tmp/hivewe-renderer-pr-handoff-a03d45d.zip`

The Debug package validator installed and signed
`build/macOS-Debug-WithTests/install/HiveWE.app`, verified clean source
`a03d45d23fe5`, required backend `metal`, byte-identical bundled test-map and
Warcraft-free fixture resources, the bundled Metal shader, no forbidden
XQuartz/OpenGL dylib links, codesign, installed-app save/reload, mutated
save/reload, MPQ export/reload, renderer probes, renderer-probe parity,
map-canvas smoke, save/reload map-render parity, MPQ map-render parity, preview
probes, preview-probe parity, shared-preview probes, shared-preview parity, the
full Warcraft-backed installed Metal smoke with all 30 required PNG artifacts,
and the built DragNDrop DMG plus the app mounted from that DMG. The Debug
installed executable was
`build/macOS-Debug-WithTests/install/HiveWE.app/Contents/MacOS/HiveWE` with
SHA-256 `a435a74b617c4f9f9695a20bc8b6e5a8a90aad34e1bebcc5613a9600b175edb2`
and size 78,907,872 bytes.

The Release package validator installed and signed
`build/macOS-Release/install/HiveWE.app`, verified clean source `a03d45d23fe5`,
required backend `metal`, byte-identical bundled resources, the bundled Metal
shader, no forbidden XQuartz/OpenGL dylib links, codesign, installed-app
save/reload, mutated save/reload, MPQ export/reload, renderer probes,
renderer-probe parity, map-canvas smoke, save/reload map-render parity, MPQ
map-render parity, preview probes, preview-probe parity, shared-preview probes,
shared-preview parity, and the built DragNDrop DMG plus the app mounted from
that DMG. The Release installed executable was
`build/macOS-Release/install/HiveWE.app/Contents/MacOS/HiveWE` with SHA-256
`3a9b349b87d993fefca6dafc5032cf67cba4e9581df2322a32acfa3513d92cc6` and size
26,829,424 bytes. The full Warcraft-backed installed smoke was covered on the
Debug package for this revision.

The installed Debug default capture passed `--pr-gate`, packaged 24 PNG
artifacts, and recorded source `a03d45d23fe5`, backend `metal`, and app
`build/macOS-Debug-WithTests/install/HiveWE.app`. The installed Debug MPQ
capture passed `--pr-gate --map-render-only --map-after-mpq-export-reload`,
packaged 9 PNG map-render/picking artifacts, and recorded the same source,
backend, and app. The full installed Metal smoke frame was 1920x897 with
1,720,521 varied pixels. The handoff zip is 78 MB with SHA-256
`79a03769a876adc94b00d29f118fc7aa22142648bc32d31a43f897c3f590f781`.

The local installed-app audit was:

```shell
python3 tools/audit_renderer_pr_evidence.py \
  --metal-artifact-archive /tmp/hivewe-installed-metal-pr-evidence-a03d45d/hivewe-metal-artifacts.zip \
  --metal-mpq-artifact-archive /tmp/hivewe-installed-metal-pr-evidence-a03d45d/hivewe-metal-mpq-map-artifacts.zip \
  --installed-smoke-artifact-dir /tmp/hivewe-debug-a03d45d-full-metal-smoke-artifacts \
  --installed-smoke-capture-frame /tmp/hivewe-debug-a03d45d-full-metal-smoke.png \
  --installed-smoke-app build/macOS-Debug-WithTests/install/HiveWE.app \
  --debug-package-report-json /tmp/hivewe-debug-a03d45d-package-report.json \
  --release-package-report-json /tmp/hivewe-release-a03d45d-package-report.json \
  --allow-missing-opengl \
  --allow-missing-manual-review \
  --require-source-head a03d45d23fe5d6b5bfd34adb453e4ac7ff062b6f \
  --require-clean-source \
  --markdown-output /tmp/hivewe-a03d45d-local-installed-metal-audit.md
```

It reported 9 passing checks, 4 warnings, and 0 failures. The passing checks
prove the installed-app Metal default capture, installed-app Metal MPQ capture,
default/MPQ executable identity, Debug package identity, Release package
validation, full installed Metal smoke provenance/artifacts, and full-smoke app
identity for source `a03d45d23fe5`. The remaining warnings are the external
final-evidence gates: capture OpenGL reference bundles on a Windows/Linux
OpenGL 4.5 runtime, generate default and MPQ OpenGL-vs-Metal parity reports from
those bundles, and complete the manual visual-review note over the final diffs.

On 2026-06-01, focused local validation completed for implementation revision
`bf2edf193f6a91da80ac17d593495d73a5be88a1` after adding Metal
starting-location render/picking support:

```shell
cmake --build --preset macOS-Debug-App
build/macOS-Debug-WithTests/HiveWE.app/Contents/MacOS/HiveWE \
  --map "data/test map/" \
  --smoke-picking \
  --capture-delay-ms 1000
ctest --preset macOS-Debug-WithTests \
  -R 'metal_map_canvas_smoke|metal_map_canvas_artifacts' \
  --output-on-failure
```

The targeted app build rebuilt the map-canvas translation unit, now
`src/main_window/map_canvas_widget.cpp`, and relinked the
Metal app. The direct smoke-picking run succeeded and explicitly selected unit
`0 (sloc)` from the bundled test map before proving doodad and helper-only
doodad picking. The CTest map-canvas smoke and artifact validators passed all 3
selected tests in 25.11 seconds, covering save/reload, mutation save/reload, MPQ
export/reload, render modes, picking, synthetic previews, and map-canvas artifact
validation through the Metal startup path.

On 2026-06-01, the local macOS Debug CTest preset completed for implementation revision `34e549a3e4529fa844671b4b0b7b057543f4db40` with Warcraft content enabled:

```shell
HIVEWE_WARCRAFT_DIR="/Applications/Warcraft III" \
  ctest --preset macOS-Debug-WithTests --output-on-failure
```

All 113 tests passed in 303.18 seconds. That run included the 85 unit tests, renderer evidence helper self-tests, save/reload, mutated save/reload, MPQ export/reload, map-canvas artifact checks, map-render parity smoke, MPQ map-render parity smoke, preview probe smoke/parity, renderer probe smoke/parity, and the Warcraft-backed `metal_full_runtime_smoke`.

On 2026-06-01, the local macOS OpenGL fallback build completed for reviewed
revision `3881c2b71869c548bfb4c12bfdd4a4205456eb01` after the latest Metal
evidence documentation commit:

```shell
cmake --build --preset macOS-Debug-OpenGL
build/macOS-Debug-OpenGL/HiveWE.app/Contents/MacOS/HiveWE --print-render-backend
```

The rebuilt fallback app reported backend `opengl`. Its build provenance records
clean source `3881c2b71869c548bfb4c12bfdd4a4205456eb01`, Debug build type,
`enable_metal=0`, and `build_opengl_renderer=1`. The executable was
`build/macOS-Debug-OpenGL/HiveWE.app/Contents/MacOS/HiveWE` with SHA-256
`fdc32fadef06fa18d545b4bcb48881e178e0c71869ed2fcfdf27bdcab2a6642f` and size
80,500,968 bytes. This remains compile/backend-identity evidence only; real
OpenGL visual parity capture still needs a Windows/Linux OpenGL 4.5 runtime host
because macOS cannot provide the required runtime context.

Earlier on 2026-06-01, the local macOS OpenGL fallback build completed for implementation revision `39690d15e15b8dfed43452b8acdb9e1f3c849479`:

```shell
cmake --build --preset macOS-Debug-OpenGL
build/macOS-Debug-OpenGL/HiveWE.app/Contents/MacOS/HiveWE --print-render-backend
```

The rebuilt fallback app reported backend `opengl`. The executable was `build/macOS-Debug-OpenGL/HiveWE.app/Contents/MacOS/HiveWE` with SHA-256 `4262755a6e87271643b44a074512008e0edb75c8977fa26c7b65846a49b1e08e` and size 80,500,968 bytes. This is compile/backend-identity evidence only; real OpenGL visual parity capture still needs a Windows/Linux OpenGL 4.5 runtime host because macOS cannot provide the required runtime context.

On 2026-05-31, focused local validation completed after the Metal HD-water loading and mutated shadow-map save/reload coverage changes:

- `cmake --build --preset macOS-Debug-App` completed in 335.98 seconds, and `build/macOS-Debug-WithTests/HiveWE.app/Contents/MacOS/HiveWE --print-render-backend` reported `metal`. The rebuilt executable SHA-256 is `c81ccbcd41c3e172b7ec440d06223a87ee183c729ea4d4597de1d967b0752e5a` with size 79,092,648 bytes.
- `cmake --build --preset macOS-Debug-Tests` completed in 3.53 seconds.
- `ctest --preset macOS-Debug-WithTests -R 'metal_backend_identity|metal_unit_frame_tests|metal_mutated_save_reload_smoke' --output-on-failure` passed all 3 selected tests.
- `ctest --preset macOS-Debug-WithTests -R 'metal_save_reload_smoke|metal_map_canvas_smoke|metal_map_canvas_artifacts' --output-on-failure` passed all 4 selected tests, including the map-canvas render smoke and artifact validator.
- `cmake --build --preset macOS-Debug-OpenGL` completed in 386.58 seconds, and `build/macOS-Debug-OpenGL/HiveWE.app/Contents/MacOS/HiveWE --print-render-backend` reported `opengl`. The rebuilt fallback executable SHA-256 is `9ccd5d5a433a5bdb4ea3f4fcc7f100d5ba3b6152610a7ae8e525ffa29cfecd2f` with size 80,497,896 bytes.

These focused checks are now supplemented by the latest reviewed Debug/Release
package evidence and latest reviewed Metal-side PR evidence below. OpenGL
reference bundles, cross-backend parity reports, and a manual visual-review note
are still required for maintainer-facing final evidence.

## Latest Reviewed Full Local Package Evidence

On 2026-06-01, local package and full-smoke validation completed for reviewed
revision `31f8a1ed5972221bafabc4bcdfe40186ec5d4f60`. The Debug-WithTests and
Release apps were rebuilt or confirmed current after refreshing build
provenance:

```shell
cmake --build --preset macOS-Debug-WithTests --target HiveWE
cmake --build --preset macOS-Release --target HiveWE
```

The Debug installed package validator then completed against
`build/macOS-Debug-WithTests/install/HiveWE.app` with clean-source/current-source
gates, required Debug build type, save/reload, mutated save/reload, MPQ
export/reload, renderer-probe, renderer-probe parity, map-canvas, map-render
parity, MPQ map-render parity, preview-probe, preview-probe parity,
shared-preview, shared-preview parity, CPack, DMG verification, and mounted-DMG
checks enabled. The report was written to
`/tmp/hivewe-31f8a1e-debug-package-report.json` and
`/tmp/hivewe-31f8a1e-debug-package-report.md`. All requested checks passed; the
package run intentionally skipped the full installed Warcraft-backed smoke, which
was run separately below. The installed Debug executable was
`build/macOS-Debug-WithTests/install/HiveWE.app/Contents/MacOS/HiveWE` with
SHA-256 `ff3b2f2c3268a86d457a24578b06fa6188b4b16984cbe3dff16b6ddb74ab646a`
and size 78,907,872 bytes.

The Release installed package validator then completed against
`build/macOS-Release/install/HiveWE.app` with the same requested package, smoke,
parity, CPack, DMG verification, and mounted-DMG checks enabled, and reports
written to `/tmp/hivewe-31f8a1e-release-package-report.json` and
`/tmp/hivewe-31f8a1e-release-package-report.md`. All requested checks passed;
the package run intentionally skipped the full installed Warcraft-backed smoke,
which is covered by the Debug installed full-smoke run. The installed Release
executable was `build/macOS-Release/install/HiveWE.app/Contents/MacOS/HiveWE`
with SHA-256
`9a8b049c54a9ca64062cd8566170eda48b55c72dce3478d48aa29a7dc1767511` and size
26,829,424 bytes.

The full installed Debug Metal smoke then completed with Warcraft content from
`/Applications/Warcraft III`:

```shell
python3 tools/run_metal_smoke.py \
  build/macOS-Debug-WithTests/install/HiveWE.app \
  --warcraft-dir "/Applications/Warcraft III" \
  --capture-frame /tmp/hivewe-31f8a1e-installed-metal-smoke.png \
  --artifact-dir /tmp/hivewe-31f8a1e-installed-metal-smoke-artifacts \
  --require-current-source \
  --require-clean-source
```

That run validated save/reload, mutated save/reload, MPQ export/reload, render
modes, picking, real Footman model-editor/object-grid previews, synthetic SD/HD
material previews, particles, ribbons, selection overlays, and Metal renderer
probes through the installed Debug package. It wrote
`/tmp/hivewe-31f8a1e-installed-metal-smoke-artifacts/metal-smoke-provenance.json`,
validated all 30 required PNG artifacts, and captured
`/tmp/hivewe-31f8a1e-installed-metal-smoke.png` at 1920x897 with 1,720,521
varied pixels and SHA-256
`07cf0200524d1727f3b67a3b66461f49fffad24345da978e8570a801c8bce7f9`.
Detached validation of these artifacts should use
`--require-source-head 31f8a1ed5972221bafabc4bcdfe40186ec5d4f60
--require-clean-source`; rerun the evidence capture after any renderer/source
change that must be included in final maintainer-facing evidence.

## Previous Full Local Package Evidence

On 2026-06-01, build provenance was refreshed for reviewed revision
`c935620445ced16b394a20488d125ae1f7b70a8b` without recompiling the already
validated Debug and Release artifacts:

```shell
cmake --build --preset macOS-Debug-WithTests --target HiveWE_build_provenance
cmake --build --preset macOS-Release --target HiveWE_build_provenance
```

The Debug installed package validator then completed against
`build/macOS-Debug-WithTests/install/HiveWE.app` with
`--require-current-source --require-clean-source --require-build-type Debug
--require-full-smoke --warcraft-dir "/Applications/Warcraft III"`, all
save/reload, MPQ export/reload, renderer-probe, preview-probe, shared-preview,
map-canvas, map-render parity, MPQ map-render parity, CPack, DMG verification,
and mounted-DMG checks enabled, and package reports written to
`/tmp/hivewe-debug-c935620-package-report.json` and
`/tmp/hivewe-debug-c935620-package-report.md`. The JSON report records 24
passing checks out of 24. The installed Debug executable SHA-256 was
`1a9590f537a1c9f8874de99c0c20c1cf06f490b66d20efea4c539cd1f51db54a`, size
78,887,648 bytes. The full installed Metal smoke produced
`/tmp/hivewe-debug-c935620-full-metal-smoke.png` at 1920x897 with 1,720,498
varied pixels and retained all 30 PNG artifacts in
`/tmp/hivewe-debug-c935620-full-metal-smoke-artifacts`.

The Release installed package validator then completed against
`build/macOS-Release/install/HiveWE.app` with `--skip-smoke`,
`--require-current-source --require-clean-source --require-build-type Release`,
all save/reload, MPQ export/reload, renderer-probe, preview-probe,
shared-preview, map-canvas, map-render parity, MPQ map-render parity, CPack, DMG
verification, and mounted-DMG checks enabled, and package reports written to
`/tmp/hivewe-release-c935620-package-report.json` and
`/tmp/hivewe-release-c935620-package-report.md`. The JSON report records 23
passing checks out of 24; the intentionally skipped Release full
Warcraft-backed smoke is covered by the Debug package/full-smoke run above. The
installed Release executable SHA-256 was
`9de5988ea5c7dfc3648c83d9fc99cb28f5f119d5b3e0f804afc7d631fd7788b3`, size
26,811,328 bytes.

Earlier on 2026-06-01, the local macOS Debug package validator completed for implementation revision `42c4d7ddb9a5fb92b5b4423c0e678f1dbbdaeb49`:

```shell
python3 tools/check_macos_package.py \
  --build-dir build/macOS-Debug-WithTests \
  --require-current-source \
  --require-clean-source \
  --require-build-type Debug \
  --require-full-smoke \
  --warcraft-dir "/Applications/Warcraft III" \
  --installed-smoke-capture-frame /tmp/hivewe-debug-42c4d7d-full-metal-smoke.png \
  --installed-smoke-artifact-dir /tmp/hivewe-debug-42c4d7d-full-metal-smoke-artifacts \
  --save-reload-smoke \
  --mutated-save-reload-smoke \
  --mpq-export-reload-smoke \
  --renderer-probe-smoke \
  --renderer-probe-parity-smoke \
  --map-canvas-smoke \
  --map-render-parity-smoke \
  --mpq-map-render-parity-smoke \
  --preview-probe-smoke \
  --preview-probe-parity-smoke \
  --shared-preview-probe-smoke \
  --shared-preview-parity-smoke \
  --run-cpack \
  --package-report-json /tmp/hivewe-debug-42c4d7d-package-report.json \
  --package-report-markdown /tmp/hivewe-debug-42c4d7d-package-report.md
```

That run installed `build/macOS-Debug-WithTests/install/HiveWE.app`, verified clean Debug build provenance, bundled `data/test map/`, bundled `data/warcraft/`, and `data/shaders/metal_map_renderer.metal`, rejected forbidden XQuartz/OpenGL dylibs or links, passed codesign and backend identity, passed installed-app save/reload, mutated save/reload, MPQ export/reload, renderer-probe, preview-probe, shared-preview, save/reload map-render parity, MPQ map-render parity, full installed smoke, CPack DMG creation, DMG checksum verification, and mounted-DMG app revalidation. The JSON report records 24 passing checks out of 24. The generated Debug executable was `build/macOS-Debug-WithTests/install/HiveWE.app/Contents/MacOS/HiveWE` with SHA-256 `ef965e87aab31fab2b97a6fb6690ae25950818acc4b70fad8806291300544bbe` and size 78,887,648 bytes. The full installed smoke was captured at `2026-06-01T00:31:42.257145+00:00`, validated all 30 required PNG artifacts, and produced `/tmp/hivewe-debug-42c4d7d-full-metal-smoke.png` at 1920x897 with 1,720,498 varied pixels and SHA-256 `c8d022801168c4e7232b911a0bf5ce3839d9844190888c84effee9f551622d94`.

On 2026-06-01, the local macOS Release package validator completed for the same implementation revision:

```shell
python3 tools/check_macos_package.py \
  --build-dir build/macOS-Release \
  --skip-smoke \
  --require-current-source \
  --require-clean-source \
  --require-build-type Release \
  --save-reload-smoke \
  --mutated-save-reload-smoke \
  --mpq-export-reload-smoke \
  --renderer-probe-smoke \
  --renderer-probe-parity-smoke \
  --map-canvas-smoke \
  --map-render-parity-smoke \
  --mpq-map-render-parity-smoke \
  --shared-preview-probe-smoke \
  --shared-preview-parity-smoke \
  --preview-probe-smoke \
  --preview-probe-parity-smoke \
  --run-cpack \
  --package-report-json /tmp/hivewe-release-42c4d7d-package-report.json \
  --package-report-markdown /tmp/hivewe-release-42c4d7d-package-report.md
```

That run installed `build/macOS-Release/install/HiveWE.app`, verified clean Release build provenance, bundled resources and Metal shader identity, rejected forbidden XQuartz/OpenGL dylibs or links, passed codesign and backend identity, passed installed-app save/reload, mutated save/reload, MPQ export/reload, renderer-probe, preview-probe, shared-preview, save/reload map-render parity, MPQ map-render parity, CPack DMG creation, DMG checksum verification, and mounted-DMG app revalidation. The generated Release executable was `build/macOS-Release/install/HiveWE.app/Contents/MacOS/HiveWE` with SHA-256 `c5d2cd48e7e51bc0a1ab88ab046ff512a13a0991a04b2933bed1c82b76b97c85` and size 26,811,328 bytes. The JSON report records 23 passing checks out of 24; the only unchecked item is the intentionally skipped full installed Warcraft-backed smoke, which is covered by the Debug package/full-smoke run above.

On 2026-06-01, the local macOS Debug package validator completed for implementation revision `a57b2a8d2365a9c3d3d84c380bab2fcdbfe95a33` using the full Warcraft-backed package command recorded above. It installed `build/macOS-Debug-WithTests/install/HiveWE.app`, verified clean build provenance, bundled `data/test map/`, bundled `data/warcraft/`, and `data/shaders/metal_map_renderer.metal`, rejected forbidden XQuartz/OpenGL dylibs or links, required backend `metal`, passed installed-app save/reload, mutated save/reload, MPQ export/reload, renderer-probe, preview-probe, shared-preview, save/reload map-render parity, MPQ map-render parity, and full installed smoke gates, built `HiveWE-0.11.0-Darwin.dmg`, verified the DMG checksum, mounted it read-only, and revalidated the app bundle inside. The JSON report at `/tmp/hivewe-debug-a57b2a8-package-report.json` records 24 passing checks out of 24.

On 2026-06-01, the local macOS Release package validator completed for implementation revision `ef0a3f000bd48ff35525aff6893d116e88b25115`:

```shell
python3 tools/check_macos_package.py \
  --build-dir build/macOS-Release \
  --skip-smoke \
  --require-current-source \
  --require-clean-source \
  --require-build-type Release \
  --save-reload-smoke \
  --mutated-save-reload-smoke \
  --mpq-export-reload-smoke \
  --renderer-probe-smoke \
  --renderer-probe-parity-smoke \
  --map-canvas-smoke \
  --map-render-parity-smoke \
  --mpq-map-render-parity-smoke \
  --shared-preview-probe-smoke \
  --shared-preview-parity-smoke \
  --preview-probe-smoke \
  --preview-probe-parity-smoke \
  --run-cpack \
  --package-report-json /tmp/hivewe-release-ef0a3f0-package-report.json \
  --package-report-markdown /tmp/hivewe-release-ef0a3f0-package-report.md
```

That run installed `build/macOS-Release/install/HiveWE.app`, verified clean Release build provenance, bundled resources and the Metal shader, rejected forbidden XQuartz/OpenGL dylibs or links, passed codesign and backend identity, passed installed-app save/reload, mutated save/reload, MPQ export/reload, renderer-probe, preview-probe, shared-preview, save/reload map-render parity, and MPQ map-render parity gates, built `HiveWE-0.11.0-Darwin.dmg`, verified the DMG checksum, mounted it read-only, and revalidated the app bundle inside. The generated Release executable was `build/macOS-Release/install/HiveWE.app/Contents/MacOS/HiveWE` with SHA-256 `1c6aa20ef127a86f153d74ae4ce85c5ba87a54b3517959443029d5731469de7f` and size 26,811,328 bytes. The JSON report at `/tmp/hivewe-release-ef0a3f0-package-report.json` records 23 passing checks out of 24; the only unchecked item is the intentionally skipped full installed Warcraft-backed smoke, which is covered by the Debug package/full-smoke run above.

On 2026-05-31, the local macOS Debug package validator completed for implementation revision `837c9d6afa8e260b2f5a05906a92b199b602a36d` using the full Warcraft-backed smoke command recorded above. It installed `build/macOS-Debug-WithTests/install/HiveWE.app`, verified clean build provenance, bundled `data/test map/`, bundled `data/warcraft/`, and `data/shaders/metal_map_renderer.metal`, rejected forbidden XQuartz/OpenGL dylibs or links, required backend `metal`, passed installed-app save/reload, mutated save/reload, MPQ export/reload, renderer-probe, preview-probe, shared-preview, save/reload map-render parity, MPQ map-render parity, and full installed smoke gates, built `HiveWE-0.11.0-Darwin.dmg`, verified the DMG checksum, mounted it read-only, and revalidated the app bundle inside. The JSON report at `/tmp/hivewe-debug-837c9d6-package-report.json` records 23 passing checks out of 23.

On 2026-05-31, the local macOS Release package validator completed for the same implementation revision:

```shell
python3 tools/check_macos_package.py \
  --build-dir build/macOS-Release \
  --skip-smoke \
  --require-current-source \
  --require-clean-source \
  --require-build-type Release \
  --save-reload-smoke \
  --mutated-save-reload-smoke \
  --mpq-export-reload-smoke \
  --renderer-probe-smoke \
  --renderer-probe-parity-smoke \
  --map-canvas-smoke \
  --map-render-parity-smoke \
  --mpq-map-render-parity-smoke \
  --preview-probe-smoke \
  --preview-probe-parity-smoke \
  --shared-preview-probe-smoke \
  --shared-preview-parity-smoke \
  --run-cpack \
  --package-report-json /tmp/hivewe-release-837c9d6-package-report.json \
  --package-report-markdown /tmp/hivewe-release-837c9d6-package-report.md
```

That run installed `build/macOS-Release/install/HiveWE.app`, verified the same bundle/provenance/resource/dylib/backend requirements, passed the installed-app save/reload, mutated save/reload, MPQ export/reload, renderer-probe, preview-probe, shared-preview, save/reload map-render parity, and MPQ map-render parity gates, built and verified the DragNDrop DMG, mounted it read-only, and revalidated the bundled app inside. The Release validation completed in 217.95 seconds. The generated Release executable was `build/macOS-Release/install/HiveWE.app/Contents/MacOS/HiveWE` with SHA-256 `f24151a12ba47069a756c1a93919e823ee2794ffebf7ded1c1ac559695eb486a` and size 26,751,184 bytes. The JSON report records 22 passing checks out of 23; the only unchecked item is the intentionally skipped full installed Warcraft-backed smoke, which is covered by the Debug package run above.

## Latest Reviewed Metal-Side PR Evidence Audit

On 2026-06-01, local installed-app Metal-side PR evidence was captured from
`build/macOS-Debug-WithTests/install/HiveWE.app` for reviewed revision
`31f8a1ed5972221bafabc4bcdfe40186ec5d4f60`:

- Default Metal PR bundle: `/tmp/hivewe-metal-pr-evidence-31f8a1e/hivewe-metal-artifacts.zip`
- MPQ export/reload Metal map-render bundle: `/tmp/hivewe-metal-pr-evidence-31f8a1e/hivewe-metal-mpq-map-artifacts.zip`
- Capture summary: `/tmp/hivewe-metal-pr-evidence-31f8a1e/renderer-pr-capture-summary.md`
- Full installed Metal smoke artifacts: `/tmp/hivewe-31f8a1e-installed-metal-smoke-artifacts`
- Full installed Metal smoke frame: `/tmp/hivewe-31f8a1e-installed-metal-smoke.png`
- Debug package report: `/tmp/hivewe-31f8a1e-debug-package-report.json`
- Release package report: `/tmp/hivewe-31f8a1e-release-package-report.json`
- Local installed-app audit: `/tmp/hivewe-31f8a1e-local-metal-audit.md`
- Metal-side handoff bundle: `/tmp/hivewe-renderer-pr-handoff-31f8a1e.zip`

The default Metal capture passed `--pr-gate` and packaged 24 PNG artifacts
covering saved/reloaded map render modes, picking, real Footman
model-editor/object-grid previews, synthetic SD/HD material previews, particles,
ribbons, selection overlays, and Metal renderer probes. The MPQ capture passed
`--pr-gate --map-render-only --map-after-mpq-export-reload` and packaged 9 PNG
map-render/picking artifacts after MPQ export/unpack/reload. Both capture
manifests record clean source
`31f8a1ed5972221bafabc4bcdfe40186ec5d4f60`, backend `metal`, installed Debug
executable SHA-256
`ff3b2f2c3268a86d457a24578b06fa6188b4b16984cbe3dff16b6ddb74ab646a`, and size
78,907,872 bytes, matching the Debug package validation report. The default
archive SHA-256 is
`19674f54bdaa17f43072bd24e1e6664d129b17ac52d628ede24bb0ddbeb35610`, and the
MPQ archive SHA-256 is
`d840ebf6fbff5f526f96871a0a4e91f427ab18612e70110543df1f7df36f9752`.

`tools/create_renderer_pr_handoff.py` validated and packaged this Metal-side
evidence into `/tmp/hivewe-renderer-pr-handoff-31f8a1e.zip`. The archive is 78
MB with SHA-256
`9bcb4389f4bbb59ce701133bd0d6e4215ad9df1d39414f7557d3f46a62030d25`, and its
generated README includes source-pinned OpenGL capture, default/MPQ parity,
manual-review-template, and strict final-audit commands using the bundled Metal
evidence paths.

The installed-app local audit was:

```shell
python3 tools/audit_renderer_pr_evidence.py \
  --metal-artifact-archive /tmp/hivewe-metal-pr-evidence-31f8a1e/hivewe-metal-artifacts.zip \
  --metal-mpq-artifact-archive /tmp/hivewe-metal-pr-evidence-31f8a1e/hivewe-metal-mpq-map-artifacts.zip \
  --installed-smoke-app build/macOS-Debug-WithTests/install/HiveWE.app \
  --installed-smoke-artifact-dir /tmp/hivewe-31f8a1e-installed-metal-smoke-artifacts \
  --installed-smoke-capture-frame /tmp/hivewe-31f8a1e-installed-metal-smoke.png \
  --debug-package-report-json /tmp/hivewe-31f8a1e-debug-package-report.json \
  --release-package-report-json /tmp/hivewe-31f8a1e-release-package-report.json \
  --allow-missing-opengl \
  --require-source-head 31f8a1ed5972221bafabc4bcdfe40186ec5d4f60 \
  --require-clean-source \
  --markdown-output /tmp/hivewe-31f8a1e-local-metal-audit.md
```

It reported 9 passing checks, 4 warnings, and 0 failures. The passing checks
prove the installed-app Metal default capture, installed-app Metal MPQ capture,
default/MPQ executable identity, package identity, full installed Metal smoke app
identity, Debug package report, and Release package report. The remaining
warnings are external final-evidence gates: capture OpenGL reference bundles on
a Windows/Linux OpenGL 4.5 runtime, generate default and MPQ OpenGL-vs-Metal
parity reports from those bundles, then complete the manual visual-review note
over the final diffs.

This Metal-side handoff bundle is intentionally not final PR evidence. A strict
audit for `31f8a1ed5972221bafabc4bcdfe40186ec5d4f60` still requires the
unavailable external OpenGL/manual-review gates: OpenGL default capture, OpenGL
MPQ capture, final default parity report, MPQ parity report, and manual
visual-review note.

## Previous Metal-Side PR Evidence Audit

On 2026-06-01, local installed-app Metal-side PR evidence was captured from
`build/macOS-Debug-WithTests/install/HiveWE.app` for implementation revision
`c935620445ced16b394a20488d125ae1f7b70a8b`:

- Default Metal PR bundle: `/tmp/hivewe-installed-metal-pr-evidence-c935620/hivewe-metal-artifacts.zip`
- MPQ export/reload Metal map-render bundle: `/tmp/hivewe-installed-metal-pr-evidence-c935620/hivewe-metal-mpq-map-artifacts.zip`
- Capture summary: `/tmp/hivewe-installed-metal-pr-evidence-c935620/renderer-pr-capture-summary.md`
- Full installed Metal smoke artifacts: `/tmp/hivewe-debug-c935620-full-metal-smoke-artifacts`
- Full installed Metal smoke frame: `/tmp/hivewe-debug-c935620-full-metal-smoke.png`
- Debug package report: `/tmp/hivewe-debug-c935620-package-report.json`
- Release package report: `/tmp/hivewe-release-c935620-package-report.json`
- Local installed-app audit: `/tmp/hivewe-installed-metal-pr-evidence-c935620/local-installed-metal-audit.md`
- Metal-side handoff bundle: `/tmp/hivewe-renderer-pr-handoff-c935620.zip`

The default capture packaged 24 PNG artifacts covering saved/reloaded map
render modes, picking, real Footman model-editor/object-grid previews,
synthetic SD/HD material previews, particles, ribbons, selection overlays, and
Metal renderer probes. The MPQ capture packaged 9 PNG map-render/picking
artifacts after MPQ export/unpack/reload. Both capture manifests record clean
source `c935620445ced16b394a20488d125ae1f7b70a8b`, backend `metal`, installed
Debug executable SHA-256
`1a9590f537a1c9f8874de99c0c20c1cf06f490b66d20efea4c539cd1f51db54a`, and size
78,887,648 bytes, matching the Debug package validation report.

`tools/create_renderer_pr_handoff.py` validated and packaged the current
Metal-side evidence into `/tmp/hivewe-renderer-pr-handoff-c935620.zip`. The
archive is 78 MB with SHA-256
`6666f0c0acdf9c5d7f10e7f68ca6fd8234253cb939253b0a727080e0c9b1b7a9`, and its
generated README includes source-pinned OpenGL capture, default/MPQ parity,
manual-review-template, and strict final-audit commands using the bundled Metal
evidence paths.

The installed-app local audit was:

```shell
python3 tools/audit_renderer_pr_evidence.py \
  --metal-artifact-archive /tmp/hivewe-installed-metal-pr-evidence-c935620/hivewe-metal-artifacts.zip \
  --metal-mpq-artifact-archive /tmp/hivewe-installed-metal-pr-evidence-c935620/hivewe-metal-mpq-map-artifacts.zip \
  --installed-smoke-artifact-dir /tmp/hivewe-debug-c935620-full-metal-smoke-artifacts \
  --installed-smoke-capture-frame /tmp/hivewe-debug-c935620-full-metal-smoke.png \
  --installed-smoke-app build/macOS-Debug-WithTests/install/HiveWE.app \
  --debug-package-report-json /tmp/hivewe-debug-c935620-package-report.json \
  --release-package-report-json /tmp/hivewe-release-c935620-package-report.json \
  --allow-missing-opengl \
  --allow-missing-manual-review \
  --require-current-source \
  --require-clean-source \
  --markdown-output /tmp/hivewe-installed-metal-pr-evidence-c935620/local-installed-metal-audit.md
```

It reported 9 passing checks, 4 warnings, and 0 failures. The passing checks
prove the installed-app Metal default capture, installed-app Metal MPQ capture,
default/MPQ executable identity, package identity, full installed Metal smoke app
identity, Debug package report, and Release package report. The remaining
warnings are external final-evidence gates: capture OpenGL reference bundles on
a Windows/Linux OpenGL 4.5 runtime, generate default and MPQ OpenGL-vs-Metal
parity reports from those bundles, then complete the manual visual-review note
over the final diffs.

This Metal-side handoff bundle is intentionally not final PR evidence.
A strict audit for `c935620445ced16b394a20488d125ae1f7b70a8b` fails only on the
unavailable external OpenGL/manual-review gates: missing OpenGL default capture,
missing OpenGL MPQ capture, missing final default parity report, missing MPQ
parity report, and missing manual visual-review note.

Earlier on 2026-06-01, local installed-app Metal-side PR evidence was captured from `build/macOS-Debug-WithTests/install/HiveWE.app` for implementation revision `42c4d7ddb9a5fb92b5b4423c0e678f1dbbdaeb49`:

- Default Metal PR bundle: `/tmp/hivewe-installed-metal-pr-evidence-42c4d7d/hivewe-metal-artifacts.zip`
- MPQ export/reload Metal map-render bundle: `/tmp/hivewe-installed-metal-pr-evidence-42c4d7d/hivewe-metal-mpq-map-artifacts.zip`
- Capture summary: `/tmp/hivewe-installed-metal-pr-evidence-42c4d7d/renderer-pr-capture-summary.md`
- Full installed Metal smoke artifacts: `/tmp/hivewe-debug-42c4d7d-full-metal-smoke-artifacts`
- Full installed Metal smoke frame: `/tmp/hivewe-debug-42c4d7d-full-metal-smoke.png`
- Debug package report: `/tmp/hivewe-debug-42c4d7d-package-report.json`
- Release package report: `/tmp/hivewe-release-42c4d7d-package-report.json`
- Local installed-app audit: `/tmp/hivewe-installed-metal-pr-evidence-42c4d7d/local-installed-metal-audit.md`
- Metal-side handoff bundle: `/tmp/hivewe-renderer-pr-handoff-42c4d7d.zip`

The default capture was recorded at `2026-06-01T00:39:04.094211+00:00`, passed `--pr-gate`, and packaged 24 PNG artifacts covering saved/reloaded map render modes, picking, real Footman model-editor/object-grid previews, synthetic SD/HD material previews, particles, ribbons, selection overlays, and Metal renderer probes. The MPQ capture was recorded at `2026-06-01T00:39:27.760194+00:00`, passed `--pr-gate --map-render-only --map-after-mpq-export-reload`, and packaged 9 PNG map-render/picking artifacts. Both capture manifests record clean source `42c4d7ddb9a5fb92b5b4423c0e678f1dbbdaeb49`, backend `metal`, installed Debug executable SHA-256 `ef965e87aab31fab2b97a6fb6690ae25950818acc4b70fad8806291300544bbe`, and size 78,887,648 bytes, matching the Debug package validation report.

`tools/create_renderer_pr_handoff.py` validated and packaged that Metal-side evidence into `/tmp/hivewe-renderer-pr-handoff-42c4d7d.zip`. The archive is 78 MB with SHA-256 `a1fd8c8c0c2ba94ba55683e743a6b4875cc2596ba0b1df635011e39e2bca6bb0`, and its generated README includes source-pinned OpenGL capture, default/MPQ parity, manual-review-template, and strict final-audit commands using the bundled Metal evidence paths.

The installed-app local audit was:

```shell
python3 tools/audit_renderer_pr_evidence.py \
  --metal-artifact-archive /tmp/hivewe-installed-metal-pr-evidence-42c4d7d/hivewe-metal-artifacts.zip \
  --metal-mpq-artifact-archive /tmp/hivewe-installed-metal-pr-evidence-42c4d7d/hivewe-metal-mpq-map-artifacts.zip \
  --installed-smoke-artifact-dir /tmp/hivewe-debug-42c4d7d-full-metal-smoke-artifacts \
  --installed-smoke-capture-frame /tmp/hivewe-debug-42c4d7d-full-metal-smoke.png \
  --installed-smoke-app build/macOS-Debug-WithTests/install/HiveWE.app \
  --debug-package-report-json /tmp/hivewe-debug-42c4d7d-package-report.json \
  --release-package-report-json /tmp/hivewe-release-42c4d7d-package-report.json \
  --allow-missing-opengl \
  --allow-missing-manual-review \
  --require-source-head 42c4d7ddb9a5fb92b5b4423c0e678f1dbbdaeb49 \
  --require-clean-source \
  --markdown-output /tmp/hivewe-installed-metal-pr-evidence-42c4d7d/local-installed-metal-audit.md
```

It reported 9 passing checks, 4 warnings, and 0 failures. The passing checks prove the installed-app Metal default capture, installed-app Metal MPQ capture, default/MPQ executable identity, package identity, full installed Metal smoke app identity, Debug package report, and Release package report. The remaining warnings are external final-evidence gates: capture OpenGL reference bundles on a Windows/Linux OpenGL 4.5 runtime, generate default and MPQ OpenGL-vs-Metal parity reports from those bundles, then complete the manual visual-review note over the final diffs.

On 2026-06-01, local build-tree Metal-side PR evidence was captured from `build/macOS-Debug-WithTests/HiveWE.app` for implementation revision `2f49669b87e734086de050db58891f8845941750`:

- Default Metal PR bundle: `/tmp/hivewe-metal-pr-evidence-2f49669/hivewe-metal-artifacts.zip`
- MPQ export/reload Metal map-render bundle: `/tmp/hivewe-metal-pr-evidence-2f49669/hivewe-metal-mpq-map-artifacts.zip`
- Capture summary: `/tmp/hivewe-metal-pr-evidence-2f49669/renderer-pr-capture-summary.md`
- Build-tree full Metal smoke artifacts: `/tmp/hivewe-full-metal-smoke-2f49669-artifacts`
- Build-tree full Metal smoke frame: `/tmp/hivewe-full-metal-smoke-2f49669.png`
- Local audit: `/tmp/hivewe-metal-pr-evidence-2f49669/local-metal-audit.md`

The default capture was recorded at `2026-05-31T23:58:28.723321+00:00`, passed `--pr-gate`, and packaged 24 PNG artifacts covering saved/reloaded map render modes, picking, real Footman model-editor/object-grid previews, synthetic SD/HD material previews, particles, ribbons, selection overlays, and Metal renderer probes. The MPQ capture was recorded at `2026-05-31T23:59:17.125278+00:00`, passed `--pr-gate --map-render-only --map-after-mpq-export-reload`, and packaged 9 PNG map-render/picking artifacts. Both capture manifests record clean source `2f49669b87e734086de050db58891f8845941750`, backend `metal`, executable SHA-256 `f2217a48a778c36d2113d28a8b46e5a58b0563fc541fafc88024590404a8cd40`, and size 79,330,712 bytes.

The build-tree full Metal smoke rerun captured `2026-06-01T00:01:22.228758+00:00`, validated save/reload, mutated save/reload, MPQ export/reload, render-mode toggles, picking, real and synthetic previews, renderer probes, `metal-smoke-provenance.json`, and all 30 required PNG artifacts. Its frame `/tmp/hivewe-full-metal-smoke-2f49669.png` was 1920x897 with 1,720,498 varied pixels and SHA-256 `229ce210ce31ea4802c01fb4768f2c2d7f99f0640a8f1b65f43403c52359439d`.

The local Metal-only audit was:

```shell
python3 tools/audit_renderer_pr_evidence.py \
  --metal-artifact-archive /tmp/hivewe-metal-pr-evidence-2f49669/hivewe-metal-artifacts.zip \
  --metal-mpq-artifact-archive /tmp/hivewe-metal-pr-evidence-2f49669/hivewe-metal-mpq-map-artifacts.zip \
  --installed-smoke-artifact-dir /tmp/hivewe-full-metal-smoke-2f49669-artifacts \
  --installed-smoke-capture-frame /tmp/hivewe-full-metal-smoke-2f49669.png \
  --allow-missing-opengl \
  --allow-missing-manual-review \
  --allow-missing-installed-app-identity \
  --require-current-source \
  --require-clean-source \
  --markdown-output /tmp/hivewe-metal-pr-evidence-2f49669/local-metal-audit.md
```

It reported 4 passing checks, 9 warnings, and 0 failures. The passing checks prove the current Metal default capture, Metal MPQ capture, default/MPQ executable identity, and full Metal smoke artifacts. The warnings are the remaining final-evidence boundary: this build-tree capture is not package-identity proof, current Debug/Release package reports for the same capture executable were not supplied to this audit, OpenGL reference bundles and default/MPQ parity reports are still missing, and the manual visual-review note cannot be completed until final OpenGL-vs-Metal diffs exist.

On 2026-05-31, local Metal-side PR evidence was captured from `build/macOS-Debug-WithTests/install/HiveWE.app` for implementation revision `837c9d6afa8e260b2f5a05906a92b199b602a36d`:

- Default Metal PR bundle: `/tmp/hivewe-metal-pr-evidence-837c9d6/hivewe-metal-artifacts.zip`
- MPQ export/reload Metal map-render bundle: `/tmp/hivewe-metal-pr-evidence-837c9d6/hivewe-metal-mpq-map-artifacts.zip`
- Capture summary: `/tmp/hivewe-metal-pr-evidence-837c9d6/renderer-pr-capture-summary.md`
- Full installed Metal smoke artifacts: `/tmp/hivewe-debug-837c9d6-full-metal-smoke-artifacts`
- Full installed Metal smoke frame: `/tmp/hivewe-debug-837c9d6-full-metal-smoke.png`
- Debug package report: `/tmp/hivewe-debug-837c9d6-package-report.json`
- Release package report: `/tmp/hivewe-release-837c9d6-package-report.json`

The capture helper completed in 82.19 seconds. The default archive contains the saved/reloaded map render, picking, real Footman model-editor/object-grid previews, synthetic SD/HD material previews, particles, ribbons, selection overlays, and Metal renderer probes with 24 PNG artifacts. The MPQ archive contains the MPQ export/unpack/reload map-render and picking evidence with 9 PNG artifacts.

The Metal-only local audit completed in 20.54 seconds and wrote `/tmp/hivewe-local-837c9d6-metal-only-audit.md`:

```shell
python3 tools/audit_renderer_pr_evidence.py \
  --metal-artifact-archive /tmp/hivewe-metal-pr-evidence-837c9d6/hivewe-metal-artifacts.zip \
  --metal-mpq-artifact-archive /tmp/hivewe-metal-pr-evidence-837c9d6/hivewe-metal-mpq-map-artifacts.zip \
  --installed-smoke-artifact-dir /tmp/hivewe-debug-837c9d6-full-metal-smoke-artifacts \
  --installed-smoke-capture-frame /tmp/hivewe-debug-837c9d6-full-metal-smoke.png \
  --installed-smoke-app build/macOS-Debug-WithTests/install/HiveWE.app \
  --debug-package-report-json /tmp/hivewe-debug-837c9d6-package-report.json \
  --release-package-report-json /tmp/hivewe-release-837c9d6-package-report.json \
  --require-current-source \
  --require-clean-source \
  --allow-missing-opengl \
  --allow-missing-manual-review \
  --markdown-output /tmp/hivewe-local-837c9d6-metal-only-audit.md
```

The audit passed the Metal capture, Metal package identity, Metal MPQ capture, Metal MPQ package identity, installed full Metal smoke, installed full-smoke app identity, Debug package report, and Release package report checks. It reported 8 passing checks, 4 warnings, and 0 failures. The warnings are the remaining maintainer-facing evidence boundary: capture the Windows/Linux OpenGL 4.5 reference bundles, generate the default and MPQ OpenGL-vs-Metal parity reports, and complete the manual visual-review note for the final diffs.

## Archived Full Local Package Evidence

On 2026-05-31, the local macOS Debug package validator completed for implementation revision `4699f7953746f77c45e95fa2434c5d7fa231df19`:

```shell
python3 tools/check_macos_package.py \
  --build-dir build/macOS-Debug-WithTests \
  --require-current-source \
  --require-clean-source \
  --require-build-type Debug \
  --require-full-smoke \
  --warcraft-dir "/Applications/Warcraft III" \
  --installed-smoke-capture-frame /tmp/hivewe-debug-4699f79-full-metal-smoke.png \
  --installed-smoke-artifact-dir /tmp/hivewe-debug-4699f79-full-metal-smoke-artifacts \
  --save-reload-smoke \
  --mutated-save-reload-smoke \
  --mpq-export-reload-smoke \
  --renderer-probe-smoke \
  --renderer-probe-parity-smoke \
  --map-canvas-smoke \
  --map-render-parity-smoke \
  --mpq-map-render-parity-smoke \
  --preview-probe-smoke \
  --preview-probe-parity-smoke \
  --shared-preview-probe-smoke \
  --shared-preview-parity-smoke \
  --run-cpack \
  --package-report-json /tmp/hivewe-debug-4699f79-package-report.json \
  --package-report-markdown /tmp/hivewe-debug-4699f79-package-report.md
```

That run installed `build/macOS-Debug-WithTests/install/HiveWE.app`, verified build provenance, bundled `data/test map/`, bundled `data/warcraft/`, and `data/shaders/metal_map_renderer.metal`, rejected forbidden XQuartz/OpenGL dylibs or links, required backend `metal`, passed installed-app save/reload, mutated save/reload, MPQ export/reload, renderer-probe, preview-probe, shared-preview, save/reload map-render parity, and MPQ map-render parity gates, ran the full installed Warcraft-backed Metal smoke recorded above, built `HiveWE-0.11.0-Darwin.dmg`, verified the DMG checksum, mounted it read-only, and revalidated the app bundle inside. The generated Debug executable was `build/macOS-Debug-WithTests/install/HiveWE.app/Contents/MacOS/HiveWE` with SHA-256 `c213b13ddcc3995736be0fe558800e64cb2325ab1bdbb5a6638c9d6e59a63610` and size 78,649,856 bytes.

On 2026-05-31, the local macOS Release package validator completed for implementation revision `4699f7953746f77c45e95fa2434c5d7fa231df19`:

```shell
python3 tools/check_macos_package.py \
  --build-dir build/macOS-Release \
  --skip-smoke \
  --require-current-source \
  --require-clean-source \
  --require-build-type Release \
  --save-reload-smoke \
  --mutated-save-reload-smoke \
  --mpq-export-reload-smoke \
  --renderer-probe-smoke \
  --renderer-probe-parity-smoke \
  --map-canvas-smoke \
  --map-render-parity-smoke \
  --mpq-map-render-parity-smoke \
  --preview-probe-smoke \
  --preview-probe-parity-smoke \
  --shared-preview-probe-smoke \
  --shared-preview-parity-smoke \
  --run-cpack \
  --package-report-json /tmp/hivewe-release-4699f79-package-report.json \
  --package-report-markdown /tmp/hivewe-release-4699f79-package-report.md
```

That run installed `build/macOS-Release/install/HiveWE.app`, verified build provenance, bundled `data/test map/`, bundled `data/warcraft/`, and `data/shaders/metal_map_renderer.metal`, rejected forbidden XQuartz/OpenGL dylibs or links, required backend `metal`, passed installed-app save/reload, mutated save/reload, MPQ export/reload, renderer-probe, preview-probe, shared-preview, save/reload map-render parity, and MPQ map-render parity gates, built `HiveWE-0.11.0-Darwin.dmg`, verified the DMG checksum, mounted it read-only, and revalidated the app bundle inside. The generated Release executable was `build/macOS-Release/install/HiveWE.app/Contents/MacOS/HiveWE` with SHA-256 `ef451bacbbd0fc4e91b8fe24848a73423e66ff15e697f0c42b14cacffc1d7e63` and size 26,750,592 bytes. The full installed Warcraft-backed smoke was intentionally covered on the Debug installed app above; use the same `--require-full-smoke` package-validator path for Release if a Release full-smoke artifact is needed.

## Archived OpenGL Fallback Evidence

On 2026-05-31, the macOS OpenGL fallback compile gate completed for implementation revision `4699f7953746f77c45e95fa2434c5d7fa231df19`:

```shell
cmake --preset macOS-Debug-OpenGL
cmake --build --preset macOS-Debug-OpenGL
./build/macOS-Debug-OpenGL/HiveWE.app/Contents/MacOS/HiveWE --print-render-backend
```

The configure step reused already-installed vcpkg dependencies and completed in 2.29 seconds. The separate OpenGL build tree compiled successfully in 584.96 seconds, and `--print-render-backend` reported `opengl`. The bundled build provenance recorded `enable_metal=0`, `build_opengl_renderer=1`, clean source revision `4699f7953746f77c45e95fa2434c5d7fa231df19`, and Debug build type. The generated fallback executable was `build/macOS-Debug-OpenGL/HiveWE.app/Contents/MacOS/HiveWE` with SHA-256 `d3fb7428567a27d21aa73ce33f6145ea06ce02e362a8c2ff88488c77c05bc150` and size 80,497,256 bytes. This remains a compile/backend-identity gate on macOS, not runtime visual parity, because Apple's OpenGL runtime does not expose the OpenGL 4.5 profile required by the legacy renderer.

## Archived Local PR Evidence Audit

On 2026-05-31, local Metal-side PR evidence was captured from `build/macOS-Debug-WithTests/install/HiveWE.app` for implementation revision `4699f7953746f77c45e95fa2434c5d7fa231df19`. Runtime commits have landed after these app/package artifacts, so they are archived evidence rather than current-head PR evidence. Replay audits for these archived artifacts should use `--require-source-head 4699f7953746f77c45e95fa2434c5d7fa231df19` instead of `--require-current-source`:

- Default Metal PR bundle: `/tmp/hivewe-metal-pr-evidence-4699f79/hivewe-metal-artifacts.zip`
- MPQ export/reload Metal map-render bundle: `/tmp/hivewe-metal-pr-evidence-4699f79/hivewe-metal-mpq-map-artifacts.zip`
- Capture summary: `/tmp/hivewe-metal-pr-evidence-4699f79/renderer-pr-capture-summary.md`
- Full installed Metal smoke artifacts: `/tmp/hivewe-debug-4699f79-full-metal-smoke-artifacts`
- Full installed Metal smoke frame: `/tmp/hivewe-debug-4699f79-full-metal-smoke.png`
- Debug package report: `/tmp/hivewe-debug-4699f79-package-report.json`
- Release package report: `/tmp/hivewe-release-4699f79-package-report.json`

The Metal-only local audit passed with 8 passing checks, 4 warnings, and 0 failures:

```shell
python3 tools/audit_renderer_pr_evidence.py \
  --metal-artifact-archive /tmp/hivewe-metal-pr-evidence-4699f79/hivewe-metal-artifacts.zip \
  --metal-mpq-artifact-archive /tmp/hivewe-metal-pr-evidence-4699f79/hivewe-metal-mpq-map-artifacts.zip \
  --installed-smoke-artifact-dir /tmp/hivewe-debug-4699f79-full-metal-smoke-artifacts \
  --installed-smoke-capture-frame /tmp/hivewe-debug-4699f79-full-metal-smoke.png \
  --installed-smoke-app build/macOS-Debug-WithTests/install/HiveWE.app \
  --debug-package-report-json /tmp/hivewe-debug-4699f79-package-report.json \
  --release-package-report-json /tmp/hivewe-release-4699f79-package-report.json \
  --require-source-head 4699f7953746f77c45e95fa2434c5d7fa231df19 \
  --require-clean-source \
  --allow-missing-opengl \
  --allow-missing-manual-review \
  --markdown-output /tmp/hivewe-local-4699f79-metal-only-audit-after-relative-handoff.md
```

The stricter pre-review audit with `--require-mpq-parity-report` passed the Metal capture, Metal MPQ capture, installed full Metal smoke, Debug package, and Release package checks, then failed only because the Windows/Linux OpenGL 4.5 MPQ capture and resulting MPQ parity report are not available on this macOS host. It also warned that the default OpenGL reference capture, final renderer parity report, and manual visual-review note are still missing. This is the expected remaining PR evidence boundary: the macOS Metal side is internally consistent, but maintainer-facing completion still requires OpenGL reference bundles from a capable OpenGL host, cross-backend default and MPQ parity reports, and a human review note for the final diffs.

## PR Evidence Checklist

For a maintainer-facing PR, attach or link these exact artifacts:

- CI result for the pinned `macos-metal` job, including the printed macOS/Xcode/CMake/Ninja/Homebrew LLVM/ccache/vcpkg versions.
- `ctest --preset macOS-Debug-WithTests --output-on-failure` output from a clean tree.
- `ctest --preset macOS-Release --output-on-failure` output from a clean tree.
- Installed Debug bundle validation from `tools/check_macos_package.py --build-dir build/macOS-Debug-WithTests --skip-smoke --require-current-source --require-clean-source --require-build-type Debug --save-reload-smoke --mutated-save-reload-smoke --mpq-export-reload-smoke --renderer-probe-smoke --renderer-probe-parity-smoke --map-canvas-smoke --map-render-parity-smoke --mpq-map-render-parity-smoke --preview-probe-smoke --preview-probe-parity-smoke --shared-preview-probe-smoke --shared-preview-parity-smoke --run-cpack --package-report-json <debug-package-report.json> --package-report-markdown <debug-package-report.md>`.
- Release bundle validation from `tools/check_macos_package.py --build-dir build/macOS-Release --skip-smoke --require-current-source --require-clean-source --require-build-type Release --save-reload-smoke --mutated-save-reload-smoke --mpq-export-reload-smoke --renderer-probe-smoke --renderer-probe-parity-smoke --map-canvas-smoke --map-render-parity-smoke --mpq-map-render-parity-smoke --preview-probe-smoke --preview-probe-parity-smoke --shared-preview-probe-smoke --shared-preview-parity-smoke --run-cpack --package-report-json <release-package-report.json> --package-report-markdown <release-package-report.md>`.
- Warcraft-backed full runtime smoke output and artifact directory when proprietary Warcraft content is available: `python3 tools/run_metal_smoke.py build/macOS-Debug-WithTests/install/HiveWE.app --warcraft-dir "<Warcraft III>" --capture-frame <png> --artifact-dir <dir> --require-current-source --require-clean-source`.
- OpenGL reference artifact directory captured on a Windows/Linux OpenGL 4.5 runtime with `tools/run_renderer_parity.py --capture-only opengl --opengl-app <OpenGL HiveWE executable> --opengl-artifact-dir <opengl artifacts> --map "data/test map/" --warcraft-dir "<Warcraft III>" --pr-gate`. For Windows, the `Build HiveWE` workflow can be manually dispatched with `windows_runner` pointing at a GPU-capable self-hosted runner and `windows_warcraft_dir` pointing at that machine's Warcraft III install; it uploads the default and MPQ OpenGL bundles as `windows-opengl-pr-renderer-evidence`. Renderer-impacting pull requests now fail the final evidence job instead of uploading only a skip note when any OpenGL, Metal, installed-smoke, or package-report artifact is missing; set the manual dispatch input `renderer_pr_evidence_required` to require the same evidence set on demand.
- Metal candidate artifact directory captured from the reviewed installed macOS Debug package app with `tools/run_renderer_parity.py --capture-only metal --metal-app build/macOS-Debug-WithTests/install/HiveWE.app --metal-artifact-dir <metal artifacts> --map "data/test map/" --warcraft-dir "<Warcraft III>" --pr-gate`. The `macos-metal` CI job uploads this packaged candidate bundle, a separate Metal MPQ export/reload map-render bundle, and a local missing-OpenGL audit report when `HIVEWE_WARCRAFT_DIR` is configured.
- Optional zipped OpenGL reference and Metal candidate bundles from `tools/package_renderer_capture.py --artifact-dir <artifacts> --expected-backend <opengl|metal> --require-current-source --require-clean-source --output <artifacts.zip>` for transfer between capture hosts or PR artifact storage; each zip summary should list the required artifact names, captured dimensions/SHA-256 prefixes, host/app binary provenance, a source-pinned mode-specific comparison command, and PR-gate reminders.
- When validating detached installed apps, DMGs, full-smoke artifacts, or final PR evidence audits after the branch has advanced, replace `--require-current-source` with `--require-source-head <reviewed sha>` and keep `--require-clean-source`.
- Final `tools/run_renderer_parity.py --skip-capture --opengl-artifact-archive <opengl artifacts.zip> --metal-artifact-archive <metal artifacts.zip> --report-dir <report dir> --pr-gate` report directory containing `renderer-parity.md`, `renderer-parity.json`, `renderer-parity-provenance.json`, and diff PNGs.
- MPQ export/reload map-render parity report from `tools/run_renderer_parity.py --skip-capture --pr-gate --map-render-only --map-after-mpq-export-reload --opengl-artifact-archive <opengl mpq artifacts.zip> --metal-artifact-archive <metal mpq artifacts.zip> --report-dir <mpq report dir>`. The final PR audit command below uses `--require-mpq-parity-report`, so this report must be present. The `macos-metal` CI job uploads the Metal MPQ bundle as `hivewe-metal-pr-mpq-map-artifacts.zip`; pair it with a matching OpenGL MPQ bundle captured on the OpenGL host, and pass both MPQ bundles to the final audit so it can verify the report used the submitted MPQ evidence.
- If both CI-side bundles are present, the `final-renderer-pr-evidence` artifact from the `renderer-pr-evidence` job contains the generated default parity report, MPQ parity report, automated pre-review audit, and prefilled manual-review template. This artifact is still pre-review evidence until the template is replaced by a real visual-review note and the strict final audit is rerun with the installed macOS app available.
- A short manual visual-review note covering the final OpenGL/Metal diff metrics, especially any nonzero diffs for terrain cliffs, water, SD/HD materials, team colors, transparency, selection overlays, picking, particles, model editor previews, object-grid previews, post-save/reload map frames, and MPQ export/reload map frames. Include the reviewed source revision in the note. Generate the starter note with `tools/audit_renderer_pr_evidence.py --write-manual-review-template <manual review note> --opengl-artifact-archive <opengl artifacts.zip> --metal-artifact-archive <metal artifacts.zip> --opengl-mpq-artifact-archive <opengl mpq artifacts.zip> --metal-mpq-artifact-archive <metal mpq artifacts.zip> --parity-report-dir <report dir> --mpq-parity-report-dir <mpq report dir> --require-current-source`, or use `--require-source-head <reviewed sha>` instead of `--require-current-source` for detached evidence, then replace every TODO/placeholder before the final audit.
- Final `tools/audit_renderer_pr_evidence.py --opengl-artifact-archive <opengl artifacts.zip> --metal-artifact-archive <metal artifacts.zip> --opengl-mpq-artifact-archive <opengl mpq artifacts.zip> --metal-mpq-artifact-archive <metal mpq artifacts.zip> --parity-report-dir <report dir> --mpq-parity-report-dir <mpq report dir> --installed-smoke-artifact-dir <dir> --installed-smoke-capture-frame <png> --debug-package-report-json <debug-package-report.json> --release-package-report-json <release-package-report.json> --manual-review-note <manual review note> --require-current-source --require-clean-source --require-mpq-parity-report --markdown-output <audit.md>` report showing all required automated PR evidence, installed full-smoke app identity, Debug/Release package validation, MPQ capture/parity provenance, same-executable default/MPQ captures for each backend, and the manual visual-review note are present. Add `--installed-smoke-app <installed HiveWE.app>` when auditing on the macOS machine that still has the installed bundle; otherwise the auditor verifies identity by matching the full-smoke executable fingerprint to the Debug package report. Use `--require-source-head <reviewed sha>` instead of `--require-current-source` when auditing archived evidence for a specific reviewed commit after more commits have landed.

## Remaining PR Risks

- Full visual parity still needs OpenGL reference artifacts and maintainer review across terrain, cliffs, water, SD/HD models, particles, team colors, transparency, selections, and brush modes. The parity runner above now validates deterministic map-render, Metal renderer-probe, Metal preview-probe, and shared-preview frames and produces numeric PNG diff metrics plus amplified diff images for that review.
- CORN/PopcornFX rendering is a Metal fallback billboard path, not a full `.pkfx` runtime; exact PopcornFX simulation remains outside the current renderer implementation and needs reference-content review before claiming game-perfect behavior.
- `ParticleEmitter1` model-spawn emitters now render child model instances through the shared Metal model path, preserve an orthonormal emitter basis captured at spawn time, sample child animations by particle age, evaluate out-of-order child node parents before children, replay nested emitters through animated child poses, and cover bounded nested frame/resource recursion. Exact Warcraft behavior for unusual real nested content still needs OpenGL/reference-content visual review.
- The macOS OpenGL fallback is a compile-preservation gate only on this host; runtime parity capture should use the existing Windows/Linux OpenGL renderer because the legacy renderer requires OpenGL 4.5 while Apple's OpenGL runtime exposes at most OpenGL 4.1.
