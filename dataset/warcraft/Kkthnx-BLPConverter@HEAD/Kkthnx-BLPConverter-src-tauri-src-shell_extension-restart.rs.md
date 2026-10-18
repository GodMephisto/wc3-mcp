---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src-tauri/src/shell_extension/restart.rs"
ref: HEAD
---

use std::fs;
use std::path::PathBuf;
use std::process::{Command, Stdio};
use std::thread::sleep;
use std::time::Duration;

fn explorer_thumb_cache_dir() -> Option<PathBuf> {
    let local = std::env::var_os("LOCALAPPDATA")?;
    Some(
        PathBuf::from(local)
            .join("Microsoft")
            .join("Windows")
            .join("Explorer"),
    )
}

fn should_delete_explorer_cache_file(name: &str) -> bool {
    let lower = name.to_ascii_lowercase();
    lower.ends_with(".db")
        && (lower.starts_with("thumbcache_")
            || lower.starts_with("iconcache_")
            || lower.starts_with("iconcache"))
}

fn clear_thumbnail_cache() {
    let Some(dir) = explorer_thumb_cache_dir() else {
        return;
    };

    let Ok(entries) = fs::read_dir(&dir) else {
        return;
    };

    for entry in entries.flatten() {
        let name = entry.file_name();
        let name = name.to_string_lossy();
        if should_delete_explorer_cache_file(&name) {
            let _ = fs::remove_file(entry.path());
        }
    }
}

pub fn restart_explorer() -> Result<(), String> {
    let _ = Command::new("taskkill")
        .args(["/F", "/IM", "explorer.exe"])
        .stdin(Stdio::null())
        .stdout(Stdio::null())
        .stderr(Stdio::null())
        .status();

    sleep(Duration::from_millis(400));

    // Clear the cache only after Explorer is down. While it is running it holds
    // thumbcache_*.db open, so deleting earlier just fails silently.
    clear_thumbnail_cache();

    Command::new("explorer.exe")
        .stdin(Stdio::null())
        .stdout(Stdio::null())
        .stderr(Stdio::null())
        .spawn()
        .or_else(|_| {
            Command::new("cmd")
                .args(["/C", "start", "", "explorer.exe"])
                .stdin(Stdio::null())
                .stdout(Stdio::null())
                .stderr(Stdio::null())
                .spawn()
        })
        .map_err(|err| err.to_string())?;

    Ok(())
}
