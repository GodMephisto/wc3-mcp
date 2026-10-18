---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/chunk/cams.rs"
ref: HEAD
---

use super::utils::*;
use super::*;
use crate::encoder::error::Error as EncodeError;
use crate::parser::primitives::parse_all;
use crate::parser::Parser;
use crate::types::camera::Camera;
use crate::types::materialize::*;

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Cams {
    pub cameras: Vec<Camera>,
}

impl Chunk for Cams {
    fn tag() -> Tag {
        Tag([0x43, 0x41, 0x4d, 0x53]) // CAMS
    }
}

impl Materialized for Cams {
    type Version = u32;

    fn parse_versioned(version: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, _) = context("CAMS header", Self::expect_header)(input)?;
        let (input, cameras) = context(
            "cameras",
            parse_all(|input| Materialized::parse_versioned(version, input)),
        )(input)?;
        Ok((input, Cams { cameras }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_chunk::<Self, _>(|output| encode_fixed_vec(&self.cameras)(output))(output)
    }
}
