---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/GlueDropdown/GlueDropdown.tsx"
ref: HEAD
---

import { useEffect, useRef, useState } from "react";
import {
  loadBlpDataUrl,
  loadDataUrlImage,
  drawGlueNineSliceToCanvas,
} from "../../utils/glueButton";
import "./style.css";

const BLP = {
  bg: "./buttons/glue/GlueScreen-Button1-BackdropBackground.blp",
  bgDown: "./buttons/glue/GlueScreen-Button1-BackdropBackground-Down.blp",
  border: "./buttons/glue/GlueScreen-Button1-BorderedBackdropBorder.blp",
  borderDown:
    "./buttons/glue/GlueScreen-Button1-BorderedBackdropBorder-Down.blp",
  hover: "./buttons/glue/bnet-button01-highlight-mouse.blp",
  popupBg: "./buttons/popup/GlueScreen-Pulldown-BackdropBackground.blp",
  arrow: "./buttons/popup/GlueScreen-Pulldown-Arrow.blp",
};

interface Urls {
  bg: string;
  bgDown: string;
  border: string;
  borderDown: string;
  hover: string;
  popupBg: string;
  arrow: string;
}

function useBlpUrls(): Urls {
  const [urls, setUrls] = useState<Urls>({
    bg: "",
    bgDown: "",
    border: "",
    borderDown: "",
    hover: "",
    popupBg: "",
    arrow: "",
  });

  useEffect(() => {
    void Promise.all(Object.values(BLP).map((p) => loadBlpDataUrl(p))).then(
      ([bg, bgDown, border, borderDown, hover, popupBg, arrow]) => {
        setUrls({ bg, bgDown, border, borderDown, hover, popupBg, arrow });
      },
    );
  }, []);

  return urls;
}

interface GlueDropdownProps<T extends string> {
  options: readonly T[];
  value: T;
  onChange: (value: T) => void;
  /** Optional formatter for display text */
  label?: (value: T) => string;
}

export default function GlueDropdown<T extends string>({
  options,
  value,
  onChange,
  label,
}: GlueDropdownProps<T>) {
  const urls = useBlpUrls();
  const triggerCanvasRef = useRef<HTMLCanvasElement>(null);
  const menuCanvasRef = useRef<HTMLCanvasElement>(null);
  const anchorRef = useRef<HTMLDivElement>(null);

  const [open, setOpen] = useState(false);
  const [hovered, setHovered] = useState(false);

  // Close when clicking outside
  useEffect(() => {
    if (!open) return;
    const handler = (e: MouseEvent) => {
      if (anchorRef.current && !anchorRef.current.contains(e.target as Node)) {
        setOpen(false);
      }
    };
    document.addEventListener("mousedown", handler);
    return () => document.removeEventListener("mousedown", handler);
  }, [open]);

  // Draw trigger button canvas
  useEffect(() => {
    const canvas = triggerCanvasRef.current;
    const bgUrl = open ? urls.bgDown : urls.bg;
    const borderUrl = open ? urls.borderDown : urls.border;
    const hoverActive = hovered && !open;
    if (!canvas || !bgUrl || !borderUrl) return;
    let cancelled = false;

    void Promise.all([
      loadDataUrlImage(bgUrl),
      loadDataUrlImage(borderUrl),
      urls.hover ? loadDataUrlImage(urls.hover) : Promise.resolve(null),
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
    open,
    hovered,
    urls.bg,
    urls.bgDown,
    urls.border,
    urls.borderDown,
    urls.hover,
  ]);

  // Draw menu canvas (delayed by rAF to ensure layout is computed)
  useEffect(() => {
    const canvas = menuCanvasRef.current;
    const bgUrl = urls.popupBg || urls.bg;
    const borderUrl = urls.border;
    if (!open || !canvas || !bgUrl || !borderUrl) return;
    let cancelled = false;

    const rafId = requestAnimationFrame(() => {
      void Promise.all([loadDataUrlImage(bgUrl), loadDataUrlImage(borderUrl)])
        .then(([bgImg, borderImg]) => {
          if (cancelled) return;
          drawGlueNineSliceToCanvas(canvas, bgImg, borderImg, null, false);
        })
        .catch((err) => console.error(err));
    });

    return () => {
      cancelled = true;
      cancelAnimationFrame(rafId);
    };
  }, [open, urls.popupBg, urls.bg, urls.border]);

  const displayText = label ? label(value) : value;

  return (
    <div className="popup-trigger-anchor" ref={anchorRef}>
      <button
        type="button"
        className="popup-trigger-button"
        aria-haspopup="listbox"
        aria-expanded={open}
        onMouseEnter={() => setHovered(true)}
        onMouseLeave={() => setHovered(false)}
        onClick={() => setOpen((prev) => !prev)}
      >
        <canvas ref={triggerCanvasRef} className="popup-trigger-canvas" />
        <span className="popup-title-text">{displayText}</span>
        {urls.arrow && (
          <img src={urls.arrow} alt="" className="popup-arrow-icon" />
        )}
      </button>
      {open && (
        <div className="popup-menu-demo" aria-label="Build order selector">
          <canvas ref={menuCanvasRef} className="popup-menu-canvas" />
          {options.map((option) => (
            <div
              key={option}
              role="option"
              aria-selected={value === option}
              className={`popup-menu-option${value === option ? " selected" : ""}`}
              onClick={() => {
                onChange(option);
                setOpen(false);
              }}
            >
              {label ? label(option) : option}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
