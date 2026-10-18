---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/sections/HeroSection.tsx"
ref: HEAD
---

import { useEffect, useState, type ReactNode } from "react";
import { loadBlpDataUrl } from "../../utils/glueButton";
import ItemModel from "../ItemModel";
import { FEATURE_ITEMS } from "../../lib/featureItems";

interface Props {
  /** Custom CTA content. Omit → renders default canvas button. Pass null → no CTA. */
  actions?: ReactNode | null;
  getStartedPressed?: boolean;
  getStartedCanvasRef?: React.RefObject<HTMLCanvasElement | null>;
  installCanvasRef?: React.RefObject<HTMLCanvasElement | null>;
  onGetStartedMouseEnter?: () => void;
  onGetStartedMouseLeave?: () => void;
  onGetStartedMouseDown?: () => void;
  onGetStartedMouseUp?: () => void;
  onGetStartedClick?: () => void;
}

const RACE_ICONS: { label: string; path: string }[] = [
  { label: "Human", path: "./resources/ResourceHuman.blp" },
  { label: "Orc", path: "./resources/ResourceOrc.blp" },
  { label: "Night Elf", path: "./resources/ResourceNightElf.blp" },
  { label: "Undead", path: "./resources/ResourceUndead.blp" },
];

function RaceBadge({ label, path }: { label: string; path: string }) {
  const [iconUrl, setIconUrl] = useState("");

  useEffect(() => {
    void loadBlpDataUrl(path)
      .then(setIconUrl)
      .catch(() => {});
  }, [path]);

  return (
    <div className="hero-race-badge">
      {iconUrl && <img src={iconUrl} alt="" className="hero-race-badge-icon" />}
      <span>{label}</span>
    </div>
  );
}

const FEATURE_LABELS = [
  { id: "timer", label: "Step Timer" },
  { id: "sound", label: "Sound Cues" },
  { id: "import", label: "Build Orders" },
  { id: "skill", label: "Skill Tracker" },
];

export default function HeroSection(props: Props) {
  return (
    <section className="hero-section">
      <div className="hero-bg-glow" aria-hidden="true" />
      <div className="hero-content">
        <img src="./logo.png" alt="WC3BUILDS" className="hero-logo" />
        <p className="hero-subtitle">
          Build order guide for Warcraft&nbsp;III Practice.
        </p>
        <p className="hero-description">
          Step-by-step build orders for all four races with a{" "}
          <strong>live timer</strong>, <strong>sound cues</strong> for each
          step, <strong>skill order tracker</strong>, and{" "}
          <strong>import support</strong> for custom builds. Follow along in
          real time as you practice.
        </p>

        <div className="hero-features">
          {FEATURE_LABELS.map((f) => {
            const itemCfg = FEATURE_ITEMS.find((i) => i.id === f.id);
            const hasModel = !!itemCfg?.modelPath;
            return (
              <div key={f.id} className="hero-feature-tile">
                {hasModel ? (
                  <div className="hero-feature-model-wrap">
                    <ItemModel
                      modelPath={itemCfg!.modelPath}
                      textures={itemCfg!.textures}
                    />
                  </div>
                ) : (
                  <span className="hero-feature-value" aria-hidden="true">
                    {itemCfg?.fallbackChar ?? "\u25a0"}
                  </span>
                )}
                <span className="hero-feature-label">{f.label}</span>
              </div>
            );
          })}
        </div>

        <div className="hero-races">
          {RACE_ICONS.map((r) => (
            <RaceBadge key={r.label} label={r.label} path={r.path} />
          ))}
        </div>

        {props.actions === undefined ? (
          <div className="hero-cta-row hero-cta-row--components">
            <button
              type="button"
              className="esc-option-preview hero-get-started-btn"
              onMouseEnter={props.onGetStartedMouseEnter}
              onMouseLeave={props.onGetStartedMouseLeave}
              onMouseDown={props.onGetStartedMouseDown}
              onMouseUp={props.onGetStartedMouseUp}
              onClick={props.onGetStartedClick}
            >
              <canvas
                ref={props.getStartedCanvasRef}
                className="esc-option-canvas"
              />
              <span
                className="esc-option-label"
                style={{
                  transform: props.getStartedPressed
                    ? "translate(2px, 2px)"
                    : "none",
                }}
              >
                {"See\u00A0"}
                <span className="button-hotkey">B</span>uilds
              </span>
            </button>
          </div>
        ) : props.actions != null ? (
          <div className="hero-cta-row">{props.actions}</div>
        ) : null}

        <div className="hero-footer">
          <div className="hero-footer-divider" />
          <p className="hero-footer-quote">
            "Let them come, Frostmourne hungers..."
          </p>
          <span className="hero-footer-version">v0.2 — Reign of Builds</span>
        </div>
      </div>
    </section>
  );
}
