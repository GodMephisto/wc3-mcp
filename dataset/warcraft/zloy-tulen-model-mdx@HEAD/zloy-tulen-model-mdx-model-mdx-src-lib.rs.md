---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/lib.rs"
ref: HEAD
---

/// Defines parsed structure of MDX model
pub mod types;
/// Decodes bytes of MDX model into typed structure
pub mod parser;
/// Encodes in memory MDX into byte stream
pub mod encoder;

pub use types::*;