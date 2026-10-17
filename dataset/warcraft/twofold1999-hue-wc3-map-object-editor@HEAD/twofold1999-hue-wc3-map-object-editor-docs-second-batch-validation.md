---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "docs/second-batch-validation.md"
ref: HEAD
---

<pre class="overflow-visible! px-0!" data-start="125" data-end="1367"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span># second-batch-validation</span><br/><br/><span>This document records the second-batch sample validation results for the Patchwork-based round-trip workflow.</span><br/><br/><span>## Goal</span><br/><br/><span>The second-batch sample was created to test whether the current importer/exporter/model pipeline could generalize beyond the first batch of basic object edits.</span><br/><br/><span>The main goal was not just to “run successfully,” but to expose new field patterns such as:</span><br/><br/><span>- additional unit combat fields</span><br/><span>- repeated and multi-level ability fields</span><br/><span>- item text/reference fields</span><br/><span>- more complex exporter symmetry cases</span><br/><br/><span>---</span><br/><br/><span>## Sample scope</span><br/><br/><span>The second-batch sample introduced new edits across:</span><br/><br/><span>- Units</span><br/><span>- Abilities</span><br/><span>- Items</span><br/><br/><span>Representative test objects included:</span><br/><br/><span>### Units</span><br/><span>- `h000` - 步兵测试</span><br/><span>- `h001` - 矮人火枪手测试</span><br/><br/><span>### Abilities</span><br/><span>- `A000` - 心灵之火测试</span><br/><span>- `A001` - 能提高英雄属性的物测试</span><br/><span>- `A002` - 专注光环测试</span><br/><br/><span>### Items</span><br/><span>- `I000` - 巨人力量腰带+60测试</span><br/><span>- `I001` - 中和权杖测试</span><br/><br/><span>---</span><br/><br/><span>## Validation workflow</span><br/><br/><span>The second-batch sample was validated through the same Patchwork round-trip workflow used by the project:</span><br/><br/><span>```text id="c0xx8r"</span><br/><span>input directory</span><br/><span>-> import-patchwork-dir</span><br/><span>-> inspect raw fields</span><br/><span>-> extend internal model mappings</span><br/><span>-> export-patchwork</span><br/><span>-> export-patchwork-war</span><br/><span>-> re-import generated war files</span><br/><span>-> validate summary/list output</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

A one-command version of this workflow is now available through:

<pre class="overflow-visible! px-0!" data-start="1435" data-end="1522"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> validate-roundtrip ./samples/probe-input-02</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

---

## Main findings

## Units

The second-batch unit sample exposed a much richer combat-related field set than the first batch.

Newly validated unit field patterns included:

* primary / secondary attack ranges
* primary / secondary attack speeds
* minimum attack range
* primary / secondary dice sides
* primary / secondary weapon text fields
* secondary attack type text
* primary target flags
* primary target preferences

Examples of newly mapped Patchwork unit fields:

* `ua1r`
* `ua2r`
* `usr1`
* `usr2`
* `uamn`
* `usd1`
* `usd2`
* `ua1w`
* `ua2w`
* `ua2t`
* `ua1g`
* `ua1p`

At the end of second-batch unit validation, the remaining observed unmapped field was:

* `ucs2`

This means the second-batch unit sample was reduced from a large unknown combat-field cluster to a single isolated pending field.

---

## Abilities

The second-batch ability sample confirmed that the current system can handle additional repeated and multi-level fields beyond the first batch.

Newly validated ability field patterns included:

* repeated mana-cost levels
* repeated area-like values
* repeated `Inf1` values
* continued support for max level

Examples of newly mapped Patchwork ability fields:

* repeated `amcs`
* repeated `aare`
* repeated `Inf1`
* single-value `Inf2`
* single-value `Inf3`

This validated the current strategy of:

* keeping stable scalar fields where useful
* introducing array-based fields for repeated values
* avoiding premature hardcoding of uncertain gameplay semantics

---

## Items

The second-batch item sample validated additional text and reference fields beyond the first batch.

Newly validated item field patterns included:

* tooltip text
* linked ability id
* level variance

Examples of newly mapped Patchwork item fields:

* `utub`
* `iabi`
* `ilvo`

This confirmed that the importer/exporter pipeline can support:

* resolved string fields
* raw object-id references
* additional single-value metadata fields

---

## Export symmetry validation

Second-batch validation did not stop at import/show behavior.

The project also verified that newly mapped second-batch fields were exported back into Patchwork JSON and then converted back into Warcraft III object files.

This included:

### Units

Examples of validated exported fields:

* `ua1r`
* `ua2r`
* `ua1w`
* `ua2w`
* `ua2t`
* `ua1g`
* `ua1p`
* `usr1`
* `usr2`
* `uamn`
* `usd1`
* `usd2`

### Abilities

Examples of validated exported fields:

* repeated `amcs`
* repeated `aare`
* repeated `Inf1`
* `alev`

### Items

Examples of validated exported fields:

* `utub`
* `iabi`
* `ilvo`
* `ipri`
* `istr`
* `ilev`

This confirmed that second-batch field work was not importer-only; exporter symmetry also held for the newly added mappings.

---

## Round-trip result

The second-batch sample completed a full round-trip successfully.

Final validation command:

<pre class="overflow-visible! px-0!" data-start="4377" data-end="4464"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> validate-roundtrip ./samples/probe-input-02</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

Final observed result:

* Units: 2
* Abilities: 3
* Items: 2

Validated final object lists included:

* `h000`, `h001`
* `A000`, `A001`, `A002`
* `I000`, `I001`

This confirms that the current round-trip workflow remains stable under a more complex second-batch sample than the original baseline set.

---

## Current conclusion

The second-batch sample demonstrates that the project is no longer limited to a single narrow input pattern.

It now supports a broader range of:

* unit combat field variants
* repeated ability values
* multi-level ability data
* item text/reference metadata
* importer/exporter symmetry across multiple sample batches

In practical terms, the second-batch validation shows that the current architecture has begun to generalize across different real Patchwork object patterns.

---

## Remaining follow-up items

Known remaining follow-up from second-batch validation:

* unit field `ucs2` is still unmapped
* semantic placeholder fields still exist in units:
* `combatType`
* `race`
* `attackType`
* `armorType`

These do not block the current round-trip workflow, but remain targets for future refinement.
