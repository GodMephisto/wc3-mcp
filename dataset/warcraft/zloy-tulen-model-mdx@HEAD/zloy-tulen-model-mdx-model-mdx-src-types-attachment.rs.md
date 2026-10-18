---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/attachment.rs"
ref: HEAD
---

use super::chunk::utils::*;
use super::chunk::*;
use super::materialize::*;
use super::node::*;
use super::tracks::*;
use log::*;

// Attachment {
//   uint32 inclusiveSize
//   Node node
//   char[260] path
//   uint32 attachmentId
//   (KATV)
// }
// KATV: float visibility

/// Maximum length of path field of [Attachment]
pub const ATCH_PATH_LEN: usize = 260;

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Attachment {
    pub node: Node,
    pub path: Literal<ATCH_PATH_LEN>,
    pub attachment_id: u32,
    pub katv: Option<Katv>,
}

impl Materialized for Attachment {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        parse_inclusive_sized(|input| {
            let (input, node) = context("node", Materialized::parse)(input)?;
            let (input, path) = context("path", Materialized::parse)(input)?;
            let (input, attachment_id) = context("attachment_id", Materialized::parse)(input)?;
            let mut katv: Option<Katv> = None;
            let (input, _) = parse_tagged(|tag, input| {
                if tag == Katv::tag() {
                    let (input, chunk) = context("KATV chunk", Materialized::parse)(input)?;
                    katv = Some(chunk);
                    Ok((input, false))
                } else {
                    let found: String = format!("{}", tag);
                    error!("Unknown tag {}", found);
                    return Err(nom::Err::Failure(ParseError::UnknownTag(found)));
                }
            })(input)?;
            Ok((
                input,
                Attachment {
                    node,
                    path,
                    attachment_id,
                    katv,
                },
            ))
        })(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_inclusive_sized(output, |output| {
            self.node.encode(output)?;
            self.path.encode(output)?;
            self.attachment_id.encode(output)?;
            if let Some(v) = &self.katv {
                v.encode(output)?;
            }
            Ok(())
        })
    }
}

/// Holds `visibility`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Katv(TrackChunk<f32>);

impl Chunk for Katv {
    fn tag() -> Tag {
        Tag([0x4B, 0x41, 0x54, 0x56]) // KATV
    }
}

impl Materialized for Katv {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KATV track", Materialized::parse)(input)?;
        Ok((input, Katv(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}
