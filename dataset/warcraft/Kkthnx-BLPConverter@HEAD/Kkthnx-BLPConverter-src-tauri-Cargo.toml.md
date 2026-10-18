---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src-tauri/Cargo.toml"
ref: HEAD
---

[package]
name = "blp-converter"
version = "1.2.1"
description = "Compact WoW BLP and PNG converter"
authors = ["Kkthnx"]
license = "LicenseRef-All-Rights-Reserved"
publish = false
edition = "2021"
rust-version = "1.77"

[lib]
name = "blp_converter_lib"
crate-type = ["staticlib", "cdylib", "rlib"]

[build-dependencies]
tauri-build = { version = "2", features = [] }

[dependencies]
tauri = { version = "2", features = [] }
tauri-plugin-dialog = "2"
serde = { version = "1", features = ["derive"] }
image = "0.24"
image-blp = "1"
rayon = "1"
walkdir = "2"

[target.'cfg(windows)'.dependencies]
blp-shell-ext = { path = "../blp-shell-ext" }
winreg = "0.55"
windows = { version = "0.62", features = ["Win32_UI_Shell"] }
