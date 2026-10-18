---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/components/ui/warcraftcn/checkbox.tsx"
ref: HEAD
---

"use client";

import * as React from "react";
import { Checkbox as CheckboxPrimitive } from "radix-ui";
import { cva, type VariantProps } from "class-variance-authority";

import { cn } from "@/lib/utils";

import "@/components/ui/warcraftcn/styles/warcraft.css";

const checkboxVariants = cva(
  "inline-flex items-center gap-3 cursor-pointer select-none fantasy mb-2 font-bold",
  {
    variants: {
      faction: {
        default: "text-yellow-800 dark:text-yellow-100",
        orc: "text-red-700 dark:text-red-100",
        elf: "text-green-700 dark:text-green-100",
        human: "text-blue-700 dark:text-blue-100",
        undead: "text-purple-700 dark:text-purple-100",
      },
    },
    defaultVariants: {
      faction: "default",
    },
  }
);

type Faction = "default" | "orc" | "elf" | "human" | "undead";

interface CheckboxProps
  extends Omit<
      React.ComponentProps<typeof CheckboxPrimitive.Root>,
      "children" | "asChild"
    >,
    VariantProps<typeof checkboxVariants> {
  faction?: Faction;
  children?: React.ReactNode;
}

function Checkbox({
  faction = "default",
  children,
  className,
  disabled,
  id,
  ...props
}: CheckboxProps) {
  return (
    <label
      htmlFor={id}
      className={cn(
        checkboxVariants({ faction }),
        disabled && "opacity-50 cursor-not-allowed"
      )}
    >
      <CheckboxPrimitive.Root
        data-slot="checkbox"
        className={cn("wc-checkbox", `wc-checkbox-${faction}`, className)}
        disabled={disabled}
        id={id}
        {...props}
      >
        <CheckboxPrimitive.Indicator />
      </CheckboxPrimitive.Root>
      {children}
    </label>
  );
}

export { Checkbox, checkboxVariants };
export type { CheckboxProps, Faction };