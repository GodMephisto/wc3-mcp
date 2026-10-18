---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/GlueScrollbar/GlueScrollbar.tsx"
ref: HEAD
---

import { useCallback, useEffect, useRef, useState } from "react";
import {
  loadBlpDataUrl,
  loadDataUrlImage,
  drawTemplateNineSliceToCanvas,
} from "../../utils/glueButton";
import "./style.css";

const BLP = {
  bg: "./buttons/scrollbar/GlueScreen-Scrollbar-BackdropBackground.blp",
  border: "./buttons/scrollbar/GlueScreen-Scrollbar-BackdropBorder.blp",
  up: "./buttons/scrollbar/GlueScreen-Scrollbar-UpArrow.blp",
  down: "./buttons/scrollbar/GlueScreen-Scrollbar-DownArrow.blp",
  knob: "./buttons/scrollbar/SinglePlayerSkirmish-ScrollBarKnob.blp",
};

const CORNER_RATIO = 0.008 / 0.0165;
const INSET_RATIO = 0.004 / 0.0165;
const THUMB_SIZE = 16;

interface Urls {
  bg: string;
  border: string;
  up: string;
  down: string;
  knob: string;
}

function useBlpUrls(): Urls {
  const [urls, setUrls] = useState<Urls>({
    bg: "",
    border: "",
    up: "",
    down: "",
    knob: "",
  });

  useEffect(() => {
    void Promise.all(Object.values(BLP).map((p) => loadBlpDataUrl(p))).then(
      ([bg, border, up, down, knob]) => {
        setUrls({ bg, border, up, down, knob });
      },
    );
  }, []);

  return urls;
}

function thumbTopPercent(value: number, trackPx: number): string {
  if (trackPx <= 0) return "0%";
  const clamped = Math.max(0, Math.min(100, value));
  const min = THUMB_SIZE * 0.5;
  const max = Math.max(min, trackPx - THUMB_SIZE * 0.5);
  const center = min + ((max - min) * clamped) / 100;
  return `${(center / trackPx) * 100}%`;
}

interface GlueScrollbarProps {
  /** 0–100 scroll percentage */
  value: number;
  onChange: (value: number) => void;
}

export default function GlueScrollbar({ value, onChange }: GlueScrollbarProps) {
  const urls = useBlpUrls();
  const trackRef = useRef<HTMLDivElement>(null);
  const trackCanvasRef = useRef<HTMLCanvasElement>(null);
  const [trackHeight, setTrackHeight] = useState(132);

  // Measure track height
  useEffect(() => {
    const el = trackRef.current;
    if (!el) return;
    const ro = new ResizeObserver((entries) => {
      for (const entry of entries) {
        setTrackHeight(entry.contentRect.height);
      }
    });
    ro.observe(el);
    setTrackHeight(el.clientHeight);
    return () => ro.disconnect();
  }, []);

  // Draw track canvas
  useEffect(() => {
    const canvas = trackCanvasRef.current;
    if (!canvas || !urls.bg || !urls.border) return;
    let cancelled = false;

    void Promise.all([loadDataUrlImage(urls.bg), loadDataUrlImage(urls.border)])
      .then(([bgImg, borderImg]) => {
        if (cancelled) return;
        const w = canvas.clientWidth || 1;
        drawTemplateNineSliceToCanvas(canvas, bgImg, borderImg, {
          cornerPx: w * CORNER_RATIO,
          insetPx: w * INSET_RATIO,
          tileBackground: true,
          opaqueBaseFill: "#000",
        });
      })
      .catch((err) => console.error(err));

    return () => {
      cancelled = true;
    };
  }, [urls.bg, urls.border, trackHeight]);

  const bump = useCallback(
    (delta: number) => {
      onChange(Math.max(0, Math.min(100, value + delta)));
    },
    [value, onChange],
  );

  return (
    <div className="glue-scrollbar-demo">
      <button
        type="button"
        className="glue-scroll-arrow"
        aria-label="Scroll up"
        onClick={() => bump(-8)}
      >
        {urls.up && <img src={urls.up} alt="" />}
      </button>
      <div className="glue-scroll-track" ref={trackRef}>
        <canvas ref={trackCanvasRef} className="glue-scroll-track-canvas" />
        <div
          className="glue-scroll-thumb"
          style={{
            backgroundImage: urls.knob ? `url("${urls.knob}")` : undefined,
            top: thumbTopPercent(value, trackHeight),
          }}
        />
        <input
          type="range"
          min="0"
          max="100"
          value={value}
          aria-label="Scroll position"
          onChange={(e) => onChange(Number(e.currentTarget.value))}
        />
      </div>
      <button
        type="button"
        className="glue-scroll-arrow"
        aria-label="Scroll down"
        onClick={() => bump(8)}
      >
        {urls.down && <img src={urls.down} alt="" />}
      </button>
    </div>
  );
}
