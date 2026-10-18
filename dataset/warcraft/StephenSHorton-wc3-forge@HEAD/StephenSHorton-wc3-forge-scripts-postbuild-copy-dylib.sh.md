---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "scripts/postbuild-copy-dylib.sh"
ref: HEAD
---

#!/usr/bin/env bash
# Wails postBuildHook: copy the vendored CascLib dylib next to the compiled
# binary inside the .app bundle.
#
# The Go side (internal/casc/casc.go) loads libcasc.dylib via purego.Dlopen.
# Its install_name is rewritten to @loader_path/libcasc.dylib by
# scripts/build-casclib-macos.sh, so the loader resolves it relative to the
# binary that called dlopen — which means the dylib must sit alongside that
# binary inside Contents/MacOS/. Without this hook the user gets the
# cryptic "dlopen ... image not found" at runtime (with zero units rendered).
#
# Wails v2 passes the freshly built binary path as $1 via the `${bin}`
# substitution in wails.json. On macOS that path is the inner Mach-O of the
# .app bundle, e.g. build/bin/wc3-forge.app/Contents/MacOS/wc3-forge. We
# take its parent directory as the destination. If the substitution ever
# changes to hand us the .app bundle root, we transparently descend into
# Contents/MacOS.
set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
SRC_DIR="$SCRIPT_DIR/casclib"
SRC_DYLIB="$SRC_DIR/libcasc.dylib"

if [[ ! -f "$SRC_DYLIB" ]]; then
  cat >&2 <<EOF
libcasc.dylib not found at $SRC_DYLIB.

Build it once with:
  $SCRIPT_DIR/build-casclib-macos.sh

This downloads CascLib upstream, compiles it via cmake, and drops the dylib
into scripts/casclib/. It's .gitignored — every dev machine builds its own.
EOF
  exit 1
fi

BIN="${1:-}"
if [[ -z "$BIN" ]]; then
  echo "usage: $(basename "$0") <bin>" >&2
  exit 1
fi

# Resolve the destination directory:
#   - <something>.app                       → <something>.app/Contents/MacOS
#   - <something>.app/Contents/MacOS/<exe>  → <something>.app/Contents/MacOS
#   - <other-file>                          → dirname <other-file>
if [[ "$BIN" == *.app ]]; then
  DEST_DIR="$BIN/Contents/MacOS"
else
  DEST_DIR="$(dirname "$BIN")"
fi

if [[ ! -d "$DEST_DIR" ]]; then
  mkdir -p "$DEST_DIR"
fi

cp -v "$SRC_DYLIB" "$DEST_DIR/libcasc.dylib"
