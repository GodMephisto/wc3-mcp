---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/components/ui/warcraftcn/accordion.tsx"
ref: HEAD
---

"use client";

import * as React from "react";
import { Accordion as AccordionPrimitive } from "radix-ui";
import { cva, type VariantProps } from "class-variance-authority";

import { cn } from "@/lib/utils";

import "@/components/ui/warcraftcn/styles/warcraft.css";

const accordionRootVariants = cva("flex w-full flex-col", {
  variants: {
    variant: {
      default:
        "fantasy rounded-xl relative flex gap-2 py-[0.6rem] px-[0.7rem] text-[hsl(0_0%_95%)] bg-transparent shadow-none before:content-none after:content-none",
    },
  },
  defaultVariants: {
    variant: "default",
  },
});

type AccordionVariantProps = VariantProps<typeof accordionRootVariants>;

const AccordionStyleContext = React.createContext<{
  variant: NonNullable<AccordionVariantProps["variant"]>;
}>({
  variant: "default",
});

function Accordion({
  className,
  variant,
  ...props
}: React.ComponentProps<typeof AccordionPrimitive.Root> &
  AccordionVariantProps) {
  const resolvedVariant = variant ?? "default";
  return (
    <AccordionStyleContext.Provider value={{ variant: resolvedVariant }}>
      <AccordionPrimitive.Root
        data-slot="accordion"
        className={cn(accordionRootVariants({ variant }), className)}
        {...props}
      />
    </AccordionStyleContext.Provider>
  );
}

function AccordionItem({
  className,
  ...props
}: React.ComponentProps<typeof AccordionPrimitive.Item>) {
  return (
    <AccordionPrimitive.Item
      data-slot="accordion-item"
      className={cn(className)}
      {...props}
    />
  );
}

function AccordionTrigger({
  className,
  children,
  icon = "sword",
  ...props
}: React.ComponentProps<typeof AccordionPrimitive.Trigger> & {
  icon?: "sword" | "shield" | "rune-stone";
}) {
  return (
    <AccordionPrimitive.Header
      className={cn(
        "flex",

        "border-solid wc-accordion-header-border [border-image-repeat:stretch]",
        "border-6 [border-image-slice:16_fill]",
        "bg-cover bg-center bg-no-repeat",
        "m-0 mt-0 mb-0 min-h-14",
      )}
    >
      <AccordionPrimitive.Trigger
        data-slot="accordion-trigger"
        className={cn(
          "group/accordion-trigger relative flex w-full items-center overflow-hidden px-5 py-3 text-center text-sm text-white outline-none transition-all duration-300",
          "focus-visible:ring-ring/50 focus-visible:ring-2",
          "disabled:pointer-events-none disabled:opacity-60",
          "transition-[box-shadow] duration-[420ms] ease-[cubic-bezier(0.22,1,0.36,1)]",
          "data-[state=open]:shadow-[0_0_0_1px_rgba(255,215,120,0.4),0_0_12px_rgba(255,200,100,0.35),0_0_40px_rgba(255,180,80,0.25),0_20px_40px_rgba(0,0,0,0.45)]",
          "after:content-[''] after:absolute after:inset-0 after:bg-[linear-gradient(120deg,transparent_30%,rgba(255,220,140,0.25),transparent_70%)] after:opacity-0 after:pointer-events-none",
          "data-[state=open]:after:animate-[wc-light-sweep_600ms_ease-out]",
          className,
        )}
        {...props}
      >
        {children}
        <span
          data-slot="accordion-trigger-icon"
          aria-hidden="true"
          className={cn(
            "inline-block bg-center bg-no-repeat bg-contain pointer-events-none absolute right-4 size-4 shrink-0 transition-transform duration-300 group-data-[state=open]/accordion-trigger:rotate-180",
            icon === "shield"
              ? "wc-accordion-medieval-icon--shield"
              : icon === "rune-stone"
                ? "wc-accordion-medieval-icon--rune-stone"
                : "wc-accordion-medieval-icon--sword",
          )}
        />
      </AccordionPrimitive.Trigger>
    </AccordionPrimitive.Header>
  );
}

function AccordionContent({
  className,
  children,
  ...props
}: React.ComponentProps<typeof AccordionPrimitive.Content>) {
  return (
    <AccordionPrimitive.Content
      data-slot="accordion-content"
      className={cn(
        "overflow-hidden text-sm will-change-[height,opacity,filter]",
        "transition-[filter] duration-300 ease-out",

        "wc-accordion-scroll-content data-[state=open]:animate-[wc-accordion-down_200ms_ease-out] data-[state=closed]:animate-[wc-accordion-up_200ms_ease-out]",

        "border-solid wc-accordion-content-border [border-image-repeat:stretch]",
        "border-0 [border-image-slice:16_fill]",
        "bg-cover bg-center bg-no-repeat",
      )}
      {...props}
    >
      <div
        className={cn(
          "[&_a]:hover:text-foreground [&_a]:underline [&_a]:underline-offset-3 [&_p:not(:last-child)]:mb-4",

          "px-5 pt-2 pb-4",

          className,
        )}
      >
        <div className="relative my-6">
          <div className="h-px bg-gradient-to-r from-transparent via-[#9a6f2f] to-transparent" />
          <div className="absolute inset-0 flex justify-center -top-4">
            <span className="px-3 text-[#b98a3a] drop-shadow-[0_0_4px_rgba(120,78,24,0.45)]">
              ✦
            </span>
          </div>
        </div>

        {children}
      </div>
    </AccordionPrimitive.Content>
  );
}

export { Accordion, AccordionItem, AccordionTrigger, AccordionContent };
