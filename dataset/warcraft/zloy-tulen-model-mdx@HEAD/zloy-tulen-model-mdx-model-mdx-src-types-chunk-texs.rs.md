---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/chunk/texs.rs"
ref: HEAD
---

use super::utils::Tag;
use super::*;
use crate::encoder::error::Error as EncodeError;
use crate::parser::Parser;
use crate::types::materialize::Materialized;
use crate::types::texture::{Texture, TEXTURE_SIZE};

#[derive(Debug, Clone, PartialEq, Eq, PartialOrd, Ord, Hash)]
pub struct Texs {
    pub textures: Vec<Texture>,
}

impl Chunk for Texs {
    fn tag() -> Tag {
        Tag([0x54, 0x45, 0x58, 0x53]) // TEXS
    }
}

impl Materialized for Texs {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, textures) = parse_fixed_elements_chunk::<TEXTURE_SIZE, Self, _>(input)?;
        Ok((input, Self { textures }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_chunk::<Self, _>(|output| encode_fixed_vec(&self.textures)(output))(output)
    }
}
