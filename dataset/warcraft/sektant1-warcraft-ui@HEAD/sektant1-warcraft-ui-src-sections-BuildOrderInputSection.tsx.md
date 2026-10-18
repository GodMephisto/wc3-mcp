---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/sections/BuildOrderInputSection.tsx"
ref: HEAD
---

import { useState } from "react";
import type { Race } from "../../lib/types";
import EscEditBox from "../EscEditBox";
import BnetEditBox from "../BnetEditBox";
import GlueDropdown from "../GlueDropdown";
import GlueSmallButton from "../GlueSmallButton";
import GlueBorderedButton from "../GlueBorderedButton";
import GlueMenuButton from "../GlueMenuButton";
import BuildCheckbox from "../BuildCheckbox";
import { importBuilds } from "../../state/buildOrders";

const RACES: Race[] = ["Human", "Orc", "NightElf", "Undead"];
const RACE_LABELS: Record<Race, string> = {
  Human: "Human",
  Orc: "Orc",
  NightElf: "Night Elf",
  Undead: "Undead",
};
const DIFFICULTIES = ["Beginner", "Intermediate", "Advanced"] as const;
type Difficulty = (typeof DIFFICULTIES)[number];

const SKILL_ICON_OVERRIDES: Record<string, string> = {
  "Holy Light": "BTNHolyBolt",
  Bladestorm: "BTNWhirlwind",
  "Wind Walk": "BTNWindWalkOn",
  "Devotion Aura": "BTNDevotion",
  "Divine Shield": "BTNDivineIntervention",
  Resurrection: "BTNResurrection",
  "Storm Bolt": "BTNStormBolt",
  "Thunder Clap": "BTNThunderClap",
  Avatar: "BTNAvatar",
  "Flame Strike": "BTNWallOfFire",
  Banish: "BTNBanish",
  Phoenix: "BTNMarkOfFire",
  "Brilliance Aura": "BTNBrilliance",
  "Water Elemental": "BTNSummonWaterElemental",
  Blizzard: "BTNBlizzard",
  "Mass Teleport": "BTNMassTeleport",
  "Feral Spirit": "BTNSpiritWolf",
  "Chain Lightning": "BTNChainLightning",
  Earthquake: "BTNEarthquake",
  "Healing Wave": "BTNHealingWave",
  Hex: "BTNHex",
  "Serpent Ward": "BTNSerpentWard",
  "Big Bad Voodoo": "BTNBigBadVoodoo",
  "War Stomp": "BTNWarStomp",
  "Endurance Aura": "BTNCommand",
  Reincarnation: "BTNReincarnation",
  "Shadow Strike": "BTNShadowStrike",
  "Fan of Knives": "BTNFanOfKnives",
  Blink: "BTNBlink",
  Vengeance: "BTNVengeance",
  "Mana Burn": "BTNManaBurn",
  Evasion: "BTNEvasion",
  Metamorphosis: "BTNMetamorphosis",
  "Critical Strike": "BTNCriticalStrike",
  "Force of Nature": "BTNEnt",
  "Entangling Roots": "BTNEntanglingRoots",
  Tranquility: "BTNTranquility",
  Scout: "BTNSentinel",
  "Trueshot Aura": "BTNTrueShot",
  Starfall: "BTNStarFall",
  "Frost Nova": "BTNGlacier",
  "Dark Ritual": "BTNDarkRitual",
  "Frost Armor": "BTNFrostArmor",
  "Death and Decay": "BTNDeathAndDecay",
  Impale: "BTNImpale",
  "Spiked Carapace": "BTNThornShield",
  "Locust Swarm": "BTNLocustSwarm",
  "Death Coil": "BTNDeathCoil",
  "Unholy Aura": "BTNUnholyAura",
  "Animate Dead": "BTNAnimateDead",
  "Death Pact": "BTNDeathPact",
};

function skillToIconPath(skillName: string): string {
  const override = SKILL_ICON_OVERRIDES[skillName];
  if (override) return `./buttons/command/${override}.blp`;
  const pascal = skillName.replace(/\s+/g, "");
  return `./buttons/command/BTN${pascal}.blp`;
}

interface StepRow {
  time: string;
  instruction: string;
  food: string;
  isKey: boolean;
}

interface SkillRow {
  level: string;
  skill: string;
}

