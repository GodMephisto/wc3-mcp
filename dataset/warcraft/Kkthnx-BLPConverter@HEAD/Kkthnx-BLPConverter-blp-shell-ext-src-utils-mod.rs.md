---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "blp-shell-ext/src/utils/mod.rs"
ref: HEAD
---

mod create_hbitmap_bgra;
mod resize_fit_rgba;
mod rgba_to_bgra;

pub use create_hbitmap_bgra::create_hbitmap_bgra;
pub use resize_fit_rgba::resize_fit_rgba;
pub use rgba_to_bgra::rgba_to_bgra;
