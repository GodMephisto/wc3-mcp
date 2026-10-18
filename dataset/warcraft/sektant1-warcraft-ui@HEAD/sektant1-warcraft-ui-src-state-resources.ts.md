---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/state/resources.ts"
ref: HEAD
---

import { createExternalStore } from './createStore';

const goldTargetStore = createExternalStore(500);
const goldCurrentStore = createExternalStore(500);
const lumberTargetStore = createExternalStore(300);
const lumberCurrentStore = createExternalStore(300);

function stepCounter(current: number, target: number): number {
  if (current === target) return current;
  const diff = target - current;
  const next = current + Math.sign(diff) * Math.max(1, Math.floor(Math.abs(diff) * 0.2));
  return Math.abs(target - next) < 2 ? target : next;
}

export function tickResources() {
  goldTargetStore.set(prev => Math.max(0, Math.min(9999, prev + Math.floor(Math.random() * 20) - 5)));
  lumberTargetStore.set(prev => Math.max(0, Math.min(9999, prev + Math.floor(Math.random() * 15) - 4)));
}

export function animateCounters() {
  goldCurrentStore.set(prev => stepCounter(prev, goldTargetStore.get()));
  lumberCurrentStore.set(prev => stepCounter(prev, lumberTargetStore.get()));
}

// Plain getters (non-reactive — use hooks in components)
export const goldCurrent = goldCurrentStore.get;
export const lumberCurrent = lumberCurrentStore.get;
export const goldTarget = goldTargetStore.get;
export const lumberTarget = lumberTargetStore.get;

// Setters (work anywhere)
export const setGoldTarget = goldTargetStore.set;
export const setLumberTarget = lumberTargetStore.set;

// Auto-start animation loops on first hook use
let tickInterval: ReturnType<typeof setInterval> | null = null;
let rafId = 0;

function startLoops() {
  if (tickInterval) return;
  tickInterval = setInterval(tickResources, 1000);
  const loop = () => {
    animateCounters();
    rafId = requestAnimationFrame(loop);
  };
  rafId = requestAnimationFrame(loop);
}

// Hooks (subscribe in React components) — auto-start animation loops on first use
export const useGoldCurrent = () => {
  startLoops();
  return goldCurrentStore.useValue();
};
export const useLumberCurrent = () => {
  startLoops();
  return lumberCurrentStore.useValue();
};
export const useGoldTarget = goldTargetStore.useValue;
export const useLumberTarget = lumberTargetStore.useValue;

// Cheat codes
let cheatBuffer = '';
const CHEATS: Record<string, () => void> = {
  greedisgood: () => {
    goldTargetStore.set(prev => Math.min(9999, prev + 500));
    lumberTargetStore.set(prev => Math.min(9999, prev + 500));
  },
  keysersoze: () => { goldTargetStore.set(prev => Math.min(9999, prev + 500)); },
  leafittome: () => { lumberTargetStore.set(prev => Math.min(9999, prev + 500)); },
};

export function handleCheatKey(key: string) {
  if (key.length === 1 && /[a-z]/i.test(key)) {
    cheatBuffer += key.toLowerCase();
    if (cheatBuffer.length > 20) cheatBuffer = cheatBuffer.slice(-20);
    for (const [code, fn] of Object.entries(CHEATS)) {
      if (cheatBuffer.endsWith(code)) {
        fn();
        cheatBuffer = '';
        break;
      }
    }
  }
}
