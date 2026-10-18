---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/extent.rs"
ref: HEAD
---

use super::materialize::{EncodeError, Materialized, Parser};
use crate::parser::primitives::{le_f32, times};
use nom::error::context;

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Extent {
    pub bounds_radius: f32,
    pub minimum: [f32; 3],
    pub maximum: [f32; 3],
}

impl Materialized for Extent {
    type Version = u32;

    /// Parse the chunk from given input
    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, bounds_radius) = context("boundsRadius", le_f32)(input)?;
        let (input, minimum) = context("minimum", times::<3, f32, _>(le_f32))(input)?;
        let (input, maximum) = context("maximum", times::<3, f32, _>(le_f32))(input)?;
        Ok((
            input,
            Extent {
                bounds_radius,
                minimum,
                maximum,
            },
        ))
    }

    /// Encode the chunk to byte stream
    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        output.extend(self.bounds_radius.to_le_bytes());
        for v in self.minimum {
            output.extend(v.to_le_bytes());
        }
        for v in self.maximum {
            output.extend(v.to_le_bytes());
        }
        Ok(())
    }
}
