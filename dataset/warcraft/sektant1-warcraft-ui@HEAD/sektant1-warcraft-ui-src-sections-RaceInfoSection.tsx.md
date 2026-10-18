---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/sections/RaceInfoSection.tsx"
ref: HEAD
---

import { useState } from "react";
import type { Race } from "../../lib/types";
import {
  RACE_DATA,
  type BuildingInfo,
  type UnitInfo,
  type HeroInfo,
  type HeroAttribute,
  type SkillInfo,
} from "../../lib/raceData";
import { ALL_HEROES } from "../../lib/heroConfig";
import { useCurrentRace, setCurrentRace, RACES } from "../../state/race";
import BlpIcon from "../BlpIcon";
import HeroPortraitModel from "../HeroPortraitModel";
import GlueSmallButton from "../GlueSmallButton";

type Tab = "buildings" | "units" | "heroes";

const CMD = (file: string) => `./buttons/command/${file}.blp`;
const RES = (file: string) => `./resources/${file}.blp`;

// Normalize hero name for lookup across naming inconsistencies
const normName = (s: string) => s.toLowerCase().replace(/\s+/g, "");

// ── Attack / Armor type metadata ──────────────────────────────────────────────

interface TypeMeta { symbol: string; color: string; tooltip: string; }

const ATTACK_META: Record<string, TypeMeta> = {
  Normal: { symbol: "⚔",  color: "#c8a855", tooltip: "Normal — Full damage to most armor. Reduced vs Fortified (50%)." },
  Pierce: { symbol: "↑",  color: "#7ab8e0", tooltip: "Pierce — Bonus vs Light (150%). Reduced vs Fortified (50%). Ideal vs air." },
  Siege:  { symbol: "💥", color: "#e07a40", tooltip: "Siege — Bonus vs Fortified (150%). Best for destroying buildings." },
  Magic:  { symbol: "✦",  color: "#a070e0", tooltip: "Magic — Bonus vs Medium (175%). 0 damage vs Magic Immune units." },
  Chaos:  { symbol: "☠",  color: "#e04040", tooltip: "Chaos — Full damage to ALL armor types with no reductions." },
  Spell:  { symbol: "✧",  color: "#80c0ff", tooltip: "Spell — Like Magic, but blocked by Spell Immunity and Spell Resistance." },
  Hero:   { symbol: "♛",  color: "#fcd312", tooltip: "Hero — Uses hero damage bonuses. Effective vs most armor types." },
};

const ARMOR_META: Record<string, TypeMeta> = {
  Unarmored: { symbol: "○", color: "#aab8c8", tooltip: "Unarmored — Vulnerable to all attack types." },
  Light:     { symbol: "◔", color: "#70c870", tooltip: "Light — Takes 150% from Pierce. Common on ranged and flying units." },
  Medium:    { symbol: "◑", color: "#c8c870", tooltip: "Medium — Takes 175% from Magic. Common on standard infantry." },
  Heavy:     { symbol: "◕", color: "#8080d0", tooltip: "Heavy — Resistant to Normal (75%) and Magic (50%). Armored melee." },
  Fortified: { symbol: "●", color: "#c87040", tooltip: "Fortified — Only Siege deals full damage. Used by buildings." },
  Hero:      { symbol: "♛", color: "#fcd312", tooltip: "Hero armor — Balanced resistances across all attack types." },
  Divine:    { symbol: "✡", color: "#ffffff", tooltip: "Divine — Immune to almost all damage. Extremely rare." },
};

function TypeChip({ label, value, kind }: { label: string; value: string; kind: "attack" | "armor" }) {
  const meta = (kind === "attack" ? ATTACK_META : ARMOR_META)[value];
  return (
    <div
      className={`ri-stat-chip ri-stat-chip--${kind}`}
      title={meta?.tooltip ?? value}
      style={meta ? { "--ri-type-color": meta.color } as React.CSSProperties : undefined}
    >
      {meta && <span className="ri-stat-symbol">{meta.symbol}</span>}
      <span className="ri-stat-label">{label}</span>
      <span className="ri-stat-value" style={meta ? { color: meta.color } : undefined}>{value}</span>
    </div>
  );
}

// ── Resource icons ────────────────────────────────────────────────────────────

function GoldIcon()   { return <BlpIcon path={RES("ResourceGold")}   size={16} className="ri-res-icon" title="Gold"   />; }
function LumberIcon() { return <BlpIcon path={RES("ResourceLumber")} size={16} className="ri-res-icon" title="Lumber" />; }
function FoodIcon()   { return <BlpIcon path={RES("ResourceSupply")} size={16} className="ri-res-icon" title="Food"   />; }

// ── Building table ────────────────────────────────────────────────────────────

