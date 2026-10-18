---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/src/types/texture.rs"
ref: HEAD
---

use super::chunk::utils::*;
use super::chunk::*;
use super::materialize::*;
use super::tracks::TrackChunk;
use log::*;

/// Amount of bytes one texture record takes
pub const TEXTURE_SIZE: usize = 268;
/// Length of `file_name` field of [Texture]
pub const TEXTURE_FILENAME_LEN: usize = 260;

// Texture {
//     uint32 replaceableId
//     char[260] fileName
//     uint32 flags
//   }
#[derive(Debug, Clone, PartialEq, Eq, PartialOrd, Ord, Hash)]
pub struct Texture {
    pub replaceable_id: u32,
    pub file_name: Literal<TEXTURE_FILENAME_LEN>,
    pub flags: u32,
}

impl Materialized for Texture {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, replaceable_id) = context("replaceable_id", Materialized::parse)(input)?;
        let (input, file_name) = context("file_name", Materialized::parse)(input)?;
        let (input, flags) = context("flags", Materialized::parse)(input)?;
        Ok((
            input,
            Texture {
                replaceable_id,
                file_name,
                flags,
            },
        ))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.replaceable_id.encode(output)?;
        self.file_name.encode(output)?;
        self.flags.encode(output)
    }
}

// TextureAnimation {
//     uint32 inclusiveSize
//     (KTAT)
//     (KTAR)
//     (KTAS)
//   }
// KTAT: float[3] translation
// KTAR: float[4] rotation
// KTAS: float[3] scaling
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct TextureAnimation {
    pub ktat: Option<Ktat>,
    pub ktar: Option<Ktar>,
    pub ktas: Option<Ktas>,
}

impl Materialized for TextureAnimation {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        parse_inclusive_sized(|input| {
            let mut ktat: Option<Ktat> = None;
            let mut ktar: Option<Ktar> = None;
            let mut ktas: Option<Ktas> = None;
            let (input, _) = parse_tagged(|tag, input| {
                if tag == Ktat::tag() {
                    let (input, chunk) = context("KTAT chunk", Materialized::parse)(input)?;
                    ktat = Some(chunk);
                    Ok((input, false))
                } else if tag == Ktar::tag() {
                    let (input, chunk) = context("KTAR chunk", Materialized::parse)(input)?;
                    ktar = Some(chunk);
                    Ok((input, false))
                } else if tag == Ktas::tag() {
                    let (input, chunk) = context("KTAS chunk", Materialized::parse)(input)?;
                    ktas = Some(chunk);
                    Ok((input, false))
                } else {
                    let found: String = format!("{}", tag);
                    error!("Unknown tag {}", found);
                    return Err(nom::Err::Failure(ParseError::UnknownTag(found)));
                }
            })(input)?;
            Ok((input, TextureAnimation { ktat, ktar, ktas }))
        })(input)
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        encode_inclusive_sized(output, |output| {
            if let Some(chunk) = &self.ktat {
                chunk.encode(output)?;
            }
            if let Some(chunk) = &self.ktar {
                chunk.encode(output)?;
            }
            if let Some(chunk) = &self.ktas {
                chunk.encode(output)?;
            }
            Ok(())
        })
    }
}

/// Holds `translation`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Ktat(TrackChunk<[f32; 3]>);

impl Chunk for Ktat {
    fn tag() -> Tag {
        Tag([0x4b, 0x54, 0x41, 0x54]) // KTAT
    }
}

impl Materialized for Ktat {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KTAT track", Materialized::parse)(input)?;
        Ok((input, Ktat(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `rotation`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Ktar(TrackChunk<[f32; 4]>);

impl Chunk for Ktar {
    fn tag() -> Tag {
        Tag([0x4b, 0x54, 0x41, 0x52]) // KTAR
    }
}

impl Materialized for Ktar {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KTAR track", Materialized::parse)(input)?;
        Ok((input, Ktar(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}

/// Holds `scaling`
#[derive(Debug, Clone, PartialEq, PartialOrd)]
pub struct Ktas(TrackChunk<[f32; 3]>);

impl Chunk for Ktas {
    fn tag() -> Tag {
        Tag([0x4b, 0x54, 0x41, 0x53]) // KTAS
    }
}

impl Materialized for Ktas {
    type Version = u32;

    fn parse_versioned(_: Option<Self::Version>, input: &[u8]) -> Parser<Self> {
        let (input, chunk) = context("KTAS track", Materialized::parse)(input)?;
        Ok((input, Ktas(chunk)))
    }

    fn encode(&self, output: &mut Vec<u8>) -> Result<(), EncodeError> {
        self.0.encode(output)
    }
}
