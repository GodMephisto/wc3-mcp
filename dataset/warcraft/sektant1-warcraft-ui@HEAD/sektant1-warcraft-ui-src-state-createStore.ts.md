---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/state/createStore.ts"
ref: HEAD
---

import { useSyncExternalStore } from 'react';

/**
 * Minimal external store compatible with React's useSyncExternalStore.
 * Allows module-level state that can be read/written from anywhere
 * and subscribed to from React components.
 */
export function createExternalStore<T>(initialValue: T) {
  let value = initialValue;
  const listeners = new Set<() => void>();

  function get(): T {
    return value;
  }

  function set(next: T | ((prev: T) => T)): void {
    const nextValue = typeof next === 'function'
      ? (next as (prev: T) => T)(value)
      : next;
    if (!Object.is(nextValue, value)) {
      value = nextValue;
      listeners.forEach(l => l());
    }
  }

  function subscribe(listener: () => void): () => void {
    listeners.add(listener);
    return () => { listeners.delete(listener); };
  }

  /** Hook — subscribe a React component to this store's value. */
  function useValue(): T {
    return useSyncExternalStore(subscribe, get);
  }

  return { get, set, useValue };
}
