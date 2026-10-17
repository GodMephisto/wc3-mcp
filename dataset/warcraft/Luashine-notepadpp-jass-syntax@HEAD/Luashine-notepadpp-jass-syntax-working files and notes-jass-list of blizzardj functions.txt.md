---
type: vendor-doc
source_repo: "https://github.com/Luashine/notepadpp-jass-syntax"
source_path: "working files and notes/jass-list of blizzardj functions.txt"
ref: HEAD
---

# Command to extract functions defined in blizzard.j:

# Simple version:
grep 'function' blizzard.j | grep 'takes' | cut -d' ' -f2

# Future-proof version:
grep 'function' blizzard.j | grep 'takes' | sed -e 's/[[:space:]]\{1,\}takes.*$//' -e 's/native[[:space:]]*//' -e 's/constant[[:space:]]*//' -e 's/function[[:space:]]*//' -e '/^[[:space:]]*\/\//d'