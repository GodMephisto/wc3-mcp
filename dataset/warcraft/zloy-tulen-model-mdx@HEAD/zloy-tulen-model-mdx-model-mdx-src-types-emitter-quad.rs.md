---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/emitter/quad.rs"
ref: HEAD
---

use super::super::{chunk::utils::*, chunk::*, materialize::*, node::Node, tracks::TrackChunk};
use log::*;
use std::fmt;

#[derive(Debug, Clone, Copy, PartialEq, PartialOrd, Eq, Ord, Hash)]
pub enum FilterMode {
    Blend,
    Additive,
    Modulate,
    Modulate2x,
    AlphaKey,
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
            0 => Ok(FilterMode::Blend),
            1 => Ok(FilterMode::Additive),
            2 => Ok(FilterMode::Modulate),
            3 => Ok(FilterMode::Modulate2x),
            4 => Ok(FilterMode::AlphaKey),
            _ => Err(UnknownFilterMode(value)),
        }
    }
}

impl From<FilterMode> for u32 {
    fn from(value: FilterMode) -> u32 {
        match value {
            FilterMode::Blend => 0,
            FilterMode::Additive => 1,
            FilterMode::Modulate => 2,
            FilterMode::Modulate2x => 3,
            FilterMode::AlphaKey => 4,
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

#[derive(Debug, Clone, Copy, PartialEq, PartialOrd, Eq, Ord, Hash)]
pub enum HeadTail {
    Head,
    Tail,
    Both,
}

#[derive(Debug, Clone, Copy, PartialEq, PartialOrd, Eq, Ord, Hash)]
pub struct UnknownHeadTail(pub u32);

impl std::error::Error for UnknownHeadTail {}

impl fmt::Display for UnknownHeadTail {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "Unknown filter mode {}", self.0)
    }
}

impl TryFrom<u32> for HeadTail {
    type Error = UnknownHeadTail;

    fn try_from(value: u32) -> Result<Self, Self::Error> {
        match value {
            0 => Ok(HeadTail::Head),
            1 => Ok(HeadTail::Tail),
            2 => Ok(HeadTail::Both),
            _ => Err(UnknownHeadTail(value)),
        }
    }
}

impl From<HeadTail> for u32 {
    fn from(value: HeadTail) -> u32 {
        match value {
            HeadTail::Head => 0,
            HeadTail::Tail => 1,
            HeadTail::Both => 2,
        }
    }
}

impl Materialized for HeadTail {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, flag): (&[u8], u32) = Materialized::parse(input)?;
        let filter = flag
            .try_into()
            .map_err(|e: UnknownHeadTail| nom::Err::Failure(e.into()))?;
        Ok((input, filter))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        let flag: u32 = (*self).into();
        flag.encode(output)
    }
}

// ParticleEmitter2 {
//     uint32 inclusiveSize
//     Node node
//     float speed
//     float variation
//     float latitude
//     float gravity
//     float lifespan
//     float emissionRate
//     float length
//     float width
//     uint32 filterMode // 0: blend
//                       // 1: additive
//                       // 2: modulate
//                       // 3: modulate 2x
//                       // 4: alpha key
//     uint32 rows
//     uint32 columns
//     uint32 headOrTail // 0: head
//                       // 1: tail
//                       // 2: both
//     float tailLength
//     float time
//     float[3][3] segmentColor
//     uint8[3] segmentAlpha
//     float[3] segmentScaling
//     uint32[3] headInterval
//     uint32[3] headDecayInterval
//     uint32[3] tailInterval
//     uint32[3] tailDecayInterval
//     uint32 textureId
//     uint32 squirt
//     uint32 priorityPlane
//     uint32 replaceableId
//     (KP2S)
//     (KP2R)
//     (KP2L)
//     (KP2G)
//     (KP2E)
//     (KP2N)
//     (KP2W)
//     (KP2V)
//   }
// KP2E: float emissionRate
// KP2G: float gravity
// KP2L: float latitude
// KP2S: float speed
// KP2V: float visibility
// KP2R: float variation
// KP2N: float length
// KP2W: float width
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct ParticleEmitter2 {
    pub node: Node,
    pub speed: f32,
    pub variation: f32,
    pub latitude: f32,
    pub gravity: f32,
    pub lifespan: f32,
    pub emission_rate: f32,
    pub length: f32,
    pub width: f32,
    pub filter_mode: FilterMode,
    pub rows: u32,
    pub columns: u32,
    pub head_or_tail: HeadTail,
    pub tail_length: f32,
    pub time: f32,
    pub segment_color: [[f32; 3]; 3],
    pub segment_alpha: [u8; 3],
    pub segment_scaling: [f32; 3],
    pub head_interval: [u32; 3],
    pub head_decay_interval: [u32; 3],
    pub tail_interval: [u32; 3],
    pub tail_decay_interval: [u32; 3],
    pub texture_id: u32,
    pub squirt: u32,
    pub priority_plane: u32,
    pub replaceable_id: u32,
    pub kp2s: Option<Kp2s>,
    pub kp2r: Option<Kp2r>,
    pub kp2l: Option<Kp2l>,
    pub kp2g: Option<Kp2g>,
    pub kp2e: Option<Kp2e>,
    pub kp2n: Option<Kp2n>,
    pub kp2w: Option<Kp2w>,
    pub kp2v: Option<Kp2v>,
    pub ordered: Option<Vec<Tag>>,
}

