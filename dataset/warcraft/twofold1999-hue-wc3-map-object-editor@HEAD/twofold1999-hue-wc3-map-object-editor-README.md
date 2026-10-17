---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "README.md"
ref: HEAD
---

<pre class="overflow-visible! px-0!" data-start="265" data-end="1301"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span># wc3-map-object-editor</span><br/><br/><span>A TypeScript CLI for importing, inspecting, editing, exporting, and round-trip validating Warcraft III object data through Patchwork.</span><br/><br/><span>This project focuses on the **object-data layer** of Warcraft III maps and is evolving into a practical **Warcraft III map modding toolbox** built on top of a stable Patchwork-based workflow.</span><br/><br/><span>---</span><br/><br/><span>## Why this project exists</span><br/><br/><span>Warcraft III object data is difficult to work with directly because it sits between:</span><br/><br/><span>- binary object files such as `war3map.w3u`, `war3map.w3a`, `war3map.w3t`</span><br/><span>- string-table based text references such as `TRIGSTR_*`</span><br/><span>- object-specific field structures that vary across units, abilities, and items</span><br/><span>- repeated and multi-level data patterns, especially in abilities</span><br/><span>- gameplay logic that often lives separately in `war3map.j`</span><br/><br/><span>This project explores a cleaner workflow:</span><br/><br/><span>```text</span><br/><span>Warcraft III object files</span><br/><span>-> Patchwork conversion</span><br/><span>-> internal unified model</span><br/><span>-> CLI inspect/edit</span><br/><span>-> export back to Patchwork JSON / war3map.*</span><br/><span>-> round-trip validation</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

The goal is not just to parse files once, but to build a **repeatable, verifiable object-data round-trip pipeline** that can also support practical map modding features.

---

## Project highlights

* **Patchwork-based import/export pipeline**
* **Unified internal `MapDataFile` model**
* **WTS string resolution for readable names/descriptions**
* **Support for repeated and multi-level ability field families**
* **Patchwork JSON export symmetry**
* **One-command round-trip validation**
* **First-pass semantic field refinement for unit combat/attack/armor fields**
* **Toolbox-style gameplay commands for practical map modding workflows**
* **Five first-pass map-level script modules built on targeted `war3map.j` rewriting**
* **Named preset systems for hero growth, starting resources, starting hero power, and player-specific starting resources**
* **A reusable script transform helper layer for `war3map.j` updates**
* **A dedicated module-level updater layer for starting hero power logic**
* **Validated across first-batch, second-batch, and third-batch samples**

---

## Current milestone

**v0.8 - script transform layering and fifth gameplay module**

At this stage, the project supports:

* Patchwork import for units / abilities / items
* WTS string resolution
* internal object model conversion
* CLI inspect/edit flow
* Patchwork JSON export
* Patchwork `json2war`
* first-batch validation
* second-batch validation
* third-batch validation
* one-command round-trip validation
* broader repeated ability field support
* broader item text/cost field support
* expanded unit combat-field coverage
* toolbox-style object tuning commands
* five map-level gameplay script modules:
* kill reward / hero growth
* starting resources
* starting hero power
* player-specific starting settings
* player-specific starting hero power
* shared script transform helpers for function-block and marker-block rewriting
* a dedicated updater layer for starting-hero-power block updates

This milestone reflects the point where the project moved beyond a multi-command toolbox and into a more structured gameplay scripting toolkit with reusable internal transformation layers.

---

## Next milestone direction

**v0.9 - broader gameplay modules and cleaner internal reuse**

The next phase focuses on two parallel directions:

1. continuing semantic refinement of the internal object model
2. expanding gameplay-level tooling while keeping internal script transformation logic disciplined

On the model side, the project is continuing to move from raw field capture toward more useful semantic values such as:

* `combatType`
* `attackType`
* `armorType`

On the gameplay side, the project now has:

* five stable script module families
* a shared `scriptTransforms` layer
* module-specific updater utilities where needed

That makes the next likely directions:

* adding a sixth clearly scoped gameplay module
* improving script transformation reuse without over-abstracting
* keeping module-level business logic cleanly separated from command registration

The strategy remains intentionally conservative:

* preserve raw imported fields
* derive semantic fields only from already observed real sample values
* avoid large speculative enum systems too early
* keep the importer/exporter/round-trip workflow stable while refining semantics
* grow script commands through small, clearly validated transformations
* only extract shared helpers when repetition is real and stable

