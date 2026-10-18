---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/sections/ControlsSection.tsx"
ref: HEAD
---

import { useState } from 'react';
import type { Renderer } from '../../renderer/Renderer';
import Checkbox from '../Checkbox';
import RadioButton from '../RadioButton';
import Slider from '../Slider';
import EditBox from '../EditBox';

interface Props {
  renderer: Renderer;
}

export default function ControlsSection(props: Props) {
  const [selectedRadio, setSelectedRadio] = useState(0);

  return (
    <div className="tab-content">
      <div className="section-card">
        <h2>EscMenu Controls</h2>
        <p className="section-desc">Race-specific checkboxes, radio buttons, sliders, and edit boxes from the in-game Escape Menu.</p>
        <div className="menu-components-grid">
          <div className="menu-widget">
            <h3>Checkboxes</h3>
            <Checkbox label="Show Health Bars" initialChecked renderer={props.renderer} />
            <Checkbox label="Show Mana Bars" renderer={props.renderer} />
            <Checkbox label="Formation Toggle" initialChecked renderer={props.renderer} />
          </div>

          <div className="menu-widget">
            <h3>Radio Buttons</h3>
            <RadioButton label="Low" selected={selectedRadio === 0} onSelect={() => setSelectedRadio(0)} renderer={props.renderer} />
            <RadioButton label="Medium" selected={selectedRadio === 1} onSelect={() => setSelectedRadio(1)} renderer={props.renderer} />
            <RadioButton label="High" selected={selectedRadio === 2} onSelect={() => setSelectedRadio(2)} renderer={props.renderer} />
          </div>

          <div className="menu-widget">
            <h3>Sliders</h3>
            <Slider label="Music Volume" initialValue={0.7} renderer={props.renderer} />
            <Slider label="SFX Volume" initialValue={0.85} renderer={props.renderer} />
            <Slider label="Mouse Speed" initialValue={0.5} renderer={props.renderer} />
          </div>

          <div className="menu-widget">
            <h3>Edit Boxes (Human)</h3>
            <EditBox text="Player Name" renderer={props.renderer} />
            <EditBox text="Azeroth-1" renderer={props.renderer} />
          </div>
        </div>
      </div>
    </div>
  );
}
