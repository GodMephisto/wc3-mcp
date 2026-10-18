---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/EscSlider/EscSlider.tsx"
ref: HEAD
---

import { useCallback, useEffect, useRef, useState } from "react";
import { useBlpTextures, useCanvasRenderer } from "../../utils/blpLoader";
import { rotateCellClockwise } from "../../utils/glueButton";
import { useCurrentRace, RACE_PREFIXES } from "../../state/race";
import "./style.css";

interface Props {
  label?: string;
  value?: number;
  onChange?: (value: number) => void;
  defaultValue?: number;
  race?: import("../../utils/types").Race;
}

const TRACK_H = 20;

export default function EscSlider({
  label,
  value: controlled,
  onChange,
  defaultValue = 0.5,
  race: raceProp,
}: Props) {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const wrapRef = useRef<HTMLDivElement>(null);
  const [internal, setInternal] = useState(controlled ?? defaultValue);
  const dragging = useRef(false);
  const globalRace = useCurrentRace();
  const race = raceProp ?? globalRace;
  const rp = RACE_PREFIXES[race];

  const isControlled = controlled !== undefined;
  const value = isControlled ? controlled : internal;

  const knobPath =
    race === "Human"
      ? "buttons/slider/slider-knob.blp"
      : `buttons/slider/${rp.esc}-slider-knob.blp`;

  const tex = useBlpTextures({
    bg: "buttons/slider/slider-background.blp",
    border: "buttons/slider/slider-border.blp",
    knob: knobPath,
  });

  const draw = useCallback(
    (ctx: CanvasRenderingContext2D, w: number, h: number) => {
      if (!tex) return;

      const trackY = Math.floor((h - TRACK_H) / 2);

      // Fill track background (tiled pattern)
      ctx.save();
      ctx.beginPath();
      ctx.rect(0, trackY, w, TRACK_H);
      ctx.clip();
      ctx.fillStyle = "#000";
      ctx.fillRect(0, trackY, w, TRACK_H);
      const pattern = ctx.createPattern(tex.bg, "repeat");
      if (pattern) {
        ctx.fillStyle = pattern;
        ctx.fillRect(0, trackY, w, TRACK_H);
      }
      ctx.restore();

      // Nine-slice border on the track rect
      const atlasW = tex.border.width;
      const atlasH = tex.border.height;
      const cellW = atlasW / 8;
      const corner = Math.max(
        1,
        Math.floor(Math.min(cellW, TRACK_H * 0.35, w / 2, TRACK_H / 2)),
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

      // Corners
      drawCell(4, 0, trackY, corner, corner);
      drawCell(5, w - corner, trackY, corner, corner);
      drawCell(6, 0, trackY + TRACK_H - corner, corner, corner);
      drawCell(7, w - corner, trackY + TRACK_H - corner, corner, corner);

      // Left / right vertical edges
      const edgeH = TRACK_H - corner * 2;
      if (edgeH > 0) {
        drawCell(0, 0, trackY + corner, corner, edgeH);
        drawCell(1, w - corner, trackY + corner, corner, edgeH);
      }

      // Top / bottom horizontal edges (rotated cells)
      const edgeW = w - corner * 2;
      if (edgeW > 0) {
        const topRot = rotateCellClockwise(tex.border, 2, cellW, atlasH);
        const botRot = rotateCellClockwise(tex.border, 3, cellW, atlasH);
        ctx.drawImage(topRot, corner, trackY, edgeW, corner);
        ctx.drawImage(botRot, corner, trackY + TRACK_H - corner, edgeW, corner);
      }

      // Knob (centered on the full canvas height, preserving aspect ratio)
      const knobH = 28;
      const knobW = knobH * (tex.knob.width / tex.knob.height);
      const knobX = value * w - knobW / 2;
      const knobY = h / 2 - knobH / 2;
      ctx.drawImage(tex.knob, knobX, knobY, knobW, knobH);
    },
    [tex, value],
  );

  useCanvasRenderer(canvasRef, draw, [tex, value]);

  useEffect(() => {
    const el = wrapRef.current;
    if (!el) return;

    const updateFromEvent = (clientX: number) => {
      const rect = el.getBoundingClientRect();
      const next = Math.max(0, Math.min(1, (clientX - rect.left) / rect.width));
      if (!isControlled) setInternal(next);
      onChange?.(next);
    };

    const onMove = (e: MouseEvent | TouchEvent) => {
      if (!dragging.current) return;
      e.preventDefault();
      const x = "touches" in e ? e.touches[0].clientX : e.clientX;
      updateFromEvent(x);
    };

    const onUp = () => {
      dragging.current = false;
      document.removeEventListener("mousemove", onMove);
      document.removeEventListener("mouseup", onUp);
      document.removeEventListener("touchmove", onMove);
      document.removeEventListener("touchend", onUp);
    };

    const onDown = (e: MouseEvent | TouchEvent) => {
      e.preventDefault();
      dragging.current = true;
      const x = "touches" in e ? e.touches[0].clientX : e.clientX;
      updateFromEvent(x);
      document.addEventListener("mousemove", onMove);
      document.addEventListener("mouseup", onUp);
      document.addEventListener("touchmove", onMove, { passive: false });
      document.addEventListener("touchend", onUp);
    };

    el.addEventListener("mousedown", onDown);
    el.addEventListener("touchstart", onDown, { passive: false });
    return () => {
      el.removeEventListener("mousedown", onDown);
      el.removeEventListener("touchstart", onDown);
      document.removeEventListener("mousemove", onMove);
      document.removeEventListener("mouseup", onUp);
      document.removeEventListener("touchmove", onMove);
      document.removeEventListener("touchend", onUp);
    };
  }, [isControlled, onChange]);

  return (
    <div className="wc-slider-row">
      {label && <span className="wc-slider-label">{label}</span>}
      <div ref={wrapRef} className="wc-slider">
        <canvas
          ref={canvasRef}
          className="wc-slider-knob"
          style={{ width: "100%", height: "100%", display: "block" }}
        />
      </div>
    </div>
  );
}
