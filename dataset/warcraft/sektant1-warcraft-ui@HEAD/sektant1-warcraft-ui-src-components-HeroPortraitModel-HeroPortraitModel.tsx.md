---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/HeroPortraitModel/HeroPortraitModel.tsx"
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

interface HeroConfig {
  modelPath: string;
  textures: Record<string, string>;
}

const HERO_CONFIG: Record<Race, HeroConfig> = {
  Human: {
    modelPath: "./models/hero-portrait/Human/HeroArchMage_portrait.mdx",
    textures: {
      "heroarchmage.blp":
        "./models/hero-portrait/textures/Human/HeroArchmage.blp",
    },
  },
  Orc: {
    modelPath: "./models/hero-portrait/Orc/HeroBladeMaster_portrait.mdx",
    textures: {
      "heroblademaster.blp":
        "./models/hero-portrait/textures/Orc/HeroBladeMaster.blp",
    },
  },
  NightElf: {
    modelPath: "./models/hero-portrait/NightElf/HeroDemonHunter_Portrait.mdx",
    textures: {
      "herodemonhunter.blp":
        "./models/hero-portrait/textures/NightElf/HeroDemonHunter.blp",
      "black32.blp": "./models/hero-portrait/textures/NightElf/Black32.blp",
    },
  },
  Undead: {
    modelPath: "./models/hero-portrait/Undead/HeroDeathKnight_portrait.mdx",
    textures: {
      "herodeathknight.blp":
        "./models/hero-portrait/textures/Undead/HeroDeathknight.blp",
    },
  },
};

const modelCache = new Map<string, ModelType>();
const blpCache = new Map<string, ImageData[]>();

function normalizeTextureName(textureImage: string): string {
  const normalized = textureImage.replaceAll("\\", "/");
  const lastSlash = normalized.lastIndexOf("/");
  return (
    lastSlash >= 0 ? normalized.slice(lastSlash + 1) : normalized
  ).toLowerCase();
}

function resolveTexturePath(
  textureImage: string,
  textures: Record<string, string>,
): string | null {
  const key = normalizeTextureName(textureImage);
  // Direct key match
  const direct = textures[key];
  if (direct) return direct;
  // Loose match (caller may have stored the full normalised key)
  for (const [k, v] of Object.entries(textures)) {
    if (normalizeTextureName(k) === key) return v;
  }
  // Shared fallbacks
  if (key.startsWith("teamcolor"))
    return "./models/hero-portrait/textures/shared/TeamColor00.blp";
  if (key.startsWith("teamglow"))
    return "./models/hero-portrait/textures/shared/TeamGlow00.blp";
  return null;
}

async function loadModelByPath(modelPath: string): Promise<ModelType> {
  const cached = modelCache.get(modelPath);
  if (cached) return cached;
  const res = await fetch(modelPath);
  if (!res.ok)
    throw new Error(`HeroPortraitModel: failed to load ${modelPath}`);
  const model = parseMDX(await res.arrayBuffer());
  modelCache.set(modelPath, model);
  return model;
}

async function loadBLPMips(path: string): Promise<ImageData[]> {
  const cached = blpCache.get(path);
  if (cached) return cached;
  const res = await fetch(path);
  if (!res.ok)
    throw new Error(`HeroPortraitModel: failed to load texture ${path}`);
  const blp = decodeBLP(await res.arrayBuffer());
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
    throw new Error(`HeroPortraitModel: no mips for ${path}`);
  blpCache.set(path, mips);
  return mips;
}

function findPortraitSequenceIndex(model: ModelType): number {
  const sequences = model.Sequences || [];
  if (sequences.length === 0) return 0;
  const byName = (predicate: (name: string) => boolean) =>
    sequences.findIndex((seq) => predicate((seq.Name || "").toLowerCase()));
  let idx = byName(
    (name) => name.includes("portrait") && !name.includes("talk"),
  );
  if (idx >= 0) return idx;
  idx = byName((name) => name.includes("stand"));
  if (idx >= 0) return idx;
  idx = byName((name) => name.includes("portrait"));
  if (idx >= 0) return idx;
  return 0;
}

export interface HeroPortraitModelProps {
  race: Race;
}

