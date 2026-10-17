---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: ".cursor/MIGRATION.md"
ref: HEAD
---

# Agent config migration (2026-06-18)

One-time record of the move from Codex/Claude agent folders to committed `.cursor/` layout.

## Path mapping

| Old | New |
|-----|-----|
| `AGENTS.md` (full ~150 lines) | `AGENTS.md` (slim index) + `.cursor/rules/*.mdc` |
| `.codex/skills/*/SKILL.md` | `.cursor/skills/*/SKILL.md` (with YAML frontmatter) |
| `.codex/plans/map_protection_plan.md` | `.cursor/plans/map_protection_plan.md` |
| (new) | `.cursor/skills/hivewe-delegate-work/SKILL.md` |

## `.claude/` audit

Contents found at migration time:

| File | Action |
|------|--------|
| `.claude/settings.local.json` | **Not migrated** — Claude Code local bash permission allowlist; Cursor uses its own permission model. No unique project guidance. |

No `CLAUDE.md`, skills, or commands were present.

## User-level Cursor skills cleanup

If you previously added global skills pointing at `.codex/skills/...`, remove or update them. Project skills under `.cursor/skills/` are discovered automatically for this repo.

Old paths to remove from user skills (if present):

- `.codex/skills/hivewe-repo-navigation/SKILL.md`
- `.codex/skills/hivewe-build-and-debug/SKILL.md`
- `.codex/skills/hivewe-ui-development/SKILL.md`
- `.codex/skills/hivewe-warcraft3-data-formats/SKILL.md`

## Smoke-test prompts

Use these after migration to confirm context routing:

1. **UI:** "Add a tooltip to the region palette" → expect `hivewe-qt-ui.mdc` in context.
2. **Format:** "MDX round-trip fails on fixture X" → expect `hivewe-file-formats.mdc` + `hivewe-cpp-modules.mdc`.
3. **Orientation:** "Where is map save orchestrated?" → Codegraph or `hivewe-repo-navigation` skill, not wide grep.

## Removed folders

- `.codex/` — deleted after copy
- `.claude/` — deleted (contained only local settings)
