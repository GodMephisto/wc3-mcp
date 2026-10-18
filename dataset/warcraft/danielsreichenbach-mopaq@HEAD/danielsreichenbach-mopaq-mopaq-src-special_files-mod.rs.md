---
type: vendor-doc
source_repo: "https://github.com/danielsreichenbach/mopaq"
source_path: "mopaq/src/special_files/mod.rs"
ref: HEAD
---

//! Special MPQ files handling: (listfile), (attributes), (signature), etc.

mod attributes;
mod info;
mod listfile;

pub use attributes::{AttributeFlags, Attributes, FileAttributes};
pub use info::{get_special_file_info, SpecialFileInfo};
pub use listfile::parse_listfile;
