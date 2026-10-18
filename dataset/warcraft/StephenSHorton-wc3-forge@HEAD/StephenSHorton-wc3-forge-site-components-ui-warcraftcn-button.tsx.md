---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/components/ui/warcraftcn/button.tsx"
ref: HEAD
---

import { Slot } from "@radix-ui/react-slot";
import { cva, type VariantProps } from "class-variance-authority";
import type * as React from "react";

import { cn } from "@/lib/utils";

import "@/components/ui/warcraftcn/styles/warcraft.css";

const buttonVariants = cva(
  "fantasy inline-flex shrink-0 items-center justify-center gap-2 whitespace-nowrap rounded-md font-medium text-sm outline-none transition-all duration-100 focus-visible:border-ring focus-visible:ring-[3px] focus-visible:ring-ring/50 disabled:pointer-events-none disabled:opacity-50 aria-invalid:border-destructive aria-invalid:ring-destructive/20 dark:aria-invalid:ring-destructive/40 [&_svg:not([class*='size-'])]:size-4 [&_svg]:pointer-events-none [&_svg]:shrink-0 active:scale-95 active:brightness-75 active:shadow-inner",
  {
    variants: {
      variant: {
        default:
          "bg-center px-5 py-4 bg-cover bg-no-repeat text-white hover:brightness-110",
        frame:
          "bg-center bg-cover bg-no-repeat text-white hover:brightness-110",
      },
    },
    defaultVariants: {
      variant: "default",
    },
  }
);

function Button({
  className,
  variant,
  asChild = false,
  style,
  ...props
}: React.ComponentProps<"button"> &
  VariantProps<typeof buttonVariants> & {
    asChild?: boolean;
  }) {
  const Comp = asChild ? Slot : "button";
  const borderImageClass =
    variant === "frame" ? "wc-btn-border-frame" : "wc-btn-border";

  return (
    <Comp
      className={cn(
        buttonVariants({ variant }),
        "border-solid [border-image-repeat:stretch] border-5 [border-image-slice:16_fill]",
        borderImageClass,
        className
      )}
      style={style}
      data-slot="button"
      {...props}
    />
  );
}

export { Button, buttonVariants };
