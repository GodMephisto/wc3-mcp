---
type: vendor-doc
source_repo: "https://github.com/WarRaft/blp-lib"
source_path: "src/lib.rs"
ref: HEAD
---

use std::ffi::CStr;
use std::os::raw::{c_char, c_int, c_uint, c_void};
use std::ptr;
use std::slice;

// Import core structures and functions from blp crate
use blp::core::image::ImageBlp;

/// Structure for storing BLP image in C-compatible format
#[repr(C)]
pub struct BlpImage {
    width: c_uint,
    height: c_uint,
    data: *mut u8,
    data_len: c_uint,
}

/// Operation result
#[repr(C)]
#[derive(Debug, PartialEq)]
pub enum BlpResult {
    Success = 0,
    InvalidInput = -1,
    ParseError = -2,
    MemoryError = -3,
    UnknownError = -99,
}

/// Loads BLP file from data buffer
///
/// # Parameters
/// * `data` - pointer to BLP file data
/// * `data_len` - data length in bytes
/// * `out_image` - pointer to BlpImage structure for storing result
///
/// # Returns
/// BlpResult::Success on success, error code on failure
#[unsafe(no_mangle)]
pub extern "C" fn blp_load_from_buffer(
    data: *const u8,
    data_len: c_uint,
    out_image: *mut BlpImage,
) -> BlpResult {
    if data.is_null() || out_image.is_null() || data_len == 0 {
        return BlpResult::InvalidInput;
    }

    let buffer = unsafe { slice::from_raw_parts(data, data_len as usize) };

    match ImageBlp::from_buf(buffer) {
        Ok(mut image) => {
            // Decode only base mip by default
            // MAX_MIPS = 16 in the blp crate. We request only the top level.
            let mut mip_visible = [false; 16];
            mip_visible[0] = true;
            match image.decode(buffer, &mip_visible) {
                Ok(_) => {
                    // Get the first (main) mip
                    if let Some(mip) = image.mipmaps.get(0) {
                        if let Some(rgba_image) = &mip.image {
                            let rgba_data = rgba_image.as_raw();
                            let data_len = rgba_data.len();

                            // Allocate memory for image data
                            let data_ptr = unsafe {
                                libc::malloc(data_len) as *mut u8
                            };

                            if data_ptr.is_null() {
                                return BlpResult::MemoryError;
                            }

                            // Copy data
                            unsafe {
                                ptr::copy_nonoverlapping(rgba_data.as_ptr(), data_ptr, data_len);

                                (*out_image).width = image.width;
                                (*out_image).height = image.height;
                                (*out_image).data = data_ptr;
                                (*out_image).data_len = data_len as c_uint;
                            }

                            BlpResult::Success
                        } else {
                            BlpResult::ParseError
                        }
                    } else {
                        BlpResult::ParseError
                    }
                },
                Err(_) => BlpResult::ParseError,
            }
        },
        Err(_) => BlpResult::ParseError,
    }
}

/// Loads BLP file from filesystem
///
/// # Parameters
/// * `filename` - path to BLP file (null-terminated string)
/// * `out_image` - pointer to BlpImage structure for storing result
///
/// # Returns
/// BlpResult::Success on success, error code on failure
#[unsafe(no_mangle)]
pub extern "C" fn blp_load_from_file(
    filename: *const c_char,
    out_image: *mut BlpImage,
) -> BlpResult {
    if filename.is_null() || out_image.is_null() {
        return BlpResult::InvalidInput;
    }

    let c_str = unsafe { CStr::from_ptr(filename) };
    let rust_str = match c_str.to_str() {
        Ok(s) => s,
        Err(_) => return BlpResult::InvalidInput,
    };

    let data = match std::fs::read(rust_str) {
        Ok(data) => data,
        Err(_) => return BlpResult::ParseError,
    };

    blp_load_from_buffer(data.as_ptr(), data.len() as c_uint, out_image)
}

