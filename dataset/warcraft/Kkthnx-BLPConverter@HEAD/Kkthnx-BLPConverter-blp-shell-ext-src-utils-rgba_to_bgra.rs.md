---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "blp-shell-ext/src/utils/rgba_to_bgra.rs"
ref: HEAD
---

/// Convert straight (non-premultiplied) RGBA to BGRA for WTSAT_ARGB thumbnails.
pub fn rgba_to_bgra(rgba: &[u8]) -> Vec<u8> {
    let pixels = rgba.len() / 4;
    let mut out = vec![0u8; rgba.len()];
    for p in 0..pixels {
        out[p * 4] = rgba[p * 4 + 2];
        out[p * 4 + 1] = rgba[p * 4 + 1];
        out[p * 4 + 2] = rgba[p * 4];
        out[p * 4 + 3] = rgba[p * 4 + 3];
    }
    out
}
