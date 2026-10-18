---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/components/docs/docs-toc.tsx"
ref: HEAD
---

"use client";

import { useEffect, useState } from "react";

export interface TocItem {
  title: React.ReactNode;
  url: string;
  depth: number;
}

export function DocsToc({ items }: { items: TocItem[] }) {
  const [activeId, setActiveId] = useState<string>("");

  useEffect(() => {
    if (items.length === 0) {
      return;
    }
    const headings = items
      .map((i) => document.getElementById(i.url.replace("#", "")))
      .filter((el): el is HTMLElement => el !== null);

    const observer = new IntersectionObserver(
      (entries) => {
        for (const entry of entries) {
          if (entry.isIntersecting) {
            setActiveId(entry.target.id);
          }
        }
      },
      { rootMargin: "0px 0px -70% 0px", threshold: 1 }
    );

    for (const h of headings) {
      observer.observe(h);
    }
    return () => observer.disconnect();
  }, [items]);

  if (items.length === 0) {
    return null;
  }

  return (
    <aside className="hidden w-56 shrink-0 xl:block">
      <div className="sticky top-20 py-10">
        <p className="fantasy mb-3 font-semibold text-amber-400/80 text-xs uppercase tracking-widest">
          On this page
        </p>
        <ul className="flex flex-col gap-1.5 border-amber-900/30 border-l">
          {items.map((item) => {
            const id = item.url.replace("#", "");
            const active = id === activeId;
            return (
              <li key={item.url}>
                <a
                  href={item.url}
                  className={`-ml-px block border-l-2 text-sm transition-colors ${
                    active
                      ? "border-amber-400 text-amber-200"
                      : "border-transparent text-stone-400 hover:text-amber-100"
                  }`}
                  style={{ paddingLeft: `${(item.depth - 1) * 12 + 12}px` }}
                >
                  {item.title}
                </a>
              </li>
            );
          })}
        </ul>
      </div>
    </aside>
  );
}
