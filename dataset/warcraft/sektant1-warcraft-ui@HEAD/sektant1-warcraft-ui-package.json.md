---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "package.json"
ref: HEAD
---

{
  "name": "@sektant1/warcraft-ui",
  "version": "1.0.0",
  "type": "module",
  "main": "./dist/warcraft-ui.umd.js",
  "module": "./dist/warcraft-ui.es.js",
  "types": "./dist/main.d.ts",
  "style": "./dist/assets/warcraft-ui.css",
  "directories": {
    "lib": "lib"
  },
  "files": [
    "dist"
  ],
  "exports": {
    ".": {
      "types": "./dist/main.d.ts",
      "import": "./dist/warcraft-ui.es.js",
      "require": "./dist/warcraft-ui.umd.js"
    },
    "./style.css": "./dist/assets/warcraft-ui.css"
  },
  "scripts": {
    "dev": "vite",
    "build": "tsc && vite build",
    "build:demo": "vite build --config vite.config.demo.ts",
    "preview": "vite preview --config vite.config.demo.ts",
    "test": "vitest run",
    "test:watch": "vitest"
  },
  "publishConfig": {
    "registry": "https://npm.pkg.github.com"
  },
  "repository": {
    "type": "git",
    "url": "git+https://github.com/sektant1/warcraft-ui.git"
  },
  "dependencies": {
    "gl-matrix": "^3.4.3",
    "war3-model": "^4.0.1"
  },
  "peerDependencies": {
    "react": "^18.0.0 || ^19.0.0",
    "react-dom": "^18.0.0 || ^19.0.0"
  },
  "devDependencies": {
    "@eslint/js": "^9.39.4",
    "@testing-library/jest-dom": "^6.9.1",
    "@testing-library/react": "^16.3.2",
    "@types/node": "^24.12.2",
    "@types/react": "^19.2.14",
    "@types/react-dom": "^19.2.3",
    "@vitejs/plugin-react": "^6.0.1",
    "eslint": "^9.39.4",
    "eslint-plugin-react-hooks": "^7.0.1",
    "eslint-plugin-react-refresh": "^0.5.2",
    "globals": "^17.4.0",
    "jsdom": "^29.0.1",
    "typescript": "~5.9.3",
    "typescript-eslint": "^8.57.0",
    "vite": "^8.0.1",
    "vite-plugin-dts": "^4.5.4",
    "vitest": "^4.1.2"
  }
}
