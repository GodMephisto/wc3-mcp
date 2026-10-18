---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/animation.rs"
ref: HEAD
---

use super::chunk::utils::*;
use super::materialize::{ParseError, *};
use super::tracks::*;
use super::*;
use log::*;

// GeosetAnimation {
//     uint32 inclusiveSize
//     float alpha
//     uint32 flags
//     float[3] color
//     uint32 geosetId
//     (KGAO)
//     (KGAC)
//   }
// KGAO: float alpha
// KGAC: float[3] color

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct GeosetAnimation {
    pub alpha: f32,
    pub flags: u32,
    pub color: [f32; 3],
    pub geoset_id: u32,
    pub kgao: Option<Kgao>,
    pub kgac: Option<Kgac>,
    pub ordered: Option<Vec<Tag>>,
}

impl Materialized for GeosetAnimation {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        parse_inclusive_sized(|input| {
            let (input, alpha) = context("alpha", Materialized::parse)(input)?;
            let (input, flags) = context("flags", Materialized::parse)(input)?;
            let (input, color) = context("color", Materialized::parse)(input)?;
            let (input, geoset_id) = context("geoset_id", Materialized::parse)(input)?;
            let mut kgao: Option<Kgao> = None;
            let mut kgac: Option<Kgac> = None;
            let mut ordered = vec![];
            trace!("Parsing tracks of layer");
            let (input, _) = parse_tagged(|tag, input| {
                if tag == Kgao::tag() {
                    let (input, chunk) = context("KGAO chunk", Materialized::parse)(input)?;
                    kgao = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Kgac::tag() {
                    let (input, chunk) = context("KGAC chunk", Materialized::parse)(input)?;
                    kgac = Some(chunk);
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
                GeosetAnimation {
                    alpha,
                    flags,
                    color,
                    geoset_id,
                    kgao,
                    kgac,
                    ordered: Some(ordered),
                },
            ))
        })(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_inclusive_sized(output, |output| {
            self.alpha.encode(output)?;
            self.flags.encode(output)?;
            self.color.encode(output)?;
            self.geoset_id.encode(output)?;
            if let Some(ordered) = &self.ordered {
                for tag in ordered {
                    if *tag == Kgao::tag() {
                        if let Some(v) = &self.kgao {
                            v.encode(output)?;
                        }
                    } else if *tag == Kgac::tag() {
                        if let Some(v) = &self.kgac {
                            v.encode(output)?;
                        }
                    } else {
                        warn!("Unknown tag {tag}, skipping...");
                    }
                }
            } else {
                if let Some(v) = &self.kgao {
                    v.encode(output)?;
                }
                if let Some(v) = &self.kgac {
                    v.encode(output)?;
                }
            }
            Ok(())
        })
    }
}

/// Holds `alpha`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kgao(TrackChunk<f32>);

impl Chunk for Kgao {
    fn tag() -> Tag {
        Tag([0x4B, 0x47, 0x41, 0x4F]) // KGAO
    }
}

impl Materialized for Kgao {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("Kgao track", Materialized::parse)(input)?;
        Ok((input, Kgao(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `alpha`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Kgac(TrackChunk<[f32; 3]>);

impl Chunk for Kgac {
    fn tag() -> Tag {
        Tag([0x4B, 0x47, 0x41, 0x43]) // KGAC
    }
}

impl Materialized for Kgac {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("Kgac track", Materialized::parse)(input)?;
        Ok((input, Kgac(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}
