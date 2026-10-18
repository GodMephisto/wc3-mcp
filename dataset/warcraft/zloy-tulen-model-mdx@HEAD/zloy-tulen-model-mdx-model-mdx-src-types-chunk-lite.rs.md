---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/chunk/lite.rs"
ref: HEAD
---

use super::utils::Tag;
use super::{encode_chunk, Chunk};
use crate::encoder::error::Error as EncodeError;
use crate::parser::{primitives::parse_all, Parser};
use crate::types::{
    light::Light,
    materialize::{context, encode_fixed_vec, Materialized},
};

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Lite {
    pub lights: Vec<Light>,
}

impl Chunk for Lite {
    fn tag() -> Tag {
        Tag([0x4c, 0x49, 0x54, 0x45]) // LITE
    }
}

impl Materialized for Lite {
    type Version = u32;

    fn parse_versioned(version: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, _) = context("HELP header", Self::expect_header)(input)?;
        let (input, lights) = context(
            "lights",
            parse_all(|input| Materialized::parse_versioned(version, input)),
        )(input)?;
        Ok((input, Lite { lights }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_chunk::<Self, _>(|output| encode_fixed_vec(&self.lights)(output))(output)
    }
}
