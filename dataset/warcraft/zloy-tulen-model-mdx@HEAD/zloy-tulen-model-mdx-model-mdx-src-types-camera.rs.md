---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/camera.rs"
ref: HEAD
---

use super::chunk::utils::*;
use super::chunk::*;
use super::materialize::*;
use super::tracks::*;
use log::*;

// Camera {
//     uint32 inclusiveSize
//     char[80] name
//     float[3] position
//     float filedOfView
//     float farClippingPlane
//     float nearClippingPlane
//     float[3] targetPosition
//     (KCTR)
//     (KTTR)
//     (KCRL)
//   }
// KCTR: float[3] translation
// KCRL: float rotation
// KTTR: float[3] targetTranslation
pub const CAMERA_NAME_LEN: usize = 80;

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Camera {
    pub name: Literal<CAMERA_NAME_LEN>,
    pub position: [f32; 3],
    pub field_of_view: f32,
    pub far_clipping_plane: f32,
    pub near_clipping_plane: f32,
    pub target_position: [f32; 3],
    pub kctr: Option<Kctr>,
    pub kttr: Option<Kttr>,
    pub kcrl: Option<Kcrl>,
    pub ordered: Option<Vec<Tag>>,
}

impl Materialized for Camera {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        parse_inclusive_sized(|input| {
            let (input, name) = context("name", Materialized::parse)(input)?;
            let (input, position) = context("position", Materialized::parse)(input)?;
            let (input, field_of_view) = context("field_of_view", Materialized::parse)(input)?;
            let (input, far_clipping_plane) =
                context("far_clipping_plane", Materialized::parse)(input)?;
            let (input, near_clipping_plane) =
                context("near_clipping_plane", Materialized::parse)(input)?;
            let (input, target_position) = context("target_position", Materialized::parse)(input)?;
            let mut kctr: Option<Kctr> = None;
            let mut kttr: Option<Kttr> = None;
            let mut kcrl: Option<Kcrl> = None;
            let mut ordered = vec![];
            let (input, _) = parse_tagged(|tag, input| {
                if tag == Kctr::tag() {
                    let (input, chunk) = context("KCTR chunk", Materialized::parse)(input)?;
                    kctr = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kttr::tag() {
                    let (input, chunk) = context("KTTR chunk", Materialized::parse)(input)?;
                    kttr = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kcrl::tag() {
                    let (input, chunk) = context("KCRL chunk", Materialized::parse)(input)?;
                    kcrl = Some(chunk);
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
                Camera {
                    name,
                    position,
                    field_of_view,
                    far_clipping_plane,
                    near_clipping_plane,
                    target_position,
                    kctr,
                    kttr,
                    kcrl,
                    ordered: Some(ordered),
                },
            ))
        })(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_inclusive_sized(output, |output| {
            self.name.encode(output)?;
            self.position.encode(output)?;
            self.field_of_view.encode(output)?;
            self.far_clipping_plane.encode(output)?;
            self.near_clipping_plane.encode(output)?;
            self.target_position.encode(output)?;
            if let Some(ordered) = &self.ordered {
                for tag in ordered {
                    if *tag == Kctr::tag() {
                        if let Some(v) = &self.kctr {
                            v.encode(output)?;
                        }
                    } else if *tag == Kttr::tag() {
                        if let Some(v) = &self.kttr {
                            v.encode(output)?;
                        }
                    } else if *tag == Kcrl::tag() {
                        if let Some(v) = &self.kcrl {
                            v.encode(output)?;
                        }
                    } else {
                        warn!("Unknown tag {tag}, skipping...");
                    }
                }
            } else {
                if let Some(v) = &self.kctr {
                    v.encode(output)?;
                }
                if let Some(v) = &self.kttr {
                    v.encode(output)?;
                }
                if let Some(v) = &self.kcrl {
                    v.encode(output)?;
                }
            }
            Ok(())
        })
    }
}

/// Holds `translation`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kctr(TrackChunk<[f32; 3]>);

impl Chunk for Kctr {
    fn tag() -> Tag {
        Tag([0x4B, 0x43, 0x54, 0x52]) // KCTR
    }
}

impl Materialized for Kctr {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KCTR track", Materialized::parse)(input)?;
        Ok((input, Kctr(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `rotation`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kcrl(TrackChunk<f32>);

impl Chunk for Kcrl {
    fn tag() -> Tag {
        Tag([0x4B, 0x43, 0x52, 0x4c]) // KCRL
    }
}

impl Materialized for Kcrl {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KCRL track", Materialized::parse)(input)?;
        Ok((input, Kcrl(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `target_translation`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kttr(TrackChunk<[f32; 3]>);

impl Chunk for Kttr {
    fn tag() -> Tag {
        Tag([0x4B, 0x54, 0x54, 0x52]) // KTTR
    }
}

impl Materialized for Kttr {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KTTR track", Materialized::parse)(input)?;
        Ok((input, Kttr(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}
