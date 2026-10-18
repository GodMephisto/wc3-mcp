---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/chunk/seqs.rs"
ref: HEAD
---

use super::utils::*;
use super::*;
use crate::types::materialize::*;
use crate::types::sequence::{Sequence, SEQUENCE_SIZE};

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Seqs {
    pub sequences: Vec<Sequence>,
}

impl Chunk for Seqs {
    fn tag() -> Tag {
        Tag([0x53, 0x45, 0x51, 0x53]) // SEQS
    }
}

impl Materialized for Seqs {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, sequences) = parse_fixed_elements_chunk::<SEQUENCE_SIZE, Seqs, _>(input)?;
        Ok((input, Seqs { sequences }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_chunk::<Self, _>(|output| encode_fixed_vec(&self.sequences)(output))(output)
    }
}
