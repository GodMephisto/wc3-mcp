---
type: vendor-doc
source_repo: "https://github.com/WarRaft/blp-rs"
source_path: "cargo-publish.sh"
ref: HEAD
---

#!/usr/bin/env bash
set -euo pipefail

cargo publish --locked
