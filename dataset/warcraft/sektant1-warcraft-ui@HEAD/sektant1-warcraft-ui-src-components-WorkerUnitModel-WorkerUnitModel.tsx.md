---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/WorkerUnitModel/WorkerUnitModel.tsx"
ref: HEAD
---

import { useEffect, useRef } from "react";
import {
  ModelRenderer,
  decodeBLP,
  getBLPImageData,
  parseMDX,
} from "war3-model";
import { mat3, mat4, quat, vec3 } from "gl-matrix";
import type { Race } from "../../utils/types";
import "./style.css";

type ModelType = ReturnType<typeof parseMDX>;
type WorkerSide = "left" | "right";

interface WorkerUnitConfig {
  modelPath: string;
  textures: Record<string, string>;
}

const WORKER_UNIT_CONFIG: Record<Race, WorkerUnitConfig> = {
  Human: {
    modelPath: "./models/worker-units/Human/peasant.mdx",
    textures: {
      "textures/peasant.blp": "./models/worker-units/textures/Peasant.blp",
      "textures/gutz.blp": "./models/worker-units/textures/gutz.blp",
    },
  },
  Orc: {
    modelPath: "./models/worker-units/Orc/peon.mdx",
    textures: {
      "units/orc/peon/peon.blp": "./models/worker-units/Orc/peon.blp",
      "textures/gutz.blp": "./models/worker-units/textures/gutz.blp",
    },
  },
  NightElf: {
    modelPath: "./models/worker-units/NightElf/Archer.mdx",
    textures: {
      "textures/ranger.blp": "./models/worker-units/textures/Ranger.blp",
      "textures/star2_32.blp": "./models/worker-units/textures/star2_32.blp",
      "textures/gutz.blp": "./models/worker-units/textures/gutz.blp",
    },
  },
  Undead: {
    modelPath: "./models/worker-units/Undead/Acolyte.mdx",
    textures: {
      "textures/acolyte.blp": "./models/worker-units/textures/Acolyte.blp",
      "textures/green_star.blp":
        "./models/worker-units/textures/Green_Star.blp",
      "textures/demonrune3.blp":
        "./models/worker-units/textures/DemonRune3.blp",
      "textures/gutz.blp": "./models/worker-units/textures/gutz.blp",
    },
  },
};

const modelCache = new Map<string, ModelType>();
const blpCache = new Map<string, ImageData[]>();

function normalizeTexturePath(textureImage: string): string {
  return textureImage.replaceAll("\\", "/").toLowerCase();
}

function getTexturePath(textureImage: string, race: Race): string | null {
  const normalized = normalizeTexturePath(textureImage);
  if (!normalized) return null;
  return WORKER_UNIT_CONFIG[race].textures[normalized] || null;
}

async function loadModel(path: string): Promise<ModelType> {
  const cached = modelCache.get(path);
  if (cached) return cached;
  const res = await fetch(path);
  if (!res.ok) throw new Error(`Failed to load worker unit model: ${path}`);
  const ab = await res.arrayBuffer();
  const model = parseMDX(ab);
  modelCache.set(path, model);
  return model;
}

async function loadBLPMips(path: string): Promise<ImageData[]> {
  const cached = blpCache.get(path);
  if (cached) return cached;
  const res = await fetch(path);
  if (!res.ok) throw new Error(`Failed to load worker unit texture: ${path}`);
  const ab = await res.arrayBuffer();
  const blp = decodeBLP(ab);
  const mips: ImageData[] = [];
  const maxLevels = Math.max(
    1,
    Math.floor(Math.log2(Math.max(blp.width, blp.height))) + 1,
  );
  for (let i = 0; i < maxLevels; i++) {
    const w = blp.width >> i;
    const h = blp.height >> i;
    if (w <= 0 || h <= 0) break;
    try {
      const img = getBLPImageData(blp, i);
      if (!img || !img.width || !img.height) break;
      mips.push(
        new ImageData(new Uint8ClampedArray(img.data), img.width, img.height),
      );
      if (img.width === 1 && img.height === 1) break;
    } catch {
      break;
    }
  }
  if (mips.length === 0)
    throw new Error(`No mip levels decoded for worker unit texture: ${path}`);
  blpCache.set(path, mips);
  return mips;
}

