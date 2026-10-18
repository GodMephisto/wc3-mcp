---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/mdx-components.tsx"
ref: HEAD
---

import defaultMdxComponents from "fumadocs-ui/mdx";
import {
  Accordion as FdAccordion,
  Accordions,
} from "fumadocs-ui/components/accordion";
import { Callout } from "fumadocs-ui/components/callout";
import { Card as FdCard, Cards } from "fumadocs-ui/components/card";
import { Tab, Tabs as FdTabs } from "fumadocs-ui/components/tabs";
import type { MDXComponents } from "mdx/types";

import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/components/ui/warcraftcn/accordion";
import { Avatar } from "@/components/ui/warcraftcn/avatar";
import { Badge } from "@/components/ui/warcraftcn/badge";
import { Button } from "@/components/ui/warcraftcn/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/warcraftcn/card";
import { Checkbox } from "@/components/ui/warcraftcn/checkbox";
import { Cursor } from "@/components/ui/warcraftcn/cursor";
import { Showcase } from "@/components/docs/showcase";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/warcraftcn/dropdown-menu";
import { Input } from "@/components/ui/warcraftcn/input";
import { Label } from "@/components/ui/warcraftcn/label";
import {
  Pagination,
  PaginationContent,
  PaginationEllipsis,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
} from "@/components/ui/warcraftcn/pagination";
import { RadioGroup, RadioGroupItem } from "@/components/ui/warcraftcn/radio-group";
import { Skeleton } from "@/components/ui/warcraftcn/skeleton";
import { Spinner } from "@/components/ui/warcraftcn/spinner";
import {
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
} from "@/components/ui/warcraftcn/tabs";
import { Textarea } from "@/components/ui/warcraftcn/textarea";
import {
  Tooltip,
  TooltipBody,
  TooltipContent,
  TooltipProvider,
  TooltipTitle,
  TooltipTrigger,
} from "@/components/ui/warcraftcn/tooltip";

// Every warcraftcn component is exposed to MDX under a `Wc*` name so docs pages
// can show them off without clashing with Fumadocs' own Card / Tabs / Accordion.
export function getMDXComponents(components?: MDXComponents): MDXComponents {
  return {
    ...defaultMdxComponents,
    // Fumadocs interactive MDX components used in the docs.
    Callout,
    Card: FdCard,
    Cards,
    Tabs: FdTabs,
    Tab,
    Accordion: FdAccordion,
    Accordions,
    Cursor,
    Showcase,
    WcButton: Button,
    WcBadge: Badge,
    WcAvatar: Avatar,
    WcCheckbox: Checkbox,
    WcInput: Input,
    WcLabel: Label,
    WcTextarea: Textarea,
    WcSkeleton: Skeleton,
    WcSpinner: Spinner,
    WcCard: Card,
    WcCardHeader: CardHeader,
    WcCardFooter: CardFooter,
    WcCardTitle: CardTitle,
    WcCardDescription: CardDescription,
    WcCardContent: CardContent,
    WcAccordion: Accordion,
    WcAccordionItem: AccordionItem,
    WcAccordionTrigger: AccordionTrigger,
    WcAccordionContent: AccordionContent,
    WcTabs: Tabs,
    WcTabsList: TabsList,
    WcTabsTrigger: TabsTrigger,
    WcTabsContent: TabsContent,
    WcTooltip: Tooltip,
    WcTooltipTrigger: TooltipTrigger,
    WcTooltipContent: TooltipContent,
    WcTooltipTitle: TooltipTitle,
    WcTooltipBody: TooltipBody,
    WcTooltipProvider: TooltipProvider,
    WcRadioGroup: RadioGroup,
    WcRadioGroupItem: RadioGroupItem,
    WcPagination: Pagination,
    WcPaginationContent: PaginationContent,
    WcPaginationItem: PaginationItem,
    WcPaginationLink: PaginationLink,
    WcPaginationPrevious: PaginationPrevious,
    WcPaginationNext: PaginationNext,
    WcPaginationEllipsis: PaginationEllipsis,
    WcDropdownMenu: DropdownMenu,
    WcDropdownMenuTrigger: DropdownMenuTrigger,
    WcDropdownMenuContent: DropdownMenuContent,
    WcDropdownMenuItem: DropdownMenuItem,
    WcDropdownMenuLabel: DropdownMenuLabel,
    WcDropdownMenuSeparator: DropdownMenuSeparator,
    ...components,
  };
}
