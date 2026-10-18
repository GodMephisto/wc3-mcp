---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/vite.config.ts"
ref: HEAD
---

import {defineConfig} from 'vite'
import {svelte} from '@sveltejs/vite-plugin-svelte'
import tailwindcss from '@tailwindcss/vite'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [tailwindcss(), svelte()],
  resolve: {
    alias: {
      // mdx-m3-viewer's ModelViewer extends Node's EventEmitter. Without a
      // polyfill Vite leaves `require('events')` as a bare specifier and
      // the bundled code crashes with "Class extends value undefined".
      // The `events` npm package is the standard browser-side polyfill.
      events: 'events',
      $lib: '/src/lib',
      $components: '/src/lib/components',
    },
  },
  optimizeDeps: {
    include: ['mdx-m3-viewer', 'events'],
  },
  build: {
    commonjsOptions: {
      transformMixedEsModules: true,
    },
  },
})
