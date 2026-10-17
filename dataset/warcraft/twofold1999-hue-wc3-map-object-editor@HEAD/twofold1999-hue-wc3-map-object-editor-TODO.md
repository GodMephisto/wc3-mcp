---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "TODO.md"
ref: HEAD
---

# TODO

This file tracks the current unfinished work and near-term priorities for `wc3-map-object-editor`.

It is intentionally short, practical, and aligned with the current project direction.

---

## Current status

The project is no longer only an object-data round-trip prototype.

It now has four visible layers:

1. **object-data pipeline**
2. **toolbox-style object tuning commands**
3. **multiple focused map-level gameplay script modules**
4. **reusable script transformation layers**

---

## Stable foundation already in place

Completed and already working:

- Patchwork import for units / abilities / items
- WTS string resolution
- unified internal `MapDataFile`
- CLI inspect/edit flow
- Patchwork JSON export
- Patchwork `json2war`
- first-batch validation
- second-batch validation
- third-batch validation
- one-command round-trip validation

---

## Current toolbox status

Implemented object-data toolbox commands:

- `unit buff`
- `item buff`
- `ability tune`

Implemented object-data presets:

- `unit buff --preset boss`
- `unit buff --preset elite`

---

## Current gameplay script modules

### 1. Kill reward / hero growth

Implemented commands:

- `set-kill-gold-multiplier`
- `set-kill-gold-flat-bonus`
- `set-kill-lumber-bonus`
- `set-kill-hero-stat-bonus --stat str|agi|int`
- `set-kill-exp-bonus`
- `set-kill-hero-growth-preset`
- `format-kills-trigger`

Implemented named hero growth presets:

- `warrior`
- `mage`
- `assassin`
- `tank`
- `support`

Current behavior:

- named presets can be applied through `set-kill-hero-growth-preset --preset ...`
- explicit numeric flags override preset values
- zero-valued growth entries are omitted from final output
- final hero growth blocks are normalized into a stable canonical order:
  - `SetHeroStr`
  - `SetHeroAgi`
  - `SetHeroInt`
  - `AddHeroXP`

### 2. Starting resources

Implemented commands:

- `set-starting-gold`
- `set-starting-lumber`
- `set-starting-resources-preset`

Implemented named starting resource presets:

- `standard`
- `rich`
- `scarce`

Current behavior:

- starting gold and lumber are written as compact begin/end marked blocks
- repeated execution updates existing blocks instead of duplicating them
- explicit numeric flags override preset values

### 3. Starting hero power

Implemented commands:

- `set-starting-hero-level`
- `set-starting-hero-skill-points`
- `format-starting-hero-power`
- `set-starting-hero-power-preset`

Implemented named starting hero power presets:

- `standard`
- `strong`
- `elite`

Current behavior:

- the 5 known hero-selection action functions are updated consistently
- explicit numeric flags override preset values
- starting hero level / skill point lines stay compact and adjacent
- repeated execution updates rather than duplicates

### 4. Player-specific starting settings

Implemented commands:

- `set-player-starting-gold`
- `set-player-starting-lumber`
- `set-player-starting-resources-preset`

Implemented named player-specific starting resource presets:

- `vip`
- `boosted`
- `handicap`

Current behavior:

- unified starting resource blocks remain intact as defaults
- player-specific override blocks are written separately
- explicit numeric flags override preset values
- repeated execution updates existing player-specific override blocks instead of duplicating them

### 5. Player-specific starting hero power

Implemented commands:

- `set-player-starting-hero-level`
- `set-player-starting-hero-skill-points`
- `set-player-starting-hero-power-preset`
- `format-player-starting-hero-power`

Implemented named player-specific hero power presets:

- `standard`
- `strong`
- `elite`

Current behavior:

- the same 5 known hero-selection action functions are updated consistently
- global hero power remains the default
- player-specific hero power override blocks are written separately
- explicit numeric flags override preset values
- repeated execution updates existing override blocks instead of duplicating them
- readable ordering is preserved:
  - global level
  - player level block
  - global skill points
  - player skill-points block
  - select unit

This is the point where the project clearly became a multi-module gameplay scripting toolbox with reusable internal transform layers rather than a collection of isolated commands.

---

## Current internal scripting structure

The project now has two clear internal helper layers for script rewriting.

### Shared script transform layer

Current shared helpers cover:

- regex escaping
- newline detection
- call-line indent detection
- function-block replacement
- marker-block upsert
- blank-line compaction

### Module-level helper layer

Current example:

- `starting-hero-power-utils`
- `starting-hero-power-updaters`

This structure keeps:

- generic text transformation in shared helpers
- module-specific block-update logic in module helpers
- command files focused on parsing and orchestration

---

## Current milestone direction

**v0.8 - script transform layering and fifth gameplay module**

