---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/components/docs/docs-pager.tsx"
ref: HEAD
---

import {
  Pagination,
  PaginationContent,
  PaginationItem,
  PaginationNext,
  PaginationPrevious,
} from "@/components/ui/warcraftcn/pagination";
import type { DocLink } from "@/lib/docs-tree";

export function DocsPager({
  prev,
  next,
}: {
  prev: DocLink | null;
  next: DocLink | null;
}) {
  if (!(prev || next)) {
    return null;
  }

  return (
    <Pagination className="mt-12 border-amber-900/30 border-t pt-6">
      <PaginationContent className="w-full justify-between gap-4">
        <PaginationItem>
          {prev ? (
            <PaginationPrevious href={prev.url} title={prev.title}>
              {/* label provided by component */}
            </PaginationPrevious>
          ) : (
            <span />
          )}
        </PaginationItem>

        <div className="hidden flex-col items-end justify-center text-right sm:flex">
          {next && (
            <>
              <span className="fantasy text-amber-500/70 text-xs uppercase tracking-widest">
                Next
              </span>
              <span className="fantasy text-amber-200 text-sm">
                {next.title}
              </span>
            </>
          )}
        </div>

        <PaginationItem>
          {next ? (
            <PaginationNext href={next.url} />
          ) : (
            <span />
          )}
        </PaginationItem>
      </PaginationContent>
    </Pagination>
  );
}
