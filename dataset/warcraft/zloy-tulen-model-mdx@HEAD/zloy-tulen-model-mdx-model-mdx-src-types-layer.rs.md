---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/layer.rs"
ref: HEAD
---

use super::materialize::*;
use super::tracks::*;
use super::utils::*;
use crate::types::chunk::*;
use bitflags::bitflags;
use log::*;
use nom::error::context;
use std::fmt;

// Layer {
//     uint32 inclusiveSize
//     uint32 filterMode // 0: none
//                       // 1: transparent
//                       // 2: blend
//                       // 3: additive
//                       // 4: add alpha
//                       // 5: modulate
//                       // 6: modulate 2x
//     uint32 shadingFlags // 0x1: unshaded
//                         // 0x2: sphere environment map
//                         // 0x4: ?
//                         // 0x8: ?
//                         // 0x10: two sided
//                         // 0x20: unfogged
//                         // 0x40: no depth test
//                         // 0x80: no depth set
//     uint32 textureId
//     uint32 textureAnimationId
//     uint32 coordId
//     float alpha
//     if (version > 800) {
//       float emissiveGain
//       float[3] fresnelColor
//       float fresnelOpacity
//       float fresnelTeamColor
//     }
//     (KMTF)
//     (KMTA)
//     if (version > 800) {
//       (KMTE)
//     }
//     if (version > 900) {
//       (KFC3)
//       (KFCA)
//       (KFTC)
//     }
//   }

#[derive(Debug, Clone, Copy, PartialEq, PartialOrd, Eq, Ord, Hash)]
pub enum FilterMode {
    None,
    Transparent,
    Blend,
    Additive,
    AddAlpha,
    Modulate,
    Modulate2x,
}

#[derive(Debug, Clone, Copy, PartialEq, PartialOrd, Eq, Ord, Hash)]
pub struct UnknownFilterMode(pub u32);

impl std::error::Error for UnknownFilterMode {}

impl fmt::Display for UnknownFilterMode {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "Unknown filter mode {}", self.0)
    }
}

impl TryFrom<u32> for FilterMode {
    type Error = UnknownFilterMode;

    fn try_from(value: u32) -> Result<Self, Self::Error> {
        match value {
            0 => Ok(FilterMode::None),
            1 => Ok(FilterMode::Transparent),
            2 => Ok(FilterMode::Blend),
            3 => Ok(FilterMode::Additive),
            4 => Ok(FilterMode::AddAlpha),
            5 => Ok(FilterMode::Modulate),
            6 => Ok(FilterMode::Modulate2x),
            _ => Err(UnknownFilterMode(value)),
        }
    }
}

impl From<FilterMode> for u32 {
    fn from(value: FilterMode) -> u32 {
        match value {
            FilterMode::None => 0,
            FilterMode::Transparent => 1,
            FilterMode::Blend => 2,
            FilterMode::Additive => 3,
            FilterMode::AddAlpha => 4,
            FilterMode::Modulate => 5,
            FilterMode::Modulate2x => 6,
        }
    }
}

impl Materialized for FilterMode {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, flag): (&[u8], u32) = Materialized::parse(input)?;
        let filter = flag
            .try_into()
            .map_err(|e: UnknownFilterMode| nom::Err::Failure(e.into()))?;
        Ok((input, filter))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        let flag: u32 = (*self).into();
        flag.encode(output)
    }
}

bitflags! {
    pub struct ShadingFlags: u32 {
        const UNSHADED = 0b00000001;
        const SPHERE_ENV_MAP = 0b00000010;
        const UNKNOWN4 = 0b00000100;
        const UNKNOWN8 = 0b00001000;
        const TWO_SIDED = 0b00010000;
        const UNFOGGED = 0b00100000;
        const NO_DEPTH_TEST = 0b01000000;
        const NO_DEPTH_SET = 0b10000000;
    }
}

