---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/app/(home)/page.tsx"
ref: HEAD
---

import Image from "next/image";
import Link from "next/link";
import { DownloadButton } from "@/components/download-button";
import { Badge } from "@/components/ui/warcraftcn/badge";
import { Button } from "@/components/ui/warcraftcn/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/warcraftcn/card";
import { asset } from "@/lib/asset";

const features = [
  {
    title: "Terrain & doodads",
    body: "Sculpt tiles and height, place and transform units and doodads — move, rotate, scale, create, delete.",
  },
  {
    title: "Full Object Editor",
    body: "Read and write all 7 definition kinds: units, items, abilities, buffs, destructables, doodads, upgrades.",
  },
  {
    title: "Complete Trigger Editor",
    body: "GUI tree + Monaco code view + WC3 IntelliSense, GUI→Lua/JASS codegen, Convert-Map-to-Lua, and Test Map.",
  },
  {
    title: "Saves in place",
    body: "Writes packaged .w3x / MPQ archives losslessly. Older JASS maps open, edit, save, and test directly.",
  },
  {
    title: "Embedded MCP server",
    body: "The editor binary is its own MCP server. No Node, no extra install — just point Claude Code at it.",
  },
  {
    title: "One shared undo stack",
    body: "Hand edits and agent edits flow through the same history. Ctrl+Z undoes Claude's work and vice versa.",
  },
];

export default function Home() {
  return (
    <main className="flex flex-col items-center px-4 pb-24">
      {/* Gradient glow */}
      <div className="pointer-events-none absolute inset-0 overflow-hidden">
        <div className="-translate-x-1/2 absolute top-40 left-1/2 h-[600px] w-[900px] rounded-full bg-[radial-gradient(circle,rgba(255,171,1,0.18)_0%,rgba(82,214,252,0.12)_45%,transparent_75%)] blur-[120px]" />
      </div>

      {/* Hero */}
      <section className="relative z-10 flex w-full max-w-4xl flex-col items-center gap-6 pt-16 text-center">
        <Image
          alt="wc3-forge"
          className="size-20 drop-shadow-[0_0_20px_rgba(255,171,1,0.35)]"
          height={96}
          src={asset("/logo.png")}
          width={96}
        />
        <div className="flex items-center gap-2">
          <Badge faction="alliance" size="sm">
            Alpha
          </Badge>
          <Badge size="sm" variant="secondary">
            Go + Wails + MCP
          </Badge>
        </div>
        <h1 className="fantasy max-w-3xl font-bold text-4xl text-amber-300 md:text-6xl">
          A Warcraft III map editor, driven by{" "}
          <span className="inline-flex items-center gap-2 whitespace-nowrap">
            <Image
              alt="Claude"
              className="inline-block size-9 align-middle md:size-12"
              height={48}
              src={asset("/claude.svg")}
              width={48}
            />
            Claude Code
          </span>
        </h1>
        <p className="max-w-2xl text-lg text-muted-foreground">
          wc3-forge is a native WC3 map editor with an embedded MCP server. Every
          edit you make by hand can also be made by an agent — and vice versa —
          across one shared editor session and one shared undo stack.
        </p>

        <div className="flex flex-col items-center gap-4 pt-2" id="download">
          <DownloadButton />
          <Link href="/docs">
            <Button className="px-8" variant="frame">
              Read the docs
            </Button>
          </Link>
        </div>
      </section>

      {/* Screenshot */}
      <section className="relative z-10 mt-16 w-full max-w-5xl">
        <div className="overflow-hidden rounded-lg border-2 border-amber-900/40 shadow-2xl">
          <Image
            alt="wc3-forge editor"
            className="h-auto w-full"
            height={1000}
            priority
            sizes="(max-width: 1024px) 100vw, 1024px"
            src={asset("/hero.png")}
            width={1600}
          />
        </div>
      </section>

      {/* Features */}
      <section className="relative z-10 mt-20 grid w-full max-w-5xl gap-5 sm:grid-cols-2 lg:grid-cols-3">
        {features.map((f) => (
          <Card key={f.title} className="text-white">
            <CardHeader>
              <CardTitle className="fantasy text-amber-200">{f.title}</CardTitle>
            </CardHeader>
            <CardContent>
              <CardDescription className="text-stone-300">
                {f.body}
              </CardDescription>
            </CardContent>
          </Card>
        ))}
      </section>

      {/* Claude Code angle */}
      <section className="relative z-10 mt-20 w-full max-w-3xl text-center">
        <h2 className="fantasy font-bold text-3xl text-amber-300">
          Two control surfaces, one editor
        </h2>
        <p className="mt-4 text-muted-foreground">
          The GUI and the MCP bridge mutate the same session and emit the same
          events. Ask Claude what's on the map, move a gold mine to a start
          location, retune an ability, or convert a map's triggers to Lua — and
          watch every call stream live in the in-editor Agent Console.
        </p>
        <div className="mt-8 flex justify-center gap-4">
          <Link href="/docs/claude-code">
            <Button>Set up Claude Code</Button>
          </Link>
          <a
            href="https://github.com/StephenSHorton/wc3-forge"
            rel="noopener noreferrer"
            target="_blank"
          >
            <Button variant="frame">View on GitHub</Button>
          </a>
        </div>
      </section>
    </main>
  );
}
