---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/app/not-found.tsx"
ref: HEAD
---

import type { Metadata } from "next";
import Link from "next/link";

import { Button } from "@/components/ui/warcraftcn/button";

export const metadata: Metadata = {
  title: "404",
};

export default function NotFound() {
  return (
    <main className="flex flex-1 flex-col items-center justify-center gap-6 px-4 py-24 text-center">
      <h1 className="fantasy font-bold text-6xl text-amber-400">404</h1>
      <p className="max-w-md text-muted-foreground">
        This path leads off the edge of the map. The scout found nothing here.
      </p>
      <Link href="/">
        <Button>Return to base</Button>
      </Link>
    </main>
  );
}
