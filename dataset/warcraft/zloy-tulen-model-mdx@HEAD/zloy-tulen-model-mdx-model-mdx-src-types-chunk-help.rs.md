---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/chunk/help.rs"
ref: HEAD
---

use super::utils::Tag;
use super::{encode_chunk, Chunk};
use crate::encoder::error::Error as EncodeError;
use crate::parser::{primitives::parse_all, Parser};
use crate::types::{
    materialize::{context, encode_fixed_vec, Materialized},
    node::Node,
};

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Help {
    pub helpers: Vec<Node>,
}

impl Chunk for Help {
    fn tag() -> Tag {
        Tag([0x48, 0x45, 0x4c, 0x50]) // HELP
    }
}

impl Materialized for Help {
    type Version = u32;

    fn parse_versioned(version: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, _) = context("HELP header", Self::expect_header)(input)?;
        let (input, helpers) = context(
            "helpers",
            parse_all(|input| Materialized::parse_versioned(version, input)),
        )(input)?;
        Ok((input, Help { helpers }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_chunk::<Self, _>(|output| encode_fixed_vec(&self.helpers)(output))(output)
    }
}