impl Materialized for ShadingFlags {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, flag): (&[u8], u32) = Materialized::parse(input)?;
        Ok((input, ShadingFlags { bits: flag }))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.bits.encode(output)
    }
}

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Layer {
    pub filter_mode: FilterMode,
    pub shading_flags: ShadingFlags,
    pub texture_id: u32,
    pub texture_animation_id: u32,
    pub coord_id: u32,
    pub alpha: f32,
    // if (version > 800)
    pub extra: Option<LayerExt>,
    // all versions
    pub kmtf: Option<Kmtf>,
    pub kmta: Option<Kmta>,
    // if (version > 800)
    pub kmte: Option<Kmte>,
    // if (version > 900)
    pub kfc3: Option<Kfc3>,
    pub kfca: Option<Kfca>,
    pub kftc: Option<Kftc>,
    pub ordered: Option<Vec<Tag>>,
}

impl Materialized for Layer {
    type Version = u32;

    fn parse_versioned(version: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        parse_inclusive_sized(|input| {
            trace!("Parsing fields of layer");
            let (input, filter_mode) = context("filter_mode", Materialized::parse)(input)?;
            let (input, shading_flags) = context("shading_flags", Materialized::parse)(input)?;
            let (input, texture_id) = context("texture_id", Materialized::parse)(input)?;
            let (input, texture_animation_id) =
                context("texture_animation_id", Materialized::parse)(input)?;
            let (input, coord_id) = context("coord_id", Materialized::parse)(input)?;
            let (input, alpha) = context("alpha", Materialized::parse)(input)?;
            let (input, extra) = parse_versioned_greater(
                version,
                800,
                context("layer extra fields", Materialized::parse),
            )(input)?;
            // they can be in any order here and optional
            let mut kmtf: Option<Kmtf> = None;
            let mut kmta: Option<Kmta> = None;
            let mut kmte: Option<Kmte> = None;
            let mut kfc3: Option<Kfc3> = None;
            let mut kfca: Option<Kfca> = None;
            let mut kftc: Option<Kftc> = None;
            let mut ordered = vec![];
            trace!("Parsing tracks of layer");
            let (input, _) = parse_tagged(|tag, input| {
                if tag == Kmtf::tag() {
                    let (input, chunk) = context("KMTF chunk", Materialized::parse)(input)?;
                    kmtf = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kmta::tag() {
                    let (input, chunk) = context("KMTA chunk", Materialized::parse)(input)?;
                    kmta = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kmte::tag() {
                    let (input, chunk) = context("KMTE chunk", Materialized::parse)(input)?;
                    kmte = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kfc3::tag() {
                    let (input, chunk) = context("KFC3 chunk", Materialized::parse)(input)?;
                    kfc3 = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kfca::tag() {
                    let (input, chunk) = context("KFCA chunk", Materialized::parse)(input)?;
                    kfca = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kftc::tag() {
                    let (input, chunk) = context("KFTC chunk", Materialized::parse)(input)?;
                    kftc = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else {
                    let found: String = std::str::from_utf8(&tag.0)
                        .map(|s| s.to_owned())
                        .unwrap_or_else(|_| format!("{:?}", tag.0));
                    error!("Unknown tag {}", found);
                    return Err(nom::Err::Failure(ParseError::UnknownTag(found)));
                }
            })(input)?;
            Ok((
                input,
                Layer {
                    filter_mode,
                    shading_flags,
                    texture_id,
                    texture_animation_id,
                    coord_id,
                    alpha,
                    extra,
                    kmtf,
                    kmta,
                    kmte,
                    kfc3,
                    kfca,
                    kftc,
                    ordered: Some(ordered),
                },
            ))
        })(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_inclusive_sized(output, |output| {
            self.filter_mode.encode(output)?;
            self.shading_flags.encode(output)?;
            self.texture_id.encode(output)?;
            self.texture_animation_id.encode(output)?;
            self.coord_id.encode(output)?;
            self.alpha.encode(output)?;
            if let Some(v) = &self.extra {
                v.encode(output)?;
            }
            if let Some(ordered) = &self.ordered {
                for tag in ordered {
                    if *tag == Kmtf::tag() {
                        if let Some(v) = &self.kmtf {
                            v.encode(output)?;
                        }
                    } else if *tag == Kmta::tag() {
                        if let Some(v) = &self.kmta {
                            v.encode(output)?;
                        }
                    } else if *tag == Kmte::tag() {
                        if let Some(v) = &self.kmte {
                            v.encode(output)?;
                        }
                    } else if *tag == Kfc3::tag() {
                        if let Some(v) = &self.kfc3 {
                            v.encode(output)?;
                        }
                    } else if *tag == Kfca::tag() {
                        if let Some(v) = &self.kfca {
                            v.encode(output)?;
                        }
                    } else if *tag == Kftc::tag() {
                        if let Some(v) = &self.kftc {
                            v.encode(output)?;
                        }
                    } else {
                        warn!("Unkown tag {tag}, skipping it...");
                    }
                }
            } else {
                if let Some(v) = &self.kmtf {
                    v.encode(output)?;
                }
                if let Some(v) = &self.kmta {
                    v.encode(output)?;
                }
                if let Some(v) = &self.kmte {
                    v.encode(output)?;
                }
                if let Some(v) = &self.kfc3 {
                    v.encode(output)?;
                }
                if let Some(v) = &self.kfca {
                    v.encode(output)?;
                }
                if let Some(v) = &self.kftc {
                    v.encode(output)?;
                }
            }
            Ok(())
        })
    }
}

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct LayerExt {
    pub emissive_gain: f32,
    pub fresnel_color: [f32; 3],
    pub fresnel_opacity: f32,
    pub fresnel_team_color: f32,
}

impl Materialized for LayerExt {
    type Version = u32;

    /// Parse the chunk from given input
    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, emissive_gain) = context("emissive_gain", Materialized::parse)(input)?;
        let (input, fresnel_color) = context("fresnel_color", Materialized::parse)(input)?;
        let (input, fresnel_opacity) = context("fresnel_opacity", Materialized::parse)(input)?;
        let (input, fresnel_team_color) =
            context("fresnel_team_color", Materialized::parse)(input)?;
        Ok((
            input,
            LayerExt {
                emissive_gain,
                fresnel_color,
                fresnel_opacity,
                fresnel_team_color,
            },
        ))
    }

    /// Encode the chunk to byte stream
    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.emissive_gain.encode(output)?;
        self.fresnel_color.encode(output)?;
        self.fresnel_opacity.encode(output)?;
        self.fresnel_team_color.encode(output)?;
        Ok(())
    }
}

/// Holds `texture_id`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kmtf(TrackChunk<u32>);

impl Chunk for Kmtf {
    fn tag() -> Tag {
        Tag([0x4B, 0x4D, 0x54, 0x46]) // KMTF
    }
}

impl Materialized for Kmtf {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KMTF track", Materialized::parse)(input)?;
        Ok((input, Kmtf(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `alpha` field
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kmta(pub TrackChunk<f32>);

impl Chunk for Kmta {
    fn tag() -> Tag {
        Tag([0x4B, 0x4D, 0x54, 0x41]) // KMTA
    }
}

impl Materialized for Kmta {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KMTA track", Materialized::parse)(input)?;
        Ok((input, Kmta(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `emissive_gain` field
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kmte(pub TrackChunk<f32>);

impl Chunk for Kmte {
    fn tag() -> Tag {
        Tag([0x4B, 0x4D, 0x54, 0x45]) // KMTE
    }
}

impl Materialized for Kmte {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KMTE track", Materialized::parse)(input)?;
        Ok((input, Kmte(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `fresnel_color`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kfc3(pub TrackChunk<[f32; 3]>);

impl Chunk for Kfc3 {
    fn tag() -> Tag {
        Tag([0x4B, 0x46, 0x43, 0x33]) // KFC3
    }
}

impl Materialized for Kfc3 {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KFC3 track", Materialized::parse)(input)?;
        Ok((input, Kfc3(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `fresnel_alpha`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kfca(pub TrackChunk<f32>);

impl Chunk for Kfca {
    fn tag() -> Tag {
        Tag([0x4B, 0x46, 0x43, 0x41]) // KFCA
    }
}

impl Materialized for Kfca {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KFCA track", Materialized::parse)(input)?;
        Ok((input, Kfca(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `fresnel_team_color`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kftc(pub TrackChunk<f32>);

impl Chunk for Kftc {
    fn tag() -> Tag {
        Tag([0x4B, 0x46, 0x43, 0x41]) // KFTC
    }
}

impl Materialized for Kftc {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KFTC track", Materialized::parse)(input)?;
        Ok((input, Kftc(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}
