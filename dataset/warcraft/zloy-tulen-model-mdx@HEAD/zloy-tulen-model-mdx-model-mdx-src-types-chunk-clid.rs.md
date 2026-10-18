---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/chunk/clid.rs"
ref: HEAD
---

use super::utils::*;
use super::*;
use crate::encoder::error::Error as EncodeError;
use crate::parser::primitives::parse_all;
use crate::parser::Parser;
use crate::types::collision::CollisionShape;
use crate::types::materialize::*;

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Clid {
    pub shapes: Vec<CollisionShape>,
}

impl Chunk for Clid {
    fn tag() -> Tag {
        Tag([0x43, 0x4c, 0x49, 0x44]) // CLID
    }
}

impl Materialized for Clid {
    type Version = u32;

    fn parse_versioned(version: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, _) = context("CLID header", Self::expect_header)(input)?;
        let (input, shapes) = context(
            "shapes",
            parse_all(|input| Materialized::parse_versioned(version, input)),
        )(input)?;
        Ok((input, Clid { shapes }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_chunk::<Self, _>(|output| encode_fixed_vec(&self.shapes)(output))(output)
    }
}
