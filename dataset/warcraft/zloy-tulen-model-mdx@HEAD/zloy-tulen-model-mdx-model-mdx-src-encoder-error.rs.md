---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/encoder/error.rs"
ref: HEAD
---

use thiserror::Error;

/// Errors that occur while encoding MDX models to bytes
#[derive(Debug, Error)]
pub enum Error {
    #[error("Size {0} cannot fit into uint32!")]
    SizeUintOverflow(usize),
    #[error("Literal can handle at most {expected}, but passed {passed}")]
    LiteralSizeOverflow {
        expected: usize, 
        passed: usize,
    },
}