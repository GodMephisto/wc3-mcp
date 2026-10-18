---
type: vendor-doc
source_repo: "https://github.com/Luashine/notepadpp-jass-syntax"
source_path: "scripts/2-extract-extended-types.sh"
ref: HEAD
---

#!/usr/bin/env bash

set -euo pipefail

# List of Common.j constants

COMMONJ="$1"

grep type "$COMMONJ" | grep extends | tr '\t' ' ' | cut -d' ' -f 2
