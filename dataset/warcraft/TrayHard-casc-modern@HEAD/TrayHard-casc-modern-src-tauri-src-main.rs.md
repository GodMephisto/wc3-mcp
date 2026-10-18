---
type: vendor-doc
source_repo: "https://github.com/TrayHard/casc-modern"
source_path: "src-tauri/src/main.rs"
ref: HEAD
---

#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")]

fn main() {
    casc_modern_lib::run()
}
