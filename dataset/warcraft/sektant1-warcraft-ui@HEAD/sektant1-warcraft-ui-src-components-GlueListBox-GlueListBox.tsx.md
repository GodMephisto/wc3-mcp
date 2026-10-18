---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/GlueListBox/GlueListBox.tsx"
ref: HEAD
---

import { useEffect, useRef, useState } from "react";
import {
  loadBlpDataUrl,
  loadDataUrlImage,
  drawGlueNineSliceToCanvas,
} from "../../utils/glueButton";
import GlueScrollbar from "../GlueScrollbar/GlueScrollbar";
import "./style.css";

const BG_BLP = "./buttons/editbox/bnet-inputbox-back.blp";
const BORDER_BLP = "./buttons/editbox/bnet-inputbox-border.blp";

interface GlueListBoxProps<T extends string> {
  items: readonly T[];
  value: T | null;
  onChange: (value: T) => void;
  label?: (item: T) => string;
  /** Height of the list box in px. Default: 200 */
  height?: number;
}

export default function GlueListBox<T extends string>({
  items,
  value,
  onChange,
  label,
  height = 200,
}: GlueListBoxProps<T>) {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const listRef = useRef<HTMLDivElement>(null);
  const [scroll, setScroll] = useState(0);

  // Draw background canvas
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
        drawGlueNineSliceToCanvas(canvas, bgImg, borderImg, null, false);
      })
      .catch(console.error);
    return () => {
      cancelled = true;
    };
  }, []);

  // Scrollbar value → list scroll position
  useEffect(() => {
    const el = listRef.current;
    if (!el) return;
    const max = el.scrollHeight - el.clientHeight;
    if (max <= 0) return;
    el.scrollTop = (scroll / 100) * max;
  }, [scroll]);

  // Native scroll → scrollbar value
  const handleScroll = () => {
    const el = listRef.current;
    if (!el) return;
    const max = el.scrollHeight - el.clientHeight;
    if (max <= 0) return;
    setScroll((el.scrollTop / max) * 100);
  };

  return (
    <div className="wc-listbox" style={{ height }}>
      <div className="wc-listbox-body">
        <canvas ref={canvasRef} className="wc-listbox-canvas" />
        <div className="wc-listbox-mask">
          <div
            ref={listRef}
            className="wc-listbox-items"
            role="listbox"
            onScroll={handleScroll}
          >
            {items.map((item) => (
              <div
                key={item}
                role="option"
                aria-selected={value === item}
                className={`wc-listbox-item${value === item ? " wc-listbox-item--selected" : ""}`}
                onClick={() => onChange(item)}
              >
                {label ? label(item) : item}
              </div>
            ))}
          </div>
        </div>
      </div>
      <GlueScrollbar value={scroll} onChange={setScroll} />
    </div>
  );
}
