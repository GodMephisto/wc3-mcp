---
type: vendor-doc
source_repo: "https://github.com/echo000/casc-rs"
source_path: "casc-rs/src/block_table/block_table_entry.rs"
ref: HEAD
---

/// Represents an entry in the CASC block table.
/// Each entry describes a block of data in the storage.
#[repr(C)]
#[derive(Debug, Clone, Copy)]
pub(crate) struct BlockTableEntry {
    /// The encoded (compressed/encrypted) size of the block.
    pub(crate) encoded_size: i32,
    /// The decoded (original) content size of the block.
    pub(crate) content_size: i32,
    /// Lower 64 bits of the block's hash.
    pub(crate) hash_lower: u64,
    /// Upper 64 bits of the block's hash.
    pub(crate) hash_upper: u64,
}
