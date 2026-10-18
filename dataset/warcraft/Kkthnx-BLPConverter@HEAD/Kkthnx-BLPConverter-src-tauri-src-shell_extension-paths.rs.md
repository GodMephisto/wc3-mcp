---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src-tauri/src/shell_extension/paths.rs"
ref: HEAD
---

use std::env;
use std::path::{Path, PathBuf};

use blp_shell_ext::{DLL_FILENAME, INSTALL_FOLDER};

pub fn legacy_dll_path() -> PathBuf {
    let base = env::var_os("LOCALAPPDATA")
        .map(PathBuf::from)
        .unwrap_or_else(|| PathBuf::from(r"C:\Users\Default\AppData\Local"));
    base.join(INSTALL_FOLDER).join(DLL_FILENAME)
}

pub fn preferred_dll_path() -> PathBuf {
    if let Ok(exe) = env::current_exe() {
        if let Some(dir) = exe.parent() {
            return dir.join(DLL_FILENAME);
        }
    }
    legacy_dll_path()
}

pub fn expected_dll_path() -> PathBuf {
    let preferred = preferred_dll_path();
    if preferred.exists() {
        return preferred;
    }
    legacy_dll_path()
}

pub fn dll_install_paths() -> Vec<PathBuf> {
    let mut paths = Vec::new();
    let preferred = preferred_dll_path();
    let legacy = legacy_dll_path();
    if !paths.iter().any(|p| p == &preferred) {
        paths.push(preferred.clone());
    }
    if legacy != preferred {
        paths.push(legacy);
    }
    paths
}

pub fn remove_installed_dlls() {
    for path in dll_install_paths() {
        if path.exists() {
            let _ = std::fs::remove_file(path);
        }
    }
}

pub fn ensure_dll_parent(path: &Path) -> std::io::Result<()> {
    if let Some(parent) = path.parent() {
        std::fs::create_dir_all(parent)?;
    }
    Ok(())
}
