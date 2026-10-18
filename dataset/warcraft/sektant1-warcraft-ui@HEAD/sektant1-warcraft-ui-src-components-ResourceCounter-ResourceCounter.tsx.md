---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/ResourceCounter/ResourceCounter.tsx"
ref: HEAD
---

import { useCallback, useRef } from "react";
import { useBlpTextures, useCanvasRenderer } from "../../utils/blpLoader";
import { useGoldCurrent, useLumberCurrent } from "../../state/resources";
import "./style.css";

type SlotType = "gold" | "lumber" | "food";

function ResourceIcon({ path }: { path: string }) {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const tex = useBlpTextures({ icon: path });

  const draw = useCallback(
    (ctx: CanvasRenderingContext2D, w: number, h: number) => {
      if (!tex) return;
      ctx.drawImage(tex.icon, 0, 0, w, h);
    },
    [tex],
  );

  useCanvasRenderer(canvasRef, draw, [tex]);

  return <canvas ref={canvasRef} className="wc-resource-icon" />;
}

function ResourceSlot({ type }: { type: SlotType }) {
  const gold = useGoldCurrent();
  const lumber = useLumberCurrent();

  const iconPath =
    type === "gold"
      ? "resources/ResourceGold.blp"
      : type === "lumber"
        ? "resources/ResourceLumber.blp"
        : "resources/ResourceSupply.blp";

  const value =
    type === "gold" ? gold : type === "lumber" ? lumber : "50 / 80";

  return (
    <div className="wc-resource-slot">
      <ResourceIcon path={iconPath} />
      <span className="wc-res-value">{value}</span>
    </div>
  );
}

export default function ResourceCounter() {
  return (
    <div className="wc-resources">
      <ResourceSlot type="gold" />
      <ResourceSlot type="lumber" />
      <ResourceSlot type="food" />
      <span className="wc-upkeep-text wc-upkeep-green">No Upkeep</span>
    </div>
  );
}
