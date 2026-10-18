---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "vite.config.ts"
ref: HEAD
---

import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

const host = process.env.TAURI_DEV_HOST;

export default defineConfig({
  plugins: [react()],
  clearScreen: false,
  server: {
    port: 1420,
    strictPort: true,
    host: host || false,
    hmr: host
      ? {
          protocol: "ws",
          host,
          port: 1421,
        }
      : undefined,
    watch: {
      // Cargo writes to workspace `target/` while Vite runs, ignore it on Windows
      // to avoid EBUSY watcher errors on locked .exe build artifacts.
      ignored: ["**/src-tauri/**", "**/target/**", "**/blp-shell-ext/**"],
    },
  },
  envPrefix: ["VITE_", "TAURI_"],
  build: {
    target: process.env.TAURI_ENV_PLATFORM === "windows" ? "chrome105" : "safari13",
    minify: !process.env.TAURI_ENV_DEBUG ? "esbuild" : false,
    sourcemap: !!process.env.TAURI_ENV_DEBUG,
  },
});
