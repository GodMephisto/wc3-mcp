---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JASS-Tree-sitter-Rust"
source_path: "src/lng/slk/uri_map.rs"
ref: HEAD
---

//! Deprecated — use `crate::util::tree_map` instead.
//!
//! Kept only as re-exports so that code that hasn't been migrated yet
//! continues to compile.

#[allow(unused_imports)]
pub use crate::util::tree_map::{PARSER_MAP, TREE_MAP};

