---
type: vendor-doc
source_repo: "https://github.com/WarRaft/blp-lib"
source_path: "build-settings.sh"
ref: HEAD
---

#!/usr/bin/env bash
# Common variables and helpers for build scripts

set -euo pipefail

# ====== SETTINGS ======
export DIST_DIR="${DIST_DIR:-dist}"
export BUILD_VARIANTS="${BUILD_VARIANTS:-$'blp-lib::library'}"
export APP_ID_BUNDLE="${APP_ID_BUNDLE:-com.warraft}"
export UPX="${UPX:-0}"
# ======================

# Size-friendly flags ONLY at rustc level (without LTO and without embed-bitcode)
# We'll enable LTO in Cargo.toml in the release profile.
_prev="${RUSTFLAGS:-}"
# clean possible embed-bitcode traces from environment
_prev="${_prev//-C embed-bitcode=no/}"
_prev="${_prev//-C embed-bitcode=yes/}"
export RUSTFLAGS="${_prev} -C codegen-units=1 -C opt-level=z -C strip=symbols -C panic=abort"
# =========================================

mkdir -p "$DIST_DIR"

need() { command -v "$1" &>/dev/null || { echo "❌ '$1' is required"; exit 1; }; }

find_llvm_strip() {
  local host sysroot p
  host="$(rustc -vV | sed -n 's/^host: //p')"
  sysroot="$(rustc --print sysroot)"
  for p in \
    "$sysroot/lib/rustlib/$host/bin/llvm-strip" \
    "$sysroot/lib/rustlib/bin/llvm-strip" \
    "/opt/homebrew/opt/llvm/bin/llvm-strip" \
    "/usr/local/opt/llvm/bin/llvm-strip" \
    "$(xcrun --find llvm-strip 2>/dev/null || true)"; do
    [[ -n "${p:-}" && -x "$p" ]] && { echo "$p"; return 0; }
  done
  return 1
}

LLVM_STRIP="$(find_llvm_strip || true)"

strip_safe() {
  # $1=path  $2=kind: macos|linux|windows
  local f="$1" kind="${2:-}"
  [[ -f "$f" ]] || return 0
  case "$kind" in
    macos)
      /usr/bin/strip -x "$f" 2> >(grep -v 'input object file already stripped' >&2) || true
      ;;
    linux|windows)
      if [[ -n "$LLVM_STRIP" ]]; then
        "$LLVM_STRIP" -s "$f" 2> >(grep -v 'already stripped' >&2) || true
      else
        echo "⚠️  Skipping strip for $f (no llvm-strip)"
      fi
      ;;
    *)
      [[ -n "$LLVM_STRIP" ]] && "$LLVM_STRIP" -s "$f" 2> >(grep -v 'already stripped' >&2) || true
      ;;
  esac
}

maybe_upx() {
  if [[ "$UPX" = "1" ]] && command -v upx &>/dev/null; then
    upx --best --lzma "$1" || true
  fi
}
