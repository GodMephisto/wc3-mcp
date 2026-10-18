---
type: vendor-doc
source_repo: "https://github.com/Luashine/notepadpp-jass-syntax"
source_path: "scripts/5-extract-bj_-globals.sh"
ref: HEAD
---

#!/usr/bin/env bash

set -euo pipefail
script=$1

GLOBALS_START=$(grep '^[[:space:]]*globals' -i --line-number "$script" | head -n1|cut -f1 -d':');
#echo "globals starts at line: $GLOBALS_START" >&2
GLOBALS_END=$(grep '^[[:space:]]*endglobals' -i --line-number "$script" | head -n1|cut -f1 -d':');
#echo "globals ends at line: $GLOBALS_END" >&2
GLOBALS_LINES=$(( GLOBALS_END - GLOBALS_START ))

GLOBALS_BLOCK=$(mktemp)

set +o pipefail # omg it was supposed to pipefail here, end of stream...
tail -n "+${GLOBALS_START}" "$script" | head "--lines=${GLOBALS_LINES}" > "$GLOBALS_BLOCK"
set -o pipefail

step1=$(mktemp)
step2=$(mktemp)

# Tip: to replace newline with spaces: | tr '\n' ' ' 

# Bash (shell) with coreutils installed. Command:

# TWO STEPS!

# First:
grep -oP 'bj_[A-Za-z0-9_]+' "$GLOBALS_BLOCK" > "$step1"

# Second (filter variables dont follow the same naming scheme):
grep 'boolexpr' "$GLOBALS_BLOCK" | grep 'null' | \
	grep -oP 'filter[A-Za-z0-9_]+' > "$step2"

sort "$step1" "$step2" | uniq

rm -- "$GLOBALS_BLOCK" "$step1" "$step2"

# You can also put it to clipboard on Windows/Cygwin:
# command... > /dev/clipboard

# Double check if all of the values are actually saved by Notepad++, currently it's
# way below limit, but who knows what the future brings
# "There're probably too many constants to fit into one group in Notepad++'s Keyword Lists.
# If so, please split them manually into two groups."