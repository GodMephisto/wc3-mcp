---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/chunk/txan.rs"
ref: HEAD
---

use super::utils::Tag;
use super::*;
use crate::encoder::error::Error as EncodeError;
use crate::parser::primitives::parse_all;
use crate::parser::Parser;
use crate::types::{materialize::Materialized, texture::TextureAnimation};
use nom::error::context;

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Txan {
    pub animations: Vec<TextureAnimation>,
}

impl Chunk for Txan {
    fn tag() -> Tag {
        Tag([0x54, 0x58, 0x41, 0x4e]) // TXAN
    }
}

impl Materialized for Txan {
    type Version = u32;

    fn parse_versioned(version: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, _) = context("TXAN header", Self::expect_header)(input)?;
        let (input, animations) = context(
            "animations",
            parse_all(|input| Materialized::parse_versioned(version, input)),
        )(input)?;
        Ok((input, Txan { animations }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_chunk::<Self, _>(|output| encode_fixed_vec(&self.animations)(output))(output)
    }
}
