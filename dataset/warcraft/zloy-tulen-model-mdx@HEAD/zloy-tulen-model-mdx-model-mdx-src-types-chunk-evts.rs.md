---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/chunk/evts.rs"
ref: HEAD
---

use super::utils::*;
use super::*;
use crate::encoder::error::Error as EncodeError;
use crate::parser::primitives::parse_all;
use crate::parser::Parser;
use crate::types::event::EventObject;
use crate::types::materialize::*;

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Evts {
    pub events: Vec<EventObject>,
}

impl Chunk for Evts {
    fn tag() -> Tag {
        Tag([0x45, 0x56, 0x54, 0x53]) // EVTS
    }
}

impl Materialized for Evts {
    type Version = u32;

    fn parse_versioned(version: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, _) = context("EVTS header", Self::expect_header)(input)?;
        let (input, events) = context(
            "events",
            parse_all(|input| Materialized::parse_versioned(version, input)),
        )(input)?;
        Ok((input, Evts { events }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_chunk::<Self, _>(|output| encode_fixed_vec(&self.events)(output))(output)
    }
}
