---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/components/ui/warcraftcn/radio-group.tsx"
ref: HEAD
---

"use client";

import * as React from "react";
import { RadioGroup as RadioGroupPrimitive } from "radix-ui";

import { cn } from "@/lib/utils";

import "@/components/ui/warcraftcn/styles/warcraft.css";

interface RadioGroupProps
  extends React.ComponentProps<typeof RadioGroupPrimitive.Root> {
  orientation?: "vertical" | "horizontal";
}

type RadioGroupItemProps = React.ComponentProps<
  typeof RadioGroupPrimitive.Item
>;

function RadioGroup({
  className,
  orientation = "vertical",
  ...props
}: RadioGroupProps) {
  return (
    <RadioGroupPrimitive.Root
      data-slot="radio-group"
      className={cn(
        "fantasy flex gap-3",
        orientation === "horizontal" ? "flex-row flex-wrap" : "flex-col",
        className
      )}
      orientation={orientation}
      {...props}
    />
  );
}

function RadioGroupItem({
  className,
  ...props
}: RadioGroupItemProps) {
  return (
    <RadioGroupPrimitive.Item
      data-slot="radio-group-item"
      className={cn("wc-radio-socket", className)}
      {...props}
    >
      <RadioGroupPrimitive.Indicator className="wc-radio-gem" />
    </RadioGroupPrimitive.Item>
  );
}

export { RadioGroup, RadioGroupItem };
export type { RadioGroupProps, RadioGroupItemProps };
