---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/HeroGallery/HeroGallery.tsx"
ref: HEAD
---

import { useCurrentRace, RACE_PREFIXES } from "../../state/race";
import "./style.css";

export interface HeroData {
  name: string;
  src: string;
  file: string;
  lore?: string;
}

interface Props {
  heroes: Record<string, HeroData[]>;
  race?: import("../../utils/types").Race;
}

export default function HeroGallery({ heroes, race: raceProp }: Props) {
  const globalRace = useCurrentRace();
  const race = raceProp ?? globalRace;
  const raceHeroes = heroes[race] || [];

  return (
    <div className="wc-hero-gallery">
      {raceHeroes.length > 0 && (
        <>
          <div className="wc-hero-group-label">
            {RACE_PREFIXES[race].display} Heroes
          </div>
          {raceHeroes.map((hero) => (
            <HeroCard key={hero.name} hero={hero} />
          ))}
        </>
      )}
      {heroes.Neutral && (
        <>
          <div className="wc-hero-group-label">Neutral / Expansion Heroes</div>
          {heroes.Neutral.map((hero) => (
            <HeroCard key={hero.name} hero={hero} />
          ))}
        </>
      )}
    </div>
  );
}

function HeroCard({ hero }: { hero: HeroData }) {
  const srcDir = hero.src + "/Glues/ScoreScreen/";
  return (
    <div className="wc-hero-card" title={hero.lore}>
      <img
        src={srcDir + hero.file}
        alt={hero.name}
        onError={(e) => {
          (e.target as HTMLImageElement).style.display = "none";
        }}
      />
      <div className="wc-hero-name">{hero.name}</div>
      {hero.lore && <div className="wc-hero-lore">{hero.lore}</div>}
    </div>
  );
}
