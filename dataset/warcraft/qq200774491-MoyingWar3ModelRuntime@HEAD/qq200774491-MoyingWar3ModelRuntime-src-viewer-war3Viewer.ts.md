---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/viewer/war3Viewer.ts"
ref: HEAD
---

import type { AbstractEngine } from '@babylonjs/core/Engines/abstractEngine';
import { VertexBuffer } from '@babylonjs/core/Buffers/buffer';
import { ArcRotateCamera } from '@babylonjs/core/Cameras/arcRotateCamera';
import { HemisphericLight } from '@babylonjs/core/Lights/hemisphericLight';
import { Color3, Color4 } from '@babylonjs/core/Maths/math.color';
import { Vector3 } from '@babylonjs/core/Maths/math.vector';
import { Scene } from '@babylonjs/core/scene';
import { ModelAdapter, type BabylonMdxModel } from '../adapters/mdx/modelAdapter';
import { War3TextureLoader } from '../adapters/textures/textureLoader';
import { detectWar3ModelFormat, parseWar3Model } from '../parser';
import type Model from '../parsers/mdlx/model';
import { Flags as LayerFlags } from '../parsers/mdlx/layer';
import { ResourceLoader } from '../resource/resourceLoader';
import type {
  LoadModelRequest,
  PlaySequenceOptions,
  ResolvedResource,
  War3Bounds,
  War3LoadResult,
  War3ModelFormat,
  War3Sequence,
  War3Viewer,
  War3ViewerOptions,
  War3ViewerWarning,
} from '../types';
import { createEngine, type EngineBackend } from './engineFactory';

const DEFAULT_BACKGROUND = [0.07, 0.08, 0.1, 1] as const;
const DEFAULT_TEAM_COLOR = 0;
// Matches the reference renderer's War3 yaw=-90°, pitch=30° after converting
// its Z-up coordinates to Babylon's Y-up scene.
const DEFAULT_CAMERA_ALPHA = Math.PI;
const DEFAULT_CAMERA_BETA = Math.PI / 3;
const TEAM_COLORS: readonly (readonly [number, number, number])[] = [
  [0.8, 0.05, 0.05], [0.1, 0.25, 0.95], [0.1, 0.75, 0.75], [0.45, 0.1, 0.65],
  [0.95, 0.9, 0.1], [0.95, 0.45, 0.05], [0.1, 0.7, 0.2], [0.95, 0.4, 0.65],
  [0.55, 0.55, 0.55], [0.45, 0.75, 0.95], [0.1, 0.35, 0.1], [0.45, 0.25, 0.1],
  [0.45, 0.1, 0.1], [0.05, 0.1, 0.45], [0.05, 0.4, 0.4], [0.25, 0.05, 0.35],
  [0.5, 0.45, 0.05], [0.5, 0.2, 0.05], [0.05, 0.35, 0.1], [0.5, 0.2, 0.35],
  [0.25, 0.25, 0.25], [0.2, 0.4, 0.55], [0.05, 0.2, 0.05], [0.25, 0.15, 0.05],
];

export async function createViewer(
  canvas: HTMLCanvasElement,
  options: War3ViewerOptions,
): Promise<War3Viewer> {
  if (!canvas) throw new TypeError('A canvas element is required.');
  if (typeof options?.resolveResource !== 'function') {
    throw new TypeError('options.resolveResource must be a function.');
  }

  const engineResult = await createEngine(
    canvas,
    options.preferWebGPU !== false,
    options.antialias !== false,
  );
  return new BabylonWar3Viewer(canvas, options, engineResult.engine, engineResult.backend, engineResult.webGpuError);
}

export class BabylonWar3Viewer implements War3Viewer {
  private readonly scene: Scene;
  private readonly camera: ArcRotateCamera;
  private readonly resources: ResourceLoader;
  private readonly textures: War3TextureLoader;
  private readonly engineWarnings: War3ViewerWarning[] = [];
  private model: BabylonMdxModel | null = null;
  private adapter: ModelAdapter | null = null;
  private loadController: AbortController | null = null;
  private loadGeneration = 0;
  private sequences: War3Sequence[] = [];
  private disposed = false;
  private playbackSpeed = 1;
  private activeSequence = -1;
  private cameraTarget = Vector3.Zero();
  private cameraRadius = 100;
  private teamColor: number | readonly [number, number, number] = DEFAULT_TEAM_COLOR;