export default function BuildOrderInputSection() {
  const [name, setName] = useState("");
  const [race, setRace] = useState<Race>("Human");
  const [vs, setVs] = useState("");
  const [difficulty, setDifficulty] = useState<Difficulty>("Beginner");
  const [description, setDescription] = useState("");
  const [note, setNote] = useState("");
  const [heroesInput, setHeroesInput] = useState("");
  const [steps, setSteps] = useState<StepRow[]>([
    { time: "0:00", instruction: "", food: "", isKey: false },
  ]);
  const [skills, setSkills] = useState<SkillRow[]>([]);
  const [status, setStatus] = useState("");

  function formatTimeInput(raw: string): string {
    const digits = raw.replace(/\D/g, "").slice(0, 4);

    if (digits.length === 0) return "";

    const ssRaw = digits.slice(-2);
    const mmRaw = digits.slice(0, -2);

    const secNum = Math.min(59, Number(ssRaw));
    const ss = secNum.toString().padStart(2, "0");

    const mm = mmRaw.length ? String(Number(mmRaw)) : "0";

    return `${mm}:${ss}`;
  }

  function formatFoodInput(raw: string): string {
    const digits = raw.replace(/\D/g, "").slice(0, 4);

    if (digits.length === 0) return "";

    const bRaw = digits.slice(-2);
    const aRaw = digits.slice(0, -2);

    const a = aRaw.length ? String(Number(aRaw)) : "0";
    const b = String(Number(bRaw)); // removes leading zero

    return `${a}/${b}`;
  }

  function updateStep<K extends keyof StepRow>(
    index: number,
    field: K,
    value: StepRow[K],
  ) {
    setSteps((prev) =>
      prev.map((s, i) => (i === index ? { ...s, [field]: value } : s)),
    );
  }

  function addStep() {
    setSteps((prev) => [
      ...prev,
      { time: "", instruction: "", food: "", isKey: false },
    ]);
  }

  function removeStep(index: number) {
    setSteps((prev) => prev.filter((_, i) => i !== index));
  }

  function updateSkill(index: number, field: keyof SkillRow, value: string) {
    setSkills((prev) =>
      prev.map((s, i) => (i === index ? { ...s, [field]: value } : s)),
    );
  }

  function addSkill() {
    setSkills((prev) => [
      ...prev,
      { level: String(prev.length + 1), skill: "" },
    ]);
  }

  function removeSkill(index: number) {
    setSkills((prev) => prev.filter((_, i) => i !== index));
  }

  function handleSubmit() {
    const heroes = heroesInput
      .split(",")
      .map((h) => h.trim())
      .filter(Boolean);
    const build = {
      name,
      race,
      heroes,
      vs: vs || undefined,
      difficulty,
      description,
      note: note || undefined,
      steps: steps.map((s, i) => ({
        order: i + 1,
        time: s.time,
        instruction: s.instruction,
        food: s.food,
        ...(s.isKey ? { isKey: true as const } : {}),
      })),
      skillOrder:
        skills.length > 0
          ? skills.map((s) => ({
              level: Number(s.level),
              skill: s.skill,
              icon: skillToIconPath(s.skill),
            }))
          : undefined,
    };

    const count = importBuilds([build]);
    if (count > 0) {
      setStatus("Build order added!");
      setName("");
      setVs("");
      setDescription("");
      setNote("");
      setHeroesInput("");
      setSteps([{ time: "0:00", instruction: "", food: "", isKey: false }]);
      setSkills([]);
    } else {
      setStatus("Fill in Name and at least one complete step.");
    }
    setTimeout(() => setStatus(""), 4000);
  }

  return (
    <section className="section-card boi-section">
      {/* ── Header ── */}
      <div className="boi-header">
        <h3>Add Build Order</h3>
        <div>
          {status && <span className="boi-status">{status}</span>}
          <GlueMenuButton onClick={handleSubmit}>+Add Build</GlueMenuButton>
        </div>
      </div>

      {/* ── Metadata ── */}
      <div className="boi-meta">
        <div className="boi-grid">
          <div className="boi-field">
            <span className="boi-label">Name</span>
            <EscEditBox
              value={name}
              placeholder="Build order name"
              onChange={setName}
            />
          </div>
          <div className="boi-field">
            <span className="boi-label">Race</span>
            <GlueDropdown
              options={RACES}
              value={race}
              onChange={(v) => setRace(v as Race)}
              label={(v) => RACE_LABELS[v as Race] ?? v}
            />
          </div>
          <div className="boi-field">
            <span className="boi-label">vs</span>
            <GlueDropdown
              options={["Any" as const, ...RACES]}
              value={vs || ("Any" as string)}
              onChange={(v) => setVs(v === "Any" ? "" : v)}
              label={(v) =>
                v === "Any" ? "Any" : (RACE_LABELS[v as Race] ?? v)
              }
            />
          </div>
          <div className="boi-field">
            <span className="boi-label">Difficulty</span>
            <GlueDropdown
              options={DIFFICULTIES}
              value={difficulty}
              onChange={(v) => setDifficulty(v as Difficulty)}
            />
          </div>
        </div>
        <div className="boi-meta-row">
          <div className="boi-field">
            <span className="boi-label">Heroes</span>
            <EscEditBox
              value={heroesInput}
              placeholder="e.g. Death Knight, Lich"
              onChange={setHeroesInput}
            />
          </div>
          <div className="boi-field boi-field--grow">
            <span className="boi-label">Description</span>
            <EscEditBox
              value={description}
              placeholder="Short description"
              onChange={setDescription}
            />
          </div>
          <div className="boi-field">
            <span className="boi-label">Note</span>
            <EscEditBox
              value={note}
              placeholder="Optional note"
              onChange={setNote}
            />
          </div>
        </div>
      </div>
      {/* ── Steps ── */}
      <div className="boi-steps-section">
        <div className="boi-section-header">
          <h4 className="boi-sub-title">Steps</h4>
          <GlueSmallButton onClick={addStep}>+ Add Step</GlueSmallButton>
        </div>
        <table className="boi-steps-table">
          <thead>
            <tr>
              <th>#</th>
              <th>Time</th>
              <th>Instruction</th>
              <th>Food</th>
              <th>⚡</th>
              <th />
            </tr>
          </thead>
          <tbody>
            {steps.map((step, i) => (
              <tr
                key={i}
                className={`boi-step-row ${step.isKey ? "is-key" : ""}`}
              >
                <td className="boi-step-num">{i + 1}</td>
                <td className="boi-col-time">
                  <BnetEditBox
                    value={step.time}
                    placeholder="0:00"
                    onChange={(v) => updateStep(i, "time", formatTimeInput(v))}
                  />
                </td>
                <td className="boi-col-instruction">
                  <BnetEditBox
                    value={step.instruction}
                    placeholder="Instruction"
                    onChange={(v) => updateStep(i, "instruction", v)}
                  />
                </td>
                <td className="boi-col-food">
                  <BnetEditBox
                    value={step.food}
                    placeholder="5/10"
                    onChange={(v) => updateStep(i, "food", formatFoodInput(v))}
                  />
                </td>
                <td className="boi-col-key">
                  <BuildCheckbox
                    checked={step.isKey}
                    onChange={(v) => updateStep(i, "isKey", v)}
                  />
                </td>
                <td className="boi-col-remove">
                  <GlueSmallButton
                    onClick={() => removeStep(i)}
                    disabled={steps.length <= 1}
                  >
                    ✕
                  </GlueSmallButton>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      {/* ── Skill Order ── */}
      <div className="boi-skills-section">
        <div className="boi-section-header">
          <h4 className="boi-sub-title">Skill Order</h4>
          <GlueSmallButton onClick={addSkill}>+ Add Skill</GlueSmallButton>
        </div>
        {skills.length > 0 && (
          <div className="boi-skill-list">
            {skills.map((skill, i) => (
              <div key={i} className="boi-skill-item">
                <span className="boi-step-num">Lv{skill.level}</span>
                <BnetEditBox
                  value={skill.skill}
                  placeholder="e.g. Storm Bolt"
                  onChange={(v) => updateSkill(i, "skill", v)}
                />
                {skill.skill && (
                  <img
                    src={skillToIconPath(skill.skill)}
                    className="boi-skill-icon"
                  />
                )}
                <GlueSmallButton onClick={() => removeSkill(i)}>
                  ✕
                </GlueSmallButton>
              </div>
            ))}
          </div>
        )}
      </div>
    </section>
  );
}
