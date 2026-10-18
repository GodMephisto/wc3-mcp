---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/components/ui/warcraftcn/textarea.tsx"
ref: HEAD
---

import type * as React from "react";

import { cn } from "@/lib/utils";

import "@/components/ui/warcraftcn/styles/warcraft.css";

function Textarea({ className, ...props }: React.ComponentProps<"textarea">) {
  return (
    <textarea
      className={cn(
        "fantasy min-w-0 bg-center bg-cover bg-no-repeat p-3 px-5 text-base shadow-xs outline-none transition-[color,box-shadow] placeholder:text-muted-foreground disabled:pointer-events-none disabled:cursor-not-allowed disabled:opacity-50 md:text-sm",
        "border-solid wc-textarea-border [border-image-repeat:stretch]",
        "border-6 [border-image-slice:16_fill]",
        "min-h-24 w-full resize-none field-sizing-content",
        className
      )}
      data-slot="textarea"
      {...props}
    />
  );
}

export { Textarea };
