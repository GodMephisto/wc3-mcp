---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "blp-shell-ext/src/utils/resize_fit_rgba.rs"
ref: HEAD
---

use image::imageops::FilterType;
use image::RgbaImage;

pub fn resize_fit_rgba(rgba: &[u8], width: u32, height: u32, max_dim: u32) -> (u32, u32, Vec<u8>) {
    if width.max(height) <= max_dim {
        return (width, height, rgba.to_vec());
    }

    let scale = max_dim as f32 / width.max(height) as f32;
    let target_w = ((width as f32 * scale).round() as u32).max(1);
    let target_h = ((height as f32 * scale).round() as u32).max(1);

    let Some(source) = RgbaImage::from_raw(width, height, rgba.to_vec()) else {
        return (width, height, rgba.to_vec());
    };

    let resized = image::imageops::resize(&source, target_w, target_h, FilterType::Triangle);
    (target_w, target_h, resized.into_raw())
}
