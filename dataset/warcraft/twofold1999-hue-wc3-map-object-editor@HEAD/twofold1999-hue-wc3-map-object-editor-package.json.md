---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "package.json"
ref: HEAD
---

{
  "name": "wc3-map-object-editor",
  "version": "1.0.0",
  "description": "TypeScript CLI for editing Warcraft III map objects",
  "main": "dist/index.js",
  "scripts": {
    "dev": "tsx watch src/index.ts",
    "dev:once": "tsx src/index.ts",
    "build": "tsc -p tsconfig.json",
    "start": "node dist/index.js"
  },
  "keywords": [],
  "author": "",
  "license": "ISC",
  "type": "commonjs",
  "dependencies": {
    "commander": "^14.0.3"
  },
  "devDependencies": {
    "@types/node": "^25.6.0",
    "tsx": "^4.21.0",
    "typescript": "^6.0.3"
  }
}
