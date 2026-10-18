---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/chunk/modl.rs"
ref: HEAD
---

use super::utils::*;
use super::*;
use crate::encoder::error::Error as EncodeError;
use crate::parser::Parser;
pub use crate::types::extent::Extent;
use crate::types::materialize::Materialized;
use nom::{error::context, number::complete::le_u32};

pub const MODL_NAME_LENGTH: usize = 80;
pub const MODL_ANIMATION_FILENAME_LENGTH: usize = 260;

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Modl {
    pub name: Literal<MODL_NAME_LENGTH>,
    pub animation_filename: Literal<MODL_ANIMATION_FILENAME_LENGTH>,
    pub extent: Extent,
    pub blend_time: u32,
}

impl Chunk for Modl {
    fn tag() -> Tag {
        Tag([0x4d, 0x4f, 0x44, 0x4c]) // MODL
    }
}

impl Materialized for Modl {
    type Version = u32;

    fn parse_versioned(version: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, _) = context("MODL header", Self::expect_header)(input)?;
        let (input, name) = context("name", Literal::<MODL_NAME_LENGTH>::parse)(input)?;
        let (input, animation_filename) = context(
            "animationFilename",
            Literal::<MODL_ANIMATION_FILENAME_LENGTH>::parse,
        )(input)?;
        let (input, extent) = context("extend", Extent::parse)(input)?;
        let (input, blend_time) = context("blendTime", le_u32)(input)?;
        Ok((
            input,
            Modl {
                name,
                animation_filename,
                extent,
                blend_time,
            },
        ))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_chunk::<Self, _>(|output| {
            self.name.encode(output)?;
            self.animation_filename.encode(output)?;
            self.extent.encode(output)?;
            self.blend_time.encode(output)
        })(output)
    }
}
