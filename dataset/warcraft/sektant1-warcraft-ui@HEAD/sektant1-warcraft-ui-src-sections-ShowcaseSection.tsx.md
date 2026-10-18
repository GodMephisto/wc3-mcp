---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/sections/ShowcaseSection.tsx"
ref: HEAD
---

import { useEffect, useRef, type ReactNode } from 'react';
import { RACE_PREFIXES, useCurrentRace } from '../../state/race';
import HeroPortraitModel from '../HeroPortraitModel';
import { getDefaultHero } from '../../lib/heroConfig';

export function ButtonsSection(props: { children: ReactNode }) {
  return (
    <section className="section-card">
      <h3>Buttons</h3>
      <p className="section-desc">
        Beveled, gold-trimmed buttons with faction-aware theming and glow states. Your design system has a primary button. Ours has four.
      </p>
      {props.children}
    </section>
  );
}

export function ControlsSection(props: { children: ReactNode }) {
  return (
    <section className="section-card">
      <h3>Controls</h3>
      <p className="section-desc">
        Checkboxes, sliders, radio buttons, and edit fields. Every form deserves the production value of an options menu.
      </p>
      {props.children}
    </section>
  );
}

export function ButtonShowcaseSection(props: { title: string; description: string; children: ReactNode }) {
  return (
    <section className="button-showcase-section">
      <h4 className="button-showcase-title">{props.title}</h4>
      <p className="button-showcase-desc">{props.description}</p>
      {props.children}
    </section>
  );
}

export function ResourceCounterSection(props: { children: ReactNode }) {
  return (
    <section className="section-card">
      <h3>Resource Counters</h3>
      <p className="section-desc">
        Animated numeric display with icon slot. For when a number needs to feel important. Ticks up smoothly like gold flowing into your treasury.
      </p>
      {props.children}
    </section>
  );
}

type CommandCardIconKey =
  | 'windWalk'
  | 'mirrorImage'
  | 'criticalStrike'
  | 'bladestorm'
  | 'skillz'
  | 'attack'
  | 'move'
  | 'stop'
  | 'holdPosition'
  | 'patrol';

interface CommandCardSectionProps {
  iconUrls?: Partial<Record<CommandCardIconKey, string>>;
  disabledIconUrls?: Partial<Record<CommandCardIconKey, string>>;
  highlightUrl?: string;
  groupBorderUrl?: string;
  subgroupBgUrl?: string;
}

