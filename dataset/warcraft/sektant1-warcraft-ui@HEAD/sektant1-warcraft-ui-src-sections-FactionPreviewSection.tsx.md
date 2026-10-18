---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/sections/FactionPreviewSection.tsx"
ref: HEAD
---

import { useState, useEffect, useRef } from 'react';
import { decodeBLP, getBLPImageData } from 'war3-model';
import type { Race } from '../../lib/types';

const previewRaces: Array<{ race: Race; label: string }> = [
  { race: 'Human', label: 'Human' },
  { race: 'Orc', label: 'Orc' },
  { race: 'NightElf', label: 'Night Elf' },
  { race: 'Undead', label: 'Undead' },
];

const escButtonPressedOffsetXPx = 2;
const escButtonPressedOffsetYPx = 2;
const escSliderTrackWidthPx = 220;
const escSliderThumbWidthPx = 28;

interface RacePreviewAssets {
  escBgUrl: string;
  escBgDownUrl: string;
  escBorderUrl: string;
  escBorderDownUrl: string;
  escHoverUrl: string;
  sliderKnobUrl: string;
}

const EMPTY_RACE_PREVIEW_ASSETS: RacePreviewAssets = {
  escBgUrl: '',
  escBgDownUrl: '',
  escBorderUrl: '',
  escBorderDownUrl: '',
  escHoverUrl: '',
  sliderKnobUrl: '',
};

const EMPTY_RACE_PREVIEW_ASSET_MAP: Record<Race, RacePreviewAssets> = {
  Human: EMPTY_RACE_PREVIEW_ASSETS,
  Orc: EMPTY_RACE_PREVIEW_ASSETS,
  NightElf: EMPTY_RACE_PREVIEW_ASSETS,
  Undead: EMPTY_RACE_PREVIEW_ASSETS,
};

const blpDataUrlCache = new Map<string, Promise<string>>();
const imageCache = new Map<string, Promise<HTMLImageElement>>();

function shouldForceOpaqueOnZeroAlpha(path: string): boolean {
  return /\/buttons\/esc\/[^/]+\/[^/]+-options-(?:menu|button)-background(?:-down|-disabled)?\.blp$/i.test(path);
}

async function loadBlpDataUrl(path: string): Promise<string> {
  const cached = blpDataUrlCache.get(path);
  if (cached) return cached;

  const promise = (async () => {
    const res = await fetch(path);
    if (!res.ok) throw new Error(`Failed to fetch ${path}`);
    const ab = await res.arrayBuffer();
    const blp = decodeBLP(ab);
    const img = getBLPImageData(blp, 0);
    const pixelData = new Uint8ClampedArray(img.data);

    if (shouldForceOpaqueOnZeroAlpha(path)) {
      let allAlphaZero = true;
      for (let i = 3; i < pixelData.length; i += 4) {
        if (pixelData[i] !== 0) {
          allAlphaZero = false;
          break;
        }
      }
      if (allAlphaZero) {
        for (let i = 3; i < pixelData.length; i += 4) {
          pixelData[i] = 255;
        }
      }
    }

    const canvas = document.createElement('canvas');
    canvas.width = img.width;
    canvas.height = img.height;
    const ctx = canvas.getContext('2d');
    if (!ctx) throw new Error(`Failed to decode ${path}`);
    ctx.putImageData(new ImageData(pixelData, img.width, img.height), 0, 0);
    return canvas.toDataURL('image/png');
  })();

  blpDataUrlCache.set(path, promise);
  return promise;
}

async function loadDataUrlImage(url: string): Promise<HTMLImageElement> {
  const cached = imageCache.get(url);
  if (cached) return cached;
  const promise = new Promise<HTMLImageElement>((resolve, reject) => {
    const img = new Image();
    img.onload = () => resolve(img);
    img.onerror = () => reject(new Error(`Failed to load image ${url}`));
    img.src = url;
  });
  imageCache.set(url, promise);
  return promise;
}

function getEscSliderKnobPath(race: Race): string {
  if (race === 'Orc') return './buttons/slider/orc-slider-knob.blp';
  if (race === 'NightElf') return './buttons/slider/nightelf-slider-knob.blp';
  if (race === 'Undead') return './buttons/slider/undead-slider-knob.blp';
  return './buttons/slider/slider-knob.blp';
}

interface EscButtonSkin {
  bg: string;
  bgDown: string;
  border: string;
  borderDown: string;
  hover: string;
}

