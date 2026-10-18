---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/chunk/vers.rs"
ref: HEAD
---

use super::utils::*;
use super::Chunk;
use crate::encoder::error::Error as EncodeError;
use crate::encoder::primitives::push_le_u32;
use crate::parser::Parser;
use crate::types::materialize::Materialized;
use nom::{error::context, number::complete::le_u32};

#[derive(Debug, Clone, PartialEq, Eq, PartialOrd, Ord, Hash)]
pub struct Vers {
    pub version: u32,
}

impl Chunk for Vers {
    fn tag() -> Tag {
        Tag([0x56, 0x45, 0x52, 0x53]) // VERS
    }
}

impl Materialized for Vers {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, _) = context("VERS header", Self::expect_header)(input)?;
        let (input, version) = context("version", le_u32)(input)?;
        Ok((input, Vers { version }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        Self::encode_header(4, output)?;
        push_le_u32(self.version, output);
        Ok(())
    }
}
