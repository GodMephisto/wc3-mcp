---
type: vendor-doc
source_repo: "https://github.com/Kingwl/w3x-google-translate"
source_path: "package.json"
ref: HEAD
---

{
  "name": "w3x-google-translate",
  "version": "0.0.1",
  "main": "./dist/index.js",
  "scripts": {
    "test": "dotenv tsx ./tests/main.ts",
    "build": "tsx ./scripts/build.ts",
    "prettier": "prettier -w \"./{scripts,src,tests}/**/*.ts\""
  },
  "author": {
    "name": "kingwl",
    "email": "kingwenlu@gmail.com"
  },
  "license": "MIT",
  "devDependencies": {
    "@types/ini": "^1.3.31",
    "@types/node": "^20.1.0",
    "dotenv": "^16.0.3",
    "dotenv-cli": "^7.2.1",
    "esbuild": "^0.17.18",
    "prettier": "^2.8.8",
    "tsx": "^3.12.7",
    "typescript": "^5.0.4",
    "ini": "^4.1.0",
    "axios": "^1.4.0"
  },
  "peerDependencies": {
    "axios": "^1.4.0"
  }
}
