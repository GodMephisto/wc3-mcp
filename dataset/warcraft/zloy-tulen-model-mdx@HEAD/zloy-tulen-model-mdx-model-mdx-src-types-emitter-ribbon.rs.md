---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/emitter/ribbon.rs"
ref: HEAD
---

use super::super::{chunk::utils::*, chunk::*, materialize::*, node::Node, tracks::TrackChunk};
use log::*;

// RibbonEmitter {
//     uint32 inclusiveSize
//     Node node
//     float heightAbove
//     float heightBelow
//     float alpha
//     float[3] color
//     float lifespan
//     uint32 textureSlot
//     uint32 emissionRate
//     uint32 rows
//     uint32 columns
//     uint32 materialId
//     float gravity
//     (KRHA)
//     (KRHB)
//     (KRAL)
//     (KRCO)
//     (KRTX)
//     (KRVS)
//   }
// KRVS: float visibility
// KRHA: float heightAbove
// KRHB: float heightBelow
// KRAL: float alpha
// KRCO: float[3] color
// KRTX: uint32 textureSlot

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct RibbonEmitter {
    pub node: Node,
    pub height_above: f32,
    pub height_below: f32,
    pub alpha: f32,
    pub color: [f32; 3],
    pub lifespan: f32,
    pub texture_slot: u32,
    pub emission_rate: u32,
    pub rows: u32,
    pub columns: u32,
    pub material_id: u32,
    pub gravity: f32,
    pub krha: Option<Krha>,
    pub krhb: Option<Krhb>,
    pub kral: Option<Kral>,
    pub krco: Option<Krco>,
    pub krtx: Option<Krtx>,
    pub krvs: Option<Krvs>,
    pub ordered: Option<Vec<Tag>>,
}

impl Materialized for RibbonEmitter {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        parse_inclusive_sized(|input| {
            let (input, node) = context("node", Materialized::parse)(input)?;
            let (input, height_above) = context("height_above", Materialized::parse)(input)?;
            let (input, height_below) = context("height_below", Materialized::parse)(input)?;
            let (input, alpha) = context("alpha", Materialized::parse)(input)?;
            let (input, color) = context("color", Materialized::parse)(input)?;
            let (input, lifespan) = context("lifespan", Materialized::parse)(input)?;
            let (input, texture_slot) = context("texture_slot", Materialized::parse)(input)?;
            let (input, emission_rate) = context("emission_rate", Materialized::parse)(input)?;
            let (input, rows) = context("rows", Materialized::parse)(input)?;
            let (input, columns) = context("columns", Materialized::parse)(input)?;
            let (input, material_id) = context("material_id", Materialized::parse)(input)?;
            let (input, gravity) = context("gravity", Materialized::parse)(input)?;
            let mut krha: Option<Krha> = None;
            let mut krhb: Option<Krhb> = None;
            let mut kral: Option<Kral> = None;
            let mut krco: Option<Krco> = None;
            let mut krtx: Option<Krtx> = None;
            let mut krvs: Option<Krvs> = None;
            let mut ordered = vec![];
            let (input, _) = parse_tagged(|tag, input| {
                if tag == Krha::tag() {
                    let (input, chunk) = context("KRHA chunk", Materialized::parse)(input)?;
                    krha = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Krhb::tag() {
                    let (input, chunk) = context("Krhb chunk", Materialized::parse)(input)?;
                    krhb = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kral::tag() {
                    let (input, chunk) = context("KRAL chunk", Materialized::parse)(input)?;
                    kral = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Krco::tag() {
                    let (input, chunk) = context("KRCO chunk", Materialized::parse)(input)?;
                    krco = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Krtx::tag() {
                    let (input, chunk) = context("KRTX chunk", Materialized::parse)(input)?;
                    krtx = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Krvs::tag() {
                    let (input, chunk) = context("KRVS chunk", Materialized::parse)(input)?;
                    krvs = Some(chunk);
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
                RibbonEmitter {
                    node,
                    height_above,
                    height_below,
                    alpha,
                    color,
                    lifespan,
                    texture_slot,
                    emission_rate,
                    rows,
                    columns,
                    material_id,
                    gravity,
                    krha,
                    krhb,
                    kral,
                    krco,
                    krtx,
                    krvs,
                    ordered: Some(ordered),
                },
            ))
        })(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_inclusive_sized(output, |output| {
            self.node.encode(output)?;
            self.height_above.encode(output)?;
            self.height_below.encode(output)?;
            self.alpha.encode(output)?;
            self.color.encode(output)?;
            self.lifespan.encode(output)?;
            self.texture_slot.encode(output)?;
            self.emission_rate.encode(output)?;
            self.rows.encode(output)?;
            self.columns.encode(output)?;
            self.material_id.encode(output)?;
            self.gravity.encode(output)?;
            if let Some(ordered) = &self.ordered {
                for tag in ordered {
                    if *tag == Krha::tag() {
                        if let Some(chunk) = &self.krha {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Krhb::tag() {
                        if let Some(chunk) = &self.krhb {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Kral::tag() {
                        if let Some(chunk) = &self.kral {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Krco::tag() {
                        if let Some(chunk) = &self.krco {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Krtx::tag() {
                        if let Some(chunk) = &self.krtx {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Krvs::tag() {
                        if let Some(chunk) = &self.krvs {
                            chunk.encode(output)?;
                        }
                    } else {
                        warn!("Unknown tag {tag}, skipping...");
                    }
                }
            } else {
                if let Some(chunk) = &self.krha {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.krhb {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.kral {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.krco {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.krtx {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.krvs {
                    chunk.encode(output)?;
                }
            }

            Ok(())
        })
    }
}

/// Holds `heightAbove`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Krha(TrackChunk<f32>);

impl Chunk for Krha {
    fn tag() -> Tag {
        Tag([0x4B, 0x52, 0x48, 0x41]) // KRHA
    }
}

impl Materialized for Krha {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KRHA track", Materialized::parse)(input)?;
        Ok((input, Krha(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `heightBelow`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Krhb(TrackChunk<f32>);

impl Chunk for Krhb {
    fn tag() -> Tag {
        Tag([0x4B, 0x52, 0x48, 0x42]) // KRHB
    }
}

impl Materialized for Krhb {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KRHB track", Materialized::parse)(input)?;
        Ok((input, Krhb(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `alpha`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kral(TrackChunk<f32>);

impl Chunk for Kral {
    fn tag() -> Tag {
        Tag([0x4B, 0x52, 0x41, 0x4c]) // KRAL
    }
}

impl Materialized for Kral {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KRAL track", Materialized::parse)(input)?;
        Ok((input, Kral(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `color`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Krco(TrackChunk<[f32; 3]>);

impl Chunk for Krco {
    fn tag() -> Tag {
        Tag([0x4B, 0x52, 0x43, 0x4f]) // KRCO
    }
}

impl Materialized for Krco {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KRCO track", Materialized::parse)(input)?;
        Ok((input, Krco(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `textureSlot`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Krtx(TrackChunk<u32>);

impl Chunk for Krtx {
    fn tag() -> Tag {
        Tag([0x4B, 0x52, 0x54, 0x58]) // KRTX
    }
}

impl Materialized for Krtx {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KRTX track", Materialized::parse)(input)?;
        Ok((input, Krtx(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `visibility`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Krvs(TrackChunk<f32>);

impl Chunk for Krvs {
    fn tag() -> Tag {
        Tag([0x4B, 0x52, 0x56, 0x53]) // KRVS
    }
}

impl Materialized for Krvs {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KRVS track", Materialized::parse)(input)?;
        Ok((input, Krvs(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}
