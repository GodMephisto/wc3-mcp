---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/utils/glueButton.ts"
ref: HEAD
---

import { useEffect, useRef, useState } from "react";
import { decodeBLP, getBLPImageData } from "war3-model";
import { resolveAssetPath } from "../utils/config";

const iconDataUrlCache = new Map<string, Promise<string>>();
const decodedImageCache = new Map<string, Promise<HTMLImageElement>>();

function shouldForceOpaqueOnZeroAlpha(path: string): boolean {
  return /\/buttons\/esc\/[^/]+\/[^/]+-options-button-background(?:-down|-disabled)?\.blp$/i.test(
    path,
  );
}

function shouldApplyBlackColorKey(path: string): boolean {
  return /\/resources\/Resource(?:Lumber|Supply|Human|Orc|NightElf|Undead)\.blp$/i.test(
    path,
  );
}

export async function loadBlpDataUrl(path: string): Promise<string> {
  const cached = iconDataUrlCache.get(path);
  if (cached) return cached;

  const promise = (async () => {
    const res = await fetch(resolveAssetPath(path));
    if (!res.ok) throw new Error(`Failed to fetch icon ${path}`);
    const ab = await res.arrayBuffer();
    const blp = decodeBLP(ab);
    const img = getBLPImageData(blp, 0);
    const pixelData = new Uint8ClampedArray(img.data);
    if (shouldForceOpaqueOnZeroAlpha(path)) {
      let allAlphaZero = true;
      for (let i = 3; i < pixelData.length; i += 4) {
        if (pixelData[i] !== 0) {
          allAlphaZero = false;
          break;
        }
      }
      if (allAlphaZero) {
        for (let i = 3; i < pixelData.length; i += 4) {
          pixelData[i] = 255;
        }
      }
    }
    if (shouldApplyBlackColorKey(path)) {
      for (let i = 0; i < pixelData.length; i += 4) {
        const r = pixelData[i];
        const g = pixelData[i + 1];
        const b = pixelData[i + 2];
        const a = pixelData[i + 3];
        if (a > 224 && r <= 10 && g <= 10 && b <= 10) {
          pixelData[i + 3] = 0;
        }
      }
    }
    const canvas = document.createElement("canvas");
    canvas.width = img.width;
    canvas.height = img.height;
    const ctx = canvas.getContext("2d");
    if (!ctx) throw new Error(`Failed to decode icon ${path}`);
    ctx.putImageData(new ImageData(pixelData, img.width, img.height), 0, 0);
    return canvas.toDataURL("image/png");
  })();

  iconDataUrlCache.set(path, promise);
  return promise;
}

export async function loadDataUrlImage(url: string): Promise<HTMLImageElement> {
  const cached = decodedImageCache.get(url);
  if (cached) return cached;
  const promise = new Promise<HTMLImageElement>((resolve, reject) => {
    const img = new Image();
    img.onload = () => resolve(img);
    img.onerror = reject;
    img.src = url;
  });
  decodedImageCache.set(url, promise);
  return promise;
}

export function rotateCellClockwise(
  atlas: HTMLImageElement | HTMLCanvasElement,
  cellIndex: number,
  cellW: number,
  cellH: number,
): HTMLCanvasElement {
  const canvas = document.createElement("canvas");
  canvas.width = cellH;
  canvas.height = cellW;
  const ctx = canvas.getContext("2d");
  if (!ctx) return canvas;
  ctx.translate(cellH, 0);
  ctx.rotate(Math.PI / 2);
  ctx.drawImage(atlas, cellIndex * cellW, 0, cellW, cellH, 0, 0, cellW, cellH);
  return canvas;
}

/* ── Template nine-slice (scrollbar / slider / textarea tracks) ── */

export interface NineSliceTemplateOptions {
  cornerPx: number;
  insetPx: number;
  tileBackground?: boolean;
  tileHorizontalEdges?: boolean;
  opaqueBaseFill?: string | null;
  fillCornerMaxPx?: number;
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
    ctx.drawImage(
      tile,
      0,
      0,
      srcW,
      tile.height,
      x + drawn,
      y,
      segmentW,
      height,
    );
    drawn += segmentW;
  }
}

