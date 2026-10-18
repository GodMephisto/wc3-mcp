---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/EscCheckbox/EscCheckbox.tsx"
ref: HEAD
---

import { useCallback, useRef, useState } from "react";
import { useBlpTextures, useCanvasRenderer } from "../../utils/blpLoader";
import { useCurrentRace, RACE_PREFIXES } from "../../state/race";
import "./style.css";

interface Props {
  label: string;
  checked?: boolean;
  disabled?: boolean;
  onChange?: (checked: boolean) => void;
  race?: import("../../utils/types").Race;
}

export default function EscCheckbox({
  label,
  checked: controlledChecked,
  disabled = false,
  onChange,
  race: raceProp,
}: Props) {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const [internalChecked, setInternalChecked] = useState(
    controlledChecked ?? false,
  );
  const globalRace = useCurrentRace();
  const race = raceProp ?? globalRace;
  const rp = RACE_PREFIXES[race];

  const isControlled = controlledChecked !== undefined;
  const checked = isControlled ? controlledChecked : internalChecked;

  const depressedPath =
    race === "Human"
      ? "buttons/checkbox/checkbox-depressed.blp"
      : `buttons/checkbox/${rp.esc}-checkbox-depressed.blp`;

  const tex = useBlpTextures({
    bg: "buttons/checkbox/checkbox-background.blp",
    check: "buttons/checkbox/checkbox-check.blp",
    depressed: depressedPath,
  });

  const draw = useCallback(
    (ctx: CanvasRenderingContext2D, w: number, h: number) => {
      if (!tex) return;
      const src = disabled ? tex.depressed : tex.bg;
      ctx.drawImage(src, 0, 0, w, h);
      if (checked) {
        ctx.drawImage(tex.check, 0, 0, w, h);
      }
    },
    [tex, checked, disabled],
  );

  useCanvasRenderer(canvasRef, draw, [tex, checked, disabled]);

  const toggle = () => {
    if (disabled) return;
    if (isControlled) {
      onChange?.(!checked);
    } else {
      setInternalChecked((v) => {
        onChange?.(!v);
        return !v;
      });
    }
  };

  return (
    <div
      className={`wc-checkbox${checked ? " wc-checkbox--checked" : ""}${disabled ? " wc-checkbox--disabled" : ""}`}
      role="checkbox"
      aria-checked={checked}
      aria-disabled={disabled}
      tabIndex={disabled ? -1 : 0}
      onClick={toggle}
      onKeyDown={(e) => e.key === " " && toggle()}
    >
      <canvas ref={canvasRef} className="wc-checkbox-visual" />
      <span className="wc-checkbox-label">{label}</span>
    </div>
  );
}
