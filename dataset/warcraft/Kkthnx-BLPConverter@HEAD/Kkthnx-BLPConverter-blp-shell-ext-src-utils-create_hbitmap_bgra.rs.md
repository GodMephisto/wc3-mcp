---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "blp-shell-ext/src/utils/create_hbitmap_bgra.rs"
ref: HEAD
---

use std::ptr::null_mut;

use windows::Win32::Foundation::{E_FAIL, E_INVALIDARG};
use windows::Win32::Graphics::Gdi::{
    CreateDIBSection, BITMAPINFO, BITMAPV5HEADER, BI_BITFIELDS, DIB_RGB_COLORS, DeleteObject,
    HBITMAP,
};

const LCS_SRGB_U32: u32 = 0x7352_4742;

/// Build a 32bpp top-down BGRA DIB section for an `WTSAT_ARGB` thumbnail.
///
/// The pixels are straight (non-premultiplied) BGRA. That matches the Windows
/// SDK RecipeThumbnailProvider sample, which hands the shell a `32bppBGRA`
/// bitmap with `WTSAT_ARGB` and does not premultiply. Do not premultiply here.
pub unsafe fn create_hbitmap_bgra(
    width: i32,
    height: i32,
    pixels_bgra: &[u8],
) -> windows::core::Result<HBITMAP> {
    use core::ffi::c_void;
    use core::mem::{size_of, zeroed};
    use core::ptr::copy_nonoverlapping;

    let mut v5: BITMAPV5HEADER = zeroed();
    v5.bV5Size = size_of::<BITMAPV5HEADER>() as u32;
    v5.bV5Width = width;
    v5.bV5Height = -height;
    v5.bV5Planes = 1;
    v5.bV5BitCount = 32;
    v5.bV5Compression = BI_BITFIELDS;
    v5.bV5RedMask = 0x00FF_0000;
    v5.bV5GreenMask = 0x0000_FF00;
    v5.bV5BlueMask = 0x0000_00FF;
    v5.bV5AlphaMask = 0xFF00_0000;
    v5.bV5CSType = LCS_SRGB_U32;

    let mut bits: *mut c_void = null_mut();
    let hbmp = CreateDIBSection(
        None,
        &*(&v5 as *const BITMAPV5HEADER as *const BITMAPINFO),
        DIB_RGB_COLORS,
        &mut bits,
        None,
        0,
    )?;

    if bits.is_null() {
        let _ = DeleteObject(hbmp.into());
        return Err(windows::core::Error::from(E_FAIL));
    }

    let expected = (width as usize) * (height as usize) * 4;
    if pixels_bgra.len() != expected {
        let _ = DeleteObject(hbmp.into());
        return Err(windows::core::Error::from(E_INVALIDARG));
    }

    copy_nonoverlapping(pixels_bgra.as_ptr(), bits as *mut u8, expected);
    Ok(hbmp)
}
