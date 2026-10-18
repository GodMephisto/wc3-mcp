---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/BottomHud/BottomHud.tsx"
ref: HEAD
---

import { useCurrentRace, RACE_PREFIXES } from "../../state/race";
import type { Race } from "../../utils/types";
import "./style.css";

interface Props {
  race?: Race;
}

export default function BottomHud({ race: raceProp }: Props = {}) {
  const globalRace = useCurrentRace();
  const race = raceProp ?? globalRace;
  const rp = RACE_PREFIXES[race];

  const tileSrc = (n: string) =>
    `war3/Console/${race}/${rp.tile}UITile0${n}.png`;

  return (
    <div className="wc-hud-bar">
      <img className="wc-hud-tile-01" src={tileSrc("1")} alt="" />
      <img className="wc-hud-tile-02" src={tileSrc("2")} alt="" />
      <img className="wc-hud-tile-03" src={tileSrc("3")} alt="" />
      <img className="wc-hud-tile-04" src={tileSrc("4")} alt="" />
    </div>
  );
}
