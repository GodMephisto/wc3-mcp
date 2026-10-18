---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/app/docs/layout.tsx"
ref: HEAD
---

import { DocsSidebar } from "@/components/docs/docs-sidebar";
import { SiteNav } from "@/components/site-nav";
import { getDocLinks } from "@/lib/docs-tree";

export default function Layout({ children }: { children: React.ReactNode }) {
  const links = getDocLinks();

  return (
    <div className="flex min-h-screen flex-col">
      <SiteNav />
      <div className="mx-auto flex w-full max-w-7xl flex-1 gap-8 px-4">
        <DocsSidebar links={links} />
        <main className="min-w-0 flex-1">{children}</main>
      </div>
    </div>
  );
}
