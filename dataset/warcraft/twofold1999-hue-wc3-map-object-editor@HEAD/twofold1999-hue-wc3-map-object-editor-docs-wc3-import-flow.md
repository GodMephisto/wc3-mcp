---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "docs/wc3-import-flow.md"
ref: HEAD
---

# WC3 Import Flow (Current State)

## 1) High-Level Architecture
- CLI layer: Commander-based commands in `src/cli/commands/*`.
- Services: domain logic for units/abilities/items plus import/export and change history.
- Storage: `MapDataStorageService` reads/writes the `MapDataFile` envelope from the current workspace file.
- Workspace: `WorkspaceService` tracks current project path in `temp/current-project.json`.
- Project importer/exporter abstraction:
  - `ProjectImporter` / `ProjectExporter` interfaces (`src/services/project-io.ts`)
  - JSON implementations (`JsonProjectImporter`, `JsonProjectExporter`)
- WC3 adapter layer:
  - `Wc3ProjectImporter` (translator-based adapter skeleton)
  - `Wc3ProjectExporter` (placeholder; not implemented yet)

## 2) Current Development Import Flow
1. Run CLI command: `import-fake-map <mapPath>`
2. Command calls `Wc3ProjectImporter.importFromFile(sourcePath)`
3. `importFromFile` calls `runTranslator(sourcePath)`
4. `runTranslator` invokes configured translator command (currently fake script)
5. Fake translator writes translator-style JSON to `temp/translator-output.json`
6. Importer reads translator JSON and maps via `mapTranslatorDataToMapData(...)`
7. Command exports mapped `MapDataFile` to `temp/project.json`
8. Workspace is switched to `temp/project.json`

Related dev command:
- `import-translator-sample <jsonPath>` bypasses translator execution and maps translator JSON directly.

## 3) Translator Config Contract
Config file: `config/translator.json`

Shape:
```json
{
  "command": "",
  "args": []
}
```

Rules:
- `command`: string (required)
- `args`: array of strings (required)
- If `command` is empty/blank, importer throws:
  - `WC3 translator command is not configured.`

Invocation contract:
```text
<command> ...args <sourcePath> <outputJsonPath>
```

Current fixed output path:
- `temp/translator-output.json`

## 4) Translator Output Schema (Expected)
- `map.name`
- `objects.units`
- `objects.abilities`
- `objects.items`

Important mappings:
- Unit: `rawcode -> id`
- Unit: `hp -> hitPoints`
- Ability: `rawcode -> id`
- Item: `rawcode -> id`

Missing sections are treated as empty arrays where reasonable.

## 5) Internal Target Format
Mapped output is converted into `MapDataFile` envelope:

```ts
{
  meta: { source, version, mapName? },
  units: UnitDetails[],
  abilities?: AbilityDetails[],
  items?: ItemDetails[]
}
```

Primary workspace project file:
- `temp/project.json`

## 6) Current Limitations
- Translator execution is wired, but development currently uses a fake translator script.
- Real Warcraft III `.w3x` / `.w3m` binary parsing is not implemented yet.
- `Wc3ProjectExporter` is still a placeholder (`not implemented yet`).