function findIdleSequenceIndex(model: ModelType, race: Race): number {
  const sequences = model.Sequences || [];
  if (sequences.length === 0) return 0;
  const sequenceNames = sequences.map((seq) => (seq.Name || "").toLowerCase());
  const preferencesByRace: Record<Race, readonly string[]> = {
    Human: [
      "stand work",
      "stand lumber",
      "stand ready",
      "stand gold",
      "stand - 3",
      "stand -3",
      "stand 2",
      "stand - 2",
      "stand",
    ],
    Orc: [
      "attack lumber",
      "stand - stretch",
      "stand - itch head",
      "stand - talk gesture",
      "stand 2",
      "stand - 2",
      "stand",
    ],
    NightElf: [
      "attack",
      "stand - 3",
      "stand -3",
      "stand 2",
      "stand - 2",
      "stand",
    ],
    Undead: ["stand work", "stand 2", "stand - 2", "stand ready", "stand"],
  };
  for (const pref of preferencesByRace[race]) {
    const idx = sequenceNames.findIndex((name) => name.includes(pref));
    if (idx >= 0) return idx;
  }
  const fallback = sequenceNames.findIndex(
    (name) =>
      name.includes("stand") &&
      !name.includes("death") &&
      !name.includes("decay"),
  );
  return fallback >= 0 ? fallback : 0;
}

function getSequenceInterval(
  model: ModelType,
  index: number,
): { start: number; length: number } {
  const seq = model.Sequences?.[index];
  const start = seq?.Interval?.[0] ?? 0;
  const end = seq?.Interval?.[1] ?? start + 1000;
  return { start, length: Math.max(1, end - start) };
}

function getExtentsForSequence(
  model: ModelType,
  sequenceIndex: number,
): {
  min: readonly [number, number, number] | null;
  max: readonly [number, number, number] | null;
} {
  const seq = model.Sequences?.[sequenceIndex];
  const seqMin = seq?.MinimumExtent;
  const seqMax = seq?.MaximumExtent;
  if (seqMin && seqMax)
    return {
      min: [seqMin[0], seqMin[1], seqMin[2]],
      max: [seqMax[0], seqMax[1], seqMax[2]],
    };
  const modelMin = model.Info?.MinimumExtent;
  const modelMax = model.Info?.MaximumExtent;
  if (modelMin && modelMax)
    return {
      min: [modelMin[0], modelMin[1], modelMin[2]],
      max: [modelMax[0], modelMax[1], modelMax[2]],
    };
  return { min: null, max: null };
}

interface WorkerUnitModelProps {
  race: Race;
  side: WorkerSide;
}

