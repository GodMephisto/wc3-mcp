---
type: vendor-doc
source_repo: "https://github.com/echo000/casc-rs"
source_path: "casc-rs/src/casc_file_frame.rs"
ref: HEAD
---

/// Represents a frame within a CASC file, describing a segment of file data.
pub(crate) struct CascFileFrame {
    /// The virtual start offset of the frame within the file.
    pub(crate) virtual_start_offset: u64,
    /// The virtual end offset of the frame within the file.
    pub(crate) virtual_end_offset: u64,
    /// The offset of the frame within the archive.
    pub(crate) archive_offset: u64,
    /// The encoded (compressed/encrypted) size of the frame.
    pub(crate) encoded_size: u32,
    /// The decoded (original) content size of the frame.
    pub(crate) content_size: u32,
}