  constructor(
    private readonly canvas: HTMLCanvasElement,
    private readonly options: War3ViewerOptions,
    private readonly engine: AbstractEngine,
    readonly backend: EngineBackend,
    webGpuError?: unknown,
  ) {
    this.scene = new Scene(engine);
    const background = options.backgroundColor ?? DEFAULT_BACKGROUND;
    this.scene.clearColor = new Color4(background[0], background[1], background[2], background[3]);
    this.camera = new ArcRotateCamera(
      'War3Camera',
      DEFAULT_CAMERA_ALPHA,
      DEFAULT_CAMERA_BETA,
      100,
      Vector3.Zero(),
      this.scene,
    );
    this.camera.minZ = 0.05;
    this.camera.wheelPrecision = 20;
    this.camera.panningSensibility = 100;
    if (typeof window !== 'undefined') this.camera.attachControl(canvas, true);
    const light = new HemisphericLight('War3Light', new Vector3(0.25, 1, -0.25), this.scene);
    light.intensity = 1.15;

    this.resources = new ResourceLoader({
      resolveResource: options.resolveResource,
      maxConcurrentRequests: clampInteger(options.maxConcurrentRequests ?? 6, 1, 16),
      timeoutMs: Math.max(0, options.resourceTimeoutMs ?? 30_000),
    });
    this.textures = new War3TextureLoader(this.scene, this.resources);

    if (backend === 'webgl2' && webGpuError) {
      this.emitWarning(this.engineWarnings, {
        code: 'engine-fallback',
        message: 'WebGPU initialization failed or was unavailable; using WebGL2.',
        cause: webGpuError,
      });
    }
    this.engine.runRenderLoop(() => {
      if (!this.disposed) this.scene.render();
    });
  }

  async loadModel(request: LoadModelRequest): Promise<War3LoadResult> {
    this.assertActive();
    if (!request.path?.trim()) throw new TypeError('loadModel requires a non-empty path.');

    const startedAt = now();
    const generation = ++this.loadGeneration;
    this.loadController?.abort(new DOMException('A newer model load started.', 'AbortError'));
    this.loadController = new AbortController();
    const signal = this.loadController.signal;
    this.disposeModel();

    const warnings = [...this.engineWarnings];
    const requestCountAtStart = this.resources.requestCount;
    let modelDownloadMs = 0;
    let source: ResolvedResource;

    if (request.bytes) {
      source = { bytes: request.bytes, resolvedPath: request.path, mime: request.mime };
    } else {
      const downloadStarted = now();
      source = await this.resources.load({
        kind: 'model',
        requestedPath: request.path,
        candidatePaths: [request.path],
        modelPath: request.path,
        signal,
        useCache: false,
      });
      modelDownloadMs = now() - downloadStarted;
    }
    this.assertCurrent(generation, signal);

    const parseStarted = now();
    let format: War3ModelFormat;
    try {
      format = detectWar3ModelFormat(source.resolvedPath || request.path, source.bytes, source.mime ?? request.mime);
    } catch (error) {
      if (!request.format) throw error;
      format = request.format;
    }
    const parsed = parseWar3Model(
      format === 'mdl' ? forceExtension(request.path, 'mdl') : forceExtension(request.path, 'mdx'),
      source.bytes,
      source.mime ?? request.mime,
    );
    const parseMs = now() - parseStarted;
    this.assertCurrent(generation, signal);

    const geometryStarted = now();
    const adapter = new ModelAdapter(this.scene, this.textures);
    const model = await adapter.loadModel(parsed.model, {
      name: parsed.model.name || basename(request.path),
      modelPath: request.path,
      // Many third-party MDX exporters omit TwoSided on meshes that require it.
      // Depth writes keep rear geometry hidden while compatibility mode is active.
      backFaceCulling: this.options.backFaceCulling ?? false,
      signal,
      onWarning: (warning) => this.emitWarning(warnings, warning),
    });
    this.assertCurrent(generation, signal);
    model.rotation.x = -Math.PI / 2;
    model.computeWorldMatrix(true);
    this.model = model;
    this.adapter = adapter;
    const bounds = this.computeBounds(model, parsed.model, warnings);
    this.frameCamera(bounds);
    this.sequences = toSequences(parsed.model);
    const geometryMs = now() - geometryStarted;

    if (request.autoPlay !== false && this.sequences.length) this.playSequence(0);
    this.applyTeamColor(this.teamColor);
    this.scene.render();
    const firstFrameMs = now() - startedAt;
    this.options.onFirstFrame?.({ modelPath: request.path, format, elapsedMs: firstFrameMs });

    await model.resourcesReady;
    this.assertCurrent(generation, signal);
    this.applyTeamColor(this.teamColor);
    const allResourcesMs = now() - startedAt;

    return {
      format,
      sequences: [...this.sequences],
      bounds,
      warnings: [...warnings],
      timings: {
        modelDownloadMs,
        parseMs,
        geometryMs,
        firstFrameMs,
        allResourcesMs,
        requestCount: this.resources.requestCount - requestCountAtStart,
      },
    };
  }

