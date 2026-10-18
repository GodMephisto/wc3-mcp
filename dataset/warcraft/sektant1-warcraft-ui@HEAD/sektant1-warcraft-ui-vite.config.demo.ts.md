---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "vite.config.demo.ts"
ref: HEAD
---

import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import { resolve } from "node:path";

export default defineConfig({
  plugins: [react()],
  base: "/warcraft-ui/",
  resolve: {
    alias: {
      "@sektant1/warcraft-ui/style.css": resolve(__dirname, "src/index.css"),
      "@sektant1/warcraft-ui": resolve(__dirname, "lib/main.ts"),
    },
  },
  build: {
    outDir: "demo/dist",
    emptyOutDir: true,
  },
});
