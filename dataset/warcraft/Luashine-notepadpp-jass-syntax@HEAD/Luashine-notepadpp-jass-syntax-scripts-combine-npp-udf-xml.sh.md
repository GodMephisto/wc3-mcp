---
type: vendor-doc
source_repo: "https://github.com/Luashine/notepadpp-jass-syntax"
source_path: "scripts/combine-npp-udf-xml.sh"
ref: HEAD
---

#!/usr/bin/env bash

# Combine multiple UDF XMLs into one for release

# Usage: $0 UDF_1.xml UDF_2.xml ... > combined.xml

grep -E '<NotepadPlus.*?>' "$1"
echo '<!-- XML concatenated using shell script from notepad++ jass syntax package by Luashine -->'
for file in "$@"; do
	grep -vE '</?NotepadPlus.*?>' "$file"
done
grep -E '</NotepadPlus.*?>' "$1"
