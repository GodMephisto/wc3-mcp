---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/chunk/corn.rs"
ref: HEAD
---

use super::Chunk;
use crate::encoder::error::Error as EncodeError;
use crate::parser::Parser;
use crate::types::materialize::Materialized;
use super::utils::Tag;

#[derive(Debug, Clone, PartialEq, Eq, PartialOrd, Ord, Hash)]
pub struct Corn {}

impl Chunk for Corn {
    fn tag() -> Tag {
        Tag([0x43, 0x4f, 0x52, 0x4e]) // CORN
    }
}

impl Materialized for Corn {
    type Version = u32;

    fn parse_versioned(version: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        unimplemented!();
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        unimplemented!();
    }
}
