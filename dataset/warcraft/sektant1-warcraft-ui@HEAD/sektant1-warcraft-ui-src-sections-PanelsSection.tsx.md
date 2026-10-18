---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/sections/PanelsSection.tsx"
ref: HEAD
---

import type { Renderer } from '../../renderer/Renderer';
import MenuPanel from '../MenuPanel';
import HeroGallery from '../HeroGallery';
import { useCurrentRace, RACE_PREFIXES } from '../../state/race';

interface Props {
  renderer: Renderer;
}

export default function PanelsSection(props: Props) {
  const race = useCurrentRace();
  const rp = RACE_PREFIXES[race];
  const consolePath = `war3/Console/${race}/`;

  return (
    <div className="tab-content">
      <div className="section-card">
        <h2>Menu Background</h2>
        <p className="section-desc">Race-specific EscMenu panel with background + border composite.</p>
        <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '12px' }}>
          <MenuPanel renderer={props.renderer} />
          <div style={{ textAlign: 'center' }}>
            <div style={{ fontSize: '0.75rem', color: 'var(--text-dim)', marginBottom: '4px' }}>Border Texture Strip (512x64)</div>
            <img src={`war3/Widgets/EscMenu/${race}/${rp.esc}-options-menu-border.png`} style={{ height: '40px', imageRendering: 'auto' }} alt="" />
          </div>
        </div>
      </div>

      <div className="section-card">
        <h2>Loading Screen</h2>
        <p className="section-desc">2x2 grid with shared left quadrants + race-specific right quadrants.</p>
        <div className="loading-preview">
          <div className="loading-grid">
            <img src="war3/Glues/Loading/Multiplayer/Loading-TopLeft.png" alt="Top Left" />
            <img src={`war3/Glues/Loading/Multiplayer/Loading-${race}-TopRight.png`} alt="Top Right" />
            <img src="war3/Glues/Loading/Multiplayer/Loading-BotLeft.png" alt="Bottom Left" />
            <img src={`war3/Glues/Loading/Multiplayer/Loading-${race}-BotRight.png`} alt="Bottom Right" />
          </div>
        </div>
      </div>

      <div className="section-card">
        <h2>Hero Portrait Gallery</h2>
        <p className="section-desc">Score screen hero portraits from Reign of Chaos and The Frozen Throne.</p>
        <HeroGallery />
      </div>

      <div className="section-card">
        <h2>HUD Tile Breakdown</h2>
        <p className="section-desc">Individual HUD tiles, portrait mask, and inventory cover for the current race.</p>
        <div className="hud-tiles-preview">
          <div className="tile-card">
            <img src={`${consolePath}${rp.tile}UITile01.png`} alt="Tile 01" />
            <div className="tile-label">Tile 01 (512x512)</div>
          </div>
          <div className="tile-card">
            <img src={`${consolePath}${rp.tile}UITile02.png`} alt="Tile 02" />
            <div className="tile-label">Tile 02 (512x512)</div>
          </div>
          <div className="tile-card">
            <img src={`${consolePath}${rp.tile}UITile03.png`} alt="Tile 03" />
            <div className="tile-label">Tile 03 (512x512)</div>
          </div>
          <div className="tile-card">
            <img src={`${consolePath}${rp.tile}UITile04.png`} alt="Tile 04" />
            <div className="tile-label">Tile 04 (64x512)</div>
          </div>
        </div>
        <div className="hud-tiles-preview" style={{ marginTop: '12px' }}>
          <div className="tile-card">
            <img src={`${consolePath}${rp.tile}UIPortraitMask.png`} alt="Portrait Mask" />
            <div className="tile-label">Portrait Mask</div>
          </div>
          <div className="tile-card">
            <img src={`${consolePath}${rp.tile}UITile-TimeIndicator.png`} alt="Time Indicator" />
            <div className="tile-label">Time Indicator</div>
          </div>
          <div className="tile-card">
            <img src={`${consolePath}${rp.tile}UITile-InventoryCover.png`} alt="Inventory Cover" />
            <div className="tile-label">Inventory Cover</div>
          </div>
        </div>
      </div>
    </div>
  );
}
