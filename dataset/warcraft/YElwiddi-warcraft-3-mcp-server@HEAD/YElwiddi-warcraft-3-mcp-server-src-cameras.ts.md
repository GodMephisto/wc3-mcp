---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "src/cameras.ts"
ref: HEAD
---

/**
 * war3map.w3c — predefined cameras (JASS `gg_cam_<Name>`). Round-trips byte-for-byte
 * through wc3maptranslator.
 */
import { createRequire } from 'node:module';
import { toArrayBuffer } from './map.js';
import type { War3Map } from './map.js';

const require = createRequire(import.meta.url);
const { CamerasTranslator } = require('wc3maptranslator') as {
  CamerasTranslator: typeof import('wc3maptranslator').CamerasTranslator;
};

export interface Camera {
  target: { x: number; y: number };
  offsetZ: number;
  rotation: number;
  aoa: number; // angle of attack
  distance: number;
  roll: number;
  fov: number; // field of view
  farClipping: number;
  nearClipping: number;
  localPitch: number;
  localYaw: number;
  localRoll: number;
  name: string;
}

function read(map: War3Map): Camera[] {
  const file = map.get('war3map.w3c');
  if (!file) return [];
  return CamerasTranslator.warToJson(Buffer.from(file.bytes())).json as unknown as Camera[];
}

function write(map: War3Map, cameras: Camera[]): void {
  const { buffer } = CamerasTranslator.jsonToWar(cameras as never);
  map.set('war3map.w3c', toArrayBuffer(buffer));
}

export function readCameras(map: War3Map): Camera[] {
  return read(map);
}

const DEFAULT_CAMERA: Omit<Camera, 'target' | 'name'> = {
  offsetZ: 0,
  rotation: 90,
  aoa: 304, // World Editor default angle of attack
  distance: 1650,
  roll: 0,
  fov: 70,
  farClipping: 5000,
  nearClipping: 16,
  localPitch: 0,
  localYaw: 0,
  localRoll: 0,
};

export interface NewCamera extends Partial<Camera> {
  x: number;
  y: number;
  name?: string;
}

/** Append a camera; unspecified fields take World Editor defaults. Caller saves. */
export function addCamera(map: War3Map, c: NewCamera): Camera {
  const cameras = read(map);
  const camera: Camera = {
    ...DEFAULT_CAMERA,
    ...c,
    target: { x: c.x, y: c.y },
    name: c.name ?? `Camera_${String(cameras.length + 1).padStart(3, '0')}`,
  };
  cameras.push(camera);
  write(map, cameras);
  return camera;
}

export function deleteCameras(map: War3Map, sel: { name?: string; index?: number }): Camera[] {
  if (sel.name === undefined && sel.index === undefined) {
    throw new Error('Camera selector must specify name or index');
  }
  const cameras = read(map);
  const removed: Camera[] = [];
  const kept = cameras.filter((c, i) => {
    const hit = (sel.name !== undefined && c.name === sel.name) || (sel.index !== undefined && i === sel.index);
    if (hit) removed.push(c);
    return !hit;
  });
  if (removed.length > 0) write(map, kept);
  return removed;
}
