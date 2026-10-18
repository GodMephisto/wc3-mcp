---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/ItemModel/ItemModel.tsx"
ref: HEAD
---

import { useEffect, useRef } from "react";
import { ModelRenderer, decodeBLP, getBLPImageData, parseMDX } from "war3-model";
import { mat3, mat4, quat, vec3 } from "gl-matrix";

type ModelType = ReturnType<typeof parseMDX>;

const modelCache = new Map<string, ModelType>();
const blpCache = new Map<string, ImageData[]>();

function normaliseKey(textureImage: string): string {
  const s = textureImage.replaceAll("\\", "/").toLowerCase();
  const slash = s.lastIndexOf("/");
  return slash >= 0 ? s.slice(slash + 1) : s;
}

async function loadModel(modelPath: string): Promise<ModelType> {
  const cached = modelCache.get(modelPath);
  if (cached) return cached;
  const res = await fetch(modelPath);
  if (!res.ok) throw new Error(`ItemModel: failed to fetch ${modelPath}`);
  const model = parseMDX(await res.arrayBuffer());
  modelCache.set(modelPath, model);
  return model;
}

async function loadBLPMips(path: string): Promise<ImageData[]> {
  const cached = blpCache.get(path);
  if (cached) return cached;
  const res = await fetch(path);
  if (!res.ok) throw new Error(`ItemModel: failed to fetch texture ${path}`);
  const blp = decodeBLP(await res.arrayBuffer());
  const mips: ImageData[] = [];
  const maxLevels = Math.max(1, Math.floor(Math.log2(Math.max(blp.width, blp.height))) + 1);
  for (let i = 0; i < maxLevels; i++) {
    const w = blp.width >> i;
    const h = blp.height >> i;
    if (w <= 0 || h <= 0) break;
    try {
      const img = getBLPImageData(blp, i);
      if (!img?.width || !img?.height) break;
      mips.push(new ImageData(new Uint8ClampedArray(img.data), img.width, img.height));
      if (img.width === 1 && img.height === 1) break;
    } catch { break; }
  }
  if (mips.length === 0) throw new Error(`ItemModel: no mips decoded for ${path}`);
  blpCache.set(path, mips);
  return mips;
}

function findStandSequence(model: ModelType): number {
  const seqs = model.Sequences ?? [];
  let idx = seqs.findIndex((s) => (s.Name ?? "").toLowerCase().includes("stand"));
  if (idx < 0) idx = 0;
  return idx;
}

interface ItemModelProps {
  modelPath: string;
  textures: Record<string, string>;
  /** Full rotations per second. Default 0.12 (~1 rotation every 8 s). */
  rpm?: number;
}

export default function ItemModel({ modelPath, textures, rpm = 0.12 }: ItemModelProps) {
  const canvasRef = useRef<HTMLCanvasElement>(null);

  useEffect(() => {
    if (!modelPath) return;

    const canvas = canvasRef.current!;
    const s = {
      gl: null as WebGLRenderingContext | WebGL2RenderingContext | null,
      renderer: null as ModelRenderer | null,
      rafId: 0,
      disposed: false,
      initVersion: 0,
      mvBase: mat4.create(),
      mvMatrix: mat4.create(),
      pMatrix: mat4.create(),
      rotRad: 0,
      lastTs: 0,
    };

    const destroy = () => {
      if (!s.renderer) return;
      try { s.renderer.destroy(); } catch { /* ignore */ }
      s.renderer = null;
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
      const aspect = Math.max(1e-6, canvas.clientWidth / Math.max(1, canvas.clientHeight));
      mat4.perspective(s.pMatrix, 0.7, aspect, 4, 2000);
    };

    const buildBaseCamera = (model: ModelType) => {
      const radius = Math.max(32, model.Info?.BoundsRadius ?? 64);
      const dist = radius * 2.8;
      const elev = radius * 0.6;
      const centerZ = ((model.Info?.MinimumExtent?.[2] ?? 0) + (model.Info?.MaximumExtent?.[2] ?? 0)) * 0.5;
      // Camera fixed on the -Y side looking toward +Y, slightly above
      const eye = vec3.fromValues(0, -dist, centerZ + elev);
      const target = vec3.fromValues(0, 0, centerZ);
      mat4.lookAt(s.mvBase, eye, target, vec3.fromValues(0, 0, 1));
    };

    const startLoop = () => {
      s.lastTs = performance.now();
      const tick = (now: number) => {
        if (s.disposed) return;
        s.rafId = requestAnimationFrame(tick);
        if (!s.gl || !s.renderer) return;
        resizeCanvas();
        const dt = Math.min(100, now - s.lastTs);
        s.lastTs = now;
        s.rotRad += (dt / 1000) * rpm * Math.PI * 2;
        // Rotate model around Z axis
        const rot = mat4.create();
        mat4.rotateZ(rot, rot, s.rotRad);
        mat4.multiply(s.mvMatrix, s.mvBase, rot);

        s.renderer.update(dt);
        s.gl.clearColor(0, 0, 0, 0);
        s.gl.clear(s.gl.COLOR_BUFFER_BIT | s.gl.DEPTH_BUFFER_BIT);
        s.renderer.render(s.mvMatrix, s.pMatrix, { env: false, wireframe: false });
      };
      s.rafId = requestAnimationFrame(tick);
    };

    const init = async (version: number) => {
      const model = await loadModel(modelPath);
      if (s.disposed || version !== s.initVersion || !s.gl) return;
      destroy();
      s.renderer = new ModelRenderer(model);
      s.renderer.initGL(s.gl);
      buildBaseCamera(model);
      s.renderer.setSequence(findStandSequence(model));

      // Set camera position for lighting
      const eye = vec3.create();
      mat4.getTranslation(eye, s.mvBase);
      const wMat = mat4.create();
      if (mat4.invert(wMat, s.mvBase)) {
        const rot = mat3.create();
        mat3.fromMat4(rot, wMat);
        const q = quat.create();
        quat.fromMat3(q, rot);
        quat.normalize(q, q);
        s.renderer.setCamera(eye, q);
      }
      s.renderer.setLightPosition(eye);
      s.renderer.setLightColor(vec3.fromValues(1, 1, 1));

      const textureKeys = Object.keys(textures);
      await Promise.all(
        (model.Textures ?? []).map(async (tex) => {
          const key = normaliseKey(tex.Image);
          const matchedKey = textureKeys.find((k) => normaliseKey(k) === key || k === key);
          const path = matchedKey ? textures[matchedKey] : null;
          if (!path) return;
          const mips = await loadBLPMips(path);
          if (!s.disposed && version === s.initVersion) {
            s.renderer!.setTextureImageData(tex.Image, mips);
          }
        }),
      );
    };

    s.gl = canvas.getContext("webgl2", {
      alpha: true, premultipliedAlpha: false, antialias: true, depth: true,
    }) as WebGL2RenderingContext | null;
    if (!s.gl) {
      s.gl = canvas.getContext("webgl", {
        alpha: true, premultipliedAlpha: false, antialias: true, depth: true,
      });
    }
    if (!s.gl) return;

    resizeCanvas();
    startLoop();
    void init(++s.initVersion).catch((e) => console.error("ItemModel init error:", e));

    return () => {
      s.disposed = true;
      s.initVersion++;
      cancelAnimationFrame(s.rafId);
      destroy();
    };
  }, [modelPath, textures, rpm]);

  if (!modelPath) return null;

  return (
    <canvas
      ref={canvasRef}
      style={{
        position: "absolute",
        inset: 0,
        width: "100%",
        height: "100%",
        display: "block",
        pointerEvents: "none",
      }}
    />
  );
}
