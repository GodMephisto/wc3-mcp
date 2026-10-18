---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/sections/BordersSection.tsx"
ref: HEAD
---

import { useState, useEffect } from 'react';
import { ALL_BORDER_PATHS, BORDER_CATEGORIES } from '../../data/borderManifest';

interface Props {
  loadBlpDataUrl: (path: string) => Promise<string>;
}

interface BorderRenderEntry {
  previewUrl: string;
}

const decodedImageCache = new Map<string, Promise<HTMLImageElement>>();

function isAtlasNineSlice(img: HTMLImageElement): boolean {
  const ratio = img.width / Math.max(1, img.height);
  return ratio >= 7.25 && ratio <= 8.75 && img.width >= 64 && img.height >= 8;
}

async function loadDataUrlImage(url: string): Promise<HTMLImageElement> {
  const cached = decodedImageCache.get(url);
  if (cached) return cached;
  const promise = new Promise<HTMLImageElement>((resolve, reject) => {
    const img = new Image();
    img.onload = () => resolve(img);
    img.onerror = () => reject(new Error(`Failed to load image ${url}`));
    img.src = url;
  });
  decodedImageCache.set(url, promise);
  return promise;
}

function rotateCellClockwise(atlas: HTMLImageElement, cellIndex: number, cellW: number, cellH: number): HTMLCanvasElement {
  const canvas = document.createElement('canvas');
  canvas.width = cellH;
  canvas.height = cellW;
  const ctx = canvas.getContext('2d');
  if (!ctx) return canvas;
  ctx.translate(cellH, 0);
  ctx.rotate(Math.PI / 2);
  ctx.drawImage(atlas, cellIndex * cellW, 0, cellW, cellH, 0, 0, cellW, cellH);
  return canvas;
}

function drawHorizontallyTiledEdge(
  ctx: CanvasRenderingContext2D,
  tile: HTMLCanvasElement,
  x: number,
  y: number,
  width: number,
  height: number,
) {
  if (width <= 0 || height <= 0) return;

  let drawn = 0;
  while (drawn < width) {
    const segmentW = Math.min(height, width - drawn);
    const srcW = (segmentW / height) * tile.width;
    ctx.drawImage(tile, 0, 0, srcW, tile.height, x + drawn, y, segmentW, height);
    drawn += segmentW;
  }
}

function drawVerticallyTiledAtlasEdge(
  ctx: CanvasRenderingContext2D,
  atlas: HTMLImageElement,
  cellIndex: number,
  cellW: number,
  cellH: number,
  x: number,
  y: number,
  width: number,
  height: number,
) {
  if (width <= 0 || height <= 0) return;

  let drawn = 0;
  while (drawn < height) {
    const segmentH = Math.min(width, height - drawn);
    const srcH = (segmentH / width) * cellH;
    ctx.drawImage(atlas, cellIndex * cellW, 0, cellW, srcH, x, y + drawn, width, segmentH);
    drawn += segmentH;
  }
}

function renderAtlasBorderPreview(atlas: HTMLImageElement, skipVerticalSides: boolean): string {
  const cssW = 156;
  const cssH = 96;
  const frameSide = 82;
  const frameX = Math.floor((cssW - frameSide) / 2);
  const frameY = Math.floor((cssH - frameSide) / 2);
  const dpr = 2;
  const canvas = document.createElement('canvas');
  canvas.width = cssW * dpr;
  canvas.height = cssH * dpr;

  const ctx = canvas.getContext('2d');
  if (!ctx) return '';

  ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
  ctx.clearRect(0, 0, cssW, cssH);
  ctx.imageSmoothingEnabled = true;

  const atlasW = atlas.width;
  const atlasH = atlas.height;
  const cellW = atlasW / 8;
  const corner = Math.max(4, Math.floor(Math.min(atlasH * 0.8, frameSide / 3)));

  const drawCell = (idx: number, x: number, y: number, w: number, h: number) => {
    ctx.drawImage(atlas, idx * cellW, 0, cellW, atlasH, x, y, w, h);
  };

  drawCell(4, frameX, frameY, corner, corner);
  drawCell(5, frameX + frameSide - corner, frameY, corner, corner);
  drawCell(6, frameX, frameY + frameSide - corner, corner, corner);
  drawCell(7, frameX + frameSide - corner, frameY + frameSide - corner, corner, corner);

  const edgeH = frameSide - corner * 2;
  if (!skipVerticalSides && edgeH > 0) {
    drawVerticallyTiledAtlasEdge(ctx, atlas, 0, cellW, atlasH, frameX, frameY + corner, corner, edgeH);
    drawVerticallyTiledAtlasEdge(ctx, atlas, 1, cellW, atlasH, frameX + frameSide - corner, frameY + corner, corner, edgeH);
  }

  const edgeW = frameSide - corner * 2;
  if (edgeW > 0) {
    const topRot = rotateCellClockwise(atlas, 2, cellW, atlasH);
    const bottomRot = rotateCellClockwise(atlas, 3, cellW, atlasH);
    drawHorizontallyTiledEdge(ctx, topRot, frameX + corner, frameY, edgeW, corner);
    drawHorizontallyTiledEdge(ctx, bottomRot, frameX + corner, frameY + frameSide - corner, edgeW, corner);
  }

  return canvas.toDataURL('image/png');
}

function isCinematicBorder(path: string): boolean {
  return path.toLowerCase().includes('cinematic');
}

async function buildBorderPreview(path: string, rawUrl: string): Promise<string> {
  const img = await loadDataUrlImage(rawUrl);
  if (!isAtlasNineSlice(img)) return rawUrl;
  return renderAtlasBorderPreview(img, isCinematicBorder(path)) || rawUrl;
}

export default function BordersSection(props: Props) {
  const [rendered, setRendered] = useState<Record<string, BorderRenderEntry>>({});
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let cancelled = false;
    void (async () => {
      const results = await Promise.all(
        ALL_BORDER_PATHS.map(async (path) => {
          try {
            const rawUrl = await props.loadBlpDataUrl(path);
            const previewUrl = await buildBorderPreview(path, rawUrl);
            return [path, { previewUrl }] as const;
          } catch (e) {
            console.warn(`[BordersSection] failed to load ${path}`, e);
            return null;
          }
        }),
      );
      if (cancelled) return;
      const map: Record<string, BorderRenderEntry> = {};
      for (const r of results) {
        if (r) map[r[0]] = r[1];
      }
      setRendered(map);
      setLoading(false);
    })();
    return () => { cancelled = true; };
  }, []);

  return (
    <div className="tab-content">
      <div className="section-card">
        <h2>Border Textures</h2>
        <p className="section-desc">
          All {ALL_BORDER_PATHS.length} border BLP textures across {BORDER_CATEGORIES.length} categories.
          These 9-slice atlases, bar borders, and panel borders define the chrome around every UI element.
        </p>
        {loading ? (
          <div className="borders-loading">Decoding border textures…</div>
        ) : (
          BORDER_CATEGORIES.map((cat) => (
            <div key={cat.title} className="borders-category">
              <h4 className="borders-category-title">{cat.title}</h4>
              <p className="borders-category-desc">{cat.desc}</p>
              <div className="borders-grid">
                {cat.entries.map((entry) => {
                  const image = rendered[entry.path];
                  return image ? (
                    <div key={entry.path} className="border-item">
                      <img className="border-thumbnail" src={image.previewUrl} alt={entry.label} />
                      <div className="border-label">{entry.label}</div>
                    </div>
                  ) : null;
                })}
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
}
