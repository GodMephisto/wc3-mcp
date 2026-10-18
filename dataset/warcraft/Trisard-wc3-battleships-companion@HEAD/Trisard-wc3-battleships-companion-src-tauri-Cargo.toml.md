---
type: vendor-doc
source_repo: "https://github.com/Trisard/wc3-battleships-companion"
source_path: "src-tauri/Cargo.toml"
ref: HEAD
---

[package]
name = "battleships-companion"
version = "0.1.0"
description = "Warcraft III custom game companion application for Battleships."
authors = ["Tristan"]
edition = "2021"

# See more keys and their definitions at https://doc.rust-lang.org/cargo/reference/manifest.html

[lib]
# The `_lib` suffix may seem redundant but it is necessary
# to make the lib name unique and wouldn't conflict with the bin name.
# This seems to be only an issue on Windows, see https://github.com/rust-lang/cargo/issues/8519
name = "battleships_companion_lib"
crate-type = ["staticlib", "cdylib", "rlib"]

[build-dependencies]
tauri-build = { version = "2", features = [] }

[dependencies]
tauri = { version = "2", features = [] }
tauri-plugin-opener = "2"
serde = { version = "1", features = ["derive"] }
serde_json = "1"
wc3_local_api = "0.1.0"

