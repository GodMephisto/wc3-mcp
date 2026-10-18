---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/index.ts"
ref: HEAD
---

import type { War3Viewer, War3ViewerOptions } from './types';
import { createViewer } from './viewer/war3Viewer';

export { detectWar3ModelFormat, parseWar3Model } from './parser';
export type { ParsedWar3Model } from './parser';
export { ResourceLoader, ResourceNotFoundError } from './resource/resourceLoader';
export type { ResourceLoaderOptions, ResourceLoadRequest } from './resource/resourceLoader';

export type {
  LoadModelRequest,
  PlaySequenceOptions,
  ResolvedResource,
  ResolveResource,
  ResolveResourceRequest,
  War3Bounds,
  War3FirstFrameEvent,
  War3LoadResult,
  War3LoadTimings,
  War3ModelFormat,
  War3ResourceKind,
  War3Sequence,
  War3Viewer,
  War3ViewerOptions,
  War3ViewerWarning,
} from './types';

export async function createWar3BabylonViewer(
  canvas: HTMLCanvasElement,
  options: War3ViewerOptions,
): Promise<War3Viewer> {
  return createViewer(canvas, options);
}
