---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/chunk/geos.rs"
ref: HEAD
---

use super::utils::*;
use super::*;
use crate::parser::primitives::parse_all;
use crate::types::geoset::*;
use crate::types::materialize::*;

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Geos {
    pub geosets: Vec<Geoset>,
}

impl Chunk for Geos {
    fn tag() -> Tag {
        Tag([0x47, 0x45, 0x4f, 0x53]) // GEOS
    }
}

impl Materialized for Geos {
    type Version = u32;

    fn parse_versioned(version: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, _) = context("GEOS header", Self::expect_header)(input)?;
        let (input, geosets) = context(
            "geosets",
            parse_all(|input| Materialized::parse_versioned(version, input)),
        )(input)?;
        Ok((input, Geos { geosets }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_chunk::<Self, _>(|output| encode_fixed_vec(&self.geosets)(output))(output)
    }
}
