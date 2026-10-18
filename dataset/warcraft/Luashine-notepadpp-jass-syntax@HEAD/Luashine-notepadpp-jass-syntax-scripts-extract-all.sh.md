---
type: vendor-doc
source_repo: "https://github.com/Luashine/notepadpp-jass-syntax"
source_path: "scripts/extract-all.sh"
ref: HEAD
---

#!/usr/bin/env bash

set -euo pipefail

# /home/userr/git/jass-history/timeline/scripts/blizzard.j
# /home/userr/git/jass-history/timeline/scripts/common.j

USAGE=$(cat <<EOF
$0 "path/to/common.j" "path/to/blizzard.j"

NOTE: DO NOT USE THIS WITH ANNOTATED JASSDOC FILES!
EOF
)

if [[ $# -lt 2 ]]; then
	echo "$USAGE" >&2
	exit 1
fi

COMMONJ="$1"
BLIZZARDJ="$2"
MYPATH="$(dirname "$(realpath "$0")")"

if [[ ! -e "$COMMONJ" ]]; then
	echo "common.j doesnt exist: '$COMMONJ'" >&2
	exit 1
fi
if [[ ! -e "$BLIZZARDJ" ]]; then
	echo "blizzard.j doesnt exist: '$BLIZZARDJ'" >&2
	exit 1
fi

function run() {
	local script="$1"
	shift;
	echo "Running: $script" >&2
	"$MYPATH/$script" "$@"
}

function lf_to_space() {
	tr '\n' ' '
}

function count_bytes() {
	local file="$1"
	stat --format '%s' "$file"
}

function assert_line_count() {
	local file="$1"
	local lineCountExpect="$2"
	local lineCount="$(wc -l "$file" | cut -d' ' -f1)"
	# actually bump line count by 1, cuz we have no trailing \n anywhere
	lineCount=$(( lineCount + 1 ))
	
	if [[ "$(count_bytes "$file")" -eq 0 ]]; then
		lineCount=0
	fi
	if [[ "$lineCount" != "$lineCountExpect" ]]; then
		printf "Expected line count=%s, got=%s for file='%s'\n" \
			"$lineCountExpect" "$lineCount" "$file" >&2
		exit 2
	fi
}

function foldpp() {
	# fold file to Notepad++ max group text length
	# absolute limit is 30719 (Notepad++ 8.9.1)
	# ...but fold FOLDS AFTER the given width.
	# So we need enough buffer, else N++ silently doesn't load the UDF.
	# The longest name is currently 48
	fold --spaces --width="30640" -
}

# 1st group: keywords like: type, takes, local
# 2nd group: types like: nothing array string trigger
# 3rd group: common.j natives part 1
# 4th group: common.j natives part 2
# 5th group: bj_ constants
# 6th group: BJ functions
# 7th group: common.j constants part 1
# 8th group: common.j constants part 2

groupTypes_extended="output-2-extended-types.txt"
groupTypes_base="input-2-list of builtin types jass.txt"
group2="output-2-all-types.group2.txt"

groupNatives_all="output-3-commonj-natives.all.txt"
groupNatives_3="output-3-commonj-natives.group3.txt"
groupNatives_4="output-3-commonj-natives.group4.txt"

group5="output-5-bj_-globals.group5.txt"
group6="output-6-blizzardj-functions.group6.txt"

groupCommonConst_all="output-7-commonj-constants.all.txt"
groupCommonConst_7="output-7-commonj-constants.group7.txt"
groupCommonConst_8="output-7-commonj-constants.group8.txt"

run "2-extract-extended-types.sh" "$COMMONJ" | lf_to_space | foldpp \
	> "$groupTypes_extended"
assert_line_count "$groupTypes_extended" 1
# none of our files have a newline, so at least add a space inbetween!
cat "$groupTypes_base" <(printf " ") "$groupTypes_extended" | lf_to_space \
	> "$group2"

run "3-extract-commonj-natives.sh" "$COMMONJ" | lf_to_space | foldpp \
	> "$groupNatives_all"
assert_line_count "$groupNatives_all" 2
head -n 1 "$groupNatives_all" > "$groupNatives_3"
tail -n 1 "$groupNatives_all" > "$groupNatives_4"

run "5-extract-bj_-globals.sh" "$BLIZZARDJ" | lf_to_space | foldpp \
	> "$group5"
assert_line_count "$group5" 1

run "6-extract-blizzardj-functions.sh" "$BLIZZARDJ" | lf_to_space | foldpp \
	> "$group6"
assert_line_count "$group6" 1

run "7-extract-commonj-constants.sh" "$COMMONJ" | lf_to_space | foldpp \
	> "$groupCommonConst_all"
assert_line_count "$groupCommonConst_all" 2
head -n 1 "$groupCommonConst_all" > "$groupCommonConst_7"
tail -n 1 "$groupCommonConst_all" > "$groupCommonConst_8"

echo "Finished!" >&2
