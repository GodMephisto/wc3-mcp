---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "package.json"
ref: HEAD
---

{
  "name": "wc3-mcp",
  "version": "1.1.0",
  "description": "Model Context Protocol server that lets Claude read and edit Warcraft III (.w3x/.w3m) maps",
  "type": "module",
  "license": "MIT",
  "author": "YElwiddi",
  "homepage": "https://github.com/YElwiddi/warcraft-3-mcp-server#readme",
  "repository": {
    "type": "git",
    "url": "https://github.com/YElwiddi/warcraft-3-mcp-server.git"
  },
  "keywords": [
    "mcp",
    "model-context-protocol",
    "claude",
    "warcraft-3",
    "warcraft",
    "w3x",
    "map-editor",
    "world-editor"
  ],
  "bin": {
    "wc3-mcp": "./dist/index.js"
  },
  "files": [
    "dist"
  ],
  "scripts": {
    "build": "tsc",
    "gen:triggerdata": "node scripts/gen_triggerdata.mjs",
    "start": "node dist/index.js",
    "dev": "tsx watch src/index.ts",
    "inspect": "npx @modelcontextprotocol/inspector node dist/index.js"
  },
  "engines": {
    "node": ">=18"
  },
  "dependencies": {
    "@modelcontextprotocol/sdk": "^1.29.0",
    "mdx-m3-viewer": "^5.12.0",
    "wc3maptranslator": "^5.0.0",
    "zod": "^4.4.3"
  },
  "devDependencies": {
    "@types/node": "^22.10.0",
    "tsx": "^4.19.2",
    "typescript": "^5.7.0"
  }
}
