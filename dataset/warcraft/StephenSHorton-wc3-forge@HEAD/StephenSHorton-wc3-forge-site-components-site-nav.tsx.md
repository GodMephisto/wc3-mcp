---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/components/site-nav.tsx"
ref: HEAD
---

"use client";

import { Github } from "lucide-react";
import Image from "next/image";
import Link from "next/link";
import { usePathname } from "next/navigation";

import { CursorPicker } from "@/components/cursor-picker";
import { Button } from "@/components/ui/warcraftcn/button";
import { asset } from "@/lib/asset";

const LINKS = [
  { href: "/docs", label: "Docs" },
  { href: "/docs/tool-reference", label: "MCP Tools" },
  { href: "/docs/components", label: "Showcase" },
];

export function SiteNav() {
  const pathname = usePathname();

  return (
    <header className="sticky top-0 z-50 border-amber-900/40 border-b bg-background/80 backdrop-blur-md">
      <div className="mx-auto flex h-16 w-full max-w-7xl items-center gap-4 px-4">
        <Link className="flex items-center gap-2" href="/">
          <Image alt="wc3-forge" height={28} src={asset("/logo.png")} width={28} />
          <span className="fantasy font-bold text-amber-200 text-lg tracking-wide">
            wc3-forge
          </span>
        </Link>

        <nav className="ml-4 hidden items-center gap-1 md:flex">
          {LINKS.map((l) => {
            const active =
              l.href === "/docs"
                ? pathname === "/docs"
                : pathname.startsWith(l.href);
            return (
              <Link
                key={l.href}
                href={l.href}
                className={`fantasy px-3 py-1.5 text-sm transition-colors ${
                  active
                    ? "text-amber-300 [text-shadow:0_0_8px_rgba(251,191,36,0.4)]"
                    : "text-stone-300 hover:text-amber-200"
                }`}
              >
                {l.label}
              </Link>
            );
          })}
        </nav>

        <div className="ml-auto flex items-center gap-2">
          <CursorPicker />
          <Button asChild className="h-9 px-3" variant="frame">
            <a
              aria-label="GitHub repository"
              href="https://github.com/StephenSHorton/wc3-forge"
              rel="noopener noreferrer"
              target="_blank"
            >
              <Github className="size-4" />
              <span className="hidden lg:inline">GitHub</span>
            </a>
          </Button>
          <Button asChild className="hidden h-9 px-4 sm:inline-flex">
            <Link href="/#download">Download</Link>
          </Button>
        </div>
      </div>
    </header>
  );
}