  listSequences(): readonly War3Sequence[] {
    return [...this.sequences];
  }

  playSequence(index: number, options: PlaySequenceOptions = {}): void {
    this.assertActive();
    if (!Number.isInteger(index) || index < 0 || index >= this.sequences.length) {
      throw new RangeError(`Sequence index ${index} is out of range.`);
    }
    this.activeSequence = index;
    this.model?.playSequence?.(index, options.loop ?? this.sequences[index]?.looping ?? true, this.playbackSpeed);
  }

  pause(): void {
    this.assertActive();
    this.model?.activeAnimation?.pause();
    for (const animation of this.model?.globalAnimations ?? []) animation.pause();
    this.model?.setParticleSpawnEnabled?.(false);
  }

  setPlaybackSpeed(speed: number): void {
    this.assertActive();
    if (!Number.isFinite(speed) || speed <= 0) throw new RangeError('Playback speed must be greater than zero.');
    this.playbackSpeed = speed;
    if (this.model?.activeAnimation) this.model.activeAnimation.speedRatio = speed;
    for (const animation of this.model?.globalAnimations ?? []) animation.speedRatio = speed;
  }

  setTeamColor(color: number | readonly [number, number, number]): void {
    this.assertActive();
    this.teamColor = color;
    this.applyTeamColor(color);
  }

  resetCamera(): void {
    this.assertActive();
    // ArcRotateCamera.setTarget() rebuilds alpha, beta, and radius from the old
    // position, so the authored view must be applied after changing the target.
    this.camera.setTarget(this.cameraTarget);
    this.camera.alpha = DEFAULT_CAMERA_ALPHA;
    this.camera.beta = DEFAULT_CAMERA_BETA;
    this.camera.radius = this.cameraRadius;
    this.camera.inertialAlphaOffset = 0;
    this.camera.inertialBetaOffset = 0;
    this.camera.inertialRadiusOffset = 0;
  }

  resize(): void {
    this.assertActive();
    this.engine.resize();
  }

  dispose(): void {
    if (this.disposed) return;
    this.disposed = true;
    this.loadController?.abort(new DOMException('Viewer disposed.', 'AbortError'));
    this.loadController = null;
    this.engine.stopRenderLoop();
    this.disposeModel();
    this.textures.dispose();
    this.resources.dispose();
    this.scene.dispose();
    this.engine.dispose();
  }

