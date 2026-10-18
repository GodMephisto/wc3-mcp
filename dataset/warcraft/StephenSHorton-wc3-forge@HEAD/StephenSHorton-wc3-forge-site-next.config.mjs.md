---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/next.config.mjs"
ref: HEAD
---

import path from "node:path";
import { fileURLToPath } from "node:url";
import { createMDX } from "fumadocs-mdx/next";

const withMDX = createMDX();

const root = path.dirname(fileURLToPath(import.meta.url));

// GitHub Pages serves a project site under /<repo>. Set NEXT_PUBLIC_BASE_PATH
// to "/wc3-forge" in CI; leave empty for local dev or a custom domain.
const basePath = process.env.NEXT_PUBLIC_BASE_PATH ?? "";

/** @type {import('next').NextConfig} */
const config = {
  output: "export",
  basePath,
  // Emit each route as <route>/index.html so both /docs and /docs/ resolve on
  // GitHub Pages (without this, only the no-slash form works).
  trailingSlash: true,
  images: { unoptimized: true },
  reactStrictMode: true,
  turbopack: { root },
};

export default withMDX(config);
