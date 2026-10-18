---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/EscRadioButton/EscRadioButton.tsx"
ref: HEAD
---

import { useCallback, useRef } from "react";
import { useBlpTextures, useCanvasRenderer } from "../../utils/blpLoader";
import "./style.css";

interface Props {
  label: string;
  selected: boolean;
  onSelect: () => void;
}

export default function EscRadioButton({ label, selected, onSelect }: Props) {
  const canvasRef = useRef<HTMLCanvasElement>(null);

  const tex = useBlpTextures({
    bg: "buttons/radio/radiobutton-background.blp",
    dot: "buttons/radio/radiobutton-button.blp",
  });

  const draw = useCallback(
    (ctx: CanvasRenderingContext2D, w: number, h: number) => {
      if (!tex) return;
      ctx.drawImage(tex.bg, 0, 0, w, h);
      if (selected) {
        ctx.drawImage(tex.dot, 0, 0, w, h);
      }
    },
    [tex, selected],
  );

  useCanvasRenderer(canvasRef, draw, [tex, selected]);

  return (
    <div
      className={`wc-radio${selected ? " wc-radio--selected" : ""}`}
      role="radio"
      aria-checked={selected}
      tabIndex={0}
      onClick={onSelect}
      onKeyDown={(e) => e.key === " " && onSelect()}
    >
      <canvas ref={canvasRef} className="wc-radio-visual" />
      <span className="wc-radio-label">{label}</span>
    </div>
  );
}
