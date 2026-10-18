---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/state/race.ts"
ref: HEAD
---

import { createExternalStore } from "./createStore";
import type { Race, RacePrefix } from "../utils/types";

export const RACES: Race[] = ["Human", "Orc", "NightElf", "Undead"];

export const RACE_PREFIXES: Record<Race, RacePrefix> = {
  Human: { tile: "Human", esc: "human", lower: "human", display: "Human" },
  Orc: { tile: "Orc", esc: "orc", lower: "orc", display: "Orc" },
  NightElf: {
    tile: "NightElf",
    esc: "nightelf",
    lower: "nightelf",
    display: "Night Elf",
  },
  Undead: { tile: "Undead", esc: "undead", lower: "undead", display: "Undead" },
};

const raceStore = createExternalStore<Race>("Human");

/** Read current race (non-reactive — use useCurrentRace in components). */
export const currentRace = raceStore.get;

/** Set current race (works anywhere). */
export const setCurrentRace = raceStore.set;

/** Hook — subscribe to current race in a React component. */
export const useCurrentRace = raceStore.useValue;
