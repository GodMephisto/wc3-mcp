---
type: vendor-doc
source_repo: "https://github.com/DaniilVanin/pg2-jass-sector-toolkit"
source_path: "README.md"
ref: HEAD
---

# PG2 JASS Sector Toolkit

Small Python tools for working with the main `war3map.j` script inside
Warcraft III maps where the normal MPQ file listing is damaged, hidden, or not
useful because of PG2-style protection.

The toolkit is focused on one practical task:

1. find the compressed JASS sector block inside a `.w3x`;
2. extract the script;
3. normalize unsafe JASS rawcode/FourCC literals;
4. apply script edits;
5. put the script back into the same map block.

It does not rebuild the whole MPQ archive and does not try to recover every
file from the map.

## What Is Included

```text
tools/
  pg2_jass_tool.py             # scan, extract, import main JASS sector block
  normalize_jass_rawcodes.py   # convert FourCC/rawcode and hex literals to integers
  compact_jass_comments.py     # remove full-line comments and blank lines safely
  verify_jass_import.py        # verify that a map contains the expected script
  build_jass_workflow.py       # reusable prepare/build workflow template
```

Optional sample maps can be stored next to the tools:

```text
PG2.w3x   # PG2-style reference/sample map for scanner checks
a.w3x     # small sample map for quick scanner checks
```

They are samples for testing the scanner behavior. They are not required for
normal use.

## Requirements

- Python 3.10 or newer;
- no external Python packages.

Only the Python standard library is used:

```text
argparse
dataclasses
difflib
pathlib
struct
sys
zlib
```

The current workspace was tested with Python `3.12.13` and zlib `1.3.1`.

## How The Scanner Works

Warcraft III maps are MPQ archives. A protected map can keep the actual script
data as ordinary compressed sectors while making the normal MPQ tables hard to
use. `pg2_jass_tool.py` therefore opens the map as a binary file and searches
for MPQ-style compressed sector chains directly.

The scanner looks for:

- zlib streams preceded by MPQ compression marker `0x02`;
- a valid sector offset table before the streams;
- JASS markers inside decompressed samples.

Markers used for scoring include:

```text
function main
function config
InitCustomTriggers
gg_trg_
globals
endglobals
function InitTrig
```

The highest-scoring block is treated as the main `war3map.j` block.

## Basic Commands

Scan a map:

```powershell
python tools\pg2_jass_tool.py scan input.w3x --limit 5
```

Extract the main JASS script:

```powershell
python tools\pg2_jass_tool.py extract input.w3x out\war3map.j
```

Import a changed script back into the map:

```powershell
python tools\pg2_jass_tool.py import input.w3x out\war3map.compact.j output.w3x
```

The import is in-place inside the old sector block. The new script must fit
inside the original script file size after compacting.

Verify that the output map contains the expected script:

```powershell
python tools\verify_jass_import.py output.w3x out\war3map.compact.j
```

## Rawcode And FourCC Normalization

JASS allows object ids to be written as four-byte single-quoted literals:

```jass
'hfoo'
```

This is the same value as an integer:

```jass
1751543663
```

Protected maps may contain raw bytes inside those quotes. Some of those bytes
can look like line breaks or invalid text to editors and formatters. The
normalizer converts only code literals outside strings and comments:

```powershell
python tools\normalize_jass_rawcodes.py out\war3map.j out\war3map.normalized.j
```

Example:

```jass
call X('hfoo', "'hfoo'") // 'hfoo'
```

After normalization:

```jass
call X(1751543663, "'hfoo'") // 'hfoo'
```

The `"..."` string and the `//` comment are left untouched.

Hex literals like `$002f` are also converted when they appear in normal code.

## Safe Compact

`compact_jass_comments.py` removes only:

- blank lines;
- full-line `//` comments.

It does not remove functions, globals, triggers, string contents, inline
comments, or code that only looks unused.

Run it on a generated or copied script:

```powershell
python tools\compact_jass_comments.py out\war3map.normalized.j
```

The compactor is byte-aware. It avoids splitting a JASS rawcode literal just
because one of its four bytes happens to be `\r` or `\n`.

## Manual Workflow

The tools are intentionally small and independent. A typical manual workflow is:

```text
input.w3x
  -> extract war3map.j
  -> normalize rawcodes/FourCC
  -> edit the JASS script
  -> compact comments and blank lines
  -> import script into output.w3x
  -> verify the imported script
```

Example command sequence:

```powershell
python tools\pg2_jass_tool.py scan input.w3x --limit 5
python tools\pg2_jass_tool.py extract input.w3x out\war3map.j
python tools\normalize_jass_rawcodes.py out\war3map.j out\war3map.normalized.j
```

After editing the normalized script, make a compact copy and import it:

```powershell
Copy-Item out\war3map.normalized.j out\war3map.compact.j
python tools\compact_jass_comments.py out\war3map.compact.j
python tools\pg2_jass_tool.py import input.w3x out\war3map.compact.j output.w3x
python tools\verify_jass_import.py output.w3x out\war3map.compact.j
```

## Scripted Workflow Template

For repeatable projects, use `tools\build_jass_workflow.py` as a starting
point. It has two normal stages.

Prepare files for editing:

```powershell
python tools\build_jass_workflow.py prepare --map maps\my_map.w3x --name my_map
```

This writes:

```text
out/my_map.war3map.j
out/my_map.normalized.j
out/my_map.edited.j
```

Then edit `out/my_map.edited.j` manually, or edit the
`apply_custom_changes()` function inside `tools\build_jass_workflow.py` to make
the patch repeatable in Python.

Build the patched map:

```powershell
python tools\build_jass_workflow.py build --map maps\my_map.w3x --name my_map
```

This writes:

```text
out/my_map.patched.j
out/my_map.compact.j
out/my_map.patch.diff
out/my_map.check.j
build/my_map.patched.w3x
```

The build stage compacts the edited script, imports it into a new map, and runs
`verify_jass_import.py` automatically.

## Output Verification

After import, run:

```powershell
python tools\verify_jass_import.py output.w3x out\war3map.compact.j
```

The verifier extracts the main JASS block from `output.w3x`, checks that it
starts with the exact bytes from `out\war3map.compact.j`, and verifies that the
remaining bytes are only padding spaces.

To save the extracted script while verifying:

```powershell
python tools\verify_jass_import.py output.w3x out\war3map.compact.j --dump-extracted out\check-war3map.j
```

## Limits

- The toolkit works with the main JASS sector block, not with every MPQ file.
- Import keeps the existing block layout and cannot grow beyond the original
  script size.
- The scanner is heuristic. It is designed for PG2-style layouts that still use
  compressed MPQ sectors.
- Opening the result in World Editor is not guaranteed. The goal is a playable
  map with a replaced main script.

Project-specific patch code should live in a copied or edited workflow file, not
inside the low-level sector tools.

## License

The MIT license applies to the Python tools and documentation only.
Warcraft III maps, extracted scripts, and game assets are not covered unless explicitly stated.