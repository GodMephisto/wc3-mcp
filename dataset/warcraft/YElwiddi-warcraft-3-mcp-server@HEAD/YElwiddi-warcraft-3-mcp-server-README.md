---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "README.md"
ref: HEAD
---

# Warcraft 3 MCP Server

**Edit your Warcraft III maps by just talking to Claude.**

This is an MCP server that connects Claude to your Warcraft III
`.w3x` / `.w3m` map files. Once it's set up, you can say things like:

> *"Add a footman for Player 1 in the middle of the map."*
> *"Double the hit points of every grunt."*
> *"Add a trigger that spawns 5 wolves every 30 seconds, then open the map in the editor."*
> *"Create a GUI trigger that makes footmen kill grunts on their first attack."*

…and Claude makes the change directly in your map file.

> **How it works in one line:** Warcraft's World Editor has no plugin/automation system, so
> this edits the map *file* directly. Your change is really in the map — open it in the World
> Editor or play it and it's there.

---

## Install (5 minutes)

### Step 1 — Install Node.js
Download and install **Node.js 18 or newer** from [nodejs.org](https://nodejs.org).
(To check if you already have it, open a terminal and run `node --version`.)

### Step 2 — Download this project and build it
Open a terminal (Command Prompt, PowerShell, or Terminal) and run:

```bash
git clone https://github.com/YElwiddi/warcraft-3-mcp-server.git
cd warcraft-3-mcp-server
npm install
npm run build
```

That's it — the server is now built into the `dist` folder.

### Step 3 — Connect it to Claude

Pick whichever you use:

**Claude Code (the CLI)** — run this one command (use the full path to the folder you just
built):

```bash
claude mcp add wc3 --scope user -- node "/full/path/to/warcraft-3-mcp-server/dist/index.js"
```

**Claude Desktop (the app)** — open **Settings → Developer → Edit Config**, and add a `wc3`
entry under `mcpServers` (keep any servers you already have):

```jsonc
{
  "mcpServers": {
    "wc3": {
      "command": "node",
      "args": ["C:/full/path/to/warcraft-3-mcp-server/dist/index.js"]
    }
  }
}
```

### Step 4 — Restart Claude
Close and reopen Claude Code / Claude Desktop so it loads the new tools. You're done!

> 💡 **Tip:** Use forward slashes `/` in the path even on Windows, e.g.
> `C:/Users/You/warcraft-3-mcp-server/dist/index.js`.

---

## Try it

Just ask Claude in plain English and give it the **full path to your map**. For example:

> *"Use wc3 to give me an overview of `C:\Users\Me\Desktop\MyMap.w3x`."*

By default, edits are saved to a copy next to your map (`MyMap.edited.w3x`) so your original is
safe. Tell Claude *"overwrite the original"* if you want it to edit the file in place.

---

## What Claude can do (the tools)

### Look at a map
| Tool | What it does |
|---|---|
| `wc3_overview` | A quick summary of the whole map (great starting point) |
| `wc3_map_info` | Map name, author, players, size |
| `wc3_list_files` / `wc3_read_file` | List or read any file inside the map |
| `wc3_read_objects` | Read units / items / abilities / etc. and their stats |
| `wc3_read_placements` / `wc3_list_units` / `wc3_list_doodads` | What's placed on the map |
| `wc3_read_script` | The map's script (JASS / Lua) |
| `wc3_read_strings` | Text strings (names, tooltips, quest text) |
| `wc3_read_regions` / `wc3_read_cameras` / `wc3_read_sounds` | Regions, cameras, sounds |
| `wc3_read_terrain` | Terrain summary (size, tiles, height) |
| `wc3_read_triggers` / `wc3_read_imports` | GUI triggers in full (events, conditions, actions) and imported files |

### Change a map
| Tool | What it does |
|---|---|
| `wc3_place_unit` / `wc3_modify_unit` / `wc3_delete_unit` | Add, move, or remove units |
| `wc3_place_doodad` / `wc3_modify_doodad` / `wc3_delete_doodad` | Add, move, or remove doodads/trees/etc. |
| `wc3_add_region` / `wc3_modify_region` / `wc3_delete_region` | Manage regions |
| `wc3_add_camera` / `wc3_delete_camera` | Manage cameras |
| `wc3_write_objects` | Change unit/item/ability stats |
| `wc3_set_string` | Change text strings |
| `wc3_write_script` / `wc3_replace_file` | Replace the script or any internal file |
| `wc3_add_gui_trigger` / `wc3_modify_gui_trigger` | Create or edit real GUI triggers (visible in the Trigger Editor, survive saves) |

### Create new things
| Tool | What it does |
|---|---|
| `wc3_create_object` | Make a custom unit/item/ability based on an existing one |
| `wc3_set_durable_script` | Add game logic that runs immediately, **survives editor re-saves**, and shows up editable in the Trigger Editor |

### See your change
| Tool | What it does |
|---|---|
| `wc3_open_in_editor` | Open the map in the Warcraft III World Editor |
| `wc3_test_map` | Launch the map in Warcraft III to play it |

---

## GUI triggers (new in 1.1)

Claude can read and edit the **real GUI trigger tree** — the same triggers you see in the
Trigger Editor (F4).

> *"Change that trigger's ability from War Drums to Bash."*
> *"Create a GUI trigger that makes footmen kill grunts on their first attack."*

Ask for a trigger…

![Asking Claude to add a GUI trigger](docs/usage-claude.png)

…and it's a real trigger, right there in the Trigger Editor:

![The created trigger shown in the World Editor's Trigger Editor](docs/usage-trigger-editor.png)

What to know:

- Changes are written to the editor's own trigger data (`war3map.wtg`), so they **show up in
  the Trigger Editor and survive your saves**.
- **Safe by design:** before any write, the server proves it can rebuild your map's triggers
  byte-for-byte. If it can't, it refuses to touch them.
- After a GUI change, **open the map and Save once** — that's when the editor compiles
  triggers into the playable script. Then playtest.
- As with all edits: close the map in the editor first, reopen after.

---

## Good to know

- **Your originals are safe** — edits save to a `.edited` copy unless you ask to overwrite.
- **Triggers / new logic:** `wc3_set_durable_script` stores your JASS/Lua in `war3map.wct` (the
  editor's own trigger storage) *and* mirrors it into the map script — so it runs immediately
  **and** survives editor re-saves, appearing in the Trigger Editor where you can edit it later.
  For an auto-running trigger, create one custom-text trigger in the editor first (e.g. name it
  `MCPDurable`) and save once; the header slot (`slot:"header"`) needs no setup.
- **Very large or protected maps** may have a few parts that can't be read; Claude will tell you
  which part and keep working on the rest.
- This is **not affiliated with Blizzard Entertainment**. Warcraft III is a trademark of
  Blizzard. This tool only reads/writes your own map files.

---

## License

[MIT](LICENSE) — free and open source. Do whatever you like with it.
