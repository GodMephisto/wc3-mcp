---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/image-blp"
source_path: "image-blp/src/parser/types.rs"
ref: HEAD
---

pub use super::error::Error;
use nom::IResult;

/// Binary parser for BLP format that produces [Error] when something went wrong
pub type Parser<'a, T> = IResult<&'a [u8], T, Error<&'a [u8]>>;
