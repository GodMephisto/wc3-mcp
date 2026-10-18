---
type: vendor-doc
source_repo: "https://github.com/WarRaft/blp-lib"
source_path: "build-only.sh"
ref: HEAD
---

#!/usr/bin/env bash
set -euo pipefail
source "$(dirname "$0")/build-settings.sh"

need cargo; need rustup; need jq; need lipo; need file

# Reset the human-readable build log for this top-level run
mkdir -p build
: > build/build-info.txt   # truncate to zero (or create)

# Optional: stable ID for this script run (will appear in the log)
export BLP_BUILD_ID="${BLP_BUILD_ID:-$(od -An -N8 -tu8 /dev/urandom | tr -d ' ')}"

# Write header line instead of leaving file empty
{
  printf "===== 🛠️  Library Build @ %s UTC =====\n" "$(date -u '+%Y-%m-%d %H:%M:%S')"
  printf "🆔 Build ID : %s\n" "$BLP_BUILD_ID"
  printf "\n"
} > build/build-info.txt

PROJECT_NAME="$(cargo metadata --no-deps --format-version 1 | jq -r '.packages[0].name')"

# --- clean dist ---
rm -rf "$DIST_DIR"
mkdir -p "$DIST_DIR"

build_variant() {
  local lib_name="$1"
  local feature_spec="$2"
  local packaging="${3:-library}"
  local feature_label="${feature_spec:-none}"
  local cargo_features=(--no-default-features)
  if [[ -n "$feature_spec" && "$feature_spec" != "-" ]]; then
    cargo_features+=(--features "$feature_spec")
  fi

  printf "\n=== 🔨 Building %s (features: %s, packaging: %s) ===\n" "$lib_name" "$feature_label" "$packaging"

  # ===== macOS universal =====
  printf "📦 macOS universal libraries...\n"
  rustup target add aarch64-apple-darwin x86_64-apple-darwin &>/dev/null || true
  cargo build --release --target aarch64-apple-darwin --locked "${cargo_features[@]}"
  cargo build --release --target x86_64-apple-darwin --locked "${cargo_features[@]}"

  # Create universal static library (convert hyphen to underscore for file names)
  local lib_file_name="${lib_name//-/_}"
  # Use "blp" as the library name instead of "blp-lib"
  local output_name="blp"
  local mac_static="$DIST_DIR/lib${output_name}-macos.a"
  lipo -create \
    -output "$mac_static" \
    "target/aarch64-apple-darwin/release/lib${lib_file_name}.a" \
    "target/x86_64-apple-darwin/release/lib${lib_file_name}.a"
  
  # Create universal dynamic library
  local mac_dynamic="$DIST_DIR/lib${output_name}-macos.dylib"
  lipo -create \
    -output "$mac_dynamic" \
    "target/aarch64-apple-darwin/release/lib${lib_file_name}.dylib" \
    "target/x86_64-apple-darwin/release/lib${lib_file_name}.dylib"
  
  strip_safe "$mac_static" macos
  strip_safe "$mac_dynamic" macos
  file "$mac_static"
  file "$mac_dynamic"

  # ===== Windows =====
  printf "🪟 Windows libraries...\n"
  rustup target add x86_64-pc-windows-gnu &>/dev/null || true
  cargo build --release --target x86_64-pc-windows-gnu --locked "${cargo_features[@]}"
  
  local win_static="$DIST_DIR/lib${output_name}-windows.a"
  local win_dynamic="$DIST_DIR/${output_name}-windows.dll"
  cp "target/x86_64-pc-windows-gnu/release/lib${lib_file_name}.a" "$win_static"
  cp "target/x86_64-pc-windows-gnu/release/${lib_file_name}.dll" "$win_dynamic"
  strip_safe "$win_static" windows
  strip_safe "$win_dynamic" windows
  file "$win_static"
  file "$win_dynamic"

  # ===== Linux (musl) =====
  printf "🐧 Linux libraries...\n"
  rustup target add x86_64-unknown-linux-musl &>/dev/null || true
  cargo build --release --target x86_64-unknown-linux-musl --locked "${cargo_features[@]}"
  
  local lin_static="$DIST_DIR/lib${output_name}-linux.a"
  cp "target/x86_64-unknown-linux-musl/release/lib${lib_file_name}.a" "$lin_static"
  strip_safe "$lin_static" linux
  file "$lin_static"
  
  # Try to create dynamic library if cdylib is available (non-musl targets)
  if [[ -f "target/x86_64-unknown-linux-musl/release/lib${lib_file_name}.so" ]]; then
    local lin_dynamic="$DIST_DIR/lib${output_name}-linux.so"
    cp "target/x86_64-unknown-linux-musl/release/lib${lib_file_name}.so" "$lin_dynamic"
    strip_safe "$lin_dynamic" linux
    file "$lin_dynamic"
  else
    printf "⚠️  Dynamic library not available for musl target\n"
  fi
  
  # Copy header file to distribution
  cp "include/${lib_file_name}.h" "$DIST_DIR/${output_name}.h"
}

while IFS= read -r spec; do
  spec="${spec//$'\r'/}"
  [[ -z "${spec//[[:space:]]/}" ]] && continue
  IFS=':' read -r lib_name feature_spec packaging <<<"$spec"
  build_variant "$lib_name" "$feature_spec" "$packaging"
done <<<"$BUILD_VARIANTS"

# --- checksums ---
printf "\n🔐 Checksums...\n"
(
  cd "$DIST_DIR"
  rm -f SHA256SUMS.txt
  if command -v shasum &>/dev/null; then
    find . -maxdepth 1 -type f ! -name 'SHA256SUMS.txt' -exec shasum -a 256 {} \; > SHA256SUMS.txt
  else
    find . -maxdepth 1 -type f ! -name 'SHA256SUMS.txt' -exec sha256sum {} \; > SHA256SUMS.txt
  fi
)

# --- summary ---
printf "\n✅ Done. Contents of '%s':\n" "$DIST_DIR"
ls -lh "$DIST_DIR"