function getEscButtonSkin(race: Race): EscButtonSkin {
  const humanBorder = './buttons/esc/human/human-options-button-border-up.blp';
  const humanBorderDown = './buttons/esc/human/human-options-button-border-down.blp';

  if (race === 'Human') {
    return {
      bg: './buttons/esc/human/human-options-menu-background.blp',
      bgDown: './buttons/esc/human/human-options-menu-background.blp',
      border: humanBorder,
      borderDown: humanBorderDown,
      hover: '/buttons/esc/human/human-options-button-highlight.blp',
    };
  }

  if (race === 'Orc') {
    return {
      bg: './buttons/esc/orc/orc-options-button-background.blp',
      bgDown: './buttons/esc/orc/orc-options-button-background-down.blp',
      border: humanBorder,
      borderDown: humanBorderDown,
      hover: './buttons/esc/orc/orc-options-button-highlight.blp',
    };
  }

  if (race === 'NightElf') {
    return {
      bg: './buttons/esc/nightelf/nightelf-options-button-background.blp',
      bgDown: './buttons/esc/nightelf/nightelf-options-button-background-down.blp',
      border: humanBorder,
      borderDown: humanBorderDown,
      hover: './buttons/esc/nightelf/nightelf-options-button-highlight.blp',
    };
  }

  return {
    bg: './buttons/esc/undead/undead-options-button-background.blp',
    bgDown: './buttons/esc/undead/undead-options-button-background-down.blp',
    border: humanBorder,
    borderDown: humanBorderDown,
    hover: './buttons/esc/undead/undead-options-button-highlight.blp',
  };
}

function sliderThumbCenterPercent(valuePct: number, trackPx: number, thumbPx: number): string {
  const clamped = Math.max(0, Math.min(100, valuePct));
  const min = thumbPx * 0.5;
  const max = Math.max(min, trackPx - thumbPx * 0.5);
  const center = min + ((max - min) * clamped) / 100;
  return `${(center / trackPx) * 100}%`;
}

function rotateCellClockwise(atlas: HTMLImageElement, cellIndex: number, cellW: number, cellH: number): HTMLCanvasElement {
  const canvas = document.createElement('canvas');
  canvas.width = cellH;
  canvas.height = cellW;
  const ctx = canvas.getContext('2d');
  if (!ctx) return canvas;
  ctx.translate(cellH, 0);
  ctx.rotate(Math.PI / 2);
  ctx.drawImage(atlas, cellIndex * cellW, 0, cellW, cellH, 0, 0, cellW, cellH);
  return canvas;
}

function drawGlueNineSliceToCanvas(
  canvas: HTMLCanvasElement,
  bgImg: HTMLImageElement,
  borderImg: HTMLImageElement,
  hoverImg: HTMLImageElement | null,
  hovered: boolean,
) {
  const cssW = Math.max(1, canvas.clientWidth || 1);
  const cssH = Math.max(1, canvas.clientHeight || 1);
  const dpr = window.devicePixelRatio || 1;
  const pxW = Math.max(1, Math.round(cssW * dpr));
  const pxH = Math.max(1, Math.round(cssH * dpr));
  if (canvas.width !== pxW || canvas.height !== pxH) {
    canvas.width = pxW;
    canvas.height = pxH;
  }

  const ctx = canvas.getContext('2d');
  if (!ctx) return;
  ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
  ctx.clearRect(0, 0, cssW, cssH);

  const atlasW = borderImg.width;
  const atlasH = borderImg.height;
  const cellW = atlasW / 8;
  const corner = Math.max(1, Math.floor(Math.min(cellW, cssH * 0.35, cssW / 2, cssH / 2)));
  const inset = Math.max(1, Math.round(corner * 0.25));

  ctx.fillStyle = '#000';
  ctx.fillRect(inset, inset, cssW - inset * 2, cssH - inset * 2);

  const pattern = ctx.createPattern(bgImg, 'repeat');
  if (pattern) {
    ctx.fillStyle = pattern;
    ctx.fillRect(inset, inset, cssW - inset * 2, cssH - inset * 2);
  }

  const drawCell = (idx: number, x: number, y: number, w: number, h: number) => {
    ctx.drawImage(borderImg, idx * cellW, 0, cellW, atlasH, x, y, w, h);
  };

  drawCell(4, 0, 0, corner, corner);
  drawCell(5, cssW - corner, 0, corner, corner);
  drawCell(6, 0, cssH - corner, corner, corner);
  drawCell(7, cssW - corner, cssH - corner, corner, corner);

  const edgeH = cssH - corner * 2;
  if (edgeH > 0) {
    drawCell(0, 0, corner, corner, edgeH);
    drawCell(1, cssW - corner, corner, corner, edgeH);
  }

  const edgeW = cssW - corner * 2;
  if (edgeW > 0) {
    const topRot = rotateCellClockwise(borderImg, 2, cellW, atlasH);
    const bottomRot = rotateCellClockwise(borderImg, 3, cellW, atlasH);
    ctx.drawImage(topRot, corner, 0, edgeW, corner);
    ctx.drawImage(bottomRot, corner, cssH - corner, edgeW, corner);
  }

  if (hovered && hoverImg) {
    const temp = document.createElement('canvas');
    temp.width = pxW;
    temp.height = pxH;
    const tCtx = temp.getContext('2d');
    if (tCtx) {
      tCtx.setTransform(dpr, 0, 0, dpr, 0, 0);
      tCtx.drawImage(canvas, 0, 0, cssW, cssH);
      tCtx.globalCompositeOperation = 'source-in';
      tCtx.drawImage(hoverImg, 0, 0, cssW, cssH);
      ctx.save();
      ctx.globalCompositeOperation = 'lighter';
      ctx.drawImage(temp, 0, 0, cssW, cssH);
      ctx.restore();
    }
  }
}

