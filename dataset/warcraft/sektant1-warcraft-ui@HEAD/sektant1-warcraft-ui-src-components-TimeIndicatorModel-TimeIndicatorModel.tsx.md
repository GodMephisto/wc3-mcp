---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/TimeIndicatorModel/TimeIndicatorModel.tsx"
ref: HEAD
---

import { useEffect, useRef, useState } from "react";
import { ModelRenderer, decodeBLP, getBLPImageData, parseMDX } from "war3-model";
import { mat4, quat, vec3 } from "gl-matrix";
import { RACE_PREFIXES } from "../../state/race";
import type { Race } from "../../utils/types";
import "./style.css";

type ModelType = ReturnType<typeof parseMDX>;

const modelCache = new Map<Race, ModelType>();
const blpCache = new Map<string, ImageData[]>();
const widgetMaskCache = new Map<Race, string>();

function computeVisibleBounds2D(model: ModelType) {
  let minX = Infinity, maxX = -Infinity, minY = Infinity, maxY = -Infinity;
  let hasVisible = false;
  for (const geoset of model.Geosets || []) {
    if (!geoset.Faces || geoset.Faces.length === 0) continue;
    const verts = geoset.Vertices;
    for (let i = 0; i < verts.length; i += 3) {
      const x = verts[i], y = verts[i + 1];
      if (x < minX) minX = x;
      if (x > maxX) maxX = x;
      if (y < minY) minY = y;
      if (y > maxY) maxY = y;
      hasVisible = true;
    }
  }
  return hasVisible ? { minX, maxX, minY, maxY } : null;
}

function disableBackdropQuads(model: ModelType) {
  const hiddenNodes = new Set(["plane01", "sunrise/set"]);
  for (const geoset of model.Geosets || []) {
    const nodeIndex = geoset.Groups?.[0]?.[0];
    if (nodeIndex == null) continue;
    const nodeName = model.Nodes?.[nodeIndex]?.Name?.toLowerCase();
    if (!nodeName || !hiddenNodes.has(nodeName)) continue;
    geoset.Faces = new Uint16Array(0);
  }
}

function getModelPath(race: Race): string {
  const tile = RACE_PREFIXES[race].tile;
  return `/models/time-indicator/${tile}UI-TimeIndicator.mdx`;
}

function getClockTexturePath(textureImage: string, race: Race): string | null {
  const norm = textureImage.replaceAll("\\", "/").toLowerCase();
  const tile = RACE_PREFIXES[race].tile;
  const base = `/models/time-indicator/textures/${race}/`;

  if (norm.includes("genericglowfaded")) return `${base}GenericGlowFaded.blp`;
  if (norm.includes("star3")) return `${base}star3.blp`;
  if (norm.includes("genericglow2_32")) return `${base}GenericGlow2_32.blp`;
  if (norm.includes("timeindicatorframe")) return `${base}${tile}UITile-TimeIndicatorFrame.blp`;
  if (norm.includes("timeindicator")) return `${base}HumanUITile-TimeIndicator.blp`;
  return null;
}

function getFrameTexturePath(race: Race): string {
  const tile = RACE_PREFIXES[race].tile;
  return `/models/time-indicator/textures/${race}/${tile}UITile-TimeIndicatorFrame.blp`;
}

function findTransparentSeed(alpha: Uint8ClampedArray, width: number, height: number, startX: number, startY: number): number {
  const inBounds = (x: number, y: number) => x >= 0 && y >= 0 && x < width && y < height;
  const alphaAt = (x: number, y: number) => alpha[(y * width + x) * 4 + 3];
  if (inBounds(startX, startY) && alphaAt(startX, startY) === 0) return startY * width + startX;
  const maxRadius = Math.max(width, height);
  for (let r = 1; r < maxRadius; r++) {
    for (let dy = -r; dy <= r; dy++) {
      const y = startY + dy;
      const x1 = startX - r, x2 = startX + r;
      if (inBounds(x1, y) && alphaAt(x1, y) === 0) return y * width + x1;
      if (inBounds(x2, y) && alphaAt(x2, y) === 0) return y * width + x2;
    }
    for (let dx = -r + 1; dx <= r - 1; dx++) {
      const x = startX + dx;
      const y1 = startY - r, y2 = startY + r;
      if (inBounds(x, y1) && alphaAt(x, y1) === 0) return y1 * width + x;
      if (inBounds(x, y2) && alphaAt(x, y2) === 0) return y2 * width + x;
    }
  }
  return -1;
}

