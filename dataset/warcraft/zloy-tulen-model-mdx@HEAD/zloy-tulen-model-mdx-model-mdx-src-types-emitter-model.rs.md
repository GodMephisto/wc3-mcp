---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/emitter/model.rs"
ref: HEAD
---

use super::super::{chunk::utils::*, chunk::*, materialize::*, node::Node, tracks::TrackChunk};
use log::*;

// ParticleEmitter {
//     uint32 inclusiveSize
//     Node node
//     float emissionRate
//     float gravity
//     float longitude
//     float latitude
//     char[260] spawnModelFileName
//     float lifespan
//     float initialiVelocity
//     (KPEE)
//     (KPEG)
//     (KPLN)
//     (KPLT)
//     (KPEL)
//     (KPES)
//     (KPEV)
//   }
// KPEE: float emissionRate
// KPEG: float gravity
// KPLN: float longitude
// KPLT: float latitude
// KPEL: float lifespan
// KPES: float speed
// KPEV: float visibility

pub const EMITTER_FILENAME_LENGTH: usize = 260;

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct ParticleEmitter {
    pub node: Node,
    pub emission_rate: f32,
    pub gravity: f32,
    pub longitude: f32,
    pub latitude: f32,
    pub spawn_model_file_name: Literal<EMITTER_FILENAME_LENGTH>,
    pub lifespan: f32,
    pub initial_velocity: f32,
    pub kpee: Option<Kpee>,
    pub kpeg: Option<Kpeg>,
    pub kpln: Option<Kpln>,
    pub kplt: Option<Kplt>,
    pub kpel: Option<Kpel>,
    pub kpes: Option<Kpes>,
    pub kpev: Option<Kpev>,
    pub ordered: Option<Vec<Tag>>,
}

