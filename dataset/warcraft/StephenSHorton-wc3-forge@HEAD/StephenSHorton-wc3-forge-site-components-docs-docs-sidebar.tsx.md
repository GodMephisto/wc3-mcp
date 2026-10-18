---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/components/docs/docs-sidebar.tsx"
ref: HEAD
---

"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useMemo, useState } from "react";

import { Badge } from "@/components/ui/warcraftcn/badge";
import { Input } from "@/components/ui/warcraftcn/input";
import type { DocLink } from "@/lib/docs-tree";

export function DocsSidebar({ links }: { links: DocLink[] }) {
  const pathname = usePathname();
  const [query, setQuery] = useState("");

  const filtered = useMemo(() => {
    const q = query.trim().toLowerCase();
    if (!q) {
      return links;
    }
    return links.filter((l) => l.title.toLowerCase().includes(q));
  }, [links, query]);

  return (
    <aside className="hidden w-64 shrink-0 lg:block">
      <div className="sticky top-20 flex flex-col gap-4 py-10">
        <div className="flex items-center justify-between">
          <span className="fantasy font-semibold text-amber-400/80 text-xs uppercase tracking-widest">
            The Archives
          </span>
          <Badge size="sm" variant="secondary">
            alpha
          </Badge>
        </div>

        <Input
          aria-label="Filter docs"
          className="w-full text-sm"
          onChange={(e) => setQuery(e.target.value)}
          placeholder="Search the archives…"
          value={query}
        />

        <nav className="flex flex-col gap-0.5">
          {filtered.map((l) => {
            const active = pathname === l.url;
            return (
              <Link
                key={l.url}
                href={l.url}
                className={`fantasy border-l-2 px-3 py-1.5 text-sm transition-all ${
                  active
                    ? "border-amber-400 bg-amber-950/30 text-amber-200 [text-shadow:0_0_8px_rgba(251,191,36,0.35)]"
                    : "border-transparent text-stone-400 hover:border-amber-700/50 hover:text-amber-100"
                }`}
              >
                {l.title}
              </Link>
            );
          })}
          {filtered.length === 0 && (
            <p className="px-3 py-2 text-muted-foreground text-sm italic">
              No scrolls match “{query}”.
            </p>
          )}
        </nav>
      </div>
    </aside>
  );
}