export default function HeroPortraitModel(props: HeroPortraitModelProps) {
  const { modelPath, textures } = HERO_CONFIG[props.race];
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const stateRef = useRef({
    gl: null as WebGLRenderingContext | WebGL2RenderingContext | null,
    modelRenderer: null as ModelRenderer | null,
    resizeObserver: null as ResizeObserver | null,
    rafId: 0,
    disposed: false,
    initVersion: 0,
    mvMatrix: mat4.create(),
    pMatrix: mat4.create(),
    worldCameraMatrix: mat4.create(),
    cameraPos: vec3.create(),
    cameraTarget: vec3.create(),
    cameraQuat: quat.create(),
    cameraFov: 0.691111,
    cameraNear: 8,
    cameraFar: 1800,
  });

  useEffect(() => {
    if (!modelPath) return;

    const s = stateRef.current;
    s.disposed = false;
    const canvas = canvasRef.current!;

    const destroyRenderer = () => {
      if (!s.modelRenderer) return;
      try {
        s.modelRenderer.destroy();
      } catch (err) {
        console.warn("hero portrait destroy failed", err);
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

    const configureCamera = (model: ModelType) => {
      const cam = model.Cameras?.[0];
      if (cam) {
        vec3.set(
          s.cameraPos,
          cam.Position[0],
          cam.Position[1],
          cam.Position[2],
        );
        vec3.set(
          s.cameraTarget,
          cam.TargetPosition[0],
          cam.TargetPosition[1],
          cam.TargetPosition[2],
        );
        s.cameraFov = Math.max(0.35, cam.FieldOfView || 0.691111);
        s.cameraNear = Math.max(4, cam.NearClip || 8);
        s.cameraFar = Math.max(s.cameraNear + 100, cam.FarClip || 1800);
        mat4.lookAt(
          s.mvMatrix,
          s.cameraPos,
          s.cameraTarget,
          vec3.fromValues(0, 0, 1),
        );
      } else {
        const min = model.Info?.MinimumExtent;
        const max = model.Info?.MaximumExtent;
        if (min && max) {
          const cx = (min[0] + max[0]) * 0.5,
            cy = (min[1] + max[1]) * 0.5,
            cz = (min[2] + max[2]) * 0.5;
          const radius = Math.max(140, model.Info?.BoundsRadius || 180);
          vec3.set(
            s.cameraPos,
            cx + radius * 0.2,
            cy - radius * 1.8,
            cz + radius * 0.2,
          );
          vec3.set(s.cameraTarget, cx, cy, cz);
        } else {
          vec3.set(s.cameraPos, 0, -450, 120);
          vec3.set(s.cameraTarget, 0, 0, 110);
        }
        s.cameraFov = 0.691111;
        s.cameraNear = 8;
        s.cameraFar = 1800;
        mat4.lookAt(
          s.mvMatrix,
          s.cameraPos,
          s.cameraTarget,
          vec3.fromValues(0, 0, 1),
        );
      }
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
      let lastTs = performance.now();
      const tick = (now: number) => {
        if (s.disposed) return;
        s.rafId = requestAnimationFrame(tick);
        if (!s.gl || !s.modelRenderer) return;
        resizeCanvas();
        const delta = Math.min(100, now - lastTs);
        lastTs = now;
        s.modelRenderer.update(delta);
        s.gl.clearColor(0, 0, 0, 0);
        s.gl.clear(s.gl.COLOR_BUFFER_BIT | s.gl.DEPTH_BUFFER_BIT);
        s.modelRenderer.render(s.mvMatrix, s.pMatrix, {
          env: false,
          wireframe: false,
        });
      };
      s.rafId = requestAnimationFrame(tick);
    };

    const initForConfig = async (
      modelPath: string,
      textures: Record<string, string>,
    ) => {
      const version = ++s.initVersion;
      const model = await loadModelByPath(modelPath);
      if (s.disposed || version !== s.initVersion || !s.gl) return;
      destroyRenderer();
      s.modelRenderer = new ModelRenderer(model);
      s.modelRenderer.initGL(s.gl);
      configureCamera(model);
      s.modelRenderer.setCamera(s.cameraPos, s.cameraQuat);
      s.modelRenderer.setLightPosition(s.cameraPos);
      s.modelRenderer.setLightColor(vec3.fromValues(1, 1, 1));
      s.modelRenderer.setSequence(findPortraitSequenceIndex(model));
      await Promise.all(
        (model.Textures || []).map(async (tex) => {
          const texturePath = resolveTexturePath(tex.Image, textures);
          if (!texturePath) return;
          const mips = await loadBLPMips(texturePath);
          if (!s.disposed && version === s.initVersion) {
            s.modelRenderer!.setTextureImageData(tex.Image, mips);
          }
        }),
      );
    };

    // Store for prop-change effect
    (s as any)._initForConfig = initForConfig;

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
    void initForConfig(modelPath, textures).catch(console.error);

    return () => {
      s.disposed = true;
      s.initVersion++;
      cancelAnimationFrame(s.rafId);
      s.resizeObserver?.disconnect();
      destroyRenderer();
    };
  }, []);

  // Reload when modelPath or textures change
  useEffect(() => {
    if (!modelPath) return;
    const s = stateRef.current;
    if (!s.gl || s.disposed) return;
    const fn = (s as any)._initForConfig as
      | ((modelPath: string, textures: Record<string, string>) => Promise<void>)
      | undefined;
    if (fn) void fn(modelPath, textures).catch(console.error);
  }, [props.race]);

  if (!modelPath) return null;

  const maskSrc = `./console/${props.race}/${props.race}UIPortraitWindowMask.png`;
  const frameSrc = `./console/${props.race}/${props.race}UIPortraitFrameCrop.png`;

  return (
    <div className="hero-portrait">
      <div
        className="hero-portrait__window"
        style={{
          WebkitMaskImage: `url("${maskSrc}")`,
          maskImage: `url("${maskSrc}")`,
        }}
      >
        <canvas ref={canvasRef} className="hero-portrait__canvas" />
      </div>
      <img src={frameSrc} alt="" className="hero-portrait__frame" />
    </div>
  );
}
