---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/components/ui/warcraftcn/tabs.tsx"
ref: HEAD
---

import * as RadixTabs from "@radix-ui/react-tabs";
import * as React from "react";
import { cva, type VariantProps } from "class-variance-authority";
import { cn } from "@/lib/utils";
import "@/components/ui/warcraftcn/styles/warcraft.css";

const tabVariants = cva(
  "wc-tabs-root w-full max-w-full flex",
  {
    variants: {
      faction: {
        default: "wc-tabs-default text-white",
        orc: "wc-tabs-orc",
        elf: "wc-tabs-elf",
        human: "wc-tabs-human",
        undead: "wc-tabs-undead",
      },
    },
    defaultVariants: { faction: "default" },
  }
);

// Responsive trigger style enhancements
const triggerVariants = cva(
  "wc-tab-trigger relative flex items-center justify-center font-semibold tracking-wide transition-transform duration-200 select-none whitespace-nowrap sm:max-w-[40px] lg:max-w-[50px]",
  {
    variants: {
      disabled: {
        false: "cursor-pointer",
        true: "cursor-not-allowed opacity-60 bg-gray-300 dark:bg-neutral-700 text-gray-500 dark:text-neutral-400",
      },
    },
    defaultVariants: { disabled: false },
  }
);

type FactionType = "default" | "orc" | "elf" | "human" | "undead";
type TabsProps = Omit<React.ComponentPropsWithoutRef<typeof RadixTabs.Root>, "orientation"> & VariantProps<typeof tabVariants> & { faction?: FactionType };

const Tabs = React.forwardRef<React.ComponentRef<typeof RadixTabs.Root>, TabsProps>(
  ({ className, faction = "default", ...props }, ref) => (
    <RadixTabs.Root
      ref={ref}
      className={cn(
        tabVariants({ faction }),
        className,
        "flex flex-col w-full max-w-full data-[orientation='vertical']:flex-row" // responsive: container is full width and stacks on mobile
      )}
      {...props}
    />
  )
);
Tabs.displayName = "Tabs";

// Responsive TabsList, keeps scrollability at all sizes and stacks on vertical
type TabsListProps = React.ComponentPropsWithoutRef<typeof RadixTabs.List>;

const TabsList = React.forwardRef<React.ComponentRef<typeof RadixTabs.List>, TabsListProps>(
  ({ className, ...props }, ref) => (
    <RadixTabs.List
      ref={ref}
      className={cn(
        "fantasy wc-tab-list relative no-scrollbar bg-transparent",
        // Responsive horizontal scroll for small screens
        "flex w-full gap-1 px-1 overflow-x-auto no-scrollbar sm:gap-2 sm:h-[40px] sm:px-0 mb-1",
        // Responsive: stack vertically for vertical orientation & allow vertical scroll if needed
        "data-[orientation='vertical']:flex-col data-[orientation='vertical']:w-auto data-[orientation='vertical']:h-auto",
        "data-[orientation='vertical']:min-w-[100px] data-[orientation='vertical']:max-w-[240px] data-[orientation='vertical']:overflow-y-auto data-[orientation='vertical']:overflow-x-hidden",
        // For horizontal, allow scroll overflow
        "data-[orientation='horizontal']:overflow-x-auto data-[orientation='horizontal']:overflow-y-hidden",
        className,
        "mb-1"
      )}
      style={{
        height:"100%"
      }}
      {...props}
    />
  )
);
TabsList.displayName = "TabsList";

type TabsTriggerProps = React.ComponentPropsWithoutRef<typeof RadixTabs.Trigger> & { disabled?: boolean };

const TabsTrigger = React.forwardRef<React.ComponentRef<typeof RadixTabs.Trigger>, TabsTriggerProps>(
  ({ className, children, disabled = false, ...props }, ref) => (
    <RadixTabs.Trigger
      ref={ref}
      className={cn(
        triggerVariants({ disabled }),
        // Responsive sizing & padding
        "min-w-0 w-full h-[38px] px-2 text-xs",
        "sm:min-w-[96px] sm:h-[40px] sm:px-2 sm:text-sm",
        "md:min-w-[120px] md:h-[44px] md:px-3 md:text-base",
        "lg:min-w-[140px] lg:h-[48px] lg:px-6 lg:text-lg",
        // Stacking/vertical orientation tweaks
        "data-[orientation='vertical']:wc-tab-trigger-vertical data-[orientation='vertical']:w-full data-[orientation='vertical']:min-w-[100px] data-[orientation='vertical']:max-w-full data-[orientation='vertical']:h-[38px] data-[orientation='vertical']:min-h-[38px] data-[orientation='vertical']:my-0 data-[orientation='vertical']:mb-1 data-[orientation='vertical']:sm:mb-[2px] data-[orientation='vertical']:px-2 data-[orientation='vertical']:text-xs data-[orientation='vertical']:md:px-4 data-[orientation='vertical']:md:text-base data-[orientation='vertical']:lg:px-6 data-[orientation='vertical']:lg:text-lg data-[orientation='vertical']:text-left data-[orientation='vertical']:justify-start data-[orientation='vertical']:rounded-l-lg data-[orientation='vertical']:border-l-4 data-[orientation='vertical']:border-solid data-[orientation='vertical']:border-transparent",
        className
      )}
      disabled={disabled}
      aria-disabled={disabled}
      tabIndex={disabled ? -1 : undefined}
      {...props}
    >
      <span className={cn(
        "tab-label relative truncate tab-label-base"
        )}>
        {children}
      </span>
      <span className="tab-burst" />
    </RadixTabs.Trigger>
  )
);
TabsTrigger.displayName = "TabsTrigger";

// Responsive content panel sizing and paddings
const TabsContent = React.forwardRef<
  React.ComponentRef<typeof RadixTabs.Content>,
  React.ComponentPropsWithoutRef<typeof RadixTabs.Content>
>(({ className, ...props }, ref) => (
  <RadixTabs.Content
    ref={ref}
    className={cn(
      "wc-tab-content relative p-2 min-h-[140px] text-[#f3e7c4] bg-cover bg-no-repeat bg-center flex-1 md:w-full overflow-x-auto",
      // Adaptive paddings and height for each breakpoint
      "sm:p-4 sm:min-h-[180px]",
      "md:p-6 md:min-h-[220px]",
      "lg:p-20 lg:min-h-[360px]",
	    "lg:data-[orientation='vertical']:w-[400px]",
      // Allow responsive vertical orientation height
      "data-[orientation='vertical']:min-h-[140px] data-[orientation='vertical']:h-auto",      
      className
    )}
    {...props}
  />
));
TabsContent.displayName = "TabsContent";

export { Tabs, TabsList, TabsTrigger, TabsContent };