function BuildingTable({ buildings }: { buildings: BuildingInfo[] }) {
  const [expanded, setExpanded] = useState<string | null>(null);

  return (
    <div className="ri-hero-table-wrap">
      <table className="ri-hero-table">
        <thead>
          <tr>
            <th>Building</th>
            <th>Hit Points</th>
            <th>Armor</th>
            <th>Production</th>
            <th><GoldIcon /></th>
            <th><LumberIcon /></th>
          </tr>
        </thead>
        <tbody>
          {buildings.map((b) => {
            const isExpanded = expanded === b.id;
            return (
              <>
                <tr
                  key={b.id}
                  className={`ri-hero-row${isExpanded ? " ri-hero-row--expanded" : ""}`}
                  onClick={() => setExpanded(isExpanded ? null : b.id)}
                >
                  <td>
                    <div className="ri-htd-hero">
                      <BlpIcon path={CMD(b.icon)} size={36} className="ri-htd-icon" />
                      <span className="ri-htd-name">{b.name}</span>
                    </div>
                  </td>
                  <td className="ri-htd-stat ri-htd-hp">{b.hp}</td>
                  <td className="ri-htd-stat">{b.armor} (Fortified)</td>
                  <td className="ri-htd-stat">{b.buildTime > 0 ? `${b.buildTime}s` : "—"}</td>
                  <td className="ri-htd-stat ri-htd-gold">{b.gold}</td>
                  <td className="ri-htd-stat ri-htd-lumber">{b.lumber}</td>
                </tr>

                {isExpanded && (
                  <tr key={`${b.id}-detail`} className="ri-hero-detail-row">
                    <td colSpan={6}>
                      <div className="ri-unit-detail-inner">
                        {b.foodGiven !== undefined && (
                          <div className="ri-unit-detail-info">
                            <span className="ri-info-label">Food</span>
                            <span className="ri-info-value">+{b.foodGiven}</span>
                          </div>
                        )}
                        {b.prerequisites.length > 0 && (
                          <div className="ri-unit-detail-matchup">
                            <span className="ri-info-label">Requires</span>
                            <div className="ri-tag-list">
                              {b.prerequisites.map((p) => <span key={p} className="ri-tag ri-tag--req">{p}</span>)}
                            </div>
                          </div>
                        )}
                        {b.trains.length > 0 && (
                          <div className="ri-unit-detail-matchup">
                            <span className="ri-info-label">Trains</span>
                            <div className="ri-tag-list">
                              {b.trains.map((t) => <span key={t} className="ri-tag ri-tag--train">{t}</span>)}
                            </div>
                          </div>
                        )}
                        {b.unlocks && b.unlocks.length > 0 && (
                          <div className="ri-unit-detail-matchup">
                            <span className="ri-info-label">Unlocks</span>
                            <div className="ri-tag-list">
                              {b.unlocks.map((u) => <span key={u} className="ri-tag ri-tag--unlock">{u}</span>)}
                            </div>
                          </div>
                        )}
                      </div>
                    </td>
                  </tr>
                )}
              </>
            );
          })}
        </tbody>
      </table>
    </div>
  );
}

// ── Unit table ────────────────────────────────────────────────────────────────

