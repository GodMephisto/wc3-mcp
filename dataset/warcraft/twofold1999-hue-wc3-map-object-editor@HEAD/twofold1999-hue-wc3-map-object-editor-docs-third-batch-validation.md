---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "docs/third-batch-validation.md"
ref: HEAD
---

<pre class="overflow-visible! px-0!" data-start="135" data-end="1646"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span># third-batch-validation</span><br/><br/><span>This document records the third-batch sample validation results for the Patchwork-based object-data workflow in `wc3-map-object-editor`.</span><br/><br/><span>The third batch focused less on “basic field coverage” and more on whether the current importer/exporter/model design could continue absorbing new field structures without breaking the existing workflow.</span><br/><br/><span>---</span><br/><br/><span>## Goal</span><br/><br/><span>The third-batch sample was designed to probe the next layer of structure beyond the first two validation rounds.</span><br/><br/><span>Its main goals were:</span><br/><br/><span>- validate broader field coverage</span><br/><span>- test whether new repeated ability field families could be absorbed cleanly</span><br/><span>- test whether item text/numeric extensions still fit the current model</span><br/><span>- confirm that the current importer/exporter pattern still generalizes</span><br/><span>- continue using the existing round-trip workflow rather than introducing a separate validation path</span><br/><br/><span>---</span><br/><br/><span>## Sample scope</span><br/><br/><span>The third-batch sample intentionally stayed small but high-signal.</span><br/><br/><span>Representative test objects included:</span><br/><br/><span>### Units</span><br/><span>- `h000` - 步兵测试03</span><br/><br/><span>### Abilities</span><br/><span>- `A000` - 群星坠落测试03</span><br/><span>- `A001` - 能提高英雄属性的物品测试03</span><br/><br/><span>### Items</span><br/><span>- `I000` - 国王之冠测试03</span><br/><br/><span>This batch was designed as a boundary sample rather than a broad sample.</span><br/><br/><span>---</span><br/><br/><span>## Validation workflow</span><br/><br/><span>The third-batch sample followed the same project workflow:</span><br/><br/><span>```text id="mim1ys"</span><br/><span>input directory</span><br/><span>-> import-patchwork-dir</span><br/><span>-> inspect raw fields</span><br/><span>-> extend internal mappings</span><br/><span>-> validate show output</span><br/><span>-> export-patchwork</span><br/><span>-> export-patchwork-war</span><br/><span>-> re-import validation</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

The same one-command validation flow remains applicable:

<pre class="overflow-visible! px-0!" data-start="1706" data-end="1793"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute inset-x-4 top-12 bottom-4"><div class="pointer-events-none sticky z-40 shrink-0 z-1!"><div class="sticky bg-token-border-light"></div></div></div><div class="relative"><div class=""><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span class="ͼd">npm</span><span> run dev:once </span><span class="ͼf">--</span><span> validate-roundtrip ./samples/probe-input-03</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

---

## Main findings

## Abilities

The third-batch ability sample exposed a stronger repeated-field pattern than the first two batches.

### A000 - repeated field expansion

`A000` introduced additional repeated Patchwork ability fields, including:

* repeated `acdn`
* repeated `adur`
* repeated `ahdu`
* single `amsp`

These were mapped into the internal model as:

* `cooldownLevels`
* `castDurationLevels`
* `heroDurationLevels`
* `missileSpeed`

This was important because it showed that the ability model could continue expanding its repeated-field support without requiring a redesign.

The batch also confirmed continued support for:

* repeated `amcs`
* repeated `aare`

Together, this means the project now supports multiple repeated numeric field families within the same ability object.

### A001 - attribute-style ability fields

`A001` exposed a different ability pattern:

* `Istr`
* `Iagi`
* `Iint`

These were mapped into the internal model as:

* `bonusStrength`
* `bonusAgility`
* `bonusIntelligence`

This confirmed that the project can support both:

* repeated/multi-level ability structures
* attribute-style scalar ability fields

within the same overall ability pipeline.

---

## Items

The third-batch item sample introduced two additional item fields:

* `utip`
* `ilum`

