---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/sections/BundleSizeSection.tsx"
ref: HEAD
---

import { useEffect, useRef } from 'react';
import { parseMDX } from 'war3-model';

const components = [
  { name: 'GoldButton', size: 1.2 },
  { name: 'ResourceCounter', size: 0.8 },
  { name: 'CommandCard', size: 1.9 },
  { name: 'Tooltip', size: 1.4 },
  { name: 'PortraitFrame', size: 1.1 },
  { name: 'HealthBar', size: 0.6 },
  { name: 'Minimap', size: 2.1 },
  { name: 'Dialog', size: 1.7 },
  { name: 'ChatBubble', size: 0.9 },
  { name: 'HeroCard', size: 2.4 },
] as const;

const maxSize = Math.max(...components.map((c) => c.size));
const imageCache = new Map<string, Promise<HTMLImageElement>>();
const buildProgressBarModelPath = './models/build-progress/BuildProgressBar.mdx';
const fallbackCapRatio = 4 / 128;
let buildProgressBarUvPromise: Promise<{ minU: number; maxU: number }> | null = null;

interface Props {
  buildFillUrl: string;
  buildBorderUrl: string;
}

interface BuildProgressRowBarProps {
  valuePct: number;
  fillUrl: string;
  borderUrl: string;
}

function normalizeTexturePath(path: string): string {
  return path.replace(/\\/g, '/').toLowerCase();
}

async function loadBuildProgressBarUvDomain(): Promise<{ minU: number; maxU: number }> {
  if (buildProgressBarUvPromise) return buildProgressBarUvPromise;

  buildProgressBarUvPromise = (async () => {
    const fallback = { minU: 0, maxU: 1 };
    try {
      const res = await fetch(buildProgressBarModelPath);
      if (!res.ok) return fallback;
      const ab = await res.arrayBuffer();
      const model: any = parseMDX(ab);
      const textures: Array<{ Image?: string }> = model.Textures || [];
      const borderTexIndex = textures.findIndex((tex) =>
        normalizeTexturePath(String(tex?.Image || '')).endsWith('./human-buildprogressbar-border2.blp'),
      );
      if (borderTexIndex < 0) return fallback;

      const materials: Array<{ Layers?: Array<{ TextureID?: number }> }> = model.Materials || [];
      const borderMaterialIndex = materials.findIndex((material) =>
        (material?.Layers || []).some((layer) => layer?.TextureID === borderTexIndex),
      );
      if (borderMaterialIndex < 0) return fallback;

      const geosets: Array<{ MaterialID?: number; TVertices?: number[][] }> = model.Geosets || [];
      const borderGeoset = geosets.find((geoset) => geoset.MaterialID === borderMaterialIndex);
      const uv = borderGeoset?.TVertices?.[0];
      if (!uv || uv.length < 2) return fallback;

      let minU = Infinity;
      let maxU = -Infinity;
      for (let i = 0; i < uv.length; i += 2) {
        const u = uv[i];
        if (u < minU) minU = u;
        if (u > maxU) maxU = u;
      }
      if (!Number.isFinite(minU) || !Number.isFinite(maxU) || maxU <= minU) return fallback;
      return { minU, maxU };
    } catch (err) {
      console.warn('Failed to load BuildProgressBar UV domain, falling back to texture-domain caps.', err);
      return fallback;
    }
  })();

  return buildProgressBarUvPromise;
}

async function loadDataUrlImage(url: string): Promise<HTMLImageElement> {
  const cached = imageCache.get(url);
  if (cached) return cached;
  const promise = new Promise<HTMLImageElement>((resolve, reject) => {
    const img = new Image();
    img.onload = () => resolve(img);
    img.onerror = () => reject(new Error(`Failed to load image ${url}`));
    img.src = url;
  });
  imageCache.set(url, promise);
  return promise;
}

function deriveBuildBarCapRatio(
  borderImg: HTMLImageElement,
  uvDomain: { minU: number; maxU: number },
): number {
  const width = borderImg.width;
  const height = borderImg.height;
  if (width <= 2 || height <= 0) return fallbackCapRatio;

  const scratch = document.createElement('canvas');
  scratch.width = width;
  scratch.height = height;
  const scratchCtx = scratch.getContext('2d', { willReadFrequently: true });
  if (!scratchCtx) return fallbackCapRatio;

  scratchCtx.clearRect(0, 0, width, height);
  scratchCtx.drawImage(borderImg, 0, 0);
  const data = scratchCtx.getImageData(0, 0, width, height).data;
  const row = Math.floor(height / 2);

  const alphaAt = (x: number) => data[(row * width + x) * 4 + 3];
  let firstOpaque = 0;
  while (firstOpaque < width && alphaAt(firstOpaque) === 0) firstOpaque += 1;

  let firstGap = firstOpaque;
  while (firstGap < width && alphaAt(firstGap) !== 0) firstGap += 1;
  if (firstGap <= firstOpaque || firstGap >= width) return fallbackCapRatio;

  const uSpan = Math.max(1e-6, uvDomain.maxU - uvDomain.minU);
  const uAtGap = firstGap / width;
  const ratio = (uAtGap - uvDomain.minU) / uSpan;
  return Math.max(1 / width, Math.min(0.45, ratio));
}

