---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/components/ui/skeleton.tsx"
ref: HEAD
---

import { cn } from "@/lib/utils";

function Skeleton({ className, ...props }: React.ComponentProps<"div">) {
  return (
    <div
      className={cn("animate-pulse rounded-md bg-accent", className)}
      data-slot="skeleton"
      {...props}
    />
  );
}

export { Skeleton };