---

## Core workflow

<pre class="overflow-visible! px-0!" data-start="4814" data-end="5086"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute end-1.5 top-1 z-2 md:end-2 md:top-1"></div><div class="relative"><div class="pe-11 pt-3"><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span>Patchwork-compatible input directory</span><br/><span>-> Patchwork war2json</span><br/><span>-> Patchwork JSON</span><br/><span>-> PatchworkProjectImporter</span><br/><span>-> MapDataFile</span><br/><span>-> CLI inspect/edit</span><br/><span>-> PatchworkProjectExporter</span><br/><span>-> Patchwork JSON</span><br/><span>-> Patchwork json2war</span><br/><span>-> generated war3map.* files</span><br/><span>-> re-import validation</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

---

## What is currently supported

### Units

Current support includes a growing set of unit fields such as:

* name
* hitPoints
* acquisitionRange
* attackBaseDamage
* secondaryAttackBase
* primary / secondary attack range
* primary / secondary attack speed
* primary / secondary weapon type text
* secondary attack type text
* primary target flags / target preferences
* turn rate / secondary turn rate
* movement priority
* various cost and combat numeric fields

First-pass semantic refinement has also started for:

* `combatType`
* `attackType`
* `armorType`

### Abilities

Current support includes:

* name
* manaCost
* cooldown
* castRange
* maxLevel
* repeated multi-level values
* repeated Patchwork ability fields such as:
* `mls1`
* `Inf1`
* `aare`
* repeated `amcs`
* repeated `acdn`
* repeated `adur`
* repeated `ahdu`
* scalar attribute-style fields such as:
* `bonusStrength`
* `bonusAgility`
* `bonusIntelligence`

### Items

Current support includes:

* name
* goldCost
* lumberCost
* cooldown
* description
* tooltipText
* properNameText
* abilityId
* level
* levelVariance
* priority
* strengthBonus
* armorTypeText

For the detailed mapping list, see the docs folder.

---

## Example commands

### Import object data

<pre class="overflow-visible! px-0!" data-start="6351" data-end="6425"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> import-patchwork-dir ./samples/probe-input</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

### Inspect objects

<pre class="overflow-visible! px-0!" data-start="6448" data-end="6661"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> unit list</span><br/><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> ability list</span><br/><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> item list</span><br/><br/><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> unit show h001</span><br/><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> ability show A002</span><br/><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> item show I000</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

### Inspect raw Patchwork fields

<pre class="overflow-visible! px-0!" data-start="6697" data-end="6813"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> unit raw h001</span><br/><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> ability raw A002</span><br/><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> item raw I000</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

### Export Patchwork JSON

<pre class="overflow-visible! px-0!" data-start="6842" data-end="6914"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> export-patchwork ./temp/patchwork-export</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

### Export Warcraft III object files

<pre class="overflow-visible! px-0!" data-start="6954" data-end="7051"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> export-patchwork-war ./temp/patchwork-export ./temp/patchwork-war</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

### Run full round-trip validation

<pre class="overflow-visible! px-0!" data-start="7089" data-end="7164"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> validate-roundtrip ./samples/probe-input-02</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

---

## Toolbox commands

In addition to low-level object inspection and field-by-field editing, the project now includes a first-pass **toolbox layer** for faster map modding workflows.

These commands are designed to update gameplay-relevant values through more goal-oriented entry points instead of only low-level field edits.

The toolbox currently spans both:

* **object-data tuning**
* **map-level gameplay script modification**

### Buff a unit

You can update multiple unit stats in one command, either by passing values directly or by applying a preset.

Manual example:

<pre class="overflow-visible! px-0!" data-start="7748" data-end="7852"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> unit buff h000 </span><span class="ͼf">--hp</span><span></span><span class="ͼb">5000</span><span></span><span class="ͼf">--damage</span><span></span><span class="ͼb">200</span><span></span><span class="ͼf">--armor</span><span></span><span class="ͼb">20</span><span></span><span class="ͼf">--range</span><span></span><span class="ͼb">900</span><span></span><span class="ͼf">--speed</span><span></span><span class="ͼb">350</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

Preset examples:

<pre class="overflow-visible! px-0!" data-start="7872" data-end="7982"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> unit buff h000 </span><span class="ͼf">--preset</span><span> boss</span><br/><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> unit buff h000 </span><span class="ͼf">--preset</span><span> elite</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

Supported flags:

