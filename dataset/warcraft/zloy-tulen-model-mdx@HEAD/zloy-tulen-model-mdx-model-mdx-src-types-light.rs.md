---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/light.rs"
ref: HEAD
---

use super::chunk::utils::*;
use super::chunk::*;
use super::materialize::*;
use super::node::*;
use super::tracks::*;
use log::*;
use std::fmt;

#[derive(Debug, Clone, Copy, PartialEq, PartialOrd, Eq, Ord, Hash)]
pub enum LightType {
    Omni,
    Directional,
    Ambient,
}

#[derive(Debug, Clone, Copy, PartialEq, PartialOrd, Eq, Ord, Hash)]
pub struct UnknownLightType(pub u32);

impl std::error::Error for UnknownLightType {}

impl fmt::Display for UnknownLightType {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "Unknown light type {}", self.0)
    }
}

impl TryFrom<u32> for LightType {
    type Error = UnknownLightType;

    fn try_from(value: u32) -> Result<Self, Self::Error> {
        match value {
            0 => Ok(LightType::Omni),
            1 => Ok(LightType::Directional),
            2 => Ok(LightType::Ambient),
            _ => Err(UnknownLightType(value)),
        }
    }
}

impl From<LightType> for u32 {
    fn from(value: LightType) -> u32 {
        match value {
            LightType::Omni => 0,
            LightType::Directional => 1,
            LightType::Ambient => 2,
        }
    }
}

impl Materialized for LightType {
    type Version = ();

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, flag): (&[u8], u32) = Materialized::parse(input)?;
        let filter = flag
            .try_into()
            .map_err(|e: UnknownLightType| nom::Err::Failure(e.into()))?;
        Ok((input, filter))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        let flag: u32 = (*self).into();
        flag.encode(output)
    }
}

// Light {
//     uint32 inclusiveSize
//     Node node
//     uint32 type // 0: omni light
//                 // 1: directional light
//                 // 2: ambient light
//     float attenuationStart
//     float attenuationEnd
//     float[3] color
//     float intensity
//     float[3] ambientColor
//     float ambientIntensity
//     (KLAS)
//     (KLAE)
//     (KLAC)
//     (KLAI)
//     (KLBI)
//     (KLBC)
//     (KLAV)
//   }
// KLAS: float attenuationStart
// KLAE: float attenuationStartEnd
// KLAC: float[3] color
// KLAI: float intensity
// KLBI: float ambientIntensity
// KLBC: float[3] ambientColor
// KLAV: float visibility

#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Light {
    pub node: Node,
    pub light_type: LightType,
    pub attenuation_start: f32,
    pub attenuation_end: f32,
    pub color: [f32; 3],
    pub intensity: f32,
    pub ambient_color: [f32; 3],
    pub ambient_intensity: f32,
    pub klas: Option<Klas>,
    pub klae: Option<Klae>,
    pub klac: Option<Klac>,
    pub klai: Option<Klai>,
    pub klbi: Option<Klbi>,
    pub klbc: Option<Klbc>,
    pub klav: Option<Klav>,
    pub ordered: Option<Vec<Tag>>,
}

