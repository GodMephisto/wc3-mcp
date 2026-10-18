---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/chunk/geoa.rs"
ref: HEAD
---

use super::utils::*;
use super::*;
use crate::encoder::error::Error as EncodeError;
use crate::parser::primitives::parse_all;
use crate::parser::Parser;
use crate::types::animation::GeosetAnimation;
use crate::types::materialize::Materialized;

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Geoa {
    pub animations: Vec<GeosetAnimation>,
}

impl Chunk for Geoa {
    fn tag() -> Tag {
        Tag([0x47, 0x45, 0x4f, 0x41]) // GEOA
    }
}

impl Materialized for Geoa {
    type Version = u32;

    fn parse_versioned(version: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, _) = context("GEOA header", Self::expect_header)(input)?;
        let (input, animations) = context(
            "animations",
            parse_all(|input| Materialized::parse_versioned(version, input)),
        )(input)?;
        Ok((input, Geoa { animations }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_chunk::<Self, _>(|output| encode_fixed_vec(&self.animations)(output))(output)
    }
}
