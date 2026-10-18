---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "tsup.config.ts"
ref: HEAD
---

import { defineConfig } from 'tsup';

export default defineConfig({
  entry: ['src/index.ts', 'src/index.browser.ts'],
  format: ['cjs', 'esm'],
  dts: true,
  clean: true,
  sourcemap: true,
  target: 'es2020',
});