interface FactionCardProps {
  race: Race;
  label: string;
  assets: RacePreviewAssets;
  sliderTrackBgUrl: string;
  sliderTrackBorderAtlasUrl: string;
  resizeTick: number;
}

function FactionPreviewCard(props: FactionCardProps) {
  const escButtonCanvasRef = useRef<HTMLCanvasElement>(null);
  const sliderCanvasRef = useRef<HTMLCanvasElement>(null);
  const sliderTrackRef = useRef<HTMLDivElement>(null);

  const [escButtonHovered, setEscButtonHovered] = useState(false);
  const [escButtonPressed, setEscButtonPressed] = useState(false);
  const [sliderValue, setSliderValue] = useState(55);

  const trackPx = Math.max(1, Math.round(sliderTrackRef.current?.clientWidth || escSliderTrackWidthPx));
  const sliderKnobLeft = sliderThumbCenterPercent(sliderValue, trackPx, escSliderThumbWidthPx);

  useEffect(() => {
    const canvas = escButtonCanvasRef.current;
    const bgUrl = escButtonPressed ? props.assets.escBgDownUrl : props.assets.escBgUrl;
    const borderUrl = escButtonPressed ? props.assets.escBorderDownUrl : props.assets.escBorderUrl;
    const hoverUrl = props.assets.escHoverUrl;
    const hoverActive = escButtonHovered && !escButtonPressed;
    if (!canvas || !bgUrl || !borderUrl) return;
    let cancelled = false;

    void Promise.all([
      loadDataUrlImage(bgUrl),
      loadDataUrlImage(borderUrl),
      hoverUrl ? loadDataUrlImage(hoverUrl) : Promise.resolve(null),
    ])
      .then(([bgImg, borderImg, hoverImg]) => {
        if (cancelled) return;
        drawGlueNineSliceToCanvas(canvas, bgImg, borderImg, hoverImg, hoverActive);
      })
      .catch((err) => console.error(err));

    return () => { cancelled = true; };
  }, [escButtonHovered, escButtonPressed, props.assets]);

  useEffect(() => {
    const canvas = sliderCanvasRef.current;
    const bgUrl = props.sliderTrackBgUrl;
    const borderUrl = props.sliderTrackBorderAtlasUrl;
    if (!canvas || !bgUrl || !borderUrl) return;
    let cancelled = false;

    void Promise.all([loadDataUrlImage(bgUrl), loadDataUrlImage(borderUrl)])
      .then(([bgImg, borderImg]) => {
        if (cancelled) return;
        drawGlueNineSliceToCanvas(canvas, bgImg, borderImg, null, false);
      })
      .catch((err) => console.error(err));

    return () => { cancelled = true; };
  }, [props.sliderTrackBgUrl, props.sliderTrackBorderAtlasUrl, props.resizeTick]);

  return (
    <article className="faction-preview-card">
      <div className="faction-preview-label">{props.label}</div>
      <div className="faction-preview-samples">
        <div className="faction-preview-control">
          <div className="faction-preview-control-label">Button</div>
          <button
            type="button"
            className="esc-option-preview faction-preview-option-button"
            onMouseEnter={() => setEscButtonHovered(true)}
            onMouseLeave={() => {
              setEscButtonHovered(false);
              setEscButtonPressed(false);
            }}
            onMouseDown={() => setEscButtonPressed(true)}
            onMouseUp={() => setEscButtonPressed(false)}
          >
            <canvas ref={escButtonCanvasRef} className="esc-option-canvas" />
            <span
              className="esc-option-label"
              style={{
                transform: escButtonPressed
                  ? `translate(${escButtonPressedOffsetXPx}px, ${escButtonPressedOffsetYPx}px)`
                  : 'none',
              }}
            >
              <span className="button-hotkey">O</span>ptions
            </span>
          </button>
        </div>

        <div className="faction-preview-control">
          <div className="faction-preview-control-label">Slider</div>
          <label className="esc-slider-demo faction-preview-slider-demo">
            <span className="esc-slider-value">{sliderValue}%</span>
            <div ref={sliderTrackRef} className="esc-slider-track faction-preview-slider-track">
              <canvas ref={sliderCanvasRef} className="esc-slider-canvas" />
              <div
                className="esc-slider-knob"
                style={{
                  backgroundImage: props.assets.sliderKnobUrl ? `url("${props.assets.sliderKnobUrl}")` : 'none',
                  left: sliderKnobLeft,
                }}
              />
              <input
                type="range"
                min="0"
                max="100"
                value={sliderValue}
                aria-label={`${props.label} slider preview`}
                onChange={(event) => setSliderValue(Number(event.currentTarget.value))}
              />
            </div>
          </label>
        </div>

      </div>
    </article>
  );
}

