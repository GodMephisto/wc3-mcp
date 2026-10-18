---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/resource/resourceLoader.ts"
ref: HEAD
---

import type {
  ResolvedResource,
  ResolveResource,
  War3ResourceKind,
} from '../types';
import { abortError, ConcurrencyQueue } from './concurrencyQueue';

export interface ResourceLoaderOptions {
  resolveResource: ResolveResource;
  maxConcurrentRequests: number;
  timeoutMs: number;
}

export interface ResourceLoadRequest {
  kind: War3ResourceKind;
  requestedPath: string;
  candidatePaths?: readonly string[];
  modelPath: string;
  signal: AbortSignal;
  useCache?: boolean;
}

export class ResourceNotFoundError extends Error {
  constructor(readonly requestedPath: string) {
    super(`Resource not found: ${requestedPath}`);
    this.name = 'ResourceNotFoundError';
  }
}

export class ResourceLoader {
  private readonly queue: ConcurrencyQueue;
  private readonly cache = new Map<string, ResolvedResource>();
  private readonly pending = new Map<string, Promise<ResolvedResource>>();
  private readonly activeControllers = new Set<AbortController>();
  private disposed = false;
  private _requestCount = 0;

  constructor(private readonly options: ResourceLoaderOptions) {
    this.queue = new ConcurrencyQueue(options.maxConcurrentRequests);
  }

  get requestCount(): number {
    return this._requestCount;
  }

  load(request: ResourceLoadRequest): Promise<ResolvedResource> {
    if (this.disposed) return Promise.reject(abortError('Resource loader is disposed.'));
    if (request.signal.aborted) {
      return Promise.reject(request.signal.reason ?? abortError());
    }

    const candidatePaths = uniquePaths(request.candidatePaths?.length
      ? request.candidatePaths
      : [request.requestedPath]);
    const key = cacheKey(request.kind, request.modelPath, candidatePaths);
    const cached = request.useCache === false ? undefined : this.cache.get(key);
    if (cached) return Promise.resolve(cached);

    const existing = this.pending.get(key);
    if (existing) return waitForSignal(existing, request.signal);

    const pending = this.queue.schedule(
      () => this.resolve({ ...request, candidatePaths }),
      request.signal,
    ).then((resource) => {
      if (request.useCache !== false) this.cache.set(key, resource);
      return resource;
    }).finally(() => {
      this.pending.delete(key);
    });

    this.pending.set(key, pending);
    return pending;
  }

  clear(): void {
    this.cache.clear();
  }

  dispose(): void {
    if (this.disposed) return;
    this.disposed = true;
    const reason = abortError('Resource loader is disposed.');
    for (const controller of this.activeControllers) controller.abort(reason);
    this.activeControllers.clear();
    this.queue.dispose(reason);
    this.cache.clear();
    this.pending.clear();
  }

  private async resolve(
    request: ResourceLoadRequest & { candidatePaths: readonly string[] },
  ): Promise<ResolvedResource> {
    const controller = new AbortController();
    this.activeControllers.add(controller);
    const forwardAbort = () => controller.abort(request.signal.reason ?? abortError());
    request.signal.addEventListener('abort', forwardAbort, { once: true });
    const timeoutId = this.options.timeoutMs > 0
      ? setTimeout(() => controller.abort(abortError(`Resource request timed out after ${this.options.timeoutMs}ms.`)), this.options.timeoutMs)
      : undefined;

    try {
      this._requestCount += 1;
      const resource = await waitForSignal(this.options.resolveResource({
        kind: request.kind,
        requestedPath: request.requestedPath,
        candidatePaths: request.candidatePaths,
        modelPath: request.modelPath,
        signal: controller.signal,
      }), controller.signal);
      if (!resource) throw new ResourceNotFoundError(request.requestedPath);
      if (!resource.resolvedPath) {
        throw new Error(`Resolver returned an empty resolvedPath for "${request.requestedPath}".`);
      }
      return resource;
    } finally {
      if (timeoutId !== undefined) clearTimeout(timeoutId);
      request.signal.removeEventListener('abort', forwardAbort);
      this.activeControllers.delete(controller);
    }
  }
}

function uniquePaths(paths: readonly string[]): string[] {
  const seen = new Set<string>();
  const result: string[] = [];
  for (const path of paths) {
    const normalized = path.trim().replace(/\\/g, '/');
    const key = normalized.toLowerCase();
    if (!normalized || seen.has(key)) continue;
    seen.add(key);
    result.push(normalized);
  }
  return result;
}

function cacheKey(
  kind: War3ResourceKind,
  modelPath: string,
  candidates: readonly string[],
): string {
  const group = kind === 'particle-texture' ? 'texture' : kind;
  return `${group}\0${modelPath.toLowerCase()}\0${candidates.map((path) => path.toLowerCase()).join('\0')}`;
}

function waitForSignal<T>(promise: Promise<T>, signal: AbortSignal): Promise<T> {
  if (signal.aborted) return Promise.reject(signal.reason ?? abortError());
  return new Promise<T>((resolve, reject) => {
    const onAbort = () => reject(signal.reason ?? abortError());
    signal.addEventListener('abort', onAbort, { once: true });
    void promise.then(resolve, reject).finally(() => signal.removeEventListener('abort', onAbort));
  });
}
