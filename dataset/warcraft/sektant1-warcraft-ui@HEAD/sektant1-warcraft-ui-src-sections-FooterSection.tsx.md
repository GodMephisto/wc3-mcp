---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/sections/FooterSection.tsx"
ref: HEAD
---

import WorkerUnitModel from "../WorkerUnitModel";
import type { Race } from "../../lib/types";

interface Props {
  race: Race;
}

export default function FooterSection(props: Props) {
  const raceYOffsetPx = () => {
    if (props.race === "Orc") return 20;
    if (props.race === "NightElf") return 15;
    if (props.race === "Undead") return -30;
    return 0;
  };

  return (
    <footer className="site-footer">
      <div
        className="footer-worker-unit footer-worker-unit--left"
        aria-hidden="true"
        style={
          {
            "--footer-worker-y-offset": `${raceYOffsetPx()}px`,
          } as React.CSSProperties
        }
      >
        <WorkerUnitModel race={props.race} side="left" />
      </div>
      <div
        className="footer-worker-unit footer-worker-unit--right"
        aria-hidden="true"
        style={
          {
            "--footer-worker-y-offset": `${raceYOffsetPx()}px`,
          } as React.CSSProperties
        }
      >
        <WorkerUnitModel race={props.race} side="right" />
      </div>
      <div className="footer-content">
        <p className="footer-links">
          WC3 Build Order Tools is a study project with no monetization goals.{" "}
        </p>
        <p className="footer-joke">
          <em>"My life for the horde!"</em>
        </p>
        <p className="footer-signoff">@sektant1 2026</p>
      </div>
    </footer>
  );
}
