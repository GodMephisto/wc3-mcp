---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/chunk/glbs.rs"
ref: HEAD
---

use super::utils::*;
use super::*;
use crate::types::materialize::*;

/// Size of single global sequence ID
pub const GLOBALS_SIZE: usize = 4;

#[derive(Debug, Clone, PartialEq, Eq, PartialOrd, Ord, Hash)]
pub struct Glbs {
    pub global_sequences: Vec<u32>, 
}

impl Chunk for Glbs {
    fn tag() -> Tag {
        Tag([0x47, 0x4c, 0x42, 0x53]) // GLBS
    }
}

impl Materialized for Glbs {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, global_sequences) = parse_fixed_elements_chunk::<GLOBALS_SIZE, Self, _>(input)?;
        Ok((input, Glbs { global_sequences }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_chunk::<Self, _>(|output| encode_fixed_vec(&self.global_sequences)(output))(output)
    }
}