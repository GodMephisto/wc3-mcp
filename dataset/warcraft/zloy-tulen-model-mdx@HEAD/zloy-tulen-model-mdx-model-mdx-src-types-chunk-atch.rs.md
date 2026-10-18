---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/chunk/atch.rs"
ref: HEAD
---

use super::utils::Tag;
use super::*;
use crate::encoder::error::Error as EncodeError;
use crate::parser::{primitives::*, Parser};
use crate::types::attachment::Attachment;
use crate::types::materialize::*;

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Atch {
    pub attachments: Vec<Attachment>,
}

impl Chunk for Atch {
    fn tag() -> Tag {
        Tag([0x41, 0x54, 0x43, 0x48]) // ATCH
    }
}

impl Materialized for Atch {
    type Version = u32;

    fn parse_versioned(version: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, _) = context("ATCH header", Self::expect_header)(input)?;
        let (input, attachments) = context(
            "attachments",
            parse_all(|input| Materialized::parse_versioned(version, input)),
        )(input)?;
        Ok((input, Atch { attachments }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_chunk::<Self, _>(|output| encode_fixed_vec(&self.attachments)(output))(output)
    }
}