export default function FactionPreviewSection() {
  const [raceAssets, setRaceAssets] = useState<Record<Race, RacePreviewAssets>>(EMPTY_RACE_PREVIEW_ASSET_MAP);
  const [sliderTrackBgUrl, setSliderTrackBgUrl] = useState('');
  const [sliderTrackBorderAtlasUrl, setSliderTrackBorderAtlasUrl] = useState('');
  const [resizeTick, setResizeTick] = useState(0);

  useEffect(() => {
    let cancelled = false;

    void Promise.all([
      Promise.all(
        previewRaces.map(async ({ race }) => {
          const escSkin = getEscButtonSkin(race);
          const [escBgUrl, escBgDownUrl, escBorderUrl, escBorderDownUrl, escHoverUrl, sliderKnobUrl] = await Promise.all([
            loadBlpDataUrl(escSkin.bg),
            loadBlpDataUrl(escSkin.bgDown),
            loadBlpDataUrl(escSkin.border),
            loadBlpDataUrl(escSkin.borderDown),
            loadBlpDataUrl(escSkin.hover),
            loadBlpDataUrl(getEscSliderKnobPath(race)),
          ]);

          return [
            race,
            {
              escBgUrl,
              escBgDownUrl,
              escBorderUrl,
              escBorderDownUrl,
              escHoverUrl,
              sliderKnobUrl,
            } satisfies RacePreviewAssets,
          ] as const;
        }),
      ),
      loadBlpDataUrl('./buttons/slider/slider-background.blp'),
      loadBlpDataUrl('./buttons/slider/slider-border.blp'),
    ])
      .then(([entries, sliderBg, sliderBorder]) => {
        if (cancelled) return;
        const next: Record<Race, RacePreviewAssets> = {
          Human: { ...EMPTY_RACE_PREVIEW_ASSETS },
          Orc: { ...EMPTY_RACE_PREVIEW_ASSETS },
          NightElf: { ...EMPTY_RACE_PREVIEW_ASSETS },
          Undead: { ...EMPTY_RACE_PREVIEW_ASSETS },
        };
        for (const [race, assets] of entries) {
          next[race] = assets;
        }
        setRaceAssets(next);
        setSliderTrackBgUrl(sliderBg);
        setSliderTrackBorderAtlasUrl(sliderBorder);
      })
      .catch((err) => console.error(err));

    const onResize = () => setResizeTick((value) => value + 1);
    window.addEventListener('resize', onResize);

    return () => {
      cancelled = true;
      window.removeEventListener('resize', onResize);
    };
  }, []);

  return (
    <section className="section-card faction-preview-section">
      <p className="section-desc">
        Every component adapts to your chosen faction. One prop changes everything. We wish CSS had thought of this.
      </p>
      <div className="faction-preview">
        {previewRaces.map(({ race, label }) => (
          <FactionPreviewCard
            key={race}
            race={race}
            label={label}
            assets={raceAssets[race]}
            sliderTrackBgUrl={sliderTrackBgUrl}
            sliderTrackBorderAtlasUrl={sliderTrackBorderAtlasUrl}
            resizeTick={resizeTick}
          />
        ))}
      </div>
    </section>
  );
}