export default function WorkerUnitModel(props: WorkerUnitModelProps) {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const stateRef = useRef({
    gl: null as WebGLRenderingContext | WebGL2RenderingContext | null,
    modelRenderer: null as ModelRenderer | null,
    resizeObserver: null as ResizeObserver | null,
    rafId: 0,
    disposed: false,
    initVersion: 0,
    sequenceIndex: 0,
    sequenceStart: 0,
    sequenceLength: 1000,
    animStartMs: 0,
    mvMatrix: mat4.create(),
    pMatrix: mat4.create(),
    worldCameraMatrix: mat4.create(),
    cameraPos: vec3.create(),
    cameraTarget: vec3.create(),
    cameraQuat: quat.create(),
    cameraFov: 0.78,
    cameraNear: 8,
    cameraFar: 2400,
  });

  useEffect(() => {
    const s = stateRef.current;
    s.disposed = false;
    const canvas = canvasRef.current!;

    const destroyRenderer = () => {
      if (!s.modelRenderer) return;
      try {
        s.modelRenderer.destroy();
      } catch (err) {
        console.warn("worker unit renderer destroy failed", err);
      } finally {
        s.modelRenderer = null;
      }
    };

    const updateProjection = () => {
      const aspect = Math.max(
        1e-6,
        canvas.clientWidth / Math.max(1, canvas.clientHeight),
      );
      mat4.perspective(
        s.pMatrix,
        s.cameraFov,
        aspect,
        s.cameraNear,
        s.cameraFar,
      );
    };

    const resizeCanvas = () => {
      if (!s.gl) return;
      const dpr = window.devicePixelRatio || 1;
      const w = Math.max(1, Math.round(canvas.clientWidth * dpr));
      const h = Math.max(1, Math.round(canvas.clientHeight * dpr));
      if (canvas.width !== w || canvas.height !== h) {
        canvas.width = w;
        canvas.height = h;
      }
      s.gl.viewport(0, 0, w, h);
      updateProjection();
    };

    // Per-race camera tweaks: targetZ shifts the look-at point vertically,
    // distance multiplies the auto-computed pull-back distance.
    const CAMERA_TWEAKS: Record<Race, { targetZ: number; dist: number }> = {
      Human: { targetZ: 0, dist: 1.0 },
      Orc: { targetZ: 0, dist: 1.0 },
      NightElf: { targetZ: 10, dist: 1.0 },
      Undead: { targetZ: 20, dist: 0.85 },
    };

    const configureCamera = (
      model: ModelType,
      sequenceIdx: number,
      race: Race,
    ) => {
      const fov = 0.75;
      s.cameraFov = fov;

      const extents = getExtentsForSequence(model, sequenceIdx);
      const extMin = extents.min;
      const extMax = extents.max;

      // Center and size of the bounding box
      let cx = 0,
        cy = 0,
        cz = 60;
      let bboxSize = 200;

      if (extMin && extMax) {
        cx = (extMin[0] + extMax[0]) * 0.5;
        cy = (extMin[1] + extMax[1]) * 0.5;
        cz = (extMin[2] + extMax[2]) * 0.5;
        const dx = extMax[0] - extMin[0];
        const dy = extMax[1] - extMin[1];
        const dz = extMax[2] - extMin[2];
        // Use the largest dimension to determine framing distance
        bboxSize = Math.max(dx, dy, dz, 80);
      }

      const tweak = CAMERA_TWEAKS[race];

      // Target: center of model, with per-race vertical adjustment
      const targetZ = cz + tweak.targetZ;
      vec3.set(s.cameraTarget, cx, cy, targetZ);

      // Camera distance: pull back enough to fit the bounding box in the FOV
      // 1.4 padding factor ensures no part of the model is clipped
      const dist = (bboxSize / (2 * Math.tan(fov / 2))) * 1.7 * tweak.dist;

      // Position: slightly to the right, straight back, same height as target
      vec3.set(
        s.cameraPos,
        cx + dist * 0.08,
        cy - dist,
        targetZ + bboxSize * 0.05,
      );

      s.cameraNear = Math.max(1, dist * 0.01);
      s.cameraFar = dist * 5;

      mat4.lookAt(
        s.mvMatrix,
        s.cameraPos,
        s.cameraTarget,
        vec3.fromValues(0, 0, 1),
      );
      if (mat4.invert(s.worldCameraMatrix, s.mvMatrix)) {
        const rot = mat3.create();
        mat3.fromMat4(rot, s.worldCameraMatrix);
        quat.fromMat3(s.cameraQuat, rot);
        quat.normalize(s.cameraQuat, s.cameraQuat);
      } else {
        quat.identity(s.cameraQuat);
      }
    };

    const startRenderLoop = () => {
      const speed = 0.8;
      const tick = (now: number) => {
        if (s.disposed) return;
        s.rafId = requestAnimationFrame(tick);
        if (!s.gl || !s.modelRenderer) return;
        resizeCanvas();
        const elapsed = (now - s.animStartMs) * speed;
        const frame = s.sequenceStart + (elapsed % s.sequenceLength);
        s.modelRenderer.setSequence(s.sequenceIndex);
        s.modelRenderer.setFrame(frame);
        s.modelRenderer.update(0);
        s.gl.clearColor(0, 0, 0, 0);
        s.gl.clear(s.gl.COLOR_BUFFER_BIT | s.gl.DEPTH_BUFFER_BIT);
        s.modelRenderer.render(s.mvMatrix, s.pMatrix, {
          env: false,
          wireframe: false,
        });
      };
      s.rafId = requestAnimationFrame(tick);
    };

    const initForRace = async (race: Race, _side: WorkerSide) => {
      const version = ++s.initVersion;
      const cfg = WORKER_UNIT_CONFIG[race];
      const model = await loadModel(cfg.modelPath);
      if (s.disposed || version !== s.initVersion || !s.gl) return;
      destroyRenderer();
      s.modelRenderer = new ModelRenderer(model);
      s.modelRenderer.initGL(s.gl);
      s.sequenceIndex = findIdleSequenceIndex(model, race);
      const interval = getSequenceInterval(model, s.sequenceIndex);
      s.sequenceStart = interval.start;
      s.sequenceLength = interval.length;
      configureCamera(model, s.sequenceIndex, race);
      s.modelRenderer.setCamera(s.cameraPos, s.cameraQuat);
      s.modelRenderer.setLightPosition(s.cameraPos);
      s.modelRenderer.setLightColor(vec3.fromValues(1, 1, 1));
      s.modelRenderer.setSequence(s.sequenceIndex);
      s.animStartMs = performance.now();
      const texturePromises = (model.Textures || []).map(async (tex) => {
        const texturePath = getTexturePath(tex.Image, race);
        if (!texturePath) return;
        try {
          const mips = await loadBLPMips(texturePath);
          if (!s.disposed && version === s.initVersion)
            s.modelRenderer!.setTextureImageData(tex.Image, mips);
        } catch (err) {
          console.warn(`failed to load worker texture ${texturePath}`, err);
        }
      });
      await Promise.all(texturePromises);
    };

    (s as any).initForRace = initForRace;

    s.gl = canvas.getContext("webgl2", {
      alpha: true,
      premultipliedAlpha: false,
      antialias: true,
      depth: true,
      stencil: false,
    }) as WebGL2RenderingContext | null;
    if (!s.gl)
      s.gl = canvas.getContext("webgl", {
        alpha: true,
        premultipliedAlpha: false,
        antialias: true,
        depth: true,
        stencil: false,
      });
    if (!s.gl) return;

    resizeCanvas();
    s.resizeObserver = new ResizeObserver(() => resizeCanvas());
    s.resizeObserver.observe(canvas);
    startRenderLoop();
    void initForRace(props.race, props.side).catch((err) => console.error(err));

    return () => {
      s.disposed = true;
      s.initVersion++;
      cancelAnimationFrame(s.rafId);
      s.resizeObserver?.disconnect();
      destroyRenderer();
    };
  }, []);

  useEffect(() => {
    const s = stateRef.current;
    if (!s.gl || s.disposed) return;
    const initForRace = (s as any).initForRace as
      | ((race: Race, side: WorkerSide) => Promise<void>)
      | undefined;
    if (initForRace)
      void initForRace(props.race, props.side).catch((err) =>
        console.error(err),
      );
  }, [props.race, props.side]);

  return (
    <canvas
      ref={canvasRef}
      className="worker-unit"
      style={{
        position: "absolute",
        inset: "0",
        width: "100%",
        height: "100%",
        display: "block",
        pointerEvents: "none",
      }}
    />
  );
}
