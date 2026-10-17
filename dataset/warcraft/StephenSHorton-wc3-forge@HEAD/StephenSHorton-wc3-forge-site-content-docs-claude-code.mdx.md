---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/content/docs/claude-code.mdx"
ref: HEAD
---

# Set up Claude Code

---
title: Set up Claude Code
description: Register wc3-forge's embedded MCP server and start editing maps with an agent.
---

The MCP server is **built into the wc3-forge binary**. Passing `--mcp` runs it as
an MCP stdio server — a thin proxy to a running editor instance. No Node, no extra
install: you point Claude Code at the same executable you launch.

## 1. Register the MCP server

<Tabs items={["Windows", "macOS"]}>
  <Tab value="Windows">

```powershell
# Installed binary:
claude mcp add wc3-forge --scope user -- "C:\Program Files\wc3-forge\wc3-forge.exe" --mcp

# ...or a local build:
claude mcp add wc3-forge --scope user -- "$PWD\build\bin\wc3-forge.exe" --mcp
```

  </Tab>
  <Tab value="macOS">

There is no prebuilt macOS binary — point Claude Code at the app you
[built from source](/docs/installation#build-from-source) (and in-app updates
aren't available on macOS):

```bash
claude mcp add wc3-forge --scope user -- "$PWD/build/bin/wc3-forge.app/Contents/MacOS/wc3-forge" --mcp
```

  </Tab>
</Tabs>

<Callout type="info">
  `--scope` must come **before** `--`; everything after `--` is the command Claude
  launches. Verify with `claude mcp list` — `wc3-forge` should show ✓ Connected.
</Callout>

## 2. Launch and talk to it

Open the editor, load a map (**File → Open Map…**), then talk to it in Claude Code:

```text
What's the current map?
> map_status → "Fountain of Manipulation", 184 units, 2031 doodads

Move the gold mine at position 0,0 to where the player 1 start location is.
> units_list → finds creation_number 17 (typeid 'ngol' at -512, 384, 0)
> camera_set_view → pans to confirm
> units_move → applied

Save it.
> map_save → ok
```

The **Agent Console** (`Ctrl+` `` ` `` in the editor) streams every bridge call
live — names, durations, params, and results. Use it to watch what an agent is
doing in real time.

## Running several editors at once

Multi-instance is a primary use case. Each wc3-forge picks an unused port and
writes its own per-pid lockfile at `~/.wc3-forge/mcp/<pid>.lock`.

- `sessions_list` enumerates running instances.
- `session_select` pins subsequent tool calls to one.
- `window_set_title` labels an instance so parallel agents can tell them apart.

See the full [MCP tool reference](/docs/tool-reference) for everything Claude can do.