* `--preset`
* `--hp`
* `--damage`
* `--armor`
* `--range`
* `--speed`

Current presets:

* `boss`
* `elite`

Manual flags can override preset values.

### Buff an item

<pre class="overflow-visible! px-0!" data-start="8172" data-end="8275"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> item buff I000 </span><span class="ͼf">--gold</span><span></span><span class="ͼb">5000</span><span></span><span class="ͼf">--lumber</span><span></span><span class="ͼb">200</span><span></span><span class="ͼf">--str</span><span></span><span class="ͼb">20</span><span></span><span class="ͼf">--level</span><span></span><span class="ͼb">9</span><span></span><span class="ͼf">--cooldown</span><span></span><span class="ͼb">3</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

Supported flags:

* `--gold`
* `--lumber`
* `--str`
* `--level`
* `--cooldown`

### Tune an ability

<pre class="overflow-visible! px-0!" data-start="8378" data-end="8462"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> ability tune A000 </span><span class="ͼf">--mana</span><span></span><span class="ͼb">50</span><span></span><span class="ͼf">--cooldown</span><span></span><span class="ͼb">2</span><span></span><span class="ͼf">--range</span><span></span><span class="ͼb">800</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

Supported flags:

* `--mana`
* `--cooldown`
* `--range`

---

## Map-level gameplay script modules

The project now includes five focused `war3map.j` gameplay modules.

### Module 1: Kill reward / hero growth

This module is centered on `Trig_Kills_Actions`.

#### Kill gold multiplier

<pre class="overflow-visible! px-0!" data-start="8751" data-end="8865"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-kill-gold-multiplier </span><span class="ͼf">--value</span><span></span><span class="ͼb">2</span><span></span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

#### Kill gold flat bonus

<pre class="overflow-visible! px-0!" data-start="8894" data-end="9010"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-kill-gold-flat-bonus </span><span class="ͼf">--value</span><span></span><span class="ͼb">100</span><span></span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

#### Kill lumber bonus

<pre class="overflow-visible! px-0!" data-start="9036" data-end="9147"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-kill-lumber-bonus </span><span class="ͼf">--value</span><span></span><span class="ͼb">8</span><span></span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

#### Kill hero stat bonus

<pre class="overflow-visible! px-0!" data-start="9176" data-end="9529"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-kill-hero-stat-bonus </span><span class="ͼf">--stat</span><span> str </span><span class="ͼf">--value</span><span></span><span class="ͼb">2</span><span></span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span><br/><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-kill-hero-stat-bonus </span><span class="ͼf">--stat</span><span> agi </span><span class="ͼf">--value</span><span></span><span class="ͼb">1</span><span></span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span><br/><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-kill-hero-stat-bonus </span><span class="ͼf">--stat</span><span> int </span><span class="ͼf">--value</span><span></span><span class="ͼb">2</span><span></span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

Supported stats:

* `str`
* `agi`
* `int`

#### Kill hero XP bonus

<pre class="overflow-visible! px-0!" data-start="9599" data-end="9708"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-kill-exp-bonus </span><span class="ͼf">--value</span><span></span><span class="ͼb">50</span><span></span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

#### Hero growth preset

<pre class="overflow-visible! px-0!" data-start="9735" data-end="10427"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-kill-hero-growth-preset </span><span class="ͼf">--preset</span><span> warrior </span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span><br/><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-kill-hero-growth-preset </span><span class="ͼf">--preset</span><span> mage </span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span><br/><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-kill-hero-growth-preset </span><span class="ͼf">--preset</span><span> assassin </span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span><br/><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-kill-hero-growth-preset </span><span class="ͼf">--preset</span><span> tank </span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span><br/><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-kill-hero-growth-preset </span><span class="ͼf">--preset</span><span> support </span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span><br/><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-kill-hero-growth-preset </span><span class="ͼf">--preset</span><span> warrior </span><span class="ͼf">--int</span><span></span><span class="ͼb">2</span><span></span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

Supported preset values:

* `warrior`
* `mage`
* `assassin`
* `tank`
* `support`

Current preset values:

* `warrior`
* `str = 3`
* `agi = 1`
* `int = 0`
* `xp = 40`
* `mage`
* `str = 0`
* `agi = 1`
* `int = 3`
* `xp = 40`
* `assassin`
* `str = 1`
* `agi = 3`
* `int = 0`
* `xp = 40`
* `tank`
* `str = 4`
* `agi = 0`
* `int = 0`
* `xp = 50`
* `support`
* `str = 1`
* `agi = 1`
* `int = 2`
* `xp = 60`

