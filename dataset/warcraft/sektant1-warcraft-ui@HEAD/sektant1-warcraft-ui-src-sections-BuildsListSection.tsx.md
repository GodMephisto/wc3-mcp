---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/sections/BuildsListSection.tsx"
ref: HEAD
---

import { useState, type CSSProperties } from "react";
import { useBuildOrders } from "../../state/buildOrders";
import { setPendingBuildSelect } from "../../state/pendingBuildSelect";
import { setPage } from "../../state/page";
import type { BuildOrder } from "../../data/buildOrders";
import type { Race } from "../../lib/types";
import HeroIconImg from "../HeroIconImg";

type Filter = Race | "All";

const FILTERS: Filter[] = ["All", "Human", "Orc", "NightElf", "Undead"];

const FILTER_LABEL: Record<Filter, string> = {
  All: "All",
  Human: "Human",
  Orc: "Orc",
  NightElf: "Night Elf",
  Undead: "Undead",
};

const RACE_ACCENT: Record<Race, string> = {
  Human:    "#5a8fcc",
  Orc:      "#bb0a1b",
  NightElf: "#4aaa5a",
  Undead:   "#9a5ac4",
};

const RACE_LABEL: Record<Race, string> = {
  Human:    "Human",
  Orc:      "Orc",
  NightElf: "Night Elf",
  Undead:   "Undead",
};

function diffClass(d?: string) {
  if (d === "Beginner") return "bls-card-diff--beginner";
  if (d === "Intermediate") return "bls-card-diff--intermediate";
  if (d === "Advanced") return "bls-card-diff--advanced";
  return "";
}

function BuildListCard({ build }: { build: BuildOrder }) {
  const race = build.race as Race;
  const accent = RACE_ACCENT[race] ?? "#7b8aaf";

  const handlePractice = () => {
    setPendingBuildSelect({ name: build.name, race: build.race });
    setPage("build-orders");
  };

  return (
    <article
      className="bls-card"
      style={{ "--bls-accent": accent } as CSSProperties}
    >
      <div className="bls-card-header">
        <span className="bls-card-race">{RACE_LABEL[race] ?? race}</span>
        {build.difficulty && (
          <span className={`bls-card-diff ${diffClass(build.difficulty)}`}>
            {build.difficulty}
          </span>
        )}
      </div>

      <h3 className="bls-card-name">{build.name}</h3>

      {build.vs && (
        <p className="bls-card-vs">vs. {build.vs}</p>
      )}

      {build.description && (
        <p className="bls-card-desc">{build.description}</p>
      )}

      <div className="bls-card-heroes">
        {build.heroes && build.heroes.length > 0 ? (
          build.heroes.map((h, i) => (
            <span
              key={h}
              className={`bls-card-hero-chip${i === 0 ? " bls-card-hero-chip--primary" : ""}`}
              title={h}
            >
              <HeroIconImg heroName={h} size={i === 0 ? 22 : 18} />
            </span>
          ))
        ) : (
          <span className="bls-card-hero-chip bls-card-hero-chip--fallback" title="Any Hero">
            <HeroIconImg size={18} />
          </span>
        )}
      </div>

      <div className="bls-card-footer">
        <span className="bls-card-steps">{build.steps.length} steps</span>
        <button
          type="button"
          className="bls-card-btn"
          onClick={handlePractice}
        >
          Practice
        </button>
      </div>
    </article>
  );
}

export default function BuildsListSection() {
  const allBuilds = useBuildOrders();
  const [filter, setFilter] = useState<Filter>("All");

  const visible =
    filter === "All" ? allBuilds : allBuilds.filter((b) => b.race === filter);

  return (
    <section className="section-card bls-section">
      <div className="bls-header">
        <div className="bls-header-left">
          <h2>All Builds</h2>
          <span className="bls-count">
            {visible.length} of {allBuilds.length}
          </span>
        </div>

        <div
          className="bls-filters"
          role="tablist"
          aria-label="Filter by race"
        >
          {FILTERS.map((f) => (
            <button
              key={f}
              type="button"
              role="tab"
              aria-selected={filter === f}
              className={`home-race-tab${filter === f ? " home-race-tab--active" : ""}`}
              onClick={() => setFilter(f)}
            >
              {FILTER_LABEL[f]}
            </button>
          ))}
        </div>
      </div>

      {visible.length === 0 ? (
        <p className="bls-empty">No builds found for this filter.</p>
      ) : (
        <div className="bls-grid">
          {visible.map((build, i) => (
            <BuildListCard key={`${build.race}-${build.name}-${i}`} build={build} />
          ))}
        </div>
      )}
    </section>
  );
}
