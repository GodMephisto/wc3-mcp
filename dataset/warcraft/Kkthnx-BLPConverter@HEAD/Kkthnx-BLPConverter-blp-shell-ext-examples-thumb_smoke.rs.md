---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "blp-shell-ext/examples/thumb_smoke.rs"
ref: HEAD
---

//! Smoke-test the BLPView thumbnail COM provider outside Explorer.
//!
//! Usage:
//!   cargo run -p blp-shell-ext --example thumb_smoke -- "C:\path\file.blp"

use std::env;

use blp_shell_ext::CLSID_BLP_THUMB;
use windows::core::{Interface, PCWSTR};
use windows::Win32::Graphics::Gdi::{DeleteObject, HBITMAP};
use windows::Win32::System::Com::{
    CoCreateInstance, CoInitializeEx, CLSCTX_INPROC_SERVER, COINIT_APARTMENTTHREADED,
};
use windows::Win32::UI::Shell::{SHCreateStreamOnFileEx, IThumbnailProvider, SHCreateItemFromParsingName, IShellItem, IInitializeWithItem, SIGDN_FILESYSPATH};
use windows::Win32::UI::Shell::PropertiesSystem::{IInitializeWithFile, IInitializeWithStream};

use windows::Win32::UI::Shell::WTSAT_ARGB;

fn wide(path: &str) -> Vec<u16> {
    use std::os::windows::ffi::OsStrExt;
    std::ffi::OsStr::new(path)
        .encode_wide()
        .chain(std::iter::once(0))
        .collect()
}

fn main() {
    let path = env::args()
        .nth(1)
        .expect("usage: thumb_smoke <file.blp>");

    unsafe {
        let _ = CoInitializeEx(None, COINIT_APARTMENTTHREADED);
    }

    println!("Testing BLPView thumbnail for:\n  {path}\n");

    if let Err(err) = test_with_file(&path) {
        eprintln!("IInitializeWithFile failed: {err}");
    } else {
        println!("IInitializeWithFile: OK");
    }

    if let Err(err) = test_with_item(&path) {
        eprintln!("IInitializeWithItem failed: {err}");
    } else {
        println!("IInitializeWithItem: OK");
    }

    if let Err(err) = test_with_stream(&path) {
        eprintln!("IInitializeWithStream failed: {err}");
    } else {
        println!("IInitializeWithStream: OK");
    }
}

fn test_with_file(path: &str) -> windows::core::Result<()> {
    unsafe {
        let provider: IThumbnailProvider = CoCreateInstance(&CLSID_BLP_THUMB, None, CLSCTX_INPROC_SERVER)?;
        let init: IInitializeWithFile = provider.cast()?;
        let wpath = wide(path);
        init.Initialize(PCWSTR(wpath.as_ptr()), 0x2)?; // STGM_READ
        get_thumb(&provider, 256)
    }
}

fn test_with_stream(path: &str) -> windows::core::Result<()> {
    unsafe {
        let provider: IThumbnailProvider =
            CoCreateInstance(&CLSID_BLP_THUMB, None, CLSCTX_INPROC_SERVER)?;
        let init: IInitializeWithStream = provider.cast()?;
        let wpath = wide(path);
        let stream = SHCreateStreamOnFileEx(
            PCWSTR(wpath.as_ptr()),
            0x0,
            0,
            false,
            None,
        )?;
        init.Initialize(&stream, 0x2)?;
        get_thumb(&provider, 256)
    }
}

fn test_with_item(path: &str) -> windows::core::Result<()> {
    unsafe {
        let wpath = wide(path);
        let item: IShellItem =
            SHCreateItemFromParsingName(PCWSTR(wpath.as_ptr()), None)?;
        let provider: IThumbnailProvider = CoCreateInstance(&CLSID_BLP_THUMB, None, CLSCTX_INPROC_SERVER)?;
        let init: IInitializeWithItem = provider.cast()?;
        init.Initialize(&item, 0x2)?;
        let display = item.GetDisplayName(SIGDN_FILESYSPATH)?;
        let shown = widestring::U16CStr::from_ptr_str(display.0).to_string_lossy();
        windows::Win32::System::Com::CoTaskMemFree(Some(display.0 as *const _));
        println!("  Shell item path: {shown}");
        get_thumb(&provider, 256)
    }
}

unsafe fn get_thumb(provider: &IThumbnailProvider, cx: u32) -> windows::core::Result<()> {
    let mut hbmp = HBITMAP::default();
    let mut alpha = WTSAT_ARGB;
    provider.GetThumbnail(cx, &mut hbmp, &mut alpha)?;
    if hbmp.0.is_null() {
        return Err(windows::core::Error::from(windows::Win32::Foundation::E_FAIL));
    }
    println!("  GetThumbnail({cx}): OK (hbmp={:?}, alpha={alpha:?})", hbmp.0);
    let _ = DeleteObject(hbmp.into());
    Ok(())
}