Behavior:

* explicit numeric flags override preset values
* zero-valued growth entries are omitted from final output
* final hero growth output is normalized into one shared hero growth block

#### Kill trigger formatting

<pre class="overflow-visible! px-0!" data-start="11095" data-end="11195"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> format-kills-trigger </span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

Behavior:

* merges repeated hero stat / XP blocks into one shared hero growth block
* applies canonical ordering inside the hero block:
* `SetHeroStr`
* `SetHeroAgi`
* `SetHeroInt`
* `AddHeroXP`

---

### Module 2: Starting resources

This module is centered on `Trig_Initialization_Actions`.

#### Starting gold

<pre class="overflow-visible! px-0!" data-start="11520" data-end="11629"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-starting-gold </span><span class="ͼf">--value</span><span></span><span class="ͼb">900</span><span></span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

#### Starting lumber

<pre class="overflow-visible! px-0!" data-start="11653" data-end="11764"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-starting-lumber </span><span class="ͼf">--value</span><span></span><span class="ͼb">200</span><span></span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

#### Starting resources preset

<pre class="overflow-visible! px-0!" data-start="11798" data-end="12162"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-starting-resources-preset </span><span class="ͼf">--preset</span><span> standard </span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span><br/><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-starting-resources-preset </span><span class="ͼf">--preset</span><span> rich </span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span><br/><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-starting-resources-preset </span><span class="ͼf">--preset</span><span> scarce </span><span class="ͼf">--gold</span><span></span><span class="ͼb">300</span><span></span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

Supported preset values:

* `standard`
* `rich`
* `scarce`

Current preset values:

* `standard`
* `gold = 500`
* `lumber = 150`
* `rich`
* `gold = 1000`
* `lumber = 250`
* `scarce`
* `gold = 250`
* `lumber = 50`

Behavior:

* unified resource blocks use compact begin/end markers
* explicit numeric flags override preset values
* repeated execution updates existing blocks instead of duplicating them

---

### Module 3: Starting hero power

This module is centered on the known hero-selection action functions:

* `Trig_Hermit_Crab_Actions`
* `Trig_Cameo_Crab_Actions`
* `Trig_Trove_Lobster_Actions`
* `Trig_Spiny_Snapper_Actions`
* `Trig_Frog_Jester_Actions`

#### Starting hero level

<pre class="overflow-visible! px-0!" data-start="12865" data-end="12978"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-starting-hero-level </span><span class="ͼf">--value</span><span></span><span class="ͼb">5</span><span></span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

#### Starting hero skill points

<pre class="overflow-visible! px-0!" data-start="13013" data-end="13133"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-starting-hero-skill-points </span><span class="ͼf">--value</span><span></span><span class="ͼb">3</span><span></span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

#### Starting hero power formatting

<pre class="overflow-visible! px-0!" data-start="13172" data-end="13278"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> format-starting-hero-power </span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

#### Starting hero power preset

<pre class="overflow-visible! px-0!" data-start="13313" data-end="13687"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-starting-hero-power-preset </span><span class="ͼf">--preset</span><span> standard </span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span><br/><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-starting-hero-power-preset </span><span class="ͼf">--preset</span><span> strong </span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span><br/><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-starting-hero-power-preset </span><span class="ͼf">--preset</span><span> elite </span><span class="ͼf">--skill-points</span><span></span><span class="ͼb">9</span><span></span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

Supported preset values:

* `standard`
* `strong`
* `elite`

Current preset values:

* `standard`
* `level = 3`
* `skill-points = 1`
* `strong`
* `level = 6`
* `skill-points = 4`
* `elite`
* `level = 10`
* `skill-points = 8`

Behavior:

* explicit numeric flags override preset values
* updates remain compact and readable across all 5 supported action functions

---

### Module 4: Player-specific starting settings

This module extends the starting-resources system with per-player override blocks inside `Trig_Initialization_Actions`.

#### Player-specific starting gold

<pre class="overflow-visible! px-0!" data-start="14276" data-end="14404"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-player-starting-gold </span><span class="ͼf">--player</span><span></span><span class="ͼb">1</span><span></span><span class="ͼf">--value</span><span></span><span class="ͼb">1500</span><span></span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

#### Player-specific starting lumber

