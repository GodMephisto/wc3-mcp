---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JASS-Tree-sitter-Rust"
source_path: "publish.sh"
ref: HEAD
---

#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"

echo "🔨 Running build.sh..."
"$SCRIPT_DIR/build.sh"

echo "🔢 Bumping patch version (without git tag)..."
npm version patch --no-git-tag-version --force

echo "📦 Publishing patch..."
vsce publish

