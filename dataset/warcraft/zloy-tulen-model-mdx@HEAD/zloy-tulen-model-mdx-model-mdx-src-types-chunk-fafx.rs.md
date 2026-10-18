---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/chunk/fafx.rs"
ref: HEAD
---

use super::Chunk;
use crate::encoder::error::Error as EncodeError;
use crate::parser::Parser;
use crate::types::materialize::Materialized;
use super::utils::Tag;

#[derive(Debug, Clone, PartialEq, Eq, PartialOrd, Ord, Hash)]
pub struct Fafx {}

impl Chunk for Fafx {
    fn tag() -> Tag {
        Tag([0x46, 0x41, 0x46, 0x58]) // FAFX
    }
}

impl Materialized for Fafx {
    type Version = u32;

    fn parse_versioned(version: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        unimplemented!();
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        unimplemented!();
    }
}