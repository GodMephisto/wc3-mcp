---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/utils/heroConfig.ts"
ref: HEAD
---

import type { Race } from "./types";

export type HeroFaction = Race | "Tavern";

export interface HeroConfig {
  id: string;
  name: string;
  faction: HeroFaction;
  /** MDX model path. Empty string = file not yet added. */
  modelPath: string;
  /** Map from normalised texture key → public path. */
  textures: Record<string, string>;
}

export const HERO_FACTIONS: HeroFaction[] = [
  "Human",
  "Orc",
  "NightElf",
  "Undead",
  "Tavern",
];

export const FACTION_DISPLAY: Record<HeroFaction, string> = {
  Human: "Human",
  Orc: "Orc",
  NightElf: "Night Elf",
  Undead: "Undead",
  Tavern: "Tavern",
};

export const ALL_HEROES: HeroConfig[] = [
  // ── Human ──────────────────────────────────────────────────────────────
  {
    id: "archmage",
    name: "Archmage",
    faction: "Human",
    modelPath: "./models/hero-portrait/Human/HeroArchMage_portrait.mdx",
    textures: {
      "heroarchmage.blp":
        "./models/hero-portrait/textures/Human/HeroArchmage.blp",
    },
  },
  {
    id: "mountain-king",
    name: "Mountain King",
    faction: "Human",
    modelPath: "",
    textures: {},
  },
  {
    id: "paladin",
    name: "Paladin",
    faction: "Human",
    modelPath: "",
    textures: {},
  },
  {
    id: "blood-mage",
    name: "Blood Mage",
    faction: "Human",
    modelPath: "",
    textures: {},
  },

  // ── Orc ────────────────────────────────────────────────────────────────
  {
    id: "blade-master",
    name: "Blade Master",
    faction: "Orc",
    modelPath: "./models/hero-portrait/Orc/HeroBladeMaster_portrait.mdx",
    textures: {
      "heroblademaster.blp":
        "./models/hero-portrait/textures/Orc/HeroBladeMaster.blp",
    },
  },
  {
    id: "far-seer",
    name: "Far Seer",
    faction: "Orc",
    modelPath: "",
    textures: {},
  },
  {
    id: "tauren-chieftain",
    name: "Tauren Chieftain",
    faction: "Orc",
    modelPath: "",
    textures: {},
  },
  {
    id: "shadow-hunter",
    name: "Shadow Hunter",
    faction: "Orc",
    modelPath: "",
    textures: {},
  },

  // ── Night Elf ───────────────────────────────────────────────────────────
  {
    id: "demon-hunter",
    name: "Demon Hunter",
    faction: "NightElf",
    modelPath:
      "./models/hero-portrait/NightElf/HeroDemonHunter_Portrait.mdx",
    textures: {
      "herodemonhunter.blp":
        "./models/hero-portrait/textures/NightElf/HeroDemonHunter.blp",
      "black32.blp":
        "./models/hero-portrait/textures/NightElf/Black32.blp",
    },
  },
  {
    id: "keeper-of-grove",
    name: "Keeper of the Grove",
    faction: "NightElf",
    modelPath: "",
    textures: {},
  },
  {
    id: "priestess-of-moon",
    name: "Priestess of the Moon",
    faction: "NightElf",
    modelPath: "",
    textures: {},
  },
  {
    id: "warden",
    name: "Warden",
    faction: "NightElf",
    modelPath: "",
    textures: {},
  },

  // ── Undead ─────────────────────────────────────────────────────────────
  {
    id: "death-knight",
    name: "Death Knight",
    faction: "Undead",
    modelPath:
      "./models/hero-portrait/Undead/HeroDeathKnight_portrait.mdx",
    textures: {
      "herodeathknight.blp":
        "./models/hero-portrait/textures/Undead/HeroDeathknight.blp",
    },
  },
  {
    id: "lich",
    name: "Lich",
    faction: "Undead",
    modelPath: "",
    textures: {},
  },
  {
    id: "dread-lord",
    name: "Dread Lord",
    faction: "Undead",
    modelPath: "",
    textures: {},
  },
  {
    id: "crypt-lord",
    name: "Crypt Lord",
    faction: "Undead",
    modelPath: "",
    textures: {},
  },

  // ── Tavern ─────────────────────────────────────────────────────────────
  {
    id: "dark-ranger",
    name: "Dark Ranger",
    faction: "Tavern",
    modelPath: "",
    textures: {},
  },
  {
    id: "pit-lord",
    name: "Pit Lord",
    faction: "Tavern",
    modelPath: "",
    textures: {},
  },
  {
    id: "pandaren",
    name: "Pandaren Brewmaster",
    faction: "Tavern",
    modelPath: "",
    textures: {},
  },
  {
    id: "sea-witch",
    name: "Sea Witch",
    faction: "Tavern",
    modelPath: "",
    textures: {},
  },
  {
    id: "beastmaster",
    name: "Beastmaster",
    faction: "Tavern",
    modelPath: "",
    textures: {},
  },
  {
    id: "tinker",
    name: "Goblin Tinker",
    faction: "Tavern",
    modelPath: "",
    textures: {},
  },
  {
    id: "alchemist",
    name: "Goblin Alchemist",
    faction: "Tavern",
    modelPath: "",
    textures: {},
  },
  {
    id: "firelord",
    name: "Firelord",
    faction: "Tavern",
    modelPath: "",
    textures: {},
  },
];

export function getHeroesByFaction(faction: HeroFaction): HeroConfig[] {
  return ALL_HEROES.filter((h) => h.faction === faction);
}

/** First hero with a modelPath set, or the first hero if none are ready. */
export function getDefaultHero(faction: HeroFaction): HeroConfig {
  const heroes = getHeroesByFaction(faction);
  return (heroes.find((h) => h.modelPath !== "") ?? heroes[0])!;
}