export function CommandCardSection(props: CommandCardSectionProps) {
  const slots = [
    { hotkey: 'Q', label: 'Move', iconKey: 'move' as const, state: 'ready' as const },
    { hotkey: 'W', label: 'Stop', iconKey: 'stop' as const, state: 'ready' as const },
    { hotkey: 'E', label: 'Hold Position', iconKey: 'holdPosition' as const, state: 'ready' as const },
    { hotkey: 'R', label: 'Attack', iconKey: 'attack' as const, state: 'ready' as const },
    { hotkey: 'A', label: 'Patrol', iconKey: 'patrol' as const, state: 'ready' as const },
    { hotkey: 'S', label: 'Empty Slot', state: 'empty' as const },
    { hotkey: 'D', label: 'Empty Slot', state: 'empty' as const },
    { hotkey: 'F', label: 'Learn Skill', iconKey: 'skillz' as const, state: 'levelup' as const },
    { hotkey: 'Z', label: 'Wind Walk', iconKey: 'windWalk' as const, state: 'active' as const },
    { hotkey: 'X', label: 'Mirror Image', iconKey: 'mirrorImage' as const, state: 'ready' as const },
    { hotkey: 'C', label: 'Critical Strike', iconKey: 'criticalStrike' as const, state: 'passive' as const },
    { hotkey: 'V', label: 'Bladestorm', iconKey: 'bladestorm' as const, state: 'ready' as const },
  ];

  return (
    <section className="section-card">
      <h3>CommandCard</h3>
      <p className="section-desc">
        A spatial action grid with hotkey hints, cooldown states, and empty-slot affordances. Faster than a command palette — your users already know the layout.
      </p>
      <div className="command-card-preview">
        <div className="command-card-grid" role="grid" aria-label="Command card preview">
          {slots.map((slot) => (
            <div key={slot.hotkey} className="command-card-cell" role="gridcell">
              {slot.state === 'empty' ? (
                <div
                  className="cmd-btn cmd-btn-empty"
                  aria-hidden="true"
                >
                  {props.groupBorderUrl && <img src={props.groupBorderUrl} alt="" className="cmd-btn-frame" />}
                </div>
              ) : (
                (() => {
                  const iconUrl = slot.iconKey ? (props.iconUrls?.[slot.iconKey] || '') : '';
                  const showHighlight = slot.state === 'active';

                  return (
                    <button
                      type="button"
                      className={`cmd-btn${slot.state === 'passive' ? ' cmd-btn--passive' : ''}${slot.state === 'levelup' ? ' cmd-btn--levelup' : ''}`}
                      aria-label={`${slot.label}${slot.hotkey ? ` (${slot.hotkey})` : ''}`}
                      aria-keyshortcuts={slot.hotkey || undefined}
                    >
                      {iconUrl ? (
                        <img src={iconUrl} alt="" className="cmd-btn-icon" />
                      ) : (
                        <div className="cmd-btn-fallback-icon" aria-hidden="true">*</div>
                      )}
                      {props.groupBorderUrl && <img src={props.groupBorderUrl} alt="" className="cmd-btn-frame" />}
                      {showHighlight && (
                        <div
                          className="cmd-btn-highlight cmd-btn-highlight--on"
                          style={props.highlightUrl ? { backgroundImage: `url("${props.highlightUrl}")` } : undefined}
                        />
                      )}
                      {slot.hotkey && <span className="cmd-btn-hotkey" aria-hidden="true">{slot.hotkey}</span>}
                    </button>
                  );
                })()
              )}
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}

interface UnitQueueSectionProps {
  backdropUrl?: string;
  iconUrl?: string;
}

const UNIT_QUEUE_SIZE = { width: 241, height: 93 } as const;
const UNIT_QUEUE_ACTIVE_ICON_BOX = { x: 6, y: 5, width: 42, height: 42 } as const;
const UNIT_QUEUE_ICON_BOXES = [
  UNIT_QUEUE_ACTIVE_ICON_BOX,
  { x: 5, y: 57, width: 31, height: 30 },
  { x: 45, y: 58, width: 31, height: 30 },
  { x: 85, y: 58, width: 31, height: 30 },
  { x: 125, y: 58, width: 31, height: 30 },
] as const;

async function loadImage(src: string): Promise<HTMLImageElement> {
  return new Promise((resolve, reject) => {
    const img = new Image();
    img.onload = () => resolve(img);
    img.onerror = () => reject(new Error(`Failed to load image: ${src}`));
    img.src = src;
  });
}

function drawNearest(
  ctx: CanvasRenderingContext2D,
  image: HTMLImageElement,
  x: number,
  y: number,
  width: number,
  height: number,
) {
  ctx.save();
  ctx.imageSmoothingEnabled = false;
  ctx.drawImage(image, x, y, width, height);
  ctx.restore();
}

function drawUnitQueuePreview(
  canvas: HTMLCanvasElement,
  backdropImage: HTMLImageElement,
  iconImage: HTMLImageElement | null,
) {
  const rect = canvas.getBoundingClientRect();
  const cssW = Math.max(1, rect.width);
  const cssH = Math.max(1, rect.height);
  const dpr = window.devicePixelRatio || 1;
  const pxW = Math.max(1, Math.round(cssW * dpr));
  const pxH = Math.max(1, Math.round(cssH * dpr));
  if (canvas.width !== pxW || canvas.height !== pxH) {
    canvas.width = pxW;
    canvas.height = pxH;
  }

  const ctx = canvas.getContext('2d');
  if (!ctx) return;

  const scaleX = cssW / UNIT_QUEUE_SIZE.width;
  const scaleY = cssH / UNIT_QUEUE_SIZE.height;

  ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
  ctx.clearRect(0, 0, cssW, cssH);
  ctx.imageSmoothingEnabled = false;
  ctx.drawImage(backdropImage, 0, 0, cssW, cssH);
  if (!iconImage) return;

  for (const box of UNIT_QUEUE_ICON_BOXES) {
    drawNearest(ctx, iconImage, box.x * scaleX, box.y * scaleY, box.width * scaleX, box.height * scaleY);
  }
}

export function UnitQueueSection(props: UnitQueueSectionProps) {
  const canvasRef = useRef<HTMLCanvasElement>(null);

  useEffect(() => {
    const canvas = canvasRef.current;
    const backdropUrl = props.backdropUrl;
    const iconUrl = props.iconUrl;
    if (!canvas || !backdropUrl) return;

    let cancelled = false;
    void Promise.all([
      loadImage(backdropUrl),
      iconUrl ? loadImage(iconUrl) : Promise.resolve(null),
    ])
      .then(([backdropImage, iconImage]) => {
        if (cancelled) return;
        drawUnitQueuePreview(canvas, backdropImage, iconImage);
      })
      .catch((error) => console.error(error));

    return () => {
      cancelled = true;
    };
  }, [props.backdropUrl, props.iconUrl]);

  return (
    <section className="section-card">
      <h3>Unit Queue</h3>
      <p className="section-desc">
        A task queue with progress indicator and cancelable pending slots. Perfect for managing build pipelines, AI agent workflows, or anything that trains in 30 seconds.
      </p>
      <div className="unit-queue-preview">
        <div className="unit-queue-stage" aria-label="Build queue backdrop preview">
          {props.backdropUrl ? (
            <canvas ref={canvasRef} className="unit-queue-canvas" />
          ) : (
            <div className="unit-queue-fallback" />
          )}
        </div>
      </div>
    </section>
  );
}

export function TooltipSection(props: {
  backgroundUrl?: string;
  borderImageUrl?: string;
  goldIconUrl?: string;
  lumberIconUrl?: string;
  supplyIconUrl?: string;
}) {
  const tooltipSkinStyle = {
    '--tooltip-bg': props.backgroundUrl ? `url("${props.backgroundUrl}")` : '',
    '--tooltip-border': props.borderImageUrl ? `url("${props.borderImageUrl}")` : '',
  } as React.CSSProperties;

  return (
    <section className="section-card">
      <h3>Tooltip</h3>
      <p className="section-desc">
        Rich content tooltips with title, description, hotkey badge, and inline resource costs. Your <code>title</code> attribute could never.
      </p>
      <div className="tooltip-preview-grid">
        <article className="tooltip-preview" style={tooltipSkinStyle}>
          <div className="tooltip-preview-head">
            <h4>Build Footman</h4>
            <span className="tooltip-hotkey">F</span>
          </div>
          <p>Basic frontline infantry. Strong against early pressure and pairs with priest support.</p>
          <div className="tooltip-costs">
            <span className="tooltip-cost"><img src={props.goldIconUrl} alt="" /> 135</span>
            <span className="tooltip-cost"><img src={props.lumberIconUrl} alt="" /> 0</span>
            <span className="tooltip-cost"><img src={props.supplyIconUrl} alt="" /> 2</span>
          </div>
        </article>
      </div>
    </section>
  );
}

export function PortraitFrameSection() {
  const race = useCurrentRace();
  const racePrefix = RACE_PREFIXES[race].tile;
  const portraitFrameSrc = `./console/${race}/${racePrefix}UIPortraitFrameCrop.png`;
  const portraitMaskSrc = `./console/${race}/${racePrefix}UIPortraitWindowMask.png`;

  return (
    <section className="section-card">
      <h3>PortraitFrame</h3>
      <p className="section-desc">
        A hero-tier avatar frame with live 3D portrait rendering. Because your users deserve better than a circle crop.
      </p>
      <div className="portrait-showcase">
        <div className="portrait-stage-wrap">
          <div className="portrait-stage">
            <div
              className="portrait-window"
              style={{
                '--portrait-window-mask': `url("${portraitMaskSrc}")`,
              } as React.CSSProperties}
            >
              {(() => { const h = getDefaultHero(race); return <HeroPortraitModel modelPath={h.modelPath} textures={h.textures} />; })()}
            </div>
            <img src={portraitFrameSrc} alt="" className="portrait-frame-overlay" />
          </div>
        </div>
        <div className="button-state-label">3D Hero Portrait ({race})</div>
      </div>
    </section>
  );
}

export function BarsSection(props: { children: ReactNode }) {
  return (
    <section className="section-card">
      <h3>HealthBar / ManaBar / XPBar / Progress / Loading</h3>
      <p className="section-desc">
        Health, mana, XP, build, and fullscreen loading bars with animated drain, fill, and layered overlays.
      </p>
      {props.children}
    </section>
  );
}
