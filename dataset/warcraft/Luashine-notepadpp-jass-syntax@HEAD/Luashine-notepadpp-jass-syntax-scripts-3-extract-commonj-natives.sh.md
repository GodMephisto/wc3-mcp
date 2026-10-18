---
type: vendor-doc
source_repo: "https://github.com/Luashine/notepadpp-jass-syntax"
source_path: "scripts/3-extract-commonj-natives.sh"
ref: HEAD
---

#!/usr/bin/env bash

set -euo pipefail
script=$1

# Command to extract native function definitions from common.j file:

# grep 1, 2: only lines with 'native' and takes
# sed 1: remove everything after the function name
# sed 2, 3: remove 'native' and 'const' keywords
# sed 4: exclude commented lines

# PS: Bonus points if you rewrite the below to a single sed/awk command
# with or without sort+uniq part, it's there just in case

grep 'native' "$script" | grep 'takes' \
	| sed \
	-e 's/[[:space:]]\{1,\}takes.*$//' \
	-e 's/native[[:space:]]*//' \
	-e 's/constant[[:space:]]*//' \
	-e '/^[[:space:]]*\/\//d' \
	| sort | uniq

# Note: you must split the list in two groups again, it's too long for Notepad++
