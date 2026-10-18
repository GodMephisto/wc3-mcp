---
type: vendor-doc
source_repo: "https://github.com/WarRaft/blp-lib"
source_path: "Cargo.toml"
ref: HEAD
---

[package]
name = "blp-lib"
version = "0.1.1"
edition = "2024"


[lib]
# Use both cdylib and staticlib except for Linux, where only staticlib is supported
crate-type = [
	"cdylib", # macOS, Windows
	"staticlib" # all platforms
]

# Platform-specific crate-type overrides
[target.'cfg(target_os = "linux")'.lib]
crate-type = ["staticlib"]

[dependencies]
blp = "0.1.32"
libc = "0.2.177"

