---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "blp-shell-ext/src/decode.rs"
ref: HEAD
---

use image_blp::convert::blp_to_image;
use image_blp::parser::load_blp_from_buf;

pub fn decode_blp_rgba(data: &[u8]) -> Result<(u32, u32, Vec<u8>), ()> {
    let blp = load_blp_from_buf(data).map_err(|_| ())?;
    let image = blp_to_image(&blp, 0).map_err(|_| ())?;
    let rgba = image.to_rgba8();
    let (width, height) = rgba.dimensions();
    Ok((width, height, rgba.into_raw()))
}