<pre class="overflow-visible! px-0!" data-start="14444" data-end="14573"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-player-starting-lumber </span><span class="ͼf">--player</span><span></span><span class="ͼb">1</span><span></span><span class="ͼf">--value</span><span></span><span class="ͼb">300</span><span></span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

#### Player-specific starting resources preset

<pre class="overflow-visible! px-0!" data-start="14623" data-end="15041"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-player-starting-resources-preset </span><span class="ͼf">--player</span><span></span><span class="ͼb">1</span><span></span><span class="ͼf">--preset</span><span> vip </span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span><br/><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-player-starting-resources-preset </span><span class="ͼf">--player</span><span></span><span class="ͼb">1</span><span></span><span class="ͼf">--preset</span><span> boosted </span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span><br/><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-player-starting-resources-preset </span><span class="ͼf">--player</span><span></span><span class="ͼb">1</span><span></span><span class="ͼf">--preset</span><span> handicap </span><span class="ͼf">--gold</span><span></span><span class="ͼb">200</span><span></span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

Supported preset values:

* `vip`
* `boosted`
* `handicap`

Current preset values:

* `vip`
* `gold = 1500`
* `lumber = 300`
* `boosted`
* `gold = 1000`
* `lumber = 200`
* `handicap`
* `gold = 150`
* `lumber = 25`

Behavior:

* unified starting resource blocks remain intact as defaults
* player-specific override blocks are written separately
* explicit numeric flags override preset values
* repeated execution updates existing override blocks instead of duplicating them

---

### Module 5: Player-specific starting hero power

This module extends the hero-selection action functions with per-player hero power override blocks.

#### Player-specific starting hero level

<pre class="overflow-visible! px-0!" data-start="15729" data-end="15861"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-player-starting-hero-level </span><span class="ͼf">--player</span><span></span><span class="ͼb">1</span><span></span><span class="ͼf">--value</span><span></span><span class="ͼb">12</span><span></span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

#### Player-specific starting hero skill points

<pre class="overflow-visible! px-0!" data-start="15912" data-end="16051"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-player-starting-hero-skill-points </span><span class="ͼf">--player</span><span></span><span class="ͼb">1</span><span></span><span class="ͼf">--value</span><span></span><span class="ͼb">11</span><span></span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

#### Player-specific starting hero power preset

<pre class="overflow-visible! px-0!" data-start="16102" data-end="16542"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-player-starting-hero-power-preset </span><span class="ͼf">--player</span><span></span><span class="ͼb">1</span><span></span><span class="ͼf">--preset</span><span> standard </span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span><br/><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-player-starting-hero-power-preset </span><span class="ͼf">--player</span><span></span><span class="ͼb">1</span><span></span><span class="ͼf">--preset</span><span> strong </span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span><br/><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> set-player-starting-hero-power-preset </span><span class="ͼf">--player</span><span></span><span class="ͼb">1</span><span></span><span class="ͼf">--preset</span><span> elite </span><span class="ͼf">--level</span><span></span><span class="ͼb">13</span><span></span><span class="ͼf">--skill-points</span><span></span><span class="ͼb">12</span><span></span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

Supported preset values:

* `standard`
* `strong`
* `elite`

Current preset values:

* `standard`
* `level = 3`
* `skill-points = 1`
* `strong`
* `level = 6`
* `skill-points = 4`
* `elite`
* `level = 10`
* `skill-points = 8`

#### Player-specific hero power formatting

<pre class="overflow-visible! px-0!" data-start="16826" data-end="16939"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> format-player-starting-hero-power </span><span class="ͼf">--script</span><span> ./WC3MapTranslator/test/data/war3map.j</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

Behavior:

* global hero power remains the default
* player-specific override blocks are inserted separately
* repeated execution updates override blocks instead of duplicating them
* ordering stays readable:
* global level
* player level block
* global skill points
* player skill-points block
* select unit

---

## Internal script transformation structure

The project now has a clearer internal scripting layout.

### Shared script transform helpers

A shared helper layer handles common `war3map.j` rewrite tasks such as:

* newline detection
* function-block replacement
* marker-block upsert
* regex escaping
* call-line indent detection
* compact blank-line normalization

### Module-level script helpers

Where repetition is clearly module-specific, helper logic stays at the module level.

Current example:

* `starting-hero-power-utils`
* `starting-hero-power-updaters`

This keeps:

* generic text-rewrite operations in shared helpers
* module-specific block-update logic in module helpers
* command files focused on argument parsing and orchestration

