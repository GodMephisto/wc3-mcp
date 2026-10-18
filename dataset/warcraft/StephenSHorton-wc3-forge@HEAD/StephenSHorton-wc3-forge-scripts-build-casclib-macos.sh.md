---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "scripts/build-casclib-macos.sh"
ref: HEAD
---

#!/usr/bin/env bash
# Build CascLib as a shared library (libcasc.dylib) for macOS, then drop it
# next to the existing Windows DLLs in scripts/casclib/. The Go side loads
# it dynamically via purego (see internal/casc/casc.go) — no cgo, no link-
# time dependency, just dlopen at startup.
#
# Why a script instead of vendoring the .dylib: macOS dylibs are
# architecture-specific (arm64 vs x86_64) and version-sensitive enough that
# checking a prebuilt copy into git creates more confusion than it saves.
# Building from source on each developer's machine produces a binary that
# matches their Mac.
#
# Run once after cloning, then whenever CascLib upstream updates.
set -euo pipefail

REPO_REV="${CASCLIB_REV:-master}"
SRCDIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/casclib" && pwd)"
WORK="$(mktemp -d)"
trap 'rm -rf "$WORK"' EXIT

echo "Cloning CascLib@$REPO_REV into $WORK …"
git clone --depth 1 --branch "$REPO_REV" \
  https://github.com/ladislav-zezula/CascLib "$WORK/CascLib"

cd "$WORK/CascLib"
cmake -B build -S . \
  -DCASC_BUILD_SHARED_LIB=ON \
  -DCASC_BUILD_STATIC_LIB=OFF \
  -DCMAKE_BUILD_TYPE=Release \
  -DCMAKE_OSX_DEPLOYMENT_TARGET=11.0 \
  -DCMAKE_POLICY_VERSION_MINIMUM=3.5
cmake --build build --config Release -j

# CascLib's CMake produces a casc.framework bundle on macOS (not a plain
# libcasc.dylib). The Mach-O binary inside the framework IS the dylib — we
# pull it out and copy alongside the existing Windows DLLs. The Go loader
# (purego.Dlopen) picks the right filename per-platform.
DYLIB="$(find build -type f \( -name 'libcasc*.dylib' -o -path '*casc.framework/casc' -o -path '*casc.framework/Versions/*/casc' \) -print -quit)"
if [[ -z "$DYLIB" ]]; then
  echo "build did not produce a casc dylib (looked for libcasc*.dylib and casc.framework/casc)" >&2
  exit 1
fi

cp -v "$DYLIB" "$SRCDIR/libcasc.dylib"
# Strip the absolute install_name so dlopen by relative path works once
# the dylib is copied next to the binary at postbuild time.
install_name_tool -id "@loader_path/libcasc.dylib" "$SRCDIR/libcasc.dylib"

echo "Done. libcasc.dylib at: $SRCDIR/libcasc.dylib"
