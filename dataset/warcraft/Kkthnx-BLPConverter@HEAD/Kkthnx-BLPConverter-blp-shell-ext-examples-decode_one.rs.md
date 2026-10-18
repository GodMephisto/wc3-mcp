---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "blp-shell-ext/examples/decode_one.rs"
ref: HEAD
---

use image::GenericImageView;
use image_blp::convert::blp_to_image;
use image_blp::parser::load_blp_from_buf;

fn main() {
    let path = std::env::args()
        .nth(1)
        .expect("usage: decode_one <path.blp>");
    let data = std::fs::read(&path).unwrap_or_else(|e| panic!("read {path}: {e}"));
    match load_blp_from_buf(&data) {
        Ok(blp) => match blp_to_image(&blp, 0) {
            Ok(image) => {
                let (w, h) = image.dimensions();
                println!("OK {w}x{h} ({} bytes)", data.len());
            }
            Err(e) => println!("IMAGE FAILED: {e} ({} bytes)", data.len()),
        },
        Err(e) => println!("BLP PARSE FAILED: {e} ({} bytes)", data.len()),
    }
}
