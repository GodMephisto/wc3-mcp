---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/content/docs/building.mdx"
ref: HEAD
---

# Building from source

---
title: Building from source
description: Build the wc3-forge desktop app with Wails.
---

`wails build` is the canonical build. (`go build` alone produces a runnable binary
but skips the embedded frontend and the postbuild library copy, leaving you with
something that loads neither the UI nor CASC.)

```bash
wails build      # production build
wails dev        # dev build with Vite HMR
```

Output paths differ per OS:

- **Windows:** `build/bin/wc3-forge.exe`
- **macOS:** `build/bin/wc3-forge.app/Contents/MacOS/wc3-forge`

The `postBuildHooks` entry in `wails.json` copies the vendored CASC libraries next
to the binary so it's self-contained.

## Tests & checks

```bash
go vet ./...
go test ./...
cd frontend && npm ci          # first-time + after package.json changes
cd frontend && npm run check   # svelte-check (typecheck)
cd frontend && npm run build   # vite build (also part of `wails build`)
```

## Architecture in one breath

A single Wails executable. Go owns map parsing/I/O and the MCP bridge; TypeScript
+ Svelte owns the 3D viewport and panels. Two control surfaces (the GUI and
external MCP clients) converge on the same `forge.Session` singleton, so a
JSON-RPC `units.move` and a viewport drag end up in the same mutator and emit the
same events.

For the full architecture, file-format notes, and the MCP wire contract, see the
[repository README](https://github.com/StephenSHorton/wc3-forge#readme) and
`CLAUDE.md`.
