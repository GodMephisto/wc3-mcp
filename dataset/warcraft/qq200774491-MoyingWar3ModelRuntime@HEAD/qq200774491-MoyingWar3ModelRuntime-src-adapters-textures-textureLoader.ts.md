---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/adapters/textures/textureLoader.ts"
ref: HEAD
---

import '@babylonjs/core/Materials/Textures/Loaders/ddsTextureLoader';
import '@babylonjs/core/Materials/Textures/Loaders/tgaTextureLoader';
import type { BaseTexture } from '@babylonjs/core/Materials/Textures/baseTexture';
import { RawTexture } from '@babylonjs/core/Materials/Textures/rawTexture';
import { Texture } from '@babylonjs/core/Materials/Textures/texture';
import type { Scene } from '@babylonjs/core/scene';
import { decode, getImageData } from '../../parsers/blp/decode';
import type { ResourceLoader } from '../../resource/resourceLoader';
import type { War3ResourceKind } from '../../types';

export interface TextureLoadRequest {
  kind: Extract<War3ResourceKind, 'texture' | 'particle-texture'>;
  requestedPath: string;
  candidatePaths: readonly string[];
  modelPath: string;
  signal: AbortSignal;
  wrapMode?: number;
}

export class War3TextureLoader {
  private readonly cache = new Map<string, Promise<BaseTexture>>();
  private readonly textures = new Set<BaseTexture>();
  private disposed = false;

  constructor(
    private readonly scene: Scene,
    private readonly resources: ResourceLoader,
  ) {}

  load(request: TextureLoadRequest): Promise<BaseTexture> {
    if (this.disposed) {
      return Promise.reject(new DOMException('Texture loader is disposed.', 'AbortError'));
    }
    const key = textureKey(request);
    const existing = this.cache.get(key);
    if (existing) return existing;

    const pending = this.loadUncached(request).catch((error) => {
      this.cache.delete(key);
      throw error;
    });
    this.cache.set(key, pending);
    return pending;
  }

  dispose(): void {
    if (this.disposed) return;
    this.disposed = true;
    for (const texture of this.textures) texture.dispose();
    this.textures.clear();
    this.cache.clear();
  }

  private async loadUncached(request: TextureLoadRequest): Promise<BaseTexture> {
    const resource = await this.resources.load({
      kind: request.kind,
      requestedPath: request.requestedPath,
      candidatePaths: request.candidatePaths,
      modelPath: request.modelPath,
      signal: request.signal,
    });
    if (request.signal.aborted) throw request.signal.reason;

    const bytes = toUint8Array(resource.bytes);
    const texture = isBlp(resource.resolvedPath, bytes)
      ? createBlpTexture(this.scene, bytes, resource.resolvedPath)
      : await createStandardTexture(
          this.scene,
          bytes,
          resource.resolvedPath,
          resource.mime,
        );

    applyWrapMode(texture, request.wrapMode ?? 0);
    if (this.disposed || request.signal.aborted) {
      texture.dispose();
      throw request.signal.reason ?? new DOMException('Texture load was aborted.', 'AbortError');
    }
    this.textures.add(texture);
    return texture;
  }
}

function createBlpTexture(scene: Scene, bytes: Uint8Array, path: string): RawTexture {
  const blp = decode(toArrayBuffer(bytes));
  const image = getImageData(blp, 0);
  const texture = RawTexture.CreateRGBATexture(
    image.data,
    image.width,
    image.height,
    scene,
    true,
    false,
    Texture.TRILINEAR_SAMPLINGMODE,
  );
  texture.name = path;
  texture.gammaSpace = true;
  texture.hasAlpha = true;
  texture.anisotropicFilteringLevel = 16;
  return texture;
}

function createStandardTexture(
  scene: Scene,
  bytes: Uint8Array,
  path: string,
  mime?: string,
): Promise<Texture> {
  return new Promise<Texture>((resolve, reject) => {
    let texture: Texture;
    texture = new Texture(path, scene, {
      noMipmap: false,
      invertY: false,
      samplingMode: Texture.TRILINEAR_SAMPLINGMODE,
      buffer: bytes,
      mimeType: mime ?? mimeFromPath(path),
      forcedExtension: forcedExtension(path),
      onLoad: () => resolve(texture),
      onError: (message, exception) => {
        texture?.dispose();
        reject(exception instanceof Error ? exception : new Error(message || `Failed to decode texture: ${path}`));
      },
    });
    texture.name = path;
    texture.hasAlpha = /\.(?:png|tga)$/i.test(path);
    texture.anisotropicFilteringLevel = 16;
  });
}

function textureKey(request: TextureLoadRequest): string {
  return [
    request.kind,
    request.modelPath.toLowerCase(),
    request.wrapMode ?? 0,
    ...request.candidatePaths.map((path) => path.replace(/\\/g, '/').toLowerCase()),
  ].join('\0');
}

function isBlp(path: string, bytes: Uint8Array): boolean {
  return /\.blp$/i.test(path) || (
    bytes[0] === 0x42 && bytes[1] === 0x4c && bytes[2] === 0x50
  );
}

function forcedExtension(path: string): string | undefined {
  const extension = path.match(/\.(dds|tga)$/i)?.[0];
  return extension?.toLowerCase();
}

function mimeFromPath(path: string): string | undefined {
  if (/\.png$/i.test(path)) return 'image/png';
  if (/\.jpe?g$/i.test(path)) return 'image/jpeg';
  if (/\.tga$/i.test(path)) return 'image/x-tga';
  if (/\.dds$/i.test(path)) return 'image/vnd-ms.dds';
  return undefined;
}

function applyWrapMode(texture: BaseTexture, wrapMode: number): void {
  texture.wrapU = (wrapMode & 1) !== 0 ? Texture.WRAP_ADDRESSMODE : Texture.CLAMP_ADDRESSMODE;
  texture.wrapV = (wrapMode & 2) !== 0 ? Texture.WRAP_ADDRESSMODE : Texture.CLAMP_ADDRESSMODE;
}

function toUint8Array(bytes: ArrayBuffer | Uint8Array): Uint8Array {
  return bytes instanceof Uint8Array ? bytes : new Uint8Array(bytes);
}

function toArrayBuffer(bytes: Uint8Array): ArrayBuffer {
  return bytes.buffer.slice(bytes.byteOffset, bytes.byteOffset + bytes.byteLength) as ArrayBuffer;
}
