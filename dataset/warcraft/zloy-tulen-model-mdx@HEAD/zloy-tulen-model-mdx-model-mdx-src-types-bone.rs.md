---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/bone.rs"
ref: HEAD
---

use super::materialize::*;
use super::node::Node;

// Bone {
//     Node node
//     uint32 geosetId
//     uint32 geosetAnimationId
//   }

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Bone {
    pub node: Node,
    pub geoset_id: u32,
    pub geoset_animation_id: u32,
}

impl Materialized for Bone {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, node) = context("node", Materialized::parse)(input)?;
        let (input, geoset_id) = context("geoset_id", Materialized::parse)(input)?;
        let (input, geoset_animation_id) = context("geoset_animation_id", Materialized::parse)(input)?;
        Ok((input, Bone {
            node, geoset_id, geoset_animation_id
        }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.node.encode(output)?;
        self.geoset_id.encode(output)?;
        self.geoset_animation_id.encode(output)
    }
}