async function loadBLPMips(path: string): Promise<ImageData[]> {
  const cached = blpCache.get(path);
  if (cached) return cached;
  const res = await fetch(path);
  if (!res.ok) throw new Error(`Failed to fetch texture ${path}`);
  const ab = await res.arrayBuffer();
  const blp = decodeBLP(ab);
  const mips: ImageData[] = [];
  const maxLevels = Math.max(1, Math.floor(Math.log2(Math.max(blp.width, blp.height))) + 1);
  for (let i = 0; i < maxLevels; i++) {
    const w = blp.width >> i, h = blp.height >> i;
    if (w <= 0 || h <= 0) break;
    try {
      const img = getBLPImageData(blp, i);
      if (!img || !img.width || !img.height) break;
      mips.push(new ImageData(img.data, img.width, img.height));
      if (img.width === 1 && img.height === 1) break;
    } catch {
      break;
    }
  }
  if (mips.length === 0) throw new Error(`No mip levels decoded for ${path}`);
  blpCache.set(path, mips);
  return mips;
}

async function loadWidgetMaskDataUrl(race: Race): Promise<string> {
  const cached = widgetMaskCache.get(race);
  if (cached) return cached;

  const frame = (await loadBLPMips(getFrameTexturePath(race)))[0];
  if (!frame || frame.width <= 0 || frame.height <= 0) throw new Error(`Invalid frame texture for ${race}`);

  const { width, height } = frame;
  const seedX = Math.round(width * 0.49), seedY = Math.round(height * 0.34);
  const seed = findTransparentSeed(frame.data, width, height, seedX, seedY);
  if (seed < 0) throw new Error(`Unable to locate clock aperture for ${race}`);

  const aperture = new Uint8Array(width * height);
  const queue = new Int32Array(width * height);
  let head = 0, tail = 0;
  queue[tail++] = seed;
  aperture[seed] = 1;

  while (head < tail) {
    const cur = queue[head++];
    const x = cur % width, y = (cur / width) | 0;
    for (const [nx, ny] of [[x - 1, y], [x + 1, y], [x, y - 1], [x, y + 1]]) {
      if (nx < 0 || ny < 0 || nx >= width || ny >= height) continue;
      const next = ny * width + nx;
      if (aperture[next]) continue;
      if (frame.data[next * 4 + 3] !== 0) continue;
      aperture[next] = 1;
      queue[tail++] = next;
    }
  }

  const pixels = new Uint8ClampedArray(width * height * 4);
  for (let i = 0; i < width * height; i++) {
    const frameOpaque = frame.data[i * 4 + 3] > 0;
    const keep = frameOpaque || aperture[i] === 1;
    if (!keep) continue;
    const p = i * 4;
    pixels[p] = pixels[p + 1] = pixels[p + 2] = pixels[p + 3] = 255;
  }

  const canvas = document.createElement("canvas");
  canvas.width = width;
  canvas.height = height;
  const ctx = canvas.getContext("2d")!;
  ctx.putImageData(new ImageData(pixels, width, height), 0, 0);
  const dataUrl = canvas.toDataURL("image/png");
  widgetMaskCache.set(race, dataUrl);
  return dataUrl;
}

async function loadModel(race: Race): Promise<ModelType> {
  const cached = modelCache.get(race);
  if (cached) return cached;
  const res = await fetch(getModelPath(race));
  if (!res.ok) throw new Error(`Failed to fetch clock model for ${race}`);
  const ab = await res.arrayBuffer();
  const model = parseMDX(ab);
  disableBackdropQuads(model);
  modelCache.set(race, model);
  return model;
}

interface Props {
  race: Race;
}

