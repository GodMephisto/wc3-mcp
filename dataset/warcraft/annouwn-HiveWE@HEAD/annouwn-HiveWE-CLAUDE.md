---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "CLAUDE.md"
ref: HEAD
---

# HiveWE — Claude Code Guidance

HiveWE is a native C++23 Warcraft III World Editor replacement (`.ixx` modules + Qt). Preserve WC3 map/editor compatibility — this is not a generic game engine.

All detailed guidance lives in `.cursor/` (rules, skills, plans), with `AGENTS.md` as the Cursor/Codex-facing index. This file is the same layering adapted for Claude Code, whose loading mechanics differ from Cursor's:

- Cursor auto-loads `alwaysApply: true` rules and glob-matched scoped rules. Claude Code only auto-loads this file. So the always-on rules are **inlined below**, and scoped rules are loaded by explicitly reading the `.mdc` file when you touch matching paths.
- Cursor discovers `.cursor/skills/` automatically. Claude Code's Skill tool only discovers `.claude/skills/`. This repo has `core.symlinks=false`, so a symlinked or duplicated `.claude/skills/` would drift from `.cursor/skills/` (the deleted-then-reinstated `.claude/` in `.cursor/MIGRATION.md` is exactly that trap). Instead, **read the `.cursor/skills/*/SKILL.md` file directly with the Read tool** when a task matches — treat it as a playbook, not a registered Skill.
- Cursor subagent names (`explore`, `generalPurpose`, `shell`, `bugbot`, `security-review`) don't exist in Claude Code. Use the Agent tool with `subagent_type: "Explore"` or `"general-purpose"`; use the `security-review` Skill when explicitly asked; run shell commands directly instead of delegating.

`.cursor/` remains the single source of truth. If you learn a durable repo fact, update `.cursor/rules/` or `.cursor/skills/`, not this file — except the inlined always-on excerpt below, which should be kept in sync if `hivewe-core.mdc` or `hivewe-build-policy.mdc` change.

## Always-on rules

### Architecture boundaries

- File formats: `src/file_formats/` or owning `src/base/` component — not UI classes.
- Qt/editors: `src/main_window/`, `src/menus/`, `src/object_editor/`, `src/trigger_editor/`, `src/model_editor/`.
- Rendering/resources: `src/resources/`, `src/base/render_manager.ixx`, `src/main_window/glwidget.cpp`.
- `Map` (`src/base/map/map.ixx`) orchestrates load/save order, undo, rendering wiring.
- `Hierarchy` owns asset lookup order (overrides → local → map → CASC). Do not bypass.
- `ResourceManager` caches shared resources. Do not duplicate loading ad hoc.
- `GLThreadPool` and post-load `glFinish()` are intentional sync points.

### Safe changes

- Small edits in the owning subsystem. Preserve `Map` load/save order unless verified.
- Reuse Qt models, docks, palettes — no parallel UI state.
- Verify Qt parent ownership before changing lifetimes.
- Verify main-thread GL assumptions when touching async loading.

### Workflows

**Bug fix:** Identify layer (UI / map / render / format) → read owner first → narrowest test → broader verification.

**Feature:** Extend existing subsystem → match undo/save flows → identify WC3 files → add tests for parsers.

### Pitfalls

- Do not break `Map` load/save sequencing or `Hierarchy` lookup order.
- Do not remove `glFinish()` / GL thread-pool sync without proof.
- No broad abstractions for one-off editor behavior.
- Regions/player starts/palettes are recent — check viewport AND save/script paths.
- Watch Qt ownership, `QProcess`, GL resources on map reload.

### Build policy (Windows-first: VS 2022 17.14+, `VCPKG_ROOT`, overlay ports in `overlay-ports/`)

