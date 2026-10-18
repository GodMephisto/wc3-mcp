---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "vite.config.ts"
ref: HEAD
---

import { defineConfig } from 'vite';

export default defineConfig({
  build: {
    emptyOutDir: false,
    lib: {
      entry: 'src/index.ts',
      formats: ['es'],
      fileName: () => 'index.js',
    },
    sourcemap: true,
    minify: 'esbuild',
    rollupOptions: {
      external: (id) => id === '@babylonjs/core' || id.startsWith('@babylonjs/core/'),
    },
  },
});
