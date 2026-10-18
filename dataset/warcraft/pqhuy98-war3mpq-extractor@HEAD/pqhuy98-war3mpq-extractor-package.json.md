---
type: vendor-doc
source_repo: "https://github.com/pqhuy98/war3mpq-extractor"
source_path: "package.json"
ref: HEAD
---

{
  "name": "war3mpq-extractor",
  "version": "1.0.0",
  "description": "",
  "main": "index.js",
  "type": "module",
  "scripts": {
    "model": "node extract-models.js",
    "extension": "node list-extensions.js",
    "hero": "node extract-heroes.js",
    "unit": "node extract-units.js",
    "unit-category": "node extract-unit-category.js",
    "ability": "node extract-abilities.js",
    "lightning": "node extract-lightnings.js",
    "unit-sound": "node extract-unit-sound.js",
    "generate-ts": "node generate-ts.js",
    "bundle-voice": "node src/bundle-voice.js",
    "item": "node generate-items.js",
    "generate-dialogues": "tsx src/generate-dialogues.ts",
    "compare-v4": "tsx src/compare-v4.ts"
  },
  "author": "",
  "license": "ISC",
  "dependencies": {
    "csv-parser": "^3.0.0",
    "dotenv": "^16.4.5",
    "fluent-ffmpeg": "^2.1.3",
    "glob": "^8.0.3",
    "ts-node": "^10.9.2"
  },
  "devDependencies": {
    "elevenlabs": "^0.16.1",
    "eslint": "^9.3.0",
    "tsx": "^4.19.1"
  }
}