The next stage is now split across two parallel directions:

### A. semantic model refinement

Continue improving the quality of the internal object model rather than only adding more raw fields.

Current first-pass semantic refinement already started for:

- `combatType`
- `attackType`
- `armorType`

Likely future candidates:

- `race`
- more attack/armor semantics
- selective raw field cleanup where the semantics are already clear from samples

### B. gameplay module expansion

Continue extending the project from object-data tuning toward practical map modding commands, while keeping the internal script transformation layers disciplined.

Current module families:

- kill reward / hero growth
- starting resources
- starting hero power
- player-specific starting settings
- player-specific starting hero power

---

## High-priority next work

### 1. Keep semantic refinement moving carefully

The model still has placeholder-heavy areas.

High-value next candidates:

- `race`
- more attack/armor semantics
- selective raw field cleanup where the semantics are already clear from samples

Rule:

- stay sample-driven
- avoid speculative enums
- preserve raw imported values where useful

---

### 2. Decide the sixth gameplay script module

The project now has five stable module families.
The next major code step should likely be a **sixth** clearly scoped gameplay module instead of endlessly deepening current ones.

Good candidates:

- player-specific starting items / loadout
- difficulty-aware starting settings
- player-specific kill reward overrides
- another tightly scoped combat or progression trigger module

Do **not** start with a huge generalized scripting framework.

---

### 3. Keep helper extraction disciplined

The project now has real shared helpers and module helpers.
That is useful, but it can become messy if abstraction expands too quickly.

Rules going forward:

- only extract shared helpers when repetition is clearly real
- keep module-level business logic out of shared helpers
- avoid command files importing business logic from each other
- prefer small, explicit helper layers over broad frameworks

---

## Medium-priority work

### 4. Improve script transformation reuse carefully

Now that the helper layers exist, further reuse should be done only where patterns are stable.

Likely candidates:

- additional module-specific updater files
- better shared handling for known insert-anchor strategies
- more consistent formatter patterns across modules

This should be done carefully, without breaking already validated commands.

---

### 5. Keep the toolbox structure coherent

The toolbox now has four clear layers:

#### Object-data tuning

- unit / item / ability tuning
- presets
- object inspection
- exporter / round-trip workflow

#### Gameplay script modules

- kill reward / hero growth
- starting resources
- starting hero power
- player-specific starting settings
- player-specific starting hero power

#### Internal script helpers

- shared script transforms
- module-level updaters / utils

Future work should continue reinforcing these categories instead of becoming a random command list.

---

### 6. Keep README / docs consistency

README and TODO should continue reflecting both external features and internal structure.

Keep aligned:

- `README.md`
- `TODO.md`
- `docs/object-field-mapping-status.md`
- `docs/second-batch-validation.md`
- `docs/third-batch-validation.md`
- `docs/roundtrip-workflow.md`

If the next milestone adds another major gameplay module, it may finally be worth adding a dedicated docs page for gameplay script modules.

---

### 7. Keep exporter/round-trip confidence high

Even though gameplay script commands do not go through the same object-data exporter flow, the object-data foundation is still central.

Continue preserving confidence through:

- repeated sample imports
- object-data round-trip validation
- careful separation between object-data features and script-level features

The project should not lose its validation-first character while adding modding features.

---

## Low-priority / later ideas

### 8. Smarter trigger rewriting

Script modification is still intentionally string-based.

Possible later directions:

- more structured trigger block rewriting
- limited pattern abstraction across commands
- more reusable anchor-resolution helpers
- additional module-level updater files

This is not needed yet if the current focused commands stay stable.

---

### 9. Broader gameplay systems

Later, the project may expand into broader gameplay systems such as:

- more advanced growth systems
- hero loadout systems
- additional reward rules
- broader trigger logic injection
- player-specific power systems
- more conditional/difficulty-aware configuration

These are later-stage directions and should only happen after the current five gameplay modules remain stable.

---

### 10. Full map packaging workflow

The project still does **not** attempt full `.w3x` rebuild or packaging workflows.

Possible later direction:

- reinsert modified script/object files into full map packages
- validate broader end-to-end rebuild behavior

Still out of scope for the current phase.

---

## Current recommended next step

If continuing from the current state, pick **one** of these:

1. continue semantic field refinement
2. add a sixth clearly scoped gameplay script module
3. improve helper reuse only where repetition is already real
4. strengthen docs only when the next large phase lands

Do not try to do all of them in one pass.

---

## Working rule

Keep using the current development loop:

1. inspect the current data / script structure
2. add one narrowly scoped feature
3. verify visible output
4. verify repeatability / update behavior
5. keep formatting and readability under control
6. document major milestones when a feature group becomes real

This rule is one of the main reasons the project has remained stable while growing.
