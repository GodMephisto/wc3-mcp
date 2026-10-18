---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src-tauri/src/shell_extension/status.rs"
ref: HEAD
---

use std::path::Path;

use blp_shell_ext::{GuidExt, CLSID_BLP_THUMB, DEFAULT_EXT, SHELL_THUMB_HANDLER_CATID};
use winreg::enums::HKEY_CURRENT_USER;
use winreg::RegKey;

use crate::types::BlpViewStatus;
use super::conflict::legacy_conflict_message;
use super::paths::expected_dll_path;
use super::reg_helpers::RegKeyHelper;

pub fn get_blpview_status() -> BlpViewStatus {
    let dll_path = expected_dll_path();
    let clsid = CLSID_BLP_THUMB.to_braced_upper();
    let thumb_catid = SHELL_THUMB_HANDLER_CATID.to_braced_upper();
    let root = RegKey::predef(HKEY_CURRENT_USER);

    let registry_installed = root
        .open_subkey(format!(r"Software\Classes\CLSID\{clsid}"))
        .is_ok();

    let dll_exists = dll_path.exists()
        && dll_path.metadata().map(|m| m.len() > 0).unwrap_or(false);

    let shellex_ok = RegKeyHelper::open(
        &root,
        format!(r"Software\Classes\SystemFileAssociations\{DEFAULT_EXT}\ShellEx\{thumb_catid}"),
    )
    .ok()
    .and_then(|key| key.get::<String>("").ok())
    .map(|value| value.eq_ignore_ascii_case(&clsid))
    .unwrap_or(false);

    let dll_path_matches = root
        .open_subkey(format!(r"Software\Classes\CLSID\{clsid}\InprocServer32"))
        .ok()
        .and_then(|key| key.get_value::<String, _>("").ok())
        .map(|value| Path::new(&value) == dll_path.as_path())
        .unwrap_or(false);

    let approved_ok = RegKeyHelper::open(
        &root,
        r"Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved",
    )
    .ok()
    .and_then(|key| key.get::<String>(&clsid).ok())
    .is_some();

    let isolation_ok = root
        .open_subkey(format!(r"Software\Classes\CLSID\{clsid}"))
        .ok()
        .and_then(|key| key.get_value::<u32, _>("DisableProcessIsolation").ok())
        .map(|value| value != 0)
        .unwrap_or(false);

    let installed = registry_installed
        && dll_exists
        && shellex_ok
        && dll_path_matches
        && approved_ok
        && isolation_ok;

    let message = if let Some(conflict) = legacy_conflict_message() {
        conflict.into()
    } else if installed {
        "BLPView is active. .blp thumbnails show in Windows Explorer. Use Large or Extra large icons view.".into()
    } else if registry_installed && !dll_exists {
        "BLPView registry entries exist but the thumbnail DLL is missing. Reinstall BLPView.".into()
    } else if registry_installed && !isolation_ok {
        "BLPView needs a reinstall to refresh Explorer thumbnail settings.".into()
    } else if registry_installed && (!shellex_ok || !dll_path_matches || !approved_ok) {
        "BLPView registration looks incomplete. Reinstall BLPView.".into()
    } else {
        "Install BLPView to show .blp thumbnails directly in Windows Explorer.".into()
    };

    BlpViewStatus {
        installed,
        dll_path: dll_path.display().to_string(),
        supported: true,
        message,
    }
}