  private computeBounds(
    model: BabylonMdxModel,
    parsed: Model,
    warnings: War3ViewerWarning[],
  ): War3Bounds {
    for (const mesh of model.meshes) {
      mesh.computeWorldMatrix(true);
      mesh.refreshBoundingInfo({ applySkeleton: true });
    }
    const hierarchy = model.getHierarchyBoundingVectors(true);
    let minimum = hierarchy.min;
    let maximum = hierarchy.max;
    let usedFallback = false;

    if (!validBounds(minimum, maximum)) {
      const raw = rawMeshBounds(model);
      if (raw) {
        minimum = raw.minimum;
        maximum = raw.maximum;
      } else {
        const extent = parserExtentBounds(parsed);
        minimum = extent.minimum;
        maximum = extent.maximum;
      }
      usedFallback = true;
      this.emitWarning(warnings, {
        code: 'invalid-bounds',
        message: 'Mesh bounds were invalid; the camera used a geometry/parser fallback.',
      });
    }

    const center = minimum.add(maximum).scale(0.5);
    const radius = Math.max(Vector3.Distance(minimum, maximum) * 0.5, 0.5);
    return {
      minimum: [minimum.x, minimum.y, minimum.z],
      maximum: [maximum.x, maximum.y, maximum.z],
      center: [center.x, center.y, center.z],
      radius,
      usedFallback,
    };
  }

  private frameCamera(bounds: War3Bounds): void {
    this.cameraTarget = new Vector3(bounds.center[0], bounds.center[1], bounds.center[2]);
    this.cameraRadius = Math.max(bounds.radius * 2.6, 2);
    this.camera.lowerRadiusLimit = Math.max(bounds.radius * 0.08, 0.1);
    this.camera.upperRadiusLimit = Math.max(bounds.radius * 25, this.cameraRadius * 2);
    this.camera.maxZ = Math.max(bounds.radius * 100, 1000);
    this.resetCamera();
  }

  private applyTeamColor(value: number | readonly [number, number, number]): void {
    if (!this.model?.materialLayers) return;
    const color = toTeamColor(value);
    for (let materialIndex = 0; materialIndex < this.model.mdxModel.materials.length; materialIndex++) {
      const sourceMaterial = this.model.mdxModel.materials[materialIndex];
      const materials = this.model.materialLayers[materialIndex] ?? [];
      for (let layerIndex = 0; layerIndex < sourceMaterial.layers.length; layerIndex++) {
        const layer = sourceMaterial.layers[layerIndex];
        const texture = this.model.mdxModel.textures[layer.textureId];
        if (texture?.replaceableId !== 1 && texture?.replaceableId !== 2) continue;
        const material = materials[layerIndex];
        if (!material) continue;
        if (material.diffuseTexture) {
          material.opacityTexture = material.diffuseTexture;
          material.diffuseTexture = null;
          material.emissiveTexture = null;
        }
        if (!material.opacityTexture) {
          // A replaceable texture is the mask, not optional decoration. Without it,
          // drawing the mesh would turn team-color/glow cards into solid rectangles.
          material.disableColorWrite = true;
          material.disableDepthWrite = true;
          continue;
        }
        material.disableColorWrite = false;
        material.disableDepthWrite = (layer.flags & LayerFlags.NoDepthSet) !== 0;
        material.diffuseColor.copyFrom(color);
        material.emissiveColor.copyFrom(color);
      }
    }
  }

  private emitWarning(target: War3ViewerWarning[], warning: War3ViewerWarning): void {
    if (target.some((item) => item.code === warning.code && item.feature === warning.feature && item.requestedPath === warning.requestedPath)) {
      return;
    }
    target.push(warning);
    this.options.onWarning?.(warning);
  }

  private disposeModel(): void {
    if (this.model) {
      this.model.stopSequence?.();
      for (const animation of this.model.globalAnimations ?? []) animation.stop();
      this.model.globalAnimations = [];
      this.model.particleSystem?.dispose();
      this.model.animationRuntime?.dispose();
      this.model.billboardRuntime?.dispose();
      const layerMaterials = new Set((this.model.materialLayers ?? []).flat());
      for (const mesh of this.model.meshes) {
        if (mesh.material && !layerMaterials.has(mesh.material as never)) {
          mesh.material.dispose(false, false);
        }
        mesh.dispose(false, false);
      }
      for (const layers of this.model.materialLayers ?? []) {
        for (const material of layers) material.dispose(false, false);
      }
      this.model.skeleton?.dispose();
      this.model.dispose(false, false);
      this.model = null;
    }
    this.adapter?.dispose();
    this.adapter = null;
    this.sequences = [];
    this.activeSequence = -1;
  }

