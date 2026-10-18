---
type: vendor-doc
source_repo: "https://github.com/WarRaft/blp-lib"
source_path: ".cargo/config.toml"
ref: HEAD
---

[target.x86_64-pc-windows-gnu]
# brew install mingw-w64
linker = "x86_64-w64-mingw32-gcc"

[target.x86_64-unknown-linux-musl]
# https://github.com/FiloSottile/homebrew-musl-cross
linker = "x86_64-linux-musl-gcc"