export function drawTemplateNineSliceToCanvas(
  canvas: HTMLCanvasElement,
  bgImg: HTMLImageElement,
  borderImg: HTMLImageElement,
  options: NineSliceTemplateOptions,
) {
  const cssW = Math.max(1, canvas.clientWidth || 1);
  const cssH = Math.max(1, canvas.clientHeight || 1);
  const dpr = window.devicePixelRatio || 1;
  const pxW = Math.max(1, Math.round(cssW * dpr));
  const pxH = Math.max(1, Math.round(cssH * dpr));
  if (canvas.width !== pxW || canvas.height !== pxH) {
    canvas.width = pxW;
    canvas.height = pxH;
  }

  const ctx = canvas.getContext("2d");
  if (!ctx) return;
  ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
  ctx.clearRect(0, 0, cssW, cssH);

  const atlasW = borderImg.width;
  const atlasH = borderImg.height;
  const cellW = atlasW / 8;
  const sourceCornerMax = Math.max(1, Math.floor(Math.min(cellW, atlasH)));
  const corner = Math.max(
    1,
    Math.floor(Math.min(options.cornerPx, sourceCornerMax, cssW / 2, cssH / 2)),
  );
  const inset = Math.max(
    0,
    Math.floor(Math.min(options.insetPx, cssW / 2, cssH / 2)),
  );
  const innerX = inset;
  const innerY = inset;
  const innerW = Math.max(0, cssW - inset * 2);
  const innerH = Math.max(0, cssH - inset * 2);
  const fillCornerCapPx = options.fillCornerMaxPx ?? 8;
  const fillCorner = Math.max(
    0,
    Math.floor(Math.min(corner, fillCornerCapPx, innerW / 2, innerH / 2)),
  );

  if (innerW > 0 && innerH > 0) {
    ctx.save();
    ctx.beginPath();
    if (fillCorner > 0) {
      ctx.roundRect(innerX, innerY, innerW, innerH, fillCorner);
    } else {
      ctx.rect(innerX, innerY, innerW, innerH);
    }
    ctx.clip();

    if (options.opaqueBaseFill) {
      ctx.fillStyle = options.opaqueBaseFill;
      ctx.fillRect(innerX, innerY, innerW, innerH);
    }
    if (options.tileBackground === false) {
      ctx.drawImage(
        bgImg,
        0,
        0,
        bgImg.width,
        bgImg.height,
        innerX,
        innerY,
        innerW,
        innerH,
      );
    } else {
      const pattern = ctx.createPattern(bgImg, "repeat");
      if (pattern) {
        ctx.fillStyle = pattern;
        ctx.fillRect(innerX, innerY, innerW, innerH);
      }
    }
    ctx.restore();
  }

  const drawCell = (
    idx: number,
    x: number,
    y: number,
    w: number,
    h: number,
  ) => {
    ctx.drawImage(borderImg, idx * cellW, 0, cellW, atlasH, x, y, w, h);
  };

  drawCell(4, 0, 0, corner, corner);
  drawCell(5, cssW - corner, 0, corner, corner);
  drawCell(6, 0, cssH - corner, corner, corner);
  drawCell(7, cssW - corner, cssH - corner, corner, corner);

  const edgeH = cssH - corner * 2;
  if (edgeH > 0) {
    drawCell(0, 0, corner, corner, edgeH);
    drawCell(1, cssW - corner, corner, corner, edgeH);
  }

  const edgeW = cssW - corner * 2;
  if (edgeW > 0) {
    const topRot = rotateCellClockwise(borderImg, 2, cellW, atlasH);
    const bottomRot = rotateCellClockwise(borderImg, 3, cellW, atlasH);
    if (options.tileHorizontalEdges) {
      drawHorizontallyTiledEdge(ctx, topRot, corner, 0, edgeW, corner);
      drawHorizontallyTiledEdge(
        ctx,
        bottomRot,
        corner,
        cssH - corner,
        edgeW,
        corner,
      );
    } else {
      ctx.drawImage(topRot, corner, 0, edgeW, corner);
      ctx.drawImage(bottomRot, corner, cssH - corner, edgeW, corner);
    }
  }
}

/* ── Glue nine-slice (buttons) ── */

export function drawGlueNineSliceToCanvas(
  canvas: HTMLCanvasElement,
  bgImg: HTMLImageElement,
  borderImg: HTMLImageElement,
  hoverImg: HTMLImageElement | null,
  hovered: boolean,
) {
  const cssW = Math.max(1, canvas.clientWidth || 1);
  const cssH = Math.max(1, canvas.clientHeight || 1);
  const dpr = window.devicePixelRatio || 1;
  const pxW = Math.max(1, Math.round(cssW * dpr));
  const pxH = Math.max(1, Math.round(cssH * dpr));
  if (canvas.width !== pxW || canvas.height !== pxH) {
    canvas.width = pxW;
    canvas.height = pxH;
  }

  const ctx = canvas.getContext("2d");
  if (!ctx) return;
  ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
  ctx.clearRect(0, 0, cssW, cssH);

  const atlasW = borderImg.width;
  const atlasH = borderImg.height;
  const cellW = atlasW / 8;
  const corner = Math.max(
    1,
    Math.floor(Math.min(cellW, cssH * 0.35, cssW / 2, cssH / 2)),
  );
  const inset = Math.max(1, Math.round(corner * 0.25));

  ctx.fillStyle = "#000";
  ctx.fillRect(inset, inset, cssW - inset * 2, cssH - inset * 2);

  const pattern = ctx.createPattern(bgImg, "repeat");
  if (pattern) {
    ctx.fillStyle = pattern;
    ctx.fillRect(inset, inset, cssW - inset * 2, cssH - inset * 2);
  }

  const drawCell = (
    idx: number,
    x: number,
    y: number,
    w: number,
    h: number,
  ) => {
    ctx.drawImage(borderImg, idx * cellW, 0, cellW, atlasH, x, y, w, h);
  };

  drawCell(4, 0, 0, corner, corner);
  drawCell(5, cssW - corner, 0, corner, corner);
  drawCell(6, 0, cssH - corner, corner, corner);
  drawCell(7, cssW - corner, cssH - corner, corner, corner);

  const edgeH = cssH - corner * 2;
  if (edgeH > 0) {
    drawCell(0, 0, corner, corner, edgeH);
    drawCell(1, cssW - corner, corner, corner, edgeH);
  }

  const edgeW = cssW - corner * 2;
  if (edgeW > 0) {
    const topRot = rotateCellClockwise(borderImg, 2, cellW, atlasH);
    const bottomRot = rotateCellClockwise(borderImg, 3, cellW, atlasH);
    ctx.drawImage(topRot, corner, 0, edgeW, corner);
    ctx.drawImage(bottomRot, corner, cssH - corner, edgeW, corner);
  }

  if (hovered && hoverImg) {
    const temp = document.createElement("canvas");
    temp.width = pxW;
    temp.height = pxH;
    const tCtx = temp.getContext("2d");
    if (tCtx) {
      tCtx.setTransform(dpr, 0, 0, dpr, 0, 0);
      tCtx.drawImage(canvas, 0, 0, cssW, cssH);
      tCtx.globalCompositeOperation = "source-in";
      tCtx.drawImage(hoverImg, 0, 0, cssW, cssH);
      ctx.save();
      ctx.globalCompositeOperation = "lighter";
      ctx.drawImage(temp, 0, 0, cssW, cssH);
      ctx.restore();
    }
  }
}

