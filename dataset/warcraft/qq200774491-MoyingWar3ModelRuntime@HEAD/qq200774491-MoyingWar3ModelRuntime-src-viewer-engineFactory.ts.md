---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/viewer/engineFactory.ts"
ref: HEAD
---

import type { AbstractEngine } from '@babylonjs/core/Engines/abstractEngine';
import { Engine } from '@babylonjs/core/Engines/engine';
import { WebGPUEngine } from '@babylonjs/core/Engines/webgpuEngine';

export type EngineBackend = 'webgpu' | 'webgl2';

export interface EngineFactoryResult {
  engine: AbstractEngine;
  backend: EngineBackend;
  webGpuError?: unknown;
}

export async function createEngine(
  canvas: HTMLCanvasElement,
  preferWebGPU: boolean,
  antialias: boolean,
): Promise<EngineFactoryResult> {
  let webGpuError: unknown;

  if (preferWebGPU) {
    try {
      if (await WebGPUEngine.IsSupportedAsync) {
        const engine = new WebGPUEngine(canvas, {
          antialias,
          adaptToDeviceRatio: true,
          powerPreference: 'high-performance',
        });
        await engine.initAsync();
        return { engine, backend: 'webgpu' };
      }
      webGpuError = new Error('WebGPU is not supported by this browser or GPU.');
    } catch (error) {
      webGpuError = error;
    }
  }

  const engine = new Engine(canvas, antialias, {
    alpha: false,
    premultipliedAlpha: false,
    preserveDrawingBuffer: false,
    stencil: true,
    powerPreference: 'high-performance',
    disableWebGL2Support: false,
  }, true);
  if (engine.webGLVersion < 2) {
    engine.dispose();
    throw new Error('WebGL2 is required when WebGPU is unavailable.');
  }
  return { engine, backend: 'webgl2', webGpuError };
}
