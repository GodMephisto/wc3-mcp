---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/LoadingBar/LoadingBar.tsx"
ref: HEAD
---

import { useCallback, useRef } from "react";
import { useBlpTextures, useCanvasRenderer } from "../../utils/blpLoader";
import "./style.css";

interface Props {
  progress: number;
}

export default function LoadingBar({ progress }: Props) {
  const canvasRef = useRef<HTMLCanvasElement>(null);

  const tex = useBlpTextures({
    bg: "loading/Loading-BarBackground.blp",
    fill: "loading/Loading-BarFill.blp",
    border: "loading/Loading-BarBorder.blp",
    glass: "loading/Loading-BarGlass.blp",
    glow: "loading/Loading-BarGlow.blp",
  });

  const pct = Math.max(0, Math.min(progress, 100));

  const draw = useCallback(
    (ctx: CanvasRenderingContext2D, w: number, h: number) => {
      if (!tex) return;

      // Tiled background
      const bgPat = ctx.createPattern(tex.bg, "repeat");
      if (bgPat) {
        ctx.fillStyle = bgPat;
        ctx.fillRect(0, 0, w, h);
      }

      // Fill (stretched to progress width)
      const fillW = (w * pct) / 100;
      const inset = h * 0.2;
      if (fillW > 0) {
        ctx.drawImage(
          tex.fill,
          0,
          0,
          tex.fill.width,
          tex.fill.height,
          0,
          inset,
          fillW,
          h - inset * 2,
        );
      }

      // Border overlay
      ctx.drawImage(
        tex.border,
        0,
        0,
        tex.border.width,
        tex.border.height,
        0,
        0,
        w,
        h,
      );

      // Glass overlay
      ctx.save();
      ctx.globalAlpha = 1;
      ctx.drawImage(
        tex.glass,
        0,
        0,
        tex.glass.width,
        tex.glass.height,
        0,
        0,
        w,
        h,
      );
      ctx.restore();

      // Glow at fill edge (additive blend)
      if (fillW > 0 && pct < 100) {
        ctx.save();
        ctx.globalCompositeOperation = "lighter";
        const glowW = Math.min(h * 1.5, w * 0.15);
        const glowX = fillW - glowW / 2;
        ctx.drawImage(
          tex.glow,
          0,
          0,
          tex.glow.width,
          tex.glow.height,
          glowX,
          0,
          glowW,
          h,
        );
        ctx.restore();
      }
    },
    [tex, pct],
  );

  useCanvasRenderer(canvasRef, draw, [tex, pct]);

  return (
    <div className="loading-bar-composite">
      <canvas
        ref={canvasRef}
        style={{
          position: "absolute",
          inset: 0,
          width: "100%",
          height: "100%",
        }}
      />
      <div className="lb-text">{Math.floor(pct)}%</div>
    </div>
  );
}