  private assertActive(): void {
    if (this.disposed) throw new DOMException('Viewer is disposed.', 'InvalidStateError');
  }

  private assertCurrent(generation: number, signal: AbortSignal): void {
    this.assertActive();
    if (signal.aborted || generation !== this.loadGeneration) {
      throw signal.reason ?? new DOMException('Model load was superseded.', 'AbortError');
    }
  }
}

function toSequences(model: Model): War3Sequence[] {
  return model.sequences.map((sequence, index) => {
    const start = sequence.interval[0] ?? 0;
    const end = sequence.interval[1] ?? start;
    return {
      index,
      name: sequence.name?.trim() || `Sequence ${index + 1}`,
      start,
      end,
      duration: Math.max(0, end - start),
      looping: sequence.nonLooping === 0,
    };
  });
}

function rawMeshBounds(model: BabylonMdxModel): { minimum: Vector3; maximum: Vector3 } | null {
  const minimum = new Vector3(Number.POSITIVE_INFINITY, Number.POSITIVE_INFINITY, Number.POSITIVE_INFINITY);
  const maximum = new Vector3(Number.NEGATIVE_INFINITY, Number.NEGATIVE_INFINITY, Number.NEGATIVE_INFINITY);
  let count = 0;
  for (const mesh of model.meshes) {
    const positions = mesh.getVerticesData(VertexBuffer.PositionKind);
    if (!positions) continue;
    const world = mesh.computeWorldMatrix(true);
    for (let index = 0; index < positions.length; index += 3) {
      const point = Vector3.TransformCoordinates(new Vector3(
        positions[index] ?? 0,
        positions[index + 1] ?? 0,
        positions[index + 2] ?? 0,
      ), world);
      minimum.minimizeInPlace(point);
      maximum.maximizeInPlace(point);
      count += 1;
    }
  }
  return count && validBounds(minimum, maximum) ? { minimum, maximum } : null;
}

function parserExtentBounds(model: Model): { minimum: Vector3; maximum: Vector3 } {
  const min = model.extent.min;
  const max = model.extent.max;
  const minimum = new Vector3(-(max[0] ?? 0), min[2] ?? 0, -(max[1] ?? 0));
  const maximum = new Vector3(-(min[0] ?? 0), max[2] ?? 0, -(min[1] ?? 0));
  if (validBounds(minimum, maximum)) return { minimum, maximum };
  return { minimum: new Vector3(-32, -32, -32), maximum: new Vector3(32, 32, 32) };
}

function validBounds(minimum: Vector3, maximum: Vector3): boolean {
  const values = [minimum.x, minimum.y, minimum.z, maximum.x, maximum.y, maximum.z];
  if (!values.every(Number.isFinite)) return false;
  const span = maximum.subtract(minimum);
  const diagonal = span.length();
  return span.x >= 0 && span.y >= 0 && span.z >= 0 && diagonal > 0.0001 && diagonal < 1e9;
}

function toTeamColor(value: number | readonly [number, number, number]): Color3 {
  if (typeof value === 'number') {
    const tuple = TEAM_COLORS[clampInteger(value, 0, TEAM_COLORS.length - 1)] ?? TEAM_COLORS[0]!;
    return Color3.FromArray(tuple);
  }
  const scale = value.some((component) => component > 1) ? 255 : 1;
  return new Color3(
    clamp(value[0] / scale, 0, 1),
    clamp(value[1] / scale, 0, 1),
    clamp(value[2] / scale, 0, 1),
  );
}

function clampInteger(value: number, minimum: number, maximum: number): number {
  return Math.round(clamp(value, minimum, maximum));
}

function clamp(value: number, minimum: number, maximum: number): number {
  return Math.min(maximum, Math.max(minimum, value));
}

function now(): number {
  return typeof performance !== 'undefined' ? performance.now() : Date.now();
}

function basename(path: string): string {
  return path.replace(/\\/g, '/').split('/').pop() || 'War3Model';
}

function forceExtension(path: string, extension: War3ModelFormat): string {
  return /\.[^.\\/]+$/.test(path) ? path.replace(/\.[^.\\/]+$/, `.${extension}`) : `${path}.${extension}`;
}