export default function TimeIndicatorModel({ race }: Props) {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const [widgetMaskUrl, setWidgetMaskUrl] = useState("");
  const stateRef = useRef({
    gl: null as WebGLRenderingContext | WebGL2RenderingContext | null,
    modelRenderer: null as ModelRenderer | null,
    rafId: 0,
    disposed: false,
    initVersion: 0,
    viewMinX: 0, viewMaxX: 1, viewMinY: 0, viewMaxY: 1,
  });

  const mvMatrix = useRef(mat4.create());
  const pMatrix = useRef(mat4.create());

  useEffect(() => {
    const s = stateRef.current;
    const canvas = canvasRef.current!;

    s.gl = canvas.getContext("webgl2", {
      alpha: true, premultipliedAlpha: false, antialias: true, depth: true, stencil: false,
    }) as WebGL2RenderingContext | null;
    if (!s.gl) {
      s.gl = canvas.getContext("webgl", {
        alpha: true, premultipliedAlpha: false, antialias: true, depth: true, stencil: false,
      });
    }
    if (!s.gl) return;

    const resizeCanvas = () => {
      if (!canvas || !s.gl) return;
      const dpr = window.devicePixelRatio || 1;
      const w = Math.max(1, Math.round(canvas.clientWidth * dpr));
      const h = Math.max(1, Math.round(canvas.clientHeight * dpr));
      if (canvas.width !== w || canvas.height !== h) {
        canvas.width = w;
        canvas.height = h;
      }
      s.gl.viewport(0, 0, w, h);
    };

    resizeCanvas();
    const observer = new ResizeObserver(() => resizeCanvas());
    observer.observe(canvas);

    const startTs = performance.now();
    const tick = (now: number) => {
      if (s.disposed) return;
      s.rafId = requestAnimationFrame(tick);
      if (!s.gl || !s.modelRenderer) return;

      resizeCanvas();
      const frame = ((now - startTs) * 1.0) % 60000;
      s.modelRenderer.setSequence(0);
      s.modelRenderer.setFrame(frame);
      s.modelRenderer.update(0);

      mat4.identity(mvMatrix.current);
      const baseW = Math.max(1e-6, s.viewMaxX - s.viewMinX);
      const baseH = Math.max(1e-6, s.viewMaxY - s.viewMinY);
      const cx = (s.viewMinX + s.viewMaxX) * 0.5;
      const cy = (s.viewMinY + s.viewMaxY) * 0.5;
      const canvasAspect = Math.max(1e-6, canvas.clientWidth / Math.max(1, canvas.clientHeight));
      const modelAspect = baseW / baseH;
      let viewW = baseW * 1.04, viewH = baseH * 1.04;
      if (canvasAspect > modelAspect) viewW = viewH * canvasAspect;
      else viewH = viewW / canvasAspect;
      mat4.ortho(pMatrix.current, cx - viewW * 0.5, cx + viewW * 0.5, cy - viewH * 0.5, cy + viewH * 0.5, -10, 10);
      s.gl.clearColor(0, 0, 0, 0);
      s.gl.clear(s.gl.COLOR_BUFFER_BIT | s.gl.DEPTH_BUFFER_BIT);
      s.modelRenderer.render(mvMatrix.current, pMatrix.current, { env: false, wireframe: false });
    };
    s.rafId = requestAnimationFrame(tick);

    return () => {
      s.disposed = true;
      s.initVersion++;
      cancelAnimationFrame(s.rafId);
      observer.disconnect();
      if (s.modelRenderer) {
        try { s.modelRenderer.destroy(); } catch {}
        s.modelRenderer = null;
      }
    };
  }, []);

  useEffect(() => {
    const s = stateRef.current;
    if (!s.gl) return;

    const version = ++s.initVersion;

    (async () => {
      const model = await loadModel(race);
      if (s.disposed || version !== s.initVersion) return;

      if (s.modelRenderer) {
        try { s.modelRenderer.destroy(); } catch {}
      }
      s.modelRenderer = new ModelRenderer(model);
      s.modelRenderer.initGL(s.gl!);
      s.modelRenderer.setCamera(vec3.fromValues(0, 0, 2), quat.fromValues(0, 0, 0, 1));
      s.modelRenderer.setLightPosition(vec3.fromValues(0, 0, 2));
      s.modelRenderer.setLightColor(vec3.fromValues(1, 1, 1));

      const bounds = computeVisibleBounds2D(model);
      if (bounds) {
        s.viewMinX = bounds.minX; s.viewMaxX = bounds.maxX;
        s.viewMinY = bounds.minY; s.viewMaxY = bounds.maxY;
      } else {
        const extMin = model.Info?.MinimumExtent;
        const extMax = model.Info?.MaximumExtent;
        if (extMin && extMax) {
          s.viewMinX = extMin[0]; s.viewMaxX = extMax[0];
          s.viewMinY = extMin[1]; s.viewMaxY = extMax[1];
        }
      }

      const texturePromises = (model.Textures || []).map(async (tex) => {
        const path = getClockTexturePath(tex.Image, race);
        if (!path) return;
        const mips = await loadBLPMips(path);
        if (!s.disposed && version === s.initVersion) {
          s.modelRenderer!.setTextureImageData(tex.Image, mips);
        }
      });

      const [maskUrl] = await Promise.all([
        loadWidgetMaskDataUrl(race),
        Promise.all(texturePromises),
      ]);
      if (s.disposed || version !== s.initVersion) return;
      setWidgetMaskUrl(maskUrl);
      s.modelRenderer.setSequence(0);
    })();
  }, [race]);

  return (
    <canvas
      ref={canvasRef}
      className="wc-time-indicator"
      style={{
        maskImage: widgetMaskUrl ? `url("${widgetMaskUrl}")` : "none",
        WebkitMaskImage: widgetMaskUrl ? `url("${widgetMaskUrl}")` : "none",
      }}
    />
  );
}
