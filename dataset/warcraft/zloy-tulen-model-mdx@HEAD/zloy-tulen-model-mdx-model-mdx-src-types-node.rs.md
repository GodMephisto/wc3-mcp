---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/node.rs"
ref: HEAD
---

use super::chunk::utils::*;
use super::chunk::*;
use super::materialize::*;
use super::tracks::*;
use bitflags::*;
use log::*;

bitflags! {
    pub struct NodeFlags: u32 {
        const HELPER = 0x0;
        const DONT_INHERIT_TRANSLATION = 0x1;
        const DONT_INHERIT_ROTATION = 0x2;
        const DONT_INHERIT_SCALING = 0x4;
        const BILLBOARDED = 0x8;
        const BILLBOARDED_LOCK_X = 0x10;
        const BILLBOARDED_LOCK_Y = 0x20;
        const BILLBOARDED_LOCK_Z = 0x40;
        const CAMERA_ANCHORED = 0x80;
        const BONE = 0x100;
        const LIGHT = 0x200;
        const EVENT_OBJECT = 0x400;
        const ATTACHMENT = 0x800;
        const PARTICLE_EMITTER = 0x1000;
        const COLLISION_SHAPE = 0x2000;
        const RIBBON_EMITTER = 0x4000;
        // if particle emitter: emitter uses mdl, if particle emitter 2: unshaded
        const EMITTER_MOD_1 = 0x8000;
        // if particle emitter: emitter uses tga, if particle emitter 2: sort primitives far z
        const EMITTER_MOD_2 = 0x10000;
        const LINE_EMITTER = 0x20000;
        const UNFOGGED = 0x40000;
        const MODEL_SPACE = 0x80000;
        const XY_QUAD = 0x100000;
    }
}

impl Materialized for NodeFlags {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, flag): (&[u8], u32) = Materialized::parse(input)?;
        Ok((input, NodeFlags { bits: flag }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.bits.encode(output)
    }
}

/// Maximum length of node name
pub const NODE_NAME_LEN: usize = 80;

// Node {
//     uint32 inclusiveSize
//     char[80] name
//     uint32 objectId
//     uint32 parentId
//     uint32 flags // 0x0: helper
//                  // 0x1: dont inherit translation
//                  // 0x2: dont inherit rotation
//                  // 0x4: dont inherit scaling
//                  // 0x8: billboarded
//                  // 0x10: billboarded lock x
//                  // 0x20: billboarded lock y
//                  // 0x40: billboarded lock z
//                  // 0x80: camera anchored
//                  // 0x100: bone
//                  // 0x200: light
//                  // 0x400 event object
//                  // 0x800: attachment
//                  // 0x1000 particle emitter
//                  // 0x2000: collision shape
//                  // 0x4000: ribbon emitter
//                  // 0x8000: if particle emitter: emitter uses mdl, if particle emitter 2: unshaded
//                  // 0x10000: if particle emitter: emitter uses tga, if particle emitter 2: sort primitives far z
//                  // 0x20000: line emitter
//                  // 0x40000: unfogged
//                  // 0x80000: model space
//                  // 0x100000: xy quad
//     (KGTR)
//     (KGRT)
//     (KGSC)
//   }
// KGTR: float[3] translation
// KGRT: float[4] rotation
// KGSC: float[3] scaling

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Node {
    pub name: Literal<NODE_NAME_LEN>,
    pub object_id: u32,
    pub parent_id: u32,
    pub flags: NodeFlags,
    pub kgtr: Option<Kgtr>,
    pub kgrt: Option<Kgrt>,
    pub kgsc: Option<Kgsc>,
    pub ordered: Option<Vec<Tag>>,
}

impl Materialized for Node {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        parse_inclusive_sized(|input| {
            let (input, name) = context("name", Materialized::parse)(input)?;
            let (input, object_id) = context("object_id", Materialized::parse)(input)?;
            let (input, parent_id) = context("parent_id", Materialized::parse)(input)?;
            let (input, flags) = context("flags", Materialized::parse)(input)?;
            let mut kgtr: Option<Kgtr> = None;
            let mut kgrt: Option<Kgrt> = None;
            let mut kgsc: Option<Kgsc> = None;
            let mut ordered = vec![];
            let (input, _) = parse_tagged(|tag, input| {
                if tag == Kgtr::tag() {
                    let (input, chunk) = context("KGTR chunk", Materialized::parse)(input)?;
                    kgtr = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kgrt::tag() {
                    let (input, chunk) = context("KGRT chunk", Materialized::parse)(input)?;
                    kgrt = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kgsc::tag() {
                    let (input, chunk) = context("KGSC chunk", Materialized::parse)(input)?;
                    kgsc = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else {
                    let found: String = format!("{}", tag);
                    error!("Unknown tag {}", found);
                    return Err(nom::Err::Failure(ParseError::UnknownTag(found)));
                }
            })(input)?;
            Ok((
                input,
                Node {
                    name,
                    object_id,
                    parent_id,
                    flags,
                    kgtr,
                    kgrt,
                    kgsc,
                    ordered: Some(ordered),
                },
            ))
        })(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_inclusive_sized(output, |output| {
            self.name.encode(output)?;
            self.object_id.encode(output)?;
            self.parent_id.encode(output)?;
            self.flags.encode(output)?;
            if let Some(ordered) = &self.ordered {
                for &tag in ordered {
                    if tag == Kgtr::tag() {
                        if let Some(v) = &self.kgtr {
                            v.encode(output)?;
                        }
                    } else if tag == Kgrt::tag() {
                        if let Some(v) = &self.kgrt {
                            v.encode(output)?;
                        }
                    } else if tag == Kgsc::tag() {
                        if let Some(v) = &self.kgsc {
                            v.encode(output)?;
                        }
                    } else {
                        warn!("Unknown tag {tag}, skipping...");
                    }
                }
            } else {
                if let Some(v) = &self.kgtr {
                    v.encode(output)?;
                }
                if let Some(v) = &self.kgrt {
                    v.encode(output)?;
                }
                if let Some(v) = &self.kgsc {
                    v.encode(output)?;
                }
            }
            Ok(())
        })
    }
}

/// Holds `translation`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kgtr(TrackChunk<[f32; 3]>);

impl Chunk for Kgtr {
    fn tag() -> Tag {
        Tag([0x4B, 0x47, 0x54, 0x52]) // KGTR
    }
}

impl Materialized for Kgtr {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KGTR track", Materialized::parse)(input)?;
        Ok((input, Kgtr(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `rotation`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kgrt(TrackChunk<[f32; 4]>);

impl Chunk for Kgrt {
    fn tag() -> Tag {
        Tag([0x4B, 0x47, 0x52, 0x54]) // KGRT
    }
}

impl Materialized for Kgrt {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KGRT track", Materialized::parse)(input)?;
        Ok((input, Kgrt(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `scaling`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kgsc(TrackChunk<[f32; 3]>);

impl Chunk for Kgsc {
    fn tag() -> Tag {
        Tag([0x4B, 0x47, 0x53, 0x43]) // KGSC
    }
}

impl Materialized for Kgsc {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KGSC track", Materialized::parse)(input)?;
        Ok((input, Kgsc(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}
