---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/StatBar/StatBar.tsx"
ref: HEAD
---

import { useCallback, useRef } from "react";
import { useBlpTextures, useCanvasRenderer } from "../../utils/blpLoader";
import { useCurrentRace, RACE_PREFIXES } from "../../state/race";
import "./style.css";

interface Props {
  label?: string;
  type: "health" | "mana" | "xp" | "build";
  fillPercent: number;
  maxValue?: number;
  height?: number;
  hasBorder?: boolean;
  race?: import("../../utils/types").Race;
}

// WC3 model fill profiles — derived from geoset widths and scale keys
const UNIT_BAR_PROFILE = {
  fillToFrameWidth: 0.02793 / 0.04065,
  minScaleX: 0.037246,
  maxScaleX: 1.44899,
};

const COMPACT_BAR_PROFILE = {
  fillToFrameWidth: 0.26408 / 0.17776,
  minScaleX: 0.014839,
  maxScaleX: 0.17776 / 0.26408,
};

const BUILD_BAR_PROFILE = {
  fillToFrameWidth: 0.26413 / 0.10538,
  minScaleX: 0.014839,
  maxScaleX: 0.389779,
};

function modelBarFillRatio(
  valuePct: number,
  profile: { fillToFrameWidth: number; minScaleX: number; maxScaleX: number },
): number {
  const pct = Math.max(0, Math.min(100, valuePct));
  const scaleX =
    profile.minScaleX + ((profile.maxScaleX - profile.minScaleX) * pct) / 100;
  return Math.max(0, Math.min(1, profile.fillToFrameWidth * scaleX));
}

function mixRgb(
  a: [number, number, number],
  b: [number, number, number],
  t: number,
): [number, number, number] {
  const c = Math.max(0, Math.min(1, t));
  return [
    Math.round(a[0] + (b[0] - a[0]) * c),
    Math.round(a[1] + (b[1] - a[1]) * c),
    Math.round(a[2] + (b[2] - a[2]) * c),
  ];
}

function rgbHex([r, g, b]: [number, number, number]): string {
  return `#${[r, g, b].map((c) => c.toString(16).padStart(2, "0")).join("")}`;
}

function healthBarTint(pct: number): string {
  const p = Math.max(0, Math.min(100, pct));
  const low: [number, number, number] = [220, 42, 22];
  const mid: [number, number, number] = [238, 201, 46];
  const high: [number, number, number] = [67, 213, 61];
  if (p <= 35) return rgbHex(mixRgb(low, mid, p / 35));
  return rgbHex(mixRgb(mid, high, (p - 35) / 65));
}

const FILL_TINTS: Record<string, string | ((pct: number) => string)> = {
  health: healthBarTint,
  mana: "#2f4fd4",
  xp: "#7f49b5",
  build: "",
};

export default function StatBar({
  label,
  type,
  fillPercent,
  maxValue,
  height = 18,
  hasBorder: _ = false,
  race: raceProp,
}: Props) {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const globalRace = useCurrentRace();
  const race = raceProp ?? globalRace;
  const rp = RACE_PREFIXES[race];

  // Health/mana use statbar-color.blp + statbar-edge.blp + statbar-highlight.blp
  // XP uses bigbar-fill.blp + xpbar-border.blp + statbar-highlight.blp
  // Build uses buildprogressbar-fill.blp + buildprogressbar-border.blp + statbar-highlight.blp
  const isUnit = type === "health" || type === "mana";

  const paths: Record<string, string> = {
    highlight: `bars/${rp.lower}-statbar-highlight.blp`,
  };

  if (isUnit) {
    paths.fill = `bars/${rp.lower}-statbar-color.blp`;
    paths.border = `bars/${rp.lower}-statbar-edge.blp`;
  } else if (type === "xp") {
    paths.fill = `bars/${rp.lower}-bigbar-fill.blp`;
    paths.border = `bars/${rp.lower}-xpbar-border.blp`;
  } else {
    paths.fill = `bars/${rp.lower}-buildprogressbar-fill.blp`;
    paths.border = `bars/${rp.lower}-buildprogressbar-border.blp`;
  }

  const tex = useBlpTextures(paths);

  const draw = useCallback(
    (ctx: CanvasRenderingContext2D, w: number, h: number) => {
      if (!tex) return;

      // Choose fill profile
      const profile = isUnit
        ? UNIT_BAR_PROFILE
        : type === "build"
          ? BUILD_BAR_PROFILE
          : COMPACT_BAR_PROFILE;

      const fillRatio = modelBarFillRatio(fillPercent, profile);
      const fillW = w * fillRatio;

      // Draw fill (stretched, not tiled)
      if (fillW > 0) {
        ctx.drawImage(tex.fill, 0, 0, tex.fill.width, tex.fill.height, 0, 0, fillW, h);

        // Apply tint via multiply composite
        const tintSpec = FILL_TINTS[type];
        const tint =
          typeof tintSpec === "function" ? tintSpec(fillPercent) : tintSpec;
        if (tint) {
          ctx.save();
          ctx.globalCompositeOperation = "multiply";
          ctx.fillStyle = tint;
          ctx.fillRect(0, 0, fillW, h);
          ctx.restore();
        }
      }

      // Highlight overlay (additive blend)
      if (tex.highlight) {
        ctx.save();
        ctx.globalCompositeOperation = "lighter";
        ctx.drawImage(tex.highlight, 0, 0, tex.highlight.width, tex.highlight.height, 0, 0, w, h);
        ctx.restore();
      }

      // Border overlay
      ctx.drawImage(tex.border, 0, 0, tex.border.width, tex.border.height, 0, 0, w, h);
    },
    [tex, fillPercent, type, isUnit],
  );

  useCanvasRenderer(canvasRef, draw, [tex, fillPercent, type]);

  const barText = () => {
    const pct = Math.round(fillPercent * 10) / 10;
    if (type === "build") return pct + "%";
    if (maxValue) {
      return (
        Math.floor((maxValue * fillPercent) / 100) + "\u00a0/ " + maxValue
      );
    }
    return pct + "%";
  };

  return (
    <div className="wc-bar-row">
      {label && <span className="wc-bar-label">{label}</span>}
      <div className="wc-stat-bar" style={{ height: height + "px" }}>
        <canvas
          ref={canvasRef}
          style={{
            position: "absolute",
            inset: 0,
            width: "100%",
            height: "100%",
          }}
        />
        <div
          className="wc-bar-text"
          style={height < 18 ? { fontSize: "0.6rem" } : undefined}
        >
          {barText()}
        </div>
      </div>
    </div>
  );
}
