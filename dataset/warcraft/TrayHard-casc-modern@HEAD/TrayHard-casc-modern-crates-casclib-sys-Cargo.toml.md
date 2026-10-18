---
type: vendor-doc
source_repo: "https://github.com/TrayHard/casc-modern"
source_path: "crates/casclib-sys/Cargo.toml"
ref: HEAD
---

[package]
name = "casclib-sys"
version.workspace = true
edition.workspace = true
license = "MIT"
authors.workspace = true
links = "casc"
description = "Hand-written FFI bindings to Ladislav Zezula's CascLib (vendored, built from source)."

[lib]
doctest = false

[build-dependencies]
cc = "1"
