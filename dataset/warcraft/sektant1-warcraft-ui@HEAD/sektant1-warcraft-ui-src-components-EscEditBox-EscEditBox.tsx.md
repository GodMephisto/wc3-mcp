---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/EscEditBox/EscEditBox.tsx"
ref: HEAD
---

import { useCallback } from "react";
import { useBlpTextures, useCanvasRenderer } from "../../utils/blpLoader";
import { rotateCellClockwise } from "../../utils/glueButton";
import { useRef } from "react";
import "./style.css";

interface Props {
  value: string;
  placeholder?: string;
  onChange: (value: string) => void;
}

export default function EscEditBox({ value, placeholder, onChange }: Props) {
  const canvasRef = useRef<HTMLCanvasElement>(null);

  const tex = useBlpTextures({
    bg: "borders/esc/editbox-background.blp",
    border: "borders/esc/editbox-border.blp",
  });

  const draw = useCallback(
    (ctx: CanvasRenderingContext2D, w: number, h: number) => {
      if (!tex) return;

      // Fill background (tiled)
      ctx.fillStyle = "#000";
      ctx.fillRect(0, 0, w, h);
      const pattern = ctx.createPattern(tex.bg, "repeat");
      if (pattern) {
        ctx.fillStyle = pattern;
        ctx.fillRect(0, 0, w, h);
      }

      // Nine-slice border
      const atlasW = tex.border.width;
      const atlasH = tex.border.height;
      const cellW = atlasW / 8;
      const corner = Math.max(
        1,
        Math.floor(Math.min(cellW, h * 0.35, w / 2, h / 2)),
      );

      const drawCell = (
        idx: number,
        x: number,
        y: number,
        cw: number,
        ch: number,
      ) => {
        if (cw <= 0 || ch <= 0) return;
        ctx.drawImage(tex.border, idx * cellW, 0, cellW, atlasH, x, y, cw, ch);
      };

      drawCell(4, 0, 0, corner, corner);
      drawCell(5, w - corner, 0, corner, corner);
      drawCell(6, 0, h - corner, corner, corner);
      drawCell(7, w - corner, h - corner, corner, corner);

      const edgeH = h - corner * 2;
      if (edgeH > 0) {
        drawCell(0, 0, corner, corner, edgeH);
        drawCell(1, w - corner, corner, corner, edgeH);
      }

      const edgeW = w - corner * 2;
      if (edgeW > 0) {
        const topRot = rotateCellClockwise(tex.border, 2, cellW, atlasH);
        const botRot = rotateCellClockwise(tex.border, 3, cellW, atlasH);
        ctx.drawImage(topRot, corner, 0, edgeW, corner);
        ctx.drawImage(botRot, corner, h - corner, edgeW, corner);
      }
    },
    [tex],
  );

  useCanvasRenderer(canvasRef, draw, [tex]);

  return (
    <label className="wc3-editbox wc3-editbox--esc">
      <canvas
        ref={canvasRef}
        className="wc3-editbox-canvas"
        style={{
          position: "absolute",
          inset: 0,
          width: "100%",
          height: "100%",
          pointerEvents: "none",
        }}
      />
      <input
        type="text"
        value={value}
        placeholder={placeholder}
        onChange={(e) => onChange(e.currentTarget.value)}
        style={{ position: "relative", zIndex: 1 }}
      />
    </label>
  );
}
