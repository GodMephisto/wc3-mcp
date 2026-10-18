---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/components/ui/warcraftcn/input.tsx"
ref: HEAD
---

import type * as React from "react";

import { cn } from "@/lib/utils";

import "@/components/ui/warcraftcn/styles/warcraft.css";

function Input({ className, type, ...props }: React.ComponentProps<"input">) {
  return (
    <input
      className={cn(
        "fantasy min-w-0 bg-center bg-cover bg-no-repeat p-3 text-base shadow-xs outline-none transition-[color,box-shadow] selection:text-primary-foreground file:inline-flex file:h-7 file:border-0 file:bg-transparent file:font-medium file:text-foreground file:text-sm placeholder:text-muted-foreground disabled:pointer-events-none disabled:cursor-not-allowed disabled:opacity-50 md:text-sm",
        "border-solid wc-input-border [border-image-repeat:stretch]",
        "border-6 [border-image-slice:16_fill]",
        className
      )}
      data-slot="input"
      type={type}
      {...props}
    />
  );
}

export { Input };
