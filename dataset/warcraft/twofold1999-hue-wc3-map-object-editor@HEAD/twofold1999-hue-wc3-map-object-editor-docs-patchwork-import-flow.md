---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "docs/patchwork-import-flow.md"
ref: HEAD
---

# Patchwork Import Flow (Current State)

## 1) High-Level Flow
1. Input: Patchwork output directory containing:
   - `war3map.w3u.json` (units)
   - `war3map.w3a.json` (abilities)
   - `war3map.w3t.json` (items)
2. `PatchworkProjectImporter.importFromDirectory(...)` reads those files.
3. Adapters map Patchwork `custom` data into internal objects:
   - `PatchworkUnitsAdapter`
   - `PatchworkAbilitiesAdapter`
   - `PatchworkItemsAdapter`
4. Import result becomes internal `MapDataFile`.
5. In CLI flows (for example `import-patchwork`):
   - `JsonProjectExporter` writes `temp/project.json`
   - `WorkspaceService` points current project to `temp/project.json`

## 2) Patchwork JSON Schema Summary
- Root shape includes:
  - `original`
  - `custom`
- Current implementation uses `custom` only.
- `custom` keys use composite id format:
  - `newId:baseId`
  - current mapping uses the part before `:` as internal `id`
- Each key maps to an array of modification records, typically with fields like:
  - `id`
  - `type`
  - `level`
  - `column`
  - `value`

## 3) Current Field Mappings
### Units (`war3map.w3u.json`)
- `unam` -> `name`
- `uhpm` -> `hitPoints`

### Abilities (`war3map.w3a.json`)
- `anam` -> `name`
- `amcs` -> `manaCost`
- `acdn` -> `cooldown`
- `aran` -> `castRange`

### Items (`war3map.w3t.json`)
- `unam` -> `name`
- `igol` -> `goldCost`
- `ides` -> `description`
- `ihtp` -> `cooldown`

## 4) String Resolution
- `StringResolverService` is used by Patchwork adapters.
- `TRIGSTR_*` values currently pass through unchanged.
- Future goal: resolve `TRIGSTR_*` via real string-table sources.

## 5) Current Limitations
- `original` section is ignored.
- Many WC3 object fields are not mapped yet.
- No real TRIGSTR table integration yet.
- Unit fields `combatType`, `race`, `attackType`, and `armorType` still default to `"Unknown"`.
