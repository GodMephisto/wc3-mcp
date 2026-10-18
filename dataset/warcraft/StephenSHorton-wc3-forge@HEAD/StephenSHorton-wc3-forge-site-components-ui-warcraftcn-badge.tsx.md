---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/components/ui/warcraftcn/badge.tsx"
ref: HEAD
---

import { Slot } from "@radix-ui/react-slot";
import { cva, type VariantProps } from "class-variance-authority";
import type * as React from "react";

import { cn } from "@/lib/utils";

import "@/components/ui/warcraftcn/styles/warcraft.css";

const badgeVariants = cva(
  "fantasy inline-flex items-center gap-1.5 transition-all duration-100 focus:outline-none focus:ring-2 focus:ring-amber-500/50 [border-image-repeat:stretch] border-solid",
  {
    variants: {
      variant: {
        default:
          "text-amber-100 [text-shadow:0_0_8px_rgba(251,191,36,0.4)] wc-btn-border-frame [border-image-slice:16_fill]",
        secondary:
          "text-slate-200 wc-btn-border [border-image-slice:16_fill]",
        destructive:
          "text-red-200 [text-shadow:0_0_8px_rgba(239,68,68,0.4)] wc-btn-border-frame brightness-75 hue-rotate-[320deg] [border-image-slice:16_fill]",
        outline:
          "border-amber-900/50 bg-black/40 text-amber-200/80 hover:bg-black/60",
      },
      size: {
        default: "px-3 py-1 text-xs border-[3px]",
        sm: "px-2 py-0.5 text-[10px] border-[2px]",
        lg: "px-4 py-1.5 text-sm border-[4px]",
      },
      faction: {
        none: "",
        alliance:
          "text-blue-100 [text-shadow:0_0_10px_rgba(59,130,246,0.5)] border-blue-900/50 bg-blue-950/40",
        horde:
          "text-red-100 [text-shadow:0_0_10px_rgba(239,68,68,0.5)] border-red-900/50 bg-red-950/40",
      },
      shape: {
        default: "rounded-md",
        shield: "rounded-t-sm rounded-b-xl border-b-[5px]",
        banner: "rounded-none clip-path-banner",
      },
    },
    defaultVariants: {
      variant: "default",
      size: "default",
      faction: "none",
      shape: "default",
    },
  }
);

export interface BadgeProps
  extends React.HTMLAttributes<HTMLDivElement>,
    VariantProps<typeof badgeVariants> {
  asChild?: boolean;
}

function Badge({
  className,
  variant,
  size,
  faction,
  shape,
  asChild = false,
  children,
  ...props
}: BadgeProps) {
  const Comp = asChild ? Slot : "div";

  return (
    <Comp
      className={cn(
        badgeVariants({ variant, size, faction, shape }),
        className
      )}
      {...props}
    >
      {children}
    </Comp>
  );
}

Badge.displayName = "Badge";

export { Badge, badgeVariants };
