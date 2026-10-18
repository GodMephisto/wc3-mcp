---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/state/page.ts"
ref: HEAD
---

import { createExternalStore } from "./createStore";

const pageStore = createExternalStore<string>("home");

export const currentPage = pageStore.get;
export const setPage = pageStore.set;
export const usePage = pageStore.useValue;
