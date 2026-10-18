---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/event.rs"
ref: HEAD
---

use super::chunk::utils::*;
use super::materialize::*;
use super::node::Node;

// EventObject {
//     Node node
//     char[4] "KEVT"
//     uint32 tracksCount
//     uint32 globalSequenceId
//     uint32[tracksCount] tracks
//   }

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct EventObject {
    pub node: Node,
    pub global_sequence_id: u32,
    pub tracks: Vec<u32>,
}

impl EventObject {
    pub fn kevt() -> Tag {
        Tag([0x4b, 0x45, 0x56, 0x54]) // KEVT
    }
}

impl Materialized for EventObject {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, node) = context("node", Materialized::parse)(input)?;
        let (input, _) = EventObject::kevt().expect(input)?;
        let (input, tracks_count): (&[u8], u32) =
            context("tracks_count", Materialized::parse)(input)?;
        let (input, global_sequence_id) =
            context("global_sequence_id", Materialized::parse)(input)?;
        let (input, tracks) = context("tracks", |input| {
            parse_fixed_vec(tracks_count as usize)(input)
        })(input)?;

        Ok((
            input,
            EventObject {
                node,
                global_sequence_id,
                tracks,
            },
        ))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.node.encode(output)?;
        EventObject::kevt().encode(output)?;
        (self.tracks.len() as u32).encode(output)?;
        self.global_sequence_id.encode(output)?;
        encode_fixed_vec(&self.tracks)(output)
    }
}