---

## Why these gameplay modules matter

These script modules mark the transition from a raw object-data pipeline into a more practical Warcraft III modding toolbox.

Instead of editing only one field at a time, the toolbox now provides structured entry points for:

* object stat tuning
* preset-based balance adjustments
* kill reward logic
* hero growth logic
* starting resources
* starting hero power
* player-specific opening advantages or handicaps
* player-specific hero power overrides

The project now has a clear shape:

* **object-data round-trip pipeline**
* **object-data toolbox commands**
* **multiple focused map-level gameplay script modules**
* **reusable script transformation layers**

---

## Example validation output

<pre class="overflow-visible! px-0!" data-start="18760" data-end="19782"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute end-1.5 top-1 z-2 md:end-2 md:top-1"></div><div class="relative"><div class="pe-11 pt-3"><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span>Stage 1/4: importing source...</span><br/><span>  Source input directory: G:\WC3 Map Object Editor\samples\probe-input-02</span><br/><span>  Patchwork json output: G:\WC3 Map Object Editor\temp\patchwork-import</span><br/><span>Stage 2/4: exporting patchwork json...</span><br/><span>  Workspace project source: G:\WC3 Map Object Editor\temp\project.json</span><br/><span>  Round-trip export directory: G:\WC3 Map Object Editor\temp\roundtrip-export</span><br/><span>Stage 3/4: exporting war files...</span><br/><span>  Patchwork json input: G:\WC3 Map Object Editor\temp\roundtrip-export</span><br/><span>  War output directory: G:\WC3 Map Object Editor\temp\roundtrip-war</span><br/><span>Stage 4/4: re-importing generated war files...</span><br/><span>  War input directory: G:\WC3 Map Object Editor\temp\roundtrip-war</span><br/><span>  Patchwork json output: G:\WC3 Map Object Editor\temp\patchwork-import</span><br/><span>Final Object Counts:</span><br/><span>Units: 2</span><br/><span>Abilities: 3</span><br/><span>Items: 2</span><br/><span>Validation Complete:</span><br/><span>Final workspace project: G:\WC3 Map Object Editor\temp\project.json</span><br/><span>Round-trip export directory: G:\WC3 Map Object Editor\temp\roundtrip-export</span><br/><span>Round-trip war directory: G:\WC3 Map Object Editor\temp\roundtrip-war</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

This remains the main regression path for the object-data foundation.

---

## Validation status

The project has been validated across multiple sample batches, with each batch targeting a different layer of object-data complexity.

### First-batch sample

Validated:

* baseline unit / ability / item support
* WTS string resolution
* core Patchwork import/export flow

### Second-batch sample

Validated:

* richer unit combat-field coverage
* repeated and multi-level ability field support
* additional item text/reference handling
* exporter symmetry for newly mapped fields
* stable one-command round-trip validation

### Third-batch sample

Validated:

* additional repeated ability field families
* scalar attribute-style ability fields
* additional item text/cost fields
* broader field coverage without changing the core workflow
* continued regression stability under a more targeted boundary sample

---

## Documentation

* `docs/object-field-mapping-status.md`
* `docs/second-batch-validation.md`
* `docs/third-batch-validation.md`
* `docs/roundtrip-workflow.md`

These documents track:

* field mapping status
* second-batch expansion results
* third-batch expansion results
* round-trip workflow details

---

## Known limitations

The project currently focuses on the **object-data layer** plus several first-pass  **script modification modules** , not full map packaging.

Known limitations include:

* some isolated object-data fields are still unmapped, such as `ucs2`
* some semantic model fields are still incomplete or placeholder-based:
* `race`
* additional attack/armor/ruleset semantics beyond the current first-pass mappings
* terminal encoding can make direct JSON viewing in PowerShell look incorrect unless UTF-8 is explicitly used
* the current workflow validates object-data round-trips, not full `.w3x` rebuild workflows
* gameplay script support is still intentionally narrow and based on known trigger patterns
* script modification remains string-based, even though its internal structure is getting cleaner

---

## Tech stack

* TypeScript
* Node.js
* Commander
* Patchwork MapConverter
* JSON-based internal data model
* CLI-first workflow

---

## Project direction

Planned next directions include:

* continue semantic field refinement
* expand gameplay modules carefully
* strengthen regression validation
* test against more complex sample maps
* improve script transformation reuse where repetition is real
* evolve toward a more complete Warcraft III map modding toolbox
