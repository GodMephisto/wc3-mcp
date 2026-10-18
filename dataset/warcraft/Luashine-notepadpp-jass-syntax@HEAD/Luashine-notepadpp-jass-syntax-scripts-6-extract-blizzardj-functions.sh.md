---
type: vendor-doc
source_repo: "https://github.com/Luashine/notepadpp-jass-syntax"
source_path: "scripts/6-extract-blizzardj-functions.sh"
ref: HEAD
---

#!/usr/bin/env bash

set -euo pipefail
script=$1

# Tip: to replace newline with spaces: | tr '\n' ' ' 

# Command to extract functions defined in blizzard.j:

# Simple version:
#grep 'function' "$script" | grep 'takes' | cut -d' ' -f2

# Future-proof version:
grep 'function' "$script" \
| grep 'takes' \
| sed \
	-e 's/[[:space:]]\{1,\}takes.*$//' \
	-e 's/native[[:space:]]*//' \
	-e 's/constant[[:space:]]*//' \
	-e 's/function[[:space:]]*//' \
	-e '/^[[:space:]]*\/\//d'