/// Frees memory allocated for BlpImage
///
/// # Parameters
/// * `image` - pointer to BlpImage structure to free
#[unsafe(no_mangle)]
pub extern "C" fn blp_free_image(image: *mut BlpImage) {
    if image.is_null() {
        return;
    }

    unsafe {
        let img = &mut *image;
        if !img.data.is_null() {
            libc::free(img.data as *mut c_void);
            img.data = ptr::null_mut();
            img.data_len = 0;
        }
    }
}

/// Gets library version information
///
/// # Returns
/// Pointer to version string (static memory, no need to free)
#[unsafe(no_mangle)]
pub extern "C" fn blp_get_version() -> *const c_char {
    // Use CARGO_PKG_VERSION env var set at build time
    static VERSION: &str = concat!("blp-lib ", env!("CARGO_PKG_VERSION"), "\0");
    VERSION.as_ptr() as *const c_char
}

/// Checks if data buffer is a valid BLP file
///
/// # Parameters
/// * `data` - pointer to data to check
/// * `data_len` - data length in bytes
///
/// # Returns
/// 1 if data is a valid BLP file, 0 otherwise
#[unsafe(no_mangle)]
pub extern "C" fn blp_is_valid(data: *const u8, data_len: c_uint) -> c_int {
    if data.is_null() || data_len == 0 {
        return 0;
    }

    let buffer = unsafe { slice::from_raw_parts(data, data_len as usize) };

    match ImageBlp::from_buf(buffer) {
        Ok(_) => 1,
        Err(_) => 0,
    }
}

// === Additional decode/extract helpers ===

/// Decode a specific mip level to a PNG file (from in-memory .blp bytes).
#[unsafe(no_mangle)]
pub extern "C" fn blp_decode_mip_to_png_from_buffer(
    blp_data: *const u8,
    blp_len: c_uint,
    mip_index: c_uint,
    output_png_path: *const c_char,
) -> BlpResult {
    if blp_data.is_null() || blp_len == 0 || output_png_path.is_null() {
        return BlpResult::InvalidInput;
    }
    let buf = unsafe { slice::from_raw_parts(blp_data, blp_len as usize) };
    let mut img = match ImageBlp::from_buf(buf) {
        Ok(i) => i,
        Err(_) => return BlpResult::ParseError,
    };

    let mi = mip_index as usize;
    // Build visibility mask for only the requested mip
    let mut flags = [false; 16];
    if mi < 16 { flags[mi] = true; } else { return BlpResult::InvalidInput; }
    if let Err(_) = img.decode(buf, &flags) { return BlpResult::ParseError; }

    if mi >= img.mipmaps.len() { return BlpResult::InvalidInput; }
    let mip = &img.mipmaps[mi];

    let out_path = unsafe { CStr::from_ptr(output_png_path) };
    let out_str = match out_path.to_str() { Ok(s) => s, Err(_) => return BlpResult::InvalidInput };

    match img.export_png(mip, std::path::Path::new(out_str)) {
        Ok(_) => BlpResult::Success,
        Err(_) => BlpResult::UnknownError,
    }
}

/// Decode a specific mip level to a PNG file (from .blp file on disk).
#[unsafe(no_mangle)]
pub extern "C" fn blp_decode_mip_to_png_from_file(
    blp_path: *const c_char,
    mip_index: c_uint,
    output_png_path: *const c_char,
) -> BlpResult {
    if blp_path.is_null() || output_png_path.is_null() { return BlpResult::InvalidInput; }
    let in_path = unsafe { CStr::from_ptr(blp_path) };
    let in_str = match in_path.to_str() { Ok(s) => s, Err(_) => return BlpResult::InvalidInput };
    let data = match std::fs::read(in_str) { Ok(d) => d, Err(_) => return BlpResult::ParseError };
    blp_decode_mip_to_png_from_buffer(data.as_ptr(), data.len() as c_uint, mip_index, output_png_path)
}

