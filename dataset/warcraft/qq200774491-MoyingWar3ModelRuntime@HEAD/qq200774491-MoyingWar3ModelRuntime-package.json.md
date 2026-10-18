---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "package.json"
ref: HEAD
---

{
  "name": "@moying/war3-model-runtime",
  "version": "0.1.0",
  "description": "A high-performance Warcraft III MDX/MDL parsing and rendering library powered by Babylon.js.",
  "license": "MIT",
  "type": "module",
  "files": [
    "dist",
    "examples",
    "LICENSE",
    "README.md",
    "THIRD_PARTY_NOTICES.md"
  ],
  "exports": {
    ".": {
      "types": "./dist/index.d.ts",
      "import": "./dist/index.js"
    }
  },
  "types": "./dist/index.d.ts",
  "sideEffects": false,
  "scripts": {
    "clean": "node -e \"require('node:fs').rmSync('dist',{recursive:true,force:true})\"",
    "typecheck": "tsc --noEmit -p tsconfig.json",
    "viewer": "vite --config vite.viewer.config.ts",
    "smoke:models": "tsx test/smoke-real-models.ts",
    "build": "npm run clean && tsc -p tsconfig.build.json && vite build",
    "test": "npm run build && tsx --test test/*.test.ts",
    "smoke:pack": "npm run build && node scripts/smoke-pack.mjs",
    "pack:check": "npm pack --dry-run",
    "prepack": "npm run build"
  },
  "peerDependencies": {
    "@babylonjs/core": ">=7.54.3 <8"
  },
  "devDependencies": {
    "@babylonjs/core": "7.54.3",
    "@types/node": "^24.0.0",
    "@webgpu/types": "^0.1.67",
    "tsx": "^4.20.6",
    "typescript": "^5.9.3",
    "vite": "^5.4.21"
  },
  "engines": {
    "node": ">=20"
  },
  "repository": {
    "type": "git",
    "url": "git+ssh://git@github.com/qq200774491/MoyingWar3ModelRuntime.git"
  }
}