function UnitTable({ units }: { units: UnitInfo[] }) {
  const [expanded, setExpanded] = useState<string | null>(null);

  return (
    <div className="ri-hero-table-wrap">
      <table className="ri-hero-table">
        <thead>
          <tr>
            <th>Unit</th>
            <th>Damage</th>
            <th>HP</th>
            <th>Mana</th>
            <th>Armor</th>
            <th>Production</th>
            <th><GoldIcon /></th>
            <th><LumberIcon /></th>
            <th><FoodIcon /></th>
          </tr>
        </thead>
        <tbody>
          {units.map((u) => {
            const isExpanded = expanded === u.id;
            return (
              <>
                <tr
                  key={u.id}
                  className={`ri-hero-row${isExpanded ? " ri-hero-row--expanded" : ""}`}
                  onClick={() => setExpanded(isExpanded ? null : u.id)}
                >
                  <td>
                    <div className="ri-htd-hero">
                      <BlpIcon path={CMD(u.icon)} size={38} className="ri-htd-icon" />
                      <span className="ri-htd-name">{u.name}</span>
                    </div>
                  </td>
                  <td className="ri-htd-stat">{u.damage}</td>
                  <td className="ri-htd-stat ri-htd-hp">{u.hp}</td>
                  <td className="ri-htd-stat ri-htd-mana">{u.mana}</td>
                  <td className="ri-htd-stat">{u.armor} ({u.armorType})</td>
                  <td className="ri-htd-stat">{u.buildTime > 0 ? `${u.buildTime}s` : "—"}</td>
                  <td className="ri-htd-stat ri-htd-gold">{u.gold}</td>
                  <td className="ri-htd-stat ri-htd-lumber">{u.lumber}</td>
                  <td className="ri-htd-stat ri-htd-food">{u.food}</td>
                </tr>

                {isExpanded && (
                  <tr key={`${u.id}-detail`} className="ri-hero-detail-row">
                    <td colSpan={9}>
                      <div className="ri-unit-detail-inner">
                        <div className="ri-unit-detail-types">
                          <TypeChip label="Attack" value={u.attackType} kind="attack" />
                          <TypeChip label="Armor"  value={u.armorType}  kind="armor"  />
                        </div>
                        <div className="ri-unit-detail-info">
                          <span className="ri-info-label">Trained at</span>
                          <span className="ri-info-value">{u.trainedAt}</span>
                          {u.requires && u.requires.length > 0 && (
                            <>
                              <span className="ri-info-label">Requires</span>
                              <div className="ri-tag-list">
                                {u.requires.map((r) => <span key={r} className="ri-tag ri-tag--req">{r}</span>)}
                              </div>
                            </>
                          )}
                        </div>
                        {u.goodAgainst.length > 0 && (
                          <div className="ri-unit-detail-matchup">
                            <span className="ri-info-label ri-info-label--good">Strong vs</span>
                            <div className="ri-tag-list">
                              {u.goodAgainst.map((g) => <span key={g} className="ri-tag ri-tag--good">{g}</span>)}
                            </div>
                          </div>
                        )}
                        {u.counters.length > 0 && (
                          <div className="ri-unit-detail-matchup">
                            <span className="ri-info-label ri-info-label--weak">Weak vs</span>
                            <div className="ri-tag-list">
                              {u.counters.map((c) => <span key={c} className="ri-tag ri-tag--weak">{c}</span>)}
                            </div>
                          </div>
                        )}
                      </div>
                    </td>
                  </tr>
                )}
              </>
            );
          })}
        </tbody>
      </table>
    </div>
  );
}

// ── Attribute chip ────────────────────────────────────────────────────────────

const ATTR_META: Record<HeroAttribute, { label: string; color: string; abbr: string }> = {
  Strength:     { label: "Strength",     color: "#e05050", abbr: "STR" },
  Agility:      { label: "Agility",      color: "#50c870", abbr: "AGI" },
  Intelligence: { label: "Intelligence", color: "#5090e0", abbr: "INT" },
};

function AttributeChip({ attr }: { attr: HeroAttribute }) {
  const m = ATTR_META[attr];
  return (
    <span className="ri-attr-chip" style={{ "--ri-attr-color": m.color } as React.CSSProperties}>
      <span className="ri-attr-abbr">{m.abbr}</span>
      <span className="ri-attr-label">{m.label}</span>
    </span>
  );
}

// ── Hero table ────────────────────────────────────────────────────────────────