impl Materialized for ParticleEmitter2 {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        parse_inclusive_sized(|input| {
            let (input, node) = context("node", Materialized::parse)(input)?;
            let (input, speed) = context("speed", Materialized::parse)(input)?;
            let (input, variation) = context("variation", Materialized::parse)(input)?;
            let (input, latitude) = context("latitude", Materialized::parse)(input)?;
            let (input, gravity) = context("gravity", Materialized::parse)(input)?;
            let (input, lifespan) = context("lifespan", Materialized::parse)(input)?;
            let (input, emission_rate) = context("emission_rate", Materialized::parse)(input)?;
            let (input, length) = context("length", Materialized::parse)(input)?;
            let (input, width) = context("width", Materialized::parse)(input)?;
            let (input, filter_mode) = context("filter_mode", Materialized::parse)(input)?;
            let (input, rows) = context("rows", Materialized::parse)(input)?;
            let (input, columns) = context("columns", Materialized::parse)(input)?;
            let (input, head_or_tail) = context("head_or_tail", Materialized::parse)(input)?;
            let (input, tail_length) = context("tail_length", Materialized::parse)(input)?;
            let (input, time) = context("time", Materialized::parse)(input)?;
            let (input, segment_color) = context("segment_color", Materialized::parse)(input)?;
            let (input, segment_alpha) = context("segment_alpha", Materialized::parse)(input)?;
            let (input, segment_scaling) = context("segment_scaling", Materialized::parse)(input)?;
            let (input, head_interval) = context("head_interval", Materialized::parse)(input)?;
            let (input, head_decay_interval) =
                context("head_decay_interval", Materialized::parse)(input)?;
            let (input, tail_interval) = context("tail_interval", Materialized::parse)(input)?;
            let (input, tail_decay_interval) =
                context("tail_decay_interval", Materialized::parse)(input)?;
            let (input, texture_id) = context("texture_id", Materialized::parse)(input)?;
            let (input, squirt) = context("squirt", Materialized::parse)(input)?;
            let (input, priority_plane) = context("priority_plane", Materialized::parse)(input)?;
            let (input, replaceable_id) = context("replaceable_id", Materialized::parse)(input)?;
            let mut kp2s: Option<Kp2s> = None;
            let mut kp2r: Option<Kp2r> = None;
            let mut kp2l: Option<Kp2l> = None;
            let mut kp2g: Option<Kp2g> = None;
            let mut kp2e: Option<Kp2e> = None;
            let mut kp2n: Option<Kp2n> = None;
            let mut kp2w: Option<Kp2w> = None;
            let mut kp2v: Option<Kp2v> = None;
            let mut ordered = vec![];
            let (input, _) = parse_tagged(|tag, input| {
                if tag == Kp2s::tag() {
                    let (input, chunk) = context("KP2S chunk", Materialized::parse)(input)?;
                    kp2s = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kp2r::tag() {
                    let (input, chunk) = context("KP2R chunk", Materialized::parse)(input)?;
                    kp2r = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kp2l::tag() {
                    let (input, chunk) = context("KP2L chunk", Materialized::parse)(input)?;
                    kp2l = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kp2g::tag() {
                    let (input, chunk) = context("KP2G chunk", Materialized::parse)(input)?;
                    kp2g = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kp2e::tag() {
                    let (input, chunk) = context("KP2E chunk", Materialized::parse)(input)?;
                    kp2e = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kp2n::tag() {
                    let (input, chunk) = context("KP2N chunk", Materialized::parse)(input)?;
                    kp2n = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kp2w::tag() {
                    let (input, chunk) = context("KP2W chunk", Materialized::parse)(input)?;
                    kp2w = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kp2v::tag() {
                    let (input, chunk) = context("KP2V chunk", Materialized::parse)(input)?;
                    kp2v = Some(chunk);
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
                ParticleEmitter2 {
                    node,
                    speed,
                    variation,
                    latitude,
                    gravity,
                    lifespan,
                    emission_rate,
                    length,
                    width,
                    filter_mode,
                    rows,
                    columns,
                    head_or_tail,
                    tail_length,
                    time,
                    segment_color,
                    segment_alpha,
                    segment_scaling,
                    head_interval,
                    head_decay_interval,
                    tail_interval,
                    tail_decay_interval,
                    texture_id,
                    squirt,
                    priority_plane,
                    replaceable_id,
                    kp2s,
                    kp2r,
                    kp2l,
                    kp2g,
                    kp2e,
                    kp2n,
                    kp2w,
                    kp2v,
                    ordered: Some(ordered),
                },
            ))
        })(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_inclusive_sized(output, |output| {
            self.node.encode(output)?;
            self.speed.encode(output)?;
            self.variation.encode(output)?;
            self.latitude.encode(output)?;
            self.gravity.encode(output)?;
            self.lifespan.encode(output)?;
            self.emission_rate.encode(output)?;
            self.length.encode(output)?;
            self.width.encode(output)?;
            self.filter_mode.encode(output)?;
            self.rows.encode(output)?;
            self.columns.encode(output)?;
            self.head_or_tail.encode(output)?;
            self.tail_length.encode(output)?;
            self.time.encode(output)?;
            self.segment_color.encode(output)?;
            self.segment_alpha.encode(output)?;
            self.segment_scaling.encode(output)?;
            self.head_interval.encode(output)?;
            self.head_decay_interval.encode(output)?;
            self.tail_interval.encode(output)?;
            self.tail_decay_interval.encode(output)?;
            self.texture_id.encode(output)?;
            self.squirt.encode(output)?;
            self.priority_plane.encode(output)?;
            self.replaceable_id.encode(output)?;
            if let Some(ordered) = &self.ordered {
                for tag in ordered {
                    if *tag == Kp2s::tag() {
                        if let Some(chunk) = &self.kp2s {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Kp2r::tag() {
                        if let Some(chunk) = &self.kp2r {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Kp2l::tag() {
                        if let Some(chunk) = &self.kp2l {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Kp2g::tag() {
                        if let Some(chunk) = &self.kp2g {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Kp2e::tag() {
                        if let Some(chunk) = &self.kp2e {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Kp2n::tag() {
                        if let Some(chunk) = &self.kp2n {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Kp2w::tag() {
                        if let Some(chunk) = &self.kp2w {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Kp2v::tag() {
                        if let Some(chunk) = &self.kp2v {
                            chunk.encode(output)?;
                        }
                    } else {
                        warn!("Unknown tag {tag}, skipping...");
                    }
                }
            } else {
                if let Some(chunk) = &self.kp2s {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.kp2r {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.kp2l {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.kp2g {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.kp2e {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.kp2n {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.kp2w {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.kp2v {
                    chunk.encode(output)?;
                }
            }

            Ok(())
        })
    }
}

/// Holds `speed`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kp2s(TrackChunk<f32>);

impl Chunk for Kp2s {
    fn tag() -> Tag {
        Tag([0x4B, 0x50, 0x32, 0x53]) // KP2S
    }
}

impl Materialized for Kp2s {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KP2S track", Materialized::parse)(input)?;
        Ok((input, Kp2s(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `variation`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kp2r(TrackChunk<f32>);

impl Chunk for Kp2r {
    fn tag() -> Tag {
        Tag([0x4B, 0x50, 0x32, 0x52]) // KP2R
    }
}

impl Materialized for Kp2r {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KP2R track", Materialized::parse)(input)?;
        Ok((input, Kp2r(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `latitude`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kp2l(TrackChunk<f32>);

impl Chunk for Kp2l {
    fn tag() -> Tag {
        Tag([0x4B, 0x50, 0x32, 0x4c]) // KP2L
    }
}

impl Materialized for Kp2l {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KP2L track", Materialized::parse)(input)?;
        Ok((input, Kp2l(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `gravity`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kp2g(TrackChunk<f32>);

impl Chunk for Kp2g {
    fn tag() -> Tag {
        Tag([0x4B, 0x50, 0x32, 0x47]) // KP2G
    }
}

impl Materialized for Kp2g {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KP2G track", Materialized::parse)(input)?;
        Ok((input, Kp2g(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `emissionRate`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kp2e(TrackChunk<f32>);

impl Chunk for Kp2e {
    fn tag() -> Tag {
        Tag([0x4B, 0x50, 0x32, 0x45]) // KP2E
    }
}

impl Materialized for Kp2e {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KP2E track", Materialized::parse)(input)?;
        Ok((input, Kp2e(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `length`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kp2n(TrackChunk<f32>);

impl Chunk for Kp2n {
    fn tag() -> Tag {
        Tag([0x4B, 0x50, 0x32, 0x4e]) // KP2N
    }
}

impl Materialized for Kp2n {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KP2N track", Materialized::parse)(input)?;
        Ok((input, Kp2n(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `width`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kp2w(TrackChunk<f32>);

impl Chunk for Kp2w {
    fn tag() -> Tag {
        Tag([0x4B, 0x50, 0x32, 0x57]) // KP2W
    }
}

impl Materialized for Kp2w {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KP2W track", Materialized::parse)(input)?;
        Ok((input, Kp2w(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `visibility`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kp2v(TrackChunk<f32>);

impl Chunk for Kp2v {
    fn tag() -> Tag {
        Tag([0x4B, 0x50, 0x32, 0x56]) // KP2V
    }
}

impl Materialized for Kp2v {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KP2V track", Materialized::parse)(input)?;
        Ok((input, Kp2v(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}
