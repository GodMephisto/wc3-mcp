---
type: vendor-doc
source_repo: "https://github.com/lichenhao/war3map_parser"
source_path: "package.json"
ref: HEAD
---

{
  "name": "v5",
  "private": true,
  "version": "0.0.0",
  "type": "module",
  "scripts": {
    "dev": "vite",
    "build": "tsc && vite build",
    "preview": "vite preview"
  },
  "devDependencies": {
    "@types/node": "^25.0.3",
    "@types/pako": "^2.0.4",
    "typescript": "~5.9.3",
    "vite": "npm:rolldown-vite@7.2.5"
  },
  "pnpm": {
    "overrides": {
      "vite": "npm:rolldown-vite@7.2.5"
    }
  },
  "dependencies": {
    "pako": "^2.1.0"
  }
}
