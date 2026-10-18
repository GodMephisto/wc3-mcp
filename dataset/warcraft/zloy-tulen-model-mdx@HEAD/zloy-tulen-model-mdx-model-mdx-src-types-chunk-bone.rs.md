---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/chunk/bone.rs"
ref: HEAD
---

use crate::types::bone::Bone;
use super::utils::*;
use super::*;
use crate::encoder::error::Error as EncodeError;
use crate::parser::primitives::parse_all;
use crate::parser::Parser;
use crate::types::materialize::*;

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct BoneChunk {
    bones: Vec<Bone>,
}

impl Chunk for BoneChunk {
    fn tag() -> Tag {
        Tag([0x42, 0x4f, 0x4e, 0x45]) // Bone
    }
}

impl Materialized for BoneChunk {
    type Version = u32;

    fn parse_versioned(version: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, _) = context("BONE header", Self::expect_header)(input)?;
        let (input, bones) = context(
            "bones",
            parse_all(|input| Materialized::parse_versioned(version, input)),
        )(input)?;
        Ok((input, BoneChunk { bones }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_chunk::<Self, _>(|output| encode_fixed_vec(&self.bones)(output))(output)
    }
}
