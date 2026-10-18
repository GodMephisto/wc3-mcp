---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "vitest.config.ts"
ref: HEAD
---

import { defineConfig } from "vitest/config";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  test: {
    environment: "jsdom",
    globals: true,
    setupFiles: ["./src/__tests__/setup.ts"],
  },
});
