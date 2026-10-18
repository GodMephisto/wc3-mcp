---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/chunk/prem.rs"
ref: HEAD
---

use super::utils::Tag;
use super::*;
use crate::encoder::error::Error as EncodeError;
use crate::parser::primitives::parse_all;
use crate::parser::Parser;
use crate::types::{emitter::ParticleEmitter, materialize::Materialized};
use nom::error::context;

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Prem {
    pub emitters: Vec<ParticleEmitter>,
}

impl Chunk for Prem {
    fn tag() -> Tag {
        Tag([0x50, 0x52, 0x45, 0x4d]) // PREM
    }
}

impl Materialized for Prem {
    type Version = u32;

    fn parse_versioned(version: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, _) = context("PREM header", Self::expect_header)(input)?;
        let (input, emitters) = context(
            "emitters",
            parse_all(|input| Materialized::parse_versioned(version, input)),
        )(input)?;
        Ok((input, Prem { emitters }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_chunk::<Self, _>(|output| encode_fixed_vec(&self.emitters)(output))(output)
    }
}