---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src-tauri/src/shell_extension/conflict.rs"
ref: HEAD
---

use std::path::Path;

use blp_shell_ext::{GuidExt, SHELL_THUMB_HANDLER_CATID};
use winreg::enums::HKEY_LOCAL_MACHINE;
use winreg::RegKey;

/// Legacy machine-wide BLPView thumbnail handler (often left behind after uninstall).
pub const LEGACY_BLPVIEW_CLSID: &str = "{E0122E63-E244-47F5-A5D6-DA122FEEB170}";
pub const LEGACY_BLPVIEW_DLL: &str = r"C:\Program Files\BLPView\BLPView64.dll";

pub struct LegacyBlpViewConflict {
    pub machine_handler_registered: bool,
    pub legacy_dll_missing: bool,
}

pub fn detect_legacy_blpview_conflict() -> LegacyBlpViewConflict {
    let thumb_catid = SHELL_THUMB_HANDLER_CATID.to_braced_upper();
    let machine = RegKey::predef(HKEY_LOCAL_MACHINE);

    let machine_handler_registered = machine
        .open_subkey(format!(
            r"Software\Classes\.blp\ShellEx\{thumb_catid}"
        ))
        .ok()
        .and_then(|key| key.get_value::<String, _>("").ok())
        .map(|value| value.eq_ignore_ascii_case(LEGACY_BLPVIEW_CLSID))
        .unwrap_or(false);

    let legacy_dll_missing = !Path::new(LEGACY_BLPVIEW_DLL).exists();

    LegacyBlpViewConflict {
        machine_handler_registered,
        legacy_dll_missing,
    }
}

pub fn legacy_conflict_message() -> Option<&'static str> {
    let conflict = detect_legacy_blpview_conflict();
    if conflict.machine_handler_registered && conflict.legacy_dll_missing {
        Some(
            "A broken legacy BLPView handler is still registered in Windows (machine-wide). \
             This can block Explorer thumbnails. Remove the old BLPView entry from the \
             Apps list in Windows Settings, or uninstall it with admin rights, then reinstall BLPView here.",
        )
    } else {
        None
    }
}
