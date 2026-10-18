---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/utils/blpLoader.ts"
ref: HEAD
---

import { useEffect, useRef, useState, type RefObject } from "react";
import { decodeBLP, getBLPImageData } from "war3-model";
import { resolveAssetPath } from "./config";

type CanvasSource = HTMLCanvasElement;

const cache = new Map<string, Promise<CanvasSource>>();

function applyAlphaFixes(path: string, pixels: Uint8ClampedArray) {
  // Black color key for resource icons
  if (/\/resources\/Resource/.test(path)) {
    for (let i = 0; i < pixels.length; i += 4) {
      if (
        pixels[i + 3] > 224 &&
        pixels[i] <= 10 &&
        pixels[i + 1] <= 10 &&
        pixels[i + 2] <= 10
      ) {
        pixels[i + 3] = 0;
      }
    }
  }
  // Force opaque on fully-transparent ESC button backgrounds
  if (
    /\/buttons\/esc\/[^/]+\/[^/]+-options-(?:button-background|menu-background)(?:-down|-disabled)?\.blp$/i.test(
      path,
    )
  ) {
    let allZero = true;
    for (let i = 3; i < pixels.length; i += 4) {
      if (pixels[i] !== 0) {
        allZero = false;
        break;
      }
    }
    if (allZero) {
      for (let i = 3; i < pixels.length; i += 4) pixels[i] = 255;
    }
  }
}

export function loadBlpCanvas(path: string): Promise<CanvasSource> {
  const cached = cache.get(path);
  if (cached) return cached;

  const promise = (async () => {
    const res = await fetch(resolveAssetPath(path));
    if (!res.ok) throw new Error(`Failed to fetch ${path}`);
    const ab = await res.arrayBuffer();
    const blp = decodeBLP(ab);
    const imgData = getBLPImageData(blp, 0);
    const pixels = new Uint8ClampedArray(imgData.data);
    applyAlphaFixes(path, pixels);

    const canvas = document.createElement("canvas");
    canvas.width = imgData.width;
    canvas.height = imgData.height;
    const ctx = canvas.getContext("2d")!;
    ctx.putImageData(
      new ImageData(pixels, imgData.width, imgData.height),
      0,
      0,
    );
    return canvas;
  })();

  cache.set(path, promise);
  return promise;
}

export function useBlpTextures<K extends string>(
  paths: Record<K, string>,
): Record<K, CanvasSource> | null {
  const [textures, setTextures] = useState<Record<K, CanvasSource> | null>(
    null,
  );
  const pathsKey = Object.values<string>(paths).join("\n");

  useEffect(() => {
    let cancelled = false;
    const keys = Object.keys(paths) as K[];
    Promise.all(keys.map((k) => loadBlpCanvas(paths[k])))
      .then((canvases) => {
        if (cancelled) return;
        const result = {} as Record<K, CanvasSource>;
        keys.forEach((k, i) => {
          result[k] = canvases[i];
        });
        setTextures(result);
      })
      .catch(console.error);
    return () => {
      cancelled = true;
    };
  }, [pathsKey]);

  return textures;
}

export function useCanvasRenderer(
  canvasRef: RefObject<HTMLCanvasElement | null>,
  draw: (ctx: CanvasRenderingContext2D, w: number, h: number) => void,
  deps: unknown[],
) {
  const drawRef = useRef(draw);
  drawRef.current = draw;

  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;

    const render = () => {
      const cssW = canvas.clientWidth;
      const cssH = canvas.clientHeight;
      if (cssW <= 0 || cssH <= 0) return;
      const dpr = window.devicePixelRatio || 1;
      const pxW = Math.round(cssW * dpr);
      const pxH = Math.round(cssH * dpr);
      if (canvas.width !== pxW || canvas.height !== pxH) {
        canvas.width = pxW;
        canvas.height = pxH;
      }
      const ctx = canvas.getContext("2d");
      if (!ctx) return;
      ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
      ctx.clearRect(0, 0, cssW, cssH);
      drawRef.current(ctx, cssW, cssH);
    };

    render();
    const ro = new ResizeObserver(render);
    ro.observe(canvas);
    return () => ro.disconnect();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, deps);
}
