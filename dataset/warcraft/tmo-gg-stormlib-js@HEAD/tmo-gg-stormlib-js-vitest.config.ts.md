---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "vitest.config.ts"
ref: HEAD
---

import { defineConfig } from 'vitest/config';

export default defineConfig({
  test: {
    globals: true,
    testTimeout: 10000,
  },
});
