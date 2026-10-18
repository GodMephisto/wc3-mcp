---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/types.ts"
ref: HEAD
---

export type War3ModelFormat = 'mdx' | 'mdl';

export type War3ResourceKind =
  | 'model'
  | 'texture'
  | 'particle-texture'
  | 'attachment';

export interface ResolveResourceRequest {
  kind: War3ResourceKind;
  requestedPath: string;
  candidatePaths: readonly string[];
  modelPath: string;
  signal: AbortSignal;
}

export interface ResolvedResource {
  bytes: ArrayBuffer | Uint8Array;
  resolvedPath: string;
  mime?: string;
}

export type ResolveResource = (
  request: ResolveResourceRequest,
) => Promise<ResolvedResource | undefined>;

export interface War3ViewerOptions {
  resolveResource: ResolveResource;
  preferWebGPU?: boolean;
  antialias?: boolean;
  backgroundColor?: readonly [number, number, number, number];
  /** Enable back-face culling. Defaults to false for third-party MDX compatibility. */
  backFaceCulling?: boolean;
  maxConcurrentRequests?: number;
  resourceTimeoutMs?: number;
  onWarning?: (warning: War3ViewerWarning) => void;
  onFirstFrame?: (event: War3FirstFrameEvent) => void;
}

export interface War3FirstFrameEvent {
  modelPath: string;
  format: War3ModelFormat;
  elapsedMs: number;
}

export interface LoadModelRequest {
  path: string;
  bytes?: ArrayBuffer | Uint8Array;
  format?: War3ModelFormat;
  mime?: string;
  autoPlay?: boolean;
}

export interface War3Sequence {
  index: number;
  name: string;
  start: number;
  end: number;
  duration: number;
  looping: boolean;
}

export interface War3Bounds {
  minimum: readonly [number, number, number];
  maximum: readonly [number, number, number];
  center: readonly [number, number, number];
  radius: number;
  usedFallback: boolean;
}

export interface War3ViewerWarning {
  code:
    | 'missing-resource'
    | 'unsupported-feature'
    | 'invalid-bounds'
    | 'engine-fallback'
    | 'resource-error';
  message: string;
  requestedPath?: string;
  feature?: string;
  cause?: unknown;
}

export interface War3LoadTimings {
  modelDownloadMs: number;
  parseMs: number;
  geometryMs: number;
  firstFrameMs: number;
  allResourcesMs: number;
  requestCount: number;
}

export interface War3LoadResult {
  format: War3ModelFormat;
  sequences: readonly War3Sequence[];
  bounds: War3Bounds;
  warnings: readonly War3ViewerWarning[];
  timings: War3LoadTimings;
}

export interface PlaySequenceOptions {
  loop?: boolean;
  reset?: boolean;
}

export interface War3Viewer {
  loadModel(request: LoadModelRequest): Promise<War3LoadResult>;
  listSequences(): readonly War3Sequence[];
  playSequence(index: number, options?: PlaySequenceOptions): void;
  pause(): void;
  setPlaybackSpeed(speed: number): void;
  setTeamColor(color: number | readonly [number, number, number]): void;
  resetCamera(): void;
  resize(): void;
  dispose(): void;
}
