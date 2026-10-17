---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: ".cursor/skills/hivewe-warcraft3-data-formats/SKILL.md"
ref: HEAD
---

---
name: hivewe-warcraft3-data-formats
description: >-
  Guides Warcraft III map serialization, SLK/INI merges, CASC/MPQ, MDL/MDX, and trigger formats.
  Use for corrupted saves, load failures, round-trip bugs, or war3map/war3mapSkin editing.
disable-model-invocation: true
---

# HiveWE Warcraft III Data Formats

## When to use

`war3map.*`, `war3mapSkin.*`, SLK/INI merges, CASC/MPQ, MDL/MDX, generated map script output, version-specific format bugs.

## Owner map

| Concern | Location |
|---------|----------|
| Load/save orchestration | `src/base/map/map.ixx` |
| Asset lookup order | `src/base/hierarchy.ixx` |
| Binary helpers | `BinaryReader`, `BinaryWriter` |
| MDL/MDX | `src/file_formats/mdx/` |
| Triggers/script | `src/base/triggers/` |
| Object SLK merges | `src/object_editor/slk_conversions.ixx` |

Fixtures: `data/test map/`, `data/warcraft/`. Tests: `tests/binary_writer_test.cpp`, `tests/modification_tables_test.cpp`, `tests/mdl_reader_test.cpp`.

## Workflow

1. Read **both** load and save paths before editing one side.
2. Preserve version handling and round-trip behavior.
3. Smallest fixture/map artifact that reproduces the issue.
4. Minimal compatibility fix; mark uncertain assumptions `Needs verification`.
5. Add/update tests for deterministic parser/serializer changes.

## Verification checklist

- [ ] Relevant doctest coverage passes
- [ ] MDL/MDX round-trip tests pass
- [ ] `data/test map/` load + save smoke test
- [ ] Load/save order changes: verify open existing map AND save back out

## Common mistakes

- Save-only changes without checking load path.
- Breaking `Map::load()` sequencing (SLK, overrides, terrain, objects, triggers, imports).
- "Cleanup" refactors while fixing compatibility bugs.
