---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/BnetEditBox/BnetEditBox.tsx"
ref: HEAD
---

import { useEffect, useRef } from "react";
import {
  loadBlpDataUrl,
  loadDataUrlImage,
  drawTemplateNineSliceToCanvas,
} from "../../utils/glueButton";
import "./style.css";

const BG_BLP = "./buttons/editbox/bnet-inputbox-back.blp";
const BORDER_BLP = "./buttons/editbox/bnet-inputbox-border.blp";
const CORNER_RATIO = 0.032 / 0.04;
const INSET_RATIO = 0.004 / 0.04;

interface Props {
  value: string;
  placeholder?: string;
  onChange: (value: string) => void;
  onBlur?: () => void;
}

export default function BnetEditBox({
  value,
  placeholder,
  onChange,
  onBlur,
}: Props) {
  const canvasRef = useRef<HTMLCanvasElement>(null);

  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;
    let cancelled = false;

    void Promise.all([
      loadBlpDataUrl(BG_BLP).then(loadDataUrlImage),
      loadBlpDataUrl(BORDER_BLP).then(loadDataUrlImage),
    ])
      .then(([bgImg, borderImg]) => {
        if (cancelled) return;
        const h = canvas.clientHeight || 1;
        drawTemplateNineSliceToCanvas(canvas, bgImg, borderImg, {
          cornerPx: h * CORNER_RATIO,
          insetPx: h * INSET_RATIO,
          tileBackground: true,
          tileHorizontalEdges: true,
          opaqueBaseFill: "#000",
        });
      })
      .catch((err) => console.error(err));

    return () => {
      cancelled = true;
    };
  }, []);

  return (
    <label className="wc3-editbox wc3-editbox--bnet">
      <canvas ref={canvasRef} className="wc3-editbox-canvas" />
      <input
        type="text"
        value={value}
        placeholder={placeholder}
        onChange={(e) => onChange(e.currentTarget.value)}
        onBlur={onBlur}
      />
    </label>
  );
}
