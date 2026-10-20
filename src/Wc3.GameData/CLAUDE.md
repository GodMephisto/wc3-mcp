# Wc3.GameData — Reforged base object data (CASC)

Reads the installed WC3 base data so `object get` can merge base ⊕ map deltas. Refs `CascLib.NET` ONLY (no Model/Commands).

- **Open**: `GameData.TryOpen(gameDirOverride, out GameDataContext ctx, out diag)` — locates the install (registry/`C:\Warcraft III`/paths, or override), opens CASC once, builds all 7 stores + strings. **Cached per install dir** (process lifetime; SLK data assumed stable). Never throws → on failure, callers degrade to deltas-only + a diagnostic.
- **CASC paths** (note the `:` mod-layer syntax): `war3.w3mod:units\unitmetadata.slk`, `...unitdata.slk` (+ unitbalance/unitweapons/unitui/unitabilities), `abilitymetadata.slk`/`abilitydata.slk`, etc. Strings: `war3.w3mod:_locales\enus.w3mod:ui\worldeditstrings.txt`, `...units\<race>unitstrings.txt`.
- **Metadata mapping** (confirmed): field code = metadata **row-key (`ID` col)**; data-SLK column = **`field` col**; data SLK file = **`slk` col** (lowercased + `.slk`). Data SLKs keyed by rawcode in the first column.
- **Object-data modification shapes** (War3Net): unit/item/destructable/buff = `Simple`; ability/upgrade = `Level`; doodad = `Variation`. Leveled/variation fields are keyed `code:N` in merged output. Always read `.Value` (the `ValueAs*` accessors throw on type mismatch).
- **Name field codes**: `unam` (unit & item), `anam`, `bnam`, `dnam`, `fnam`, `gnam`. Base item/ability/buff/upgrade names live in profile `.txt` (unread → rawcode fallback).
- **wts gotchas**: an empty `STRING n { }` parses to `TriggerString.Value == null` (guard `?? ""`); a **truncated** `STRING n {` makes War3Net `ReadTriggerStrings` spin **forever** — never feed it truncated input.
- `GameData` the facade class sits in namespace `Wc3.GameData` → referenced as `GameData.GameData` (known smell; rename candidate).
