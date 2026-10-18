---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/sections/ButtonsSection.tsx"
ref: HEAD
---

import type { Renderer } from '../../renderer/Renderer';
import GlueButton from '../GlueButton';
import MenuButton from '../MenuButton';
import OptionButton from '../OptionButton';
import { useCurrentRace, RACE_PREFIXES } from '../../state/race';

interface Props {
  renderer: Renderer;
}

const GLUE_BG = 'war3/Widgets/Glues/GlueScreen-Button1-BackdropBackground.png';
const GLUE_BORDER = 'war3/Widgets/Glues/GlueScreen-Button1-BackdropBorder.png';
const GLUE_BG_DOWN = 'war3/Widgets/Glues/GlueScreen-Button1-BackdropBackground-Down.png';
const GLUE_BORDER_DOWN = 'war3/Widgets/Glues/GlueScreen-Button1-BackdropBorder-Down.png';
const GLUE_BORDERED = 'war3/Widgets/Glues/GlueScreen-Button1-BorderedBackdropBorder.png';
const GLUE_BORDERED_DOWN = 'war3/Widgets/Glues/GlueScreen-Button1-BorderedBackdropBorder-Down.png';
const GLUE_HOVER = 'war3/Widgets/BattleNet/bnet-button01-highlight-mouse.png';
const GLUE_BG_DISABLED = 'war3/Widgets/Glues/GlueScreen-Button1-BackdropBackground-Disabled.png';
const GLUE_BORDER_DISABLED = 'war3/Widgets/Glues/GlueScreen-Button1-BackdropBorder-Disabled.png';
const GLUE_FRAME = 'war3/Widgets/Glues/GlueScreen-Button1-Border.png';

const MENU_LABELS = ['Single Player', 'Multi Player', 'Options', 'Exit Game'];

