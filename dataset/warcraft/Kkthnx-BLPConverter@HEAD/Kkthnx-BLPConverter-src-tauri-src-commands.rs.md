---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src-tauri/src/commands.rs"
ref: HEAD
---

use rayon::prelude::*;

use crate::conversion::convert_paths_batch;
use crate::metadata::{collect_supported_files, extract_metadata};
use crate::shell_extension::{
    get_blpview_status, install_blpview, restart_explorer, uninstall_blpview,
};
use crate::types::{
    AssetKind, BatchConvertResult, BlpViewActionResult, BlpViewStatus, ConversionSettings,
    FileMetadata, ScanPathsResult,
};

#[tauri::command]
pub async fn scan_paths(paths: Vec<String>) -> ScanPathsResult {
    // Scanning parses every dropped file (and walks folders) on a thread pool.
    // Run it off the main thread so the UI and drag events stay responsive.
    tauri::async_runtime::spawn_blocking(move || scan_paths_blocking(paths))
        .await
        .unwrap_or_else(|_| ScanPathsResult {
            assets: Vec::new(),
            errors: vec!["Scan task failed unexpectedly".into()],
        })
}

fn scan_paths_blocking(paths: Vec<String>) -> ScanPathsResult {
    let (files, mut errors) = collect_supported_files(&paths);

    let assets: Vec<FileMetadata> = files
        .par_iter()
        .map(|path| extract_metadata(path))
        .collect();

    errors.extend(
        assets
            .iter()
            .filter_map(|asset| asset.error.as_ref().map(|e| format!("{}: {e}", asset.path))),
    );

    ScanPathsResult { assets, errors }
}

#[tauri::command]
pub async fn convert_paths(
    paths: Vec<String>,
    kind: AssetKind,
    settings: ConversionSettings,
) -> BatchConvertResult {
    // Conversion is CPU heavy and may process whole folders. Offload it to a
    // blocking thread so a large batch never freezes the webview.
    tauri::async_runtime::spawn_blocking(move || convert_paths_batch(&paths, kind, &settings))
        .await
        .unwrap_or_else(|_| BatchConvertResult {
            succeeded: 0,
            failed: 0,
            scan_errors: vec!["Conversion task failed unexpectedly".into()],
            results: Vec::new(),
        })
}

#[tauri::command]
pub fn blpview_status() -> BlpViewStatus {
    get_blpview_status()
}

#[tauri::command]
pub fn blpview_install() -> Result<BlpViewActionResult, String> {
    install_blpview()
}

#[tauri::command]
pub fn blpview_uninstall() -> Result<BlpViewActionResult, String> {
    uninstall_blpview()
}

#[tauri::command]
pub fn blpview_restart_explorer() -> Result<(), String> {
    restart_explorer()
}
