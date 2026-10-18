---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/components/cursor-picker.tsx"
ref: HEAD
---

"use client";

import { ChevronDown, MousePointer2 } from "lucide-react";
import { useEffect, useState } from "react";

import { Button } from "@/components/ui/warcraftcn/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/warcraftcn/dropdown-menu";

const FACTIONS = [
  { key: "orc", label: "Orc", glyph: "🪓" },
  { key: "human", label: "Human", glyph: "🛡️" },
  { key: "elf", label: "Night Elf", glyph: "🍃" },
  { key: "undead", label: "Undead", glyph: "💀" },
  { key: "default", label: "System", glyph: "🖱️" },
] as const;

type Faction = (typeof FACTIONS)[number]["key"];

const CURSOR_CLASSES = [
  "wc-orc-cursor",
  "wc-human-cursor",
  "wc-elf-cursor",
  "wc-undead-cursor",
];

const STORAGE_KEY = "wc3-forge:cursor-faction";

function applyFaction(faction: Faction) {
  const body = document.body;
  body.classList.remove(...CURSOR_CLASSES);
  if (faction !== "default") {
    body.classList.add(`wc-${faction}-cursor`);
  }
}

export function CursorPicker() {
  const [faction, setFaction] = useState<Faction>("orc");

  // Restore the saved choice and apply the default cursor on first paint.
  useEffect(() => {
    const saved = (localStorage.getItem(STORAGE_KEY) as Faction) || "orc";
    setFaction(saved);
    applyFaction(saved);
  }, []);

  const choose = (next: Faction) => {
    setFaction(next);
    applyFaction(next);
    localStorage.setItem(STORAGE_KEY, next);
  };

  const current = FACTIONS.find((f) => f.key === faction) ?? FACTIONS[0];

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button
          aria-label="Choose cursor"
          className="h-9 gap-1.5 px-3 text-sm"
          variant="frame"
        >
          <MousePointer2 className="size-4" />
          <span className="hidden sm:inline">{current.glyph}</span>
          <ChevronDown className="size-3 opacity-70" />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end">
        <DropdownMenuLabel>Cursor</DropdownMenuLabel>
        <DropdownMenuSeparator />
        {FACTIONS.map((f) => (
          <DropdownMenuItem
            key={f.key}
            onSelect={() => choose(f.key)}
            data-active={f.key === faction}
            className="data-[active=true]:text-amber-300"
          >
            <span aria-hidden className="mr-1">
              {f.glyph}
            </span>
            {f.label}
            {f.key === faction && (
              <span className="ml-auto text-amber-400">✦</span>
            )}
          </DropdownMenuItem>
        ))}
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