/// Extract (without decoding) a specific mip as a raw JPEG file (only for JPEG-BLP) from in-memory bytes.
#[unsafe(no_mangle)]
pub extern "C" fn blp_extract_mip_to_jpg_from_buffer(
    blp_data: *const u8,
    blp_len: c_uint,
    mip_index: c_uint,
    output_jpg_path: *const c_char,
) -> BlpResult {
    if blp_data.is_null() || blp_len == 0 || output_jpg_path.is_null() {
        return BlpResult::InvalidInput;
    }
    let buf = unsafe { slice::from_raw_parts(blp_data, blp_len as usize) };
    let img = match ImageBlp::from_buf(buf) {
        Ok(i) => i,
        Err(_) => return BlpResult::ParseError,
    };

    let mi = mip_index as usize;
    if mi >= img.mipmaps.len() { return BlpResult::InvalidInput; }
    let mip = &img.mipmaps[mi];

    let out_path = unsafe { CStr::from_ptr(output_jpg_path) };
    let out_str = match out_path.to_str() { Ok(s) => s, Err(_) => return BlpResult::InvalidInput };
    match img.export_jpg(mip, buf, std::path::Path::new(out_str)) {
        Ok(_) => BlpResult::Success,
        Err(_) => BlpResult::ParseError,
    }
}

/// Extract (without decoding) a specific mip as a raw JPEG file (only for JPEG-BLP) from file on disk.
#[unsafe(no_mangle)]
pub extern "C" fn blp_extract_mip_to_jpg_from_file(
    blp_path: *const c_char,
    mip_index: c_uint,
    output_jpg_path: *const c_char,
) -> BlpResult {
    if blp_path.is_null() || output_jpg_path.is_null() { return BlpResult::InvalidInput; }
    let in_path = unsafe { CStr::from_ptr(blp_path) };
    let in_str = match in_path.to_str() { Ok(s) => s, Err(_) => return BlpResult::InvalidInput };
    let data = match std::fs::read(in_str) { Ok(d) => d, Err(_) => return BlpResult::ParseError };
    blp_extract_mip_to_jpg_from_buffer(data.as_ptr(), data.len() as c_uint, mip_index, output_jpg_path)
}

/// Helper: build mip visibility vector from count (first `mip_count` = true)
fn build_mip_visibility_from_count(mip_count: usize) -> [bool; 16] {
    let mut flags = [false; 16];
    let n = core::cmp::min(mip_count, 16);
    for i in 0..n { flags[i] = true; }
    flags
}

/// Encodes an input image file (PNG/JPEG/etc.) to BLP file on disk.
/// Only mips up to `mip_count` will be generated (at least 1).
/// Quality is 0..=100 (higher = better quality/larger size) when applicable.
#[unsafe(no_mangle)]
pub extern "C" fn blp_encode_file_to_blp(
    input_image_path: *const c_char,
    output_blp_path: *const c_char,
    quality: u8,
    mip_count: c_uint,
) -> BlpResult {
    if input_image_path.is_null() || output_blp_path.is_null() { return BlpResult::InvalidInput; }

    let in_path = unsafe { CStr::from_ptr(input_image_path) };
    let in_str = match in_path.to_str() { Ok(s) => s, Err(_) => return BlpResult::InvalidInput };
    let data = match std::fs::read(in_str) { Ok(d) => d, Err(_) => return BlpResult::ParseError };
    blp_encode_bytes_to_blp(
        data.as_ptr(),
        data.len() as c_uint,
        output_blp_path,
        quality,
        mip_count,
    )
}

/// Encodes an input image file (PNG/JPEG/etc.) to BLP using explicit mip flags.
/// `mip_visible` is an array of 0/1 flags; missing entries are treated as false.
#[unsafe(no_mangle)]
pub extern "C" fn blp_encode_file_to_blp_with_flags(
    input_image_path: *const c_char,
    output_blp_path: *const c_char,
    quality: u8,
    mip_visible: *const u8,
    mip_visible_len: c_uint,
) -> BlpResult {
    if input_image_path.is_null() || output_blp_path.is_null() { return BlpResult::InvalidInput; }

    let in_path = unsafe { CStr::from_ptr(input_image_path) };
    let in_str = match in_path.to_str() { Ok(s) => s, Err(_) => return BlpResult::InvalidInput };
    let data = match std::fs::read(in_str) { Ok(d) => d, Err(_) => return BlpResult::ParseError };
    blp_encode_bytes_to_blp_with_flags(
        data.as_ptr(),
        data.len() as c_uint,
        output_blp_path,
        quality,
        mip_visible,
        mip_visible_len,
    )
}

