---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/chunk/pivt.rs"
ref: HEAD
---

use super::utils::Tag;
use super::*;
use crate::encoder::error::Error as EncodeError;
use crate::parser::Parser;
use crate::types::materialize::*;

pub const PIVT_SIZE: usize = 12;

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Pivt {
    pub points: Vec<[f32; 3]>,
}

impl Chunk for Pivt {
    fn tag() -> Tag {
        Tag([0x50, 0x49, 0x56, 0x54]) // PIVT
    }
}

impl Materialized for Pivt {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, points) = parse_fixed_elements_chunk::<PIVT_SIZE, Pivt, _>(input)?;
        Ok((input, Pivt { points }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_chunk::<Self, _>(|output| encode_fixed_vec(&self.points)(output))(output)
    }
}
