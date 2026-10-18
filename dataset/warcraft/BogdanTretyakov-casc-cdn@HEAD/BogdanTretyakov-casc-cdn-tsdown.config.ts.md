---
type: vendor-doc
source_repo: "https://github.com/BogdanTretyakov/casc-cdn"
source_path: "tsdown.config.ts"
ref: HEAD
---

import { defineConfig } from 'tsdown';

// Reference: https://tsdown.dev/reference/config-options
export default defineConfig({
  entry: ['src/index.ts'],
  dts: true,
  hash: false,
  copy: ['README.md', 'LICENSE', 'package.json'],
});