- **Do not run `cmake --build ...` or any build/test command** unless the user explicitly re-allows it for the current task. Prefer fixing compile issues from **pasted build errors** first.
- When allowed, use presets, not ad hoc flags: `cmake --preset Release`, `cmake --build --preset Release`, `ctest --preset Release`. Presets: `Debug`, `Release`, `ReleaseTracing` (Tracy profiling).
- Custom vcpkg overlay ports are intentional patches — inspect before changing source. CI reference: `.github/workflows/compile.yml`.
- VS open-folder builds may need Administrator (post-build `data/` symlink). `LNK1104` on `HiveWE.exe` is often a running editor holding the file lock.

### Verification

- Parser changes: `ctest --preset Release`; MDL/MDX round-trip tests.
- UI: open affected editor, check shortcuts/docking/persistence.
- Map save: `data/test map/` load/save smoke test.
- Say explicitly if build/tests were not run.

## Scoped rules — read before editing matching files

| When touching | Read | Topic |
|---|---|---|
| `**/*.ixx` | [`.cursor/rules/hivewe-cpp-modules.mdc`](.cursor/rules/hivewe-cpp-modules.mdc) | Module style, naming, globals, error handling by layer |
| `src/{main_window,menus,object_editor,trigger_editor,model_editor,custom_widgets}/**/*.{cpp,h,ui}` | [`.cursor/rules/hivewe-qt-ui.mdc`](.cursor/rules/hivewe-qt-ui.mdc) | Qt docking, palettes, models, shortcuts |
| `src/{file_formats,base/map,base/triggers}/**/*` | [`.cursor/rules/hivewe-file-formats.mdc`](.cursor/rules/hivewe-file-formats.mdc) | WC3 binary format compatibility, round-trips |
| `src/resources/**`, `src/main_window/**`, `data/shaders/**` | [`.cursor/rules/hivewe-rendering.mdc`](.cursor/rules/hivewe-rendering.mdc) | OpenGL, GPU resources, shaders, debug overlay |

## Skills — read when a task matches

Read the file directly (Read tool); these are playbooks, not registered Claude Code Skills (see mechanics note above).

| Skill | Read when |
|---|---|
| [`hivewe-repo-navigation`](.cursor/skills/hivewe-repo-navigation/SKILL.md) | Finding where code lives, tracing call paths, deciding which subsystem owns a change |
| [`hivewe-build-and-debug`](.cursor/skills/hivewe-build-and-debug/SKILL.md) | Diagnosing compile/link errors, CMake presets, vcpkg, Tracy profiling |
| [`hivewe-ui-development`](.cursor/skills/hivewe-ui-development/SKILL.md) | Editing palettes, docking, `.ui` files, Qt item models |
| [`hivewe-warcraft3-data-formats`](.cursor/skills/hivewe-warcraft3-data-formats/SKILL.md) | Corrupted saves, load failures, round-trip bugs, war3map/war3mapSkin editing |
| [`hivewe-delegate-work`](.cursor/skills/hivewe-delegate-work/SKILL.md) | Deciding Codegraph vs. inline vs. Agent-tool delegation for a task |

## Subagent routing (Claude Code terms)

- **Inline:** single-file fixes, pasted compiler errors, known subsystem owner.
- **Codegraph `codegraph_explore`:** symbol/call-path questions — prefer before wide search or delegating.
- **`Agent(subagent_type: "Explore")`:** unknown ownership across Map / Hierarchy / hivewe.cpp; launch two in parallel if both UI and format layers are unknown.
- **`Agent(subagent_type: "general-purpose")`:** cross-cutting features spanning 3+ folders, multi-step coordinated refactors.
- **Shell (git status/diff, preset listing, file inventory):** run directly — no delegation needed.
- **`security-review` Skill:** only when the user explicitly asks (MPQ export, external `QProcess`, file I/O on user paths).
- **Do not auto-build** — see build policy above.

Full rationale: [`hivewe-delegate-work`](.cursor/skills/hivewe-delegate-work/SKILL.md).

## Plans

Reference [`.cursor/plans/`](.cursor/plans/) for planned work (e.g. `map_protection_plan.md`). `.codex/plans/` and `.codex/skills/` are empty legacy stubs from a prior tool migration — no content lives there.