export default function ButtonsSection(props: Props) {
  const race = useCurrentRace();
  const rp = RACE_PREFIXES[race];
  const consoleBtnPath = `war3/Widgets/Console/${race}/${rp.lower}`;
  const cmdBtnPath = `${consoleBtnPath}-console-button`;

  return (
    <div className="tab-content">
      {/* Glue Screen Buttons */}
      <div className="section-card">
        <h2>Glue Screen Buttons (9-Slice Composited)</h2>
        <p className="section-desc">Main menu buttons use 9-slice compositing: a 32x32 tiled background + a 256x32 border atlas (8 pieces of 32x32: TL, T, TR, R, BR, B, BL, L). Shared across all races.</p>
        <div className="glue-buttons-grid">
          <div className="glue-btn-demo">
            <GlueButton bgPath={GLUE_BG} borderPath={GLUE_BORDER} bgDownPath={GLUE_BG_DOWN} borderDownPath={GLUE_BORDER_DOWN} hoverPath={GLUE_HOVER} renderer={props.renderer}>
              <div className="gb-label"><span className="hotkey">S</span>ingle Player</div>
            </GlueButton>
            <div className="glue-btn-state-label">Normal</div>
          </div>
          <div className="glue-btn-demo">
            <GlueButton bgPath={GLUE_BG} borderPath={GLUE_BORDERED} bgDownPath={GLUE_BG_DOWN} borderDownPath={GLUE_BORDERED_DOWN} hoverPath={GLUE_HOVER} renderer={props.renderer}>
              <div className="gb-label"><span className="hotkey">S</span>ingle Player</div>
            </GlueButton>
            <div className="glue-btn-state-label">Bordered</div>
          </div>
          <div className="glue-btn-demo">
            <GlueButton bgPath={GLUE_BG_DISABLED} borderPath={GLUE_BORDER_DISABLED} disabled renderer={props.renderer}>
              <div className="gb-label" style={{ opacity: 0.5 }}>Single Player</div>
            </GlueButton>
            <div className="glue-btn-state-label">Disabled</div>
          </div>
        </div>

        <h3 style={{ marginTop: '28px', color: 'var(--wc3-gold)', textShadow: 'var(--wc3-shadow)' }}>Main Menu Buttons</h3>
        <p className="section-desc">In the game, 9-slice buttons are drawn inside pre-rendered border frames (BackdropBackground stretched with BackdropBlendAll). The GLUETEXTBUTTON is a child of the base BACKDROP frame.</p>
        <div className="glue-menu-grid">
          {MENU_LABELS.map((label) => {
            const hotkey = label[0];
            const rest = label.slice(1);
            return (
              <MenuButton key={label} frameSrc={GLUE_FRAME} bgPath={GLUE_BG} borderPath={GLUE_BORDER} bgDownPath={GLUE_BG_DOWN} borderDownPath={GLUE_BORDER_DOWN} hoverPath={GLUE_HOVER} renderer={props.renderer}>
                <div className="gb-label"><span className="hotkey">{hotkey}</span>{rest}</div>
              </MenuButton>
            );
          })}
        </div>
      </div>

      {/* EscMenu Option Buttons */}
      <div className="section-card">
        <h2>EscMenu Option Buttons</h2>
        <p className="section-desc">Race-specific option buttons with background + border + highlight layers.</p>
        <div style={{ display: 'flex', flexWrap: 'wrap', gap: '8px', justifyContent: 'center' }}>
          {['Save Game', 'Options', 'End Game'].map((label) =>
            <OptionButton key={label} label={label} renderer={props.renderer} />
          )}
        </div>
      </div>

      {/* Console Buttons */}
      <div className="section-card">
        <h2>Console Buttons</h2>
        <p className="section-desc">Minimap controls, formation toggles, and command button grid from the in-game HUD console.</p>
        <div className="console-buttons-grid">
          <ConsoleButtonGroup title="Minimap - Ally" items={[
            { src: `${consoleBtnPath}-minimap-ally-active.png`, label: 'Active' },
            { src: `${consoleBtnPath}-minimap-ally-disabled.png`, label: 'Disabled' },
            { src: `${consoleBtnPath}-minimap-ally-active-down.png`, label: 'Down' },
          ]} />
          <ConsoleButtonGroup title="Minimap - Ping" items={[
            { src: `${consoleBtnPath}-minimap-ping-active.png`, label: 'Active' },
            { src: `${consoleBtnPath}-minimap-ping-disabled.png`, label: 'Disabled' },
            { src: `${consoleBtnPath}-minimap-ping-active-down.png`, label: 'Down' },
          ]} />
          <ConsoleButtonGroup title="Minimap - Terrain" items={[
            { src: `${consoleBtnPath}-minimap-terrain-active.png`, label: 'Active' },
            { src: `${consoleBtnPath}-minimap-terrain-disabled.png`, label: 'Disabled' },
            { src: `${consoleBtnPath}-minimap-terrain-active-down.png`, label: 'Down' },
          ]} />
          <ConsoleButtonGroup title="Formation" items={[
            { src: `${consoleBtnPath}-formation-on.png`, label: 'On' },
            { src: `${consoleBtnPath}-formation-off.png`, label: 'Off' },
            { src: `${consoleBtnPath}-formation-on-down.png`, label: 'On Down' },
            { src: `${consoleBtnPath}-formation-off-down.png`, label: 'Off Down' },
          ]} />
          <ConsoleButtonGroup title="Console Buttons" items={[
            { src: `${cmdBtnPath}-up.png`, label: 'Up' },
            { src: `${cmdBtnPath}-down.png`, label: 'Down' },
            { src: `${cmdBtnPath}-highlight.png`, label: 'Highlight' },
            { src: `${cmdBtnPath}-back-active.png`, label: 'Back' },
            { src: `${cmdBtnPath}-back-disabled.png`, label: 'Back Off' },
          ]} />
        </div>

        <h3>Command Button Grid</h3>
        <div style={{ textAlign: 'center' }}>
          <div className="command-grid">
            {Array(12).fill(null).map((_, i) =>
              <div key={i} className="command-cell">
                <img src={`war3/Widgets/Console/${race}/CommandButton/${rp.lower}-activebutton.png`} alt="" />
              </div>
            )}
          </div>
          <div style={{ display: 'flex', gap: '8px', justifyContent: 'center', marginTop: '10px' }}>
            <div className="tooltip-res-card">
              <img src={`war3/Widgets/Console/${race}/CommandButton/${rp.lower}-activebutton.png`} style={{ width: '48px', height: '48px', border: '1px solid var(--card-border)', borderRadius: '2px' }} />
              <div className="res-label">Active Button</div>
            </div>
            <div className="tooltip-res-card">
              <img src={`war3/Widgets/Console/${race}/CommandButton/${rp.lower}-multipleselection-border.png`} style={{ width: '48px', height: '48px', border: '1px solid var(--card-border)', borderRadius: '2px' }} />
              <div className="res-label">Selection</div>
            </div>
            <div className="tooltip-res-card">
              <img src={`war3/Widgets/Console/${race}/CommandButton/${rp.lower}-multipleselection-heroglow.png`} style={{ width: '48px', height: '48px', border: '1px solid var(--card-border)', borderRadius: '2px' }} />
              <div className="res-label">Hero Glow</div>
            </div>
            <div className="tooltip-res-card">
              <img src={`war3/Widgets/Console/${race}/CommandButton/${rp.lower}-subgroup-background.png`} style={{ width: '48px', height: '48px', border: '1px solid var(--card-border)', borderRadius: '2px' }} />
              <div className="res-label">Subgroup BG</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

function ConsoleButtonGroup(props: { title: string; items: { src: string; label: string }[] }) {
  return (
    <div className="console-btn-group">
      <h4>{props.title}</h4>
      <div className="console-btn-row">
        {props.items.map((item) =>
          <div key={item.label} className="console-btn-card">
            <img src={item.src} alt={item.label} />
            <div className="cb-label">{item.label}</div>
          </div>
        )}
      </div>
    </div>
  );
}
