#!/usr/bin/env bash
# Build CascLib for this Mac and put the result where the server looks for it.
#
# CascLib.NET ships a native library for win-x64 only, so on macOS the server can read
# map archives but not the game's own data. Object fields fall back to the map's deltas,
# textures cannot be resolved, and the game data tests fail with "Unable to load shared
# library". Building CascLib here fixes that. It is upstream C++, unmodified.
#
# Usage, from anywhere:
#   tools/build-casclib-macos.sh [target-dir]
# target-dir defaults to ~/.local/opt/wc3-mcp and the library is named libCascLib.dylib,
# which is what .NET probes for when the managed wrapper asks for "CascLib".
set -euo pipefail

if [ "$(uname -s)" != "Darwin" ]; then
    echo "this builds the macOS library; Windows ships CascLib.dll in the package" >&2
    exit 1
fi

target="${1:-$HOME/.local/opt/wc3-mcp}"
work="${TMPDIR:-/tmp}/casclib-build"
repo="$work/CascLib"

command -v cmake >/dev/null || { echo "cmake is missing, brew install cmake" >&2; exit 1; }
xcode-select -p >/dev/null || { echo "Xcode command line tools are missing" >&2; exit 1; }

mkdir -p "$work"
if [ ! -d "$repo" ]; then
    git clone --depth 1 https://github.com/ladislav-zezula/CascLib.git "$repo"
fi

# The project asks for a CMake older than 3.5, which CMake 4 refuses outright.
cmake -S "$repo" -B "$repo/build" \
    -DCMAKE_BUILD_TYPE=Release \
    -DCASC_BUILD_SHARED_LIB=ON \
    -DCASC_BUILD_TESTS=OFF \
    -DCMAKE_POLICY_VERSION_MINIMUM=3.5
cmake --build "$repo/build" --parallel "$(sysctl -n hw.ncpu)"

# CMake marks the target FRAMEWORK on Apple, so the binary lives inside the bundle. It is
# installed under the name the managed wrapper asks for, CascLib.dll, because the wrapper's
# DllImport names that literal and macOS probing will not find a .dylib it never asked for.
# dlopen reads a Mach-O whatever the extension says.
mkdir -p "$target"
cp "$repo/build/casc.framework/casc" "$target/CascLib.dll"
echo "installed $target/CascLib.dll"
echo "game data now opens from the server installed there"
