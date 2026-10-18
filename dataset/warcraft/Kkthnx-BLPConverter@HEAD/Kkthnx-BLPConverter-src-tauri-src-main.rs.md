---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src-tauri/src/main.rs"
ref: HEAD
---

// Prevents additional console window on Windows in release
#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")]

fn main() {
    blp_converter_lib::run();
}
