---
type: vendor-doc
source_repo: "https://github.com/WarRaft/blp-lib"
source_path: "build-examples.sh"
ref: HEAD
---

#!/usr/bin/env bash
set -euo pipefail

root_dir="$(cd "$(dirname "$0")" && pwd)"
cd "$root_dir"

echo "==> Building Rust library (release)"
cargo build --release

out_dir="$root_dir/examples/dist"
rm -rf "$out_dir"
mkdir -p "$out_dir"

cc=${CC:-gcc}
includes=( -I"$root_dir/include" )
libdir=( -L"$root_dir/target/release" )
libs=( -lblp_lib )

uname_s=$(uname -s || echo Unknown)
ldflags=()
case "$uname_s" in
  Darwin) ldflags+=( -ldl ) ;;
  Linux)  ldflags+=( -ldl -lpthread ) ;;
  MINGW*|MSYS*|CYGWIN*) ldflags+=() ;;
esac

build_one() {
  local src="$1"
  local bin="$2"
  echo "-- $src -> $bin"
  "$cc" -std=c99 -Wall -Wextra "${includes[@]}" "${libdir[@]}" "$src" -o "$bin" "${libs[@]}" "${ldflags[@]}"
}

echo "==> Building C examples"
build_one "$root_dir/examples/encode_file.c" "$out_dir/encode_file"
build_one "$root_dir/examples/decode_file.c" "$out_dir/decode_file"
build_one "$root_dir/examples/encode_dir.c" "$out_dir/encode_dir"
build_one "$root_dir/examples/decode_dir.c" "$out_dir/decode_dir"

echo "==> Done. Binaries in $out_dir"
