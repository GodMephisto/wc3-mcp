---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/chunk/ribb.rs"
ref: HEAD
---

use super::utils::Tag;
use super::*;
use crate::encoder::error::Error as EncodeError;
use crate::parser::primitives::parse_all;
use crate::parser::Parser;
use crate::types::{emitter::RibbonEmitter, materialize::Materialized};
use nom::error::context;

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Ribb {
    pub emitters: Vec<RibbonEmitter>,
}

impl Chunk for Ribb {
    fn tag() -> Tag {
        Tag([0x52, 0x49, 0x42, 0x42]) // RIBB
    }
}

impl Materialized for Ribb {
    type Version = u32;

    fn parse_versioned(version: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, _) = context("RIBB header", Self::expect_header)(input)?;
        let (input, emitters) = context(
            "emitters",
            parse_all(|input| Materialized::parse_versioned(version, input)),
        )(input)?;
        Ok((input, Ribb { emitters }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_chunk::<Self, _>(|output| encode_fixed_vec(&self.emitters)(output))(output)
    }
}