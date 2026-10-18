---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/components/docs/showcase.tsx"
ref: HEAD
---

"use client";

import { Bell } from "lucide-react";

import { Accordion, AccordionContent, AccordionItem, AccordionTrigger } from "@/components/ui/warcraftcn/accordion";
import { Avatar } from "@/components/ui/warcraftcn/avatar";
import { Badge } from "@/components/ui/warcraftcn/badge";
import { Button } from "@/components/ui/warcraftcn/button";
import { Checkbox } from "@/components/ui/warcraftcn/checkbox";
import { Cursor } from "@/components/ui/warcraftcn/cursor";
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
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/warcraftcn/tabs";
import { Textarea } from "@/components/ui/warcraftcn/textarea";
import { triggerScrollToast } from "@/components/ui/warcraftcn/toast";
import {
  Tooltip,
  TooltipBody,
  TooltipContent,
  TooltipTitle,
  TooltipTrigger,
} from "@/components/ui/warcraftcn/tooltip";

function Section({
  title,
  children,
}: {
  title: string;
  children: React.ReactNode;
}) {
  return (
    <section className="not-prose mt-10">
      <h3 className="fantasy mb-4 font-semibold text-amber-300 text-xl">
        {title}
      </h3>
      <div className="flex flex-wrap items-start gap-4 rounded-lg border border-amber-900/30 bg-black/20 p-6">
        {children}
      </div>
    </section>
  );
}

const FACTIONS = ["orc", "human", "elf", "undead"] as const;

export function Showcase() {
  return (
    <div className="not-prose">
      <Section title="Buttons">
        <Button>Forge Map</Button>
        <Button variant="frame">Open…</Button>
        <Button disabled>Saving…</Button>
      </Section>

      <Section title="Badges">
        <Badge>Default</Badge>
        <Badge variant="secondary">Secondary</Badge>
        <Badge variant="destructive">Destructive</Badge>
        <Badge faction="alliance">Alliance</Badge>
        <Badge faction="horde">Horde</Badge>
        <Badge shape="shield">Shield</Badge>
        <Badge shape="banner">Banner</Badge>
      </Section>

      <Section title="Tooltips — item rarity">
        {(["default", "uncommon", "rare", "epic", "legendary"] as const).map(
          (variant) => (
            <Tooltip key={variant}>
              <TooltipTrigger asChild>
                <Button variant="frame">{variant}</Button>
              </TooltipTrigger>
              <TooltipContent variant={variant}>
                <TooltipTitle>Tome of {variant}</TooltipTitle>
                <TooltipBody>Hover styling keyed to item quality.</TooltipBody>
              </TooltipContent>
            </Tooltip>
          )
        )}
      </Section>

      <Section title="Tabs">
        <Tabs className="w-full max-w-xl" defaultValue="orc" faction="orc">
          <TabsList>
            <TabsTrigger value="orc">Orc</TabsTrigger>
            <TabsTrigger value="human">Human</TabsTrigger>
            <TabsTrigger value="elf">Elf</TabsTrigger>
            <TabsTrigger value="undead">Undead</TabsTrigger>
          </TabsList>
          <TabsContent value="orc">Fierce warriors forged in battle.</TabsContent>
          <TabsContent value="human">Defenders of the Alliance.</TabsContent>
          <TabsContent value="elf">Guardians of the ancient forests.</TabsContent>
          <TabsContent value="undead">Minions of the Lich King.</TabsContent>
        </Tabs>
      </Section>

      <Section title="Accordion">
        <Accordion className="w-full max-w-xl" collapsible type="single">
          <AccordionItem value="a">
            <AccordionTrigger icon="sword">What is wc3-forge?</AccordionTrigger>
            <AccordionContent>
              A native Warcraft III map editor with an embedded MCP server.
            </AccordionContent>
          </AccordionItem>
          <AccordionItem value="b">
            <AccordionTrigger icon="shield">Is it open source?</AccordionTrigger>
            <AccordionContent>Yes — GPL-3.0-or-later.</AccordionContent>
          </AccordionItem>
        </Accordion>
      </Section>

      <Section title="Avatars — factions">
        <Avatar fallback="🪓" faction="orc" size="sm" />
        <Avatar fallback="🛡️" faction="human" size="sm" />
        <Avatar fallback="🍃" faction="elf" size="sm" />
        <Avatar fallback="💀" faction="undead" size="sm" />
      </Section>

      <Section title="Checkboxes & radios">
        <div className="flex flex-col gap-2">
          {FACTIONS.map((f) => (
            <Checkbox defaultChecked={f === "orc"} faction={f} id={`cb-${f}`} key={f}>
              {f}
            </Checkbox>
          ))}
        </div>
        <RadioGroup className="ml-8" defaultValue="orc">
          {FACTIONS.map((f) => (
            <div className="flex items-center gap-2" key={f}>
              <RadioGroupItem id={`rb-${f}`} value={f} />
              <Label htmlFor={`rb-${f}`}>{f}</Label>
            </div>
          ))}
        </RadioGroup>
      </Section>

      <Section title="Form fields">
        <div className="flex w-full max-w-md flex-col gap-3">
          <Label htmlFor="map-name" required>
            Map name
          </Label>
          <Input id="map-name" placeholder="(2)EchoIsles.w3x" />
          <Label htmlFor="map-desc">Description</Label>
          <Textarea id="map-desc" placeholder="A duel map for two players…" />
        </div>
      </Section>

      <Section title="Pagination">
        <Pagination>
          <PaginationContent>
            <PaginationItem>
              <PaginationPrevious href="#" />
            </PaginationItem>
            <PaginationItem>
              <PaginationLink href="#">1</PaginationLink>
            </PaginationItem>
            <PaginationItem>
              <PaginationLink href="#" isActive>
                2
              </PaginationLink>
            </PaginationItem>
            <PaginationItem>
              <PaginationLink href="#">3</PaginationLink>
            </PaginationItem>
            <PaginationItem>
              <PaginationEllipsis />
            </PaginationItem>
            <PaginationItem>
              <PaginationNext href="#" />
            </PaginationItem>
          </PaginationContent>
        </Pagination>
      </Section>

      <Section title="Spinner & skeletons">
        <Spinner />
        <div className="flex flex-1 flex-wrap gap-3">
          {FACTIONS.map((f) => (
            <Skeleton className="h-16 w-32" faction={f} key={f} />
          ))}
        </div>
      </Section>

      <Section title="Toast (Sonner)">
        <Button
          onClick={() =>
            triggerScrollToast({
              message: "Map saved to the archives!",
              faction: "human",
              variant: "success",
            })
          }
        >
          <Bell className="size-4" /> Summon a scroll
        </Button>
      </Section>

      <Section title="Cursor regions">
        {FACTIONS.map((f) => (
          <Cursor faction={f} key={f}>
            <div className="grid h-20 w-32 place-content-center rounded-md border border-amber-900/40 bg-black/30 text-amber-200 text-sm capitalize">
              {f}
            </div>
          </Cursor>
        ))}
      </Section>
    </div>
  );
}
