---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/content/docs/faq.mdx"
ref: HEAD
---

# FAQ

---
title: FAQ
description: Common questions about wc3-forge, the MCP bridge, and supported maps.
---

<Accordions type="single">
  <Accordion title="Is wc3-forge free and open source?">
    Yes. wc3-forge is licensed GPL-3.0-or-later. It's an independent Go
    reimplementation that draws heavily on [HiveWE](https://github.com/stijnherfst/HiveWE)
    for WC3 file-format details and rendering logic — see `CREDITS.md` in the repo.
  </Accordion>

  <Accordion title="Do I need Warcraft III installed?">
    For full asset rendering, yes — wc3-forge mounts the CASC storage from your
    WC3 install. Folder-extracted maps still open without CASC. Point
    `WC3FORGE_WC3_PATH` at a non-default install location if needed.
  </Accordion>

  <Accordion title="Does it work with old JASS maps?">
    Yes. Older JASS maps open, edit, save, and Test Map directly — no conversion
    required. There's also a Convert-Map-to-Lua flow when you want it.
  </Accordion>

  <Accordion title="Can it save packaged .w3x files?">
    Yes. wc3-forge saves in place, including packaged `.w3x` / MPQ archives, and
    preserves unlisted imports via a lossless raw block copy-through.
  </Accordion>

  <Accordion title="Do I need Node.js for the MCP server?">
    No. The MCP server is embedded in the wc3-forge binary — run it with `--mcp`.
    A legacy standalone Node server still lives in the repo's `mcp/` folder, but
    it isn't required.
  </Accordion>

  <Accordion title="Can I run more than one editor at once?">
    Yes — multi-instance is a primary use case. Each instance picks an unused port
    and writes a per-pid lockfile. Use `sessions_list` / `session_select` to point
    Claude at a specific one, and `window_set_title` to label them.
  </Accordion>
</Accordions>
