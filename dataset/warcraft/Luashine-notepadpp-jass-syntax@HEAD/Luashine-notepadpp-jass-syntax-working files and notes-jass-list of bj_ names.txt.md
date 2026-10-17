---
type: vendor-doc
source_repo: "https://github.com/Luashine/notepadpp-jass-syntax"
source_path: "working files and notes/jass-list of bj_ names.txt"
ref: HEAD
---

# Bash (shell) with coreutils installed. Command:

# TWO STEPS!

# First:
grep -oP 'bj_[A-Za-z0-9_]+' blizzard.j | sort | uniq

# Second (filter variables dont follow the same naming scheme):
grep 'boolexpr' blizzard.j |grep 'null' | grep -oP 'filter[A-Za-z0-9_]+' | sort | uniq

# You can also put it to clipboard on Windows/Cygwin:
# command... > /dev/clipboard

# Double check if all of the values are actually saved by Notepad++, currently it's
# way below limit, but who knows what the future brings
# "There're probably too many constants to fit into one group in Notepad++'s Keyword Lists. If so, please split them manually into two groups."