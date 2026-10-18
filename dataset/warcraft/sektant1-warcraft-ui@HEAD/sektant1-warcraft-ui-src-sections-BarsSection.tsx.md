---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/sections/BarsSection.tsx"
ref: HEAD
---

import type { Renderer } from '../../renderer/Renderer';
import StatBar from '../StatBar';
import LoadingBar from '../LoadingBar';
import { useCurrentRace, RACE_PREFIXES } from '../../state/race';

interface Props {
  renderer: Renderer;
  healthPct: number;
  manaPct: number;
  xpPct: number;
  buildPct: number;
  loadingProgress: number;
}

export default function BarsSection(props: Props) {
  const race = useCurrentRace();
  const rp = RACE_PREFIXES[race];

  return (
    <div className="tab-content">
      <div className="section-card">
        <h2>Health / Mana / XP Bars</h2>
        <p className="section-desc">Animated stat bars that cycle between random fill percentages every 3 seconds.</p>
        <div className="bars-demo">
          <StatBar label="Health" type="health" fillPercent={props.healthPct} maxValue={650} renderer={props.renderer} />
          <StatBar label="Mana" type="mana" fillPercent={props.manaPct} maxValue={300} renderer={props.renderer} />
          <StatBar label="Experience" type="xp" fillPercent={props.xpPct} maxValue={800} height={14} hasBorder renderer={props.renderer} />
          <StatBar label="Build Progress" type="build" fillPercent={props.buildPct} height={14} hasBorder renderer={props.renderer} />
        </div>
        <div style={{ display: 'flex', gap: '12px', justifyContent: 'center', marginTop: '16px', flexWrap: 'wrap' }}>
          <div className="tooltip-res-card"><img src={`war3/Feedback/HPBarConsole/${rp.lower}-healthbar-fill.png`} style={{ width: '64px', height: '16px', imageRendering: 'auto' }} /><div className="res-label">Health Fill</div></div>
          <div className="tooltip-res-card"><img src={`war3/Feedback/ManaBarConsole/${rp.lower}-manabar-fill.png`} style={{ width: '64px', height: '16px', imageRendering: 'auto' }} /><div className="res-label">Mana Fill</div></div>
          <div className="tooltip-res-card"><img src={`war3/Feedback/XpBar/${rp.lower}-bigbar-fill.png`} style={{ width: '64px', height: '16px', imageRendering: 'auto' }} /><div className="res-label">XP Fill</div></div>
          <div className="tooltip-res-card"><img src={`war3/Feedback/BuildProgressBar/${rp.lower}-buildprogressbar-fill.png`} style={{ width: '64px', height: '16px', imageRendering: 'auto' }} /><div className="res-label">Build Fill</div></div>
        </div>
      </div>

      <div className="section-card">
        <h2>Loading Bar</h2>
        <p className="section-desc">Animated composite loading bar: background + fill + border + glass overlay.</p>
        <div className="loading-preview">
          <LoadingBar progress={props.loadingProgress} renderer={props.renderer} />
        </div>
      </div>
    </div>
  );
}
