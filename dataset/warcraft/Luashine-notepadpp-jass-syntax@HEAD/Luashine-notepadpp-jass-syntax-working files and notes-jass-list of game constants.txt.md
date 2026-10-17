---
type: vendor-doc
source_repo: "https://github.com/Luashine/notepadpp-jass-syntax"
source_path: "working files and notes/jass-list of game constants.txt"
ref: HEAD
---

# List of Common.j constants
# Run in bash (shell):

COMMONJ="common.j"
GLOBALS_START=$(grep globals -i --line-number "$COMMONJ" | head -n1|cut -f1 -d':'); echo "globals starts at line: $GLOBALS_START" >&2
GLOBALS_END=$(grep endglobals -i --line-number "$COMMONJ" | head -n1|cut -f1 -d':'); echo "globals ends at line: $GLOBALS_END" >&2

GLOBALS_LINES=$(( GLOBALS_END - GLOBALS_START ))

# Tail/head: only read the globals/endglobals block
# grep 0: ignore comme
# grep 1: select constant names surrounded by spaces
# grep 2: remove spaces

tail -n "+${GLOBALS_START}" "$COMMONJ"|head "--lines=${GLOBALS_LINES}" | grep -vP '^[ \t]*//' | grep -oP '[\t ]([A-Z_][A-Z_0-9]+)[\t ]'  | grep -oP '[^ \t]+' > /dev/clipboard


>&2 echo "There're probably too many constants to fit into one group in Notepad++'s Keyword Lists. If so, please split them manually into two groups."