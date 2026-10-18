---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src-tauri/build.rs"
ref: HEAD
---

fn main() {
    tauri_build::build();
    #[cfg(target_os = "windows")]
    bundle_shell_dll();
}

#[cfg(target_os = "windows")]
fn bundle_shell_dll() {
    use std::fs;
    use std::path::PathBuf;

    let manifest_dir = PathBuf::from(env!("CARGO_MANIFEST_DIR"));
    let profile = std::env::var("PROFILE").unwrap_or_else(|_| "debug".into());
    let workspace = manifest_dir.join("..");
    let dll_src = workspace.join(format!("target/{profile}/blp_shell_ext.dll"));

    let resources_dir = manifest_dir.join("resources");
    let dll_dst = resources_dir.join("blpview_thumb.dll");
    let _ = fs::create_dir_all(&resources_dir);

    if dll_src.exists() {
        if !needs_refresh(&dll_src, &dll_dst) {
            println!("cargo:rerun-if-changed={}", dll_dst.display());
        } else if let Err(err) = copy_dll_with_retries(&dll_src, &dll_dst, 12) {
            if dll_dst.exists() && dll_dst.metadata().map(|m| m.len() > 0).unwrap_or(false) {
                println!(
                    "cargo:warning=Could not refresh BLPView DLL from {} ({err}); using existing bundle",
                    dll_src.display()
                );
            } else {
                panic!("failed to copy BLPView thumbnail DLL: {err}");
            }
        } else {
            println!("cargo:rerun-if-changed={}", dll_src.display());
        }
    } else if !dll_dst.exists() || dll_dst.metadata().map(|m| m.len() == 0).unwrap_or(true) {
        let _ = fs::write(&dll_dst, []);
        println!(
            "cargo:warning=BLPView DLL missing. Build it first: cargo build -p blp-shell-ext"
        );
    }

    println!("cargo:rerun-if-changed=../blp-shell-ext/src");
    println!("cargo:rerun-if-changed={}", dll_dst.display());
}

#[cfg(target_os = "windows")]
fn needs_refresh(src: &std::path::Path, dst: &std::path::Path) -> bool {
    let Ok(src_meta) = src.metadata() else {
        return true;
    };
    let Ok(dst_meta) = dst.metadata() else {
        return true;
    };

    if src_meta.len() != dst_meta.len() {
        return true;
    }

    match (src_meta.modified(), dst_meta.modified()) {
        (Ok(src_time), Ok(dst_time)) => src_time > dst_time,
        _ => true,
    }
}

#[cfg(target_os = "windows")]
fn copy_dll_with_retries(
    src: &std::path::Path,
    dst: &std::path::Path,
    attempts: u32,
) -> std::io::Result<()> {
    use std::fs;
    use std::io;
    use std::thread::sleep;
    use std::time::Duration;

    let mut last_err = io::Error::other("copy not attempted");

    for attempt in 0..attempts {
        match fs::read(src).and_then(|bytes| fs::write(dst, bytes)) {
            Ok(()) => return Ok(()),
            Err(err) if is_sharing_violation(&err) && attempt + 1 < attempts => {
                last_err = err;
                sleep(Duration::from_millis(150 * u64::from(attempt + 1)));
            }
            Err(err) => return Err(err),
        }
    }

    Err(last_err)
}

#[cfg(target_os = "windows")]
fn is_sharing_violation(err: &std::io::Error) -> bool {
    err.raw_os_error() == Some(32)
}
