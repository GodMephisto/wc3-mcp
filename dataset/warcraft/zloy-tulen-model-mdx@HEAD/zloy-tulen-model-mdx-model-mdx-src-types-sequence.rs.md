---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/sequence.rs"
ref: HEAD
---

use super::extent::Extent;
use super::materialize::{EncodeError, Materialized, Parser};
use super::utils::*;
use nom::error::context;

pub const SEQUENCE_SIZE: usize = 132;
pub const SEQUENCE_NAME_LENGTH: usize = 80;

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Sequence {
    pub name: Literal<SEQUENCE_NAME_LENGTH>,
    pub interval: [u32; 2],
    pub move_speed: f32,
    pub flags: u32, // 1 looping, 0 not looping
    pub rarity: f32,
    pub sync_point: u32,
    pub extent: Extent,
}

impl Materialized for Sequence {
    type Version = ();

    /// Parse the chunk from given input
    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, name) = context("name", Materialized::parse)(input)?;
        let (input, interval) = context("interval", Materialized::parse)(input)?;
        let (input, move_speed) = context("moveSpeed", Materialized::parse)(input)?;
        let (input, flags) = context("flags", Materialized::parse)(input)?;
        let (input, rarity) = context("rarity", Materialized::parse)(input)?;
        let (input, sync_point) = context("syncPoint", Materialized::parse)(input)?;
        let (input, extent) = context("extent", Materialized::parse)(input)?;
        Ok((
            input,
            Sequence {
                name,
                interval,
                move_speed,
                flags,
                rarity,
                sync_point,
                extent,
            },
        ))
    }

    /// Encode the chunk to byte stream
    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.name.encode(output)?;
        for i in self.interval {
            output.extend(i.to_le_bytes());
        }
        output.extend(self.move_speed.to_le_bytes());
        output.extend(self.flags.to_le_bytes());
        output.extend(self.rarity.to_le_bytes());
        output.extend(self.sync_point.to_le_bytes());
        self.extent.encode(output)?;
        Ok(())
    }
}
