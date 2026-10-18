---
type: vendor-doc
source_repo: "https://github.com/Kingwl/w3x-google-translate"
source_path: "scripts/build.ts"
ref: HEAD
---

import { build } from "esbuild";

build({
  entryPoints: ["src/index.ts"],
  outdir: "dist",
  platform: "node",
  format: "esm",
  bundle: true,
  sourcemap: true,
  external: ["axios"],
});
