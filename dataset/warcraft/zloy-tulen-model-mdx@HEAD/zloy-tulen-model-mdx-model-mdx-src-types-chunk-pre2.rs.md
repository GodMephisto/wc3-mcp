---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/chunk/pre2.rs"
ref: HEAD
---

use super::utils::Tag;
use super::*;
use crate::encoder::error::Error as EncodeError;
use crate::parser::primitives::parse_all;
use crate::parser::Parser;
use crate::types::{emitter::ParticleEmitter2, materialize::Materialized};
use nom::error::context;


#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Pre2 {
    pub emitters: Vec<ParticleEmitter2>,
}

impl Chunk for Pre2 {
    fn tag() -> Tag {
        Tag([0x50, 0x52, 0x45, 0x32]) // PRE2
    }
}

impl Materialized for Pre2 {
    type Version = u32;

    fn parse_versioned(version: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, _) = context("PRE2 header", Self::expect_header)(input)?;
        let (input, emitters) = context(
            "emitters",
            parse_all(|input| Materialized::parse_versioned(version, input)),
        )(input)?;
        Ok((input, Pre2 { emitters }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_chunk::<Self, _>(|output| encode_fixed_vec(&self.emitters)(output))(output)
    }
}
