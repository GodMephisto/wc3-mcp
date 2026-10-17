---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: ".cursor/skills/hivewe-delegate-work/SKILL.md"
ref: HEAD
---

---
name: hivewe-delegate-work
description: >-
  Routes HiveWE tasks between inline work, Codegraph MCP, and Cursor subagents.
  Use when a task spans multiple subsystems or broad exploration would waste context.
disable-model-invocation: true
---

# HiveWE Delegate Work

## Decision order

1. **Codegraph first** for symbol location and call paths: `codegraph_explore` with symbol/file names.
2. **Inline** when ownership is known and scope is narrow.
3. **Subagent** when exploration would require many files across subsystems.

## Inline (parent agent)

- Single-file UI fix in a known palette
- Targeted parser fix with known test file
- Applying pasted compiler errors
- Edits confined to one rule's glob scope

## Delegate `explore`

- "Where does X live?" across Map / Hierarchy / hivewe.cpp
- New feature "where to hook" questions
- Unknown ownership between UI, map state, rendering, formats

Launch **two explore agents in parallel** when UI and format layers are both unknown (e.g. save bug + palette symptom).

## Delegate `generalPurpose`

- Cross-cutting features spanning 3+ folders (new editor window + save path + tests)
- Multi-step refactors with coordinated changes

## Delegate `shell`

- Git status/diff, preset listing, file inventory
- **Not** default `cmake --build` unless user re-allows (see build-policy rule)

## Delegate `bugbot` / `security-review`

- Only when user **explicitly** asks
- `security-review`: MPQ export, external `QProcess`, file I/O on user paths

## Never delegate by default

- Build verification — user pastes errors or explicitly re-enables builds
- Simple one-file edits with clear owner

## After delegation

- Synthesize subagent results; avoid re-running the same searches inline
- Re-check owning subsystem before committing to a patch location
