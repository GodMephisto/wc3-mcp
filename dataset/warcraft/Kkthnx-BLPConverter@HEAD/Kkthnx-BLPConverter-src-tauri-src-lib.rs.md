---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src-tauri/src/lib.rs"
ref: HEAD
---

mod commands;
mod conversion;
mod metadata;
mod shell_extension;
mod types;

use commands::{
    blpview_install, blpview_restart_explorer, blpview_status, blpview_uninstall, convert_paths,
    scan_paths,
};

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    tauri::Builder::default()
        .plugin(tauri_plugin_dialog::init())
        .invoke_handler(tauri::generate_handler![
            scan_paths,
            convert_paths,
            blpview_status,
            blpview_install,
            blpview_uninstall,
            blpview_restart_explorer,
        ])
        .run(tauri::generate_context!())
        .expect("error while running BLP Converter");
}
