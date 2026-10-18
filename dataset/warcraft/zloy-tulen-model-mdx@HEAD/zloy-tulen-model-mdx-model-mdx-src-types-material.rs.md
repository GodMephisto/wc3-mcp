---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/material.rs"
ref: HEAD
---

use super::materialize::*;
use super::utils::*;
use crate::types::layer::Layer;
use nom::error::context;

// Material {
//     uint32 inclusiveSize
//     uint32 priorityPlane
//     uint32 flags
//     if (version > 800) {
//       char[80] shader
//     }
//     char[4] "LAYS"
//     uint32 layersCount
//     Layer[layersCount] layers
//   }

pub const MATERIAL_SHADER_LEN: usize = 80;
pub const LAYS_TAG: Tag = Tag([0x4C, 0x41, 0x59, 0x53]);

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Material {
    pub priority_plane: u32,
    pub flags: u32,
    pub shader: Option<Literal<MATERIAL_SHADER_LEN>>,
    pub layers: Vec<Layer>,
}

impl Materialized for Material {
    type Version = u32;

    /// Parse the chunk from given input
    fn parse_versioned(version: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        parse_inclusive_sized(|input| {
            let (input, priority_plane) = context("priority_plane", Materialized::parse)(input)?;
            let (input, flags) = context("flags", Materialized::parse)(input)?;
            let (input, shader) =
                parse_versioned_greater(version, 800, context("shader", Materialized::parse))(
                    input,
                )?;
            let (input, _) = LAYS_TAG.expect(input)?;
            let (input, layers) =
                parse_len_vec(|input| Materialized::parse_versioned(version, input))(input)?;
            Ok((
                input,
                Material {
                    priority_plane,
                    flags,
                    shader,
                    layers,
                },
            ))
        })(input)
    }

    /// Encode the chunk to byte stream
    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_inclusive_sized(output, |output| {
            self.priority_plane.encode(output)?;
            self.flags.encode(output)?;
            if let Some(v) = &self.shader {
                v.encode(output)?;
            }
            LAYS_TAG.encode(output)?;
            encode_len_vec(&self.layers, output)?;
            Ok(())
        })
    }
}
