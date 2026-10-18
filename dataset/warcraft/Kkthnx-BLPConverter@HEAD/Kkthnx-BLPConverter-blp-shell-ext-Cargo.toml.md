---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "blp-shell-ext/Cargo.toml"
ref: HEAD
---

[package]
name = "blp-shell-ext"
version = "1.2.1"
edition = "2021"
description = "BLPView Windows Explorer thumbnail shell extension"
authors = ["Kkthnx"]
license = "LicenseRef-All-Rights-Reserved"
publish = false

[lib]
name = "blp_shell_ext"
crate-type = ["cdylib", "rlib"]

[dependencies]
image = "0.24"
image-blp = "1"
widestring = "1.2"
windows = { version = "0.62", features = [
    "Win32_Foundation",
    "Win32_System_Com",
    "Win32_System_Ole",
    "Win32_Storage_FileSystem",
    "Win32_Graphics_Gdi",
    "Win32_UI_Shell",
    "Win32_UI_Shell_Common",
    "Win32_UI_Shell_PropertiesSystem",
    "Win32_System_LibraryLoader",
    "Win32_System_Threading",
] }
windows-core = "0.62"
windows-implement = "0.60"

[build-dependencies]
winresource = "0.1"
