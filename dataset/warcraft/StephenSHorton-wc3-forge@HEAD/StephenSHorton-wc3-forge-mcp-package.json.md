---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "mcp/package.json"
ref: HEAD
---

{
  "name": "wc3-forge-mcp",
  "version": "0.0.1",
  "description": "MCP server that drives a running wc3-forge editor via its embedded JSON-RPC bridge.",
  "private": true,
  "type": "module",
  "bin": {
    "wc3-forge-mcp": "./dist/index.js"
  },
  "main": "dist/index.js",
  "scripts": {
    "build": "tsc",
    "dev": "tsx watch src/index.ts",
    "start": "node dist/index.js",
    "typecheck": "tsc --noEmit",
    "gen:tools": "npm run build && node scripts/gen-tools.mjs"
  },
  "dependencies": {
    "@modelcontextprotocol/sdk": "^1.0.4",
    "zod": "^3.23.8"
  },
  "devDependencies": {
    "@types/node": "^22.0.0",
    "tsx": "^4.19.0",
    "typescript": "^5.6.0",
    "zod-to-json-schema": "^3.23.5"
  },
  "engines": {
    "node": ">=20"
  }
}
