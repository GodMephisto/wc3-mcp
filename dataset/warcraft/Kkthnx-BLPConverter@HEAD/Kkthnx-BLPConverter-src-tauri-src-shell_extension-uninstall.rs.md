---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src-tauri/src/shell_extension/uninstall.rs"
ref: HEAD
---

use std::io;

use blp_shell_ext::{GuidExt, CLSID_BLP_THUMB, DEFAULT_EXT, DEFAULT_PROGID, SHELL_PREVIEW_HANDLER_CATID, SHELL_THUMB_HANDLER_CATID};
use winreg::enums::{HKEY_CURRENT_USER, KEY_READ, KEY_SET_VALUE};
use winreg::RegKey;

use crate::types::BlpViewActionResult;
use super::paths::remove_installed_dlls;
use super::reg_helpers::notify_shell_assoc;

pub fn uninstall_blpview() -> Result<BlpViewActionResult, String> {
    uninstall_inner().map_err(|err| err.to_string())?;
    Ok(BlpViewActionResult {
        success: true,
        message: "BLPView shell extension removed for the current user.".into(),
        restart_required: true,
    })
}

fn uninstall_inner() -> io::Result<()> {
    let root = RegKey::predef(HKEY_CURRENT_USER);
    let thumb_clsid = CLSID_BLP_THUMB.to_braced_upper();
    let thumb_catid = SHELL_THUMB_HANDLER_CATID.to_braced_upper();
    let preview_catid = SHELL_PREVIEW_HANDLER_CATID.to_braced_upper();

    let del_tree = |path: &str| -> io::Result<()> {
        match root.delete_subkey_all(path) {
            Ok(()) => Ok(()),
            Err(e) if e.kind() == io::ErrorKind::NotFound => Ok(()),
            Err(e) => Err(e),
        }
    };

    let del_value = |key_path: &str, value_name: &str| -> io::Result<()> {
        match root.open_subkey_with_flags(key_path, KEY_READ | KEY_SET_VALUE) {
            Ok(key) => match key.delete_value(value_name) {
                Ok(()) => Ok(()),
                Err(e) if e.kind() == io::ErrorKind::NotFound => Ok(()),
                Err(e) => Err(e),
            },
            Err(e) if e.kind() == io::ErrorKind::NotFound => Ok(()),
            Err(e) => Err(e),
        }
    };

    let remove_shellex = |root_path: &str, cat: &str| -> io::Result<()> {
        del_tree(&format!(r"{root_path}\ShellEx\{cat}"))
    };

    del_value(
        r"Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved",
        thumb_clsid.as_str(),
    )?;

    remove_shellex(&format!(r"Software\Classes\{DEFAULT_EXT}"), &thumb_catid)?;
    remove_shellex(&format!(r"Software\Classes\{DEFAULT_EXT}"), &preview_catid)?;
    remove_shellex(&format!(r"Software\Classes\{DEFAULT_PROGID}"), &thumb_catid)?;
    remove_shellex(&format!(r"Software\Classes\{DEFAULT_PROGID}"), &preview_catid)?;
    remove_shellex(
        &format!(r"Software\Classes\SystemFileAssociations\{DEFAULT_EXT}"),
        &thumb_catid,
    )?;
    remove_shellex(
        &format!(r"Software\Classes\SystemFileAssociations\{DEFAULT_EXT}"),
        &preview_catid,
    )?;
    remove_shellex(
        &format!(
            r"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\{DEFAULT_EXT}"
        ),
        &thumb_catid,
    )?;
    remove_shellex(
        &format!(
            r"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\{DEFAULT_EXT}"
        ),
        &preview_catid,
    )?;

    del_value(
        r"Software\Microsoft\Windows\CurrentVersion\Explorer\ThumbnailHandlers",
        DEFAULT_EXT,
    )?;

    del_tree(&format!(r"Software\Classes\CLSID\{thumb_clsid}"))?;
    del_tree(&format!(r"Software\Classes\{DEFAULT_EXT}\PersistentHandler"))?;
    del_tree(&format!(r"Software\Classes\{DEFAULT_PROGID}"))?;
    del_tree(&format!(r"Software\Classes\{DEFAULT_EXT}"))?;

    remove_installed_dlls();

    notify_shell_assoc("uninstall");
    Ok(())
}
