---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/chunk/snds.rs"
ref: HEAD
---

use super::Chunk;
use crate::encoder::error::Error as EncodeError;
use crate::parser::Parser;
use crate::types::materialize::Materialized;
use super::utils::Tag;

#[derive(Debug, Clone, PartialEq, Eq, PartialOrd, Ord, Hash)]
pub struct Snds {}

impl Chunk for Snds {
    fn tag() -> Tag {
        Tag([0x53, 0x4e, 0x44, 0x53]) // SNDS
    }
}

impl Materialized for Snds {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        unimplemented!();
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        unimplemented!();
    }
}
