---
type: vendor-doc
source_repo: "https://github.com/danielsreichenbach/mopaq"
source_path: "mopaq/src/compression/mod.rs"
ref: HEAD
---

//! Compression and decompression algorithms for MPQ files

mod algorithms;
mod compress;
mod decompress;
mod methods;

// Re-export the main public API
pub use compress::compress;
pub use decompress::decompress;
pub use methods::{flags, CompressionMethod};