These were mapped into the internal model as:

* `properNameText`
* `lumberCost`

This confirmed that the item pipeline could continue expanding into:

* additional string-table-backed text fields
* additional numeric cost fields

without disrupting the earlier item mapping work.

---

## Units

The third-batch sample included:

* `h000` - 步兵测试03

However, the main value of the third batch was concentrated in abilities and items rather than units.

The unit side in this batch did not become the primary expansion target because the earlier batches had already covered most of the currently active unit field-discovery work.

This means the third batch should be interpreted mainly as:

* ability structure expansion validation
* item extension validation

rather than a major unit-mapping milestone.

---

## Export symmetry validation

The third-batch work was not limited to import/show behavior.

The project continued validating that newly mapped fields could also be exported back into Patchwork JSON in a symmetric way.

This included validation of newly added third-batch fields such as:

### Abilities

* repeated `acdn`
* repeated `adur`
* repeated `ahdu`
* `amsp`
* `Istr`
* `Iagi`
* `Iint`

### Items

* `utip`
* `ilum`

This is important because it confirms that third-batch mapping work strengthened the  **round-trip pipeline** , not just the importer.

---

## Round-trip significance

The third batch further strengthened the confidence that the current architecture is not limited to a small hand-tuned sample set.

By the end of this batch, the project had demonstrated:

* support for repeated numeric ability fields across multiple field families
* support for scalar attribute-style ability fields
* support for additional item text and cost fields
* continued importer/exporter symmetry under broader sample conditions

In practical terms, the third batch increased confidence that the project can keep growing through the current workflow:

1. inspect raw Patchwork data
2. map a small batch of fields
3. validate `show`
4. validate exporter symmetry
5. validate round-trip behavior

---

## Current conclusion

The third-batch validation demonstrates that the project is continuing to generalize beyond the field patterns seen in the first two batches.

Most importantly, it shows that the current model and Patchwork workflow can continue absorbing:

* new repeated ability field families
* new scalar ability attribute fields
* new item text/cost fields

without requiring a workflow reset.

This makes the third batch a meaningful progression milestone rather than just an additional sample.

---

## Remaining follow-up items

Known follow-up items still include:

* isolated remaining unmapped fields from earlier work, such as `ucs2`
* semantic placeholder fields in units:
* `combatType`
* `race`
* `attackType`
* `armorType`

These are still valid future refinement targets, but they do not block the current object-data round-trip workflow.

---

## Suggested milestone interpretation

The third-batch results support moving the project from:

<pre class="overflow-visible! px-0!" data-start="6181" data-end="6245"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute end-1.5 top-1 z-2 md:end-2 md:top-1"></div><div class="relative"><div class="pe-11 pt-3"><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span>v0.3 - Patchwork round-trip core working</span></div></div></div></div></div></div></div></div></div></div><div class=""><div class=""></div></div></div></div></div></pre>

toward a broader milestone such as:

<pre class="overflow-visible! px-0!" data-start="6284" data-end="6362"><div class="relative w-full mt-4 mb-1"><div class=""><div class="relative"><div class="h-full min-h-0 min-w-0"><div class="h-full min-h-0 min-w-0"><div class="border border-token-border-light border-radius-3xl corner-superellipse/1.1 rounded-3xl"><div class="h-full w-full border-radius-3xl bg-token-bg-elevated-secondary corner-superellipse/1.1 overflow-clip rounded-3xl lxnfua_clipPathFallback"><div class="pointer-events-none absolute end-1.5 top-1 z-2 md:end-2 md:top-1"></div><div class="relative"><div class="pe-11 pt-3"><div class="relative z-0 flex max-w-full"><div id="code-block-viewer" dir="ltr" class="q9tKkq_viewer cm-editor z-10 light:cm-light dark:cm-light flex h-full w-full flex-col items-stretch ͼ5 ͼj"><div class="cm-scroller"><div class="cm-content q9tKkq_readonly"><span>v0.4 - broader field coverage and regression stability</span></div></div></div></div></div></div></div></div></div></div></div></div></div></pre>