/// Encodes an image provided as encoded bytes (PNG/JPG/etc.) to BLP on disk.
/// Only mips up to `mip_count` will be generated (at least 1).
#[unsafe(no_mangle)]
pub extern "C" fn blp_encode_bytes_to_blp(
    image_bytes: *const u8,
    image_len: c_uint,
    output_blp_path: *const c_char,
    quality: u8,
    mip_count: c_uint,
) -> BlpResult {
    if image_bytes.is_null() || image_len == 0 || output_blp_path.is_null() {
        return BlpResult::InvalidInput;
    }
    let buf = unsafe { slice::from_raw_parts(image_bytes, image_len as usize) };
    let mut img = match ImageBlp::from_buf(buf) {
        Ok(i) => i,
        Err(_) => return BlpResult::ParseError,
    };
    let flags = build_mip_visibility_from_count(mip_count.max(1) as usize);
    if let Err(_) = img.decode(buf, &flags) { return BlpResult::ParseError; }

    match unsafe { CStr::from_ptr(output_blp_path) }.to_str() {
        Ok(s) => {
            if let Err(_) = img.export_blp(std::path::Path::new(s), quality, &flags) {
                return BlpResult::UnknownError;
            }
            BlpResult::Success
        },
        Err(_) => BlpResult::InvalidInput,
    }
}

/// Encodes an image provided as encoded bytes (PNG/JPG/etc.) to BLP on disk using explicit mip flags.
/// `mip_visible` is an array of 0/1 flags; missing entries are treated as false.
#[unsafe(no_mangle)]
pub extern "C" fn blp_encode_bytes_to_blp_with_flags(
    image_bytes: *const u8,
    image_len: c_uint,
    output_blp_path: *const c_char,
    quality: u8,
    mip_visible: *const u8,
    mip_visible_len: c_uint,
) -> BlpResult {
    if image_bytes.is_null() || image_len == 0 || output_blp_path.is_null() {
        return BlpResult::InvalidInput;
    }
    let buf = unsafe { slice::from_raw_parts(image_bytes, image_len as usize) };
    let mut img = match ImageBlp::from_buf(buf) {
        Ok(i) => i,
        Err(_) => return BlpResult::ParseError,
    };

    // Convert flags (0/1) to [bool; 16]
    let mut flags = [false; 16];
    let n = core::cmp::min(mip_visible_len as usize, 16);
    if n > 0 && !mip_visible.is_null() {
        let raw = unsafe { slice::from_raw_parts(mip_visible, n) };
        for i in 0..n { flags[i] = raw[i] != 0; }
    } else {
        flags[0] = true; // at least base level
    }

    if let Err(_) = img.decode(buf, &flags) { return BlpResult::ParseError; }

    match unsafe { CStr::from_ptr(output_blp_path) }.to_str() {
        Ok(s) => {
            if let Err(_) = img.export_blp(std::path::Path::new(s), quality, &flags) {
                return BlpResult::UnknownError;
            }
            BlpResult::Success
        },
        Err(_) => BlpResult::InvalidInput,
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_version() {
        let version = blp_get_version();
        assert!(!version.is_null());

        let c_str = unsafe { CStr::from_ptr(version) };
        let version_str = c_str.to_str().unwrap();
        assert!(version_str.starts_with("blp-lib"));
    }

    #[test]
    fn test_invalid_input() {
        let mut image = BlpImage {
            width: 0,
            height: 0,
            data: ptr::null_mut(),
            data_len: 0,
        };

        // Test with null pointers
        assert_eq!(
            blp_load_from_buffer(ptr::null(), 0, &mut image),
            BlpResult::InvalidInput
        );

        assert_eq!(
            blp_load_from_file(ptr::null(), &mut image),
            BlpResult::InvalidInput
        );
    }
}