/**
 * Builds a 3×3 CSS border-image data URL from an 8-cell horizontal atlas
 * (cells 0-3 = edges, 4-7 = corners) — same atlas format used by all WC3 border BLPs.
 */
export function composeBorderImageFromAtlas(atlas: HTMLImageElement): string {
  const cellW = atlas.width / 8;
  const cellH = atlas.height;
  const cell = Math.max(1, Math.floor(Math.min(cellW, cellH)));
  const canvas = document.createElement("canvas");
  canvas.width = cell * 3;
  canvas.height = cell * 3;
  const ctx = canvas.getContext("2d");
  if (!ctx) return "";
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  ctx.imageSmoothingEnabled = false;

  const drawCell = (idx: number, x: number, y: number) => {
    ctx.drawImage(atlas, idx * cellW, 0, cellW, cellH, x, y, cell, cell);
  };

  drawCell(4, 0, 0);
  drawCell(5, cell * 2, 0);
  drawCell(6, 0, cell * 2);
  drawCell(7, cell * 2, cell * 2);
  drawCell(0, 0, cell);
  drawCell(1, cell * 2, cell);

  const top = rotateCellClockwise(atlas, 2, cellW, cellH);
  const bottom = rotateCellClockwise(atlas, 3, cellW, cellH);
  ctx.drawImage(top, cell, 0, cell, cell);
  ctx.drawImage(bottom, cell, cell * 2, cell, cell);

  return canvas.toDataURL("image/png");
}

export interface NineSliceUrls {
  bg: string;
  bgDown: string;
  bgDisabled: string;
  border: string;
  borderDown: string;
  borderDisabled: string;
  hover: string;
}

/**
 * Hook that drives a canvas element with nine-slice rendering and hover/press
 * state. Returns event handlers to spread onto the button element.
 */
export function useNineSliceButton(urls: NineSliceUrls, disabled?: boolean) {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const [hovered, setHovered] = useState(false);
  const [pressed, setPressed] = useState(false);

  useEffect(() => {
    const canvas = canvasRef.current;
    const activeBg = disabled
      ? urls.bgDisabled
      : pressed
        ? urls.bgDown
        : urls.bg;
    const activeBorder = disabled
      ? urls.borderDisabled
      : pressed
        ? urls.borderDown
        : urls.border;
    const hoverActive = hovered && !pressed && !disabled;
    if (!canvas || !activeBg || !activeBorder) return;
    let cancelled = false;

    void Promise.all([
      loadDataUrlImage(activeBg),
      loadDataUrlImage(activeBorder),
      urls.hover && !disabled
        ? loadDataUrlImage(urls.hover)
        : Promise.resolve(null),
    ])
      .then(([bgImg, borderImg, hoverImg]) => {
        if (cancelled) return;
        drawGlueNineSliceToCanvas(
          canvas,
          bgImg,
          borderImg,
          hoverImg,
          hoverActive,
        );
      })
      .catch((err) => console.error(err));

    return () => {
      cancelled = true;
    };
  }, [
    disabled,
    pressed,
    hovered,
    urls.bg,
    urls.bgDown,
    urls.bgDisabled,
    urls.border,
    urls.borderDown,
    urls.borderDisabled,
    urls.hover,
  ]);

  const handlers = disabled
    ? {}
    : {
        onMouseEnter: () => setHovered(true),
        onMouseLeave: () => {
          setHovered(false);
          setPressed(false);
        },
        onMouseDown: () => setPressed(true),
        onMouseUp: () => setPressed(false),
      };

  return { canvasRef, pressed, handlers };
}
