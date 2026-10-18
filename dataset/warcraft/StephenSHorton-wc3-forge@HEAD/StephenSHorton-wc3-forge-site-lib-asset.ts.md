---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/lib/asset.ts"
ref: HEAD
---

// GitHub Pages serves this project site under /wc3-forge. next/image does NOT
// prefix the configured basePath onto `public/` asset src in a static export,
// so static asset URLs must be prefixed manually. NEXT_PUBLIC_BASE_PATH is
// inlined at build time, so this works in both server and client components.
const basePath = process.env.NEXT_PUBLIC_BASE_PATH ?? "";

export function asset(path: string): string {
  return `${basePath}${path}`;
}