impl Materialized for ParticleEmitter {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        parse_inclusive_sized(|input| {
            let (input, node) = context("node", Materialized::parse)(input)?;
            let (input, emission_rate) = context("emission_rate", Materialized::parse)(input)?;
            let (input, gravity) = context("gravity", Materialized::parse)(input)?;
            let (input, longitude) = context("longitude", Materialized::parse)(input)?;
            let (input, latitude) = context("latitude", Materialized::parse)(input)?;
            let (input, spawn_model_file_name) =
                context("spawn_model_file_name", Materialized::parse)(input)?;
            let (input, lifespan) = context("lifespan", Materialized::parse)(input)?;
            let (input, initial_velocity) =
                context("initial_velocity", Materialized::parse)(input)?;
            let mut kpee: Option<Kpee> = None;
            let mut kpeg: Option<Kpeg> = None;
            let mut kpln: Option<Kpln> = None;
            let mut kplt: Option<Kplt> = None;
            let mut kpel: Option<Kpel> = None;
            let mut kpes: Option<Kpes> = None;
            let mut kpev: Option<Kpev> = None;
            let mut ordered = vec![];
            let (input, _) = parse_tagged(|tag, input| {
                if tag == Kpee::tag() {
                    let (input, chunk) = context("KPEE chunk", Materialized::parse)(input)?;
                    kpee = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kpeg::tag() {
                    let (input, chunk) = context("KPEG chunk", Materialized::parse)(input)?;
                    kpeg = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kpln::tag() {
                    let (input, chunk) = context("KPLN chunk", Materialized::parse)(input)?;
                    kpln = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kplt::tag() {
                    let (input, chunk) = context("KPLT chunk", Materialized::parse)(input)?;
                    kplt = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kpel::tag() {
                    let (input, chunk) = context("KPEL chunk", Materialized::parse)(input)?;
                    kpel = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kpes::tag() {
                    let (input, chunk) = context("KPES chunk", Materialized::parse)(input)?;
                    kpes = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kpev::tag() {
                    let (input, chunk) = context("KPEV chunk", Materialized::parse)(input)?;
                    kpev = Some(chunk);
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
                ParticleEmitter {
                    node,
                    emission_rate,
                    gravity,
                    longitude,
                    latitude,
                    spawn_model_file_name,
                    lifespan,
                    initial_velocity,
                    kpee,
                    kpeg,
                    kpln,
                    kplt,
                    kpel,
                    kpes,
                    kpev,
                    ordered: Some(ordered),
                },
            ))
        })(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_inclusive_sized(output, |output| {
            self.node.encode(output)?;
            self.emission_rate.encode(output)?;
            self.gravity.encode(output)?;
            self.longitude.encode(output)?;
            self.latitude.encode(output)?;
            self.spawn_model_file_name.encode(output)?;
            self.lifespan.encode(output)?;
            self.initial_velocity.encode(output)?;
            if let Some(ordered) = &self.ordered {
                for tag in ordered {
                    if *tag == Kpee::tag() {
                        if let Some(chunk) = &self.kpee {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Kpeg::tag() {
                        if let Some(chunk) = &self.kpeg {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Kpln::tag() {
                        if let Some(chunk) = &self.kpln {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Kplt::tag() {
                        if let Some(chunk) = &self.kplt {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Kpel::tag() {
                        if let Some(chunk) = &self.kpel {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Kpes::tag() {
                        if let Some(chunk) = &self.kpes {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Kpev::tag() {
                        if let Some(chunk) = &self.kpev {
                            chunk.encode(output)?;
                        }
                    } else {
                        warn!("Unknown tag {tag}, skipping...");
                    }
                }
            } else {
                if let Some(chunk) = &self.kpee {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.kpeg {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.kpln {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.kplt {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.kpel {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.kpes {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.kpev {
                    chunk.encode(output)?;
                }
            }

            Ok(())
        })
    }
}

/// Holds `emissionRate`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kpee(TrackChunk<f32>);

impl Chunk for Kpee {
    fn tag() -> Tag {
        Tag([0x4B, 0x50, 0x45, 0x45]) // KPEE
    }
}

impl Materialized for Kpee {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KPEE track", Materialized::parse)(input)?;
        Ok((input, Kpee(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `gravity`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kpeg(TrackChunk<f32>);

impl Chunk for Kpeg {
    fn tag() -> Tag {
        Tag([0x4B, 0x50, 0x45, 0x47]) // KPEG
    }
}

impl Materialized for Kpeg {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KPEG track", Materialized::parse)(input)?;
        Ok((input, Kpeg(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `longitude`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kpln(TrackChunk<f32>);

impl Chunk for Kpln {
    fn tag() -> Tag {
        Tag([0x4B, 0x50, 0x4c, 0x4e]) // KPLN
    }
}

impl Materialized for Kpln {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KPLN track", Materialized::parse)(input)?;
        Ok((input, Kpln(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `latitude`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kplt(TrackChunk<f32>);

impl Chunk for Kplt {
    fn tag() -> Tag {
        Tag([0x4B, 0x50, 0x4c, 0x54]) // KPLT
    }
}

impl Materialized for Kplt {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KPLT track", Materialized::parse)(input)?;
        Ok((input, Kplt(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `lifespan`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kpel(TrackChunk<f32>);

impl Chunk for Kpel {
    fn tag() -> Tag {
        Tag([0x4B, 0x50, 0x45, 0x4c]) // KPEL
    }
}

impl Materialized for Kpel {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KPEL track", Materialized::parse)(input)?;
        Ok((input, Kpel(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `speed`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kpes(TrackChunk<f32>);

impl Chunk for Kpes {
    fn tag() -> Tag {
        Tag([0x4B, 0x50, 0x45, 0x53]) // KPES
    }
}

impl Materialized for Kpes {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KPES track", Materialized::parse)(input)?;
        Ok((input, Kpes(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `visibility`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kpev(TrackChunk<f32>);

impl Chunk for Kpev {
    fn tag() -> Tag {
        Tag([0x4B, 0x50, 0x45, 0x56]) // KPEV
    }
}

impl Materialized for Kpev {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KPEV track", Materialized::parse)(input)?;
        Ok((input, Kpev(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}