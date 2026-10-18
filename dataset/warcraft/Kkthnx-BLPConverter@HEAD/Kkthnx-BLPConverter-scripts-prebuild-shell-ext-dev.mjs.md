---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "scripts/prebuild-shell-ext-dev.mjs"
ref: HEAD
---

import { execSync } from "node:child_process";
import { mkdirSync, readFileSync, writeFileSync } from "node:fs";
import { dirname, join } from "node:path";

function bundleShellDll(profile) {
  const src = join("target", profile, "blp_shell_ext.dll");
  const dst = join("src-tauri", "resources", "blpview_thumb.dll");
  mkdirSync(dirname(dst), { recursive: true });

  let lastError;
  for (let attempt = 0; attempt < 12; attempt++) {
    try {
      writeFileSync(dst, readFileSync(src));
      console.log(`Bundled BLPView DLL to ${dst}`);
      return;
    } catch (error) {
      lastError = error;
      const code = error?.code;
      if (code !== "EBUSY" && code !== "EPERM" && code !== "EACCES") {
        throw error;
      }
      Atomics.wait(
        new Int32Array(new SharedArrayBuffer(4)),
        0,
        0,
        150 * (attempt + 1),
      );
    }
  }

  throw lastError;
}

if (process.platform === "win32") {
  console.log("Building BLPView shell extension (debug)…");
  execSync("cargo build -p blp-shell-ext", { stdio: "inherit" });
  bundleShellDll("debug");
} else {
  console.log("Skipping BLPView shell extension (Windows only).");
}