function HeroTable({ heroes, race }: { heroes: HeroInfo[]; race: Race }) {
  const [expandedHero, setExpandedHero] = useState<string | null>(null);
  const [activeSkill, setActiveSkill] = useState<SkillInfo | null>(null);

  function toggleHero(name: string, firstSkill: SkillInfo) {
    if (expandedHero === name) {
      setExpandedHero(null);
      setActiveSkill(null);
    } else {
      setExpandedHero(name);
      setActiveSkill(firstSkill);
    }
  }

  return (
    <div className="ri-hero-table-wrap">
      <table className="ri-hero-table">
        <thead>
          <tr>
            <th>Hero</th>
            <th>Attribute</th>
            <th>HP</th>
            <th>Mana</th>
            <th>Damage</th>
            <th>Armor</th>
            <th>Abilities</th>
          </tr>
        </thead>
        <tbody>
          {heroes.map((h) => {
            const expanded = expandedHero === h.name;
            const portraitCfg = ALL_HEROES.find(
              (c) => normName(c.name) === normName(h.name) && c.faction === race,
            );
            const hasModel = !!portraitCfg?.modelPath;
            const skill = activeSkill && expanded ? activeSkill : h.skills[0];

            return (
              <>
                <tr
                  key={h.name}
                  className={`ri-hero-row${expanded ? " ri-hero-row--expanded" : ""}`}
                  onClick={() => toggleHero(h.name, h.skills[0])}
                >
                  <td>
                    <div className="ri-htd-hero">
                      <BlpIcon path={CMD(h.icon)} size={40} className="ri-htd-icon" />
                      <span className="ri-htd-name">{h.name}</span>
                    </div>
                  </td>
                  <td><AttributeChip attr={h.attribute} /></td>
                  <td className="ri-htd-stat ri-htd-hp">{h.hp}</td>
                  <td className="ri-htd-stat ri-htd-mana">{h.mana}</td>
                  <td className="ri-htd-stat">{h.damage}</td>
                  <td className="ri-htd-stat">{h.armor}</td>
                  <td>
                    <div className="ri-htd-skills">
                      {h.skills.map((s) => (
                        <button
                          key={s.name}
                          type="button"
                          className={`ri-htd-skill-btn${expanded && activeSkill?.name === s.name ? " ri-htd-skill-btn--active" : ""}${s.isUltimate ? " ri-htd-skill-btn--ult" : ""}`}
                          title={s.name}
                          onClick={(e) => { e.stopPropagation(); setExpandedHero(h.name); setActiveSkill(s); }}
                        >
                          <BlpIcon path={CMD(s.icon)} size={32} />
                          {s.isPassive && <span className="ri-htd-skill-dot ri-htd-skill-dot--passive" />}
                          {s.isUltimate && <span className="ri-htd-skill-dot ri-htd-skill-dot--ult" />}
                        </button>
                      ))}
                    </div>
                  </td>
                </tr>

                {expanded && (
                  <tr key={`${h.name}-detail`} className="ri-hero-detail-row">
                    <td colSpan={7}>
                      <div className="ri-hero-detail-inner">
                        {/* Portrait */}
                        <div className="ri-hero-detail-portrait">
                          {hasModel && portraitCfg ? (
                            <HeroPortraitModel
                              modelPath={portraitCfg.modelPath}
                              textures={portraitCfg.textures}
                            />
                          ) : (
                            <div className="ri-hero-portrait-placeholder">
                              <BlpIcon path={CMD(h.icon)} size={48} className="ri-hero-portrait-fallback-icon" />
                              <span className="ri-hero-portrait-hint">portrait coming soon</span>
                            </div>
                          )}
                        </div>

                        {/* Skill detail */}
                        <div className="ri-hero-detail-skill">
                          <div className="ri-skill-desc-header">
                            <BlpIcon path={CMD(skill.icon)} size={28} className="ri-skill-desc-icon" />
                            <span className="ri-skill-desc-name">{skill.name}</span>
                            {skill.isUltimate && <span className="ri-skill-badge ri-skill-badge--ult">Ultimate</span>}
                            {skill.isPassive && <span className="ri-skill-badge ri-skill-badge--passive">Passive</span>}
                          </div>
                          <p className="ri-skill-desc-text">{skill.description}</p>
                          <p className="ri-hero-detail-role">{h.role}</p>
                        </div>
                      </div>
                    </td>
                  </tr>
                )}
              </>
            );
          })}
        </tbody>
      </table>
    </div>
  );
}

const RACE_LABELS: Record<Race, string> = {
  Human: "Human",
  Orc: "Orc",
  NightElf: "Night Elf",
  Undead: "Undead",
};

// ── Main ──────────────────────────────────────────────────────────────────────

export default function RaceInfoSection() {
  const globalRace = useCurrentRace() ?? "Human";
  const [race, setRace] = useState<Race>(globalRace);

  const data = RACE_DATA[race];
  const [tab, setTab] = useState<Tab>("buildings");

  function handleRaceChange(r: Race) {
    setRace(r);
    setCurrentRace(r);
  }

  return (
    <section className="section-card ri-section" key={race}>
      {/* Race selector */}
      <div className="ri-race-tabs" role="tablist" aria-label="Race selection">
        {RACES.map((r) => (
          <GlueSmallButton
            key={r}
            className={race === r ? "home-race-tab--active" : ""}
            onClick={() => handleRaceChange(r)}
          >
            {RACE_LABELS[r]}
          </GlueSmallButton>
        ))}
      </div>

      {/* Header */}
      <div className="ri-header">
        <h2 className="ri-title">Army</h2>
        <div className="ri-tabs" role="tablist">
          {(["buildings", "units", "heroes"] as Tab[]).map((t) => (
            <GlueSmallButton
              key={t}
              className={tab === t ? "home-race-tab--active" : ""}
              onClick={() => setTab(t)}
            >
              {t === "buildings" ? "Buildings" : t === "units" ? "Units" : "Heroes"}
            </GlueSmallButton>
          ))}
        </div>
      </div>

      {/* Detail panel */}
      <div className="ri-detail-wrap">
        {tab === "buildings" && <BuildingTable key={race} buildings={data.buildings} />}
        {tab === "units"     && <UnitTable     key={race} units={data.units}         />}
        {tab === "heroes"    && <HeroTable     key={race} heroes={data.heroes} race={race} />}
      </div>
    </section>
  );
}
