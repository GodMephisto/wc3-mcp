---
type: vendor-doc
source_repo: "https://github.com/Trisard/wc3-battleships-companion"
source_path: "src-tauri/src/main.rs"
ref: HEAD
---

// Prevents additional console window on Windows in release, DO NOT REMOVE!!
#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")]

fn main() {
    battleships_companion_lib::run()
}
