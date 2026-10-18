---
type: vendor-doc
source_repo: "https://github.com/danielsreichenbach/mopaq"
source_path: "mopaq/src/compression/algorithms/mod.rs"
ref: HEAD
---

//! Compression algorithm implementations

pub(super) mod adpcm;
pub(super) mod bzip2;
pub(super) mod huffman;
pub(super) mod implode;
pub(super) mod lzma;
pub(super) mod pkware;
pub(super) mod sparse;
pub(super) mod zlib;
