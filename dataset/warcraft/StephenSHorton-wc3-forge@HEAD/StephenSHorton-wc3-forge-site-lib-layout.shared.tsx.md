---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/lib/layout.shared.tsx"
ref: HEAD
---

import type { BaseLayoutProps } from "fumadocs-ui/layouts/shared";
import Image from "next/image";

import { asset } from "@/lib/asset";

/**
 * Shared chrome options for both the home layout and the docs layout.
 * See https://fumadocs.dev/docs/ui/layouts/shared
 */
export function baseOptions(): BaseLayoutProps {
  return {
    nav: {
      title: (
        <>
          <Image alt="wc3-forge" height={24} src={asset("/logo.png")} width={24} />
          <span className="fantasy font-bold">wc3-forge</span>
        </>
      ),
      transparentMode: "top",
    },
    links: [
      {
        text: "Docs",
        url: "/docs",
      },
      {
        text: "Download",
        url: "/#download",
      },
    ],
    githubUrl: "https://github.com/StephenSHorton/wc3-forge",
  };
}
