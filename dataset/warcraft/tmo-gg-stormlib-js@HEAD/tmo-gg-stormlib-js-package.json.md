---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "package.json"
ref: HEAD
---

{
  "name": "stormlib-js",
  "version": "0.1.1",
  "description": "Pure TypeScript implementation of StormLib for reading MPQ archives",
  "author": "changmyeong",
  "license": "MIT",
  "repository": {
    "type": "git",
    "url": "https://github.com/tmo-gg/stormlib-js.git"
  },
  "homepage": "https://github.com/tmo-gg/stormlib-js",
  "bugs": {
    "url": "https://github.com/tmo-gg/stormlib-js/issues"
  },
  "main": "dist/index.js",
  "module": "dist/index.mjs",
  "types": "dist/index.d.ts",
  "exports": {
    ".": {
      "types": "./dist/index.d.ts",
      "browser": "./dist/index.browser.mjs",
      "import": "./dist/index.mjs",
      "require": "./dist/index.js"
    },
    "./browser": {
      "types": "./dist/index.browser.d.ts",
      "import": "./dist/index.browser.mjs",
      "require": "./dist/index.browser.js"
    }
  },
  "files": [
    "dist"
  ],
  "scripts": {
    "build": "tsup",
    "prepublishOnly": "npm run build",
    "test": "vitest run",
    "test:watch": "vitest",
    "typecheck": "tsc --noEmit"
  },
  "keywords": [
    "mpq",
    "stormlib",
    "blizzard",
    "archive",
    "diablo",
    "warcraft",
    "starcraft"
  ],
  "dependencies": {
    "buffer": "^6.0.3",
    "pako": "^2.1.0"
  },
  "devDependencies": {
    "@types/node": "^22.0.0",
    "@types/pako": "^2.0.3",
    "tsup": "^8.0.0",
    "typescript": "^5.5.0",
    "vitest": "^2.0.0"
  }
}