function drawThreeSliceTextureToCanvas(
  ctx: CanvasRenderingContext2D,
  img: HTMLImageElement,
  x: number,
  y: number,
  width: number,
  height: number,
  capPx: number,
) {
  if (width <= 0 || height <= 0) return;

  const srcW = img.width;
  const srcH = img.height;
  const srcCap = Math.min(capPx, Math.floor(srcW / 2));
  const dstCap = Math.min(srcCap, Math.floor(width / 2));

  if (dstCap > 0) {
    ctx.drawImage(img, 0, 0, srcCap, srcH, x, y, dstCap, height);
  }

  const dstCenterW = width - dstCap * 2;
  if (dstCenterW > 0) {
    const srcCenterX = srcCap;
    const srcCenterW = Math.max(1, srcW - srcCap * 2);
    ctx.drawImage(img, srcCenterX, 0, srcCenterW, srcH, x + dstCap, y, dstCenterW, height);
  }

  if (dstCap > 0) {
    ctx.drawImage(img, srcW - srcCap, 0, srcCap, srcH, x + width - dstCap, y, dstCap, height);
  }
}

function drawBuildProgressBarToCanvas(
  canvas: HTMLCanvasElement,
  fillImg: HTMLImageElement,
  borderImg: HTMLImageElement,
  valuePct: number,
  capRatio: number,
) {
  const rect = canvas.getBoundingClientRect();
  const cssW = Math.max(1, rect.width);
  const cssH = Math.max(1, rect.height);
  const dpr = window.devicePixelRatio || 1;
  const pxW = Math.max(1, Math.round(cssW * dpr));
  const pxH = Math.max(1, Math.round(cssH * dpr));
  if (canvas.width !== pxW || canvas.height !== pxH) {
    canvas.width = pxW;
    canvas.height = pxH;
  }

  const ctx = canvas.getContext('2d');
  if (!ctx) return;
  ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
  ctx.clearRect(0, 0, cssW, cssH);
  ctx.imageSmoothingEnabled = true;
  const capPx = Math.max(1, Math.round(cssW * capRatio));

  const fillW = Math.max(0, Math.min(cssW, (valuePct / 100) * cssW));
  if (fillW > 0) {
    drawThreeSliceTextureToCanvas(ctx, fillImg, 0, 0, fillW, cssH, capPx);
  }

  drawThreeSliceTextureToCanvas(ctx, borderImg, 0, 0, cssW, cssH, capPx);
}

function BuildProgressRowBar(props: BuildProgressRowBarProps) {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const renderNonceRef = useRef(0);
  const destroyedRef = useRef(false);

  useEffect(() => {
    destroyedRef.current = false;
    return () => { destroyedRef.current = true; };
  }, []);

  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas || destroyedRef.current) return;
    const fillUrl = props.fillUrl;
    const borderUrl = props.borderUrl;
    const currentNonce = ++renderNonceRef.current;

    if (!fillUrl || !borderUrl) {
      const ctx = canvas.getContext('2d');
      if (ctx) {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
      }
      return;
    }

    void Promise.all([
      loadDataUrlImage(fillUrl),
      loadDataUrlImage(borderUrl),
      loadBuildProgressBarUvDomain(),
    ])
      .then(([fillImg, borderImg, uvDomain]) => {
        if (destroyedRef.current || currentNonce !== renderNonceRef.current) return;
        const capRatio = deriveBuildBarCapRatio(borderImg, uvDomain);
        drawBuildProgressBarToCanvas(canvas, fillImg, borderImg, props.valuePct, capRatio);
      })
      .catch((err) => console.error(err));
  }, [props.valuePct, props.fillUrl, props.borderUrl]);

  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;

    const resizeObserver = new ResizeObserver(() => {
      const fillUrl = props.fillUrl;
      const borderUrl = props.borderUrl;
      if (!fillUrl || !borderUrl) return;
      void Promise.all([
        loadDataUrlImage(fillUrl),
        loadDataUrlImage(borderUrl),
        loadBuildProgressBarUvDomain(),
      ])
        .then(([fillImg, borderImg, uvDomain]) => {
          if (destroyedRef.current) return;
          const capRatio = deriveBuildBarCapRatio(borderImg, uvDomain);
          drawBuildProgressBarToCanvas(canvas, fillImg, borderImg, props.valuePct, capRatio);
        })
        .catch((err) => console.error(err));
    });
    resizeObserver.observe(canvas);

    return () => {
      resizeObserver.disconnect();
    };
  }, [props.fillUrl, props.borderUrl, props.valuePct]);

  return <canvas ref={canvasRef} className="bundle-chart-bar-canvas" />;
}

export default function BundleSizeSection(props: Props) {
  return (
    <section className="section-card">
      <h3>Leaner than a Blademaster with Wind Walk.</h3>
      <p className="section-desc">
        Tree-shakeable ESM. Import only what you need. Average component weighs under 1.4kb gzipped.
      </p>
      <div className="bundle-chart">
        {components.map((c) => (
          <div key={c.name} className="bundle-chart-row">
            <span className="bundle-chart-label">{c.name}</span>
            <div className="bundle-chart-bar-track">
              <BuildProgressRowBar
                valuePct={(c.size / maxSize) * 100}
                fillUrl={props.buildFillUrl}
                borderUrl={props.buildBorderUrl}
              />
            </div>
            <span className="bundle-chart-size">{c.size}kb</span>
          </div>
        ))}
      </div>
    </section>
  );
}
