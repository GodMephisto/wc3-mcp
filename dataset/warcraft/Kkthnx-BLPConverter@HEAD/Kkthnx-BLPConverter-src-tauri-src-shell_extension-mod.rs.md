---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src-tauri/src/shell_extension/mod.rs"
ref: HEAD
---

#[cfg(windows)]
mod conflict;
#[cfg(windows)]
mod paths;
#[cfg(windows)]
mod reg_helpers;
#[cfg(windows)]
mod install;
#[cfg(windows)]
mod restart;
#[cfg(windows)]
mod status;
#[cfg(windows)]
mod uninstall;

#[cfg(windows)]
pub use install::install_blpview;
#[cfg(windows)]
pub use restart::restart_explorer;
#[cfg(windows)]
pub use status::get_blpview_status;
#[cfg(windows)]
pub use uninstall::uninstall_blpview;

#[cfg(not(windows))]
pub fn get_blpview_status() -> crate::types::BlpViewStatus {
    crate::types::BlpViewStatus {
        installed: false,
        dll_path: String::new(),
        supported: false,
        message: "BLPView shell extension is only available on Windows.".into(),
    }
}

#[cfg(not(windows))]
pub fn install_blpview() -> Result<crate::types::BlpViewActionResult, String> {
    Err("BLPView shell extension is only available on Windows.".into())
}

#[cfg(not(windows))]
pub fn uninstall_blpview() -> Result<crate::types::BlpViewActionResult, String> {
    Err("BLPView shell extension is only available on Windows.".into())
}

#[cfg(not(windows))]
pub fn restart_explorer() -> Result<(), String> {
    Err("BLPView shell extension is only available on Windows.".into())
}