impl Materialized for Light {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        parse_inclusive_sized(|input| {
            let (input, node) = context("node", Materialized::parse)(input)?;
            let (input, light_type) = context("light_type", Materialized::parse)(input)?;
            let (input, attenuation_start) =
                context("attenuation_start", Materialized::parse)(input)?;
            let (input, attenuation_end) = context("attenuation_end", Materialized::parse)(input)?;
            let (input, color) = context("color", Materialized::parse)(input)?;
            let (input, intensity) = context("intensity", Materialized::parse)(input)?;
            let (input, ambient_color) = context("ambient_color", Materialized::parse)(input)?;
            let (input, ambient_intensity) =
                context("ambient_intensity", Materialized::parse)(input)?;
            let mut klas: Option<Klas> = None;
            let mut klae: Option<Klae> = None;
            let mut klac: Option<Klac> = None;
            let mut klai: Option<Klai> = None;
            let mut klbi: Option<Klbi> = None;
            let mut klbc: Option<Klbc> = None;
            let mut klav: Option<Klav> = None;
            let mut ordered = vec![];
            let (input, _) = parse_tagged(|tag, input| {
                if tag == Klas::tag() {
                    let (input, chunk) = context("KLAS chunk", Materialized::parse)(input)?;
                    klas = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Klae::tag() {
                    let (input, chunk) = context("KLAE chunk", Materialized::parse)(input)?;
                    klae = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Klac::tag() {
                    let (input, chunk) = context("KLAC chunk", Materialized::parse)(input)?;
                    klac = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Klac::tag() {
                    let (input, chunk) = context("KLAC chunk", Materialized::parse)(input)?;
                    klac = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Klai::tag() {
                    let (input, chunk) = context("KLAI chunk", Materialized::parse)(input)?;
                    klai = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Klbi::tag() {
                    let (input, chunk) = context("KLBI chunk", Materialized::parse)(input)?;
                    klbi = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Klbc::tag() {
                    let (input, chunk) = context("KLBC chunk", Materialized::parse)(input)?;
                    klbc = Some(chunk);
                    ordered.push(tag);
                    Ok((input, false))
                } else if tag == Klav::tag() {
                    let (input, chunk) = context("KLAV chunk", Materialized::parse)(input)?;
                    klav = Some(chunk);
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
                Light {
                    node,
                    light_type,
                    attenuation_start,
                    attenuation_end,
                    color,
                    intensity,
                    ambient_color,
                    ambient_intensity,
                    klas,
                    klae,
                    klac,
                    klai,
                    klbi,
                    klbc,
                    klav,
                    ordered: Some(ordered),
                },
            ))
        })(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_inclusive_sized(output, |output| {
            self.node.encode(output)?;
            self.light_type.encode(output)?;
            self.attenuation_start.encode(output)?;
            self.attenuation_end.encode(output)?;
            self.color.encode(output)?;
            self.intensity.encode(output)?;
            self.ambient_color.encode(output)?;
            self.ambient_intensity.encode(output)?;
            if let Some(ordered) = &self.ordered {
                for tag in ordered {
                    if *tag == Klas::tag() {
                        if let Some(chunk) = &self.klas {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Klae::tag() {
                        if let Some(chunk) = &self.klae {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Klac::tag() {
                        if let Some(chunk) = &self.klac {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Klai::tag() {
                        if let Some(chunk) = &self.klai {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Klbi::tag() {
                        if let Some(chunk) = &self.klbi {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Klbc::tag() {
                        if let Some(chunk) = &self.klbc {
                            chunk.encode(output)?;
                        }
                    } else if *tag == Klav::tag() {
                        if let Some(chunk) = &self.klav {
                            chunk.encode(output)?;
                        }
                    } else {
                        warn!("Unknown tag {tag}, skippping it...");
                    }
                }
            } else {
                if let Some(chunk) = &self.klas {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.klae {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.klac {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.klai {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.klbi {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.klbc {
                    chunk.encode(output)?;
                }
                if let Some(chunk) = &self.klav {
                    chunk.encode(output)?;
                }
            }
            Ok(())
        })
    }
}

/// Holds `attenuationStart`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Klas(TrackChunk<f32>);

impl Chunk for Klas {
    fn tag() -> Tag {
        Tag([0x4b, 0x4c, 0x41, 0x53]) // KLAS
    }
}

impl Materialized for Klas {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KLAS track", Materialized::parse)(input)?;
        Ok((input, Klas(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `attenuationEnd`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Klae(TrackChunk<f32>);

impl Chunk for Klae {
    fn tag() -> Tag {
        Tag([0x4b, 0x4c, 0x41, 0x45]) // KLAE
    }
}

impl Materialized for Klae {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KLAE track", Materialized::parse)(input)?;
        Ok((input, Klae(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `color`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Klac(TrackChunk<[f32; 3]>);

impl Chunk for Klac {
    fn tag() -> Tag {
        Tag([0x4b, 0x4c, 0x41, 0x43]) // KLAC
    }
}

impl Materialized for Klac {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KLAC track", Materialized::parse)(input)?;
        Ok((input, Klac(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `intensity`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Klai(TrackChunk<f32>);

impl Chunk for Klai {
    fn tag() -> Tag {
        Tag([0x4b, 0x4c, 0x41, 0x49]) // KLAI
    }
}

impl Materialized for Klai {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KLAI track", Materialized::parse)(input)?;
        Ok((input, Klai(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `ambientIntensity`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Klbi(TrackChunk<f32>);

impl Chunk for Klbi {
    fn tag() -> Tag {
        Tag([0x4b, 0x4c, 0x42, 0x49]) // KLBI
    }
}

impl Materialized for Klbi {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KLBI track", Materialized::parse)(input)?;
        Ok((input, Klbi(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `ambientIntensity`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Klbc(TrackChunk<[f32; 3]>);

impl Chunk for Klbc {
    fn tag() -> Tag {
        Tag([0x4b, 0x4c, 0x42, 0x43]) // KLBC
    }
}

impl Materialized for Klbc {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KLBC track", Materialized::parse)(input)?;
        Ok((input, Klbc(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `visibility`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Klav(TrackChunk<f32>);

impl Chunk for Klav {
    fn tag() -> Tag {
        Tag([0x4b, 0x4c, 0x41, 0x56]) // KLAV
    }
}

impl Materialized for Klav {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KLAV track", Materialized::parse)(input)?;
        Ok((input, Klav(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}
